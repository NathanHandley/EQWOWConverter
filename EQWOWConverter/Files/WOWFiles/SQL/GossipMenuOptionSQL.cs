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

using EQWOWConverter.Spells;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class GossipMenuOptionSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `gossip_menu_option` WHERE `MenuID` >= " + Configuration.CONFIG_SQL_GOSSIPMENU_MENUID_START.ToString() + " AND `MenuID` <= " + Configuration.CONFIG_SQL_GOSSIPMENU_MENUID_END + ";";
        }

        public void AddRowForClassTrainer(int menuID, int optionID, int optionIcon, string optionText, int optionBroadcastTextID, int optionType,
            int optionNpcFlag, int actionMenuID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("MenuID", menuID);
            newRow.AddInt("OptionID", optionID);
            newRow.AddInt("OptionIcon", optionIcon);
            newRow.AddString("OptionText", optionText);
            newRow.AddInt("OptionBroadcastTextID", optionBroadcastTextID);
            newRow.AddInt("OptionType", optionType);
            newRow.AddInt("OptionNpcFlag", optionNpcFlag);
            newRow.AddInt("ActionMenuID", actionMenuID);
            newRow.AddInt("ActionPoiID", 0);
            newRow.AddInt("BoxCoded", 0);
            newRow.AddInt("BoxMoney", 0);
            newRow.AddString("BoxText", string.Empty);
            newRow.AddInt("BoxBroadcastTextID", 0);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }
    }
}
