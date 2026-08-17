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

namespace EQWOWConverter.Creatures
{
    internal class CreatureVendorItemRequiredReputation
    {
        private static Dictionary<(int, int), List<CreatureVendorItemRequiredReputation>> RequiredReputationsByMerchantIDAndWOWItemID = new Dictionary<(int, int), List<CreatureVendorItemRequiredReputation>>();
        private static object RequiredReputationsLock = new object();

        public int MerchantID = 0;
        public int WOWItemID = 0;
        public int RequiredWOWFactionID = 0;
        public int RequiredReputationRank = 0;

        // Reputation ranks are 0 (Hated) through 7 (Exalted), and the condition takes a mask of every acceptable
        public int GetRequiredReputationRankMask()
        {
            int rankMask = 0;
            for (int curRank = RequiredReputationRank; curRank <= 7; curRank++)
                rankMask |= (1 << curRank);
            return rankMask;
        }

        public static Dictionary<(int, int), List<CreatureVendorItemRequiredReputation>> GetRequiredReputationsByMerchantIDAndWOWItemID()
        {
            lock (RequiredReputationsLock)
            {
                if (RequiredReputationsByMerchantIDAndWOWItemID.Count == 0)
                    PopulateRequiredReputations();
                return RequiredReputationsByMerchantIDAndWOWItemID;
            }
        }

        private static void PopulateRequiredReputations()
        {
            RequiredReputationsByMerchantIDAndWOWItemID.Clear();

            string requiredReputationsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "VendorItemRequiredReputations.csv");
            Logger.WriteDebug("Populating Creature Vendor Item Required Reputations list via file '" + requiredReputationsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(requiredReputationsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip disabled
                if (int.Parse(columns["enabled"]) == 0)
                    continue;

                CreatureVendorItemRequiredReputation newRequiredReputation = new CreatureVendorItemRequiredReputation();
                newRequiredReputation.MerchantID = int.Parse(columns["merchantid"]);
                newRequiredReputation.WOWItemID = int.Parse(columns["wow_itemid"]);
                newRequiredReputation.RequiredWOWFactionID = int.Parse(columns["required_wow_factionid"]);
                newRequiredReputation.RequiredReputationRank = int.Parse(columns["required_reputation_rank"]);
                if (newRequiredReputation.RequiredReputationRank < 0 || newRequiredReputation.RequiredReputationRank > 7)
                {
                    Logger.WriteError("Creature Vendor Item Required Reputation for merchant '" + newRequiredReputation.MerchantID + "' and item '" + newRequiredReputation.WOWItemID
                        + "' had an invalid reputation rank of '" + newRequiredReputation.RequiredReputationRank + "', so it was skipped");
                    continue;
                }

                (int, int) key = (newRequiredReputation.MerchantID, newRequiredReputation.WOWItemID);
                if (RequiredReputationsByMerchantIDAndWOWItemID.ContainsKey(key) == false)
                    RequiredReputationsByMerchantIDAndWOWItemID.Add(key, new List<CreatureVendorItemRequiredReputation>());
                RequiredReputationsByMerchantIDAndWOWItemID[key].Add(newRequiredReputation);
            }
        }
    }
}
