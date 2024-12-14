//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class AreaTriggerTeleportSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM areatrigger_teleport WHERE `Name` LIKE 'EQ %';";
        }

        public void AddRow(int areaTriggerDBCID, string descriptiveName, int targetMapID, float targetPositionX, float targetPositionY, 
            float targetPositionZ, float targetOrientation)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("ID", areaTriggerDBCID);
            newRow.AddString("Name", descriptiveName);
            newRow.AddInt("target_map", targetMapID);
            newRow.AddFloat("target_position_x", targetPositionX);
            newRow.AddFloat("target_position_y", targetPositionY);
            newRow.AddFloat("target_position_z", targetPositionZ);
            newRow.AddFloat("target_orientation", targetOrientation);
            Rows.Add(newRow);
        }
    }
}
