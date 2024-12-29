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

using EQWOWConverter.Common;
using EQWOWConverter.Zones;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EQWOWConverter.Items;

namespace EQWOWConverter.WOWFiles
{
    internal class ItemDBC : DBCFile
    {
        public void AddRow(ItemTemplate itemTemplate)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(itemTemplate.EntryID);
            newRow.AddInt(itemTemplate.ClassID);
            newRow.AddInt(itemTemplate.SubClassID);
            newRow.AddInt(-1); // Sound Override SubclassID
            newRow.AddInt(itemTemplate.Material);
            newRow.AddInt(itemTemplate.DisplayID);
            newRow.AddInt(itemTemplate.InventoryType);
            newRow.AddInt(itemTemplate.SheatheType);
            Rows.Add(newRow);
        }
    }
}
