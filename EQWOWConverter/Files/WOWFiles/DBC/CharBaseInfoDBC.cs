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

using EQWOWConverter.Common;

namespace EQWOWConverter.WOWFiles
{
    internal class CharBaseInfoDBC : DBCFile
    {
        private HashSet<(byte, byte)> RaceAndClassIDPairs = new HashSet<(byte, byte)>();

        protected override void OnPostLoadDataFromDisk()
        {
            foreach (DBCRow row in Rows)
            {
                if (row.SourceRawBytes.Count < 2)
                {
                    Logger.WriteError("CharBaseInfoDBC had a row with fewer than two source bytes in OnPostLoadDataFromDisk");
                    continue;
                }
                RaceAndClassIDPairs.Add((row.SourceRawBytes[0], row.SourceRawBytes[1]));

                // Rows are positional in the source file, so hold the race and class ordering through the sort in SaveToDisk
                row.SortValue1 = row.SourceRawBytes[0];
                row.SortValue2 = row.SourceRawBytes[1];
            }
        }

        public void AddMissingRaceClassCombinations()
        {
            int addedRowCount = 0;
            foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
            {
                if (raceType == RaceType.All)
                    continue;
                foreach (ClassWOWType classType in Enum.GetValues(typeof(ClassWOWType)))
                {
                    if (classType == ClassWOWType.All || classType == ClassWOWType.None)
                        continue;
                    byte raceID = Convert.ToByte(Convert.ToInt32(raceType));
                    byte classID = Convert.ToByte(Convert.ToInt32(classType));
                    if (RaceAndClassIDPairs.Contains((raceID, classID)) == true)
                        continue;

                    DBCRow newRow = new DBCRow();
                    newRow.AddByte(raceID); // RaceID
                    newRow.AddByte(classID); // ClassID
                    newRow.SortValue1 = raceID;
                    newRow.SortValue2 = classID;
                    Rows.Add(newRow);
                    RaceAndClassIDPairs.Add((raceID, classID));
                    addedRowCount++;
                }
            }
            Logger.WriteDebug(string.Concat("CharBaseInfoDBC added ", addedRowCount.ToString(), " race and class pairings that the source file did not have"));
        }
    }
}
