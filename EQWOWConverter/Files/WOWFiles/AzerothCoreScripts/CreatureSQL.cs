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
        // Row DBCIDs
        private static int CURRENT_GUIDID = Configuration.CONFIG_SQL_CREATURE_GUID_LOW;

        public class Row
        {
            public int Guid;
            public int ID1;
            public int MapID = 0;
            public int ZoneID = 0;
            public int AreaID = 0;
            public float PositionX = 0;
            public float PositionY = 0;
            public float PositionZ = 0;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int id1, Vector3 position, int mapID, int parentAreaID, int areaID)
        {
            // Generate unique guid
            int rowGUID = CURRENT_GUIDID;
            CURRENT_GUIDID++;

            Row newRow = new Row();
            newRow.Guid = rowGUID;
            newRow.ID1 = id1;
            newRow.MapID = mapID;
            newRow.ZoneID = parentAreaID;
            newRow.AreaID = areaID;
            newRow.PositionX = position.X;
            newRow.PositionY = position.Y;
            newRow.PositionZ = position.Z;
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
                stringBuilder.Append(row.ID1.ToString() + ", 0, 0, ");
                stringBuilder.Append(row.MapID.ToString() + ", ");
                stringBuilder.Append(row.ZoneID.ToString() + ", ");
                stringBuilder.Append(row.AreaID.ToString() + ", 1, 1, 0, ");
                stringBuilder.Append(row.PositionX.ToString() + ", ");
                stringBuilder.Append(row.PositionY.ToString() + ", ");
                stringBuilder.Append(row.PositionZ.ToString() + ", ");
                stringBuilder.AppendLine("0, 300, 0, 0, 100, 0, 1, 0, 0, 0, '', 0, 0, NULL);");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
