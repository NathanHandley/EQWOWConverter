--
-- EverQuest blindness (EQWOW)
--
-- While the player has an EverQuest blind on them, the game world goes black but the UI stays up.  The blinds are found
-- by spell ID in the player's debuffs, using EQ_BLIND_SPELL_IDS from EQBlindSpells.lua (written by the converter).
--
-- The black frame hangs off WorldFrame at the lowest frame level of the lowest strata, so it covers the 3D world while
-- every UI frame still draws over it, and hiding the UI (Alt-Z) hides UIParent but not this.  This lives in FrameXML
-- rather than an addon so it can't be switched off from the AddOns list.
--
local EQBlindness_Frame = CreateFrame("Frame", "EQBlindnessFrame", WorldFrame);
EQBlindness_Frame:SetFrameStrata("BACKGROUND");
EQBlindness_Frame:SetFrameLevel(0);
EQBlindness_Frame:SetAllPoints(WorldFrame);
EQBlindness_Frame:EnableMouse(false);

local EQBlindness_Texture = EQBlindness_Frame:CreateTexture(nil, "BACKGROUND");
EQBlindness_Texture:SetAllPoints(EQBlindness_Frame);
EQBlindness_Texture:SetTexture(0, 0, 0, 1);
EQBlindness_Frame:Hide();

-- True when any debuff on the player is an EverQuest blind
local function EQBlindness_IsPlayerBlind()
	if ( EQ_BLIND_SPELL_IDS == nil ) then
		return false;
	end
	local debuffIndex = 1;
	while ( true ) do
		local name, _, _, _, _, _, _, _, _, _, spellID = UnitDebuff("player", debuffIndex);
		if ( name == nil ) then
			return false;
		end
		if ( spellID ~= nil and EQ_BLIND_SPELL_IDS[spellID] == true ) then
			return true;
		end
		debuffIndex = debuffIndex + 1;
	end
end

local function EQBlindness_Update()
	if ( EQBlindness_IsPlayerBlind() == true ) then
		EQBlindness_Frame:Show();
	else
		EQBlindness_Frame:Hide();
	end
end

local function EQBlindness_OnEvent(self, event, unit)
	if ( event == "UNIT_AURA" and unit ~= "player" ) then
		return;
	end
	EQBlindness_Update();
end

local EQBlindness_EventFrame = CreateFrame("Frame");
EQBlindness_EventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
EQBlindness_EventFrame:RegisterEvent("UNIT_AURA");
EQBlindness_EventFrame:SetScript("OnEvent", EQBlindness_OnEvent);
