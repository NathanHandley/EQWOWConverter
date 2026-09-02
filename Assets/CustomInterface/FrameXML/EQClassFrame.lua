--
-- EQ Class character-pane tab (EQWOW / mod-everquest)
--
-- Data is pushed by the server as a hidden addon message (prefix "EQCLASS"), built by
-- EverQuestMod::SendClassInfoAddonMessageToPlayer.  Payload format (after the prefix):
--   H|<baseId>|<baseName>|<currentSecondId>|<nextSecondId>|<expPoolCurrent>|<expPoolMax>
--   ~R|<id>|<name>|<level>|<changecmd>    (one per row: None first, then each eligible secondary)
--
-- Each row has a right-hand column shared by the status text ((ACTIVE)/(PENDING)) and the Switch button
-- so they line up.  A switch is "pending" when the next class differs from the active class; while pending
-- a note and an Undo button appear.  Switch/Undo run ".class change <cmd>"; the server applies the change
-- on next login and pushes a fresh addon message so the UI reflects the new pending/active state.
--
-- A second prefix "EQEXPPOOL" (EverQuestMod::SendExpPoolAddonMessageToPlayer) carries live secondary
-- experience pool updates as "<gainedExp>|<poolCurrent>|<poolMax>".  A non-zero gainedExp means
-- experience was just banked from a kill, which is appended inline to the experience combat message via
-- a CHAT_MSG_COMBAT_XP_GAIN chat filter.  The bottom-of-pane pool section shows current/max and a
-- "Use Exp" button (runs ".class poolspend") floated just to the right of the Stored Exp readout.
--

local EQCLASS_PREFIX = "EQCLASS";
-- Secondary experience pool updates arrive under their own prefix (see EverQuestMod::SendExpPoolAddonMessageToPlayer)
local EQEXPPOOL_PREFIX = "EQEXPPOOL";

local rows = {};

-- Set when an EQEXPPOOL message reports experience was just banked from a kill; consumed by the
-- CHAT_MSG_COMBAT_XP_GAIN chat filter below so the note is appended inline to the experience line.
local pendingPoolGain = nil;

-- Last known pool figures, so a refresh that only carries one value still renders correctly
local expPoolCurrent = 0;
local expPoolMax = 0;

-- Updates the pool readout at the bottom of the pane.  current/max may be nil if not yet known.
local function EQClassFrame_UpdateExpPool(current, max)
	if ( current ) then
		expPoolCurrent = current;
	end
	if ( max ) then
		expPoolMax = max;
	end
	EQClassFrameExpPoolText:SetText("Stored Exp:  |cff4CFF00" .. expPoolCurrent .. "|r / " .. expPoolMax);
end

-- Appends the "(+X exp added to exp pool)" note inline to the experience-gain combat message
local function EQClassFrame_XPGainFilter(self, event, msg, ...)
	if ( pendingPoolGain and pendingPoolGain > 0 ) then
		local newMsg = msg .. "  (+" .. pendingPoolGain .. " exp added to exp pool)";
		pendingPoolGain = nil;
		return false, newMsg, ...;
	end
	return false;
end

local function EQClassFrame_SwitchButton_OnClick(self)
	if ( self.changeCmd ) then
		SendChatMessage(".class change " .. self.changeCmd, "SAY");
	end
end

local function EQClassFrame_UndoButton_OnClick(self)
	if ( self.undoCmd ) then
		SendChatMessage(".class change " .. self.undoCmd, "SAY");
	end
end

local function EQClassFrame_SpendButton_OnClick(self)
	-- Server applies pooled experience to the active character and pushes a refreshed class/pool state
	SendChatMessage(".class poolspend", "SAY");
end

local function EQClassFrame_SpendButton_OnEnter(self)
	GameTooltip:SetOwner(self, "ANCHOR_RIGHT");
	GameTooltip:AddLine("This will consume any available experience on the currently active secondary EQ class up to the amount needed to gain a level", 1.0, 1.0, 1.0, true);
	GameTooltip:Show();
end

local function EQClassFrame_SpendButton_OnLeave(self)
	GameTooltip:Hide();
end

local function EQClassFrame_LogoutButton_OnClick(self)
	-- Same behavior as the main menu "Logout" button
	PlaySound("igMainMenuLogout");
	Logout();
end

local function EQClassFrame_GetRow(index)
	if ( rows[index] ) then
		return rows[index];
	end

	local row = CreateFrame("Frame", "EQClassFrameRow" .. index, EQClassFrame);
	row:SetSize(310, 24);
	if ( index == 1 ) then
		-- Anchored to the frame itself so the class list is independent of the Primary/Secondary label x positions
		row:SetPoint("TOPLEFT", EQClassFrame, "TOPLEFT", 31, -156);
	else
		row:SetPoint("TOPLEFT", rows[index - 1], "BOTTOMLEFT", 0, -2);
	end

	-- Class name (left column)
	row.label = row:CreateFontString(nil, "ARTWORK", "GameFontHighlight");
	row.label:SetPoint("LEFT", row, "LEFT", 0, 0);
	row.label:SetJustifyH("LEFT");
	row.label:SetWidth(130);

	-- Level (its own fixed column so levels line up)
	row.level = row:CreateFontString(nil, "ARTWORK", "GameFontHighlight");
	row.level:SetPoint("LEFT", row, "LEFT", 151, 0);
	row.level:SetJustifyH("LEFT");
	row.level:SetWidth(95);

	-- Shared right-hand column: the Switch button and the status text occupy the same spot
	row.button = CreateFrame("Button", "$parentSwitch", row, "UIPanelButtonTemplate");
	row.button:SetSize(74, 20);
	row.button:SetPoint("RIGHT", row, "RIGHT", 0, 0);
	row.button:SetText("Switch");
	row.button:SetScript("OnClick", EQClassFrame_SwitchButton_OnClick);

	row.status = row:CreateFontString(nil, "ARTWORK", "GameFontNormalSmall");
	row.status:SetPoint("RIGHT", row, "RIGHT", 0, 0);
	row.status:SetWidth(74);
	row.status:SetJustifyH("CENTER");

	rows[index] = row;
	return row;
end

local function EQClassFrame_Render(payload)
	if ( not payload ) then
		return;
	end

	local currentId, nextId;
	local currentCmd, currentName;
	local rowIndex = 0;
	local lastRow;

	local segments = { strsplit("~", payload) };
	for _, segment in ipairs(segments) do
		local kind, f2, f3, f4, f5, f6, f7 = strsplit("|", segment);
		if ( kind == "H" ) then
			currentId = tonumber(f4);
			nextId = tonumber(f5);
			EQClassFramePrimaryText:SetText("Primary EQ Class:  |cff4CFF00" .. (f3 or "") .. "|r");
			-- The spellbook keeps a manual sorting layout per class pairing, so tell it which one is live
			EQSpellSort_SetActiveClass(tonumber(f2), currentId);
			-- f6/f7 carry the secondary experience pool current/max
			EQClassFrame_UpdateExpPool(tonumber(f6), tonumber(f7));
		elseif ( kind == "R" ) then
			rowIndex = rowIndex + 1;
			local id = tonumber(f2);
			local row = EQClassFrame_GetRow(rowIndex);
			local name = f3 or "";
			row.level:SetText("Level " .. (f4 or "1"));
			row.button.changeCmd = f5;

			if ( id == currentId ) then
				currentCmd = f5;
				currentName = name;
				-- Active secondary class: show the class name in green
				row.label:SetText("|cff4CFF00" .. name .. "|r");
				row.status:SetText("|cff4CFF00(ACTIVE)|r");
				row.status:Show();
				row.button:Hide();
			elseif ( id == nextId ) then
				row.label:SetText(name);
				row.status:SetText("|cffFFD100(PENDING)|r");
				row.status:Show();
				row.button:Hide();
			else
				row.label:SetText(name);
				row.status:Hide();
				row.button:Show();
			end
			row:Show();
			lastRow = row;
		end
	end

	-- Write the active secondary class next to its label (green), matching the primary
	EQClassFrameSecondaryHeader:SetText("Secondary EQ Class:  |cff4CFF00" .. (currentName or "None") .. "|r");

	-- Hide any cached rows no longer in use
	for i = rowIndex + 1, #rows do
		rows[i]:Hide();
	end

	-- A switch is pending when the next class differs from the active class
	local pending = (currentId ~= nil) and (nextId ~= nil) and (nextId ~= currentId);
	if ( pending and lastRow ) then
		EQClassFramePendingNote:ClearAllPoints();
		EQClassFramePendingNote:SetPoint("TOPLEFT", lastRow, "BOTTOMLEFT", 0, -16);
		EQClassFramePendingNote:Show();

		EQClassFrameUndoButton.undoCmd = currentCmd;
		EQClassFrameUndoButton:ClearAllPoints();
		EQClassFrameUndoButton:SetPoint("TOPLEFT", EQClassFramePendingNote, "BOTTOMLEFT", 0, -8);
		EQClassFrameUndoButton:Show();

		EQClassFrameLogoutButton:ClearAllPoints();
		EQClassFrameLogoutButton:SetPoint("LEFT", EQClassFrameUndoButton, "RIGHT", 20, 0);
		EQClassFrameLogoutButton:Show();
	else
		EQClassFramePendingNote:Hide();
		EQClassFrameUndoButton:Hide();
		EQClassFrameLogoutButton:Hide();
	end
end

-- Handles an EQEXPPOOL addon message ("<gained>|<current>|<max>").  A non-zero gained value means
-- experience was just banked from a kill, so it is queued for the next experience combat message.
local function EQClassFrame_HandleExpPool(payload)
	if ( not payload ) then
		return;
	end
	local gained, current, max = strsplit("|", payload);
	gained = tonumber(gained);
	if ( gained and gained > 0 ) then
		pendingPoolGain = gained;
	end
	EQClassFrame_UpdateExpPool(tonumber(current), tonumber(max));
end

-- Sends the request to the server for the current class state (silently handled by ".class uiinfo")
local function EQClassFrame_RequestInfo()
	SendChatMessage(".class uiinfo", "SAY");
end

function EQClassFrame_OnLoad(self)
	self:RegisterEvent("CHAT_MSG_ADDON");
	-- Also refresh on entering the world. On first character creation the server pushes the class
	-- state during a teleport to the EQ start zone, before the UI can receive it, so the pane stays
	-- blank until a relog. PLAYER_ENTERING_WORLD fires once the world has finished loading, at which
	-- point we re-request the info so the pane populates without needing a relog.
	self:RegisterEvent("PLAYER_ENTERING_WORLD");

	-- "switch in progress" note shown below the class list while a switch is pending
	local note = self:CreateFontString("EQClassFramePendingNote", "ARTWORK", "GameFontNormalSmall");
	note:SetWidth(312);
	note:SetJustifyH("LEFT");
	note:SetTextColor(1.0, 0.5, 0.0);
	note:SetText("Secondary class switch in progress, please log out and back in to take effect.");
	note:Hide();

	-- Undo button: reverts the pending switch (sets the next class back to the active class)
	local undo = CreateFrame("Button", "EQClassFrameUndoButton", self, "UIPanelButtonTemplate");
	undo:SetSize(80, 22);
	undo:SetText("Undo");
	undo:SetScript("OnClick", EQClassFrame_UndoButton_OnClick);
	undo:Hide();

	-- Logout Now button: same as the main menu Logout, so the pending switch can be applied immediately
	local logout = CreateFrame("Button", "EQClassFrameLogoutButton", self, "UIPanelButtonTemplate");
	logout:SetSize(100, 22);
	logout:SetText("Logout Now");
	logout:SetScript("OnClick", EQClassFrame_LogoutButton_OnClick);
	logout:Hide();

	-- Secondary experience pool section, anchored to the bottom of the pane so it stays clear of the
	-- (variable length) class list above it
	local poolHeader = self:CreateFontString("EQClassFrameExpPoolHeader", "ARTWORK", "GameFontNormal");
	poolHeader:SetPoint("BOTTOMLEFT", self, "BOTTOMLEFT", 31, 112);
	poolHeader:SetJustifyH("LEFT");
	poolHeader:SetText("Secondary Bonus Experience Pool");

	local poolText = self:CreateFontString("EQClassFrameExpPoolText", "ARTWORK", "GameFontHighlight");
	poolText:SetPoint("TOPLEFT", poolHeader, "BOTTOMLEFT", 0, -8);
	poolText:SetJustifyH("LEFT");
	poolText:SetText("Stored Exp:  |cff4CFF000|r / 0");

	-- Button floats inline just to the right of the "Stored Exp:" line, vertically centered on it
	-- (its left edge follows the text's right edge, so it shifts with the readout width)
	local spend = CreateFrame("Button", "EQClassFrameExpPoolSpendButton", self, "UIPanelButtonTemplate");
	spend:SetSize(74, 20);
	spend:SetPoint("LEFT", poolText, "RIGHT", 8, 0);
	spend:SetText("Use Exp");
	spend:SetScript("OnClick", EQClassFrame_SpendButton_OnClick);
	spend:SetScript("OnEnter", EQClassFrame_SpendButton_OnEnter);
	spend:SetScript("OnLeave", EQClassFrame_SpendButton_OnLeave);

	-- Append the "(+X exp added to exp pool)" note inline to the experience-gain combat message
	ChatFrame_AddMessageEventFilter("CHAT_MSG_COMBAT_XP_GAIN", EQClassFrame_XPGainFilter);
end

function EQClassFrame_OnShow(self)
	-- Ask the server for the latest class state (silently handled by the .class uiinfo command)
	EQClassFrame_RequestInfo();
end

function EQClassFrame_OnEvent(self, event, arg1, arg2)
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		-- The frame registers CHAT_MSG_ADDON in OnLoad and renders into its fontstrings even while
		-- hidden, so requesting here populates the pane regardless of whether it is currently open.
		EQClassFrame_RequestInfo();
		return;
	end

	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end

	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQCLASS_PREFIX ) then
		EQClassFrame_Render(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQCLASS_PREFIX .. "\t") ) then
		EQClassFrame_Render(string.gsub(arg1, "^" .. EQCLASS_PREFIX .. "\t", ""));
	elseif ( arg1 == EQEXPPOOL_PREFIX ) then
		EQClassFrame_HandleExpPool(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQEXPPOOL_PREFIX .. "\t") ) then
		EQClassFrame_HandleExpPool(string.gsub(arg1, "^" .. EQEXPPOOL_PREFIX .. "\t", ""));
	end
end
