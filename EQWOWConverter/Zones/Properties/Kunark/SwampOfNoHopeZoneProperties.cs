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
    internal class SwampOfNoHopeZoneProperties : ZoneProperties
    {
        public SwampOfNoHopeZoneProperties() : base()
        {
            AddZoneLineBox("cabeast", -98.067253f, -657.688232f, 0.000070f, ZoneLineOrientationType.North, 3172.572266f, 3068.755859f, 43.239300f, 3137.161865f, 3040.213135f, -0.374930f);
            AddZoneLineBox("cabeast", -280.219482f, -476.267853f, 0.000010f, ZoneLineOrientationType.West, 2955.181396f, 3256.399658f, -0.375170f, 2955.181396f, 3256.399658f, -0.375170f);

            // Main water
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_spwt01", 4014.024414f, 3982.306152f, -3986.161865f, -4018.020264f, -20.530670f, 500f);

            // East river and up into the cave
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1376.880859f, -2864.697021f, 1262.926880f, -3009.132080f, 14.152580f, -20.462299f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1353.485718f, -3009.097168f, 1230.548706f, -3321.931152f, 29.119940f, 14.149130f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1326.389282f, -3321.896729f, 1200.520630f, -3346.410645f, 139.146988f, 29.116489f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1310.394165f, -3346.375977f, 1105.660889f, -3515.032227f, 173.782104f, 139.143539f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1278.422119f, -3514.997803f, 1171.982666f, -3542.320557f, 175.312698f, 173.778656f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1322.860107f, -3542.286133f, 1043.090332f, -3973.453857f, 175.312775f, 175.309265f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1413.176270f, -3973.419189f, 1000.038635f, -4213.063965f, 175.283356f, 175.309357f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            // Small more east waterfall up into the cave
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1207.356934f, -3320.213867f, 1077.192871f, -3349.652100f, 138.418182f, 27.860479f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1212.583740f, -3349.617676f, 1120.218018f, -3398.383789f, 148.894485f, 138.414749f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 974.239380f, -2921.238770f, 821.814697f, -3005.347412f, 21.763670f, -20.534479f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1170.819702f, -3005.312988f, 831.463196f, -3208.402832f, 27.827190f, 21.760220f, ZoneLiquidSlantType.EastHighWestLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1166.929688f, -3208.368408f, 959.118347f, -3320.761963f, 27.867889f, 27.823740f, ZoneLiquidSlantType.EastHighWestLow, 250f);

            // East cave
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 2183.705078f, -4453.435547f, 1923.547729f, -4644.909668f, 20.937731f, 20.020670f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1923.582031f, -4455.738770f, 1898.686523f, -4593.009277f, 27.265261f, 20.934280f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1898.721069f, -4463.323242f, 1873.576538f, -4578.629395f, 38.901970f, 27.261820f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1873.611084f, -4455.667969f, 1872.611694f, -4587.056641f, 39.345970f, 38.898521f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1872.646240f, -4457.985840f, 1846.815674f, -4587.603027f, 59.747540f, 39.342522f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1846.850220f, -4461.690430f, 1799.384033f, -4590.331055f, 79.584999f, 59.744099f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1799.418579f, -4469.217773f, 1771.926514f, -4608.363281f, 87.184334f, 79.581551f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1771.961060f, -4475.539062f, 1722.945801f, -4611.530273f, 87.187950f, 87.180893f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1722.980347f, -4475.105469f, 1698.142822f, -4604.536621f, 97.931183f, 87.184509f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1698.177490f, -4476.751953f, 1666.170410f, -4596.095703f, 120.351700f, 97.927727f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1666.204956f, -4450.296387f, 1623.142212f, -4590.457031f, 140.477539f, 120.348259f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1623.176636f, -4444.395508f, 1569.536011f, -4584.356445f, 167.961014f, 140.474106f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1569.570557f, -4469.007812f, 1547.765625f, -4589.850098f, 175.146698f, 167.957581f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_spwt01", 1547.800171f, -3939.102295f, 960.924744f, -4745.595215f, 175.308105f, 175.143234f, ZoneLiquidSlantType.SouthHighNorthLow, 250f);
        }
    }
}
