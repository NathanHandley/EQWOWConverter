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

-- Adds an "EverQuest" page to the Interface Options window covering the per character settings that are
-- otherwise only reachable as typed chat commands (".eqface", ".eqshowbardpulse", ".eqhidewowgear",
-- ".eqhailwindow" and ".eqdispelmessage").  The server (mod-everquest) owns every one of these, so this page
-- never stores them itself: it shows what the server last pushed under the EQOPTIONS prefix, and clicking
-- Okay runs the same chat commands a player would have typed.  Only settings that actually changed are sent,
-- so opening the page and closing it again is silent.
--
-- The one exception is the class aura icon setting, which is purely a matter of what this client draws and so
-- lives in this addon's per character saved variables (EQ_OptionsDB) and never touches the server.
local EQOPTIONS_PREFIX = "EQOPTIONS";

local DEFAULT_FACE_ID = 0;
local DEFAULT_SHOW_BARD_PULSE = true;
local DEFAULT_HIDE_WOW_GEAR = false;
local DEFAULT_HAIL_WINDOW = false;
local DEFAULT_SHOW_CLASS_AURA_ICONS = true;
local DEFAULT_SHOW_DISPEL_MESSAGE = false;
local DEFAULT_DISPEL_COLOR = 0xFFAA00;

-- What the server last told us this character is set to
local serverValues = {
	faceID = DEFAULT_FACE_ID,
	maxFaceID = 0,
	showBardPulse = DEFAULT_SHOW_BARD_PULSE,
	hideWoWGear = DEFAULT_HIDE_WOW_GEAR,
	hailWindow = DEFAULT_HAIL_WINDOW,
	showDispelMessage = DEFAULT_SHOW_DISPEL_MESSAGE,
	dispelColor = DEFAULT_DISPEL_COLOR,
};

-- What the widgets are currently showing, which only reaches the server (or the saved variables) on Okay
local pendingValues = {};

-- ===================================================================================
-- Client side settings (saved per character, never sent to the server)
-- ===================================================================================

-- The saved table may not exist yet (first run, or before the client has loaded saved variables), so it is
-- filled in on demand.  Whatever the client loads later replaces an empty table made here, which is fine
local function EQ_Options_GetClientSettings()
	if ( EQ_OptionsDB == nil ) then
		EQ_OptionsDB = {};
	end
	if ( EQ_OptionsDB.showClassAuraIcons == nil ) then
		EQ_OptionsDB.showClassAuraIcons = DEFAULT_SHOW_CLASS_AURA_ICONS;
	end
	return EQ_OptionsDB;
end

local haveServerValues = false;

-- The Defaults button runs default() and then refresh(), so refresh has to be told to leave the values that
-- default() just put in place alone that one time instead of pulling the server's values back over them
local defaultsJustApplied = false;

-- ===================================================================================
-- Helpers
-- ===================================================================================

local function EQ_Options_ColorToRGB(color)
	local red = math.floor(color / 65536) % 256;
	local green = math.floor(color / 256) % 256;
	local blue = color % 256;
	return red / 255, green / 255, blue / 255;
end

local function EQ_Options_ClampColorByte(value)
	local byteValue = math.floor((value * 255) + 0.5);
	if ( byteValue < 0 ) then
		byteValue = 0;
	elseif ( byteValue > 255 ) then
		byteValue = 255;
	end
	return byteValue;
end

local function EQ_Options_RGBToColor(red, green, blue)
	return (EQ_Options_ClampColorByte(red) * 65536) + (EQ_Options_ClampColorByte(green) * 256) + EQ_Options_ClampColorByte(blue);
end

local function EQ_Options_ColorToHex(color)
	return string.format("%02X%02X%02X", math.floor(color / 65536) % 256, math.floor(color / 256) % 256, color % 256);
end

local function EQ_Options_CopyServerValuesToPending()
	pendingValues.faceID = serverValues.faceID;
	pendingValues.showBardPulse = serverValues.showBardPulse;
	pendingValues.hideWoWGear = serverValues.hideWoWGear;
	pendingValues.hailWindow = serverValues.hailWindow;
	pendingValues.showClassAuraIcons = EQ_Options_GetClientSettings().showClassAuraIcons;
	pendingValues.showDispelMessage = serverValues.showDispelMessage;
	pendingValues.dispelColor = serverValues.dispelColor;
end

EQ_Options_CopyServerValuesToPending();

-- ===================================================================================
-- Panel
-- ===================================================================================

local panel = CreateFrame("Frame", "EQOptionsPanel", InterfaceOptionsFramePanelContainer);
panel.name = "EverQuest";
panel:Hide();

local titleText = panel:CreateFontString("EQOptionsPanelTitle", "ARTWORK", "GameFontNormalLarge");
titleText:SetPoint("TOPLEFT", panel, "TOPLEFT", 16, -16);
titleText:SetText("EverQuest");

local function EQ_Options_Widget_OnEnter(self)
	if ( self.eqTooltipTitle == nil ) then
		return;
	end
	GameTooltip:SetOwner(self, "ANCHOR_RIGHT");
	GameTooltip:SetText(self.eqTooltipTitle, 1, 1, 1);
	if ( self.eqTooltipDescription ~= nil ) then
		GameTooltip:AddLine(self.eqTooltipDescription, nil, nil, nil, true);
	end
	GameTooltip:Show();
end

local function EQ_Options_Widget_OnLeave()
	GameTooltip:Hide();
end

-- Check buttons -------------------------------------------------------------------

local function EQ_Options_CreateCheckButton(name, anchorTo, offsetY, labelText, tooltipDescription)
	local checkButton = CreateFrame("CheckButton", name, panel, "OptionsCheckButtonTemplate");
	checkButton:SetPoint("TOPLEFT", anchorTo, "BOTTOMLEFT", 0, offsetY);
	local checkButtonText = _G[name .. "Text"];
	if ( checkButtonText ~= nil ) then
		checkButtonText:SetText(labelText);
	end
	checkButton.eqTooltipTitle = labelText;
	checkButton.eqTooltipDescription = tooltipDescription;
	checkButton:SetScript("OnEnter", EQ_Options_Widget_OnEnter);
	checkButton:SetScript("OnLeave", EQ_Options_Widget_OnLeave);
	return checkButton;
end

-- The template's own OnClick is replaced below, so the click sound it would have played is played here instead
local function EQ_Options_PlayCheckButtonSound(checkButton)
	if ( checkButton:GetChecked() ) then
		PlaySound("igMainMenuOptionCheckBoxOn");
	else
		PlaySound("igMainMenuOptionCheckBoxOff");
	end
end

local bardPulseCheckButton = EQ_Options_CreateCheckButton("EQOptionsBardPulseCheckButton", titleText, -14,
	"Show bard song pulse graphics",
	"Bard songs re-cast themselves every few seconds.  When this is off the repeat casts are silent, and only the graphic where a song first starts is shown.  Same as .eqshowbardpulse");
bardPulseCheckButton:SetScript("OnClick", function(self)
	EQ_Options_PlayCheckButtonSound(self);
	pendingValues.showBardPulse = (self:GetChecked() and true or false);
end);

local hideWoWGearCheckButton = EQ_Options_CreateCheckButton("EQOptionsHideWoWGearCheckButton", bardPulseCheckButton, -4,
	"Hide World of Warcraft gear on other players",
	"Replaces WoW equipment on other players with plain EverQuest looks of the same kind, hides anything with no EverQuest equivalent, and suppresses weapon enchant glows.  Only what you see changes, nobody's actual gear does.  Same as .eqhidewowgear");
hideWoWGearCheckButton:SetScript("OnClick", function(self)
	EQ_Options_PlayCheckButtonSound(self);
	pendingValues.hideWoWGear = (self:GetChecked() and true or false);
end);

local hailWindowCheckButton = EQ_Options_CreateCheckButton("EQOptionsHailWindowCheckButton", hideWoWGearCheckButton, -4,
	"Open hail replies on right click",
	"Many EverQuest creatures answer a hail with a line of text and nothing else.  When this is on, right clicking one of those opens its reply in a window instead of attacking it.  Same as .eqhailwindow");
hailWindowCheckButton:SetScript("OnClick", function(self)
	EQ_Options_PlayCheckButtonSound(self);
	pendingValues.hailWindow = (self:GetChecked() and true or false);
end);

local classAuraIconsCheckButton = EQ_Options_CreateCheckButton("EQOptionsClassAuraIconsCheckButton", hailWindowCheckButton, -4,
	"Show class aura icons on the buff bar",
	"Each of your EverQuest classes keeps one permanent buff icon, named after the class, that explains its aura.  Turn this off to hide those icons on your buff bar.  The auras keep working, and the situational buffs they grant (Chi Surge, Burdened Agility and the like) always show.  This one is saved on this computer for this character.");
classAuraIconsCheckButton:SetScript("OnClick", function(self)
	EQ_Options_PlayCheckButtonSound(self);
	pendingValues.showClassAuraIcons = (self:GetChecked() and true or false);
end);

local dispelMessageCheckButton = EQ_Options_CreateCheckButton("EQOptionsDispelMessageCheckButton", classAuraIconsCheckButton, -4,
	"Announce spells dispelled from me",
	"Prints a chat line naming each of your spells that a dispel strips off of you.  Same as .eqdispelmessage");

-- Dispel message color ------------------------------------------------------------

local dispelColorLabel = panel:CreateFontString("EQOptionsDispelColorLabel", "ARTWORK", "GameFontNormal");
dispelColorLabel:SetPoint("TOPLEFT", dispelMessageCheckButton, "BOTTOMLEFT", 30, -8);
dispelColorLabel:SetText("Message color");

-- Built the same way the chat settings build theirs: a white square behind the stock swatch art, which is
-- what actually carries the color by way of its vertex color
local dispelColorSwatch = CreateFrame("Button", "EQOptionsDispelColorSwatch", panel);
dispelColorSwatch:SetSize(16, 16);
dispelColorSwatch:SetPoint("LEFT", dispelColorLabel, "RIGHT", 10, 0);
dispelColorSwatch.eqTooltipTitle = "Message color";
dispelColorSwatch.eqTooltipDescription = "The color the dispel message is printed in.  Same as .eqdispelmessage color";
dispelColorSwatch:SetScript("OnEnter", EQ_Options_Widget_OnEnter);
dispelColorSwatch:SetScript("OnLeave", EQ_Options_Widget_OnLeave);

local dispelColorSwatchBg = dispelColorSwatch:CreateTexture("EQOptionsDispelColorSwatchBg", "BACKGROUND");
dispelColorSwatchBg:SetSize(14, 14);
dispelColorSwatchBg:SetPoint("CENTER", dispelColorSwatch, "CENTER", 0, 0);
dispelColorSwatchBg:SetTexture(1, 1, 1);

dispelColorSwatch:SetNormalTexture("Interface\\ChatFrame\\ChatFrameColorSwatch");

local dispelColorSample = panel:CreateFontString("EQOptionsDispelColorSample", "ARTWORK", "GameFontNormal");
dispelColorSample:SetPoint("LEFT", dispelColorSwatch, "RIGHT", 12, 0);
dispelColorSample:SetText("Your Spell has been dispelled.");

local function EQ_Options_RefreshDispelColorDisplay()
	local red, green, blue = EQ_Options_ColorToRGB(pendingValues.dispelColor);
	local swatchTexture = dispelColorSwatch:GetNormalTexture();
	if ( swatchTexture ~= nil ) then
		swatchTexture:SetVertexColor(red, green, blue);
	end
	dispelColorSample:SetTextColor(red, green, blue);
end

-- The color only matters while the message itself is on, so the row is dimmed when it is off.  It stays
-- clickable on purpose: a button with no disabled art of its own does not read as "greyed out", it reads as gone
local function EQ_Options_RefreshDispelColorEnabled()
	if ( pendingValues.showDispelMessage == true ) then
		dispelColorLabel:SetTextColor(1, 0.82, 0);
		dispelColorSwatch:SetAlpha(1);
		dispelColorSample:SetAlpha(1);
	else
		dispelColorLabel:SetTextColor(0.5, 0.5, 0.5);
		dispelColorSwatch:SetAlpha(0.4);
		dispelColorSample:SetAlpha(0.4);
	end
end

dispelMessageCheckButton:SetScript("OnClick", function(self)
	EQ_Options_PlayCheckButtonSound(self);
	pendingValues.showDispelMessage = (self:GetChecked() and true or false);
	EQ_Options_RefreshDispelColorEnabled();
end);

local function EQ_Options_DispelColorPicker_OnSwatch()
	pendingValues.dispelColor = EQ_Options_RGBToColor(ColorPickerFrame:GetColorRGB());
	EQ_Options_RefreshDispelColorDisplay();
end

local function EQ_Options_DispelColorPicker_OnCancel()
	pendingValues.dispelColor = EQ_Options_RGBToColor(ColorPicker_GetPreviousValues());
	EQ_Options_RefreshDispelColorDisplay();
end

dispelColorSwatch:SetScript("OnClick", function()
	local info = UIDropDownMenu_CreateInfo();
	info.r, info.g, info.b = EQ_Options_ColorToRGB(pendingValues.dispelColor);
	info.swatchFunc = EQ_Options_DispelColorPicker_OnSwatch;
	info.cancelFunc = EQ_Options_DispelColorPicker_OnCancel;
	info.hasOpacity = nil;
	OpenColorPicker(info);
end);

-- Illusion face -------------------------------------------------------------------

local faceSlider = CreateFrame("Slider", "EQOptionsFaceSlider", panel, "OptionsSliderTemplate");
faceSlider:SetPoint("TOPLEFT", dispelColorLabel, "BOTTOMLEFT", 0, -36);
faceSlider:SetWidth(240);
faceSlider:SetMinMaxValues(0, 1);
faceSlider:SetValueStep(1);
faceSlider.eqTooltipTitle = "Illusion face";
faceSlider.eqTooltipDescription = "The face shown on you while you are under an illusion.  Face 0 is the default face, and races with fewer faces than the one picked here use their default.  Same as .eqface";
faceSlider:SetScript("OnEnter", EQ_Options_Widget_OnEnter);
faceSlider:SetScript("OnLeave", EQ_Options_Widget_OnLeave);
_G["EQOptionsFaceSliderLow"]:SetText("0");
_G["EQOptionsFaceSliderHigh"]:SetText("0");
_G["EQOptionsFaceSliderText"]:SetText("Illusion face: 0");

faceSlider:SetScript("OnValueChanged", function(self, value)
	local faceID = math.floor(value + 0.5);
	pendingValues.faceID = faceID;
	_G["EQOptionsFaceSliderText"]:SetText("Illusion face: " .. faceID);
end);

local waitingText = panel:CreateFontString("EQOptionsWaitingText", "ARTWORK", "GameFontRedSmall");
waitingText:SetPoint("TOPLEFT", faceSlider, "BOTTOMLEFT", 0, -14);
waitingText:SetWidth(380);
waitingText:SetJustifyH("LEFT");
waitingText:SetText("Waiting on the server for this character's current settings.");
waitingText:Hide();

-- ===================================================================================
-- Moving the stored values into the widgets
-- ===================================================================================

local function EQ_Options_RefreshPanel()
	bardPulseCheckButton:SetChecked(pendingValues.showBardPulse);
	hideWoWGearCheckButton:SetChecked(pendingValues.hideWoWGear);
	hailWindowCheckButton:SetChecked(pendingValues.hailWindow);
	classAuraIconsCheckButton:SetChecked(pendingValues.showClassAuraIcons);
	dispelMessageCheckButton:SetChecked(pendingValues.showDispelMessage);
	EQ_Options_RefreshDispelColorDisplay();
	EQ_Options_RefreshDispelColorEnabled();

	-- A server that reported no illusion faces at all leaves nothing to pick between
	local maxFaceID = serverValues.maxFaceID;
	if ( maxFaceID < 1 ) then
		faceSlider:SetMinMaxValues(0, 1);
		faceSlider:SetValue(0);
		faceSlider:Disable();
		_G["EQOptionsFaceSliderHigh"]:SetText("0");
		_G["EQOptionsFaceSliderText"]:SetText("Illusion face: 0");
	else
		faceSlider:Enable();
		faceSlider:SetMinMaxValues(0, maxFaceID);
		_G["EQOptionsFaceSliderHigh"]:SetText(tostring(maxFaceID));
		faceSlider:SetValue(pendingValues.faceID);
		_G["EQOptionsFaceSliderText"]:SetText("Illusion face: " .. pendingValues.faceID);
	end

	if ( haveServerValues == true ) then
		waitingText:Hide();
	else
		waitingText:Show();
	end
end

-- ===================================================================================
-- Applying, which is nothing more than running the commands a player would have typed
-- ===================================================================================

local function EQ_Options_SendCommand(commandText)
	SendChatMessage(commandText, "SAY");
end

function panel.okay()
	-- The client side setting applies on its own, whether or not the server has answered yet
	local clientSettings = EQ_Options_GetClientSettings();
	if ( pendingValues.showClassAuraIcons ~= clientSettings.showClassAuraIcons ) then
		clientSettings.showClassAuraIcons = pendingValues.showClassAuraIcons;
		BuffFrame_Update();
	end

	-- Without the server's values there is nothing to compare against, so nothing can be known to have changed
	if ( haveServerValues == false ) then
		return;
	end

	if ( pendingValues.showBardPulse ~= serverValues.showBardPulse ) then
		EQ_Options_SendCommand(".eqshowbardpulse " .. (pendingValues.showBardPulse == true and "on" or "off"));
	end
	if ( pendingValues.hideWoWGear ~= serverValues.hideWoWGear ) then
		EQ_Options_SendCommand(".eqhidewowgear " .. (pendingValues.hideWoWGear == true and "on" or "off"));
	end
	if ( pendingValues.hailWindow ~= serverValues.hailWindow ) then
		EQ_Options_SendCommand(".eqhailwindow " .. (pendingValues.hailWindow == true and "on" or "off"));
	end
	if ( pendingValues.showDispelMessage ~= serverValues.showDispelMessage ) then
		EQ_Options_SendCommand(".eqdispelmessage " .. (pendingValues.showDispelMessage == true and "on" or "off"));
	end
	if ( pendingValues.dispelColor ~= serverValues.dispelColor ) then
		EQ_Options_SendCommand(".eqdispelmessage color " .. EQ_Options_ColorToHex(pendingValues.dispelColor));
	end
	if ( pendingValues.faceID ~= serverValues.faceID ) then
		EQ_Options_SendCommand(".eqface " .. pendingValues.faceID);
	end

	-- Nothing is written here.  The server echoes the accepted values back under EQOPTIONS, and that is what
	-- moves serverValues forward, so a command the server turned down leaves the page showing the truth
end

function panel.cancel()
	defaultsJustApplied = false;
	EQ_Options_CopyServerValuesToPending();
	EQ_Options_RefreshPanel();
end

function panel.default()
	pendingValues.faceID = DEFAULT_FACE_ID;
	pendingValues.showBardPulse = DEFAULT_SHOW_BARD_PULSE;
	pendingValues.hideWoWGear = DEFAULT_HIDE_WOW_GEAR;
	pendingValues.hailWindow = DEFAULT_HAIL_WINDOW;
	pendingValues.showClassAuraIcons = DEFAULT_SHOW_CLASS_AURA_ICONS;
	pendingValues.showDispelMessage = DEFAULT_SHOW_DISPEL_MESSAGE;
	pendingValues.dispelColor = DEFAULT_DISPEL_COLOR;
	defaultsJustApplied = true;
	EQ_Options_RefreshPanel();
end

function panel.refresh()
	if ( defaultsJustApplied == true ) then
		defaultsJustApplied = false;
	else
		EQ_Options_CopyServerValuesToPending();
	end
	EQ_Options_RefreshPanel();
end

InterfaceOptions_AddCategory(panel);

-- ===================================================================================
-- Hiding the primary class aura icons on the buff bar
-- ===================================================================================

-- Every EverQuest class's primary aura is named "<Aura name> (<Class name>)", and that suffix is what marks it.  The
-- situational auras under it (Chi Surge, Burdened Agility, Troubadour's Tempo, ...) carry no such suffix and always show
local EQ_CLASS_AURA_CLASS_NAMES = {
	["Enchanter"] = true, ["Bard"] = true, ["Monk"] = true, ["Ranger"] = true, ["Rogue"] = true, ["Paladin"] = true,
	["Shadow Knight"] = true, ["Warrior"] = true, ["Wizard"] = true, ["Magician"] = true, ["Necromancer"] = true,
	["Cleric"] = true, ["Druid"] = true, ["Shaman"] = true,
};

local function EQ_Options_IsPrimaryClassAuraName(auraName)
	if ( auraName == nil ) then
		return false;
	end
	local className = string.match(auraName, "%((.-)%)$");
	if ( className == nil ) then
		return false;
	end
	return EQ_CLASS_AURA_CLASS_NAMES[className] == true;
end

-- The stock AuraButton_Update shows the button for every buff it finds; this runs right after it and hides the ones that
-- are primary class auras when the setting is off.  The button stays created and counted, it is only not drawn
hooksecurefunc("AuraButton_Update", function(buttonName, index, filter)
	if ( filter ~= "HELPFUL" ) then
		return;
	end
	if ( EQ_Options_GetClientSettings().showClassAuraIcons == true ) then
		return;
	end
	local buff = _G[buttonName .. index];
	if ( buff == nil or not buff:IsShown() ) then
		return;
	end
	local auraName = UnitAura(PlayerFrame.unit, index, filter);
	if ( EQ_Options_IsPrimaryClassAuraName(auraName) ) then
		buff:Hide();
	end
end);

-- The stock BuffFrame_UpdateAllBuffAnchors chains every counted button off the previous one, hidden or not, which would
-- leave a gap where a hidden icon sits.  This runs right after it and lays the shown buttons out again with the hidden
-- ones skipped, following the same row and spacing rules
hooksecurefunc("BuffFrame_UpdateAllBuffAnchors", function()
	if ( EQ_Options_GetClientSettings().showClassAuraIcons == true ) then
		return;
	end
	local previousBuff, aboveBuff;
	local numBuffs = 0;
	local slack = BuffFrame.numEnchants;
	if ( BuffFrame.numConsolidated > 0 ) then
		slack = slack + 1;
	end
	for i = 1, BUFF_ACTUAL_DISPLAY do
		local buff = _G["BuffButton" .. i];
		if ( buff ~= nil and not buff.consolidated and buff:IsShown() ) then
			numBuffs = numBuffs + 1;
			local rowIndex = numBuffs + slack;
			buff:ClearAllPoints();
			if ( (rowIndex > 1) and (mod(rowIndex, BUFFS_PER_ROW) == 1) ) then
				if ( rowIndex == BUFFS_PER_ROW + 1 ) then
					buff:SetPoint("TOP", ConsolidatedBuffs, "BOTTOM", 0, -BUFF_ROW_SPACING);
				else
					buff:SetPoint("TOP", aboveBuff, "BOTTOM", 0, -BUFF_ROW_SPACING);
				end
				aboveBuff = buff;
			elseif ( rowIndex == 1 ) then
				buff:SetPoint("TOPRIGHT", BuffFrame, "TOPRIGHT", 0, 0);
			else
				if ( numBuffs == 1 ) then
					if ( BuffFrame.numEnchants > 0 ) then
						buff:SetPoint("TOPRIGHT", "TemporaryEnchantFrame", "TOPLEFT", -5, 0);
					else
						buff:SetPoint("TOPRIGHT", ConsolidatedBuffs, "TOPLEFT", -5, 0);
					end
				else
					buff:SetPoint("RIGHT", previousBuff, "LEFT", -5, 0);
				end
			end
			previousBuff = buff;
		end
	end
end);

-- ===================================================================================
-- Server state
-- ===================================================================================

function EQ_Options_HandlePayload(payload)
	if ( payload == nil ) then
		return;
	end

	local fields = {};
	for field in string.gmatch(payload, "[^\t]+") do
		table.insert(fields, field);
	end
	if ( #fields < 7 ) then
		return;
	end

	serverValues.faceID = tonumber(fields[1]) or DEFAULT_FACE_ID;
	serverValues.maxFaceID = tonumber(fields[2]) or 0;
	serverValues.showBardPulse = (fields[3] == "1");
	serverValues.hideWoWGear = (fields[4] == "1");
	serverValues.hailWindow = (fields[5] == "1");
	serverValues.showDispelMessage = (fields[6] == "1");
	serverValues.dispelColor = tonumber(fields[7], 16) or DEFAULT_DISPEL_COLOR;
	haveServerValues = true;

	-- A push that lands while the page is open is the server's answer to something that was just done there,
	-- so the page follows it rather than holding onto what it was showing
	defaultsJustApplied = false;
	EQ_Options_CopyServerValuesToPending();
	EQ_Options_RefreshPanel();
end

local eventFrame = CreateFrame("Frame", "EQOptionsEventFrame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:SetScript("OnEvent", function(self, event, arg1, arg2)
	-- The server pushes these at login, but asking again on entering the world covers the case where that
	-- message landed before this addon finished loading.  "sync" prints nothing, it only pushes the values back
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		SendChatMessage(".eqoptions sync", "SAY");
		return;
	end
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQOPTIONS_PREFIX ) then
		EQ_Options_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQOPTIONS_PREFIX .. "\t") ) then
		EQ_Options_HandlePayload(string.gsub(arg1, "^" .. EQOPTIONS_PREFIX .. "\t", ""));
	end
end);

EQ_Options_RefreshPanel();

-- ===================================================================================
-- Slash command (alternative to walking the Interface Options window)
-- ===================================================================================

SLASH_EQOPTIONS1 = "/eqoptions";
SLASH_EQOPTIONS2 = "/eqopt";
SlashCmdList["EQOPTIONS"] = function()
	-- Calling this twice is the standard 3.3.5 workaround for the first call only expanding the category list
	InterfaceOptionsFrame_OpenToCategory(panel);
	InterfaceOptionsFrame_OpenToCategory(panel);
end;
