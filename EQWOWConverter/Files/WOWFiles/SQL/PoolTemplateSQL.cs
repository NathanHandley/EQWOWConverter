﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class PoolTemplateSQL : SQLFile
    {
        public override string DeleteRowSQL()
        {
            return "DELETE FROM pool_template WHERE entry >= " + Configuration.CONFIG_SQL_POOL_TEMPLATE_ID_START.ToString() + " AND entry <= " + Configuration.CONFIG_SQL_POOL_TEMPLATE_ID_END.ToString() + " ;";
        }

        public void AddRow(int entryID, string description, int maxLimit)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entry", entryID);
            newRow.AddInt("max_limit", maxLimit);
            newRow.AddString("description", 255, description);
            Rows.Add(newRow);
        }
    }
}