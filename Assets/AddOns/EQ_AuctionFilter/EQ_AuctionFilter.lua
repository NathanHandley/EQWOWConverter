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

-- Adds a "Realm Filter" button to the auction house that opens a window listing the same categories and
-- subcategories the auction house's own left hand filter list shows.  Each one can be set to show Norrath
-- (EverQuest) items, Azeroth (World of Warcraft) items, or both, and there is a "Set all" row that puts every
-- entry on the same setting at once.
--
-- The server (mod-everquest) owns the setting and stores it per ACCOUNT, so every character on the account
-- shares one filter.  It pushes the current value under this prefix at login and after every change, and the
-- values travel as one positional string of '0' (both), '1' (Norrath) and '2' (Azeroth) characters: one for the
-- global fallback, then per category one for the category itself followed by one per subcategory.  The server
-- side copy of that layout lives in EverQuest.cpp (AuctionCategories) and the two must stay in step.
local EQAHFILTER_PREFIX = "EQAHFILTER";

local MODE_BOTH = 0;
local MODE_NORRATH = 1;
local MODE_AZEROTH = 2;
local MODE_MIXED = -1;      -- Display only, for a category whose subcategories do not all agree

-- The colour is baked into the text rather than set on the button's font string, since a button swaps font
-- objects as it is moused over and that drops any colour set on the string
local MODE_TEXT =
{
	[MODE_MIXED] = "|cff999999Mixed|r",
	[MODE_BOTH] = "|cffFFFFFFBoth|r",
	[MODE_NORRATH] = "|cff66CCFFNorrath|r",
	[MODE_AZEROTH] = "|cffFFC64DAzeroth|r",
};

-- How many subcategories each auction category has, in the order GetAuctionItemClasses returns them (Weapon,
-- Armor, Container, Consumable, Glyph, Trade Goods, Projectile, Quiver, Recipe, Gem, Miscellaneous, Quest).
-- Quest is a real auction category with no subcategories of its own, so its count is zero.  These are only used
-- to lay the payload out and to sanity check the client lists before anything is sent, since a mismatch would
-- mean settings landing on the wrong categories server side
local SUBCATEGORY_COUNTS = { 17, 10, 9, 8, 10, 15, 2, 2, 12, 9, 6, 0 };

local CATEGORY_COUNT = #SUBCATEGORY_COUNTS;
local GLOBAL_VALUE_INDEX = 1;
local categoryValueIndex = {};      -- [categoryIndex] = position in the payload of that category's own value
local subCategoryValueBase = {};    -- [categoryIndex] = position just before that category's first subcategory value
local PAYLOAD_LENGTH = 1;
for categoryIndex = 1, CATEGORY_COUNT do
	PAYLOAD_LENGTH = PAYLOAD_LENGTH + 1;
	categoryValueIndex[categoryIndex] = PAYLOAD_LENGTH;
	subCategoryValueBase[categoryIndex] = PAYLOAD_LENGTH;
	PAYLOAD_LENGTH = PAYLOAD_LENGTH + SUBCATEGORY_COUNTS[categoryIndex];
end

local NUM_VISIBLE_ROWS = 16;
local ROW_HEIGHT = 20;

local modeValues = {};              -- Flat payload values, 1 based, indexed the same way the payload string is
local categoryNames = nil;          -- Filled in from the client the first time the window is opened
local categoryShapeSeen = nil;      -- What the client's lists actually looked like, so a mismatch can say so
local subCategoryNames = nil;
local categoryExpanded = {};
local displayRows = {};             -- Flattened visible list of { categoryIndex, subCategoryIndex or nil }
local rows = {};
local saveDelayRemaining = nil;
local changedWhileOpen = false;
local pendingSearchTimeout = nil;           -- Seconds left to get a re-run of the browse search away after a change
local pendingSearchWaitingForEcho = false;  -- Still waiting on the server to confirm it has the new setting

for valueIndex = 1, PAYLOAD_LENGTH do
	modeValues[valueIndex] = MODE_BOTH;
end
for categoryIndex = 1, CATEGORY_COUNT do
	categoryExpanded[categoryIndex] = false;
end

-- ===================================================================================
-- Payload
-- ===================================================================================

local function EQ_AuctionFilter_BuildPayload()
	local characters = {};
	for valueIndex = 1, PAYLOAD_LENGTH do
		characters[valueIndex] = tostring(modeValues[valueIndex]);
	end
	return table.concat(characters);
end

local function EQ_AuctionFilter_ApplyPayload(payload)
	if ( payload == nil or string.len(payload) ~= PAYLOAD_LENGTH ) then
		return false;
	end
	local readValues = {};
	for valueIndex = 1, PAYLOAD_LENGTH do
		local value = tonumber(string.sub(payload, valueIndex, valueIndex));
		if ( value == nil or value < MODE_BOTH or value > MODE_AZEROTH ) then
			return false;
		end
		readValues[valueIndex] = value;
	end
	modeValues = readValues;
	return true;
end

local function EQ_AuctionFilter_QueueSave()
	changedWhileOpen = true;
	saveDelayRemaining = 1.0;
end

local function EQ_AuctionFilter_FlushSave()
	if ( saveDelayRemaining == nil ) then
		return;
	end
	saveDelayRemaining = nil;
	SendChatMessage(".eqauctionfilter set " .. EQ_AuctionFilter_BuildPayload(), "SAY");
end

-- ===================================================================================
-- Category lists
-- ===================================================================================

-- The names come from the client so they stay localised and stay in the auction house's own order.  If the
-- lists ever come back a different shape than the payload expects, editing is refused rather than risking
-- settings being applied to the wrong categories
local function EQ_AuctionFilter_EnsureCategoryNames()
	if ( categoryNames ~= nil ) then
		return true;
	end
	local names = { GetAuctionItemClasses() };
	local subNames = {};
	local observedShape = #names .. " categories";
	local shapeMatches = ( #names == CATEGORY_COUNT );
	for categoryIndex = 1, #names do
		subNames[categoryIndex] = { GetAuctionItemSubClasses(categoryIndex) };
		if ( categoryIndex == 1 ) then
			observedShape = observedShape .. " (" .. #subNames[categoryIndex];
		else
			observedShape = observedShape .. ", " .. #subNames[categoryIndex];
		end
		if ( SUBCATEGORY_COUNTS[categoryIndex] == nil or #subNames[categoryIndex] ~= SUBCATEGORY_COUNTS[categoryIndex] ) then
			shapeMatches = false;
		end
	end
	if ( #names > 0 ) then
		observedShape = observedShape .. ")";
	end
	categoryShapeSeen = observedShape;
	if ( shapeMatches == false ) then
		return false;
	end
	categoryNames = names;
	subCategoryNames = subNames;
	return true;
end

-- A category shows the value its subcategories share, or "Mixed" when they disagree
local function EQ_AuctionFilter_GetCategoryDisplayMode(categoryIndex)
	if ( SUBCATEGORY_COUNTS[categoryIndex] == 0 ) then
		return modeValues[categoryValueIndex[categoryIndex]];
	end
	local sharedMode = modeValues[subCategoryValueBase[categoryIndex] + 1];
	for subCategoryIndex = 2, SUBCATEGORY_COUNTS[categoryIndex] do
		if ( modeValues[subCategoryValueBase[categoryIndex] + subCategoryIndex] ~= sharedMode ) then
			return MODE_MIXED;
		end
	end
	return sharedMode;
end

local function EQ_AuctionFilter_SetCategoryMode(categoryIndex, mode)
	modeValues[categoryValueIndex[categoryIndex]] = mode;
	for subCategoryIndex = 1, SUBCATEGORY_COUNTS[categoryIndex] do
		modeValues[subCategoryValueBase[categoryIndex] + subCategoryIndex] = mode;
	end
end

local function EQ_AuctionFilter_SetAllModes(mode)
	modeValues[GLOBAL_VALUE_INDEX] = mode;
	for categoryIndex = 1, CATEGORY_COUNT do
		EQ_AuctionFilter_SetCategoryMode(categoryIndex, mode);
	end
end

-- Both -> Norrath -> Azeroth -> Both, and a "Mixed" category unifies onto Both first
local function EQ_AuctionFilter_GetNextMode(mode)
	if ( mode == MODE_MIXED ) then
		return MODE_BOTH;
	end
	if ( mode == MODE_BOTH ) then
		return MODE_NORRATH;
	end
	if ( mode == MODE_NORRATH ) then
		return MODE_AZEROTH;
	end
	return MODE_BOTH;
end

-- The category and global values are never shown as rows of their own: they are what an item falls back on when
-- the auction house has no subcategory for its subclass, or no category for its class at all.  Keeping them in
-- step with the visible rows whenever those agree makes the window mean what it looks like it means
local function EQ_AuctionFilter_SyncFallbackModes()
	local sharedMode = nil;
	for categoryIndex = 1, CATEGORY_COUNT do
		local categoryMode = EQ_AuctionFilter_GetCategoryDisplayMode(categoryIndex);
		if ( categoryMode ~= MODE_MIXED ) then
			modeValues[categoryValueIndex[categoryIndex]] = categoryMode;
		end
		if ( categoryIndex == 1 ) then
			sharedMode = categoryMode;
		elseif ( categoryMode ~= sharedMode ) then
			sharedMode = MODE_MIXED;
		end
	end
	if ( sharedMode ~= nil and sharedMode ~= MODE_MIXED ) then
		modeValues[GLOBAL_VALUE_INDEX] = sharedMode;
	end
end

local function EQ_AuctionFilter_RebuildDisplayRows()
	displayRows = {};
	for categoryIndex = 1, CATEGORY_COUNT do
		table.insert(displayRows, { categoryIndex = categoryIndex });
		if ( categoryExpanded[categoryIndex] == true ) then
			for subCategoryIndex = 1, SUBCATEGORY_COUNTS[categoryIndex] do
				table.insert(displayRows, { categoryIndex = categoryIndex, subCategoryIndex = subCategoryIndex });
			end
		end
	end
end

-- ===================================================================================
-- Window
-- ===================================================================================

local EQAuctionFilterFrame = CreateFrame("Frame", "EQAuctionFilterFrame", UIParent);
EQAuctionFilterFrame:SetSize(360, 460);
EQAuctionFilterFrame:SetPoint("CENTER", UIParent, "CENTER", 260, 0);
EQAuctionFilterFrame:SetBackdrop({
	bgFile = "Interface\\DialogFrame\\UI-DialogBox-Background",
	edgeFile = "Interface\\DialogFrame\\UI-DialogBox-Border",
	tile = true, tileSize = 32, edgeSize = 32,
	insets = { left = 11, right = 12, top = 12, bottom = 11 },
});
EQAuctionFilterFrame:SetMovable(true);
EQAuctionFilterFrame:EnableMouse(true);
EQAuctionFilterFrame:RegisterForDrag("LeftButton");
EQAuctionFilterFrame:SetScript("OnDragStart", function(self) self:StartMoving(); end);
EQAuctionFilterFrame:SetScript("OnDragStop", function(self) self:StopMovingOrSizing(); end);
EQAuctionFilterFrame:SetFrameStrata("HIGH");
EQAuctionFilterFrame:SetToplevel(true);
EQAuctionFilterFrame:Hide();
tinsert(UISpecialFrames, "EQAuctionFilterFrame");

local titleText = EQAuctionFilterFrame:CreateFontString("EQAuctionFilterFrameTitle", "ARTWORK", "GameFontNormal");
titleText:SetPoint("TOP", EQAuctionFilterFrame, "TOP", 0, -16);
titleText:SetText("Auction Realm Filter");

local closeButton = CreateFrame("Button", "EQAuctionFilterFrameCloseButton", EQAuctionFilterFrame, "UIPanelCloseButton");
closeButton:SetPoint("TOPRIGHT", EQAuctionFilterFrame, "TOPRIGHT", -6, -8);

local helpText = EQAuctionFilterFrame:CreateFontString("EQAuctionFilterFrameHelpText", "ARTWORK", "GameFontHighlightSmall");
helpText:SetPoint("TOPLEFT", EQAuctionFilterFrame, "TOPLEFT", 18, -38);
helpText:SetWidth(324);
helpText:SetJustifyH("LEFT");
helpText:SetJustifyV("TOP");
helpText:SetText("Pick which world's items each auction category shows.  |cff66CCFFNorrath|r is EverQuest, |cffFFC64DAzeroth|r is World of Warcraft.  Saved for the whole account.");

local setAllText = EQAuctionFilterFrame:CreateFontString("EQAuctionFilterFrameSetAllText", "ARTWORK", "GameFontNormalSmall");
setAllText:SetPoint("TOPLEFT", EQAuctionFilterFrame, "TOPLEFT", 18, -86);
setAllText:SetText("Set all:");

local EQ_AuctionFilter_UpdateList;

local function EQ_AuctionFilter_SetAllButton_OnClick(self)
	EQ_AuctionFilter_SetAllModes(self.mode);
	EQ_AuctionFilter_QueueSave();
	EQ_AuctionFilter_UpdateList();
end

local setAllButtons = {};
local setAllModes = { MODE_BOTH, MODE_NORRATH, MODE_AZEROTH };
for buttonIndex = 1, #setAllModes do
	local setAllButton = CreateFrame("Button", "EQAuctionFilterSetAllButton" .. buttonIndex, EQAuctionFilterFrame, "UIPanelButtonTemplate");
	setAllButton:SetSize(80, 20);
	if ( buttonIndex == 1 ) then
		setAllButton:SetPoint("LEFT", setAllText, "RIGHT", 8, 0);
	else
		setAllButton:SetPoint("LEFT", setAllButtons[buttonIndex - 1], "RIGHT", 4, 0);
	end
	setAllButton.mode = setAllModes[buttonIndex];
	setAllButton:SetText(MODE_TEXT[setAllModes[buttonIndex]]);
	setAllButton:SetScript("OnClick", EQ_AuctionFilter_SetAllButton_OnClick);
	setAllButtons[buttonIndex] = setAllButton;
end

local unavailableText = EQAuctionFilterFrame:CreateFontString("EQAuctionFilterFrameUnavailableText", "ARTWORK", "GameFontDisable");
unavailableText:SetPoint("TOP", EQAuctionFilterFrame, "TOP", 0, -140);
unavailableText:SetWidth(300);
unavailableText:SetJustifyV("TOP");
unavailableText:SetText("The auction house category list could not be read, so the realm filter is unavailable.");
unavailableText:SetJustifyH("LEFT");
unavailableText:Hide();

-- ===================================================================================
-- List rows (recycled buttons over a faux scroll frame)
-- ===================================================================================

local scrollFrame = CreateFrame("ScrollFrame", "EQAuctionFilterFrameScrollFrame", EQAuctionFilterFrame, "FauxScrollFrameTemplate");
scrollFrame:SetPoint("TOPLEFT", EQAuctionFilterFrame, "TOPLEFT", 18, -112);
scrollFrame:SetPoint("BOTTOMRIGHT", EQAuctionFilterFrame, "BOTTOMRIGHT", -38, 20);
scrollFrame:SetScript("OnVerticalScroll", function(self, offset)
	FauxScrollFrame_OnVerticalScroll(self, offset, ROW_HEIGHT, EQ_AuctionFilter_UpdateList);
end);

function EQ_AuctionFilter_UpdateList()
	FauxScrollFrame_Update(scrollFrame, #displayRows, NUM_VISIBLE_ROWS, ROW_HEIGHT);
	local offset = FauxScrollFrame_GetOffset(scrollFrame);

	for rowIndex = 1, NUM_VISIBLE_ROWS do
		local row = rows[rowIndex];
		local entry = displayRows[rowIndex + offset];
		if ( entry ) then
			row.categoryIndex = entry.categoryIndex;
			row.subCategoryIndex = entry.subCategoryIndex;
			local mode;
			if ( entry.subCategoryIndex == nil ) then
				row.nameText:SetPoint("LEFT", row, "LEFT", 22, 0);
				row.nameText:SetFontObject(GameFontNormalSmall);
				row.nameText:SetText(categoryNames[entry.categoryIndex]);
				if ( categoryExpanded[entry.categoryIndex] == true ) then
					row.expandButton:SetNormalTexture("Interface\\Buttons\\UI-MinusButton-Up");
					row.expandButton:SetPushedTexture("Interface\\Buttons\\UI-MinusButton-Down");
				else
					row.expandButton:SetNormalTexture("Interface\\Buttons\\UI-PlusButton-Up");
					row.expandButton:SetPushedTexture("Interface\\Buttons\\UI-PlusButton-Down");
				end
				row.expandButton:Show();
				if ( SUBCATEGORY_COUNTS[entry.categoryIndex] == 0 ) then
					row.expandButton:Hide();
				end
				mode = EQ_AuctionFilter_GetCategoryDisplayMode(entry.categoryIndex);
			else
				row.nameText:SetPoint("LEFT", row, "LEFT", 38, 0);
				row.nameText:SetFontObject(GameFontHighlightSmall);
				row.nameText:SetText(subCategoryNames[entry.categoryIndex][entry.subCategoryIndex]);
				row.expandButton:Hide();
				mode = modeValues[subCategoryValueBase[entry.categoryIndex] + entry.subCategoryIndex];
			end
			row.modeButton.mode = mode;
			row.modeButton:SetText(MODE_TEXT[mode]);
			row:Show();
		else
			row.categoryIndex = nil;
			row.subCategoryIndex = nil;
			row:Hide();
		end
	end
end

local function EQ_AuctionFilter_Row_OnClick(self)
	if ( self.categoryIndex == nil or self.subCategoryIndex ~= nil ) then
		return;
	end
	if ( SUBCATEGORY_COUNTS[self.categoryIndex] == 0 ) then
		return;
	end
	categoryExpanded[self.categoryIndex] = not categoryExpanded[self.categoryIndex];
	EQ_AuctionFilter_RebuildDisplayRows();
	EQ_AuctionFilter_UpdateList();
end

local function EQ_AuctionFilter_ModeButton_OnClick(self)
	local row = self:GetParent();
	if ( row.categoryIndex == nil ) then
		return;
	end
	local nextMode = EQ_AuctionFilter_GetNextMode(self.mode);
	if ( row.subCategoryIndex == nil ) then
		EQ_AuctionFilter_SetCategoryMode(row.categoryIndex, nextMode);
	else
		modeValues[subCategoryValueBase[row.categoryIndex] + row.subCategoryIndex] = nextMode;
	end
	EQ_AuctionFilter_SyncFallbackModes();
	EQ_AuctionFilter_QueueSave();
	EQ_AuctionFilter_UpdateList();
end

for rowIndex = 1, NUM_VISIBLE_ROWS do
	local row = CreateFrame("Button", "EQAuctionFilterRow" .. rowIndex, EQAuctionFilterFrame);
	row:SetHeight(ROW_HEIGHT);
	row:SetPoint("TOPLEFT", scrollFrame, "TOPLEFT", 0, -(rowIndex - 1) * ROW_HEIGHT);
	row:SetPoint("TOPRIGHT", scrollFrame, "TOPRIGHT", 0, -(rowIndex - 1) * ROW_HEIGHT);
	row:SetHighlightTexture("Interface\\QuestFrame\\UI-QuestTitleHighlight");
	row:SetScript("OnClick", EQ_AuctionFilter_Row_OnClick);

	row.expandButton = CreateFrame("Button", "EQAuctionFilterRow" .. rowIndex .. "ExpandButton", row);
	row.expandButton:SetSize(16, 16);
	row.expandButton:SetPoint("LEFT", row, "LEFT", 2, 0);
	row.expandButton:SetHighlightTexture("Interface\\Buttons\\UI-PlusButton-Hilight");
	row.expandButton:SetScript("OnClick", function(self) EQ_AuctionFilter_Row_OnClick(self:GetParent()); end);

	row.modeButton = CreateFrame("Button", "EQAuctionFilterRow" .. rowIndex .. "ModeButton", row, "UIPanelButtonTemplate");
	row.modeButton:SetSize(80, 18);
	row.modeButton:SetPoint("RIGHT", row, "RIGHT", -2, 0);
	row.modeButton:SetScript("OnClick", EQ_AuctionFilter_ModeButton_OnClick);

	row.nameText = row:CreateFontString("EQAuctionFilterRow" .. rowIndex .. "NameText", "ARTWORK", "GameFontHighlightSmall");
	row.nameText:SetPoint("LEFT", row, "LEFT", 22, 0);
	row.nameText:SetPoint("RIGHT", row.modeButton, "LEFT", -4, 0);
	row.nameText:SetJustifyH("LEFT");

	row:Hide();
	rows[rowIndex] = row;
end

-- ===================================================================================
-- Opening and closing
-- ===================================================================================

local function EQ_AuctionFilter_Show()
	if ( EQ_AuctionFilter_EnsureCategoryNames() == false ) then
		unavailableText:SetText("The auction house category list is not the shape this addon expects, so the realm filter is unavailable.  The client reported " .. (categoryShapeSeen or "nothing") .. ".");
		unavailableText:Show();
		scrollFrame:Hide();
		for rowIndex = 1, NUM_VISIBLE_ROWS do
			rows[rowIndex]:Hide();
		end
		for buttonIndex = 1, #setAllButtons do
			setAllButtons[buttonIndex]:Disable();
		end
		EQAuctionFilterFrame:Show();
		return;
	end
	unavailableText:Hide();
	scrollFrame:Show();
	for buttonIndex = 1, #setAllButtons do
		setAllButtons[buttonIndex]:Enable();
	end
	changedWhileOpen = false;
	EQ_AuctionFilter_RebuildDisplayRows();
	EQ_AuctionFilter_UpdateList();
	EQAuctionFilterFrame:Show();
end

local function EQ_AuctionFilter_Toggle()
	if ( EQAuctionFilterFrame:IsShown() == true ) then
		EQAuctionFilterFrame:Hide();
	else
		EQ_AuctionFilter_Show();
	end
end

local function EQ_AuctionFilter_CanRunBrowseSearch()
	if ( AuctionFrame == nil or AuctionFrame:IsShown() == false ) then
		return false;
	end
	if ( AuctionFrameBrowse == nil or AuctionFrameBrowse:IsShown() == false ) then
		return false;
	end
	if ( type(AuctionFrameBrowse_Search) ~= "function" ) then
		return false;
	end
	if ( not CanSendAuctionQuery("list") ) then
		return false;
	end
	return true;
end

-- Re-running the search is what makes a change visible, since the server applies the filter while it gathers the
-- results rather than to the page already on screen.  It has to hold off until the server confirms the new
-- setting: the setting travels as a chat command and the search as its own packet, and the two are not
-- necessarily handled in the order they were sent, so searching straight away can be answered with the old one
local function EQ_AuctionFilter_QueueBrowseSearch()
	pendingSearchWaitingForEcho = true;
	pendingSearchTimeout = 5.0;
end

local function EQ_AuctionFilter_UpdatePendingSearch(elapsed)
	if ( pendingSearchTimeout == nil ) then
		return;
	end
	pendingSearchTimeout = pendingSearchTimeout - elapsed;
	local giveUpWaiting = ( pendingSearchTimeout <= 0 );
	if ( pendingSearchWaitingForEcho == true and giveUpWaiting == false ) then
		return;
	end
	-- Kept pending rather than dropped, since the auction house refuses a query for a moment after the last one
	if ( EQ_AuctionFilter_CanRunBrowseSearch() == false ) then
		if ( giveUpWaiting == true ) then
			pendingSearchTimeout = nil;
			pendingSearchWaitingForEcho = false;
		end
		return;
	end
	pendingSearchTimeout = nil;
	pendingSearchWaitingForEcho = false;
	AuctionFrameBrowse_Search();
end

-- A search started while a change has not been sent yet would be answered with the old setting, so the change goes
-- now and the search is run again as soon as the server confirms it
local function EQ_AuctionFilter_SearchButton_PreClick(self)
	if ( saveDelayRemaining == nil ) then
		return;
	end
	EQ_AuctionFilter_FlushSave();
	EQ_AuctionFilter_QueueBrowseSearch();
end

EQAuctionFilterFrame:SetScript("OnHide", function(self)
	if ( changedWhileOpen == false ) then
		return;
	end
	changedWhileOpen = false;
	EQ_AuctionFilter_FlushSave();
	EQ_AuctionFilter_QueueBrowseSearch();
end);

local function EQ_AuctionFilter_AttachToAuctionUI()
	if ( EQAuctionFilterOpenButton ~= nil or AuctionFrame == nil ) then
		return;
	end
	local openButton = CreateFrame("Button", "EQAuctionFilterOpenButton", AuctionFrameBrowse or AuctionFrame, "UIPanelButtonTemplate");
	openButton:SetSize(110, 22);
	openButton:SetText("Realm Filter");
	openButton:SetScript("OnClick", EQ_AuctionFilter_Toggle);

	-- Centred in the empty stretch of the browse header to the right of the "Usable Items" checkbox.  An
	-- invisible frame spanning that gap does the centring, so no widths or offsets have to be guessed at and it
	-- stays put whatever the checkbox's text ends up being in another locale.  The gap ends at the Prev page
	-- arrow rather than the Search button, since the arrow sits on the row below but reaches further left and
	-- the button's corner would otherwise land on top of it
	if ( BrowseSearchButton ~= nil ) then
		BrowseSearchButton:HookScript("PreClick", EQ_AuctionFilter_SearchButton_PreClick);
	end

	if ( IsUsableCheckButtonText ~= nil and BrowseSearchButton ~= nil ) then
		local rightBoundFrame = BrowseSearchButton;
		if ( BrowsePrevPageButton ~= nil ) then
			rightBoundFrame = BrowsePrevPageButton;
		end
		local gapFrame = CreateFrame("Frame", "EQAuctionFilterOpenButtonGap", AuctionFrameBrowse);
		gapFrame:SetPoint("LEFT", IsUsableCheckButtonText, "RIGHT", 0, 0);
		gapFrame:SetPoint("RIGHT", rightBoundFrame, "LEFT", -4, 0);
		gapFrame:SetPoint("TOP", BrowseSearchButton, "TOP", 0, 0);
		gapFrame:SetPoint("BOTTOM", BrowseSearchButton, "BOTTOM", 0, 0);
		openButton:SetPoint("CENTER", gapFrame, "CENTER", 0, 0);
	else
		openButton:SetPoint("TOPRIGHT", AuctionFrame, "BOTTOMRIGHT", -14, 2);
	end

	AuctionFrame:HookScript("OnHide", function(self) EQAuctionFilterFrame:Hide(); end);
end

SLASH_EQAUCTIONFILTER1 = "/eqahfilter";
SlashCmdList["EQAUCTIONFILTER"] = EQ_AuctionFilter_Toggle;

-- ===================================================================================
-- Server state
-- ===================================================================================

local function EQ_AuctionFilter_HandlePayload(payload)
	-- A push landing on top of an edit that has not been sent yet would undo it, and the only pushes that arrive
	-- are the login one and the echo of a save this addon just made, so a pending save always wins
	if ( saveDelayRemaining ~= nil ) then
		return;
	end

	-- This is the server saying it has the new setting, so a search held back for it can go
	pendingSearchWaitingForEcho = false;
	if ( EQ_AuctionFilter_ApplyPayload(payload) == false ) then
		return;
	end
	if ( EQAuctionFilterFrame:IsShown() == true and categoryNames ~= nil ) then
		EQ_AuctionFilter_RebuildDisplayRows();
		EQ_AuctionFilter_UpdateList();
	end
end

local eventFrame = CreateFrame("Frame", "EQAuctionFilterEventFrame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:RegisterEvent("ADDON_LOADED");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		-- The server pushes the filter at login, but asking again covers the case where that message landed
		-- before this addon finished loading.  "sync" prints nothing, it only pushes the value back
		SendChatMessage(".eqauctionfilter sync", "SAY");
		EQ_AuctionFilter_AttachToAuctionUI();
		return;
	end
	if ( event == "ADDON_LOADED" ) then
		if ( arg1 == "Blizzard_AuctionUI" ) then
			EQ_AuctionFilter_AttachToAuctionUI();
		end
		return;
	end
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQAHFILTER_PREFIX ) then
		EQ_AuctionFilter_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQAHFILTER_PREFIX .. "\t") ) then
		EQ_AuctionFilter_HandlePayload(string.gsub(arg1, "^" .. EQAHFILTER_PREFIX .. "\t", ""));
	end
end);

-- Changes are held briefly so a run of clicks turns into one message rather than one per click
eventFrame:SetScript("OnUpdate", function(self, elapsed)
	if ( saveDelayRemaining ~= nil ) then
		saveDelayRemaining = saveDelayRemaining - elapsed;
		if ( saveDelayRemaining <= 0 ) then
			EQ_AuctionFilter_FlushSave();
		end
	end
	EQ_AuctionFilter_UpdatePendingSearch(elapsed);
end);
