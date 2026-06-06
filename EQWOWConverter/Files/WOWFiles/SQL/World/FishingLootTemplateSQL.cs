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
    internal class FishingLootTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM fishing_loot_template WHERE `Entry` >= " + Configuration.DBCID_AREATABLE_ID_START.ToString() + " AND `Entry` <= " + Configuration.DBCID_AREATABLE_ID_END + ";";
        }

        public void AddRow(int areaID, int referenceID, int itemTemplateID, float chance, bool isJunk, int groupID, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Entry", areaID);
            newRow.AddInt("Item", itemTemplateID);
            newRow.AddInt("Reference", referenceID);
            newRow.AddFloat("Chance", chance);
            newRow.AddInt("QuestRequired", 0);
            if (isJunk == false)
                newRow.AddInt("LootMode", 1); // LOOT_MODE_DEFAULT = 0x01
            else
                newRow.AddInt("LootMode", 32768); // LOOT_MODE_JUNK_FISH = 0x8000
            newRow.AddInt("GroupId", groupID);
            newRow.AddInt("MinCount", 1);
            newRow.AddInt("MaxCount", 1);
            newRow.AddString("Comment", 255, comment);
            Rows.Add(newRow);
        }
    }
}
