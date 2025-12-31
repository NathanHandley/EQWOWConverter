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
    internal class MinesOfNurgaZoneProperties : ZoneProperties
    {
        public MinesOfNurgaZoneProperties() : base()
        {
            SetZonewideEnvironmentAsIndoors(1, 15, 1, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4);
            Enable2DSoundInstances("wind_lp3", "caveloop");

            AddZoneLineBox("droga", -1389.241333f, 263.044464f, -191.937393f, ZoneLineOrientationType.North,
                -942.595520f, 225.447723f, -80.635323f, -1016.506470f, 91.078529f, -126.038986f); // West
            AddZoneLineBox("droga", -1311.640869f, -919.159363f, -79.968773f, ZoneLineOrientationType.North,
                -748.473267f, -809.300537f, -145.098434f, -823.214478f, -941.389526f, -183.636047f); // East
            AddZoneLineBox("frontiermtns", -2698.660400f, -539.507812f, -499.937439f, ZoneLineOrientationType.South,
                -2243.407227f, -1780.908203f, 19.086390f, -2328.118652f, -1935.406616f, -9.129420f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -1607.364746f, 1672.431396f, -1989.492432f, 1287.276245f, -319.874939f, 200f); // Westmost water room and connecting side
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -1632.191650f, 1407.210693f, -1718.308350f, 1302.216797f, -277.874908f, 15f); // West area, upper room pond
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -1693.050903f, 1402.835083f, -1712.303101f, 1379.131714f, -277.874908f, 15f); // West area, upper room pond out to waterfall
            // TODO: There's a triangle section just above this that should probably be done
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -2057.785156f, -1919.377441f, -2104.408936f, -1986.052856f, 20.000210f, 500f); // East waterfalls, source, one step up
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -2041.089233f, -1807.408203f, -2153.033203f, -1935.529297f, 16.000059f, 500f); // East waterfalls, source, big pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -2068.788818f, -1679.507568f, -2177.996338f, -1811.173950f, -15.999700f, 500f); // East waterfalls, second pool down
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -1939.819946f, -1610.540527f, -2047.140381f, -1659.428345f, -47.968719f, 500f); // Crossover water source
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_drowatr1", -2014.838501f, -1566.985718f, -2174.728760f, -1709.796753f, -191.937347f, 30f); // Big pool at the bottom of the east waterfalls

            AddDiscardGeometryBox(32.933071f, 44.520969f, 115.778313f, -20.912889f, -48.130749f, -8.768160f); // 0 0 0 spawn box
        }
    }
}
