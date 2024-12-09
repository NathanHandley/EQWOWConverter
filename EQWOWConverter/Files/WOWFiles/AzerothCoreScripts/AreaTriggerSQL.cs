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
    internal class AreaTriggerSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM areatrigger WHERE `entry` >= " + Configuration.CONFIG_DBCID_AREATRIGGER_ID_START.ToString() + " AND `entry` <= " + AreaTriggerDBC.CURRENT_AREATRIGGER_ID + " ;";
        }

        public void AddRow(int areaTriggerID, int mapID, float positionX, float positionY, float positionZ,
            float boxLength, float boxWidth, float boxHeight, float boxOrientation)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", areaTriggerID);
            newRow.AddInt("map", mapID);
            newRow.AddFloat("x", positionX);
            newRow.AddFloat("y", positionY);
            newRow.AddFloat("z", positionZ);
            newRow.AddFloat("radius", 0);
            newRow.AddFloat("length", boxLength);
            newRow.AddFloat("width", boxWidth);
            newRow.AddFloat("height", boxHeight);
            newRow.AddFloat("orientation", boxOrientation);
        }
    }
}
