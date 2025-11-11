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
    internal class NeriakCommonsZoneProperties : ZoneProperties
    {
        public NeriakCommonsZoneProperties() : base()
        {
            // TODO: Zone Areas
            //TODO: Base of the waterfall has collision when it shouldn't, and shares material with walls so can't change that way
            SetZonewideEnvironmentAsIndoors(35, 22, 59, ZoneFogType.Heavy);
            AddZoneLineBox("neriaka", 83.959953f, -322.479065f, -14.000000f, ZoneLineOrientationType.West, 98.161079f, -305.681519f, 12.467630f, 69.775436f, -384.671295f, -14.500000f);
            AddZoneLineBox("neriaka", -252.560760f, -455.675934f, 14.000010f, ZoneLineOrientationType.South, -252.075302f, -447.619110f, 26.454840f, -267.196991f, -490.619293f, 13.499990f);
            AddZoneLineBox("neriakc", 209.334473f, -853.563110f, -41.968079f, ZoneLineOrientationType.North, 210.713379f, -844.618347f, -31.532860f, 203.079483f, -860.849731f, -42.468700f);
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_m0003", -69.974701f, -461.815002f, -77.969276f, -452.868011f,
                -89.278128f, -466.222113f, -84.319678f, -476.131415f, 7f, 250f); // West waterfall 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", 176.435440f, -449.724335f, -75.183372f, -779.972900f, -42.968342f, 100f); // NorthWestmost lake water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -75.083372f, -449.724335f, -87.210258f, -506.557495f, -42.968342f, 100f); // SouthWestmost lake water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -124.188118f, -812.949768f, -315.905853f, -1121.356567f, -42.968342f, 29f); // Eastmost lake water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -0.381660f, -779.758301f, -84.545477f, -840.462952f, -42.968342f, 31f); // South of bar in lake
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -84.445477f, -779.758301f, -126.279922f, -840.462952f, -42.968342f, 30f); // South of bar in lake
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_m0003", 33.392849f, -780.018066f, 25.913090f, -754.987305f,
                -36.588150f, -796.803223f, -0.109150f, -814.181809f, -42.968342f, 31f, 1000f, 1000f, -1000f, -1000f, 0.4f); // Outside bar windows
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -75.048660f, -657.157166f, -84.026176f, -704.937622f, -42.968342f, 38.551629f); // SW in big lake area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -84.017327f, -668.889526f, -147.045441f, -687.034424f, -42.968342f, 15.572826f); // SW in big lake area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -84.017522f, -686f, -124.934113f, -706.436462f, -42.968342f, 15.9f); // SW in big lake area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -74.800011f, -747.240784f, -236.421478f, -814.009216f, -42.968342f, 28.49715f); // South part of big lake
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -70.274368f, -775.813171f, -84.276917f, -815.613098f, -42.968342f, 35f); // South part of big lake deep part
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -94.696983f, -706.349182f, -126.859512f, -747.437988f, -42.968342f, 19.949695f); // Small section in SW big lake, around protrusion over underground
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -118.663292f, -706.492859f, -125.724388f, -711.350220f, -42.968342f, 25.416886f); // Tiny deep spot in south pool area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -125.828957f, -686.784729f, -170.818741f, -748.498291f, -42.968342f, 35f); // Small section in SW big lake
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_m0003", 130.234726f, -797.568542f, 119.068558f, -786.073120f,
                107.652733f, -797.633850f, 119.055656f, -809.229187f, -39.968719f, 10f); // North fountain base
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", 122.595047f, -794.080505f, 115.300949f, -801.353149f, -34.999920f, 10f); // North fountain mid
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", 120.801430f, -795.769287f, 117.165176f, -799.421387f, -28.499960f, 30f); // North fountain top
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_m0003", -133.632080f, -951.659546f, -139.871582f, -945.725891f, -140.807343f, -951.617493f,
                -140.026337f, -957.608337f, -39.968609f, 10f, -135.480804f, -945.725891f, -140.807343f, -957.608337f, 0.3f); // East Fountain - Base segment north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -139.455841f, -946.005310f, -145.963593f, -950.630981f, -39.968609f, 10f); // East Fountain - Base segment center NW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -147.934845f, -946.005310f, -154.267471f, -950.621704f, -39.968609f, 10f); // East Fountain - Base segment center SW
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -147.917404f, -952.609619f, -154.267471f, -957.223267f, -39.968609f, 10f); // East Fountain - Base segment center SE 
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -139.455841f, -952.609619f, -145.963593f, -957.223267f, -39.968609f, 10f); // East Fountain - Base segment center NE
            AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType.Water, "t50_m0003", -151.865845f, -951.571350f, -153.772766f, -945.736450f, -160.001923f, -951.656799f,
                -153.700836f, -957.501465f, -39.968609f, 10f, -151.865845f, -945.736450f, -158.453781f, -957.501465f, 0.3f); // East Fountain - Base segment south
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -142.004700f, -950.647949f, -151.977570f, -952.587585f, -23.599950f, 30f); // East Fountain - Top Part NS
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_m0003", -145.975998f, -946.709351f, -147.923950f, -956.632507f, -23.599950f, 30f); // East Fountain - Top Part EW
            AddDiscardGeometryBox(53.591419f, 57.356628f, 52.387539f, -224.128967f, -29.459351f, -33.583618f);
        }
    }
}
