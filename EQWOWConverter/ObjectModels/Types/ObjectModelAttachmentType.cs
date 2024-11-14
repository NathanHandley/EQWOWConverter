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

namespace EQWOWConverter.ObjectModels.Types
{
    internal enum ObjectModelAttachmentType : UInt16
    {
        Shield_MountMain_ItemVisual0 = 0,
        HandRight_ItemVisual1 = 1,
        HandLeft_ItemVisual2 = 2,
        ElbowRight_ItemVisual3 = 3,
        ElbowLeft_ItemVisual4 = 4,
        ShoulderRight = 5,
        ShoulderLeft = 6,
        KneeRight = 7,
        KneeLeft = 8,
        HipRight = 9,
        HipLeft = 10,
        Helm = 11,
        Back = 12,
        ShoulderFlapRight = 13,
        ShoulderFlapLeft = 14,
        ChestBloodFront = 15,
        ChestBloodBack = 16,
        Breath = 17,
        PlayerName = 18,
        Base = 19,
        Head = 20,
        SpellLeftHand = 21,
        SpellRightHand = 22,
        Special1 = 23,
        Special2 = 24,
        Special3 = 25,
        SheathMainHand = 26,
        SheathOffHand = 27,
        SheathShield = 28,
        PlayerNameMounted = 29,
        LargeWeaponLeft = 30,
        LargeWeaponRight = 31,
        HipWeaponLeft = 32,
        HipWeaponRight = 33,
        Chest = 34,
        HandArrow = 35,
        /// skipping some that will never be used
        LeftFoot = 47, 
        RightFoot = 48,
        ShieldNoGlove = 49,
        SpineLow = 50
    }
}
