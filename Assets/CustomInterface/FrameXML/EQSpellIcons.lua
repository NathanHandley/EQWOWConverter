--
-- EverQuest spell icons (EQWOW)
--
-- Every EverQuest spell has two looks.  Spell.dbc carries the spell gem icon (the "Target Aware" look,
-- Spell_EQGem_<icon>_<color>), and EQSpellGenericIcons.lua (written by the converter) maps each spell ID to its
-- generic icon (Interface\Icons\Spell_EQ_<icon>), the plain icon EverQuest shows for the spell.
--
-- Two settings pick which look is shown where.  EQ_Options sets them from its per character saved variables,
-- and these are the defaults whenever it has not (or is not loaded):
--   * EQ_AURA_ICON_TYPE: buffs and debuffs on the player's buff bar and on the target, focus, party and pet frames
--   * EQ_ACTION_ICON_TYPE: action bars (and the pet bar) and the spellbook
--
EQ_SPELL_ICON_TYPE_GENERIC = "generic";
EQ_SPELL_ICON_TYPE_TARGET_AWARE = "targetaware";

EQ_AURA_ICON_TYPE = EQ_SPELL_ICON_TYPE_GENERIC;
EQ_ACTION_ICON_TYPE = EQ_SPELL_ICON_TYPE_TARGET_AWARE;

-- Returns the icon index in a spell gem texture path, or nil for anything that is not a spell gem icon
local function EQSpellIcons_GetSpellGemIconID(texture)
	if ( texture == nil ) then
		return nil;
	end
	return string.match(string.lower(texture), "spell_eqgem_(%d+)_%d+$");
end

-- The generic icon for a spell gem texture.  The spell's own entry wins, as some spells' generic icon differs from
-- the picture on their gem; without a spell ID (a macro, a pet action) the gem's own picture is used.  Anything that
-- is not a spell gem icon comes back untouched.  spellIDFunc is only called for a spell gem, since finding the ID can cost
local function EQSpellIcons_GetGenericTexture(texture, spellID, spellIDFunc, spellIDFuncArg1, spellIDFuncArg2)
	local gemIconID = EQSpellIcons_GetSpellGemIconID(texture);
	if ( gemIconID == nil ) then
		return texture;
	end
	if ( spellID == nil and spellIDFunc ~= nil ) then
		spellID = spellIDFunc(spellIDFuncArg1, spellIDFuncArg2);
	end
	local genericIconID = spellID and EQ_SPELL_GENERIC_ICONS and EQ_SPELL_GENERIC_ICONS[spellID];
	return "Interface\\Icons\\Spell_EQ_"..(genericIconID or gemIconID);
end

-- The texture to show for an aura, given what UnitAura/UnitBuff/UnitDebuff returned for it
function EQSpellIcons_GetAuraTexture(texture, spellID)
	if ( EQ_AURA_ICON_TYPE == EQ_SPELL_ICON_TYPE_TARGET_AWARE ) then
		return texture;
	end
	return EQSpellIcons_GetGenericTexture(texture, spellID);
end

-- The texture to show on an action button, given what GetActionTexture returned for it
function EQSpellIcons_GetActionTexture(texture, action)
	if ( EQ_ACTION_ICON_TYPE == EQ_SPELL_ICON_TYPE_TARGET_AWARE ) then
		return texture;
	end
	return EQSpellIcons_GetGenericTexture(texture, nil, EQSpellIcons_GetActionSpellID, action);
end

-- The texture to show on a pet action button, given what GetPetActionInfo returned for it
function EQSpellIcons_GetPetActionTexture(texture)
	if ( EQ_ACTION_ICON_TYPE == EQ_SPELL_ICON_TYPE_TARGET_AWARE ) then
		return texture;
	end
	return EQSpellIcons_GetGenericTexture(texture);
end

-- The texture to show on a spellbook button, given what GetSpellTexture returned for the slot
function EQSpellIcons_GetSpellBookTexture(texture, slot, bookType)
	if ( EQ_ACTION_ICON_TYPE == EQ_SPELL_ICON_TYPE_TARGET_AWARE ) then
		return texture;
	end
	return EQSpellIcons_GetGenericTexture(texture, nil, EQSpellIcons_GetSpellBookSpellID, slot, bookType);
end

function EQSpellIcons_GetActionSpellID(action)
	if ( action == nil ) then
		return nil;
	end
	local actionType, _, _, spellID = GetActionInfo(action);
	if ( actionType ~= "spell" ) then
		return nil;
	end
	return spellID;
end

function EQSpellIcons_GetSpellBookSpellID(slot, bookType)
	if ( slot == nil ) then
		return nil;
	end
	local spellLink = GetSpellLink(slot, bookType);
	if ( spellLink == nil ) then
		return nil;
	end
	return tonumber(string.match(spellLink, "spell:(%d+)"));
end

-- ===================================================================================
-- Redrawing after a setting changes
-- ===================================================================================

-- Only the icon textures are touched here, never the stock update functions, so nothing on the (secure) unit
-- and action buttons is written to from the options page
local EQ_ACTION_BUTTON_PREFIXES = { "ActionButton", "BonusActionButton", "MultiBarBottomLeftButton", "MultiBarBottomRightButton",
	"MultiBarRightButton", "MultiBarLeftButton" };

local function EQSpellIcons_RefreshActionButtons()
	for _, prefix in ipairs(EQ_ACTION_BUTTON_PREFIXES) do
		for i = 1, NUM_ACTIONBAR_BUTTONS do
			local button = _G[prefix..i];
			local icon = _G[prefix..i.."Icon"];
			if ( button ~= nil and icon ~= nil and button.action ~= nil and HasAction(button.action) ) then
				local texture = GetActionTexture(button.action);
				if ( texture ) then
					icon:SetTexture(EQSpellIcons_GetActionTexture(texture, button.action));
				end
			end
		end
	end
	for i = 1, NUM_PET_ACTION_SLOTS do
		local icon = _G["PetActionButton"..i.."Icon"];
		local _, _, texture, isToken = GetPetActionInfo(i);
		if ( icon ~= nil and texture and not isToken ) then
			icon:SetTexture(EQSpellIcons_GetPetActionTexture(texture));
		end
	end
end

-- Aura button N on these frames always shows aura index N, so each shown one is simply given its icon again
local function EQSpellIcons_RefreshUnitFrameAuras(frameName, unit, suffix, auraFunc)
	if ( _G[frameName] == nil or not UnitExists(unit) ) then
		return;
	end
	local i = 1;
	local auraButton = _G[frameName..suffix..i];
	while ( auraButton ~= nil ) do
		local icon = _G[frameName..suffix..i.."Icon"];
		if ( icon ~= nil and auraButton:IsShown() ) then
			local _, _, texture, _, _, _, _, _, _, _, spellID = auraFunc(unit, i);
			if ( texture ) then
				icon:SetTexture(EQSpellIcons_GetAuraTexture(texture, spellID));
			end
		end
		i = i + 1;
		auraButton = _G[frameName..suffix..i];
	end
end

local function EQSpellIcons_RefreshAuras()
	BuffFrame_Update();
	local unitFrames = { { "TargetFrame", "target" }, { "FocusFrame", "focus" }, { "PetFrame", "pet" },
		{ "TargetofTargetFrame", "targettarget" }, { "TargetofFocusFrame", "focus-target" } };
	for i = 1, MAX_PARTY_MEMBERS do
		tinsert(unitFrames, { "PartyMemberFrame"..i, "party"..i });
		tinsert(unitFrames, { "PartyMemberFrame"..i.."PetFrame", "partypet"..i });
	end
	for _, unitFrame in ipairs(unitFrames) do
		EQSpellIcons_RefreshUnitFrameAuras(unitFrame[1], unitFrame[2], "Buff", UnitBuff);
		EQSpellIcons_RefreshUnitFrameAuras(unitFrame[1], unitFrame[2], "Debuff", UnitDebuff);
	end
end

-- Called by EQ_Options with the two settings.  The spellbook is left alone, as it redraws itself every time it opens
function EQSpellIcons_SetIconTypes(auraIconType, actionIconType)
	local auraChanged = (auraIconType ~= EQ_AURA_ICON_TYPE);
	local actionChanged = (actionIconType ~= EQ_ACTION_ICON_TYPE);
	EQ_AURA_ICON_TYPE = auraIconType;
	EQ_ACTION_ICON_TYPE = actionIconType;
	if ( auraChanged ) then
		EQSpellIcons_RefreshAuras();
	end
	if ( actionChanged ) then
		EQSpellIcons_RefreshActionButtons();
	end
end
