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

using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class PlayerCreateInfoSkillsSQL : SQLFile
    {
        // Every skill in the weapon category (SkillLine.dbc categoryId 6), which is the group blizzard race gated
        private static readonly List<int> WEAPON_SKILL_IDS = new List<int>() { 43, 44, 45, 46, 54, 55, 95, 118, 136, 160, 162, 172, 173, 176, 226, 228, 229, 473 };

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("INSERT IGNORE INTO `playercreateinfo_skills` (`raceMask`, `classMask`, `skill`, `rank`, `comment`) ");
            stringBuilder.AppendLine("SELECT 0, `racegated`.`classMask`, `racegated`.`skill`, `racegated`.`rank`, `racegated`.`comment` ");
            stringBuilder.AppendLine("FROM (SELECT `classMask`, `skill`, `rank`, `comment` FROM `playercreateinfo_skills` ");
            stringBuilder.Append("      WHERE `raceMask` <> 0 AND `skill` IN (");
            for (int i = 0; i < WEAPON_SKILL_IDS.Count; i++)
            {
                stringBuilder.Append(WEAPON_SKILL_IDS[i].ToString());
                if (i < WEAPON_SKILL_IDS.Count - 1)
                    stringBuilder.Append(",");
            }
            stringBuilder.AppendLine(")) AS `racegated`; ");
            return stringBuilder.ToString();
        }
    }
}
