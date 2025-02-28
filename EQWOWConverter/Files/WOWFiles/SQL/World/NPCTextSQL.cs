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
    internal class NPCTextSQL : SQLFile
    {
        private static int CUR_ID = Configuration.SQL_NPCTEXT_ID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM `npc_text` WHERE `ID` >= " + Configuration.SQL_NPCTEXT_ID_START.ToString() + " AND `ID` <= " + Configuration.SQL_NPCTEXT_ID_END + ";";
        }

        public void AddRow(int id, string text, int broadcastID)
        {
            SQLRow newRow = new SQLRow();
			newRow.AddInt("ID", id);
			newRow.AddString("text0_0", text);
			newRow.AddString("text0_1", text);
			newRow.AddInt("BroadcastTextID0", broadcastID);
			newRow.AddInt("lang0", 0);
			newRow.AddFloat("Probability0", 0f);
			newRow.AddInt("em0_0", 0);
			newRow.AddInt("em0_1", 0);
			newRow.AddInt("em0_2", 0);
			newRow.AddInt("em0_3", 0);
			newRow.AddInt("em0_4", 0);
			newRow.AddInt("em0_5", 0);
			newRow.AddString("text1_0", string.Empty);
			newRow.AddString("text1_1", string.Empty);
			newRow.AddInt("BroadcastTextID1", 0);
			newRow.AddInt("lang1", 0);
			newRow.AddFloat("Probability1", 0f);
			newRow.AddInt("em1_0", 0);
			newRow.AddInt("em1_1", 0);
			newRow.AddInt("em1_2", 0);
			newRow.AddInt("em1_3", 0);
			newRow.AddInt("em1_4", 0);
			newRow.AddInt("em1_5", 0);
			newRow.AddString("text2_0", string.Empty);
			newRow.AddString("text2_1", string.Empty);
			newRow.AddInt("BroadcastTextID2", 0);
			newRow.AddInt("lang2", 0);
			newRow.AddFloat("Probability2", 0f);
			newRow.AddInt("em2_0", 0);
			newRow.AddInt("em2_1", 0);
			newRow.AddInt("em2_2", 0);
			newRow.AddInt("em2_3", 0);
			newRow.AddInt("em2_4", 0);
			newRow.AddInt("em2_5", 0);
			newRow.AddString("text3_0", string.Empty);
			newRow.AddString("text3_1", string.Empty);
			newRow.AddInt("BroadcastTextID3", 0);
			newRow.AddInt("lang3", 0);
			newRow.AddFloat("Probability3", 0f);
			newRow.AddInt("em3_0", 0);
			newRow.AddInt("em3_1", 0);
			newRow.AddInt("em3_2", 0);
			newRow.AddInt("em3_3", 0);
			newRow.AddInt("em3_4", 0);
			newRow.AddInt("em3_5", 0);
			newRow.AddString("text4_0", string.Empty);
			newRow.AddString("text4_1", string.Empty);
			newRow.AddInt("BroadcastTextID4", 0);
			newRow.AddInt("lang4", 0);
			newRow.AddFloat("Probability4", 0f);
			newRow.AddInt("em4_0", 0);
			newRow.AddInt("em4_1", 0);
			newRow.AddInt("em4_2", 0);
			newRow.AddInt("em4_3", 0);
			newRow.AddInt("em4_4", 0);
			newRow.AddInt("em4_5", 0);
			newRow.AddString("text5_0", string.Empty);
			newRow.AddString("text5_1", string.Empty);
			newRow.AddInt("BroadcastTextID5", 0);
			newRow.AddInt("lang5", 0);
			newRow.AddFloat("Probability5", 0f);
			newRow.AddInt("em5_0", 0);
			newRow.AddInt("em5_1", 0);
			newRow.AddInt("em5_2", 0);
			newRow.AddInt("em5_3", 0);
			newRow.AddInt("em5_4", 0);
			newRow.AddInt("em5_5", 0);
			newRow.AddString("text6_0", string.Empty);
			newRow.AddString("text6_1", string.Empty);
			newRow.AddInt("BroadcastTextID6", 0);
			newRow.AddInt("lang6", 0);
			newRow.AddFloat("Probability6", 0f);
			newRow.AddInt("em6_0", 0);
			newRow.AddInt("em6_1", 0);
			newRow.AddInt("em6_2", 0);
			newRow.AddInt("em6_3", 0);
			newRow.AddInt("em6_4", 0);
			newRow.AddInt("em6_5", 0);
			newRow.AddString("text7_0", string.Empty);
			newRow.AddString("text7_1", string.Empty);
			newRow.AddInt("BroadcastTextID7", 0);
			newRow.AddInt("lang7", 0);
			newRow.AddFloat("Probability7", 0f);
			newRow.AddInt("em7_0", 0);
			newRow.AddInt("em7_1", 0);
			newRow.AddInt("em7_2", 0);
			newRow.AddInt("em7_3", 0);
			newRow.AddInt("em7_4", 0);
			newRow.AddInt("em7_5", 0);
			newRow.AddInt("VerifiedBuild", 0);
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
