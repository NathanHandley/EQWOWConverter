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
using EQWOWConverter.Creatures;

namespace EQWOWConverter.Player
{
    internal class PlayerWOWRaceProperties
    {
        private static Dictionary<RaceType, PlayerWOWRaceProperties> WOWRacePropertiesByRaceType = new Dictionary<RaceType, PlayerWOWRaceProperties>();
        private static readonly object READ_LOCK = new object();

        public RaceType WOWRaceType = RaceType.All;
        public CreatureFactionAlignmentType Alignment = CreatureFactionAlignmentType.Neutral;
        public bool HasSlam = false;

        public static Dictionary<RaceType, PlayerWOWRaceProperties> GetAllWOWRacePropertiesByRaceType()
        {
            lock (READ_LOCK)
            {
                if (WOWRacePropertiesByRaceType.Count == 0)
                    PopulateRaceProperties();
                return WOWRacePropertiesByRaceType;
            }
        }

        private static void PopulateRaceProperties()
        {
            WOWRacePropertiesByRaceType.Clear();

            // Load in the race alignments
            string racePropertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerWOWRaceProperties.csv");
            Logger.WriteDebug("Populating player race properties via file '" + racePropertiesFile + "'");
            List<Dictionary<string, string>> racePropertiesRows = FileTool.ReadAllRowsFromFileWithHeader(racePropertiesFile, "|");
            foreach (Dictionary<string, string> columns in racePropertiesRows)
            {
                PlayerWOWRaceProperties raceProperties = new PlayerWOWRaceProperties();
                raceProperties.WOWRaceType = (RaceType)int.Parse(columns["RaceID"]);
                string alignmentString = columns["Alignment"].Trim().ToLower();
                switch (alignmentString)
                {
                    case "evil": raceProperties.Alignment = CreatureFactionAlignmentType.Evil; break;
                    case "good": raceProperties.Alignment = CreatureFactionAlignmentType.Good; break;
                    case "neutral": raceProperties.Alignment = CreatureFactionAlignmentType.Neutral;  break;
                    default:
                        {
                            Logger.WriteError("Race properties error, as the alignment string '" + alignmentString + "' has no mapping");
                        } break;
                }
                raceProperties.HasSlam = columns["HasSlam"].Trim() == "1" ? true : false;
                WOWRacePropertiesByRaceType.Add(raceProperties.WOWRaceType, raceProperties);
            }
        }
    }
}
