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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectSQL : SQLFile
    {
        private static int CUR_GUID = Configuration.SQL_GAMEOBJECT_GUID_ID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM gameobject WHERE `id` >= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString() + " AND `id` <= " + Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END + ";";
        }

        public void AddRow(int gameObjectTemplateID, int mapID, int areaID, Vector3 position, float orientation)
        {
            int curGUID = CUR_GUID;
            CUR_GUID++;

            SQLRow newRow = new SQLRow();
            newRow.AddInt("guid", curGUID);
            newRow.AddInt("id", gameObjectTemplateID);
            newRow.AddInt("map", mapID);
            newRow.AddInt("zoneId", areaID);
            newRow.AddInt("areaId", areaID);
            newRow.AddInt("spawnMask", 1);
            newRow.AddInt("phaseMask", 1);
            newRow.AddFloat("position_x", position.X);
            newRow.AddFloat("position_y", position.Y);
            newRow.AddFloat("position_z", position.Z);
            newRow.AddFloat("orientation", orientation);
            newRow.AddFloat("rotation0", 0);
            newRow.AddFloat("rotation1", 0);
            newRow.AddFloat("rotation2", 0);
            newRow.AddFloat("rotation3", 0);
            newRow.AddInt("spawntimesecs", 900);
            newRow.AddInt("animprogress", 0);
            newRow.AddInt("state", 1);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("VerifiedBuild", 0);
            newRow.AddString("Comment", string.Empty);
            Rows.Add(newRow);
        }
    }
}
