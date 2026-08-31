--  Author: Nathan Handley (nathanhandley@protonmail.com)
--  Copyright (c) 2025 Nathan Handley
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

-- EverQuest-style tracking: the server (mod-everquest) sends the nearby creature list under this prefix
-- whenever the Tracking spell is cast or ".track list" runs.  Picking an entry sends ".track start <guid>"
-- and the server then whispers periodic direction messages.  The creature guid is a 64 bit number, so it
-- is kept as a STRING everywhere here (Lua numbers lose precision above 2^53).
local EQTRACK_PREFIX = "EQTRACK";

local NUM_VISIBLE_ROWS = 14;
local ROW_HEIGHT = 18;

local SORT_COLUMN_NAME = "name";
local SORT_COLUMN_DISTANCE = "distance";

local trackEntries = {};        -- Committed list shown in the window: { guid, level, distance, name }
local pendingEntries = {};      -- Entries accumulating between the "H" header and "F" footer messages
local receivingList = false;
local maxTrackDistance = 0;
local selectedIndex = 0;
local trackedGuid = nil;        -- Guid string of the creature currently being tracked (nil when none)
local rows = {};
local sortColumn = SORT_COLUMN_DISTANCE;    -- The server builds the list nearest first, so start on that
local sortAscending = true;

-- Assigned down in the list section, but the column headers below need to call it
local EQTracking_UpdateList;

-- ===================================================================================
-- Window
-- ===================================================================================

local EQTrackingFrame = CreateFrame("Frame", "EQTrackingFrame", UIParent);
EQTrackingFrame:SetSize(340, 384);
EQTrackingFrame:SetPoint("CENTER", UIParent, "CENTER", 200, 60);
EQTrackingFrame:SetBackdrop({
	bgFile = "Interface\\DialogFrame\\UI-DialogBox-Background",
	edgeFile = "Interface\\DialogFrame\\UI-DialogBox-Border",
	tile = true, tileSize = 32, edgeSize = 32,
	insets = { left = 11, right = 12, top = 12, bottom = 11 },
});
EQTrackingFrame:SetMovable(true);
EQTrackingFrame:EnableMouse(true);
EQTrackingFrame:RegisterForDrag("LeftButton");
EQTrackingFrame:SetScript("OnDragStart", function(self) self:StartMoving(); end);
EQTrackingFrame:SetScript("OnDragStop", function(self) self:StopMovingOrSizing(); end);
EQTrackingFrame:SetFrameStrata("MEDIUM");
EQTrackingFrame:SetToplevel(true);
EQTrackingFrame:Hide();
tinsert(UISpecialFrames, "EQTrackingFrame");

local titleText = EQTrackingFrame:CreateFontString("EQTrackingFrameTitle", "ARTWORK", "GameFontNormal");
titleText:SetPoint("TOP", EQTrackingFrame, "TOP", 0, -16);
titleText:SetText("Tracking");

local closeButton = CreateFrame("Button", "EQTrackingFrameCloseButton", EQTrackingFrame, "UIPanelCloseButton");
closeButton:SetPoint("TOPRIGHT", EQTrackingFrame, "TOPRIGHT", -6, -8);

local emptyText = EQTrackingFrame:CreateFontString("EQTrackingFrameEmptyText", "ARTWORK", "GameFontDisable");
emptyText:SetPoint("TOP", EQTrackingFrame, "TOP", 0, -90);
emptyText:SetText("Nothing is within tracking range.");
emptyText:Hide();

-- ===================================================================================
-- Sorting
-- ===================================================================================

-- Sorts on the active column and falls back to the other one, so creatures that tie keep a stable order
-- from one refresh to the next.  Only the primary key follows the ascending/descending choice.
local function EQTracking_CompareEntries(leftEntry, rightEntry)
	local leftName = string.lower(leftEntry.name);
	local rightName = string.lower(rightEntry.name);
	local isLess;
	if ( sortColumn == SORT_COLUMN_NAME ) then
		if ( leftName ~= rightName ) then
			isLess = leftName < rightName;
		elseif ( leftEntry.distance ~= rightEntry.distance ) then
			return leftEntry.distance < rightEntry.distance;
		else
			return leftEntry.guid < rightEntry.guid;
		end
	else
		if ( leftEntry.distance ~= rightEntry.distance ) then
			isLess = leftEntry.distance < rightEntry.distance;
		elseif ( leftName ~= rightName ) then
			return leftName < rightName;
		else
			return leftEntry.guid < rightEntry.guid;
		end
	end
	if ( sortAscending ) then
		return isLess;
	end
	return not isLess;
end

-- Re-orders the committed list in place, keeping the highlight on whichever creature was selected
local function EQTracking_SortEntries()
	local selectedGuid = nil;
	if ( trackEntries[selectedIndex] ) then
		selectedGuid = trackEntries[selectedIndex].guid;
	end
	table.sort(trackEntries, EQTracking_CompareEntries);
	selectedIndex = 0;
	if ( selectedGuid ) then
		for entryIndex = 1, #trackEntries do
			if ( trackEntries[entryIndex].guid == selectedGuid ) then
				selectedIndex = entryIndex;
				break;
			end
		end
	end
	if ( selectedIndex == 0 and #trackEntries > 0 ) then
		selectedIndex = 1;
	end
end

-- FauxScrollFrame_SetOffset on its own leaves the scroll bar thumb behind, so drive it from the bar
local function EQTracking_ScrollToTop()
	EQTrackingFrameScrollFrameScrollBar:SetValue(0);
	FauxScrollFrame_SetOffset(EQTrackingFrameScrollFrame, 0);
end

-- ===================================================================================
-- Column headers (click to sort on that column, click it again to reverse the order)
-- ===================================================================================

local nameHeader = CreateFrame("Button", "EQTrackingFrameNameHeader", EQTrackingFrame, "WhoFrameColumnHeaderTemplate");
nameHeader:SetPoint("TOPLEFT", EQTrackingFrame, "TOPLEFT", 20, -38);
nameHeader:SetText("Name");
WhoFrameColumn_SetWidth(nameHeader, 200);
nameHeader.sortColumn = SORT_COLUMN_NAME;

local distanceHeader = CreateFrame("Button", "EQTrackingFrameDistanceHeader", EQTrackingFrame, "WhoFrameColumnHeaderTemplate");
distanceHeader:SetPoint("TOPLEFT", EQTrackingFrame, "TOPLEFT", 220, -38);
distanceHeader:SetText("Distance");
WhoFrameColumn_SetWidth(distanceHeader, 80);
distanceHeader.sortColumn = SORT_COLUMN_DISTANCE;

-- The distances themselves are right aligned numbers, so right align their header over them to match
local distanceHeaderText = distanceHeader:GetFontString();
distanceHeaderText:ClearAllPoints();
distanceHeaderText:SetPoint("RIGHT", distanceHeader, "RIGHT", -4, 0);

-- The same 9x8 slice of the sort arrow that the auction house column headers use
local nameHeaderArrow = nameHeader:CreateTexture(nil, "OVERLAY");
nameHeaderArrow:SetTexture("Interface\\Buttons\\UI-SortArrow");
nameHeaderArrow:SetSize(9, 8);
nameHeaderArrow:SetPoint("LEFT", nameHeader:GetFontString(), "RIGHT", 3, -2);
nameHeaderArrow:Hide();

local distanceHeaderArrow = distanceHeader:CreateTexture(nil, "OVERLAY");
distanceHeaderArrow:SetTexture("Interface\\Buttons\\UI-SortArrow");
distanceHeaderArrow:SetSize(9, 8);
distanceHeaderArrow:SetPoint("RIGHT", distanceHeaderText, "LEFT", -3, -2);
distanceHeaderArrow:Hide();

local function EQTracking_UpdateHeaderArrows()
	local activeArrow, inactiveArrow;
	if ( sortColumn == SORT_COLUMN_NAME ) then
		activeArrow = nameHeaderArrow;
		inactiveArrow = distanceHeaderArrow;
	else
		activeArrow = distanceHeaderArrow;
		inactiveArrow = nameHeaderArrow;
	end
	inactiveArrow:Hide();
	-- Unflipped for ascending and flipped vertically for descending, matching the auction house arrows
	if ( sortAscending ) then
		activeArrow:SetTexCoord(0, 0.5625, 0, 1.0);
	else
		activeArrow:SetTexCoord(0, 0.5625, 1.0, 0);
	end
	activeArrow:Show();
end

local function EQTracking_Header_OnClick(self)
	if ( self.sortColumn == sortColumn ) then
		sortAscending = not sortAscending;
	else
		sortColumn = self.sortColumn;
		sortAscending = true;
	end
	-- This replaces the template's own OnClick (which drives the /who window), so replay its sound
	PlaySound("igMainMenuOptionCheckBoxOn");
	EQTracking_UpdateHeaderArrows();
	EQTracking_SortEntries();
	EQTracking_ScrollToTop();
	EQTracking_UpdateList();
end

nameHeader:SetScript("OnClick", EQTracking_Header_OnClick);
distanceHeader:SetScript("OnClick", EQTracking_Header_OnClick);
EQTracking_UpdateHeaderArrows();

-- ===================================================================================
-- List rows (recycled buttons over a faux scroll frame)
-- ===================================================================================

EQTracking_UpdateList = function()
	local scrollFrame = EQTrackingFrameScrollFrame;
	FauxScrollFrame_Update(scrollFrame, #trackEntries, NUM_VISIBLE_ROWS, ROW_HEIGHT);
	local offset = FauxScrollFrame_GetOffset(scrollFrame);

	for rowIndex = 1, NUM_VISIBLE_ROWS do
		local row = rows[rowIndex];
		local entryIndex = rowIndex + offset;
		local entry = trackEntries[entryIndex];
		if ( entry ) then
			local color = GetQuestDifficultyColor(entry.level);
			row.nameText:SetText(entry.name);
			row.nameText:SetTextColor(color.r, color.g, color.b);
			row.distanceText:SetText(entry.distance .. " yd");
			row.entryIndex = entryIndex;
			-- Mark the row of the creature currently being tracked
			if ( trackedGuid and entry.guid == trackedGuid ) then
				row.trackedIcon:Show();
			else
				row.trackedIcon:Hide();
			end
			if ( entryIndex == selectedIndex ) then
				row:LockHighlight();
			else
				row:UnlockHighlight();
			end
			row:Show();
		else
			row.entryIndex = nil;
			row.trackedIcon:Hide();
			row:UnlockHighlight();
			row:Hide();
		end
	end

	if ( #trackEntries == 0 ) then
		emptyText:Show();
	else
		emptyText:Hide();
	end
end

local function EQTracking_StartTrackingSelected()
	local entry = trackEntries[selectedIndex];
	if ( entry and entry.guid ) then
		SendChatMessage(".track start " .. entry.guid, "SAY");
	end
end

local function EQTracking_Row_OnClick(self)
	if ( self.entryIndex ) then
		selectedIndex = self.entryIndex;
		EQTracking_UpdateList();
	end
end

local function EQTracking_Row_OnDoubleClick(self)
	if ( self.entryIndex ) then
		selectedIndex = self.entryIndex;
		EQTracking_UpdateList();
		EQTracking_StartTrackingSelected();
	end
end

local scrollFrame = CreateFrame("ScrollFrame", "EQTrackingFrameScrollFrame", EQTrackingFrame, "FauxScrollFrameTemplate");
scrollFrame:SetPoint("TOPLEFT", EQTrackingFrame, "TOPLEFT", 16, -62);
scrollFrame:SetPoint("BOTTOMRIGHT", EQTrackingFrame, "BOTTOMRIGHT", -38, 70);
scrollFrame:SetScript("OnVerticalScroll", function(self, offset)
	FauxScrollFrame_OnVerticalScroll(self, offset, ROW_HEIGHT, EQTracking_UpdateList);
end);

for rowIndex = 1, NUM_VISIBLE_ROWS do
	local row = CreateFrame("Button", "EQTrackingFrameRow" .. rowIndex, EQTrackingFrame);
	row:SetSize(280, ROW_HEIGHT);
	if ( rowIndex == 1 ) then
		row:SetPoint("TOPLEFT", EQTrackingFrame, "TOPLEFT", 20, -62);
	else
		row:SetPoint("TOPLEFT", rows[rowIndex - 1], "BOTTOMLEFT", 0, 0);
	end
	row:SetHighlightTexture("Interface\\QuestFrame\\UI-QuestTitleHighlight", "ADD");

	-- Marker shown on the row of the creature currently being tracked
	row.trackedIcon = row:CreateTexture(nil, "ARTWORK");
	row.trackedIcon:SetTexture("Interface\\RAIDFRAME\\ReadyCheck-Ready");
	row.trackedIcon:SetSize(14, 14);
	row.trackedIcon:SetPoint("LEFT", row, "LEFT", 0, 0);
	row.trackedIcon:Hide();

	row.nameText = row:CreateFontString(nil, "ARTWORK", "GameFontNormalSmall");
	row.nameText:SetPoint("LEFT", row, "LEFT", 16, 0);
	row.nameText:SetPoint("RIGHT", row, "RIGHT", -84, 0);
	row.nameText:SetJustifyH("LEFT");

	row.distanceText = row:CreateFontString(nil, "ARTWORK", "GameFontDisableSmall");
	row.distanceText:SetPoint("RIGHT", row, "RIGHT", -4, 0);
	row.distanceText:SetJustifyH("RIGHT");

	row:SetScript("OnClick", EQTracking_Row_OnClick);
	row:SetScript("OnDoubleClick", EQTracking_Row_OnDoubleClick);
	row:Hide();
	rows[rowIndex] = row;
end

-- ===================================================================================
-- Bottom buttons
-- ===================================================================================

local trackButton = CreateFrame("Button", "EQTrackingFrameTrackButton", EQTrackingFrame, "UIPanelButtonTemplate");
trackButton:SetSize(90, 22);
trackButton:SetPoint("BOTTOMLEFT", EQTrackingFrame, "BOTTOMLEFT", 16, 18);
trackButton:SetText("Track");
trackButton:SetScript("OnClick", EQTracking_StartTrackingSelected);

local stopButton = CreateFrame("Button", "EQTrackingFrameStopButton", EQTrackingFrame, "UIPanelButtonTemplate");
stopButton:SetSize(110, 22);
stopButton:SetPoint("LEFT", trackButton, "RIGHT", 4, 0);
stopButton:SetText("Stop Tracking");
stopButton:SetScript("OnClick", function() SendChatMessage(".track stop", "SAY"); end);

local refreshButton = CreateFrame("Button", "EQTrackingFrameRefreshButton", EQTrackingFrame, "UIPanelButtonTemplate");
refreshButton:SetSize(90, 22);
refreshButton:SetPoint("LEFT", stopButton, "RIGHT", 4, 0);
refreshButton:SetText("Refresh");
refreshButton:SetScript("OnClick", function() SendChatMessage(".track list", "SAY"); end);

-- ===================================================================================
-- Server message handling
-- ===================================================================================

-- Payload segments are "~" separated, fields "|" separated, and the first field is the record kind:
--   H|<rowCount>|<maxDistance>|<trackedGuid or empty>   starts a new list
--   R|<guid>|<level>|<distance>|<name>   one creature (batched several per message)
--   F   list complete; render and show the window
--   T|<guid or empty>   the creature now being tracked (empty when tracking stopped or the trail was lost)
--   D|<maxDistance>   the player's track range changed (level up)
local function EQTracking_HandlePayload(payload)
	if ( not payload ) then
		return;
	end
	for segment in string.gmatch(payload, "[^~]+") do
		local kind = string.sub(segment, 1, 1);
		if ( kind == "H" ) then
			local _, _, distance, headerTrackedGuid = strsplit("|", segment);
			maxTrackDistance = tonumber(distance) or 0;
			if ( headerTrackedGuid and headerTrackedGuid ~= "" ) then
				trackedGuid = headerTrackedGuid;
			else
				trackedGuid = nil;
			end
			pendingEntries = {};
			receivingList = true;
		elseif ( kind == "R" and receivingList ) then
			local _, guid, level, distance, name = strsplit("|", segment);
			if ( guid and name ) then
				tinsert(pendingEntries, {
					guid = guid,
					level = tonumber(level) or 1,
					distance = tonumber(distance) or 0,
					name = name,
				});
			end
		elseif ( kind == "F" and receivingList ) then
			receivingList = false;
			trackEntries = pendingEntries;
			pendingEntries = {};
			-- A fresh list starts on its first row, whichever column the player is sorted by
			selectedIndex = 0;
			EQTracking_SortEntries();
			if ( maxTrackDistance > 0 ) then
				titleText:SetText("Tracking  (" .. maxTrackDistance .. " yd)");
			else
				titleText:SetText("Tracking");
			end
			EQTracking_ScrollToTop();
			EQTracking_UpdateList();
			EQTrackingFrame:Show();
		elseif ( kind == "T" ) then
			local _, guid = strsplit("|", segment);
			if ( guid and guid ~= "" ) then
				trackedGuid = guid;
			else
				trackedGuid = nil;
			end
			EQTracking_UpdateList();
		elseif ( kind == "D" ) then
			local _, distance = strsplit("|", segment);
			maxTrackDistance = tonumber(distance) or 0;
			if ( maxTrackDistance > 0 ) then
				titleText:SetText("Tracking  (" .. maxTrackDistance .. " yd)");
			else
				titleText:SetText("Tracking");
			end
		end
	end
end

local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQTRACK_PREFIX ) then
		EQTracking_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQTRACK_PREFIX .. "\t") ) then
		EQTracking_HandlePayload(string.gsub(arg1, "^" .. EQTRACK_PREFIX .. "\t", ""));
	end
end);

-- ===================================================================================
-- Slash commands (alternative to casting the Tracking spell)
-- ===================================================================================

SLASH_EQTRACKING1 = "/track";
SLASH_EQTRACKING2 = "/eqtrack";
SlashCmdList["EQTRACKING"] = function(msg)
	msg = string.lower(msg or "");
	if ( msg == "stop" ) then
		SendChatMessage(".track stop", "SAY");
	else
		SendChatMessage(".track list", "SAY");
	end
end;
