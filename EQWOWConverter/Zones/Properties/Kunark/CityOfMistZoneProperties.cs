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

namespace EQWOWConverter.Zones.Properties
{
    internal class CityOfMistZoneProperties : ZoneProperties
    {
        public CityOfMistZoneProperties() : base()
        {
            // TODO: Any in-zone teleports?
            AddZoneLineBox("emeraldjungle", 300.948212f, -1798.700195f, -334.972931f, ZoneLineOrientationType.East,
                21.656549f, -808.069702f, 59.639500f,  -15.719450f, -854.674255f, -7.938090f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 181.788925f, 434.332367f, -168.714798f, 123.955620f, -17.999960f, 75); // Water around westmost building
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 62.186371f, -87.708946f, -113.876907f, -366.743774f, -35.999989f, 100f); // Water in the middle building (ring of water) and some eastbound tunnel
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 123.399017f, -271.990479f, -121.743713f, -742.099121f, -17.999990f, 100f); // Entry water and first section in, and some of the tunnels between
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 280.146210f, -503.799011f, 115.423653f, -655.627441f, -5.999990f, 100f); // Northeast room with incline water, bottom water part and tunnels
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 227.588181f, -399.097443f, 169.080063f, -419.527283f, 36.000019f, 11f); // Northeast room with incline water, top big rect
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_agua1", 218.242523f, -418.708801f, 177.775513f, -431.825958f, 36.000019f, 11f); // Northeast room with incline water, top small rect
            AddLiquidPlane(ZoneLiquidType.Water, "t75_agua1", 217.685974f, -431.825958f, 178.617783f, -503.799011f, 36.000019f, 0.014160f, ZoneLiquidSlantType.WestHighEastLow, 200f); // Northeast room with incline water, slanted water fall
        }
    }
}
