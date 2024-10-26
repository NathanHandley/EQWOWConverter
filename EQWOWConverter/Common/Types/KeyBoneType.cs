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

namespace EQWOWConverter.Common
{
    internal enum KeyBoneType : Int32
    {
        None = -1,
        ArmL = 0,
        ArmR = 1,
        ShoulderL = 2,
        ShoulderR = 3,
        SpineLow = 4,
        Waist = 5,
        Head = 6,
        Jaw = 7,
        IndexFingerR = 8,
        MiddleFingerR = 9,
        PinkyFingerR = 10,
        RingFingerR = 11,
        ThumbR = 12,
        IndexFingerL = 13,
        MiddleFingerL = 14,
        PinkyFingerL = 15,
        RingFingerL = 16,
        ThumbL = 17,
        BTH = 18, // $BTH
        CSR = 19, // $CSR
        CSL = 20, // $CSL
        _Breath = 21,
        _Name = 22,
        _NameMount = 23,
        CHD = 24, // $CHD
        CCH = 25, // $CCH
        Root = 26,
        Wheel1 = 27,
        Wheel2 = 28,
        Wheel3 = 29,
        Wheel4 = 30,
        Wheel5 = 31,
        Wheel6 = 32,
        Wheel7 = 33,
        Wheel8 = 34
    }
}
