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
    internal class KurnsTowerZoneProperties : ZoneProperties
    {
        public KurnsTowerZoneProperties() : base()
        {
            SetZonewideEnvironmentAsIndoors(50, 50, 50, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", -106.792099f, 288.794922f, -127.122070f, 268.549042f, -125.937500f, 250f); // Tiny water pool in the SW corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 123.125381f, 187.597153f, 99.923401f, 158.069412f, -120.937492f, 250f); // Small pool in the NW corner
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", -66.340874f, -101.648552f, -78.201767f, -112.540199f, -108.968742f, 250f); // Reall small pool in the middle-south
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 30.777281f, -75.552406f, 13.264740f, -120.494278f, -110.968727f, 250f); // Wide sliver near the middle
            AddLiquidVolume(ZoneLiquidType.Water, 150.724945f, -92.115112f, 24.399960f, -239.779404f, -113.204567f, -250f); // Eastmost water block that doesn't emerge on the east
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 158.640427f, -17.204069f, 53.631828f, -107.362862f, -121.937500f, 250f); // Eastmost entry (with the pillar)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 284.808868f, 51.155781f, 147.895889f, -8.759710f, -124.937492f, 250f); // North area, northmost surface
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 148.004623f, 26.082781f, 97.122833f, -6.474550f, -121.937500f, 250f); // North area, first cove with no exit but has surface
            AddLiquidVolume(ZoneLiquidType.Water, 151.284195f, 82.212471f, 90.786087f, -8.574540f, -122.718079f, -300f); // North area connection
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 114.833603f, 153.856232f, 25.347370f, -8.104440f, -121.937477f, 250f); // North area, large bulk area on the more northern side with surface
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 25.670321f, 207.826813f, -138.527527f, -10.281050f, -124.937492f, 250f); // South area, not the pillar
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", 0.835420f, 216.988266f, -69.153831f, 205.093384f, -124.937492f, 250f); // South area, not the pillar (small chunk)
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", -98.348793f, 132.563599f, -186.880676f, 59.314430f, -126.937500f, 250f); // South pillar room
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_kw1", -137.365356f, 152.194702f, -179.620819f, 131.826355f, -126.937500f, 250f); // South pillar room (chunk)
        }
    }
}
