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
    internal class EmeraldJungleZoneProperties : ZoneProperties
    {
        public EmeraldJungleZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsNoSky(22, 26, 23, ZoneFogType.Heavy, 1f);

            AddZoneLineBox("citymist", 300.490265f, -1799.661743f, -334.968658f, ZoneLineOrientationType.East,
                10.193290f, -783.147522f, 34.308090f, -10.191010f, -844.774231f, -0.500000f);

            // Fix Z
            AddZoneLineBox("fieldofbone", -1972.522217f, -1195.001099f, -1.372020f, ZoneLineOrientationType.West,
                5763.723633f, -784.296265f, 158.024155f, 5487.835449f, -2307.601807f, -61.162090f);
            AddZoneLineBox("trakanon", 3958.170654f, 1500.808472f, -343.708710f, ZoneLineOrientationType.South,
                1653.905151f, -3444.631104f, -278.644684f, 1392.652588f, -3532.577637f, -369.231049f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1b", 4096.996094f, 99.285179f, 3197.614746f, -1874.321289f,
                -364.780731f, 200f);
        }
    }
}
