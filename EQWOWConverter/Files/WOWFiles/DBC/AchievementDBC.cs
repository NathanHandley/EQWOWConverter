//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.


namespace EQWOWConverter.WOWFiles
{
    internal class AchievementDBC : DBCFile
    {
        private static int CATEGORY_FEATS_OF_STRENGTH = 81;

        public void AddRowForFeatOfStrength(int achievementID, string name, string description, int spellIconDBCID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(achievementID); // ID
            newRow.AddInt32(-1); // Faction (-1 = both factions)
            newRow.AddInt32(-1); // Instance_Id (-1 = all maps)
            newRow.AddInt32(0); // Supercedes (previous achievement in a chain)
            newRow.AddStringLang(name); // Title
            newRow.AddStringLang(description); // Description
            newRow.AddInt32(CATEGORY_FEATS_OF_STRENGTH); // Category (Achievement_Category.dbc)
            newRow.AddInt32(0); // Points (feats of strength award no points)
            newRow.AddInt32(0); // UI order
            newRow.AddInt32(0); // Flags
            newRow.AddInt32(spellIconDBCID); // IconID (SpellIcon.dbc)
            newRow.AddStringLang(string.Empty); // Reward text
            newRow.AddInt32(0); // Minimum_Criteria
            newRow.AddInt32(0); // Shares_Criteria
            Rows.Add(newRow);
        }
    }
}
