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
            Enable2DSoundInstances("wind_lp4");

            AddZoneArea("Temple of Marr", "freportn-12", "freportn-12");
            AddZoneAreaBox("Temple of Marr", 154.301849f, 404.686890f, 1000f, 117.155739f, 294.509674f, -1000f);
            AddZoneAreaBox("Temple of Marr", 137.158463f, 420.520691f, 1000f, -110.719597f, 279.679901f, -1000f);
            AddZoneAreaBox("Temple of Marr", 121.776512f, 437.634430f, 1000f, 15.115140f, 265.146179f, -1000f);
            AddZoneAreaBox("Temple of Marr", 111.346077f, 447.850525f, 1000f, 28.500401f, 251.722656f, -1000f);
            
            AddZoneArea("South Block", "freportn-07", "freportn-07");
            AddZoneAreaBox("South Block", -102.995468f, 426.914612f, 1000f, -245.911804f, 151.320679f, -1000f);
            AddZoneAreaBox("South Block", -0.161060f, 278.291382f, 1000f, -229.072342f, 139.537857f, -1000f);
            
            AddChildZoneArea("Marsheart's Chords", "West Block");
            AddZoneAreaBox("Marsheart's Chords", -70.163483f, 587.442444f, 100f, -153.691833f, 532.022095f, -100);
            
            AddChildZoneArea("City Hall", "West Block");
            AddZoneAreaBox("City Hall", -126.164543f, 503.603455f, 100f, -265.648254f, 462.070038f, -100);
            AddZoneAreaBox("City Hall", -210.213806f, 559.525208f, 100f, -265.648254f, 462.070038f, -100);
            
            AddChildZoneArea("Groflah's Forge", "West Block");
            AddZoneAreaBox("Groflah's Forge", -0.228160f, 545.561951f, 100f, -41.827419f, 447.947174f, -100);
            AddZoneAreaBox("Groflah's Forge", -0.228160f, 489.711792f, 100f, -97.880783f, 447.947174f, -100);
            
            AddZoneArea("West Block", "freportn-09", "freportn-13");
            AddZoneAreaBox("West Block", 13.415270f, 593.730652f, 1000f, -300.651031f, 426.265411f, -1000f);

            if (Configuration.AUDIO_USE_ALTERNATE_TRACKS == true)
            {
                //AddZoneArea("Hall of Truth", "freportn-14", "freportn-14");
                AddZoneArea("Office of the People", "freportn-04", "freportn-04");
                AddZoneArea("The Jade Tiger's Den", "freportn-01", "freportn-01");
            }
            else
            {
                //AddZoneArea("Hall of Truth", "freportn-10", "freportn-10", false);
                AddZoneArea("Office of the People");     
                AddZoneArea("The Jade Tiger's Den");
            }
            AddZoneArea("Hall of Truth", "freportn-10", "freportn-10", false);
            AddZoneAreaBox("Hall of Truth", 349.568451f, 174.375458f, 1000f, 126.620598f, 15.954200f, -1000f);
            AddZoneAreaBox("Hall of Truth", 349.568451f, 27.683069f, 1000f, 98.723747f, -230.993713f, -1000f);
            AddZoneAreaBox("Hall of Truth", 349.568451f, -124.064507f, 1000f, 85.137863f, -230.993713f, -1000f);
            AddZoneAreaBox("Hall of Truth", 349.568451f, -170.485733f, 1000f, 44.162498f, -230.993713f, -1000f);
            AddZoneAreaBox("Office of the People", 349.867615f, 504.611694f, 1000f, 196.286224f, 174.142166f, -1000f);
            AddZoneAreaBox("Office of the People", 349.867615f, 297.895874f, 1000f, 186.657715f, 174.142166f, -1000f);
            AddZoneAreaBox("Office of the People", 349.867615f, 284.342682f, 1000f, 172.777039f, 174.142166f, -1000f);
            AddZoneAreaBox("Office of the People", 349.867615f, 272.726349f, 1000f, 161.496704f, 184.404633f, -1000f);
            AddZoneAreaBox("The Jade Tiger's Den", 127.223984f, 111.681374f, 1000f, 0.377760f, 28.328440f, -1000f);

            AddZoneArea("Bank of Freeport", "freportn-05", "freportn-05");           
            AddZoneAreaBox("Bank of Freeport", 52.053120f, -167.295944f, 1000f, -69.702911f, -251.599686f, -1000f);
            
            AddZoneArea("The Emporium");
            AddZoneAreaBox("The Emporium", 98.126373f, -70.289772f, 1000f, 14.231530f, -167.680298f, -1000f);
            
            AddZoneArea("Tassel's Tavern", "freportn-08", "freportn-08");
            AddZoneAreaBox("Tassel's Tavern", 97.693863f, 27.693460f, 1000f, 42.200272f, -69.864670f, -1000f);
            AddZoneAreaBox("Tassel's Tavern", 97.693863f, 27.693460f, 1000f, 27.387091f, -27.503710f, -1000f);
            
            AddZoneArea("Palola's Inn", "freportn-07", "freportn-07");
            AddZoneAreaBox("Palola's Inn", -140.083038f, 27.932680f, 1000f, -184.513947f, -29.577450f, -1000f);
            AddZoneAreaBox("Palola's Inn", -140.166840f, -28.005939f, 1000f, -209.660217f, -97.695213f, -1000f);
            
            AddZoneArea("North Tunnels", "", "", false, "caveloop", "caveloop");
            AddZoneAreaBox("North Tunnels", 498.394409f, 765.014404f, 100, 304.263275f, 576.443298f, -300f);
            
            AddZoneArea("Northeast Tunnels");
            AddZoneAreaBox("Northeast Tunnels", 142.614716f, -229.190308f, 100, 38.626499f, -309.047577f, -300f);
            AddZoneAreaBox("Northeast Tunnels", 185.648834f, -264.480988f, 100, -86.916962f, -540.348816f, -300f);
            
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
