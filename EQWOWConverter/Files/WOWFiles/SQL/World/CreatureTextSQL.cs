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

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureTextSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `creature_text` WHERE `CreatureID` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `CreatureID` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRow(int creatureTemplateID, int groupID, int typeID, string messageText, int broadcastTextID, int durationInMS, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("CreatureID", creatureTemplateID);
            newRow.AddInt("GroupID", groupID);
            newRow.AddInt("ID", 0);
            newRow.AddString("Text", messageText);
            newRow.AddInt("Type", typeID); // 12 = Say, 14 = Yell, 16 = Emote, 41 = Boss Emote, 15 = Whisper, 42 = Boss Whisper
            newRow.AddInt("Language", 0);
            newRow.AddFloat("Probability", 100);
            newRow.AddInt("Emote", 0);
            newRow.AddInt("Duration", durationInMS);
            newRow.AddInt("Sound", 0);
            newRow.AddInt("BroadcastTextId", broadcastTextID);
            newRow.AddInt("TextRange", 0);
            newRow.AddString("comment", 255, comment);
            Rows.Add(newRow);
        }
    }
}