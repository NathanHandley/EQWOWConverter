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
    internal class RivervaleZoneProperties : ZoneProperties
    {
        public RivervaleZoneProperties() : base()
        {
            // TODO: Bug - You can fall through the waterfall base
            SetBaseZoneProperties("rivervale", "Rivervale", 45.3f, 1.6f, 3.8f, 0, ZoneContinentType.Antonica);
            AddValidMusicInstanceTrackIndexes(0);
            SetZonewideEnvironmentAsOutdoorsWithSky(144, 151, 144, ZoneFogType.Heavy, 0.5f, 1f);
            AddZoneLineBox("kithicor", 2012.985229f, 3825.189209f, 462.250427f, ZoneLineOrientationType.South, -384.065887f, -275.682556f, 22.469000f, -396.650330f, -290.013977f, -0.499910f);
            AddZoneLineBox("misty", 407.486847f, -2571.641357f, -10.749720f, ZoneLineOrientationType.West, -69.729698f, 134.790482f, 22.466999f, -96.162209f, 113.427109f, -0.500000f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 215.891556f, -251.910565f, -15.339250f, -512.838562f, -11.999970f, 200f); // South lake (lower)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 373.244324f, -227.197754f, 215.881556f, -336.578491f, -7.999990f, 200f); // North lake (higher)
        }
    }
}
