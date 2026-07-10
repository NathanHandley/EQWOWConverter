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
    internal class ReferenceLootTemplateSQL : SQLFile
    {

        public override string DeleteRowSQL()
        {
            return "DELETE FROM reference_loot_template WHERE `Entry` >= " + Configuration.SQL_REFERENCE_LOOT_TEMPLATE_ID_START.ToString() + " AND `Entry` <= " + Configuration.SQL_REFERENCE_LOOT_TEMPLATE_ID_END + ";";
        }

        public void AddRow(int id, int itemTemplateID, float chance, bool isFishingJunk, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Entry", id);
            newRow.AddInt("Item", itemTemplateID);
            newRow.AddInt("Reference", 0);
            newRow.AddFloat("Chance", chance);
            newRow.AddInt("QuestRequired", 0);
            if (isFishingJunk == false)
                newRow.AddInt("LootMode", 1); // LOOT_MODE_DEFAULT = 0x01
            else
                newRow.AddInt("LootMode", 32768); // LOOT_MODE_JUNK_FISH = 0x8000
            newRow.AddInt("GroupId", 1);
            newRow.AddInt("MinCount", 1);
            newRow.AddInt("MaxCount", 1);
            newRow.AddString("Comment", 255, comment);
            Rows.Add(newRow);  
        }
    }
}
