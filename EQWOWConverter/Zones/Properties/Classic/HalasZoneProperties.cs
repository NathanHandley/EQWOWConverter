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
    internal class HalasZoneProperties : ZoneProperties
    {
        public HalasZoneProperties() : base()
        {
            // TODO: Boat that goes back and forth
            // TODO: PVP in Pit of Doom
            // TODO: Perhaps add Mac's Kilts as a zone area
            // TODO: Add breath
            SetBaseZoneProperties("halas", "Halas", 0f, 0f, 3.75f, 0, ZoneContinentType.Antonica);
            
            AddZoneArea("Entry Tunnel", "halas-01", "halas-01", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, false);
            AddZoneAreaBox("Entry Tunnel", -461.854279f, 79.028740f, 114.175423f, -817.487427f, -169.417740f, -163.384399f);

            AddZoneArea("The Bound Mermaid");
            AddZoneAreaBox("The Bound Mermaid", 33.998589f, 86.185371f, 75.345596f, -40.758659f, 28.691170f, -7.556500f);

            AddZoneArea("Halas Hold");
            AddZoneAreaBox("Halas Hold", 217.330765f, 193.336746f, 58.821468f, 169.907166f, 103.222847f, -2.061110f);

            AddZoneArea("DoK's Cigars");
            AddZoneAreaBox("DoK's Cigars", 283.782135f, 290.661774f, 64.446640f, 202.926224f, 256.873383f, -3.973220f);
            AddZoneAreaBox("DoK's Cigars", 283.782135f, 290.661774f, 64.446640f, 207.013519f, 252.728821f, -3.973220f);
            AddZoneAreaBox("DoK's Cigars", 283.782135f, 290.661774f, 64.446640f, 209.839096f, 249.822372f, -3.973220f);
            AddZoneAreaBox("DoK's Cigars", 283.782135f, 290.661774f, 64.446640f, 211.932037f, 245.548798f, -3.973220f);
            AddZoneAreaBox("DoK's Cigars", 283.782135f, 290.661774f, 64.446640f, 214.345184f, 224.535614f, -3.973220f);

            AddZoneArea("The Golden Torc", "halas-08", "halas-08");
            AddZoneAreaBox("The Golden Torc", 238.510223f, 465.643677f, 80.789131f, 95.977081f, 328.518524f, -5.839420f);

            AddZoneArea("Shaman's Den");
            AddZoneAreaBox("Shaman's Den", 370.562469f, 500f, 100.831596f, 263.427856f, 332.662781f, -45.514370f);

            AddZoneArea("Pit of Doom", "halas-03", "halas-03");
            AddZoneAreaBox("Pit of Doom", 662.088928f, -279.314606f, 140.809998f, 397.346771f, -567.676392f, -45.514370f);

            AddZoneArea("McDaniel's Smokes and Spirits", "halas-07", "halas-07");
            AddZoneAreaBox("McDaniel's Smokes and Spirits", 365.038849f, -239.789658f, 92.159866f, 307.601257f, -352.602509f, -3.201780f);
            AddZoneAreaBox("McDaniel's Smokes and Spirits", 375.009552f, -267.997650f, 92.159866f, 307.601257f, -352.602509f, -3.201780f);
            AddZoneAreaBox("McDaniel's Smokes and Spirits", 378.899475f, -275.038940f, 92.159866f, 307.601257f, -352.602509f, -3.201780f);
            AddZoneAreaBox("McDaniel's Smokes and Spirits", 384.345398f, -236.876526f, 92.159866f, 307.601257f, -352.602509f, 11.183750f);

            AddZoneArea("McQuaid's Dark Stout", "halas-04", "halas-04");
            AddZoneAreaOctagonBox("McDonald's Fire Cider", 129.972458f, 82.012718f, -183.867081f, -273.888367f, -189.872543f, -268.952698f, -194.811661f, -266.338409f,
                110.081520f, 100.430153f, 109.239693f, 97.214653f, 37.773350f, -2.628820f);
            AddZoneAreaBox("McDonald's Fire Cider", 247.267914f, -134.631378f, 38.144821f, 175.779739f, -180.837265f, -2.174550f);

            AddZoneArea("Yee Majik", "halas-09", "halas-09");
            AddZoneAreaBox("Yee Majik", 145.826874f, -307.438782f, 26.357410f, 102.788857f, -359.476654f, -4.376070f);
            AddZoneAreaBox("Yee Majik", 160.033524f, -339.226288f, 26.357410f, 86.067749f, -399.233185f, -4.376070f);

            AddZoneArea("McPherson's Bloody Blades");
            AddZoneAreaOctagonBox("McPherson's Bloody Blades", 232.164093f, 165.558990f, -265.482086f, -340.089294f, -286.648346f, -302.833740f,
                -286.727203f, -301.972076f, 219.922913f, 179.911942f, 219.197540f, 180.743759f, 30.383160f, -5.666800f);

            SetZonewideEnvironmentAsOutdoorsNoSky(144, 165, 183, ZoneFogType.Heavy, 1f);
            DisableSunlight();
            AddZoneLineBox("everfrost", 3682.792725f, 372.904633f, 0.000240f, ZoneLineOrientationType.South, -664.463196f, -50.776440f, 37.469002f, -744.483093f, -101.162247f, -0.499990f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", -16.822701f, 195.248566f, -464.163391f, -189.505676f, -2.999970f, 250f); // Pool at zone line
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 199.293900f, -199.563965f, 177.719742f, -224.856445f, -0.999970f, 250f); // Well
        }
    }
}
