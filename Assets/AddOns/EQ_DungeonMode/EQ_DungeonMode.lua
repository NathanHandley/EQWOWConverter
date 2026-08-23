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

-- Adds a "Dungeon Mode" submenu to the right click menu on the player portrait, right beside the stock
-- dungeon difficulty and "Reset all instances" entries.  The server (mod-everquest) owns the setting and
-- pushes the current value under this prefix at login and any time it changes; picking an entry here just
-- runs the ".eqdungeonmode" command that the server also accepts as plain chat.
local EQDUNGEONMODE_PREFIX = "EQDUNGEONMODE";

local MENU_ROOT = "EQ_DUNGEONMODE";
local MENU_SHARED = "EQ_DUNGEONMODE_SHARED";
local MENU_INSTANCED = "EQ_DUNGEONMODE_INSTANCED";

local SHARED_TEXT = "Shared World";
local INSTANCED_TEXT = "Instanced";

-- The stock nested menus mark their active entry with a checkmark, but the code that does that is a fixed
-- list inside UnitPopup_ShowMenu with no hook, so the active entry is coloured instead
local ACTIVE_COLOR_PREFIX = "|cff4CFF00";
local ACTIVE_COLOR_SUFFIX = "|r";

local dungeonModeInstanced = false;

-- ===================================================================================
-- Menu entries
-- ===================================================================================

UnitPopupButtons[MENU_ROOT] = { text = "EQ Dungeon Mode", dist = 0, nested = 1 };
UnitPopupButtons[MENU_SHARED] = { text = SHARED_TEXT, dist = 0 };
UnitPopupButtons[MENU_INSTANCED] = { text = INSTANCED_TEXT, dist = 0 };
UnitPopupMenus[MENU_ROOT] = { MENU_SHARED, MENU_INSTANCED };

function EQ_DungeonMode_RefreshMenuText()
	if ( dungeonModeInstanced == true ) then
		UnitPopupButtons[MENU_SHARED].text = SHARED_TEXT;
		UnitPopupButtons[MENU_INSTANCED].text = ACTIVE_COLOR_PREFIX .. INSTANCED_TEXT .. ACTIVE_COLOR_SUFFIX;
	else
		UnitPopupButtons[MENU_SHARED].text = ACTIVE_COLOR_PREFIX .. SHARED_TEXT .. ACTIVE_COLOR_SUFFIX;
		UnitPopupButtons[MENU_INSTANCED].text = INSTANCED_TEXT;
	end
end

-- Sits directly above "Reset all instances", since the two go together
function EQ_DungeonMode_InsertMenuEntry()
	local selfMenu = UnitPopupMenus["SELF"];
	if ( selfMenu == nil ) then
		return;
	end
	for index, value in ipairs(selfMenu) do
		if ( value == MENU_ROOT ) then
			return;
		end
	end
	local insertIndex = #selfMenu;
	for index, value in ipairs(selfMenu) do
		if ( value == "RESET_INSTANCES" ) then
			insertIndex = index;
			break;
		end
	end
	table.insert(selfMenu, insertIndex, MENU_ROOT);
end

hooksecurefunc("UnitPopup_OnClick", function(self)
	local button = self.value;
	if ( button == MENU_SHARED ) then
		SendChatMessage(".eqdungeonmode shared", "SAY");
	elseif ( button == MENU_INSTANCED ) then
		SendChatMessage(".eqdungeonmode instanced", "SAY");
	end
end);

-- ===================================================================================
-- Server state
-- ===================================================================================

function EQ_DungeonMode_HandlePayload(payload)
	if ( payload == nil ) then
		return;
	end
	dungeonModeInstanced = (string.sub(payload, 1, 1) == "1");
	EQ_DungeonMode_RefreshMenuText();
end

local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	-- The server pushes the current mode at login, but asking again on entering the world covers the case where
	-- that message landed before this addon finished loading.  "sync" prints nothing, it only pushes the value back
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		SendChatMessage(".eqdungeonmode sync", "SAY");
		return;
	end
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQDUNGEONMODE_PREFIX ) then
		EQ_DungeonMode_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQDUNGEONMODE_PREFIX .. "\t") ) then
		EQ_DungeonMode_HandlePayload(string.gsub(arg1, "^" .. EQDUNGEONMODE_PREFIX .. "\t", ""));
	end
end);

EQ_DungeonMode_InsertMenuEntry();
EQ_DungeonMode_RefreshMenuText();

-- ===================================================================================
-- Eviction warning wording
-- ===================================================================================

-- When a group breaks up while its members are still inside an instance, the server starts an eviction countdown and the
-- stock client puts up the INSTANCE_BOOT popup, which says the player will be teleported to the nearest graveyard.  Here
-- they are put into the shared world version of the same zone instead, so the wording is corrected to match.  StaticPopup
-- reads this string fresh on every countdown tick, so replacing it on the dialog is enough for it to take effect
INSTANCE_BOOT_TIMER = "You are not in this instance's group.  You will be moved to the shared world version of this zone in %d %s.";

function EQ_DungeonMode_ApplyEvictionWarningText()
	if ( StaticPopupDialogs == nil or StaticPopupDialogs["INSTANCE_BOOT"] == nil ) then
		return;
	end
	StaticPopupDialogs["INSTANCE_BOOT"].text = INSTANCE_BOOT_TIMER;
end

EQ_DungeonMode_ApplyEvictionWarningText();

-- ===================================================================================
-- Slash command (alternative to the portrait menu)
-- ===================================================================================

SLASH_EQDUNGEONMODE1 = "/dungeonmode";
SLASH_EQDUNGEONMODE2 = "/eqdungeonmode";
SlashCmdList["EQDUNGEONMODE"] = function(msg)
	msg = string.lower(msg or "");
	if ( msg == "shared" ) then
		SendChatMessage(".eqdungeonmode shared", "SAY");
	elseif ( msg == "instanced" ) then
		SendChatMessage(".eqdungeonmode instanced", "SAY");
	else
		SendChatMessage(".eqdungeonmode", "SAY");
	end
end;
