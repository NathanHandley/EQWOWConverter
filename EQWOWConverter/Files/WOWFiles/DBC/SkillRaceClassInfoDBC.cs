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
    internal class SkillRaceClassInfoDBC : DBCFile
    {
        public void AddRow(int skillID, List<ClassWOWType> allowedClasses)
        {
            DBCRow newRow = new DBCRow();

            int classMask = CalculateClassMask(allowedClasses);
            int id = IDGenerationTool.GenerateID("SkillRaceClassInfoID", skillID.ToString(), classMask.ToString());

            newRow.AddInt32(id); // ID
            newRow.AddInt32(skillID); // SkillID
            newRow.AddInt32(-1); // RaceMask
            newRow.AddInt32(classMask); // ClassMask
            newRow.AddInt32(128); // Flags (0x80 / 128 = "Available for learning for Class/Race"
            newRow.AddInt32(0); // MinLevel
            newRow.AddInt32(0); // SkillTierID
            newRow.AddInt32(0); // SkillCostIndex

            newRow.SortValue1 = skillID;
            newRow.SortValue2 = id;

            Rows.Add(newRow);
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("SkillRaceClassInfoDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillID
                row.AddIntFromSourceRawBytes(ref byteCursor); // RaceMask
                row.AddIntFromSourceRawBytes(ref byteCursor); // ClassMask
                row.AddIntFromSourceRawBytes(ref byteCursor); // Flags
                row.AddIntFromSourceRawBytes(ref byteCursor); // MinLevel
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillTierID
                row.AddIntFromSourceRawBytes(ref byteCursor); // SkillCostIndex

                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[1]).Value; // SkillID
                row.SortValue2 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID

                // Purge raw data
                row.SourceRawBytes.Clear();
            }
        }

        private int CalculateClassMask(List<ClassWOWType> allowedClassTypes)
        {
            int allowableClass = 0;
            foreach (ClassWOWType classType in allowedClassTypes)
            {
                if (classType == ClassWOWType.All)
                    return -1;
                allowableClass += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(classType) - 1));
            }
            if (allowableClass == 0)
                allowableClass = -1;
            return allowableClass;
        }
    }
}
