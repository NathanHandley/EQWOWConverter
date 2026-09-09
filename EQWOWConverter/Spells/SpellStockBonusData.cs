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

namespace EQWOWConverter.Spells
{
    internal class SpellStockBonusData
    {
        private static readonly object StockSpellBonusDataLock = new object();
        private static Dictionary<int, SpellStockBonusData> StockSpellBonusDataBySpellID = new Dictionary<int, SpellStockBonusData>();

        public int SpellID = 0;
        public float DirectBonus = 0f;
        public float DotBonus = 0f;

        public static Dictionary<int, SpellStockBonusData> GetStockSpellBonusDataBySpellID()
        {
            lock (StockSpellBonusDataLock)
            {
                if (StockSpellBonusDataBySpellID.Count == 0)
                    PopulateStockSpellBonusData();
                return StockSpellBonusDataBySpellID;
            }
        }

        private static void PopulateStockSpellBonusData()
        {
            string bonusDataFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "WOWSpellBonusData.csv");
            Logger.WriteDebug(string.Concat("Populating stock spell bonus data via file '", bonusDataFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(bonusDataFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                SpellStockBonusData newBonusData = new SpellStockBonusData();
                if (int.TryParse(columns["entry"].Trim(), out newBonusData.SpellID) == false || newBonusData.SpellID <= 0)
                {
                    Logger.WriteError("WOWSpellBonusData row had an invalid entry of '", columns["entry"], "'");
                    continue;
                }
                if (float.TryParse(columns["direct_bonus"].Trim(), out newBonusData.DirectBonus) == false)
                {
                    Logger.WriteError("WOWSpellBonusData row for entry '", newBonusData.SpellID.ToString(), "' had an invalid direct_bonus of '", columns["direct_bonus"], "'");
                    continue;
                }
                if (float.TryParse(columns["dot_bonus"].Trim(), out newBonusData.DotBonus) == false)
                {
                    Logger.WriteError("WOWSpellBonusData row for entry '", newBonusData.SpellID.ToString(), "' had an invalid dot_bonus of '", columns["dot_bonus"], "'");
                    continue;
                }
                StockSpellBonusDataBySpellID[newBonusData.SpellID] = newBonusData;
            }
            Logger.WriteDebug(string.Concat("Populated ", StockSpellBonusDataBySpellID.Count.ToString(), " stock spell bonus data rows"));
        }
    }
}
