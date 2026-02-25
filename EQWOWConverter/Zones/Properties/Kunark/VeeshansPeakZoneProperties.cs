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
    internal class VeeshansPeakZoneProperties : ZoneProperties
    {
        public VeeshansPeakZoneProperties() : base()
        {
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", -457.174774f, -1272.401733f, -744.802429f, -1463.637573f, 299.875092f, 500f); // Southeast lava pit
            // AddLiquidVolume(ZoneLiquidType.Magma, -457.174774f, -1224.742310f, -909.169739f, -1516.084106f, 272.158234f, 67.459221f); // Southeast lava pit
            // AddOctagonLiquidShape(ZoneLiquidType.Magma, "d_m0000", 396.015472f, 242.934616f, -302.685181f, -684.056030f, -341.353851f, -679.909058f, -341.353851f, -679.909058f, 358.336853f, 280.745483f, 358.336853f, 280.745483f, 448.812469f, 50f, 0.5f); // Southeast-ish lava pool with water pools in it
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_vewater1", 339.209961f, -347.647888f, 300.815369f, -641.593140f, 452.812561f, 15f); // Southeast-ish lava pool with water pools in it
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_vewater1", 350.778534f, -352.688354f, 289.014435f, -447.225006f, 452.812531f, 4f); // Southeast-ish lava pool with water pools in it
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_vewater1", 350.778534f, -471.269012f, 289.014435f, -568.177612f, 452.812531f, 4f); // Southeast-ish lava pool with water pools in it
            // AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_vewater1", 350.778534f, -590.91564f, 289.014435f, -642.061646f, 452.812531f, 4f); // Southeast-ish lava pool with water pools in it
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", -197.819702f, -44.918411f, -249.935287f, -74.986931f, 437.812469f, 5f); // Two lava mini pools with flows into them, south of the lava pools with water pools - West
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", -197.819702f, -164.901047f, -249.935287f, -194.951813f, 437.812469f, 5f); // Two lava mini pools with flows into them, south of the lava pools with water pools - East
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", -187.329803f, 209.543884f, -363.399750f, 37.534168f, 349.875031f, 10f); // Lava room with a small waterfall in the mid-south
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 955.947144f, 1220.377563f, 292.111847f, 774.566223f, 179.937515f, 500f); // Southwest 3-room connecting lava - Large middle pool
            // AddLiquidVolume(ZoneLiquidType.Magma, 721.810852f, 1399.441162f, -150.169128f, 1056.212646f, 173.666946f, -144.953720f); // Southwest 3-room connecting lava - No-surface connecting from middle to bottom pool
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 363.018677f, 2059.642090f, -208.744522f, 1398.623413f, 0f, 500f); // Southwest 3-room connecting lava - Large bottom pool
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 443.063934f, 819.868103f, 280.527893f, 681.307434f, 199.906296f, 20f); // First step-up towards source
            // AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 440.945648f, 819.995056f, 359.045044f, 839.972717f, 818.512024f, 199.906296f, 20f, 0.7f); // Southwest 3-room connecting lava
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 386.627228f, 828.647034f, 280.527893f, 681.307434f, 199.906296f, 20f); // Southwest 3-room connecting lava
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 365.431274f, 835.699097f, 280.527893f, 681.307434f, 199.906296f, 20f); // Southwest 3-room connecting lava
            // AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 361.122711f, 679.561096f, 321.149872f, 759.727844f, 630.866699f, 239.906265f, 500f, 0.6f); // Southwest 3-room connecting lava - Second step-up (middle)
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 362.088013f, 679.484131f, 270.024261f, 615.901001f, 239.906265f, 500f); // Southwest 3-room connecting lava
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 323.166412f, 732.707458f, 261.935760f, 615.901001f, 239.906265f, 500f); // Southwest 3-room connecting lava
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 365.206573f, 639.741821f, 214.783905f, 365.740387f, 279.875000f, 500f); // Southwest 3-room connecting lava - Top level

            // AddTriangleLiquidShapeNorthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 961.475098f, 360.085693f, 1002.212952f, 361.990173f, 322.101013f, 533.781311f, 4f, 0.5f); // Center room with the 4 lava corner triangles and middle pool - NW room corner triangle
            // AddTriangleLiquidShapeNorthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 962.633667f, -40.026970f, 1000.896729f, -2.904270f, -40.880989f, 453.812561f, 4f, 0.5f); // Center room with the 4 lava corner triangles and middle pool - NE room corner triangle
            // AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 718.271423f, 360.759491f, 677.007019f, 362.968170f, 321.102753f, 533.781311f, 4f, 0.5f); // Center room with the 4 lava corner triangles and middle pool - SW room corner triangle
            // AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType.Magma, "d_m0000", 717.355042f, -40.834740f, 679.094238f, -2.817990f, -41.305679f, 453.812469f, 4f, 0.5f); // Center room with the 4 lava corner triangles and middle pool - SE room corner triangle
            // AddOctagonLiquidShape(ZoneLiquidType.Magma, "d_m0000", 915.367310f, 764.489868f, 274.726868f, 158.964523f, 237.470825f, 201.557037f, 237.470825f, 201.557037f, 880.379822f, 801.574463f, 880.379822f, 801.574463f, 525.781372f, 200f, 0.5f); // Center room with the 4 lava corner triangles and middle pool
            // AddOctagonLiquidShape(ZoneLiquidType.Magma, "d_m0000", 915.367310f, 764.489868f, 274.726868f, 85.002327f, 237.470825f, 121.396919f, 237.470825f, 121.396919f, 880.379822f, 801.574463f, 880.379822f, 801.574463f, 448.812683f, 20f, 0.5f); // Center room with the 4 lava corner triangles and middle pool

            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1826.058716f, 7.350710f, 1553.357910f, -468.822815f, 669.718872f, 11f); // Northeast lava - North segment
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1553.457910f, 2.415910f, 1393.934448f, -265.983124f, 669.718872f, 11f); // Northeast lava - Center segment
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1393.944448f, -44.106380f, 1354.711304f, -258.300262f, 669.718872f, 11f); // Northeast lava- South segment

            
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1923.876953f, 1300.815186f, 1593.642456f, 843.009277f, 639.750244f, 14f); // Northwest lava - West part
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1923.876953f, 1300.815186f, 1638.326538f, 811.289307f, 639.750244f, 14f); // Northwest lava - West part of east sliver
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1923.876953f, 811.389307f, 1641.375366f, 799.436829f, 639.750244f, 14f); // Northwest lava - West part of east sliver

            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1723.944824f, 841.598022f, 1283.606812f, 286.755402f, 254.906387f, 50f); // North bottom lava - North part
            // AddLiquidPlaneZLevel(ZoneLiquidType.Magma, "d_m0000", 1723.944824f, 804.284851f, 1208.281250f, 286.755402f, 254.906387f, 50f); // North bottom lava - North part

            AddDiscardGeometryBox(99.455254f, 57.394550f, 150.158112f, -50.228031f, -140.100006f, -23.634960f, "0 0 0 spawn cat box"); // 0 0 0 spawn cat box
        }
    }
}
