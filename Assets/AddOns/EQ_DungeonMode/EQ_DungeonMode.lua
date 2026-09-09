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
--
-- The entries are built by hand rather than registered with UnitPopup.  See the note above ENTRY_TEXT: anything an
-- addon puts into UnitPopupButtons / UnitPopupMenus is read back by the client while it builds the stock right click
-- menu, and that alone is enough to make the client refuse "Set Focus" from that menu afterwards.
local EQDUNGEONMODE_PREFIX = "EQDUNGEONMODE";

local MENU_ROOT = "EQ_DUNGEONMODE";
local MENU_SHARED = "EQ_DUNGEONMODE_SHARED";
local MENU_INSTANCED = "EQ_DUNGEONMODE_INSTANCED";

local SHARED_TEXT = "Shared World";
local INSTANCED_TEXT = "Instanced";

-- The active entry is marked by colouring its text rather than with a checkmark, so that the two entries keep the
-- same left hand alignment as the stock entries around them
local ACTIVE_COLOR_PREFIX = "|cff4CFF00";
local ACTIVE_COLOR_SUFFIX = "|r";

local dungeonModeInstanced = false;

-- ===================================================================================
-- Menu entries
-- ===================================================================================

-- Nothing here is registered in UnitPopupButtons or UnitPopupMenus, and it has to stay that way.  Anything an addon
-- writes into those two tables is read straight back out by UnitPopup_HideButtons and UnitPopup_ShowMenu while the
-- client is building the stock right click menu, and in 3.3.5 reading an addon written value marks that whole build
-- as belonging to the addon.  Every button it goes on to make carries the mark, so clicking one that runs a
-- protected function is refused with "EQ_DungeonMode has been blocked from an action only available to the Blizzard
-- UI".  "Set Focus" is exactly such an entry -- it calls FocusUnit -- which is why the root entry below is added
-- only after the stock menu has finished building, and why the submenu is opened by hand further down rather than
-- by asking for info.hasArrow
local ENTRY_TEXT = {
	[MENU_ROOT] = "EQ Dungeon Mode",
	[MENU_SHARED] = SHARED_TEXT,
	[MENU_INSTANCED] = INSTANCED_TEXT,
};

-- The order the submenu lists them in
local SUBMENU_ENTRY_VALUES = { MENU_SHARED, MENU_INSTANCED };

-- One table, refilled for each entry and handed to UIDropDownMenu_AddButton.  UIDropDownMenu_CreateInfo would do the
-- same job, but the table it hands back is shared with every other addon in the game and this one is not
local entryInfo = {};
local function EQ_DungeonMode_BuildEntryInfo()
	for key in pairs(entryInfo) do
		entryInfo[key] = nil;
	end
	return entryInfo;
end

function EQ_DungeonMode_RefreshMenuText()
	if ( dungeonModeInstanced == true ) then
		ENTRY_TEXT[MENU_SHARED] = SHARED_TEXT;
		ENTRY_TEXT[MENU_INSTANCED] = ACTIVE_COLOR_PREFIX .. INSTANCED_TEXT .. ACTIVE_COLOR_SUFFIX;
	else
		ENTRY_TEXT[MENU_SHARED] = ACTIVE_COLOR_PREFIX .. SHARED_TEXT .. ACTIVE_COLOR_SUFFIX;
		ENTRY_TEXT[MENU_INSTANCED] = INSTANCED_TEXT;
	end
end

-- ===================================================================================
-- Clicking an entry
-- ===================================================================================

-- The root entry only carries the submenu, so clicking it does nothing but close the menu, which the client has
-- already done by the time this runs
local function EQ_DungeonMode_RootEntryOnClick(button)
	return;
end

local function EQ_DungeonMode_SubMenuEntryOnClick(button)
	if ( button == nil ) then
		return;
	end
	local entryValue = button.value;
	CloseDropDownMenus();
	if ( entryValue == MENU_SHARED ) then
		SendChatMessage(".eqdungeonmode shared", "SAY");
	elseif ( entryValue == MENU_INSTANCED ) then
		SendChatMessage(".eqdungeonmode instanced", "SAY");
	end
end

-- ===================================================================================
-- Putting the entry on the menu
-- ===================================================================================

-- The dropdown buttons are shared by every menu in the game and live for the whole session, so each one is hooked at
-- most once and the handler does nothing unless the button is currently showing this addon's root entry
local hookedRootButtons = {};
local function EQ_DungeonMode_HookRootButton(button)
	local buttonName = button:GetName();
	if ( buttonName == nil or hookedRootButtons[buttonName] ~= nil ) then
		return;
	end
	hookedRootButtons[buttonName] = true;
	button:HookScript("OnEnter", function(hoveredButton)
		if ( hoveredButton ~= nil and hoveredButton.value == MENU_ROOT ) then
			EQ_DungeonMode_ShowSubMenu(hoveredButton);
		end
	end);
end

-- The stock expand arrow is a button in its own right, pinned to the right hand edge of the entry, and its OnEnter
-- opens the submenu the client's own way: ToggleDropDownMenu, reading this addon's value straight back out of the
-- entry.  For an entry carrying no info.hasArrow that call finds a level two list anchored to something other than the
-- arrow, hides it, and rebuilds it as the stock submenu for a value the client has never heard of -- and it marks the
-- unit dropdown's own initialize as this addon's on the way through, which is the exact thing the note above
-- ENTRY_TEXT is about.  The arrow sits directly on the path from the entry across to the submenu, so only the picture
-- is wanted here: the mouse is taken off it and the cursor falls through to the entry underneath
local hookedExpandArrows = {};

-- Runs whenever the shared arrow is shown again, including for the stock entries that really do want it clickable
local function EQ_DungeonMode_ExpandArrowOnShow(expandArrow)
	expandArrow:EnableMouse(true);
end

local function EQ_DungeonMode_ShowExpandArrowAsPicture(button)
	local expandArrow = _G[button:GetName() .. "ExpandArrow"];
	if ( expandArrow == nil ) then
		return;
	end
	local arrowName = expandArrow:GetName();
	if ( arrowName ~= nil and hookedExpandArrows[arrowName] == nil ) then
		hookedExpandArrows[arrowName] = true;
		expandArrow:HookScript("OnShow", EQ_DungeonMode_ExpandArrowOnShow);
	end

	-- UIDropDownMenu_AddButton hides the arrow for every entry that did not ask for one, this one included, so showing
	-- it here always goes back through the hook above and the mouse is only ever off it while it belongs to this entry
	expandArrow:Show();
	expandArrow:EnableMouse(false);
end

-- Added once the client has finished building the stock menu, so that none of this addon's own values are ever read
-- by UnitPopup_ShowMenu or UnitPopup_HideButtons.  The stock "Cancel" is taken back off and put on again underneath,
-- so the entry sits at the bottom of the menu without any stock entry being rebuilt here
local function EQ_DungeonMode_AddRootEntry(which)
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

	local info = EQ_DungeonMode_BuildEntryInfo();
	info.text = ENTRY_TEXT[MENU_ROOT];
	info.value = MENU_ROOT;
	info.owner = which;
	info.notCheckable = 1;
	info.func = EQ_DungeonMode_RootEntryOnClick;
	-- The client only widens an entry to make room for the arrow when it was asked for with info.hasArrow, which this
	-- one deliberately never does, so the same ten pixels are reserved by hand instead
	info.padding = 10;
	UIDropDownMenu_AddButton(info, 1);

	-- info.hasArrow is deliberately not asked for: it is what makes the client open the submenu through
	-- ToggleDropDownMenu -> UIDropDownMenu_Initialize, which would mark the unit dropdown's own "initialize" field as
	-- this addon's for the rest of the session and break every protected entry in the menu after that.  The arrow is
	-- only a texture, so it is shown by hand and the submenu is opened from the entry's OnEnter instead
	local rootButton = _G["DropDownList1Button" .. listFrame.numButtons];
	if ( rootButton ~= nil and rootButton.value == MENU_ROOT ) then
		EQ_DungeonMode_ShowExpandArrowAsPicture(rootButton);
		EQ_DungeonMode_HookRootButton(rootButton);
	end

	if ( cancelText ~= nil ) then
		info = EQ_DungeonMode_BuildEntryInfo();
		info.text = cancelText;
		info.value = "CANCEL";
		info.owner = which;
		info.notCheckable = 1;
		info.func = cancelFunc;
		UIDropDownMenu_AddButton(info, 1);
	end
end

-- Runs for every unit menu in the game and at both menu levels; this entry belongs on your own portrait only
hooksecurefunc("UnitPopup_ShowMenu", function(dropdownMenu, which, unit, name, userData)
	if ( UIDROPDOWNMENU_MENU_LEVEL ~= 1 ) then
		return;
	end
	if ( which ~= "SELF" ) then
		return;
	end
	EQ_DungeonMode_AddRootEntry(which);
end);

-- ===================================================================================
-- The submenu
-- ===================================================================================

-- This list is the addon's own frame, and deliberately not the client's second level one (DropDownList2).  That list
-- belongs to the client and everything it does with it assumes the client opened it: every dropdown entry without an
-- arrow closes the level below itself on hover (UIDropDownMenuButtonTemplate's OnEnter calls CloseDropDownMenus), the
-- expand arrow reopens it through ToggleDropDownMenu, any menu opening anywhere in the UI resets it, and EQ_Mentorship
-- wants it for its own submenu on this very menu.  A submenu opened there by hand loses every one of those races and
-- cannot be reached with the cursor.  Nothing in the client's dropdown code can touch this frame:
--   * its ID is above the client's menu levels, so the "CloseDropDownMenus(my level + 1)" that the stock button
--     scripts make is a no op (CloseDropDownMenus only walks up to UIDROPDOWNMENU_MAXLEVELS)
--   * its "parent" field points at the menu it hangs off, so the client's own timeout counting treats hovering this
--     list as hovering that menu and the two second countdown stops, exactly as it would for a stock submenu
-- What still has to be done by hand is closing it: when the menu goes away, and when another entry is hovered
local subMenuList = nil;
local SUBMENU_LIST_ID = UIDROPDOWNMENU_MAXLEVELS + 1;

function EQ_DungeonMode_HideSubMenu()
	if ( subMenuList ~= nil ) then
		subMenuList:Hide();
	end
end

-- Moving onto any other entry closes this, the way the client closes a stock submenu with CloseDropDownMenus.  Runs
-- for every dropdown in the game, and only ever hides a frame of this addon's own
local function EQ_DungeonMode_MenuButtonOnEnter(button)
	if ( button ~= nil and button.value == MENU_ROOT ) then
		return;
	end
	EQ_DungeonMode_HideSubMenu();
end

local function EQ_DungeonMode_GetSubMenuList()
	if ( subMenuList ~= nil ) then
		return subMenuList;
	end

	local parentList = _G["DropDownList1"];
	if ( parentList == nil ) then
		return nil;
	end

	-- The stock list template, so the entries look and behave exactly like the client's own
	subMenuList = CreateFrame("Button", "EQ_DungeonModeSubMenuList", UIParent, "UIDropDownListTemplate");
	subMenuList:SetID(SUBMENU_LIST_ID);
	subMenuList:SetToplevel(true);

	-- A template carries no size of its own (DropDownList1 and 2 get theirs from their own XML), and a frame with no
	-- size has no rect to measure or to draw
	subMenuList:SetWidth(180);
	subMenuList:SetHeight(10);
	subMenuList:Hide();

	-- Whatever takes the menu down takes this with it: a click, the escape key, the menu timing out, or another
	-- menu opening anywhere in the UI (which hides this one first)
	parentList:HookScript("OnHide", EQ_DungeonMode_HideSubMenu);
	for index = 1, UIDROPDOWNMENU_MAXBUTTONS do
		local menuButton = _G["DropDownList1Button" .. index];
		if ( menuButton ~= nil ) then
			menuButton:HookScript("OnEnter", EQ_DungeonMode_MenuButtonOnEnter);
		end
	end

	return subMenuList;
end

-- One entry, laid out the way UIDropDownMenu_AddButton would have laid it out, and handing back the width the client
-- would have worked out for it
local function EQ_DungeonMode_FillSubMenuEntry(listFrame, index, entryValue)
	local buttonName = listFrame:GetName() .. "Button" .. index;
	local button = _G[buttonName];
	local normalText = _G[buttonName .. "NormalText"];
	if ( button == nil or normalText == nil ) then
		return 0;
	end

	button:Enable();
	local invisibleButton = _G[buttonName .. "InvisibleButton"];
	if ( invisibleButton ~= nil ) then
		invisibleButton:Hide();
	end

	button:SetText(ENTRY_TEXT[entryValue]);
	button:SetNormalFontObject(GameFontHighlightSmallLeft);
	button:SetHighlightFontObject(GameFontHighlightSmallLeft);
	normalText:ClearAllPoints();
	normalText:SetPoint("LEFT", button, "LEFT", 0, 0);

	button.value = entryValue;
	button.func = EQ_DungeonMode_SubMenuEntryOnClick;
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

	-- The client's own sum: the text, its padding, less the room a checkbox would have taken
	return normalText:GetWidth() + 40 - 30;
end

-- Opened by hand from the root entry's OnEnter, since the entry deliberately carries no info.hasArrow.  The layout is
-- the level two half of ToggleDropDownMenu, deliberately kept in step with it
local function EQ_DungeonMode_BuildSubMenu(rootButton)
	if ( rootButton == nil ) then
		return;
	end
	local parentList = rootButton:GetParent();
	if ( parentList == nil ) then
		return;
	end
	local listFrame = EQ_DungeonMode_GetSubMenuList();
	if ( listFrame == nil ) then
		return;
	end

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
	if ( entryCount < 1 ) then
		return;
	end
	for index = 1, entryCount do
		local entryWidth = EQ_DungeonMode_FillSubMenuEntry(listFrame, index, SUBMENU_ENTRY_VALUES[index]);
		if ( entryWidth > listFrame.maxWidth ) then
			listFrame.maxWidth = entryWidth;
		end
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
function EQ_DungeonMode_ShowSubMenu(rootButton)
	local succeeded, errorMessage = pcall(EQ_DungeonMode_BuildSubMenu, rootButton);
	if ( succeeded == false and DEFAULT_CHAT_FRAME ~= nil ) then
		DEFAULT_CHAT_FRAME:AddMessage("|cffFF2020EQ Dungeon Mode could not open its menu: " .. tostring(errorMessage) .. "|r");
	end
end

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
