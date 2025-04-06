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
    internal class SurefallGladeZoneProperties : ZoneProperties
    {
        public SurefallGladeZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsNoSky(114, 111, 116, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            SetZonewideAmbienceSound("", "night");
            Enable2DSoundInstances("wind_lp2");

            AddZoneArea("Ranger's Guild", "qrg-03", "qrg-03", true, "", "silence");
            AddZoneAreaBox("Ranger's Guild", 194.394394f, -1.289240f, 13.855110f, 113.035820f, -70.200150f, -1.086660f);
            AddZoneAreaBox("Ranger's Guild", 152.741501f, -68.650978f, 13.855110f, 99.090302f, -124.775002f, -1.086660f);

            AddZoneArea("Jaggedpine Treefolk", "", "", false, "", "silence");
            AddZoneAreaBox("Jaggedpine Treefolk", -98.646744f, -252.945007f, 12.863730f, -140.132202f, -306.914001f, -3.714090f);
            AddZoneAreaBox("Jaggedpine Treefolk", -98.646744f, -280.769897f, 12.863730f, -167.112549f, -306.914001f, -3.714090f);

            AddZoneArea("Caves", "", "", false, "", "silence");
            AddZoneAreaBox("Caves", 910.847656f, -108.553413f, 242.904266f, 360.003510f, -739.231506f, -58.258690f);
            AddZoneAreaBox("Caves", 405.905792f, -566.127319f, 183.168060f, 0f, -555.211792f, -77.976501f);
            AddZoneAreaBox("Caves", 247.178406f, -299.769897f, 99.795021f, 0f, -458.985809f, -58.291248f);
            AddZoneAreaBox("Caves", 409.300995f, -178.286896f, 99.795021f, 200.221924f, -340.910248f, -58.291248f);
            AddZoneAreaBox("Caves", 409.300995f, -151.517303f, 99.795021f, 205.280899f, -340.910248f, -58.291248f);
            AddZoneAreaBox("Caves", 258.162933f, -144.535812f, 16.109440f, 196.461136f, -364.883026f, -4.708850f);
            AddZoneAreaBox("Caves", 388.761932f, -278.815216f, 242.904266f, 3.186390f, -508.471924f, -77.976501f);
            AddZoneAreaBox("Caves", 283.257782f, -314.457947f, 215.515167f, 52.706509f, -616.138977f, -93.161430f);
            AddZoneAreaBox("Caves", 729.727783f, -457.031494f, 140.861191f, 26.656240f, -577.515198f, -34.857800f);

            AddZoneArea("Shrine of Tunare", "qrg-02", "qrg-02", true, "", "silence");
            AddZoneAreaBox("Shrine of Tunare", -171.097961f, -367.069092f, 1.313080f, -248.541733f, -444.171204f, -0.831980f);
            AddZoneAreaBox("Shrine of Tunare", -181.442581f, -409.380524f, 4f, -238.635086f, -446.004974f, -0.831980f);
            AddZoneAreaBox("Shrine of Tunare", -200.574402f, -443.506012f, 10.636270f, -219.384384f, -446.005951f, 9.609200f);

            AddZoneLineBox("qeytoqrg", 5180.557617f, 161.911987f, -6.594880f, ZoneLineOrientationType.West, -623.557495f, 168.640945f, 0.500030f, -639.942505f, 150.659027f, -0.499970f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 264.136719f, 37.288700f, 48.358829f, -182.936539f, -3.999990f, 100f); // Pool around house on stilts
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 688.963257f, -248.454926f, 141.856928f, -567.358032f, -1.000000f, 100f); // Cave water, high
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 141.866928f, -420.606171f, 89.630241f, -567.358032f, -3.999990f, 100f); // Cave water, low
        }
    }
}
