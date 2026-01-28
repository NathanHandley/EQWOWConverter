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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class TrainerSQL : SQLFile
    {
        private static int CUR_ID = Configuration.SQL_TRAINER_ID_START;

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM `trainer` WHERE `Id` >= " + Configuration.SQL_TRAINER_ID_START.ToString() + " AND `Id` <= " + Configuration.SQL_TRAINER_ID_END + ";");
            return stringBuilder.ToString();
        }

        public void AddRow(int trainerID, int typeID, int requirement, string greetingText)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("Id", trainerID);
            newRow.AddInt("Type", typeID);
            newRow.AddInt("Requirement", requirement);
            newRow.AddString("Greeting", greetingText);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }

        public static int GenerateUniqueTrainerID()
        {
            int returnVal = CUR_ID;
            CUR_ID++;
            if (CUR_ID > Configuration.SQL_TRAINER_ID_END)
                Logger.WriteError("Exceeded maximum value of SQL_TRAINER_ID_END");
            return returnVal;
        }
    }
}
