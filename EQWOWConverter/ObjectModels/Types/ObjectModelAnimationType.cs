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

namespace EQWOWConverter.ObjectModels.WOW.Types
{
    internal enum ObjectModelAnimationType : UInt16
    {
        Stand           = 0,
        Death           = 0,
        Spell           = 2,
        Stop            = 3,
        Walk            = 4,
        Run             = 5,
        Dead            = 6,
        Rise            = 7,
        StandWound      = 8,
        CombatWound     = 9,
        CombatCritical  = 10,
        ShuffleLeft     = 11,
        ShuffleRight    = 12,
        AttackUnarmed   = 16,
        Attack1H        = 17,
        Attack2H        = 18,  
        ReadyUnarmed    = 25,
        Ready1H         = 26,
        Ready2H         = 27,
        SpelLCast       = 32,
        Fall            = 40,
        Swim            = 42,
        SwimLeft        = 43,
        SwimRight       = 44,
        SwimBackwards   = 45,           
    }
}
