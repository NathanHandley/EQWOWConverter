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

namespace EQWOWConverter.WOWFiles
{
    internal class TalentDBC : DBCFile
    {
        private static readonly int RANK_SPELL_ID_FIELD_START_BYTE_OFFSET = 16; // ID, TabID, TierID, ColumnIndex come first
        private static readonly int MAX_TALENT_RANKS = 9;

        private Dictionary<int, List<int>> RankSpellIDsByFirstRankSpellID = new Dictionary<int, List<int>>();

        protected override void OnPostLoadDataFromDisk()
        {
            foreach (DBCRow row in Rows)
            {
                List<int> rankSpellIDs = new List<int>();
                for (int i = 0; i < MAX_TALENT_RANKS; i++)
                {
                    int byteCursor = RANK_SPELL_ID_FIELD_START_BYTE_OFFSET + (i * 4);
                    int rankSpellID = row.SourceRawBytes[byteCursor] | (row.SourceRawBytes[byteCursor + 1] << 8) | (row.SourceRawBytes[byteCursor + 2] << 16) | (row.SourceRawBytes[byteCursor + 3] << 24);
                    if (rankSpellID != 0)
                        rankSpellIDs.Add(rankSpellID);
                }
                if (rankSpellIDs.Count > 0)
                    RankSpellIDsByFirstRankSpellID[rankSpellIDs[0]] = rankSpellIDs;
            }
        }

        public List<int> GetAllRankSpellIDsForFirstRankSpellID(int firstRankSpellID)
        {
            if (RankSpellIDsByFirstRankSpellID.ContainsKey(firstRankSpellID) == false)
            {
                Logger.WriteError("TalentDBC could not find a talent whose first rank spell ID is '", firstRankSpellID.ToString(), "'");
                return new List<int>();
            }
            return RankSpellIDsByFirstRankSpellID[firstRankSpellID];
        }

        public bool TryGetAllRankSpellIDsForFirstRankSpellID(int firstRankSpellID, out List<int> rankSpellIDs)
        {
            return RankSpellIDsByFirstRankSpellID.TryGetValue(firstRankSpellID, out rankSpellIDs!);
        }
    }
}
