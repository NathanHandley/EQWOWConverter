--  Author: Nathan Handley (nathanhandley@protonmail.com)
--  Copyright (c) 2026 Nathan Handley
--
--  This program is free software: you can redistribute it and/or modify
--  it under the terms of the GNU General Public License as published by
--  the Free Software Foundation, either version 3 of the License, or
--  (at your option) any later version.
--
--  This program is distributed in the hope that it will be useful,
--  but WITHOUT ANY WARRANTY; without even the implied warranty of
--  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
--  GNU General Public License for more details.
--
--  You should have received a copy of the GNU General Public License
--  along with this program.  If not, see <http://www.gnu.org/licenses/>.

-- Adds three tabs down the right hand edge of the trade skill window -- Both, EQ (Norrath) and WoW
-- (Azeroth) -- which filter the recipe list down to the world a recipe came from.  Every EQ profession
-- is merged into a WoW one (EQ Baking, Brewing and Fishing all land in Cooking, and so on), so a
-- maxed profession lists both worlds' recipes interleaved under the same subclass headers.
--
-- The recipe list is built entirely inside the client from the player's known spells, so there is no
-- server side filtering to ask for.  Instead the trade skill API functions the default UI calls are
-- replaced with wrappers that hide the rows of the unwanted world: the wrapper for GetNumTradeSkills
-- reports the filtered count, and every function that takes a row index translates the index the UI is
-- working with back to the real one before handing it to the client.  Blizzard_TradeSkillUI is then
-- left completely untouched -- it draws, selects, links and crafts out of the shorter list without
-- knowing anything changed.
--
-- Notes on that approach:
--   * The wrappers are only in place while a filter is actually on.  Picking "Both" puts every original
--     function back, so the default trade skill window runs exactly as shipped (and carries none of this
--     addon's taint) unless the player asked for a filter.  Nothing the trade skill window reaches is a
--     protected function -- DoTradeSkill is not one -- so the taint the wrappers do carry is harmless.
--   * A row's world is decided by the recipe's spell ID first (the converter gives every EQ recipe an ID
--     at or above EQ_TradeskillFilterData.SPELL_ID_MIN) and by the produced item's ID second.  A row
--     that answers neither is left visible under every setting rather than guessed at.
--   * Headers are kept when at least one of their recipes survives the filter.  A collapsed header is
--     always kept, since the client does not list what is inside one.
--   * The filtered list is rebuilt whenever the real list could have changed.  Its own event does not
--     order reliably against the trade skill window's, so instead of trusting an event the map checks
--     the real row count and profession name on every call and rebuilds when either moved.

local MODE_BOTH = 0;
local MODE_NORRATH = 1;
local MODE_AZEROTH = 2;

local REALM_NORRATH = 1;
local REALM_AZEROTH = 2;

-- Ranges written by the converter (EQTradeskillFilterLUA) into EQ_TradeskillFilterData.lua
local ITEM_ID_MIN = 0;
local ITEM_ID_MAX = 0;
local SPELL_ID_MIN = 0;
if ( type(EQ_TradeskillFilterData) == "table" ) then
	ITEM_ID_MIN = tonumber(EQ_TradeskillFilterData.ITEM_ID_MIN) or 0;
	ITEM_ID_MAX = tonumber(EQ_TradeskillFilterData.ITEM_ID_MAX) or 0;
	SPELL_ID_MIN = tonumber(EQ_TradeskillFilterData.SPELL_ID_MIN) or 0;
end

-- Without usable ranges every recipe would be called Azeroth, so the filter switches itself off instead
local rangesUsable = (ITEM_ID_MIN > 0 and ITEM_ID_MAX > ITEM_ID_MIN and SPELL_ID_MIN > 0);

local currentMode = MODE_BOTH;
local wrappersInstalled = false;

local displayToReal = {};       -- [row the UI is drawing] = real row index
local realToDisplay = {};       -- the way back, for the selected row
local mapValid = false;
local builtRealCount = -1;
local builtLineName = nil;

local tabs = {};

-- ===================================================================================
-- The untouched client functions
-- ===================================================================================

local orig =
{
	GetNumTradeSkills = GetNumTradeSkills,
	GetTradeSkillInfo = GetTradeSkillInfo,
	GetTradeSkillIcon = GetTradeSkillIcon,
	GetTradeSkillNumMade = GetTradeSkillNumMade,
	GetTradeSkillNumReagents = GetTradeSkillNumReagents,
	GetTradeSkillReagentInfo = GetTradeSkillReagentInfo,
	GetTradeSkillReagentItemLink = GetTradeSkillReagentItemLink,
	GetTradeSkillItemLink = GetTradeSkillItemLink,
	GetTradeSkillRecipeLink = GetTradeSkillRecipeLink,
	GetTradeSkillDescription = GetTradeSkillDescription,
	GetTradeSkillTools = GetTradeSkillTools,
	GetTradeSkillCooldown = GetTradeSkillCooldown,
	GetTradeSkillSelectionIndex = GetTradeSkillSelectionIndex,
	GetFirstTradeSkill = GetFirstTradeSkill,
	SelectTradeSkill = SelectTradeSkill,
	DoTradeSkill = DoTradeSkill,
	ExpandTradeSkillSubClass = ExpandTradeSkillSubClass,
	CollapseTradeSkillSubClass = CollapseTradeSkillSubClass,
	SetTradeSkillSubClassFilter = SetTradeSkillSubClassFilter,
	SetTradeSkillInvSlotFilter = SetTradeSkillInvSlotFilter,
	SetTradeSkillItemNameFilter = SetTradeSkillItemNameFilter,
	SetTradeSkillItemLevelFilter = SetTradeSkillItemLevelFilter,
	TradeSkillOnlyShowMakeable = TradeSkillOnlyShowMakeable,
	GameTooltipSetTradeSkillItem = GameTooltip.SetTradeSkillItem,
};

-- ===================================================================================
-- Which world a row came from
-- ===================================================================================

local function EQ_TradeskillFilter_GetRowRealm(realIndex)
	-- The recipe's own spell ID is the reliable answer, since the converter allocates EQ recipe spells
	-- out of a block that starts above every spell the WoW client shipped with
	local recipeLink = orig.GetTradeSkillRecipeLink(realIndex);
	if ( recipeLink ) then
		local recipeSpellID = tonumber(string.match(recipeLink, "enchant:(%d+)"));
		if ( recipeSpellID ) then
			if ( recipeSpellID >= SPELL_ID_MIN ) then
				return REALM_NORRATH;
			end
			return REALM_AZEROTH;
		end
	end

	-- Otherwise fall back on what the recipe makes
	local producedLink = orig.GetTradeSkillItemLink(realIndex);
	if ( producedLink ) then
		local producedItemID = tonumber(string.match(producedLink, "item:(%d+)"));
		if ( producedItemID ) then
			if ( producedItemID >= ITEM_ID_MIN and producedItemID <= ITEM_ID_MAX ) then
				return REALM_NORRATH;
			end
			return REALM_AZEROTH;
		end
		-- Enchanting recipes produce an enchant rather than an item
		local producedEnchantID = tonumber(string.match(producedLink, "enchant:(%d+)"));
		if ( producedEnchantID ) then
			if ( producedEnchantID >= SPELL_ID_MIN ) then
				return REALM_NORRATH;
			end
			return REALM_AZEROTH;
		end
	end

	-- Nothing to go on, so leave it visible whatever the filter is set to
	return nil;
end

local function EQ_TradeskillFilter_IsRealmWanted(realm)
	if ( realm == nil or currentMode == MODE_BOTH ) then
		return true;
	end
	if ( currentMode == MODE_NORRATH ) then
		return (realm == REALM_NORRATH);
	end
	return (realm == REALM_AZEROTH);
end

-- ===================================================================================
-- The filtered list
-- ===================================================================================

local function EQ_TradeskillFilter_AddRow(realIndex)
	table.insert(displayToReal, realIndex);
	realToDisplay[realIndex] = #displayToReal;
end

local function EQ_TradeskillFilter_RebuildMap(realCount, lineName)
	wipe(displayToReal);
	wipe(realToDisplay);

	-- An expanded header is held back until one of its recipes gets through, so headers that end up
	-- empty do not show as a heading with nothing under it
	local pendingHeaderRealIndex = nil;
	for realIndex = 1, realCount, 1 do
		local skillName, skillType, _, isExpanded = orig.GetTradeSkillInfo(realIndex);
		if ( skillType == "header" ) then
			if ( isExpanded ) then
				pendingHeaderRealIndex = realIndex;
			else
				pendingHeaderRealIndex = nil;
				EQ_TradeskillFilter_AddRow(realIndex);
			end
		elseif ( skillName ) then
			if ( EQ_TradeskillFilter_IsRealmWanted(EQ_TradeskillFilter_GetRowRealm(realIndex)) ) then
				if ( pendingHeaderRealIndex ) then
					EQ_TradeskillFilter_AddRow(pendingHeaderRealIndex);
					pendingHeaderRealIndex = nil;
				end
				EQ_TradeskillFilter_AddRow(realIndex);
			end
		end
	end

	builtRealCount = realCount;
	builtLineName = lineName;
	mapValid = true;
end

local function EQ_TradeskillFilter_EnsureMap()
	if ( currentMode == MODE_BOTH ) then
		return;
	end
	local realCount = orig.GetNumTradeSkills();
	local lineName = GetTradeSkillLine();
	if ( mapValid == true and realCount == builtRealCount and lineName == builtLineName ) then
		return;
	end
	EQ_TradeskillFilter_RebuildMap(realCount, lineName);
end

-- Row index the UI is working with -> row index the client knows
local function EQ_TradeskillFilter_ToReal(displayIndex)
	EQ_TradeskillFilter_EnsureMap();
	if ( currentMode == MODE_BOTH ) then
		return displayIndex;
	end
	if ( displayIndex == nil ) then
		return 0;
	end
	return displayToReal[displayIndex] or 0;
end

-- ===================================================================================
-- The wrappers
-- ===================================================================================

local function EQ_TradeskillFilter_GetNumTradeSkills()
	EQ_TradeskillFilter_EnsureMap();
	return #displayToReal;
end

local function EQ_TradeskillFilter_GetTradeSkillInfo(index)
	return orig.GetTradeSkillInfo(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillIcon(index)
	return orig.GetTradeSkillIcon(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillNumMade(index)
	return orig.GetTradeSkillNumMade(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillNumReagents(index)
	return orig.GetTradeSkillNumReagents(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillReagentInfo(index, reagentIndex)
	return orig.GetTradeSkillReagentInfo(EQ_TradeskillFilter_ToReal(index), reagentIndex);
end

local function EQ_TradeskillFilter_GetTradeSkillReagentItemLink(index, reagentIndex)
	return orig.GetTradeSkillReagentItemLink(EQ_TradeskillFilter_ToReal(index), reagentIndex);
end

local function EQ_TradeskillFilter_GetTradeSkillItemLink(index)
	return orig.GetTradeSkillItemLink(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillRecipeLink(index)
	return orig.GetTradeSkillRecipeLink(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillDescription(index)
	return orig.GetTradeSkillDescription(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillTools(index)
	return orig.GetTradeSkillTools(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_GetTradeSkillCooldown(index)
	return orig.GetTradeSkillCooldown(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_SelectTradeSkill(index)
	return orig.SelectTradeSkill(EQ_TradeskillFilter_ToReal(index));
end

local function EQ_TradeskillFilter_DoTradeSkill(index, repeatCount)
	return orig.DoTradeSkill(EQ_TradeskillFilter_ToReal(index), repeatCount);
end

local function EQ_TradeskillFilter_GetTradeSkillSelectionIndex()
	EQ_TradeskillFilter_EnsureMap();
	local realIndex = orig.GetTradeSkillSelectionIndex();
	if ( realIndex == nil or realIndex == 0 ) then
		return 0;
	end
	-- 0 when the selected recipe is one the filter is hiding, which reads as "nothing selected"
	return realToDisplay[realIndex] or 0;
end

local function EQ_TradeskillFilter_GetFirstTradeSkill()
	EQ_TradeskillFilter_EnsureMap();
	for displayIndex = 1, #displayToReal, 1 do
		local _, skillType = orig.GetTradeSkillInfo(displayToReal[displayIndex]);
		if ( skillType ~= "header" ) then
			return displayIndex;
		end
	end
	return 0;
end

-- Expanding or collapsing changes which rows the client lists, so the map has to be thrown away.  The
-- index is translated first, while the map still describes the list the click was made against.  An
-- index of 0 means "every header" and is passed straight through
local function EQ_TradeskillFilter_ExpandTradeSkillSubClass(index)
	local realIndex = index;
	if ( index ~= nil and index ~= 0 ) then
		realIndex = EQ_TradeskillFilter_ToReal(index);
	end
	mapValid = false;
	return orig.ExpandTradeSkillSubClass(realIndex);
end

local function EQ_TradeskillFilter_CollapseTradeSkillSubClass(index)
	local realIndex = index;
	if ( index ~= nil and index ~= 0 ) then
		realIndex = EQ_TradeskillFilter_ToReal(index);
	end
	mapValid = false;
	return orig.CollapseTradeSkillSubClass(realIndex);
end

-- The client's own filters rewrite the list under us
local function EQ_TradeskillFilter_SetTradeSkillSubClassFilter(index, checkNoSubClass, checkNoInvSlot)
	mapValid = false;
	return orig.SetTradeSkillSubClassFilter(index, checkNoSubClass, checkNoInvSlot);
end

local function EQ_TradeskillFilter_SetTradeSkillInvSlotFilter(index, checkNoSubClass, checkNoInvSlot)
	mapValid = false;
	return orig.SetTradeSkillInvSlotFilter(index, checkNoSubClass, checkNoInvSlot);
end

local function EQ_TradeskillFilter_SetTradeSkillItemNameFilter(name)
	mapValid = false;
	return orig.SetTradeSkillItemNameFilter(name);
end

local function EQ_TradeskillFilter_SetTradeSkillItemLevelFilter(minLevel, maxLevel)
	mapValid = false;
	return orig.SetTradeSkillItemLevelFilter(minLevel, maxLevel);
end

local function EQ_TradeskillFilter_TradeSkillOnlyShowMakeable(onlyShowMakeable)
	mapValid = false;
	return orig.TradeSkillOnlyShowMakeable(onlyShowMakeable);
end

local function EQ_TradeskillFilter_GameTooltipSetTradeSkillItem(self, index, reagentIndex)
	return orig.GameTooltipSetTradeSkillItem(self, EQ_TradeskillFilter_ToReal(index), reagentIndex);
end

local function EQ_TradeskillFilter_InstallWrappers()
	if ( wrappersInstalled == true ) then
		return;
	end
	GetNumTradeSkills = EQ_TradeskillFilter_GetNumTradeSkills;
	GetTradeSkillInfo = EQ_TradeskillFilter_GetTradeSkillInfo;
	GetTradeSkillIcon = EQ_TradeskillFilter_GetTradeSkillIcon;
	GetTradeSkillNumMade = EQ_TradeskillFilter_GetTradeSkillNumMade;
	GetTradeSkillNumReagents = EQ_TradeskillFilter_GetTradeSkillNumReagents;
	GetTradeSkillReagentInfo = EQ_TradeskillFilter_GetTradeSkillReagentInfo;
	GetTradeSkillReagentItemLink = EQ_TradeskillFilter_GetTradeSkillReagentItemLink;
	GetTradeSkillItemLink = EQ_TradeskillFilter_GetTradeSkillItemLink;
	GetTradeSkillRecipeLink = EQ_TradeskillFilter_GetTradeSkillRecipeLink;
	GetTradeSkillDescription = EQ_TradeskillFilter_GetTradeSkillDescription;
	GetTradeSkillTools = EQ_TradeskillFilter_GetTradeSkillTools;
	GetTradeSkillCooldown = EQ_TradeskillFilter_GetTradeSkillCooldown;
	GetTradeSkillSelectionIndex = EQ_TradeskillFilter_GetTradeSkillSelectionIndex;
	GetFirstTradeSkill = EQ_TradeskillFilter_GetFirstTradeSkill;
	SelectTradeSkill = EQ_TradeskillFilter_SelectTradeSkill;
	DoTradeSkill = EQ_TradeskillFilter_DoTradeSkill;
	ExpandTradeSkillSubClass = EQ_TradeskillFilter_ExpandTradeSkillSubClass;
	CollapseTradeSkillSubClass = EQ_TradeskillFilter_CollapseTradeSkillSubClass;
	SetTradeSkillSubClassFilter = EQ_TradeskillFilter_SetTradeSkillSubClassFilter;
	SetTradeSkillInvSlotFilter = EQ_TradeskillFilter_SetTradeSkillInvSlotFilter;
	SetTradeSkillItemNameFilter = EQ_TradeskillFilter_SetTradeSkillItemNameFilter;
	SetTradeSkillItemLevelFilter = EQ_TradeskillFilter_SetTradeSkillItemLevelFilter;
	TradeSkillOnlyShowMakeable = EQ_TradeskillFilter_TradeSkillOnlyShowMakeable;
	GameTooltip.SetTradeSkillItem = EQ_TradeskillFilter_GameTooltipSetTradeSkillItem;
	wrappersInstalled = true;
end

local function EQ_TradeskillFilter_RemoveWrappers()
	if ( wrappersInstalled == false ) then
		return;
	end
	GetNumTradeSkills = orig.GetNumTradeSkills;
	GetTradeSkillInfo = orig.GetTradeSkillInfo;
	GetTradeSkillIcon = orig.GetTradeSkillIcon;
	GetTradeSkillNumMade = orig.GetTradeSkillNumMade;
	GetTradeSkillNumReagents = orig.GetTradeSkillNumReagents;
	GetTradeSkillReagentInfo = orig.GetTradeSkillReagentInfo;
	GetTradeSkillReagentItemLink = orig.GetTradeSkillReagentItemLink;
	GetTradeSkillItemLink = orig.GetTradeSkillItemLink;
	GetTradeSkillRecipeLink = orig.GetTradeSkillRecipeLink;
	GetTradeSkillDescription = orig.GetTradeSkillDescription;
	GetTradeSkillTools = orig.GetTradeSkillTools;
	GetTradeSkillCooldown = orig.GetTradeSkillCooldown;
	GetTradeSkillSelectionIndex = orig.GetTradeSkillSelectionIndex;
	GetFirstTradeSkill = orig.GetFirstTradeSkill;
	SelectTradeSkill = orig.SelectTradeSkill;
	DoTradeSkill = orig.DoTradeSkill;
	ExpandTradeSkillSubClass = orig.ExpandTradeSkillSubClass;
	CollapseTradeSkillSubClass = orig.CollapseTradeSkillSubClass;
	SetTradeSkillSubClassFilter = orig.SetTradeSkillSubClassFilter;
	SetTradeSkillInvSlotFilter = orig.SetTradeSkillInvSlotFilter;
	SetTradeSkillItemNameFilter = orig.SetTradeSkillItemNameFilter;
	SetTradeSkillItemLevelFilter = orig.SetTradeSkillItemLevelFilter;
	TradeSkillOnlyShowMakeable = orig.TradeSkillOnlyShowMakeable;
	GameTooltip.SetTradeSkillItem = orig.GameTooltipSetTradeSkillItem;
	wrappersInstalled = false;
end

-- ===================================================================================
-- The tabs
-- ===================================================================================

local TAB_DEFINITIONS =
{
	{ mode = MODE_BOTH, label = "Both", color = "|cffFFFFFF", tooltip = "Recipes from both worlds" },
	{ mode = MODE_NORRATH, label = "EQ", color = "|cff66CCFF", tooltip = "Norrath (EverQuest) recipes only" },
	{ mode = MODE_AZEROTH, label = "WoW", color = "|cffFFC64D", tooltip = "Azeroth (World of Warcraft) recipes only" },
};

local EQ_TradeskillFilter_UpdateTabs;

local function EQ_TradeskillFilter_RefreshTradeSkillFrame()
	if ( TradeSkillFrame == nil or TradeSkillFrame:IsShown() == false ) then
		return;
	end
	FauxScrollFrame_SetOffset(TradeSkillListScrollFrame, 0);
	TradeSkillListScrollFrameScrollBar:SetValue(0);
	-- Whatever was selected may be hidden now, so start from the top of what is left.  A list the filter
	-- emptied gets no selection at all, the same as the default UI does when nothing is makeable
	if ( GetNumTradeSkills() > 0 ) then
		local firstSkillIndex = GetFirstTradeSkill();
		if ( firstSkillIndex and firstSkillIndex > 0 ) then
			TradeSkillFrame_SetSelection(firstSkillIndex);
		end
	end
	TradeSkillFrame_Update();
end

local function EQ_TradeskillFilter_SetMode(newMode)
	if ( newMode == currentMode ) then
		return;
	end
	currentMode = newMode;
	mapValid = false;
	if ( type(EQ_TradeskillFilterDB) ~= "table" ) then
		EQ_TradeskillFilterDB = {};
	end
	EQ_TradeskillFilterDB.mode = newMode;

	if ( newMode == MODE_BOTH ) then
		EQ_TradeskillFilter_RemoveWrappers();
	else
		EQ_TradeskillFilter_InstallWrappers();
	end
	EQ_TradeskillFilter_UpdateTabs();
	EQ_TradeskillFilter_RefreshTradeSkillFrame();
end

function EQ_TradeskillFilter_UpdateTabs()
	for tabIndex = 1, #tabs, 1 do
		tabs[tabIndex]:SetChecked(TAB_DEFINITIONS[tabIndex].mode == currentMode);
	end
end

local function EQ_TradeskillFilter_Tab_OnClick(self)
	-- Clicking the tab that is already on would otherwise untick it and leave no filter shown
	EQ_TradeskillFilter_SetMode(TAB_DEFINITIONS[self:GetID()].mode);
	EQ_TradeskillFilter_UpdateTabs();
end

local function EQ_TradeskillFilter_AttachToTradeSkillUI()
	if ( rangesUsable == false or TradeSkillFrame == nil or #tabs > 0 ) then
		return;
	end

	for tabIndex = 1, #TAB_DEFINITIONS, 1 do
		local tabDefinition = TAB_DEFINITIONS[tabIndex];
		local tab = CreateFrame("CheckButton", "EQTradeskillFilterTab" .. tabIndex, TradeSkillFrame, "SpellBookSkillLineTabTemplate");
		tab:SetID(tabIndex);
		tab.tooltip = tabDefinition.tooltip;
		tab:SetScript("OnClick", EQ_TradeskillFilter_Tab_OnClick);

		-- The frame's art stops 34 pixels short of its right edge (its own hit rect says so), so the tabs
		-- sit in that gap and read as part of the window, the way the spellbook's skill line tabs do
		if ( tabIndex == 1 ) then
			tab:SetPoint("TOPLEFT", TradeSkillFrame, "TOPRIGHT", -33, -65);
		else
			tab:SetPoint("TOPLEFT", tabs[tabIndex - 1], "BOTTOMLEFT", 0, -17);
		end

		-- The template leaves the icon empty for the caller to fill in.  A plain dark plate is used here
		-- instead, so the label sits on something rather than on the window art showing through
		tab:SetNormalTexture("Interface\\Buttons\\WHITE8X8");
		local plateTexture = tab:GetNormalTexture();
		if ( plateTexture ) then
			plateTexture:SetVertexColor(0.08, 0.08, 0.08, 0.9);
		end

		local labelText = tab:CreateFontString(nil, "OVERLAY", "GameFontNormalSmall");
		labelText:SetPoint("CENTER", tab, "CENTER", 0, 0);
		labelText:SetText(tabDefinition.color .. tabDefinition.label .. "|r");

		tab:Show();
		tabs[tabIndex] = tab;
	end

	EQ_TradeskillFilter_UpdateTabs();
end

-- ===================================================================================
-- Loading
-- ===================================================================================

local function EQ_TradeskillFilter_LoadSavedMode()
	local savedMode = MODE_BOTH;
	if ( type(EQ_TradeskillFilterDB) == "table" ) then
		savedMode = tonumber(EQ_TradeskillFilterDB.mode) or MODE_BOTH;
	else
		EQ_TradeskillFilterDB = {};
	end
	if ( savedMode ~= MODE_NORRATH and savedMode ~= MODE_AZEROTH ) then
		savedMode = MODE_BOTH;
	end
	if ( rangesUsable == false ) then
		savedMode = MODE_BOTH;
	end

	currentMode = savedMode;
	mapValid = false;
	if ( currentMode == MODE_BOTH ) then
		EQ_TradeskillFilter_RemoveWrappers();
	else
		EQ_TradeskillFilter_InstallWrappers();
	end
end

local function EQ_TradeskillFilter_OnEvent(self, event, arg1)
	if ( event == "ADDON_LOADED" ) then
		if ( arg1 == "EQ_TradeskillFilter" ) then
			EQ_TradeskillFilter_LoadSavedMode();
		elseif ( arg1 == "Blizzard_TradeSkillUI" ) then
			EQ_TradeskillFilter_AttachToTradeSkillUI();
			EQ_TradeskillFilter_UpdateTabs();
		end
		return;
	end
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		-- Covers the trade skill window already being loaded, which happens on a UI reload
		if ( IsAddOnLoaded("Blizzard_TradeSkillUI") ) then
			EQ_TradeskillFilter_AttachToTradeSkillUI();
			EQ_TradeskillFilter_UpdateTabs();
		end
		return;
	end
	-- The recipe list itself moved (a recipe learned, a profession opened, the client's own filters).
	-- The trade skill window registered for these first, so it has already redrawn off the old map by the
	-- time this runs; a redraw of our own puts the list right.  A list that changed size is caught during
	-- that first redraw anyway, since the row count is checked on every call
	mapValid = false;
	if ( currentMode ~= MODE_BOTH and TradeSkillFrame ~= nil and TradeSkillFrame:IsShown() and TradeSkillFrame_Update ~= nil ) then
		TradeSkillFrame_Update();
	end
end

local eventFrame = CreateFrame("Frame", "EQTradeskillFilterEventFrame");
eventFrame:RegisterEvent("ADDON_LOADED");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:RegisterEvent("TRADE_SKILL_SHOW");
eventFrame:RegisterEvent("TRADE_SKILL_CLOSE");
eventFrame:RegisterEvent("TRADE_SKILL_UPDATE");
eventFrame:RegisterEvent("TRADE_SKILL_FILTER_UPDATE");
eventFrame:RegisterEvent("SKILL_LINES_CHANGED");
eventFrame:SetScript("OnEvent", EQ_TradeskillFilter_OnEvent);

-- ===================================================================================
-- Slash command
-- ===================================================================================

local function EQ_TradeskillFilter_SlashCommand(argument)
	if ( rangesUsable == false ) then
		DEFAULT_CHAT_FRAME:AddMessage("EQ Tradeskill Filter: no EQ ID ranges were deployed to this client, so the filter is off.");
		return;
	end

	local requestedMode = nil;
	argument = string.lower(argument or "");
	if ( argument == "both" or argument == "all" ) then
		requestedMode = MODE_BOTH;
	elseif ( argument == "norrath" or argument == "eq" ) then
		requestedMode = MODE_NORRATH;
	elseif ( argument == "azeroth" or argument == "wow" ) then
		requestedMode = MODE_AZEROTH;
	end

	if ( requestedMode == nil ) then
		DEFAULT_CHAT_FRAME:AddMessage("EQ Tradeskill Filter: use /eqtsfilter both, /eqtsfilter norrath or /eqtsfilter azeroth.");
		return;
	end

	EQ_TradeskillFilter_SetMode(requestedMode);
end

SLASH_EQTRADESKILLFILTER1 = "/eqtsfilter";
SlashCmdList["EQTRADESKILLFILTER"] = EQ_TradeskillFilter_SlashCommand;
