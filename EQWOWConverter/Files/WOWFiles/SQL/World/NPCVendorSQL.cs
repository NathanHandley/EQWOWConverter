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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class NPCVendorSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM `npc_vendor` WHERE `entry` >= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString() + " AND `entry` <= " + Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH + ";";
        }

        public void AddRow(int creatureTemplateID, int itemID, int slot)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", creatureTemplateID);
            newRow.AddInt("slot", slot);
            newRow.AddInt("item", itemID);
            newRow.AddInt("maxcount", 0);
            newRow.AddInt("incrtime", 0);
            newRow.AddInt("ExtendedCost", 0);
            newRow.AddInt("VerifiedBuild", 0);
            Rows.Add(newRow);
        }
    }
}
