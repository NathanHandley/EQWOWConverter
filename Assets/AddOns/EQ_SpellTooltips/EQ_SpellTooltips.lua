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

-- The mana cost half of this addon exists because the client will not apply the server's cost modifier
-- to what it prints.  Intense Healing Exhaustion is a stacking debuff carrying a SPELLMOD_COST aura, and
-- the server does charge the multiplied cost, but the client ignores the SMSG_SET_PCT_SPELL_MODIFIER it
-- is sent (the spells sit in a private SpellFamily that does not match the caster's class family) and
-- keeps printing the base number.  So the converter stamps the per-stack rate onto the caster's tooltip
-- in a fixed shape and the cost printed here is corrected from the stack count on the player:
--
--   Intense Healing Exhaustion: +100% mana cost per stack
--
-- Nothing about the aura name or the rate is hardcoded below -- both are read out of that line, so
-- retuning SPELL_INTENSE_HEALING_EXHAUSTION_MANA_COST_PERCENT_PER_STACK needs no addon change, and neither
-- does adding another spell to the debuff, since the stamp lands on every spell it applies to.  This only
-- corrects printed text: the client still decides on its own whether a button looks affordable, so a
-- cast it thinks you can pay for can still be refused by the server when you are low on mana.

-- Matches "<aura name>: +<N>% mana cost per stack"
local COST_PER_STACK_PATTERN = "([^\n]-): %+(%d+)%% mana cost per stack";

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

-- How many stacks of the named aura the player is carrying right now (0 when it is not up).  Matched by
-- name because that is what the stamped line carries.  The debuff list is contiguous, so a nil name is the end of it.
local function EQSpellTooltips_GetDebuffStacks(auraName)
	for i = 1, 40 do
		local name, _, _, count = UnitDebuff("player", i);
		if ( not name ) then
			return 0;
		end
		if ( name == auraName ) then
			-- A single application reports a count of 0 or 1 depending on the aura
			return (count and count > 0) and count or 1;
		end
	end
	return 0;
end

-- Finds the tooltip's cost line and returns its font string plus the amount and the trailing label
-- ("Mana").  The cost is written to the LEFT column of the second line, with the range sharing that
-- line's right column ("210 Mana" / "33 yd range"), so both columns are scanned for the shape rather
-- than assuming a side.  Requiring the line to start with the number and end in exactly the MANA
-- global keeps it off the description, which says "+100% mana cost per stack" further down.
local function EQSpellTooltips_FindCostLine(tooltip, tooltipName)
	for i = 1, tooltip:NumLines() do
		for _, side in ipairs({ "TextLeft", "TextRight" }) do
			local fontString = _G[tooltipName .. side .. i];
			local text = fontString and fontString:GetText();
			if ( text ) then
				local amount, label = text:match("^([%d,]+)%s+(.+)$");
				if ( amount and label == MANA ) then
					return fontString, tonumber((amount:gsub(",", ""))), label;
				end
			end
		end
	end
	return nil;
end

-- Writes the current stack count onto an already-drawn tooltip.  This edits the existing font strings
-- rather than asking the client to rebuild, because an action button usually has no UpdateTooltip to
-- call (ActionButton_SetTooltip only assigns one when GameTooltip:SetAction reports success), which
-- left every rebuild-based refresh a no-op.  The untouched cost is kept in eqCostBase so repeated
-- renders always scale from the original number instead of compounding.
local function EQSpellTooltips_RenderCost(tooltip, stacks)
	local costFontString = tooltip.eqCostFontString;
	if ( not costFontString ) then
		return;
	end

	local multiplier = 1 + (tooltip.eqCostPercent * stacks / 100);
	costFontString:SetText(format("%d %s", floor((tooltip.eqCostBase * multiplier) + 0.5), tooltip.eqCostLabel));

	-- Blank rather than absent when the debuff falls off mid-hover, since a line cannot be removed from a
	-- drawn tooltip.  The next hover rebuilds from scratch and the gap goes away.
	local noteText = "";
	if ( stacks > 0 ) then
		local multiplierText;
		if ( multiplier == floor(multiplier) ) then
			multiplierText = format("%dx", multiplier);
		else
			multiplierText = format("%.2fx", multiplier);
		end
		noteText = format("%s (%d): %s mana cost", tooltip.eqCostAuraName, stacks, multiplierText);
	end

	local noteFontString = tooltip.eqCostNoteLine and _G[tooltip:GetName() .. "TextLeft" .. tooltip.eqCostNoteLine];
	if ( noteFontString ) then
		noteFontString:SetText(noteText);
	elseif ( noteText ~= "" ) then
		tooltip:AddLine(noteText, LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B);
		tooltip.eqCostNoteLine = tooltip:NumLines();
	end

	tooltip.eqCostDrawnStacks = stacks;
	tooltip:Show();
end

-- Runs once per draw: locates the stamp and the cost line, stashes what a re-render needs, then renders.
local function EQSpellTooltips_ApplyCostPerStack(tooltip)
	if ( tooltip.eqCostLineAdjusted ) then
		return;
	end
	local tooltipName = tooltip:GetName();
	if ( not tooltipName ) then
		return;
	end

	local auraName, percentPerStack;
	for i = 1, tooltip:NumLines() do
		local fontString = _G[tooltipName .. "TextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text ) then
			auraName, percentPerStack = text:match(COST_PER_STACK_PATTERN);
			if ( auraName ) then
				break;
			end
		end
	end
	if ( not auraName ) then
		return;
	end

	local costFontString, baseCost, costLabel = EQSpellTooltips_FindCostLine(tooltip, tooltipName);

	-- Flag before rendering: Show() re-enters this through the OnShow hook below
	tooltip.eqCostLineAdjusted = true;
	if ( not costFontString ) then
		return;
	end

	tooltip.eqCostAuraName = auraName;
	tooltip.eqCostPercent = tonumber(percentPerStack);
	tooltip.eqCostFontString = costFontString;
	tooltip.eqCostBase = baseCost;
	tooltip.eqCostLabel = costLabel;
	EQSpellTooltips_RenderCost(tooltip, EQSpellTooltips_GetDebuffStacks(auraName));
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
	tooltip.eqCostLineAdjusted = nil;
	tooltip.eqCostAuraName = nil;
	tooltip.eqCostDrawnStacks = nil;
	tooltip.eqCostFontString = nil;
	tooltip.eqCostBase = nil;
	tooltip.eqCostLabel = nil;
	tooltip.eqCostPercent = nil;
	tooltip.eqCostNoteLine = nil;
end

-- OnTooltipSetSpell covers the spellbook, action bars and chat links.  OnShow is a backstop for any
-- path that fills the tooltip without raising it; the add is guarded so it still only happens once.
GameTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_AddSpellPowerLine);
GameTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_ApplyCostPerStack);
GameTooltip:HookScript("OnShow", EQSpellTooltips_AddSpellPowerLine);
GameTooltip:HookScript("OnShow", EQSpellTooltips_ApplyCostPerStack);
GameTooltip:HookScript("OnTooltipCleared", EQSpellTooltips_Reset);
GameTooltip:HookScript("OnHide", EQSpellTooltips_Reset);
ItemRefTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_AddSpellPowerLine);
ItemRefTooltip:HookScript("OnTooltipSetSpell", EQSpellTooltips_ApplyCostPerStack);
ItemRefTooltip:HookScript("OnShow", EQSpellTooltips_AddSpellPowerLine);
ItemRefTooltip:HookScript("OnShow", EQSpellTooltips_ApplyCostPerStack);
ItemRefTooltip:HookScript("OnTooltipCleared", EQSpellTooltips_Reset);
ItemRefTooltip:HookScript("OnHide", EQSpellTooltips_Reset);

-- Spell power can move while a tooltip is parked on screen (a gear swap from the character sheet, a
-- proc or buff landing), so redraw the hovered tooltip when it does.  Only action-style buttons carry
-- an UpdateTooltip; everything else simply picks up the new value on the next hover.
local eventFrame = CreateFrame("Frame");
eventFrame:RegisterEvent("PLAYER_DAMAGE_DONE_MODS");
eventFrame:RegisterEvent("UNIT_INVENTORY_CHANGED");
eventFrame:RegisterEvent("UNIT_STATS");
-- A stack landing or expiring changes the printed cost while the tooltip is parked on an action button
eventFrame:RegisterEvent("UNIT_AURA");
eventFrame:SetScript("OnEvent", function(self, event, arg1)
	if ( arg1 and arg1 ~= "player" ) then
		return;
	end
	if ( not GameTooltip:IsShown() ) then
		return;
	end
	if ( not GameTooltip.eqSpellPowerLineAdded and not GameTooltip.eqCostLineAdjusted ) then
		return;
	end
	local owner = GameTooltip:GetOwner();
	if ( owner and owner.UpdateTooltip ) then
		owner:UpdateTooltip();
	end
end);

-- UNIT_AURA above should be enough, but a tooltip parked on a button does not always come back through
-- it, so the stack count is also watched directly.  This costs one table lookup per frame while no
-- stamped spell is hovered: everything else is gated behind eqCostAuraName, which only gets set when a
-- tooltip carrying the stamp is drawn.  While one IS hovered it is five UnitDebuff scans a second, and
-- the tooltip is only rebuilt when the count actually differs from what the current draw used.
local COST_WATCH_INTERVAL = 0.2;
local costWatchElapsed = 0;
eventFrame:SetScript("OnUpdate", function(self, elapsed)
	local auraName = GameTooltip.eqCostAuraName;
	if ( not auraName or not GameTooltip:IsShown() ) then
		return;
	end

	costWatchElapsed = costWatchElapsed + elapsed;
	if ( costWatchElapsed < COST_WATCH_INTERVAL ) then
		return;
	end
	costWatchElapsed = 0;

	if ( EQSpellTooltips_GetDebuffStacks(auraName) == GameTooltip.eqCostDrawnStacks ) then
		return;
	end

	EQSpellTooltips_RenderCost(GameTooltip, EQSpellTooltips_GetDebuffStacks(auraName));
end);
