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
    internal class CreatureHailText
    {
        private static List<CreatureHailText> HailTexts = new List<CreatureHailText>();
        private static readonly object HailTextLock = new object();

        public string ZoneShortName = string.Empty;
        public string CreatureName = string.Empty;
        public string Text = string.Empty;

        public static List<CreatureHailText> GetHailTexts()
        {
            lock (HailTextLock)
            {
                if (HailTexts.Count == 0)
                    PopulateHailTexts();
                return HailTexts;
            }
        }

        private static void PopulateHailTexts()
        {
            string hailTextsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureHailTexts.csv");
            Logger.WriteDebug(string.Concat("Populating creature hail texts via file '", hailTextsFile, "'"));
            if (File.Exists(hailTextsFile) == false)
            {
                Logger.WriteError(string.Concat("Could not load creature hail texts, file did not exist at '", hailTextsFile, "'"));
                return;
            }
            foreach (Dictionary<string, string> columns in FileTool.ReadAllRowsFromFileWithHeader(hailTextsFile, "|"))
            {
                string text = columns["hail_text"].Trim();
                if (text.Length == 0)
                    continue;
                CreatureHailText newHailText = new CreatureHailText();
                newHailText.ZoneShortName = columns["zone_shortname"];
                newHailText.CreatureName = columns["creature_name"];
                newHailText.Text = text;
                HailTexts.Add(newHailText);
            }
            Logger.WriteDebug(string.Concat("Populating creature hail texts complete, with ", HailTexts.Count.ToString(), " rows"));
        }
    }
}
