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

using EQWOWConverter.Zones;
using Google.Protobuf.WellKnownTypes;

namespace EQWOWConverter.WOWFiles
{
    internal class AreaTableDBC : DBCFile
    {
        private static int CURRENT_AREABIT = Configuration.DBCID_AREATABLE_AREABIT_BLOCK_1_START;
        private static readonly object AreaBitLock = new object();

        public void AddRow(int id, int mapID, int parentAreaID, ZoneAreaMusic? areaMusic, ZoneAreaAmbientSound? areaSound, string areaName, bool isRestingArea)
        {
            // AreaBit must always be unique
            int areaBit;
            lock(AreaBitLock)
            {
                areaBit = CURRENT_AREABIT;
                CURRENT_AREABIT++;
                if (CURRENT_AREABIT == Configuration.DBCID_AREATABLE_AREABIT_BLOCK_1_END + 1)
                    CURRENT_AREABIT = Configuration.DBCID_AREATABLE_AREABIT_BLOCK_2_START;
                else if (CURRENT_AREABIT == Configuration.DBCID_AREATABLE_AREABIT_BLOCK_2_END + 1)
                    CURRENT_AREABIT = Configuration.DBCID_AREATABLE_AREABIT_BLOCK_3_START;
                else if (CURRENT_AREABIT > Configuration.DBCID_AREATABLE_AREABIT_BLOCK_3_END)
                    Logger.WriteError("Areabit is too high, as it is over '" + Configuration.DBCID_AREATABLE_AREABIT_BLOCK_3_END + "'");
            }

            // Music
            int zoneMusicID = 0;
            if (areaMusic != null)
                zoneMusicID = areaMusic.DBCID;

            // Ambience
            int ambienceID = 0;
            if (areaSound != null)
                ambienceID = areaSound.DBCID;

            // Flags
            int flags = 0;
            if (isRestingArea == true)
                flags = 2097448; // AREA_FLAG_AllowTradeChannel + AREA_FLAG_AllowResting + AREA_FLAG_LinkedChat + AREA_FLAG_PlayersCallGuards 

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id);
            newRow.AddInt32(mapID); // Continent ID
            newRow.AddInt32(parentAreaID);
            newRow.AddInt32(areaBit);
            newRow.AddPackedFlags(flags); // Flags
            newRow.AddInt32(0); // Sound Provider Preferences
            newRow.AddInt32(0); // Sound Provider Preferences - Underwater
            newRow.AddInt32(ambienceID);
            newRow.AddInt32(zoneMusicID);
            newRow.AddInt32(0); // IntroSound
            newRow.AddInt32(0); // Exploration Level
            newRow.AddStringLang(areaName);
            newRow.AddInt32(0); // Faction Group Mask
            newRow.AddInt32(0); // LiquidTypeID 1
            newRow.AddInt32(0); // LiquidTypeID 2
            newRow.AddInt32(0); // LiquidTypeID 3
            newRow.AddInt32(0); // LiquidTypeID 4
            newRow.AddFloat(-5000f); // Minimum Elevation
            newRow.AddFloat(0); // Ambient Multiplier
            newRow.AddInt32(0); // LightID

            // Set the sort
            newRow.SortValue1 = id;

            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("AreaTableDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddPackedFlagsFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddStringLangFromSourceRawBytes(ref byteCursor, StringBlock);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);
                row.AddFloatFromSourceRawBytes(ref byteCursor);
                row.AddFloatFromSourceRawBytes(ref byteCursor);
                row.AddIntFromSourceRawBytes(ref byteCursor);

                // Attach the sort rows
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // AreaID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }
    }
}
