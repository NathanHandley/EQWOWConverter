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

-- Saves the "Manual Sorting" layout of the spellbook's Norrath tab, per character.  The spellbook lives
-- in FrameXML (SpellBookFrame.lua), which has no saved variables of its own.
--
-- This addon deliberately never hands its table to the spellbook.  Anything read out of an addon's saved
-- variables taints the code path that read it, and a tainted path is not allowed to reach CastSpell, so
-- a spellbook that read this table straight would refuse to cast ("EQ_SpellbookSort has been blocked
-- from an action only available to the Blizzard UI").  Instead the layout is a plain string handed over
-- through EQSpellSortDataHolder, a font string that FrameXML owns: text given to the client and read
-- back out of it comes back untainted.  So all that happens here is parking the saved string on that
-- font string at load, and picking the current one back up whenever the spellbook writes a new one.
--
-- The layout string covers every EQ class pairing the character has arranged, not just the one being
-- played, so switching secondary class does not cost the other classes their tabs.

local function EQ_SpellbookSort_StoreLayout()
	if ( type(EQ_SpellbookSortDB) ~= "table" ) then
		EQ_SpellbookSortDB = {};
	end
	if ( EQSpellSortDataHolder ) then
		EQ_SpellbookSortDB.layout = EQSpellSortDataHolder:GetText() or "";
	end
end

local function EQ_SpellbookSort_OnEvent(self, event, addOnName)
	if ( event == "ADDON_LOADED" ) then
		if ( addOnName ~= "EQ_SpellbookSort" ) then
			return;
		end

		if ( type(EQ_SpellbookSortDB) ~= "table" ) then
			EQ_SpellbookSortDB = {};
		end
		-- Layouts used to be saved as a table; that shape is not read any more
		EQ_SpellbookSortDB.tabs = nil;
		EQ_SpellbookSortDB.enabled = nil;

		if ( EQSpellSortDataHolder and type(EQ_SpellbookSortDB.layout) == "string" ) then
			EQSpellSortDataHolder:SetText(EQ_SpellbookSortDB.layout);
		end

		-- The spellbook calls this every time it writes a new layout out, so the saved copy keeps up with
		-- the arrangement instead of waiting on a logout that a class change may not deliver
		hooksecurefunc("EQSpellSort_OnLayoutPublished", EQ_SpellbookSort_StoreLayout);

		self:UnregisterEvent("ADDON_LOADED");
	elseif ( event == "PLAYER_LOGOUT" ) then
		EQ_SpellbookSort_StoreLayout();
	end
end

local eventFrame = CreateFrame("Frame", "EQ_SpellbookSortEventFrame");
eventFrame:RegisterEvent("ADDON_LOADED");
eventFrame:RegisterEvent("PLAYER_LOGOUT");
eventFrame:SetScript("OnEvent", EQ_SpellbookSort_OnEvent);
