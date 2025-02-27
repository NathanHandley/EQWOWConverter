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

using EQWOWConverter.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM creature WHERE `guid` >= " + Configuration.SQL_CREATURE_GUID_LOW.ToString() + " AND `guid` <= " + Configuration.SQL_CREATURE_GUID_HIGH + ";";
        }

        public void AddRow(int guid, int id1, int mapID, int zoneID, int areaID, float xPosition, float yPosition, float zPosition, 
            float orientation, CreatureMovementType movementType)
        {
            int currentWaypoint = 0;
            if (movementType == CreatureMovementType.Path)
                currentWaypoint = 1;

            SQLRow newRow = new SQLRow();
            newRow.AddInt("guid", guid);
            newRow.AddInt("id1", id1);
            newRow.AddInt("id2", 0);
            newRow.AddInt("id3", 0);
            newRow.AddInt("map", mapID);
            newRow.AddInt("zoneId", zoneID);
            newRow.AddInt("areaId", areaID);
            newRow.AddInt("spawnMask", 1);
            newRow.AddInt("phaseMask", 1);
            newRow.AddInt("equipment_id", 0);
            newRow.AddFloat("position_x", MathF.Round(xPosition, 6));
            newRow.AddFloat("position_y", MathF.Round(yPosition, 6));
            newRow.AddFloat("position_z", MathF.Round(zPosition, 6));
            newRow.AddFloat("orientation", MathF.Round(orientation, 6));
            newRow.AddInt("spawntimesecs", 300);
            newRow.AddFloat("wander_distance", 0);
            newRow.AddInt("currentwaypoint", currentWaypoint);
            newRow.AddInt("curhealth", 100);
            newRow.AddInt("curmana", 0);
            newRow.AddInt("MovementType", Convert.ToInt32(movementType));
            newRow.AddInt("npcflag", 0);
            newRow.AddInt("unit_flags", 0);
            newRow.AddInt("dynamicflags", 0);
            newRow.AddString("ScriptName", 64, string.Empty);
            newRow.AddInt("VerifiedBuild", 0);
            newRow.AddInt("CreateObject", 0);
            newRow.AddString("Comment", string.Empty);
            Rows.Add(newRow);
        }
    }
}
