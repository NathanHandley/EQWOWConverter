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
-- Gate spells get the same kind of line naming the bind point they send you to, on the spell's tooltip (spellbook, action
-- bars, links) and on the tooltip of any item whose use effect is a gate.  Which spells gate and where the bind point is
-- come the same way:
--
--   EQTETHER<tab>GATESPELLS<tab><spell ID>,<spell ID>,...   (may arrive in several pieces, each adds to the list)
--   EQTETHER<tab>BIND<tab><area name>                       (a bare "BIND" means there is no bind point in Norrath)
--
-- A bare "EQTETHER" with nothing after it means forget everything, and is sent ahead of the full list that
-- ".eqtether sync" pushes back.  Nothing is stored between sessions, the server is asked again on entering the world.
local EQTETHER_PREFIX = "EQTETHER";

local LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B = 0.3, 1.0, 0.3;

local tetherAreaNamesBySpellID = {};

-- Gate spells by ID for spell tooltips, and by name for item tooltips, which only hand back the use spell's name
local gateSpellIDs = {};
local gateSpellNames = {};

-- nil until the server says, false when there is no bind point in Norrath
local bindAreaName = nil;

local function EQ_Tethers_AddGateSpellIDs(spellIDListText)
	for spellIDText in string.gmatch(spellIDListText or "", "%d+") do
		local spellID = tonumber(spellIDText);
		gateSpellIDs[spellID] = true;
		local spellName = GetSpellInfo(spellID);
		if ( spellName ) then
			gateSpellNames[spellName] = true;
		end
	end
end

function EQ_Tethers_HandlePayload(payload)
	if ( payload == nil or payload == "" ) then
		for spellID in pairs(tetherAreaNamesBySpellID) do
			tetherAreaNamesBySpellID[spellID] = nil;
		end
		for spellID in pairs(gateSpellIDs) do
			gateSpellIDs[spellID] = nil;
		end
		for spellName in pairs(gateSpellNames) do
			gateSpellNames[spellName] = nil;
		end
		bindAreaName = nil;
		return;
	end
	local spellIDText, areaName = strsplit("\t", payload, 2);
	if ( spellIDText == "GATESPELLS" ) then
		EQ_Tethers_AddGateSpellIDs(areaName);
		return;
	end
	if ( spellIDText == "BIND" ) then
		if ( areaName == nil or areaName == "" ) then
			bindAreaName = false;
		else
			bindAreaName = areaName;
		end
		return;
	end
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

-- The bind point line for a gate spell or gate item.  OnTooltipSetSpell can fire more than once on the same draw, so it only goes on once
local function EQ_Tethers_AddBindLine(tooltip)
	if ( tooltip.eqBindLineAdded or bindAreaName == nil ) then
		return;
	end
	tooltip.eqBindLineAdded = true;
	if ( bindAreaName == false ) then
		tooltip:AddLine("You have no bind point in Norrath", LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B, true);
	else
		tooltip:AddLine("Your bind point: " .. bindAreaName, LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B, true);
	end
	tooltip:Show();
end

-- GetSpell hands the ID back directly where the client supports it, and the spell link is the fallback
local function EQ_Tethers_OnTooltipSetSpell(tooltip)
	local name, rank, spellID = tooltip:GetSpell();
	if ( not name ) then
		return;
	end
	if ( not spellID ) then
		local link = (rank and rank ~= "" and GetSpellLink(name .. "(" .. rank .. ")")) or GetSpellLink(name);
		spellID = link and link:match("spell:(%d+)");
	end
	spellID = tonumber(spellID);
	if ( spellID and gateSpellIDs[spellID] ) then
		EQ_Tethers_AddBindLine(tooltip);
	end
end

-- Gate clickies (potions, jewelry and so on) only name their use spell
local function EQ_Tethers_OnTooltipSetItem(tooltip)
	local _, itemLink = tooltip:GetItem();
	if ( not itemLink ) then
		return;
	end
	local useSpellName = GetItemSpell(itemLink);
	if ( useSpellName and gateSpellNames[useSpellName] ) then
		EQ_Tethers_AddBindLine(tooltip);
	end
end

local function EQ_Tethers_OnTooltipCleared(tooltip)
	tooltip.eqBindLineAdded = nil;
end

for _, tooltip in ipairs({ GameTooltip, ItemRefTooltip }) do
	tooltip:HookScript("OnTooltipSetSpell", EQ_Tethers_OnTooltipSetSpell);
	tooltip:HookScript("OnTooltipSetItem", EQ_Tethers_OnTooltipSetItem);
	tooltip:HookScript("OnTooltipCleared", EQ_Tethers_OnTooltipCleared);
	tooltip:HookScript("OnHide", EQ_Tethers_OnTooltipCleared);
end

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
