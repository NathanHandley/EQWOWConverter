﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Tradeskills
{
    // Mapping
    // EQ Alchemy => WOW Alchemy
    // EQ Baking => WOW Cooking
    // EQ Blacksmithing => WOW Blacksmithing
    // EQ Brewing => WOW Cooking
    // EQ Fletching => WOW Engineering
    // EQ JewelryMaking => WOW Jewelcrafting
    // EQ Poison Making => WOW Alchemy
    // EQ Pottery => WOW Blacksmithing
    // EQ Research => WOW Inscription
    // EQ Tailoring => WOW Tailoring
    // EQ Tinkering => WOW Engineering
    // EQ Fishing => WOW Cooking
    // N/A => WOW Enchanting (just the required enchanting spells)

    internal enum TradeskillType : Int32
    {
        Unknown = -1,
        None = 0,
        Alchemy = 1,
        Blacksmithing = 2,
        Cooking = 3,
        Engineering = 4,
        Jewelcrafting = 5,
        Inscription = 6,
        Tailoring = 7,
        Enchanting = 8
    }
}
