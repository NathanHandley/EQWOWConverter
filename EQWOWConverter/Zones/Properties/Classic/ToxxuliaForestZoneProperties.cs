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
    internal class ToxxuliaForestZoneProperties : ZoneProperties
    {
        public ToxxuliaForestZoneProperties() : base()
        {
            SetBaseZoneProperties("tox", "Toxxulia Forest", 203f, 2295f, -45f, 0, ZoneContinentType.Odus);
            //AddValidMusicInstanceTrackIndexes(0, 1, 3);
            SetZonewideEnvironmentAsOutdoorsNoSky(138, 125, 19, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            AddZoneLineBox("erudnext", -1552.149292f, -184.036606f, -47.968700f, ZoneLineOrientationType.North, 2574.356934f, 305.599121f, -33.937248f, 2550.955078f, 289.213013f, -48.907711f);
            AddZoneLineBox("kerraridge", 416.010834f, -930.879211f, 20.000179f, ZoneLineOrientationType.West, -495.140961f, 2684.400635f, -19.784010f, -527.409973f, 2655.238281f, -38.749310f);
            AddZoneLineBox("paineel", 852.573181f, 196.109207f, 0.000050f, ZoneLineOrientationType.West, -2613.365479f, -417.686676f, -26.624750f, -2628.005371f, -470f, -45.593510f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 2817.309326f, 2545.707275f, -2965.405029f, 1149.184570f, -60.686829f, 250f);  // Ocean
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -121.392090f, 1149.194570f, -904.174866f, 604.196167f, -60.686829f, 250f);  // Mouth of the river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -91.627731f, 647.161072f, -664.092773f, 276.586853f, -60.686829f, 250f);  // East of mouth of the river
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 802.973267f, 276.806671f, -327.662445f, -70.251373f, -60.686829f, 250f);  // Bridge river part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 790.337036f, -70.241373f, 415.553131f, -957.950867f, -60.686829f, 250f);  // River north of River
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 1089.210815f, -680.044861f, 790.327036f, -1068.042603f, -60.686829f, 250f); // River bend towards the source
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 1151.062866f, -976.408508f, 643.941528f, -1844.147217f, -60.686829f, 250f); // River source
        }
    }
}
