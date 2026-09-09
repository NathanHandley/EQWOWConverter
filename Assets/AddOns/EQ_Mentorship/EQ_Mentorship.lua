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

-- Adds an "EQ Mentorship" submenu to the right click menu on a targeted player, and on your own portrait.
-- The server (mod-everquest) owns the whole system and pushes the current state under this prefix at login
-- and any time it changes; every entry here just runs the ".eqmentorship" command that the server also
-- accepts as plain chat, so the feature still works for anyone playing without this addon.
--
-- The entries are built by hand rather than registered with UnitPopup.  See the note above ENTRY_TEXT: anything an
-- addon puts into UnitPopupButtons / UnitPopupMenus is read back by the client while it builds the stock right click
-- menu, and that alone is enough to make the client refuse "Set Focus" from that menu afterwards.
local EQMENTORSHIP_PREFIX = "EQMENTORSHIP";

local MENU_ROOT = "EQ_MENTORSHIP";
local MENU_MENTOR = "EQ_MENTORSHIP_MENTOR";
local MENU_APPRENTICE = "EQ_MENTORSHIP_APPRENTICE";
local MENU_END = "EQ_MENTORSHIP_END";

-- Matches EQ_MENTORSHIP_ROLE_* in the mod
local ROLE_NONE = 0;
local ROLE_MENTOR = 1;
local ROLE_APPRENTICE = 2;
local ROLE_ANCHOR = 3;

local ACTIVE_COLOR_PREFIX = "|cff4CFF00";
local ACTIVE_COLOR_SUFFIX = "|r";
local REASON_COLOR_PREFIX = "|cffFF2020";
local REASON_COLOR_SUFFIX = "|r";

-- What the server last told us about this character, and the two rules it sends along so an entry the server
-- would only refuse can be greyed out here instead of being clicked and bounced back as an error
local currentRole = ROLE_NONE;
local currentPartnerName = "";
local currentStandingLevel = 0;
local currentRealLevel = 0;
local currentMentorshipEnabled = true;
local currentMinLevelGap = 3;

-- What each entry does, shown as the yellow line under the entry name the same way the stock entries do it
local ROOT_DESCRIPTION = "Tether yourself to another member of your group so the two of you stand at the same level and can adventure together.";
local MENTOR_DESCRIPTION = "Drop to one level above the selected player so the two of you can adventure together. You earn no experience while mentoring, and go back to your own level when it ends.";
local APPRENTICE_DESCRIPTION = "Rise to one level below the selected player and adventure alongside them. Everything they earn is banked while it lasts, and paid out at your own level when it ends.";
local END_DESCRIPTION = "End the mentorship now and go back to your own level.";

-- Filled in whenever a unit menu is built.  A value here is why that entry is greyed out; nil means it is usable
local menuDisabledReasons = {};

-- Captured once at load.  The hooks below run for every dropdown in the game, so nothing in them may depend on a
-- global still being what it is now: one error in there would break every menu in the client, not just this one
local TOOLTIP_BODY_COLOR = NORMAL_FONT_COLOR or { r = 1.0, g = 0.82, b = 0.0 };

-- ===================================================================================
-- Menu entries
-- ===================================================================================

-- Nothing here is registered in UnitPopupButtons or UnitPopupMenus, and it has to stay that way.  Anything an addon
-- writes into those two tables is read straight back out by UnitPopup_HideButtons and UnitPopup_ShowMenu while the
-- client is building the stock right click menu, and in 3.3.5 reading an addon written value marks that whole build
-- as belonging to the addon.  Every button it goes on to make carries the mark, so clicking one that runs a
-- protected function is refused with "EQ_Mentorship has been blocked from an action only available to the Blizzard
-- UI".  "Set Focus" is exactly such an entry -- it calls FocusUnit -- which is why the root entry below is added
-- only after the stock menu has finished building, and why the submenu is opened by hand further down rather than
-- by asking for info.hasArrow
local ENTRY_TEXT = {
	[MENU_ROOT] = "EQ Mentorship",
	[MENU_MENTOR] = "Mentor Them",
	[MENU_APPRENTICE] = "Apprentice To Them",
	[MENU_END] = "End Mentorship",
};

-- The order the submenu lists them in
local SUBMENU_ENTRY_VALUES = { MENU_MENTOR, MENU_APPRENTICE, MENU_END };

-- The unit menus the entry is offered on: another player, a party or raid member, and your own portrait
local OFFERED_ON_MENUS = {
	["PLAYER"] = true,
	["PARTY"] = true,
	["RAID_PLAYER"] = true,
	["SELF"] = true,
};

-- One table, refilled for each entry and handed to UIDropDownMenu_AddButton.  UIDropDownMenu_CreateInfo would do the
-- same job, but the table it hands back is shared with every other addon in the game and this one is not
local entryInfo = {};
local function EQ_Mentorship_BuildEntryInfo()
	for key in pairs(entryInfo) do
		entryInfo[key] = nil;
	end
	return entryInfo;
end

-- The root entry is the only thing that changes shape, picking up the current role so an active tether can be
-- seen without opening the submenu
function EQ_Mentorship_RefreshMenuEntries()
	if ( currentRole == ROLE_MENTOR ) then
		ENTRY_TEXT[MENU_ROOT] = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Mentor)" .. ACTIVE_COLOR_SUFFIX;
	elseif ( currentRole == ROLE_APPRENTICE ) then
		ENTRY_TEXT[MENU_ROOT] = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Apprentice)" .. ACTIVE_COLOR_SUFFIX;
	elseif ( currentRole == ROLE_ANCHOR ) then
		ENTRY_TEXT[MENU_ROOT] = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Tethered)" .. ACTIVE_COLOR_SUFFIX;
	else
		ENTRY_TEXT[MENU_ROOT] = "EQ Mentorship";
	end
end

-- The dropdown that is being clicked through knows who it was opened on, which is the only place the target's
-- name can be read from once the menu is up
local function EQ_Mentorship_GetMenuUnit()
	local dropdownMenu = UIDROPDOWNMENU_INIT_MENU;
	if ( type(dropdownMenu) == "string" ) then
		dropdownMenu = getglobal(dropdownMenu);
	end
	if ( dropdownMenu == nil ) then
		return nil, nil;
	end

	local unit = dropdownMenu.unit;
	local name = dropdownMenu.name;
	if ( (name == nil or name == "") and unit ~= nil ) then
		name = UnitName(unit);
	end
	if ( name == nil or name == "" ) then
		return unit, nil;
	end

	-- Names can arrive with a realm attached, and the server only wants the character part
	local shortName = string.match(name, "^([^%-]+)");
	if ( shortName ~= nil and shortName ~= "" ) then
		name = shortName;
	end
	return unit, name;
end

local function EQ_Mentorship_GetMenuUnitName()
	local unit, name = EQ_Mentorship_GetMenuUnit();
	return name;
end

-- ===================================================================================
-- Eligibility
-- ===================================================================================

-- Walks your own group and looks for them in it.  UnitInParty and UnitInRaid are not used: what they answer for a
-- player who is in somebody else's group is not something to rely on, and the question here is strictly whether
-- this person is in the group the server would find them in
local function EQ_Mentorship_MatchesGroupUnit(unit, name, groupUnit)
	if ( UnitExists(groupUnit) == nil or UnitExists(groupUnit) == false ) then
		return false;
	end
	if ( unit ~= nil and UnitIsUnit(unit, groupUnit) ) then
		return true;
	end
	if ( name ~= nil and name ~= "" and name == UnitName(groupUnit) ) then
		return true;
	end
	return false;
end

local function EQ_Mentorship_IsGroupedWith(unit, name)
	local raidMemberCount = GetNumRaidMembers();
	if ( raidMemberCount > 0 ) then
		for index = 1, raidMemberCount do
			if ( EQ_Mentorship_MatchesGroupUnit(unit, name, "raid" .. index) == true ) then
				return true;
			end
		end
		return false;
	end

	local partyMemberCount = GetNumPartyMembers();
	for index = 1, partyMemberCount do
		if ( EQ_Mentorship_MatchesGroupUnit(unit, name, "party" .. index) == true ) then
			return true;
		end
	end
	return false;
end

-- Returns nil rather than a guess when the level is not known, so the entry is left usable and the server has
-- the final say, instead of the menu refusing something that would actually have worked
local function EQ_Mentorship_GetKnownLevel(unit, name)
	local level;
	if ( unit ~= nil ) then
		level = UnitLevel(unit);
	end
	if ( (level == nil or level <= 0) and name ~= nil and name ~= "" ) then
		level = UnitLevel(name);
	end
	if ( level == nil or level <= 0 ) then
		return nil;
	end
	return level;
end

local function EQ_Mentorship_IsSelf(unit, name)
	if ( unit ~= nil and UnitIsUnit("player", unit) ) then
		return true;
	end
	if ( name ~= nil and name == UnitName("player") ) then
		return true;
	end
	return false;
end

-- The yellow line under the entry name: what it does, what it needs, and when it is greyed out, why.  Built on the
-- spot every time an entry is hovered rather than stored, so it can never be left behind by a state change
function EQ_Mentorship_BuildEntryTooltipText(menuValue)
	local description = nil;
	local requirement = nil;
	if ( menuValue == MENU_ROOT ) then
		description = ROOT_DESCRIPTION;
	elseif ( menuValue == MENU_MENTOR ) then
		description = MENTOR_DESCRIPTION;
		requirement = "Requires being in a group with them, and being at least " .. tostring(currentMinLevelGap) .. " levels above them.";
	elseif ( menuValue == MENU_APPRENTICE ) then
		description = APPRENTICE_DESCRIPTION;
		requirement = "Requires being in a group with them, and them being at least " .. tostring(currentMinLevelGap) .. " levels above you.";
	elseif ( menuValue == MENU_END ) then
		description = END_DESCRIPTION;
	else
		return nil;
	end

	local tooltipText = description;
	if ( requirement ~= nil ) then
		tooltipText = tooltipText .. "|n|n" .. requirement;
	end
	local reason = menuDisabledReasons[menuValue];
	if ( reason ~= nil ) then
		tooltipText = tooltipText .. "|n|n" .. REASON_COLOR_PREFIX .. reason .. REASON_COLOR_SUFFIX;
	end
	return tooltipText;
end

-- Worked out fresh every time a unit menu is built, for whoever it was opened on.  Mirrors what the server checks
-- in RequestMentorshipForPlayer, minus the things only the server can see (the other player's own tether, combat,
-- being alive, battlegrounds), which it still refuses on its own
function EQ_Mentorship_UpdateMenuEligibility()
	local unit, name = EQ_Mentorship_GetMenuUnit();
	local displayName = name;
	if ( displayName == nil or displayName == "" ) then
		displayName = "that player";
	end

	local mentorReason = nil;
	local apprenticeReason = nil;
	local endReason = nil;

	if ( currentMentorshipEnabled == false ) then
		mentorReason = "Mentorship is turned off on this server.";
		apprenticeReason = mentorReason;
		endReason = mentorReason;
	else
		if ( currentRole == ROLE_NONE ) then
			endReason = "You are not in a mentorship.";
		end

		if ( currentRole == ROLE_MENTOR or currentRole == ROLE_APPRENTICE ) then
			mentorReason = "You are already in a mentorship with " .. currentPartnerName .. ". End that one first.";
			apprenticeReason = mentorReason;
		elseif ( currentRole == ROLE_ANCHOR ) then
			mentorReason = currentPartnerName .. " is already tethered to you. That has to end first.";
			apprenticeReason = mentorReason;
		elseif ( name == nil or name == "" or EQ_Mentorship_IsSelf(unit, name) == true ) then
			mentorReason = "Pick another character in your group to tether with.";
			apprenticeReason = mentorReason;
		elseif ( EQ_Mentorship_IsGroupedWith(unit, name) == false ) then
			mentorReason = "You have to be in a group with " .. displayName .. " first.";
			apprenticeReason = mentorReason;
		else
			local playerLevel = UnitLevel("player");
			local targetLevel = EQ_Mentorship_GetKnownLevel(unit, name);
			if ( targetLevel ~= nil and playerLevel ~= nil and playerLevel > 0 ) then
				if ( playerLevel < targetLevel + currentMinLevelGap ) then
					mentorReason = "You are level " .. tostring(playerLevel) .. ", and have to be at least level " .. tostring(targetLevel + currentMinLevelGap) .. " to mentor " .. displayName .. ".";
				end
				if ( targetLevel < playerLevel + currentMinLevelGap ) then
					apprenticeReason = displayName .. " is level " .. tostring(targetLevel) .. ", and has to be at least level " .. tostring(playerLevel + currentMinLevelGap) .. " to take you on as an apprentice.";
				end
			end
		end
	end

	menuDisabledReasons[MENU_MENTOR] = mentorReason;
	menuDisabledReasons[MENU_APPRENTICE] = apprenticeReason;
	menuDisabledReasons[MENU_END] = endReason;
end

-- The stock dropdown button only ever shows its tooltip through GameTooltip_AddNewbieTip, which draws nothing at
-- all unless the player has Enhanced Tooltips switched on.  These entries have to explain themselves either way,
-- so the tooltip is drawn here instead, laid out exactly the way the stock one would have been
function EQ_Mentorship_ShowEntryTooltip(button)
	if ( button == nil or button.value == nil ) then
		return;
	end
	local tooltipText = EQ_Mentorship_BuildEntryTooltipText(button.value);
	if ( tooltipText == nil ) then
		return;
	end
	local title = button.tooltipTitle;
	if ( title == nil ) then
		title = ENTRY_TEXT[button.value];
	end
	if ( title == nil ) then
		title = "EQ Mentorship";
	end
	GameTooltip_SetDefaultAnchor(GameTooltip, button);
	GameTooltip:SetText(title, 1.0, 1.0, 1.0);
	GameTooltip:AddLine(tooltipText, TOOLTIP_BODY_COLOR.r, TOOLTIP_BODY_COLOR.g, TOOLTIP_BODY_COLOR.b, 1);
	GameTooltip:Show();
end

-- The root entry has no info.hasArrow, so the client does not open anything when it is hovered; this does it
-- instead, and then draws the tooltip the same way as for any other entry
function EQ_Mentorship_OnEntryEnter(button)
	if ( button == nil or button.value == nil ) then
		return;
	end
	if ( button.value == MENU_ROOT ) then
		EQ_Mentorship_ShowSubMenu(button);
	end
	EQ_Mentorship_ShowEntryTooltip(button);
end

function EQ_Mentorship_ShowEntryTooltipFromInvisibleButton(invisibleButton)
	if ( invisibleButton == nil ) then
		return;
	end
	EQ_Mentorship_ShowEntryTooltip(invisibleButton:GetParent());
end

-- The dropdown buttons are shared by every menu in the game and live for the whole session, so each one is hooked
-- at most once and the handlers do nothing unless the button is currently showing one of these entries
local hookedTooltipButtons = {};
local function EQ_Mentorship_HookEntryTooltip(button)
	local buttonName = button:GetName();
	if ( buttonName == nil or hookedTooltipButtons[buttonName] ~= nil ) then
		return;
	end
	hookedTooltipButtons[buttonName] = true;
	button:HookScript("OnEnter", EQ_Mentorship_OnEntryEnter);
	local invisibleButton = _G[buttonName .. "InvisibleButton"];
	if ( invisibleButton ~= nil ) then
		invisibleButton:HookScript("OnEnter", EQ_Mentorship_ShowEntryTooltipFromInvisibleButton);
	end
end

-- The stock expand arrow is a button in its own right, pinned to the right hand edge of the entry, and its OnEnter
-- opens the submenu the client's own way: ToggleDropDownMenu, reading this addon's value straight back out of the
-- entry.  For an entry carrying no info.hasArrow that call finds the level two list anchored to something other than
-- the arrow, hides it, and rebuilds it as the stock submenu for a value the client has never heard of -- which comes
-- out empty and stays hidden -- and it marks the unit dropdown's own initialize as this addon's on the way through,
-- which is the exact thing the note above ENTRY_TEXT is about.  The arrow sits directly on the path from the entry
-- across to the submenu, so this fired every single time the cursor was moved over to pick one of the options.
-- Only the picture is wanted here, so the mouse is taken off it and the cursor falls through to the entry underneath,
-- which is what holds the submenu open
local hookedExpandArrows = {};

-- Runs whenever the shared arrow is shown again, including for the stock entries that really do want it clickable
local function EQ_Mentorship_ExpandArrowOnShow(expandArrow)
	expandArrow:EnableMouse(true);
end

local function EQ_Mentorship_ShowExpandArrowAsPicture(button)
	local expandArrow = _G[button:GetName() .. "ExpandArrow"];
	if ( expandArrow == nil ) then
		return;
	end
	local arrowName = expandArrow:GetName();
	if ( arrowName ~= nil and hookedExpandArrows[arrowName] == nil ) then
		hookedExpandArrows[arrowName] = true;
		expandArrow:HookScript("OnShow", EQ_Mentorship_ExpandArrowOnShow);
	end

	-- UIDropDownMenu_AddButton hides the arrow for every entry that did not ask for one, this one included, so showing
	-- it here always goes back through the hook above and the mouse is only ever off it while it belongs to this entry
	expandArrow:Show();
	expandArrow:EnableMouse(false);
end

-- Adds one of these entries to a dropdown list and hands back the button it landed on.  An entry that cannot be used
-- right now is handed to the client as disabled rather than being disabled afterwards, because the client disables a
-- button *before* it sets its text and its font objects, and that order is what makes the greyed out font stick
local function EQ_Mentorship_AddEntryButton(entryValue, level, entryFunc, ownerValue)
	local listFrame = _G["DropDownList" .. level];
	if ( listFrame == nil ) then
		return nil;
	end

	local info = EQ_Mentorship_BuildEntryInfo();
	info.text = ENTRY_TEXT[entryValue];
	info.value = entryValue;
	info.owner = ownerValue;
	info.notCheckable = 1;
	info.func = entryFunc;
	if ( entryValue == MENU_ROOT ) then
		-- The client only widens an entry to make room for the arrow when it was asked for with info.hasArrow, which this
		-- one deliberately never does, so the same ten pixels are reserved by hand instead
		info.padding = 10;
	end
	if ( menuDisabledReasons[entryValue] ~= nil ) then
		-- The invisible button the client shows behind a disabled entry is what keeps the mouse events flowing, which
		-- is both what stops the menu timing out and what lets the entry still be hovered for its tooltip
		info.disabled = 1;
	end
	UIDropDownMenu_AddButton(info, level);

	local button = _G["DropDownList" .. level .. "Button" .. listFrame.numButtons];
	if ( button == nil or button.value ~= entryValue ) then
		return nil;
	end
	EQ_Mentorship_HookEntryTooltip(button);
	return button;
end

-- ===================================================================================
-- Clicking an entry
-- ===================================================================================

-- The root entry only carries the submenu, so clicking it does nothing but close the menu, which the client has
-- already done by the time this runs
local function EQ_Mentorship_RootEntryOnClick(button)
	return;
end

local function EQ_Mentorship_SubMenuEntryOnClick(button)
	if ( button == nil ) then
		return;
	end
	local entryValue = button.value;
	if ( entryValue == MENU_END ) then
		CloseDropDownMenus();
		SendChatMessage(".eqmentorship end", "SAY");
		return;
	end

	-- Read before the menu is taken down, since the name is only known through the dropdown it was opened on
	local targetName = EQ_Mentorship_GetMenuUnitName();
	CloseDropDownMenus();
	if ( targetName == nil ) then
		DEFAULT_CHAT_FRAME:AddMessage("Could not tell who that menu was opened on. Target them and type /mentorship mentor instead.", 1.0, 0.3, 0.3);
		return;
	end
	if ( entryValue == MENU_MENTOR ) then
		SendChatMessage(".eqmentorship mentor " .. targetName, "SAY");
	elseif ( entryValue == MENU_APPRENTICE ) then
		SendChatMessage(".eqmentorship apprentice " .. targetName, "SAY");
	end
end

-- ===================================================================================
-- Putting the entry on the menu
-- ===================================================================================

-- Added once the client has finished building the stock menu, so that none of this addon's own values are ever read
-- by UnitPopup_ShowMenu or UnitPopup_HideButtons.  The stock "Cancel" is taken back off and put on again underneath,
-- so the entry keeps the place it was designed for without any stock entry being rebuilt here
local function EQ_Mentorship_AddRootEntry(which)
	local listFrame = _G["DropDownList1"];
	if ( listFrame == nil or type(listFrame.numButtons) ~= "number" or listFrame.numButtons < 1 ) then
		return;
	end

	local cancelText = nil;
	local cancelFunc = nil;
	local lastButton = _G["DropDownList1Button" .. listFrame.numButtons];
	if ( lastButton ~= nil and lastButton.value == "CANCEL" ) then
		cancelText = lastButton:GetText();
		cancelFunc = lastButton.func;
		listFrame.numButtons = listFrame.numButtons - 1;
	end

	-- info.hasArrow is deliberately not asked for: it is what makes the client open the submenu through
	-- ToggleDropDownMenu -> UIDropDownMenu_Initialize, which would mark the unit dropdown's own "initialize" field as
	-- this addon's for the rest of the session and break every protected entry in the menu after that.  The arrow is
	-- only a texture, so it is shown by hand and the submenu is opened from the entry's OnEnter instead
	local rootButton = EQ_Mentorship_AddEntryButton(MENU_ROOT, 1, EQ_Mentorship_RootEntryOnClick, which);
	if ( rootButton ~= nil ) then
		EQ_Mentorship_ShowExpandArrowAsPicture(rootButton);
	end

	if ( cancelText ~= nil ) then
		local info = EQ_Mentorship_BuildEntryInfo();
		info.text = cancelText;
		info.value = "CANCEL";
		info.owner = which;
		info.notCheckable = 1;
		info.func = cancelFunc;
		UIDropDownMenu_AddButton(info, 1);
	end
end

-- Runs for every unit menu in the game and at both menu levels, so it works out for itself whether this is one of
-- the menus the entry belongs on
hooksecurefunc("UnitPopup_ShowMenu", function(dropdownMenu, which, unit, name, userData)
	if ( UIDROPDOWNMENU_MENU_LEVEL ~= 1 ) then
		return;
	end
	if ( which == nil or OFFERED_ON_MENUS[which] == nil ) then
		return;
	end
	EQ_Mentorship_AddRootEntry(which);
end);

-- ===================================================================================
-- The submenu
-- ===================================================================================

-- This list is the addon's own frame, and deliberately not the client's second level one (DropDownList2).  That list
-- belongs to the client and everything it does with it assumes the client opened it: every dropdown entry without an
-- arrow closes the level below itself on hover (UIDropDownMenuButtonTemplate's OnEnter calls CloseDropDownMenus), the
-- expand arrow reopens it through ToggleDropDownMenu, and any menu opening anywhere in the UI resets it.  A submenu
-- opened there by hand loses every one of those races, which is what made this one impossible to reach with the
-- cursor.  Nothing in the client's dropdown code can touch this frame:
--   * its ID is above the client's menu levels, so the "CloseDropDownMenus(my level + 1)" that the stock button
--     scripts make is a no op (CloseDropDownMenus only walks up to UIDROPDOWNMENU_MAXLEVELS)
--   * its "parent" field points at the menu it hangs off, so the client's own timeout counting treats hovering this
--     list as hovering that menu and the two second countdown stops, exactly as it would for a stock submenu
-- What still has to be done by hand is closing it: when the menu goes away, and when another entry is hovered
local subMenuList = nil;
local SUBMENU_LIST_ID = UIDROPDOWNMENU_MAXLEVELS + 1;

function EQ_Mentorship_HideSubMenu()
	if ( subMenuList ~= nil ) then
		subMenuList:Hide();
	end
end

-- Moving onto any other entry closes this, the way the client closes a stock submenu with CloseDropDownMenus.  Runs
-- for every dropdown in the game, and only ever hides a frame of this addon's own
local function EQ_Mentorship_MenuButtonOnEnter(button)
	if ( button ~= nil and button.value == MENU_ROOT ) then
		return;
	end
	EQ_Mentorship_HideSubMenu();
end

local function EQ_Mentorship_GetSubMenuList()
	if ( subMenuList ~= nil ) then
		return subMenuList;
	end

	local parentList = _G["DropDownList1"];
	if ( parentList == nil ) then
		return nil;
	end

	-- The stock list template, so the entries look and behave exactly like the client's own
	subMenuList = CreateFrame("Button", "EQ_MentorshipSubMenuList", UIParent, "UIDropDownListTemplate");
	subMenuList:SetID(SUBMENU_LIST_ID);
	subMenuList:SetToplevel(true);

	-- A template carries no size of its own (DropDownList1 and 2 get theirs from their own XML), and a frame with no
	-- size has no rect to measure or to draw
	subMenuList:SetWidth(180);
	subMenuList:SetHeight(10);
	subMenuList:Hide();

	-- Whatever takes the menu down takes this with it: a click, the escape key, the menu timing out, or another
	-- menu opening anywhere in the UI (which hides this one first)
	parentList:HookScript("OnHide", EQ_Mentorship_HideSubMenu);
	for index = 1, UIDROPDOWNMENU_MAXBUTTONS do
		local menuButton = _G["DropDownList1Button" .. index];
		if ( menuButton ~= nil ) then
			menuButton:HookScript("OnEnter", EQ_Mentorship_MenuButtonOnEnter);
		end
	end

	return subMenuList;
end

-- One entry, laid out the way UIDropDownMenu_AddButton would have laid it out, and handing back the width the client
-- would have worked out for it.  An entry that cannot be used right now is disabled before its text and font objects
-- are set, because that order is what makes the greyed out font stick
local function EQ_Mentorship_FillSubMenuEntry(listFrame, index, entryValue)
	local buttonName = listFrame:GetName() .. "Button" .. index;
	local button = _G[buttonName];
	local normalText = _G[buttonName .. "NormalText"];
	if ( button == nil or normalText == nil ) then
		return 0;
	end

	button:SetDisabledFontObject(GameFontDisableSmallLeft);
	button:Enable();
	local invisibleButton = _G[buttonName .. "InvisibleButton"];
	if ( menuDisabledReasons[entryValue] ~= nil ) then
		button:Disable();
		-- The invisible button the client puts behind a disabled entry is what keeps the mouse events flowing, which
		-- is both what stops the menu timing out and what lets the entry still be hovered for its tooltip
		if ( invisibleButton ~= nil ) then
			invisibleButton:Show();
		end
	elseif ( invisibleButton ~= nil ) then
		invisibleButton:Hide();
	end

	button:SetText(ENTRY_TEXT[entryValue]);
	button:SetNormalFontObject(GameFontHighlightSmallLeft);
	button:SetHighlightFontObject(GameFontHighlightSmallLeft);
	normalText:ClearAllPoints();
	normalText:SetPoint("LEFT", button, "LEFT", 0, 0);

	button.value = entryValue;
	button.func = EQ_Mentorship_SubMenuEntryOnClick;
	button.owner = MENU_ROOT;
	button.notCheckable = 1;
	button.hasArrow = nil;
	button.checked = nil;
	button:UnlockHighlight();

	-- Nothing on these entries but their text
	local check = _G[buttonName .. "Check"];
	if ( check ~= nil ) then
		check:Hide();
	end
	local icon = _G[buttonName .. "Icon"];
	if ( icon ~= nil ) then
		icon:Hide();
	end
	local colorSwatch = _G[buttonName .. "ColorSwatch"];
	if ( colorSwatch ~= nil ) then
		colorSwatch:Hide();
	end
	local expandArrow = _G[buttonName .. "ExpandArrow"];
	if ( expandArrow ~= nil ) then
		expandArrow:Hide();
	end
	local highlight = _G[buttonName .. "Highlight"];
	if ( highlight ~= nil ) then
		highlight:Hide();
	end

	button:ClearAllPoints();
	button:SetPoint("TOPLEFT", listFrame, "TOPLEFT", 15, -((index - 1) * UIDROPDOWNMENU_BUTTON_HEIGHT) - UIDROPDOWNMENU_BORDER_HEIGHT);
	button:Show();

	EQ_Mentorship_HookEntryTooltip(button);

	-- The client's own sum: the text, its padding, less the room a checkbox would have taken
	return normalText:GetWidth() + 40 - 30;
end

-- Opened by hand from the root entry's OnEnter, since the entry deliberately carries no info.hasArrow.  The layout is
-- the level two half of ToggleDropDownMenu, deliberately kept in step with it
local function EQ_Mentorship_BuildSubMenu(rootButton)
	if ( rootButton == nil ) then
		return;
	end
	local parentList = rootButton:GetParent();
	if ( parentList == nil ) then
		return;
	end
	local listFrame = EQ_Mentorship_GetSubMenuList();
	if ( listFrame == nil ) then
		return;
	end

	-- State can have moved on while the menu was open, so the reasons are worked out again here
	EQ_Mentorship_UpdateMenuEligibility();

	listFrame:Hide();
	listFrame:SetScale(parentList:GetScale());
	listFrame.maxWidth = 0;
	listFrame.numButtons = 0;
	listFrame.onHide = nil;
	for index = 1, UIDROPDOWNMENU_MAXBUTTONS do
		local button = _G[listFrame:GetName() .. "Button" .. index];
		if ( button ~= nil ) then
			button:Hide();
		end
	end

	local entryCount = table.getn(SUBMENU_ENTRY_VALUES);
	for index = 1, entryCount do
		local entryWidth = EQ_Mentorship_FillSubMenuEntry(listFrame, index, SUBMENU_ENTRY_VALUES[index]);
		if ( entryWidth > listFrame.maxWidth ) then
			listFrame.maxWidth = entryWidth;
		end
	end
	if ( entryCount < 1 ) then
		return;
	end
	listFrame.numButtons = entryCount;
	listFrame:SetHeight((entryCount * UIDROPDOWNMENU_BUTTON_HEIGHT) + (UIDROPDOWNMENU_BORDER_HEIGHT * 2));

	-- Right click menus use the tooltip style backdrop, everything else the dialog one
	local openMenu = UIDROPDOWNMENU_OPEN_MENU;
	if ( type(openMenu) == "string" ) then
		openMenu = getglobal(openMenu);
	end
	local backdrop = _G[listFrame:GetName() .. "Backdrop"];
	local menuBackdrop = _G[listFrame:GetName() .. "MenuBackdrop"];
	if ( backdrop ~= nil and menuBackdrop ~= nil ) then
		if ( openMenu ~= nil and openMenu.displayMode == "MENU" ) then
			backdrop:Hide();
			menuBackdrop:Show();
		else
			backdrop:Show();
			menuBackdrop:Hide();
		end
	end

	listFrame.parentLevel = parentList:GetID();
	listFrame.parentID = rootButton:GetID();
	listFrame:SetFrameStrata(parentList:GetFrameStrata());
	listFrame:ClearAllPoints();
	listFrame:SetPoint("TOPLEFT", rootButton, "TOPRIGHT", 0, 0);
	listFrame:Show();

	-- Sized here rather than left to the template's OnShow, so the list is never left at whatever width it happened
	-- to have.  Same arithmetic the client uses: the widest entry, plus the border either side
	listFrame:SetWidth(listFrame.maxWidth + 25);
	for index = 1, entryCount do
		local button = _G[listFrame:GetName() .. "Button" .. index];
		if ( button ~= nil ) then
			button:SetWidth(listFrame.maxWidth);
		end
	end

	-- Counts as part of the menu it hangs off for the client's timeout, which has to be set after the show: the
	-- template's own OnShow points the parent field at the list one below its ID, which is not this list's menu
	listFrame.parent = parentList;
	UIDropDownMenu_StopCounting(parentList);

	-- Shown first so its real size is known, then anchored again the way the client anchors its own submenus,
	-- including the nudge back on screen when it would hang off the bottom or the right hand edge.  A frame that has
	-- only just been created does not always have a rect to measure yet, and a menu that cannot be measured is still
	-- better left where it is than taken back down, so the nudge is skipped rather than the menu closed
	local centerX, centerY = listFrame:GetCenter();
	if ( centerX == nil or centerY == nil ) then
		return;
	end
	local point = "TOPLEFT";
	local relativePoint = "TOPRIGHT";
	local xOffset = 0;
	local yOffset = 14;
	local offscreenY = ( (centerY - (listFrame:GetHeight() / 2)) < 0 );
	local offscreenX = ( listFrame:GetRight() > GetScreenWidth() );
	if ( offscreenY == true and offscreenX == true ) then
		point = "BOTTOMRIGHT";
		relativePoint = "BOTTOMLEFT";
		xOffset = -11;
		yOffset = -14;
	elseif ( offscreenY == true ) then
		point = "BOTTOMLEFT";
		relativePoint = "BOTTOMRIGHT";
		xOffset = 0;
		yOffset = -14;
	elseif ( offscreenX == true ) then
		point = "TOPRIGHT";
		relativePoint = "TOPLEFT";
		xOffset = -11;
		yOffset = 14;
	end
	listFrame:ClearAllPoints();
	listFrame:SetPoint(point, rootButton, relativePoint, xOffset, yOffset);
end

-- The client swallows an error thrown inside a script handler unless scriptErrors is turned on, and a menu that
-- quietly does nothing is a menu nobody can report properly.  Anything that goes wrong in the build says so
function EQ_Mentorship_ShowSubMenu(rootButton)
	local succeeded, errorMessage = pcall(EQ_Mentorship_BuildSubMenu, rootButton);
	if ( succeeded == false and DEFAULT_CHAT_FRAME ~= nil ) then
		DEFAULT_CHAT_FRAME:AddMessage("|cffFF2020EQ Mentorship could not open its menu: " .. tostring(errorMessage) .. "|r");
	end
end

-- ===================================================================================
-- Incoming offer
-- ===================================================================================

StaticPopupDialogs["EQ_MENTORSHIP_REQUEST"] = {
	text = "%s",
	button1 = ACCEPT,
	button2 = DECLINE,
	OnAccept = function()
		SendChatMessage(".eqmentorship accept", "SAY");
	end,
	OnCancel = function()
		SendChatMessage(".eqmentorship decline", "SAY");
	end,
	timeout = 30,
	whileDead = 1,
	hideOnEscape = 1,
	showAlert = 1,
};

local function EQ_Mentorship_HandleRequestPayload(payload)
	local requesterName, requesterRole, timeoutSeconds = string.match(payload, "^([^\t]*)\t([^\t]*)\t([^\t]*)$");
	if ( requesterName == nil or requesterName == "" ) then
		return;
	end

	local promptText;
	if ( tonumber(requesterRole) == ROLE_MENTOR ) then
		promptText = requesterName .. " wants to mentor you.\n\nThey will drop to one level above you until it ends, and earn nothing while it lasts.";
	else
		promptText = requesterName .. " wants to apprentice to you.\n\nThey will rise to one level below you until it ends, and bank everything you earn to spend at their own level.";
	end

	local requestTimeout = tonumber(timeoutSeconds);
	if ( requestTimeout == nil or requestTimeout < 5 ) then
		requestTimeout = 30;
	end
	StaticPopupDialogs["EQ_MENTORSHIP_REQUEST"].timeout = requestTimeout;
	StaticPopup_Show("EQ_MENTORSHIP_REQUEST", promptText);
end

-- ===================================================================================
-- Server state
-- ===================================================================================

local function EQ_Mentorship_HandleStatePayload(payload)
	local role, partnerName, standingLevel, realLevel, mentorshipEnabled, minLevelGap = string.match(payload, "^([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)\t([^\t]*)$");
	if ( role == nil ) then
		return;
	end
	currentRole = tonumber(role) or ROLE_NONE;
	currentPartnerName = partnerName or "";
	currentStandingLevel = tonumber(standingLevel) or 0;
	currentRealLevel = tonumber(realLevel) or 0;
	currentMentorshipEnabled = ( (tonumber(mentorshipEnabled) or 1) ~= 0 );
	currentMinLevelGap = tonumber(minLevelGap) or 3;
	if ( currentMinLevelGap < 2 ) then
		currentMinLevelGap = 2;
	end
	EQ_Mentorship_RefreshMenuEntries();

	-- An offer that is no longer relevant should not be left sitting on screen
	if ( currentRole ~= ROLE_NONE ) then
		StaticPopup_Hide("EQ_MENTORSHIP_REQUEST");
	end
end

function EQ_Mentorship_HandlePayload(payload)
	if ( payload == nil ) then
		return;
	end
	local messageKind, remainder = string.match(payload, "^([^\t]+)\t(.*)$");
	if ( messageKind == "STATE" ) then
		EQ_Mentorship_HandleStatePayload(remainder);
	elseif ( messageKind == "REQUEST" ) then
		EQ_Mentorship_HandleRequestPayload(remainder);
	end
end

local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	-- The server pushes the current state at login, but asking again on entering the world covers the case where
	-- that message landed before this addon finished loading.  "sync" prints nothing, it only pushes the state back
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		SendChatMessage(".eqmentorship sync", "SAY");
		return;
	end
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQMENTORSHIP_PREFIX ) then
		EQ_Mentorship_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQMENTORSHIP_PREFIX .. "\t") ) then
		EQ_Mentorship_HandlePayload(string.gsub(arg1, "^" .. EQMENTORSHIP_PREFIX .. "\t", ""));
	end
end);

EQ_Mentorship_RefreshMenuEntries();
EQ_Mentorship_UpdateMenuEligibility();

-- ===================================================================================
-- Slash command (alternative to the right click menu)
-- ===================================================================================

SLASH_EQMENTORSHIP1 = "/mentorship";
SLASH_EQMENTORSHIP2 = "/eqmentorship";
SlashCmdList["EQMENTORSHIP"] = function(msg)
	msg = string.lower(msg or "");
	msg = string.gsub(msg, "^%s+", "");
	msg = string.gsub(msg, "%s+$", "");

	-- A bare "mentor" or "apprentice" means the current target, which the server resolves from the selection
	if ( msg == "mentor" or msg == "apprentice" ) then
		local targetName = UnitName("target");
		if ( targetName ~= nil and UnitIsPlayer("target") == true ) then
			SendChatMessage(".eqmentorship " .. msg .. " " .. targetName, "SAY");
		else
			SendChatMessage(".eqmentorship " .. msg, "SAY");
		end
		return;
	end
	if ( msg == "" ) then
		SendChatMessage(".eqmentorship", "SAY");
		return;
	end
	SendChatMessage(".eqmentorship " .. msg, "SAY");
end;
