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
    internal class RivervaleZoneProperties : ZoneProperties
    {
        public RivervaleZoneProperties() : base()
        {
            // TODO: Bug - You can fall through the waterfall base
            // TODO: 2D Ambience (night, wind)
            SetBaseZoneProperties("rivervale", "Rivervale", 45.3f, 1.6f, 3.8f, 0, ZoneContinentType.Antonica);
            SetZonewideMusic("rivervale-00", "rivervale-00", true);
            SetZonewideAmbienceSound("", "night");
            Enable2DSoundInstances("wind_lp4", "wind_lp2");

            AddZoneArea("Bank of Rivervale", "", "", false, "", "silence");
            AddZoneAreaBox("Bank of Rivervale", 134.691025f, -143.286545f, 150f, 67.445671f, -214.820267f, -50f);
            AddZoneAreaBox("Bank of Rivervale", 122.784729f, -133.233475f, 150f, 67.445671f, -214.820267f, -50f);
            AddZoneAreaBox("Bank of Rivervale", 99.532059f, -118.852730f, 150f, 24.082541f, -199.351913f, -50f);

            AddZoneArea("Fiddy's Fishing Dock");
            AddZoneAreaBox("Fiddy's Fishing Dock", 48.544201f, -308.484436f, 150f, -24.710560f, -412.258972f, -50f);
            AddZoneAreaBox("Fiddy's Fishing Dock", 48.544201f, -308.484436f, 150f, -23.273870f, -442.484894f, -50f);
            AddZoneAreaBox("Fiddy's Fishing Dock", 64.205719f, -353.372650f, 150f, -23.273870f, -442.484894f, -50f);

            AddZoneArea("Weary Foot Rest Inn", "", "", false, "", "silence");
            AddZoneAreaBox("Weary Foot Rest Inn", 227.600006f, 17.621901f, 150f, 120.088417f, -59.832321f, -50f);
            AddZoneAreaBox("Weary Foot Rest Inn", 227.600006f, 17.621901f, 150f, 143.445099f, -84.469452f, -50f);
            AddZoneAreaBox("Weary Foot Rest Inn", 227.600006f, 17.621901f, 150f, 155.618820f, -108.269447f, -50f);
            AddZoneAreaBox("Weary Foot Rest Inn", 252.633408f, 17.621901f, -8.470640f, 68.737541f, -156.572906f, -50f);

            AddZoneArea("Bristlebane's Hall", "", "", false, "", "silence");
            AddZoneAreaBox("Bristlebane's Hall", -204.553574f, -335.878082f, -15.653620f, -253.162643f, -432.098236f, -50f);

            AddZoneArea("Fool's Gold", "", "", false, "", "silence");
            AddZoneAreaOctagonBox("Fool's Gold", -25.015270f, -94.932991f, -156.925949f, -226.882187f, -179.992004f, -203.906723f, -179.962494f, -203.859238f, -48.034370f, -72.003288f, -48.034370f, -72.003288f, 150f, -50f);

            SetZonewideEnvironmentAsOutdoorsWithSky(144, 151, 144, ZoneFogType.Heavy, 0.5f, 1f);
            AddZoneLineBox("kithicor", 2012.985229f, 3825.189209f, 462.250427f, ZoneLineOrientationType.South, -384.065887f, -275.682556f, 22.469000f, -396.650330f, -290.013977f, -0.499910f);
            AddZoneLineBox("misty", 407.486847f, -2571.641357f, -10.749720f, ZoneLineOrientationType.West, -69.729698f, 134.790482f, 22.466999f, -96.162209f, 113.427109f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 215.891556f, -251.910565f, -15.339250f, -512.838562f, -11.999970f, 200f); // South lake (lower)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 373.244324f, -227.197754f, 215.881556f, -336.578491f, -7.999990f, 200f); // North lake (higher)
        }
    }
}
