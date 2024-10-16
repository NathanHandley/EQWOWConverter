﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class GameObjectSQL
    {
        // Row GUID
        private static int CURRENT_GUIDID = Configuration.CONFIG_SQL_GAMEOBJECT_GUID_START;

        public class Row
        {
            public int GUID;
            public int GameObjectEntryID;
            public int MapID;
            public int ZoneID;
            public int AreaID;
            public int SpawnMask = 1;
            public int PhaseMask = 1;
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float Orientation = 0;
            public float Rotation0 = 0;
            public float Rotation1 = 0;
            public float Rotation2 = 0;
            public float Rotation3 = 0;
            public int SpawnTimeInSec = 900;
            public int AnimProgress = 0;
            public int State = 1;
            public string ScriptName = string.Empty;
            public int VerifiedBuild = 0;
            public string Comment = string.Empty;
        }

        List<Row> rows = new List<Row>();

        public void AddRow(int gameObjectID, int mapID, int parentAreaID, int areaID, Vector3 position)
        {
            // Generate the unique GUID
            int rowGUID = CURRENT_GUIDID;
            CURRENT_GUIDID++;

            Row newRow = new Row();
            newRow.GUID = rowGUID;
            newRow.GameObjectEntryID = gameObjectID;
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
            string fullFilePath = Path.Combine(baseFolderPath, "GameObjectSQL.sql");

            // Add the row data
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM gameobject WHERE `id` >= " + Configuration.CONFIG_DBCID_GAMEOBJECT_ID_START.ToString() + " AND `id` <= " + Configuration.CONFIG_DBCID_GAMEOBJECT_ID_END + ";");
            foreach (Row row in rows)
            {
                stringBuilder.Append("INSERT INTO `gameobject` (`guid`, `id`, `map`, `zoneId`, `areaId`, `spawnMask`, `phaseMask`, `position_x`, `position_y`, `position_z`, `orientation`, `rotation0`, `rotation1`, `rotation2`, `rotation3`, `spawntimesecs`, `animprogress`, `state`, `ScriptName`, `VerifiedBuild`, `Comment`) VALUES (");
                stringBuilder.Append(row.GUID.ToString() + ", ");
                stringBuilder.Append(row.GameObjectEntryID.ToString() + ", ");
                stringBuilder.Append(row.MapID.ToString() + ", ");
                stringBuilder.Append(row.ZoneID.ToString() + ", ");
                stringBuilder.Append(row.AreaID.ToString() + ", ");
                stringBuilder.Append(row.SpawnMask.ToString() + ", ");
                stringBuilder.Append(row.PhaseMask.ToString() + ", ");
                stringBuilder.Append(row.PositionX.ToString() + ", ");
                stringBuilder.Append(row.PositionY.ToString() + ", ");
                stringBuilder.Append(row.PositionZ.ToString() + ", ");
                stringBuilder.Append(row.Orientation.ToString() + ", ");
                stringBuilder.Append(row.Rotation0.ToString() + ", ");
                stringBuilder.Append(row.Rotation1.ToString() + ", ");
                stringBuilder.Append(row.Rotation2.ToString() + ", ");
                stringBuilder.Append(row.Rotation3.ToString() + ", ");
                stringBuilder.Append(row.SpawnTimeInSec.ToString() + ", ");
                stringBuilder.Append(row.AnimProgress.ToString() + ", ");
                stringBuilder.Append(row.State.ToString() + ", ");
                stringBuilder.Append("'" + row.ScriptName.ToString() + "', ");
                stringBuilder.Append(row.VerifiedBuild.ToString() + ", ");
                stringBuilder.AppendLine("'" + row.Comment.ToString() + "');");
            }

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
