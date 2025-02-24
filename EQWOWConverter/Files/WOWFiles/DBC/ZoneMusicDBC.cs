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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class ZoneMusicDBC : DBCFile
    {
        public void AddRow(int Id, string setName, int soundIDDay, int soundIDNight)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(Id);
            newRow.AddString(setName);
            newRow.AddInt32(180000); // Silence Interval Min - Day
            newRow.AddInt32(180000); // Silence Interval Min - Night
            newRow.AddInt32(300000); // Silence Interval Max - Day
            newRow.AddInt32(300000); // Silence Interval Max - Night
            newRow.AddInt32(soundIDDay);
            newRow.AddInt32(soundIDNight);
            Rows.Add(newRow);
        }
    }
}
