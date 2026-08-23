--
-- EQ Spell Tooltips (EQWOW / mod-everquest)
--
-- Adds a live "your spell power adds X" line under the spell power coefficient paragraph that the
-- converter stamps onto every spell-power-scaling EQ spell.  No server or mod support is needed:
-- the coefficient is read back off the tooltip text and the spell power comes from the client's own
-- paperdoll API, so the number re-reads current gear every time a tooltip is drawn.
--
-- The stamped paragraph is written by SpellTemplate.GetSpellPowerCoefficientTooltipTextForBlock and
-- carries the exact direct_bonus / dot_bonus values from the spell's spell_bonus_data row:
--
--   Spell power coefficient: 71% (Frost)
--   Spell power coefficient: 40% per tick (Shadow)
--   Spell power coefficient: 71%, 40% per tick (Fire)
--   Spell power coefficient: 57% (healing)
--
-- The parenthesized tag says which stat the percentages multiply against: a damage school name, or
-- "healing".  EQ gear only ever grants generic spell power (ItemWOWStatType.SpellPower), so the
-- schools normally all read the same, but keying off the real school keeps school-specific WoW gear
-- honest.
--
-- Where it shows: anywhere the client fires OnTooltipSetSpell -- the spellbook, action bars, the pet
-- bar, and spell links in chat.  Buff/debuff tooltips use the spell's aura description instead, which
-- carries no coefficient, so those are left alone.
--

local COEFFICIENT_LABEL = "Spell power coefficient:";

-- Tag in the stamped line -> spell school index used by GetSpellBonusDamage (1 = physical, 2-7 = magic schools)
local SCHOOL_INDEX = {
	["Physical"] = 1,
	["Holy"]     = 2,
	["Fire"]     = 3,
	["Nature"]   = 4,
	["Frost"]    = 5,
	["Shadow"]   = 6,
	["Arcane"]   = 7,
};

local LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B = 0.4, 0.8, 1.0;

-- Pulls the coefficients out of a stamped tooltip line.  Returns directPercent, perTickPercent, tag,
-- any of which is nil when the spell has no such half.
local function EQSpellTooltips_ParseCoefficients(text)
	-- The whole spell description arrives as one font string with embedded newlines, so isolate the
	-- stamped paragraph before matching percentages (a description can carry numbers of its own)
	local segment = text:match(COEFFICIENT_LABEL .. "([^\n]*)");
	if ( not segment ) then
		return nil;
	end

	local tag = segment:match("%(([^%)]+)%)");
	local directPercent, perTickPercent;
	-- Each entry is "<number>%" optionally followed by "per tick", entries separated by commas
	for percent, suffix in segment:gmatch("([%d%.]+)%%([^,%(]*)") do
		if ( suffix:find("per tick") ) then
			perTickPercent = tonumber(percent);
		else
			directPercent = tonumber(percent);
		end
	end
	if ( not directPercent and not perTickPercent ) then
		return nil;
	end
	return directPercent, perTickPercent, tag;
end

-- The spell power the stamped percentages multiply against, matching what the core uses
-- (Unit::SpellBaseDamageBonusDone for a school, Unit::SpellBaseHealingBonusDone for healing)
local function EQSpellTooltips_GetSpellPower(tag)
	if ( tag == "healing" ) then
		return GetSpellBonusHealing() or 0, "healing";
	end
	return GetSpellBonusDamage(SCHOOL_INDEX[tag] or 2) or 0, "damage";
end

local function EQSpellTooltips_BuildAddedText(spellPower, directPercent, perTickPercent, noun)
	local directAmount = directPercent and floor((spellPower * directPercent / 100) + 0.5);
	local perTickAmount = perTickPercent and floor((spellPower * perTickPercent / 100) + 0.5);

	if ( directAmount and perTickAmount ) then
		return format("%d %s and %d per tick", directAmount, noun, perTickAmount);
	elseif ( perTickAmount ) then
		return format("%d %s per tick", perTickAmount, noun);
	end
	return format("%d %s", directAmount, noun);
end

local function EQSpellTooltips_AddSpellPowerLine(tooltip)
	-- OnTooltipSetSpell can re-fire on an already-populated tooltip, so only append once per draw
	if ( tooltip.eqSpellPowerLineAdded ) then
		return;
	end
	local name = tooltip:GetName();
	if ( not name ) then
		return;
	end

	for i = 1, tooltip:NumLines() do
		local fontString = _G[name .. "TextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text and text:find(COEFFICIENT_LABEL, 1, true) ) then
			local directPercent, perTickPercent, tag = EQSpellTooltips_ParseCoefficients(text);
			if ( directPercent or perTickPercent ) then
				local spellPower, noun = EQSpellTooltips_GetSpellPower(tag);
				local addedText = EQSpellTooltips_BuildAddedText(spellPower, directPercent, perTickPercent, noun);
				-- Flag before showing: Show() re-enters this through the OnShow hook below
				tooltip.eqSpellPowerLineAdded = true;
				tooltip:AddLine(format("Your %d spell power adds %s", spellPower, addedText),
					LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B);
				tooltip:Show();
			end
			return;
		end
	end
end

local function EQSpellTooltips_Reset(tooltip)
	tooltip.eqSpellPowerLineAdded = nil;
end

-- OnTooltipSetSpell covers the spellbook, action bars and chat links.  OnShow is a backstop for any
-- path that fills the tooltip without raising it; the add is guarded so it still only happens once.
GameTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_AddSpellPowerLine);
GameTooltip:HookScript("OnShow", EQSpellTooltips_AddSpellPowerLine);
GameTooltip:HookScript("OnTooltipCleared", EQSpellTooltips_Reset);
GameTooltip:HookScript("OnHide", EQSpellTooltips_Reset);
ItemRefTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_AddSpellPowerLine);
ItemRefTooltip:HookScript("OnShow", EQSpellTooltips_AddSpellPowerLine);
ItemRefTooltip:HookScript("OnTooltipCleared", EQSpellTooltips_Reset);
ItemRefTooltip:HookScript("OnHide", EQSpellTooltips_Reset);

-- Spell power can move while a tooltip is parked on screen (a gear swap from the character sheet, a
-- proc or buff landing), so redraw the hovered tooltip when it does.  Only action-style buttons carry
-- an UpdateTooltip; everything else simply picks up the new value on the next hover.
local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("PLAYER_DAMAGE_DONE_MODS");
eventFrame:RegisterEvent("UNIT_INVENTORY_CHANGED");
eventFrame:RegisterEvent("UNIT_STATS");
eventFrame:SetScript("OnEvent", function(self, event, arg1)
	if ( arg1 and arg1 ~= "player" ) then
		return;
	end
	if ( not GameTooltip:IsShown() or not GameTooltip.eqSpellPowerLineAdded ) then
		return;
	end
	local owner = GameTooltip:GetOwner();
	if ( owner and owner.UpdateTooltip ) then
		owner:UpdateTooltip();
	end
end);
