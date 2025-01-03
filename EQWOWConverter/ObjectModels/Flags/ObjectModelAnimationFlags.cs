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

namespace EQWOWConverter.ObjectModels
{
    internal enum ObjectModelAnimationFlags : UInt32
    {
        Set0x80OnLoad                   = 0x01, // ? found this in the 010 Editor template
        Unknown1                        = 0x02,
        Unknown2                        = 0x04,
        Unknown3                        = 0x08,
        LoadLowPrioritySequence         = 0x10,
        AnimationInM2                   = 0x20,
        AliasedWithFollowupAnimation    = 0x40,
        BlendedAnimation                = 0x80,
        SequenceStored0x                = 0x100,
        BlindTimeInAndOut               = 0x200
    }
}
