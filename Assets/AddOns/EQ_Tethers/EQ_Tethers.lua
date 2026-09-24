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

-- Shows where a Gate or Hearthstone tether will take you, as an extra line on the tether buff's tooltip.  The buff's own
-- text comes from Spell.dbc and can't carry a place, so the server (mod-everquest) pushes the area name under this prefix
-- whenever a tether is made, keyed by the spell ID of the tether aura:
--
--   EQTETHER<tab><aura spell ID><tab><area name>
--
-- A bare "EQTETHER" with nothing after it means forget everything, and is sent ahead of the full list that
-- ".eqtether sync" pushes back.  Nothing is stored between sessions, the server is asked again on entering the world.
local EQTETHER_PREFIX = "EQTETHER";

local LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B = 0.3, 1.0, 0.3;

local tetherAreaNamesBySpellID = {};

function EQ_Tethers_HandlePayload(payload)
	if ( payload == nil or payload == "" ) then
		for spellID in pairs(tetherAreaNamesBySpellID) do
			tetherAreaNamesBySpellID[spellID] = nil;
		end
		return;
	end
	local spellIDText, areaName = strsplit("\t", payload, 2);
	local spellID = tonumber(spellIDText);
	if ( spellID == nil or areaName == nil or areaName == "" ) then
		return;
	end
	tetherAreaNamesBySpellID[spellID] = areaName;
end

-- Only your own tethers are known, so an aura on anything but the player is left alone
local function EQ_Tethers_AddAreaLine(tooltip, unit, spellID)
	if ( unit == nil or spellID == nil or UnitIsUnit(unit, "player") ~= 1 ) then
		return;
	end
	local areaName = tetherAreaNamesBySpellID[spellID];
	if ( areaName == nil ) then
		return;
	end
	tooltip:AddLine("Returns you to: " .. areaName, LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B, true);
	tooltip:Show();
end

-- The player's buff frame uses SetUnitAura
local function EQ_Tethers_OnSetUnitAura(tooltip, unit, index, filter)
	local spellID = select(11, UnitAura(unit, index, filter));
	EQ_Tethers_AddAreaLine(tooltip, unit, spellID);
end

-- The unit frames (player portrait when targeted, party frames) use SetUnitBuff
local function EQ_Tethers_OnSetUnitBuff(tooltip, unit, index, filter)
	local spellID = select(11, UnitBuff(unit, index, filter));
	EQ_Tethers_AddAreaLine(tooltip, unit, spellID);
end

hooksecurefunc(GameTooltip, "SetUnitAura", EQ_Tethers_OnSetUnitAura);
hooksecurefunc(GameTooltip, "SetUnitBuff", EQ_Tethers_OnSetUnitBuff);

local function EQ_Tethers_OnEvent(self, event, arg1, arg2)
	-- Asking on entering the world covers a tether that was made or loaded while this addon wasn't listening.  "sync" prints nothing
	if ( event == "PLAYER_ENTERING_WORLD" ) then
		SendChatMessage(".eqtether sync", "SAY");
		return;
	end
	if ( event ~= "CHAT_MSG_ADDON" ) then
		return;
	end
	-- 3.3.5 normally delivers (prefix, message); fall back to a tab-split if the client passes them joined
	if ( arg1 == EQTETHER_PREFIX ) then
		EQ_Tethers_HandlePayload(arg2);
	elseif ( arg1 and string.find(arg1, "^" .. EQTETHER_PREFIX .. "\t") ) then
		EQ_Tethers_HandlePayload((string.gsub(arg1, "^" .. EQTETHER_PREFIX .. "\t", "")));
	end
end

local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("CHAT_MSG_ADDON");
eventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
eventFrame:SetScript("OnEvent", EQ_Tethers_OnEvent);
