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

namespace EQWOWConverter.Player
{
    internal class PlayerClassLevelStats
    {
        private static List<PlayerClassLevelStats> Pre55DKLevelStats = new List<PlayerClassLevelStats>();
        private static readonly object PlayerClassLevelStatsLock = new object();

        public int Level;
        public int BaseHP;
        public int BaseMana;
        public int Strength;
        public int Agility;
        public int Stamina;
        public int Intellect;
        public int Spirit;

        public static List<PlayerClassLevelStats> GetPre55DKLevelStats()
        {
            lock (PlayerClassLevelStatsLock)
            {
                if (Pre55DKLevelStats.Count == 0)
                    LoadPre55DKLevelStats();
                return Pre55DKLevelStats;
            }
        }

        private static void LoadPre55DKLevelStats()
        {
            Pre55DKLevelStats.Clear();
            string statsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerClassDKPre55Stats.csv");
            Logger.WriteDebug(string.Concat("Populating DK pre 55 level stats via file '", statsFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(statsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                PlayerClassLevelStats levelStats = new PlayerClassLevelStats();
                levelStats.Level = int.Parse(columns["Level"]);
                levelStats.BaseHP = int.Parse(columns["BaseHP"]);
                levelStats.BaseMana = int.Parse(columns["BaseMana"]);
                levelStats.Strength = int.Parse(columns["Strength"]);
                levelStats.Agility = int.Parse(columns["Agility"]);
                levelStats.Stamina = int.Parse(columns["Stamina"]);
                levelStats.Intellect = int.Parse(columns["Intellect"]);
                levelStats.Spirit = int.Parse(columns["Spirit"]);
                Pre55DKLevelStats.Add(levelStats);
            }
        }
    }
}
