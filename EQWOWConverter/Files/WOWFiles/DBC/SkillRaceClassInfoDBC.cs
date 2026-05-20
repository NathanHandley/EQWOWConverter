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
        private static int CUR_SKILLRACECLASSINFO_DBCID = Configuration.DBCID_SKILLRACECLASSINFO_ID_START;

        public void AddRow(int skillID, List<ClassWOWType> allowedClasses)
        {
            DBCRow newRow = new DBCRow();

            int id = GenerateID();

            newRow.AddInt32(id); // ID
            newRow.AddInt32(skillID); // SkillID
            newRow.AddInt32(-1); // RaceMask
            newRow.AddInt32(CalculateClassMask(allowedClasses)); // ClassMask
            newRow.AddInt32(128); // Flags (0x80 / 128 = "Available for learning for Class/Race"
            newRow.AddInt32(0); // MinLevel
            newRow.AddInt32(0); // SkillTierID
            newRow.AddInt32(0); // SkillCostIndex

            newRow.SortValue1 = skillID;
            newRow.SortValue2 = id;

            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            int newID = CUR_SKILLRACECLASSINFO_DBCID;
            CUR_SKILLRACECLASSINFO_DBCID++;
            return newID;
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
