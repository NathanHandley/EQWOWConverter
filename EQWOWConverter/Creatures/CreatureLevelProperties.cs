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
    internal class CreatureLevelProperties
    {
        private static Dictionary<int, CreatureLevelProperties> CreatureLevelPropertiesByLevel = new Dictionary<int, CreatureLevelProperties>();
        private static readonly object PropertiesReadLock = new object();

        public int Level;
        public float ExperienceMod;

        public static Dictionary<int, CreatureLevelProperties> GetCreatureLevelPropertiesByLevel()
        {
            lock (PropertiesReadLock)
            {
                if (CreatureLevelPropertiesByLevel.Count == 0)
                    PopulateLevelProperties();
                return CreatureLevelPropertiesByLevel;
            }
        }

        private static void PopulateLevelProperties()
        {
            CreatureLevelPropertiesByLevel.Clear();
            string propertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureLevelProperties.csv");
            Logger.WriteDebug(string.Concat("Populating creature level properties via file '", propertiesFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(propertiesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Load the row
                CreatureLevelProperties properties = new CreatureLevelProperties();
                properties.Level = int.Parse(columns["Level"]);
                properties.ExperienceMod = float.Parse(columns["ExpMod"]);

                // Add if unique
                if (CreatureLevelPropertiesByLevel.ContainsKey(properties.Level) == true)
                {
                    Logger.WriteError(string.Concat("Failed to read in a creature level properties row since there was a duplicate row with level ", properties.Level));
                    continue;
                }
                CreatureLevelPropertiesByLevel.Add(properties.Level, properties);
            }
        }
    }
}
