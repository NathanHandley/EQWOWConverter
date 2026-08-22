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

using EQWOWConverter.Spells;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class EQSpellsLUA
    {
        public static void Generate(List<SpellTemplate> spellTemplates)
        {
            // Build body content
            StringBuilder sb = new StringBuilder();
            
            // Reference line for skill IDs
            sb.Append("local Abjuration, Alteration, Brass, Conjuration, Divination, Evocation, Singing, Stringed, Wind, Percussion, Combat = ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Abjuration).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Alteration).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Brass).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Conjuration).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Divination).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Evocation).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Singing).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Stringed).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Wind).ToString());
            sb.Append(", ");
            sb.Append(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Percussion).ToString());
            sb.Append(", ");
            sb.AppendLine(SkillLineDBC.GetIDForSkillCatagory(SpellEQSkillCategory.Combat).ToString());
            sb.AppendLine("");

            // Spell Map
            sb.AppendLine("EQ_SPELLS = {");
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                if (spellTemplate.EQSkillCategory != SpellEQSkillCategory.Unknown)
                {
                    sb.Append("[");
                    sb.Append(spellTemplate.WOWSpellID.ToString());
                    sb.Append("] = ");
                    sb.Append(spellTemplate.EQSkillCategory.ToString());
                    sb.AppendLine(",");
                }
            }
            sb.AppendLine("}");
            sb.AppendLine("");

            // Icon references
            sb.AppendLine("EQ_SKILLLINES = {");
            sb.AppendLine("{ id = Abjuration,  name = \"Abjuration\",  icon = \"Interface\\\\Icons\\\\inv_eq_abjuration\" },");
            sb.AppendLine("{ id = Alteration,  name = \"Alteration\",  icon = \"Interface\\\\Icons\\\\inv_eq_alteration\" },");
            sb.AppendLine("{ id = Brass,       name = \"Brass\",       icon = \"Interface\\\\Icons\\\\inv_eq_brass\" },");
            sb.AppendLine("{ id = Conjuration, name = \"Conjuration\", icon = \"Interface\\\\Icons\\\\inv_eq_conjuration\" },");
            sb.AppendLine("{ id = Divination,  name = \"Divination\",  icon = \"Interface\\\\Icons\\\\inv_eq_divination\" },");
            sb.AppendLine("{ id = Evocation,   name = \"Evocation\",   icon = \"Interface\\\\Icons\\\\inv_eq_evocation\" },");
            sb.AppendLine("{ id = Singing,     name = \"Singing\",     icon = \"Interface\\\\Icons\\\\inv_eq_singing\" },");
            sb.AppendLine("{ id = Stringed,    name = \"Stringed\",    icon = \"Interface\\\\Icons\\\\inv_eq_stringed\" },");
            sb.AppendLine("{ id = Wind,        name = \"Wind\",        icon = \"Interface\\\\Icons\\\\inv_eq_wind\" },");
            sb.AppendLine("{ id = Percussion,  name = \"Percussion\",  icon = \"Interface\\\\Icons\\\\inv_eq_percussion\" },");
            sb.AppendLine("{ id = Combat,      name = \"Combat\",      icon = \"Interface\\\\Icons\\\\inv_eq_combat\" },");
            sb.AppendLine("}");
            sb.AppendLine("");

            // Icons offered when a player makes their own spellbook tab
            sb.AppendLine("EQ_SPELL_TAB_ICONS = {");
            SortedDictionary<int, string> spellIconNamesByIndex = GetSpellTabIconNamesByIndex();
            foreach (KeyValuePair<int, string> spellIconNameByIndex in spellIconNamesByIndex)
            {
                sb.Append("\"Interface\\\\Icons\\\\");
                sb.Append(spellIconNameByIndex.Value);
                sb.AppendLine("\",");
            }
            sb.AppendLine("}");

            // Write it
            string outputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady", "Interface", "FrameXML");
            if (Directory.Exists(outputFolder) == false)
                Directory.CreateDirectory(outputFolder);
            string outputFileFullPath = Path.Combine(outputFolder, "EQSpells.lua");
            if (File.Exists(outputFileFullPath) == true)
                File.Delete(outputFileFullPath);
            File.WriteAllText(outputFileFullPath, sb.ToString());
        }

        private static SortedDictionary<int, string> GetSpellTabIconNamesByIndex()
        {
            SortedDictionary<int, string> iconNamesByIndex = new SortedDictionary<int, string>();
            string spellIconFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "spellicons");
            if (Directory.Exists(spellIconFolder) == false)
            {
                Logger.WriteError("Could not build the spellbook tab icon list, as no folder was at '" + spellIconFolder + "'");
                return iconNamesByIndex;
            }
            string[] spellIconFilePaths = Directory.GetFiles(spellIconFolder, "*.blp");
            foreach (string spellIconFilePath in spellIconFilePaths)
            {
                string iconName = Path.GetFileNameWithoutExtension(spellIconFilePath);
                int lastUnderscoreIndex = iconName.LastIndexOf('_');
                if (lastUnderscoreIndex == -1 || lastUnderscoreIndex == iconName.Length - 1)
                    continue;
                int iconIndex;
                if (int.TryParse(iconName.Substring(lastUnderscoreIndex + 1), out iconIndex) == false)
                    continue;
                if (iconNamesByIndex.ContainsKey(iconIndex) == true)
                    continue;
                iconNamesByIndex.Add(iconIndex, iconName);
            }
            return iconNamesByIndex;
        }
    }
}
