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
    internal class SpellTalentExclusion
    {
        private static readonly object SpellTalentExclusionLock = new object();
        private static List<SpellTalentExclusion> SpellTalentExclusions = new List<SpellTalentExclusion>();

        public int TalentRank1SpellID = 0;
        public string WOWClassName = string.Empty;
        public string TalentName = string.Empty;
        public string DescriptionOrReason = string.Empty;

        public static List<SpellTalentExclusion> GetSpellTalentExclusions()
        {
            lock (SpellTalentExclusionLock)
            {
                if (SpellTalentExclusions.Count == 0)
                    PopulateSpellTalentExclusions();
                return SpellTalentExclusions;
            }
        }

        private static void PopulateSpellTalentExclusions()
        {
            string exclusionsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellTalentExclusions.csv");
            Logger.WriteDebug(string.Concat("Populating spell talent exclusions via file '", exclusionsFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(exclusionsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                SpellTalentExclusion newExclusion = new SpellTalentExclusion();
                if (int.TryParse(columns["talent_rank1_spell_id"].Trim(), out newExclusion.TalentRank1SpellID) == false
                    || newExclusion.TalentRank1SpellID <= 0)
                {
                    Logger.WriteError("SpellTalentExclusions row had an invalid talent_rank1_spell_id of '", columns["talent_rank1_spell_id"], "'");
                    continue;
                }
                newExclusion.WOWClassName = columns["wow_class"].Trim();
                newExclusion.TalentName = columns["talent_name"].Trim();
                newExclusion.DescriptionOrReason = columns["description_or_reason"].Trim();
                SpellTalentExclusions.Add(newExclusion);
            }
            Logger.WriteDebug(string.Concat("Populating spell talent exclusions complete, ", SpellTalentExclusions.Count.ToString(), " loaded"));
        }
    }
}
