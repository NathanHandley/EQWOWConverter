//  Author: Nathan Handley(nathanhandley@protonmail.com)
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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Items
{
    internal enum ItemEquipSlotBitmaskType : Int32
    {
        None1 = 0,
        None2 = 1,
        Ear1 = 2,
        Head = 4,
        Face = 8,
        Ear2 = 16,
        Neck = 32,
        Shoulder = 64,
        Arms = 128,
        Back = 256,
        Wrist2 = 512,
        Wrist1 = 1024,
        Ranged = 2048, // ? And Ammo?
        Hands = 4096,
        Primary = 8192,
        Secondary = 16384,
        Ring1 = 32768,
        Ring2 = 65536,
        Chest = 131072,
        Legs = 262144,
        Feet = 524288,
        Waist = 1048576,
        Ammo = 2097152
    }
}
