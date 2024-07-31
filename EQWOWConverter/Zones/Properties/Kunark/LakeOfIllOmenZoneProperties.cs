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

namespace EQWOWConverter.Zones.Properties
{
    internal class LakeOfIllOmenZoneProperties : ZoneProperties
    {
        public LakeOfIllOmenZoneProperties() : base()
        {
            SetBaseZoneProperties("lakeofillomen", "Lake of Ill Omen", -5383.07f, 5747.14f, 68.27f, 0, ZoneContinent.Kunark);
            SetFogProperties(235, 235, 235, 200, 800);
            AddZoneLineBox("cabwest", -802.654480f, 767.458740f, -0.000070f, ZoneLineOrientationType.North,
                6577.715820f, -6613.837891f, 145.213730f, 6533.130859f, -6645.066895f, 34.593719f);
            AddZoneLineBox("cabwest", -985.943787f, 584.806458f, 0.000380f, ZoneLineOrientationType.East,
                6344.193848f, -6799.043945f, 182.103806f, 6315.685547f, -6843.227051f, 34.595600f);
        }
    }
}
