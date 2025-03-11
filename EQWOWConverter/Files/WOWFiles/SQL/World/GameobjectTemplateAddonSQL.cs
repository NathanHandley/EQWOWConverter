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
    internal class GameObjectTemplateAddonSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM gameobject_template_addon WHERE `entry` >= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString() + " AND `entry` <= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END + ";";
        }

        public void AddRowForLiftTrigger(int entryID)
        {
            AddRow(entryID, 0, 32, 0, 0, 0, 0, 0, 0); // Flag 1 = don't allow while in use, and 32 = nodespawn
        }

        public void AddRowForTransport(int entryID)
        {
            AddRow(entryID, 0, 40, 0, 0, 0, 0, 0, 0);
        }

        public void AddRow(int entryID, int faction, int flags, int mingold, int maxgold, int artkit0, int artkit1, int artkit2, int artkit3)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", entryID);
            newRow.AddInt("faction", faction);
            newRow.AddInt("flags", flags); // 8 = GO_FLAG_TRANSPORT, 32 = GO_FLAG_NODESPAWN
            newRow.AddInt("mingold", mingold);
            newRow.AddInt("maxgold", maxgold);
            newRow.AddInt("artkit0", artkit0);
            newRow.AddInt("artkit1", artkit1);
            newRow.AddInt("artkit2", artkit2);
            newRow.AddInt("artkit3", artkit3);
            Rows.Add(newRow);
        }
    }
}
