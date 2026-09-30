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
-- Life-for-mana spells (Cannibalize, Lich/Demi Lich, ...) carry the word "mana" in the same line:
--
--   Spell power coefficient: 50% mana (Shadow)
--   Spell power coefficient: 20% mana per tick (Shadow)
--
-- Those work the way WoW's Life Tap does -- the mana handed back grows with shadow spell power while
-- the life it costs stays flat -- so the percentage still multiplies a damage school, but what it adds
-- is mana.  The life half deliberately carries no coefficient at all, which is why it never appears here.
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
--
-- The same stamp without "per stack" is for an aura that raises the cost once rather than per stack.  The
-- Wizard's Intensified Skyfall toggle is one: it is a buff, not a debuff, and it is stamped on every rain
-- spell so the rain's printed cost follows the toggle:
--
--   Intensified Skyfall: +100% mana cost
--
-- The aura is looked up among both the player's debuffs and buffs, so either kind works.
--
-- A spell can carry more than one cost stamp, and the server adds cost percents together, so the printed
-- cost does the same.
--
-- The Shadow Knight's Spellsword's Focus toggle makes offensive spells with a cast time instant, but only
-- out to melee range.  Every such EQ spell is stamped:
--
--   Spellsword's Focus: instant cast, melee range
--
-- While that toggle is up the cast time line reads as instant and the range line as melee range, since the
-- client knows nothing of either change.  The stamp is only for characters who can turn the toggle on, so
-- it is cut out of the tooltip for everyone else (a spell name only resolves through GetSpellInfo when it is
-- in the player's spellbook).
--
-- Stock WoW spells cannot be stamped, so for those the same rule the server uses is repeated here: a stock
-- spell (ID below the converter's spell range) that is harmful, has a cast time and has no minimum range.
--
-- The action bars get the same treatment for the range indicator (the hotkey text, or the dot when there is
-- no binding): while the toggle is up, an affected spell's button reads its range from a 5 yard melee range
-- item instead of the spell's own range, which is exactly the range entry Bash uses and what the server
-- holds these casts to.

-- Matches "<aura name>: +<N>% mana cost" (or -<N>% for a discount) plus whatever follows on that line (" per stack" for a stacking aura)
local COST_STAMP_PATTERN = "([^\n]-): ([%+%-]%d+)%% mana cost([^\n]*)";
local COST_STAMP_PER_STACK_SUFFIX = " per stack";

-- Matches "<aura name>: instant cast, melee range"
local FOCUS_STAMP_PATTERN = "([^\n]-): instant cast, melee range";
local FOCUS_STAMP_MARKER = ": instant cast, melee range";

-- For stock WoW spells, which carry no stamp.  These mirror the converter's SpellClassAuras.SHADOWKNIGHT_FOCUS_NAME
-- and DBCID_SPELL_ID_START (every converter spell sits at or above it)
local STOCK_FOCUS_AURA_NAME = "Spellsword's Focus";
local CONVERTER_SPELL_ID_START = 86900;

-- Ruby Acorn, whose use spell has the 5 yard melee range entry (SpellRange 2, the one Bash uses).  IsItemInRange works off
-- the item's ID, so it does not need to be carried, only known to the client's item cache
local MELEE_RANGE_ITEM_ID = 37727;
local MELEE_RANGE_TEXT = MELEE_RANGE or "Melee Range";

-- Turns a GlobalStrings format ("%.3g sec cast", "%s yd range") into a whole-line pattern whose number slot takes any number or number range
local function EQSpellTooltips_FormatToLinePattern(formatString)
	if ( not formatString ) then
		return nil;
	end
	local escaped = formatString:gsub("[%^%$%(%)%%%.%[%]%*%+%-%?]", "%%%0");
	escaped = escaped:gsub("%%%%%%%.3g", "[%%d%%.]+");
	escaped = escaped:gsub("%%%%s", "[%%d%%.%%-]+");
	return "^" .. escaped .. "$";
end

local CAST_TIME_LINE_PATTERNS = {
	EQSpellTooltips_FormatToLinePattern(SPELL_CAST_TIME_SEC or "%.3g sec cast"),
	EQSpellTooltips_FormatToLinePattern(SPELL_CAST_TIME_MIN or "%.3g min cast"),
};
local RANGE_LINE_PATTERN = EQSpellTooltips_FormatToLinePattern(SPELL_RANGE or "%s yd range");
local INSTANT_CAST_TEXT = SPELL_CAST_TIME_INSTANT_NO_MANA or "Instant";

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

-- Pulls the coefficients out of a stamped tooltip line.  Returns directPercent, perTickPercent, tag and
-- whether the line restores mana; any of the first three is nil when the spell has no such half.
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
	-- A life-for-mana spell stamps only its mana half, so one check over the whole segment settles the noun
	local isMana = segment:find("mana", 1, true) ~= nil;
	return directPercent, perTickPercent, tag, isMana;
end

-- The spell power the stamped percentages multiply against, matching what the core uses
-- (Unit::SpellBaseDamageBonusDone for a school, Unit::SpellBaseHealingBonusDone for healing).  A mana
-- line reads the same school stat the mod script does, it just hands out mana with it instead of damage
local function EQSpellTooltips_GetSpellPower(tag, isMana)
	if ( isMana ) then
		return GetSpellBonusDamage(SCHOOL_INDEX[tag] or 6) or 0, "mana";
	end
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

-- How many stacks of the named aura the player is carrying in one aura list (0 when it is not there).
-- Matched by name because that is what the stamped line carries.  Each list is contiguous, so a nil name
-- is the end of it.
local function EQSpellTooltips_GetStacksInAuraList(auraName, auraListFunction)
	for i = 1, 40 do
		local name, _, _, count = auraListFunction("player", i);
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

-- How many stacks of the named aura the player is carrying right now, debuff or buff (0 when it is not up)
local function EQSpellTooltips_GetAuraStacks(auraName)
	local stacks = EQSpellTooltips_GetStacksInAuraList(auraName, UnitDebuff);
	if ( stacks > 0 ) then
		return stacks;
	end
	return EQSpellTooltips_GetStacksInAuraList(auraName, UnitBuff);
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

-- Finds the first font string (either column) whose whole text matches one of the patterns
local function EQSpellTooltips_FindLineMatching(tooltip, tooltipName, patterns)
	for i = 1, tooltip:NumLines() do
		for _, side in ipairs({ "TextLeft", "TextRight" }) do
			local fontString = _G[tooltipName .. side .. i];
			local text = fontString and fontString:GetText();
			if ( text ) then
				for _, pattern in ipairs(patterns) do
					if ( pattern and text:match(pattern) ) then
						return fontString, text;
					end
				end
			end
		end
	end
	return nil;
end

local function EQSpellTooltips_FormatMultiplier(multiplier)
	if ( multiplier == floor(multiplier) ) then
		return format("%dx", multiplier);
	end
	return format("%.2fx", multiplier);
end

-- The stack count behind every stamp, joined into one string so a change in any of them is easy to spot
local function EQSpellTooltips_GetStampStacksKey(stamps)
	local parts = {};
	for i, stamp in ipairs(stamps) do
		parts[i] = EQSpellTooltips_GetAuraStacks(stamp.auraName);
	end
	return table.concat(parts, ",");
end

-- The spell a tooltip shows and its ID.  GetSpell hands the ID back directly where the client supports it, and the spell link is the fallback
local function EQSpellTooltips_GetTooltipSpell(tooltip)
	local name, rank, spellID = tooltip:GetSpell();
	if ( not name ) then
		return nil;
	end
	if ( not spellID ) then
		local link = (rank and rank ~= "" and GetSpellLink(name .. "(" .. rank .. ")")) or GetSpellLink(name);
		spellID = link and link:match("spell:(%d+)");
	end
	return name, tonumber(spellID);
end

-- Whether Spellsword's Focus reaches a stock WoW spell: the same rule the server uses (harmful magic, a real cast, no minimum range), for a
-- character who has the toggle.  A channeled spell reports no cast time, which keeps it out
local function EQSpellTooltips_IsStockFocusSpellID(spellID)
	if ( not spellID or spellID >= CONVERTER_SPELL_ID_START or not GetSpellInfo(STOCK_FOCUS_AURA_NAME) ) then
		return false;
	end
	local name = GetSpellInfo(spellID);
	if ( not name or not IsHarmfulSpell(name) ) then
		return false;
	end
	-- The server only takes magic spells, which the client cannot see, so a mana cost stands in for it (keeping out rage and energy abilities with a cast, like Slam)
	local _, _, _, _, _, powerType, castTime, minRange = GetSpellInfo(spellID);
	if ( powerType ~= 0 or not castTime or castTime <= 0 or (minRange and minRange > 0) ) then
		return false;
	end
	return true;
end

-- Writes the current stack counts onto an already-drawn tooltip.  This edits the existing font strings
-- rather than asking the client to rebuild, because an action button usually has no UpdateTooltip to
-- call (ActionButton_SetTooltip only assigns one when GameTooltip:SetAction reports success), which
-- left every rebuild-based refresh a no-op.  The untouched cost, cast time and range are kept on the
-- tooltip so repeated renders always start from the original text instead of compounding.
local function EQSpellTooltips_RenderCost(tooltip)
	local stamps = tooltip.eqCostStamps;
	if ( not stamps ) then
		return;
	end

	-- Percents from different auras add together (as the server's cost modifiers do).  A per-stack stamp scales with the stack count, and any other stamp applies its percent once while the aura is up
	local totalPercent = 0;
	local noteLines = {};
	local isFocusUp = false;
	for _, stamp in ipairs(stamps) do
		local stacks = EQSpellTooltips_GetAuraStacks(stamp.auraName);
		if ( stacks > 0 ) then
			if ( stamp.isFocus ) then
				isFocusUp = true;
			else
				local appliedStacks = stamp.perStack and stacks or 1;
				totalPercent = totalPercent + (stamp.percent * appliedStacks);
				local multiplierText = EQSpellTooltips_FormatMultiplier(1 + (stamp.percent * appliedStacks / 100));
				if ( stamp.perStack ) then
					tinsert(noteLines, format("%s (%d): %s mana cost", stamp.auraName, stacks, multiplierText));
				else
					tinsert(noteLines, format("%s: %s mana cost", stamp.auraName, multiplierText));
				end
			end
		end
	end

	if ( tooltip.eqCostFontString ) then
		local multiplier = 1 + (totalPercent / 100);
		tooltip.eqCostFontString:SetText(format("%d %s", floor((tooltip.eqCostBase * multiplier) + 0.5), tooltip.eqCostLabel));
	end
	if ( tooltip.eqCastTimeFontString ) then
		tooltip.eqCastTimeFontString:SetText(isFocusUp and INSTANT_CAST_TEXT or tooltip.eqCastTimeBaseText);
	end
	if ( tooltip.eqRangeFontString ) then
		tooltip.eqRangeFontString:SetText(isFocusUp and MELEE_RANGE_TEXT or tooltip.eqRangeBaseText);
	end

	-- Blank rather than absent when the aura falls off mid-hover, since a line cannot be removed from a
	-- drawn tooltip.  The next hover rebuilds from scratch and the gap goes away.
	local noteText = table.concat(noteLines, "\n");
	local noteFontString = tooltip.eqCostNoteLine and _G[tooltip:GetName() .. "TextLeft" .. tooltip.eqCostNoteLine];
	if ( noteFontString ) then
		noteFontString:SetText(noteText);
	elseif ( noteText ~= "" ) then
		tooltip:AddLine(noteText, LINE_COLOR_R, LINE_COLOR_G, LINE_COLOR_B);
		tooltip.eqCostNoteLine = tooltip:NumLines();
	end

	tooltip.eqCostDrawnStacksKey = EQSpellTooltips_GetStampStacksKey(stamps);
	tooltip:Show();
end

-- Cuts one stamp line out of a description, together with the blank line in front of it
local function EQSpellTooltips_CutStampLine(text, stampLine)
	local startIndex, endIndex = text:find(stampLine, 1, true);
	if ( not startIndex ) then
		return text;
	end
	while ( startIndex > 1 and text:sub(startIndex - 1, startIndex - 1) == "\n" ) do
		startIndex = startIndex - 1;
	end
	return text:sub(1, startIndex - 1) .. text:sub(endIndex + 1);
end

-- Runs once per draw: locates the stamps and the lines they change, stashes what a re-render needs, then renders.
local function EQSpellTooltips_ApplyCostPerStack(tooltip)
	if ( tooltip.eqCostLineAdjusted ) then
		return;
	end
	local tooltipName = tooltip:GetName();
	if ( not tooltipName ) then
		return;
	end

	local stamps = {};
	local focusStamp;
	for i = 1, tooltip:NumLines() do
		local fontString = _G[tooltipName .. "TextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text ) then
			local keptText = text;
			if ( text:find(" mana cost", 1, true) ) then
				for auraName, percent, stampSuffix in text:gmatch(COST_STAMP_PATTERN) do
					tinsert(stamps, { auraName = auraName, percent = tonumber(percent), perStack = (stampSuffix == COST_STAMP_PER_STACK_SUFFIX) });
				end
			end
			if ( text:find(FOCUS_STAMP_MARKER, 1, true) ) then
				for stampLine, auraName in text:gmatch("(" .. FOCUS_STAMP_PATTERN .. ")") do
					if ( GetSpellInfo(auraName) ) then
						focusStamp = { auraName = auraName, isFocus = true };
					else
						-- A toggle this character cannot use
						keptText = EQSpellTooltips_CutStampLine(keptText, stampLine);
					end
				end
			end
			if ( keptText ~= text ) then
				fontString:SetText(keptText);
			end
		end
	end
	if ( not focusStamp ) then
		local _, spellID = EQSpellTooltips_GetTooltipSpell(tooltip);
		if ( EQSpellTooltips_IsStockFocusSpellID(spellID) ) then
			focusStamp = { auraName = STOCK_FOCUS_AURA_NAME, isFocus = true };
		end
	end

	-- Flag before rendering: Show() re-enters this through the OnShow hook below
	tooltip.eqCostLineAdjusted = true;

	-- The focus only rewrites a spell that shows a cast time, so a channel or an instant spell is never touched
	if ( focusStamp ) then
		tooltip.eqCastTimeFontString, tooltip.eqCastTimeBaseText = EQSpellTooltips_FindLineMatching(tooltip, tooltipName, CAST_TIME_LINE_PATTERNS);
		if ( tooltip.eqCastTimeFontString ) then
			tooltip.eqRangeFontString, tooltip.eqRangeBaseText = EQSpellTooltips_FindLineMatching(tooltip, tooltipName, { RANGE_LINE_PATTERN });
			tinsert(stamps, focusStamp);
		end
	end
	if ( #stamps == 0 ) then
		return;
	end

	local costFontString, baseCost, costLabel = EQSpellTooltips_FindCostLine(tooltip, tooltipName);
	tooltip.eqCostStamps = stamps;
	tooltip.eqCostFontString = costFontString;
	tooltip.eqCostBase = baseCost;
	tooltip.eqCostLabel = costLabel;
	EQSpellTooltips_RenderCost(tooltip);
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
			local directPercent, perTickPercent, tag, isMana = EQSpellTooltips_ParseCoefficients(text);
			if ( directPercent or perTickPercent ) then
				local spellPower, noun = EQSpellTooltips_GetSpellPower(tag, isMana);
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
	tooltip.eqCostStamps = nil;
	tooltip.eqCostDrawnStacksKey = nil;
	tooltip.eqCostFontString = nil;
	tooltip.eqCostBase = nil;
	tooltip.eqCostLabel = nil;
	tooltip.eqCastTimeFontString = nil;
	tooltip.eqCastTimeBaseText = nil;
	tooltip.eqRangeFontString = nil;
	tooltip.eqRangeBaseText = nil;
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
-- it, so the stack counts are also watched directly.  This costs one table lookup per frame while no
-- stamped spell is hovered: everything else is gated behind eqCostStamps, which only gets set when a
-- tooltip carrying a stamp is drawn.  While one IS hovered it is five aura scans a second per stamp, and
-- the tooltip is only redrawn when a count actually differs from what the current draw used.
local COST_WATCH_INTERVAL = 0.2;
local costWatchElapsed = 0;
eventFrame:SetScript("OnUpdate", function(self, elapsed)
	local stamps = GameTooltip.eqCostStamps;
	if ( not stamps or not GameTooltip:IsShown() ) then
		return;
	end

	costWatchElapsed = costWatchElapsed + elapsed;
	if ( costWatchElapsed < COST_WATCH_INTERVAL ) then
		return;
	end
	costWatchElapsed = 0;

	if ( EQSpellTooltips_GetStampStacksKey(stamps) == GameTooltip.eqCostDrawnStacksKey ) then
		return;
	end

	EQSpellTooltips_RenderCost(GameTooltip);
end);

-- Action bar range for Spellsword's Focus.  The stock ActionButton_OnUpdate re-reads IsActionInRange every TOOLTIP_UPDATE_TIME
-- and colors the hotkey text (or shows the range dot) from it, but the client measures that against the spell's own range,
-- so a Focus spell looked castable from anywhere.  This runs right after the stock update, only on the pass that just re-read
-- the range, and repaints the same indicator from the melee range item while the toggle is up.

-- Whether the spell tooltip carries the Focus stamp, read once per spell off a hidden tooltip (the stamp never changes)
local focusStampBySpellID = {};
local scanTooltip = CreateFrame("GameTooltip", "EQSpellTooltipsScanTooltip", nil, "GameTooltipTemplate");

local function EQSpellTooltips_HasFocusStamp(spellID)
	local cached = focusStampBySpellID[spellID];
	if ( cached ~= nil ) then
		return cached;
	end
	scanTooltip:SetOwner(WorldFrame, "ANCHOR_NONE");
	scanTooltip:ClearLines();
	scanTooltip:SetHyperlink("spell:" .. spellID);
	local hasStamp = false;
	for i = 1, scanTooltip:NumLines() do
		local fontString = _G["EQSpellTooltipsScanTooltipTextLeft" .. i];
		local text = fontString and fontString:GetText();
		if ( text and text:find(FOCUS_STAMP_MARKER, 1, true) ) then
			hasStamp = true;
			break;
		end
	end
	scanTooltip:Hide();
	focusStampBySpellID[spellID] = hasStamp;
	return hasStamp;
end

-- The spell an action slot casts, directly or as a macro's spell
local function EQSpellTooltips_GetActionSpellID(action)
	local actionType, id, _, globalID = GetActionInfo(action);
	if ( actionType == "spell" ) then
		if ( globalID ) then
			return tonumber(globalID);
		end
		local link = GetSpellLink(id, BOOKTYPE_SPELL or "spell");
		return tonumber(link and link:match("spell:(%d+)"));
	elseif ( actionType == "macro" ) then
		local name, rank = GetMacroSpell(id);
		if ( not name ) then
			return nil;
		end
		local link = (rank and rank ~= "" and GetSpellLink(name .. "(" .. rank .. ")")) or GetSpellLink(name);
		return tonumber(link and link:match("spell:(%d+)"));
	end
	return nil;
end

-- Whether the toggle is up, kept current from aura events so the per-button check never scans auras
local isFocusActive = false;
local function EQSpellTooltips_RefreshFocusActive()
	isFocusActive = GetSpellInfo(STOCK_FOCUS_AURA_NAME) ~= nil and EQSpellTooltips_GetStacksInAuraList(STOCK_FOCUS_AURA_NAME, UnitBuff) > 0;
end

local focusEventFrame = CreateFrame("Frame");
focusEventFrame:RegisterEvent("PLAYER_ENTERING_WORLD");
focusEventFrame:RegisterEvent("UNIT_AURA");
focusEventFrame:SetScript("OnEvent", function(self, event, arg1)
	if ( event == "UNIT_AURA" and arg1 ~= "player" ) then
		return;
	end
	if ( event == "PLAYER_ENTERING_WORLD" and not GetItemInfo(MELEE_RANGE_ITEM_ID) ) then
		-- IsItemInRange needs the item in the client's cache, and showing its link asks the server for it
		scanTooltip:SetOwner(WorldFrame, "ANCHOR_NONE");
		scanTooltip:SetHyperlink("item:" .. MELEE_RANGE_ITEM_ID);
		scanTooltip:Hide();
	end
	EQSpellTooltips_RefreshFocusActive();
end);

hooksecurefunc("ActionButton_OnUpdate", function(self, elapsed)
	-- Only the pass where the stock code just re-read the range (it resets the timer to exactly this)
	if ( not isFocusActive or not self.action or self.rangeTimer ~= TOOLTIP_UPDATE_TIME ) then
		return;
	end
	if ( not UnitExists("target") or not UnitCanAttack("player", "target") ) then
		return;
	end
	local spellID = EQSpellTooltips_GetActionSpellID(self.action);
	if ( not spellID ) then
		return;
	end
	if ( spellID >= CONVERTER_SPELL_ID_START ) then
		if ( not EQSpellTooltips_HasFocusStamp(spellID) ) then
			return;
		end
	elseif ( not EQSpellTooltips_IsStockFocusSpellID(spellID) ) then
		return;
	end
	local valid = IsItemInRange(MELEE_RANGE_ITEM_ID, "target");
	if ( valid == nil ) then
		return;
	end

	-- The same painting the stock code does
	local hotkey = _G[self:GetName() .. "HotKey"];
	if ( not hotkey ) then
		return;
	end
	if ( hotkey:GetText() == RANGE_INDICATOR ) then
		hotkey:Show();
	end
	if ( valid == 0 ) then
		hotkey:SetVertexColor(1.0, 0.1, 0.1);
	else
		hotkey:SetVertexColor(0.6, 0.6, 0.6);
	end
end);
