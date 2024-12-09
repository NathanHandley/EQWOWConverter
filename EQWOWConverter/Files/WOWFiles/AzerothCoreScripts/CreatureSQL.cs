//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureSQL
    {
        private class Row
        {
            public int Guid;
            public int ID1 = 0;
            public int ID2 = 0;
            public int ID3 = 0;
            public int MapID = 0;
            public int ZoneID = 0;
            public int AreaID = 0;
            public int SpawnMask = 1;
            public int PhaseMask = 1;
            public int EquipmentID = 0;
            public float PositionX = 0;
            public float PositionY = 0;
            public float PositionZ = 0;
            public float Orientation = 0;
            public int SpawnTimeInSec = 300;
            public float WanderDistance = 0;
            public int CurrentWaypoint = 0;
            public int CurrentHealth = 100;
            public int CurrentMana = 0;
            public CreatureMovementType MovementType = CreatureMovementType.None;
            public int NPCFlags = 0;
            public int UnitFlags = 0;
            public int DynamicFlags = 0;
            public string ScriptName = string.Empty;
            public int VerifiedBuild = 0;
            public int CreateObject = 0;
            public string Comment = string.Empty;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int guid, int id1, int mapID, int zoneID, int areaID, float xPosition, float yPosition, float zPosition, float orientation, CreatureMovementType movementType)
        {
            Row newRow = new Row();
            newRow.Guid = guid;
            newRow.ID1 = id1;
            newRow.MapID = mapID;
            newRow.PositionX = xPosition;
            newRow.PositionY = yPosition;
            newRow.PositionZ = zPosition;
            newRow.Orientation = orientation;
            newRow.AreaID = areaID;
            newRow.ZoneID = zoneID;
            newRow.MovementType = movementType;
            rows.Add(newRow);
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "Creature.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM creature WHERE `guid` >= " + Configuration.CONFIG_SQL_CREATURE_GUID_LOW.ToString() + " AND `guid` <= " + Configuration.CONFIG_SQL_CREATURE_GUID_HIGH + " ;");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `creature` (`guid`, `id1`, `id2`, `id3`, `map`, `zoneId`, `areaId`, `spawnMask`, `phaseMask`, `equipment_id`, `position_x`, `position_y`, `position_z`, `orientation`, `spawntimesecs`, `wander_distance`, `currentwaypoint`, `curhealth`, `curmana`, `MovementType`, `npcflag`, `unit_flags`, `dynamicflags`, `ScriptName`, `VerifiedBuild`, `CreateObject`, `Comment`) VALUES (");
                stringBuilder.Append(row.Guid.ToString() + ", ");
                stringBuilder.Append(row.ID1.ToString() + ", ");
                stringBuilder.Append(row.ID2.ToString() + ", ");
                stringBuilder.Append(row.ID3.ToString() + ", ");
                stringBuilder.Append(row.MapID.ToString() + ", ");
                stringBuilder.Append(row.ZoneID.ToString() + ", ");
                stringBuilder.Append(row.AreaID.ToString() + ", ");
                stringBuilder.Append(row.SpawnMask.ToString() + ", ");
                stringBuilder.Append(row.PhaseMask.ToString() + ", ");
                stringBuilder.Append(row.EquipmentID.ToString() + ", ");
                stringBuilder.Append(row.PositionX.ToString() + ", ");
                stringBuilder.Append(row.PositionY.ToString() + ", ");
                stringBuilder.Append(row.PositionZ.ToString() + ", ");
                stringBuilder.Append(row.Orientation.ToString() + ", ");
                stringBuilder.Append(row.SpawnTimeInSec.ToString() + ", ");
                stringBuilder.Append(row.WanderDistance.ToString() + ", ");
                stringBuilder.Append(row.CurrentWaypoint.ToString() + ", ");
                stringBuilder.Append(row.CurrentHealth.ToString() + ", ");
                stringBuilder.Append(row.CurrentMana.ToString() + ", ");
                stringBuilder.Append(Convert.ToInt32(row.MovementType).ToString() + ", ");
                stringBuilder.Append(row.NPCFlags.ToString() + ", ");
                stringBuilder.Append(row.UnitFlags.ToString() + ", ");
                stringBuilder.Append(row.DynamicFlags.ToString() + ", ");
                stringBuilder.Append("'" + row.ScriptName + "', ");
                stringBuilder.Append(row.VerifiedBuild.ToString() + ", ");
                stringBuilder.Append(row.CreateObject.ToString() + ", ");
                stringBuilder.Append("'" + row.Comment + "'");
                stringBuilder.AppendLine(");");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
