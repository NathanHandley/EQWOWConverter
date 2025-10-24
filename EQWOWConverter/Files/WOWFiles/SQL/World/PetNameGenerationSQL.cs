//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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
    internal class PetNameGenerationSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `pet_name_generation` WHERE `entry` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRow(int creatureTemplateEntryID, string wordPart, bool isFirstHalf)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddString("word", wordPart);
            newRow.AddInt("entry", creatureTemplateEntryID);
            if (isFirstHalf == true)
                newRow.AddInt("half", 0);
            else
                newRow.AddInt("half", 1);
            Rows.Add(newRow);
        }
    }
}
