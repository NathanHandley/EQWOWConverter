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
    internal class GameObjectTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM gameobject_template WHERE `entry` >= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString() + " AND `entry` <= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END + ";";
        }

        public void AddRow()
        {
            SQLRow newRow = new SQLRow();
			newRow.AddInt("entry", 0);
            newRow.AddInt("type", 0);
            newRow.AddInt("displayId", 0);
			newRow.AddString("name", 100, string.Empty);
            newRow.AddString("IconName", 100, string.Empty);
            newRow.AddString("castBarCaption", 100, string.Empty);
            newRow.AddString("unk1", 100, string.Empty);
			newRow.AddFloat("size", 1);
            newRow.AddInt("Data0", 0);
            newRow.AddInt("Data1", 0);
            newRow.AddInt("Data2", 0);
            newRow.AddInt("Data3", 0);
            newRow.AddInt("Data4", 0);
            newRow.AddInt("Data5", 0);
            newRow.AddInt("Data6", 0);
            newRow.AddInt("Data7", 0);
            newRow.AddInt("Data8", 0);
            newRow.AddInt("Data9", 0);
            newRow.AddInt("Data10", 0);
            newRow.AddInt("Data11", 0);
            newRow.AddInt("Data12", 0);
            newRow.AddInt("Data13", 0);
            newRow.AddInt("Data14", 0);
            newRow.AddInt("Data15", 0);
            newRow.AddInt("Data16", 0);
            newRow.AddInt("Data17", 0);
            newRow.AddInt("Data18", 0);
            newRow.AddInt("Data19", 0);
            newRow.AddInt("Data20", 0);
            newRow.AddInt("Data21", 0);
            newRow.AddInt("Data22", 0);
            newRow.AddInt("Data23", 0);
            newRow.AddString("AIName", 64, string.Empty);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("VerifiedBuild", 12340);            
            Rows.Add(newRow);
        }
    }
}
