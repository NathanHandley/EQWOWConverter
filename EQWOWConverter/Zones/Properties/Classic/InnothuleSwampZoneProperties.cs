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
    internal class InnothuleSwampZoneProperties : ZoneProperties
    {
        public InnothuleSwampZoneProperties() : base()
        {            
            // AddZoneArea("Guk");
            // AddZoneAreaBox("Guk", 305.515717f, -560.334473f, 68.895988f, 51.747478f, -913.086060f, -65.152344f);

            // AddZoneArea("North Hand", "innothule-00", "innothule-00", false);
            // AddZoneAreaBox("North Hand", 1428.922607f, 653.495667f, 93.938110f, 1056.201538f, 361.576935f, -81.741661f);

            // AddZoneArea("South Hand", "innothule-03", "innothule-03");
            // AddZoneAreaBox("South Hand", -277.917664f, 1091.021240f, 100.433762f, -642.524719f, 847.547546f, -88.879128f);

            // AddZoneArea("Dead Tower", "innothule-02", "innothule-02");
            // AddZoneAreaBox("Dead Tower", 2077.336914f, -37.029652f, 23.242470f, 1716.803101f, -277.183929f, -79.665352f);
            AddZoneLineBox("feerrott", -1020.344177f, -3092.292236f, -12.343540f, ZoneLineOrientationType.North, -1099.632080f, 2000.790283f, 9.191510f, -1250f, 1882.234253f, -12.843200f);            
            AddZoneLineBox("grobb", -179.500046f, 39.101452f, -0.000000f, ZoneLineOrientationType.West, -2781.871094f, -625.318726f, -16.126810f, -2804.662109f, -646.227112f, -35.062538f);
            AddZoneLineBox("guktop", -62.457378f, 42.394871f, 0.000010f, ZoneLineOrientationType.East, 150.598709f, -828.381348f, 0.967340f, 136.212891f, -843.098694f, -11.999980f);
            AddZoneLineBox("sro", -3168.635742f, 1032.933105f, -26.814310f, ZoneLineOrientationType.North, 2800f, 1250f, 19.084551f, 2554.791748f, 1120f, -35f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t75_sw1", 2322.104736f, 1750.974365f, -2615.490234f, -670.816833f, -31.624689f, 50f);
        }
    }
}
