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
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Items
{
    internal enum ItemWOWWeaponSubclassType : int
    {
        AxeOneHand = 0,
        AxeTwoHand = 1, 
        Bow = 2,
        Gun = 3,
        MaceOneHand = 4,
        MaceTwoHand = 5,
        Polearm = 6,
        SwordOneHand = 7,
        SwordTwoHand = 8,
        Staff = 10,
        FistWeapon = 13,
        Miscellaneous = 14, // Held items that have no weapon type, like blacksmith hammer
        Dagger = 15,
        Thrown = 16,
        Spear = 17,
        Crossbow = 18,
        Wand = 19,
        FishingPole = 20
    }
}
