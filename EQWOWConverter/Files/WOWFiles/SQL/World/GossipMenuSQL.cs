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
    internal class GossipMenuSQL : SQLFile
    {
        private static int CUR_MENU_ID = Configuration.SQL_GOSSIPMENU_MENUID_START;

        public override string DeleteRowSQL()
        {
            return "DELETE FROM `gossip_menu` WHERE `MenuID` >= " + Configuration.SQL_GOSSIPMENU_MENUID_START.ToString() + " AND `MenuID` <= " + Configuration.SQL_GOSSIPMENU_MENUID_END + ";";
        }

        public void AddRow(int menuID, int textID)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("MenuID", menuID);
            newRow.AddInt("TextID", textID);
            Rows.Add(newRow);
        }

        public static int GenerateUniqueMenuID()
        {
            int returnVal = CUR_MENU_ID;
            CUR_MENU_ID++;
            return returnVal;
        }
    }
}
