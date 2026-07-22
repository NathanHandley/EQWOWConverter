--
-- EQ Character Stats (EQWOW / mod-everquest)
--
-- Under the EQ class system every class has a mana pool and can cast spells, but the stock
-- character sheet gates all mana-related stats on UnitHasMana(), which only checks the class's
-- native power type from ChrClasses.dbc.  That makes Warriors (rage), Rogues (energy) and
-- Death Knights (runic power) show "N/A" for Mana Regen and lose the mana / spell crit lines
-- from the Intellect and Spirit tooltips, even though the server computes and sends real values.
--
-- Fix: gate on whether the player actually has a mana pool (UnitPowerMax mana > 0) instead.
--   * PaperDollFrame_SetManaRegen is replaced wholesale (it is a plain global called from
--     PaperDollFrame_UpdateStats on every refresh).
--   * PaperDollFrame_SetStat is post-hooked to add the mana-per-intellect / spell-crit-from-
--     intellect and mana-regen-from-spirit tooltip lines the stock code skipped.
--
-- The spell crit and spirit regen numbers these tooltips show come from the client's own
-- gt*.dbc game tables, which the converter patches for the same three classes
-- (DBCFileWorker.CreateGameTableDBCFiles) -- both fixes ship together.
--

local MANA_POWER_TYPE = 0;

local function EQCharacterStats_PlayerHasManaPool()
	return UnitPowerMax("player", MANA_POWER_TYPE) > 0;
end

-- Full replacement of the stock FrameXML function; identical except for the gate.
function PaperDollFrame_SetManaRegen(statFrame)
	_G[statFrame:GetName().."Label"]:SetText(format(STAT_FORMAT, MANA_REGEN));
	local text = _G[statFrame:GetName().."StatText"];
	if ( not UnitHasMana("player") and not EQCharacterStats_PlayerHasManaPool() ) then
		text:SetText(NOT_APPLICABLE);
		statFrame.tooltip = nil;
		return;
	end

	local base, casting = GetManaRegen();
	-- For classes whose native power is not mana, GetManaRegen() reads the active power's regen
	-- slot and returns 0 even though the server computes real mana regen.  Fall back to the
	-- spirit-based rate the client can compute itself (the same uninterrupted-regen formula the
	-- server uses; item/buff mp5 is not visible to the client here and is omitted)
	if ( not UnitHasMana("player") and base < 0.01 ) then
		base = GetUnitManaRegenRateFromSpirit("player");
		casting = 0;
	end
	-- All mana regen stats are displayed as mana/5 sec.
	base = floor( base * 5.0 );
	casting = floor( casting * 5.0 );
	text:SetText(base);
	statFrame.tooltip = HIGHLIGHT_FONT_COLOR_CODE .. MANA_REGEN .. FONT_COLOR_CODE_CLOSE;
	statFrame.tooltip2 = format(MANA_REGEN_TOOLTIP, base, casting);
	statFrame:Show();
end

-- Post-hook: restore the tooltip lines the stock function only builds when UnitHasMana() is true.
-- Stat index 4 is Intellect, 5 is Spirit (matching the stock PaperDollFrame_SetStat).
local EQCharacterStats_BaseSetStat = PaperDollFrame_SetStat;
function PaperDollFrame_SetStat(statFrame, statIndex)
	EQCharacterStats_BaseSetStat(statFrame, statIndex);
	if ( UnitHasMana("player") or not EQCharacterStats_PlayerHasManaPool() ) then
		return;
	end
	if ( statIndex == 4 ) then
		local _, effectiveStat = UnitStat("player", statIndex);
		local baseInt = min(20, effectiveStat);
		local moreInt = effectiveStat - baseInt;
		-- Stock code set tooltip2 to nil for non-mana classes (no pet bonus applies to them either)
		statFrame.tooltip2 = format(_G["DEFAULT_STAT4_TOOLTIP"], baseInt + moreInt*MANA_PER_INTELLECT, GetSpellCritChanceFromIntellect("player"));
	elseif ( statIndex == 5 ) then
		local regen = GetUnitManaRegenRateFromSpirit("player");
		regen = floor( regen * 5.0 );
		statFrame.tooltip2 = (statFrame.tooltip2 or "").."\n"..format(MANA_REGEN_FROM_SPIRIT, regen);
	end
end
