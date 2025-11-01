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

namespace EQWOWConverter.Items
{
    internal enum ItemWOWInventoryType: Int32
    {
        NoEquip = 0,
        Head = 1,
        Neck = 2,
        Shoulder = 3,
        Shirt = 4,
        Chest = 5, // Works the same as robe, 20
        Waist = 6,
        Legs = 7,
        Feet = 8,
        Wrists = 9,
        Hands = 10,
        Finger = 11,
        Trinket = 12,
        OneHand = 13,
        Shield = 14,
        Ranged = 15, // Bows
        Back = 16,
        TwoHand = 17,
        Bag = 18,
        Tabard = 19,
        Robe = 20, // Works the same as chest, 5
        MainHand = 21, // One-Hand, but only in main
        OffHand2 = 22, // Same as 13?
        HeldInOffHand = 23,
        Ammo = 24,
        Thrown = 25,
        RangedRight = 26, // Wands, Guns
        Quiver = 27,
        Relic = 28
    }
}
