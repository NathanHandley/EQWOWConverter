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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class NorthFreeportZoneProperties : ZoneProperties
    {
        public NorthFreeportZoneProperties() : base()
        {
            SetBaseZoneProperties("freportn", "North Freeport", 211f, -296f, 4f, 0, ZoneContinentType.Antonica);
            AddValidMusicInstanceTrackIndexes(5, 7, 8, 9, 10, 12, 13);
            AddZoneLineBox("freportw", 1588.414673f, -278.419495f, 0.000050f, ZoneLineOrientationType.East, 378.034851f, 718.198425f, -1.531000f, 361.772491f, 697.030884f, -14.499990f);
            AddZoneLineBox("freportw", 728.335388f, -581.244812f, -20.999701f, ZoneLineOrientationType.South, -15.071440f, -433.618988f, -11.531000f, -34.966301f, -454.098175f, -50f);
            AddZoneLineBox("freportw", 211.309326f, -124.670799f, -14.000000f, ZoneLineOrientationType.South, -429.537323f, 504.799438f, 14.500150f, -490.004974f, 475.620117f, -14.500010f);
            AddZoneLineBox("freportw", 252.782593f, -698.531494f, -27.999969f, ZoneLineOrientationType.South, -378.454254f, -67.828621f, 0.500040f, -448.004974f, -98.161171f, -28.499950f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -15.130130f, 362.879608f, -41.071400f, 336.649323f, -12.999990f, 10f); // Outside Temple of Marr
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 142.245224f, 363.727295f, 59.144009f, 336.929474f, -30.999981f, 10f); // Inside Temple of Marr, bottom part of fountain
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 151.630280f, 360.173492f, 142.563675f, 339.751465f, -25.000000f, 10f); // Inside Temple of Marr, top part of fountain 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 144.363464f, 358.713867f, 141.006744f, 341.559204f, -25.000000f, 10f); // Inside Temple of Marr, top part of fountain 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 142.370056f, 356.783447f, 138.959152f, 343.221436f, -25.000000f, 10f); // Inside Temple of Marr, top part of fountain + fall
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 548.706177f, 660.475281f, 495.713226f, 538.462585f, -20.999960f, 40f); // NW corner channel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 500.166443f, 574.449402f, 251.204926f, 377.377808f, -20.999960f, 40f); // NW Area through bridge
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 281.622162f, 406.833252f, 152.311096f, -1.537300f, -20.999960f, 40f); // NW area up to Hall of Truth
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 314.695953f, 4.034020f, -70.747742f, -504.712555f, -20.999960f, 40f); // Around Hall of Truth through back water exit
        }
    }
}
