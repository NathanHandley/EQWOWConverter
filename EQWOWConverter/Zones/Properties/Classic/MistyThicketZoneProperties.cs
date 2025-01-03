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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class MistyThicketZoneProperties : ZoneProperties
    {
        public MistyThicketZoneProperties() : base()
        {
            // TODO: Add child sub areas
            // TODO: Smooth out the southeast line of "West Misty Thicket" area
            SetBaseZoneProperties("misty", "Misty Thicket", 0f, 0f, 2.43f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsOutdoorsNoSky(67, 81, 34, ZoneFogType.Heavy, 1.0f);
            DisableSunlight();
            SetZonewideAmbienceSound("", "night");
            AddZoneArea("West Misty Thicket", "", "", false, "", "darkwds1");
            AddZoneAreaBox("West Misty Thicket", 1299.867676f, 1559.016846f, 466.514893f, -1342.874023f, -28.964359f, -165.042603f); // West from top of wall
            AddZoneAreaBox("West Misty Thicket", -456.799561f, 1559.016846f, 466.514893f, -1342.874023f, -307.651550f, -165.042603f); // West from top of first N-S
            AddZoneAreaBox("West Misty Thicket", -659.713318f, 1559.016846f, 466.514893f, -1342.874023f, -411.677094f, -165.042603f); // West from top of second N-S
            AddZoneAreaBox("West Misty Thicket", -863.693909f, 1559.016846f, 466.514893f, -1342.874023f, -458.057556f, -165.042603f); // West from top of last N-S
            AddZoneAreaBox("West Misty Thicket", -227.123093f, 1559.016846f, 466.514893f, -1342.874023f, -92.112007f, -165.042603f);
            AddZoneAreaBox("West Misty Thicket", -253.924362f, 1559.016846f, 466.514893f, -1342.874023f, -114.615143f, -165.042603f);
            AddZoneAreaBox("West Misty Thicket", -391.058228f, 1559.016846f, 466.514893f, -1342.874023f, -213.003998f, -165.042603f);
            AddZoneAreaBox("West Misty Thicket", -435.239960f, 1559.016846f, 466.514893f, -1342.874023f, -277.271667f, -165.042603f);
            AddZoneAreaBox("West Misty Thicket", -614.750305f, 1559.016846f, 466.514893f, -1342.874023f, -365.433685f, -165.042603f);
            AddZoneLineBox("runnyeye", 231.871094f, 150.141602f, 1.001060f, ZoneLineOrientationType.South, -826.740295f, 1443.224487f, 3.532040f, -840.195496f, 1415.736206f, -11.249970f);
            AddZoneLineBox("rivervale", -83.344131f, 97.216301f, -0.000000f, ZoneLineOrientationType.East, 418.880157f, -2588.360596f, 11.531500f, 394.495270f, -2613.019531f, -11.249990f);
        }
    }
}
