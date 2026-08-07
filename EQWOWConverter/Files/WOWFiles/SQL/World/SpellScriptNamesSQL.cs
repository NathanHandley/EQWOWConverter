//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class SpellScriptNamesSQL : SQLFile
    {
        private List<string> ReplacedCoreScriptNames = new List<string>();

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DELETE FROM `spell_script_names` WHERE (`spell_id` >= ");
            stringBuilder.Append(Configuration.DBCID_SPELL_ID_START);
            stringBuilder.Append(" AND `spell_id` <= ");
            stringBuilder.Append(Configuration.DBCID_SPELL_ID_END);
            stringBuilder.AppendLine(") OR `ScriptName` LIKE 'EverQuest\\_%';");
            foreach (string replacedCoreScriptName in ReplacedCoreScriptNames)
            {
                stringBuilder.Append("DELETE FROM `spell_script_names` WHERE `ScriptName` = '");
                stringBuilder.Append(replacedCoreScriptName);
                stringBuilder.AppendLine("';");
            }
            return stringBuilder.ToString();
        }

        public void AddRow(int spellID, string scriptName)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("spell_id", spellID); // Negative spell IDs cover all ranks of a talent
            newRow.AddString("ScriptName", scriptName);
            Rows.Add(newRow);
        }

        public void AddCoreScriptReplacementRow(int spellID, string coreScriptName, string replacementScriptName)
        {
            ReplacedCoreScriptNames.Add(coreScriptName);
            AddRow(spellID, replacementScriptName);
        }
    }
}
