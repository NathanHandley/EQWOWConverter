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
local GREY_TEXT_COLOR_PREFIX = GRAY_FONT_COLOR_CODE or "|cff808080";
local DISABLED_FONT_OBJECT = GameFontDisableSmallLeft;
local TOOLTIP_BODY_COLOR = NORMAL_FONT_COLOR or { r = 1.0, g = 0.82, b = 0.0 };

-- Every menu value this addon owns, so the dropdown hooks below can leave every other menu in the game alone
local EQ_MENTORSHIP_MENU_VALUES = {
	[MENU_ROOT] = true,
	[MENU_MENTOR] = true,
	[MENU_APPRENTICE] = true,
	[MENU_END] = true,
};

-- ===================================================================================
-- Menu entries
-- ===================================================================================

UnitPopupButtons[MENU_ROOT] = { text = "EQ Mentorship", dist = 0, nested = 1 };
UnitPopupButtons[MENU_MENTOR] = { text = "Mentor Them", dist = 0 };
UnitPopupButtons[MENU_APPRENTICE] = { text = "Apprentice To Them", dist = 0 };
UnitPopupButtons[MENU_END] = { text = "End Mentorship", dist = 0 };

-- Every entry is always listed.  One that cannot be used right now is greyed out rather than taken away, so the
-- menu reads the same every time and hovering an entry explains what is missing
UnitPopupMenus[MENU_ROOT] = { MENU_MENTOR, MENU_APPRENTICE, MENU_END };

-- The root entry is the only thing that changes shape, picking up the current role so an active tether can be
-- seen without opening the submenu
function EQ_Mentorship_RefreshMenuEntries()
	if ( currentRole == ROLE_MENTOR ) then
		UnitPopupButtons[MENU_ROOT].text = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Mentor)" .. ACTIVE_COLOR_SUFFIX;
	elseif ( currentRole == ROLE_APPRENTICE ) then
		UnitPopupButtons[MENU_ROOT].text = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Apprentice)" .. ACTIVE_COLOR_SUFFIX;
	elseif ( currentRole == ROLE_ANCHOR ) then
		UnitPopupButtons[MENU_ROOT].text = ACTIVE_COLOR_PREFIX .. "EQ Mentorship (Tethered)" .. ACTIVE_COLOR_SUFFIX;
	else
		UnitPopupButtons[MENU_ROOT].text = "EQ Mentorship";
	end
end

local function EQ_Mentorship_InsertMenuEntryInto(menuName)
	local menu = UnitPopupMenus[menuName];
	if ( menu == nil ) then
		return;
	end
	for index, value in ipairs(menu) do
		if ( value == MENU_ROOT ) then
			return;
		end
	end
	-- Sits just above "Cancel" when there is one, so it never lands between the stock grouping entries
	local insertIndex = #menu + 1;
	for index, value in ipairs(menu) do
		if ( value == "CANCEL" ) then
			insertIndex = index;
			break;
		end
	end
	table.insert(menu, insertIndex, MENU_ROOT);
end

function EQ_Mentorship_InsertMenuEntries()
	EQ_Mentorship_InsertMenuEntryInto("PLAYER");
	EQ_Mentorship_InsertMenuEntryInto("PARTY");
	EQ_Mentorship_InsertMenuEntryInto("RAID_PLAYER");
	EQ_Mentorship_InsertMenuEntryInto("SELF");
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

	-- Kept in step for anything that reads the stock newbie tooltip globals, including UnitPopup itself
	_G["NEWBIE_TOOLTIP_UNIT_" .. MENU_ROOT] = EQ_Mentorship_BuildEntryTooltipText(MENU_ROOT);
	_G["NEWBIE_TOOLTIP_UNIT_" .. MENU_MENTOR] = EQ_Mentorship_BuildEntryTooltipText(MENU_MENTOR);
	_G["NEWBIE_TOOLTIP_UNIT_" .. MENU_APPRENTICE] = EQ_Mentorship_BuildEntryTooltipText(MENU_APPRENTICE);
	_G["NEWBIE_TOOLTIP_UNIT_" .. MENU_END] = EQ_Mentorship_BuildEntryTooltipText(MENU_END);
end

-- Runs from inside UnitPopup_ShowMenu, after it has settled which unit the menu belongs to and before it starts
-- adding buttons, for the outer menu and for the submenu alike
hooksecurefunc("UnitPopup_HideButtons", EQ_Mentorship_UpdateMenuEligibility);

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
	if ( title == nil and UnitPopupButtons[button.value] ~= nil ) then
		title = UnitPopupButtons[button.value].text;
	end
	if ( title == nil ) then
		title = "EQ Mentorship";
	end
	GameTooltip_SetDefaultAnchor(GameTooltip, button);
	GameTooltip:SetText(title, 1.0, 1.0, 1.0);
	GameTooltip:AddLine(tooltipText, TOOLTIP_BODY_COLOR.r, TOOLTIP_BODY_COLOR.g, TOOLTIP_BODY_COLOR.b, 1);
	GameTooltip:Show();
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
	button:HookScript("OnEnter", EQ_Mentorship_ShowEntryTooltip);
	local invisibleButton = _G[buttonName .. "InvisibleButton"];
	if ( invisibleButton ~= nil ) then
		invisibleButton:HookScript("OnEnter", EQ_Mentorship_ShowEntryTooltipFromInvisibleButton);
	end
end

-- UnitPopup builds its submenu entries itself and offers no way to say one of them should be greyed out, so the
-- button is picked up on the way out and greyed here.  The invisible button behind a disabled entry is what keeps
-- the mouse events flowing, which is both what stops the menu timing out and what lets it still be hovered
hooksecurefunc("UIDropDownMenu_AddButton", function(info, level)
	if ( info == nil or info.value == nil or EQ_MENTORSHIP_MENU_VALUES[info.value] == nil ) then
		return;
	end
	-- Anything at all can call this, so the level is not taken on trust: concatenating a non number below would throw
	-- out of a hook that sits on every dropdown in the game
	if ( type(level) ~= "number" ) then
		level = 1;
	end
	local listFrame = _G["DropDownList" .. level];
	if ( listFrame == nil or type(listFrame.numButtons) ~= "number" ) then
		return;
	end
	local button = _G["DropDownList" .. level .. "Button" .. listFrame.numButtons];
	if ( button == nil or button.value ~= info.value ) then
		return;
	end
	EQ_Mentorship_HookEntryTooltip(button);

	if ( menuDisabledReasons[info.value] == nil ) then
		return;
	end
	-- The stock code disables a button *before* it sets its text and its font objects, which is what makes the
	-- disabled font stick.  Greying one after the fact leaves the text drawn in the normal font, so the colour is
	-- put on the string itself.  An inline colour code cannot leak into whatever entry reuses this button later,
	-- because UIDropDownMenu_AddButton sets the text again from scratch every single time
	if ( DISABLED_FONT_OBJECT ~= nil ) then
		button:SetDisabledFontObject(DISABLED_FONT_OBJECT);
	end
	button:Disable();
	local buttonText = button:GetText();
	if ( buttonText ~= nil ) then
		local plainText = string.gsub(buttonText, "|c%x%x%x%x%x%x%x%x", "");
		plainText = string.gsub(plainText, "|r", "");
		button:SetText(GREY_TEXT_COLOR_PREFIX .. plainText .. "|r");
	end
	local invisibleButton = _G[button:GetName() .. "InvisibleButton"];
	if ( invisibleButton ~= nil ) then
		invisibleButton:Show();
	end
	button.tooltipWhileDisabled = 1;
end);

hooksecurefunc("UnitPopup_OnClick", function(self)
	local button = self.value;
	if ( button ~= MENU_MENTOR and button ~= MENU_APPRENTICE and button ~= MENU_END ) then
		return;
	end
	if ( button == MENU_END ) then
		SendChatMessage(".eqmentorship end", "SAY");
		return;
	end

	local targetName = EQ_Mentorship_GetMenuUnitName();
	if ( targetName == nil ) then
		DEFAULT_CHAT_FRAME:AddMessage("Could not tell who that menu was opened on. Target them and type /mentorship mentor instead.", 1.0, 0.3, 0.3);
		return;
	end
	if ( button == MENU_MENTOR ) then
		SendChatMessage(".eqmentorship mentor " .. targetName, "SAY");
	else
		SendChatMessage(".eqmentorship apprentice " .. targetName, "SAY");
	end
end);

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

EQ_Mentorship_InsertMenuEntries();
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
