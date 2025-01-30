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
    internal enum ItemWOWStatType : Int32
    {
        Mana = 0,
        Health = 1,
        Agility = 2,
        Strength = 4,
        Intellect = 5,
        Spirit = 6,
        Stamina = 7,
        DefenseSkillRating = 12,
        DodgeRating = 13,
        ParryRating = 14,
        BlockRating = 15,
        SpellHitRating = 18,
        HitRating = 31,
        CritRating = 32,
        HasteRating = 36,
        SpellPower = 45,
        HealthRegen = 46,
        BlockValue = 48
    }
}
