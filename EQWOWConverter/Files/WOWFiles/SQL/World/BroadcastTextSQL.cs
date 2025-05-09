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

namespace EQWOWConverter.WOWFiles
{
    internal class BroadcastTextSQL : SQLFile
    {
        private static int CUR_ID = Configuration.SQL_BROADCASTTEXT_ID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM `broadcast_text` WHERE `ID` >= " + Configuration.SQL_BROADCASTTEXT_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_BROADCASTTEXT_ID_END + ";";
        }

        public void AddRow(int id, string maleText, string femaleText)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", id);
	        newRow.AddInt("LanguageID", 0);
	        newRow.AddString("MaleText", maleText);
	        newRow.AddString("FemaleText", femaleText);
	        newRow.AddInt("EmoteID1", 0);
	        newRow.AddInt("EmoteID2", 0);
	        newRow.AddInt("EmoteID3", 0);
	        newRow.AddInt("EmoteDelay1", 0);
	        newRow.AddInt("EmoteDelay2", 0);
	        newRow.AddInt("EmoteDelay3", 0);
	        newRow.AddInt("SoundEntriesId", 0);
	        newRow.AddInt("EmotesID", 0);
	        newRow.AddInt("Flags", 1);
	        newRow.AddInt("VerifiedBuild", 18019);
            Rows.Add(newRow);
        }

        public static int GenerateUniqueID()
        {
            int returnVal = CUR_ID;
            CUR_ID++;
            return returnVal;
        }
    }
}
