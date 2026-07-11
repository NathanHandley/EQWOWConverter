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

using EQWOWConverter.Creatures;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class ModEverquestCreatureEmoteSQL : SQLFile
    {
        private static int CUR_ID = 0;

        public override string DeleteRowSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DROP TABLE IF EXISTS `mod_everquest_creature_emote`; ");
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS `mod_everquest_creature_emote` ( ");
            stringBuilder.AppendLine("`ID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`CreatureTemplateID` INT(10) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EventType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EmoteType` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`ChancePct` FLOAT NOT NULL DEFAULT '100', ");
            stringBuilder.AppendLine("`Param1` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`Param2` INT(10) NOT NULL DEFAULT '0', ");
            stringBuilder.AppendLine("`EmoteText` VARCHAR(512) NOT NULL DEFAULT '', ");
            stringBuilder.AppendLine("PRIMARY KEY(`ID`) USING BTREE ); ");
            return stringBuilder.ToString();
        }

        public void AddRow(int creatureTemplateID, CreatureEmote emote)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", CUR_ID);
            newRow.AddInt("CreatureTemplateID", creatureTemplateID);
            newRow.AddInt("EventType", (int)emote.EventType);
            newRow.AddInt("EmoteType", (int)emote.EmoteType);
            newRow.AddFloat("ChancePct", emote.ChancePct);
            newRow.AddInt("Param1", emote.Param1);
            newRow.AddInt("Param2", emote.Param2);
            newRow.AddString("EmoteText", 512, emote.Text);
            Rows.Add(newRow);

            CUR_ID++;
        }
    }
}
