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
    internal class SpellPetPowerTierLevelStats
    {
        private static readonly object SpellPetPowerTierLock = new object();
        private static Dictionary<string, SortedDictionary<int, SpellPetPowerTierLevelStats>> StatsByTypeNameAndLevel = new Dictionary<string, SortedDictionary<int, SpellPetPowerTierLevelStats>>();

        public string TypeName = string.Empty;
        public int Level = 0;
        public int Health = 0;
        public int Mana = 0;
        public int Armor = 0;
        public int Strength = 0;
        public int Agility = 0;
        public int Stamina = 0;
        public int Intellect = 0;
        public int Spirit = 0;
        public int MinDamage = 0;
        public int MaxDamage = 0;

        public static SortedDictionary<int, SpellPetPowerTierLevelStats>? GetStatsByLevelForTypeName(string typeName)
        {
            lock (SpellPetPowerTierLock)
            {
                if (StatsByTypeNameAndLevel.Count == 0)
                    LoadSpellPetPowerTierData();
                if (StatsByTypeNameAndLevel.ContainsKey(typeName) == true)
                    return StatsByTypeNameAndLevel[typeName];
                Logger.WriteError("Could not find a spell pet power tier with name '", typeName, "'");
                return null;
            }
        }

        private static void LoadSpellPetPowerTierData()
        {
            string powerTierFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellPetPowerTierLevelStats.csv");
            Logger.WriteDebug(string.Concat("Loading spell pet power tier level stats via file '", powerTierFile, "'"));
            List<Dictionary<string, string>> powerTierRows = FileTool.ReadAllRowsFromFileWithHeader(powerTierFile, "|");
            foreach (Dictionary<string, string> columns in powerTierRows)
            {
                SpellPetPowerTierLevelStats levelStats = new SpellPetPowerTierLevelStats();
                levelStats.TypeName = columns["type"];
                levelStats.Level = Convert.ToInt32(columns["level"]);
                levelStats.Health = Convert.ToInt32(columns["hp"]);
                levelStats.Mana = Convert.ToInt32(columns["mana"]);
                levelStats.Armor = Convert.ToInt32(columns["armor"]);
                levelStats.Strength = Convert.ToInt32(columns["str"]);
                levelStats.Agility = Convert.ToInt32(columns["agi"]);
                levelStats.Stamina = Convert.ToInt32(columns["sta"]);
                levelStats.Intellect = Convert.ToInt32(columns["inte"]);
                levelStats.Spirit = Convert.ToInt32(columns["spi"]);
                levelStats.MinDamage = Convert.ToInt32(columns["min_dmg"]);
                levelStats.MaxDamage = Convert.ToInt32(columns["max_dmg"]);
                if (StatsByTypeNameAndLevel.ContainsKey(levelStats.TypeName) == false)
                    StatsByTypeNameAndLevel.Add(levelStats.TypeName, new SortedDictionary<int, SpellPetPowerTierLevelStats>());
                if (StatsByTypeNameAndLevel[levelStats.TypeName].ContainsKey(levelStats.Level) == true)
                {
                    Logger.WriteError("Spell pet power tier '", levelStats.TypeName, "' has more than one row for level ", levelStats.Level.ToString());
                    continue;
                }
                StatsByTypeNameAndLevel[levelStats.TypeName].Add(levelStats.Level, levelStats);
            }
            Logger.WriteDebug("Loading spell pet power tier level stats complete");
        }
    }
}
