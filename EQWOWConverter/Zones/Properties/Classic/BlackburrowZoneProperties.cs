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
    internal class BlackburrowZoneProperties : ZoneProperties
    {
        public BlackburrowZoneProperties()
        {
            // TODO: Bug: See-through ceiling in water at 78.642151f, -130.569107f, -166.715637f
            // TODO: Ladders
            SetBaseZoneProperties("blackburrow", "Blackburrow", 38.92f, -158.97f, 3.75f, 0, ZoneContinent.Antonica);
            SetFogProperties(50, 100, 90, 10, 700);
            AddZoneLineBox("everfrost", -3027.1943f, -532.2794f, -113.18725f, ZoneLineOrientationType.North, 106.64458f, -329.8163f, 13.469f, 80.88026f, -358.2026f, -0.49926078f);
            AddZoneLineBox("qeytoqrg", 3432.621094f, -1142.645020f, 0.000010f, ZoneLineOrientationType.East, -154.74507f, 20.123898f, 10.469f, -174.6326f, 10.831751f, -0.49996006f);
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 43.036591f, 68.592300f, -16.785101f, -17.381670f, -1.999990f, 3.5f); // Top Water
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 28.463570f, -17.391670f, 4.950750f, -22.964569f, -1.999990f, 3.5f); // Top Water - Waterfall part
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 106.139679f, 496.872833f, -237.143341f, -10f, -158.947485f, 150f); // Bottom Water, southwest corner
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 267.539062f, -9.99f, 94f, -90.973503f, -158.947485f, 150f); // Bottom Water, north (south part)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 276.311340f, -72.864960f, 267.529062f, -79.548123f, -158.947485f, 150f); // Bottom Water, north (south of the waterfall)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 271.277222f, -64.912010f, 267.519062f, -68.902010f, -158.947485f, 150f); // Bottom Water, north (middle corner near waterfall 1)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 272.277222f, -68.892010f, 267.519062f, -72.874960f, -158.947485f, 150f); // Bottom Water, north (middle corner near waterfall 1)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 240.898453f, -90.963503f, 234.574244f, -113.812157f, -158.947485f, 150f); // Bottom Water, east (north more)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 234.584244f, -90.963503f, 170.944214f, -124.503326f, -158.947485f, 150f); // Bottom Water, east (north)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 170.954214f, -90.963503f, 93.964104f, -160.562027f, -158.947485f, 150f); // Bottom Water, east (south)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 282.843597f, -79.548123f, 267.529062f, -90.973503f, -158.947485f, 150f); // Bottom Water, waterfall
            AddQuadrilateralLiquidShapeZLevel(LiquidType.Water, "t50_agua1", 259.918518f, -90.954803f, 248.863174f, -77.377518f, 237.386520f, -90.410217f, 240.733398f, -108.128197f,
                -158.947485f, 150f); // Bottom Water, waterfall diagonal
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 324.693939f, -41.142281f, 238.570114f, -153.171570f, -169.944504f, 150f); // Very bottom of the waterfall
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 93.974104f, -120.654640f, -97.214737f, -158.702621f, -158.947485f, 150f); // Bottom water, east strip
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -6.093100f, -96.252602f, -127.042976f, -120.664640f, -158.947485f, 150f); // Bottom water, southeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -11.501780f, -85.925957f, -127.042976f, -96.262602f, -158.947485f, 150f); // Bottom water, southeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -49.848389f, -41.481209f, -133.480148f, -85.935957f, -158.947485f, 150f); // Bottom water south-southeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -60.165241f, -28.770680f, -133.480148f, -41.491209f, -158.947485f, 150f); // Bottom water south-southeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -60.165241f, -9.974030f, -133.480148f, -28.780680f, -158.947485f, 150f); // Bottom water, south
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -14.395770f, -9.975410f, -60.337849f, -23.376610f, -158.947485f, 150f); // Bottom water, south
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 103.356796f, -9.912370f, -14.544580f, -14.245810f, -158.947485f, 150f); // Bottom water, west around the pit
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 110f, -14.238740f, 27.332451f, -22.735680f, -158.947485f, 150f); // Bottom water, west around the pit
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 115f, -22.711069f, 66.795197f, -31.8f, -158.947485f, 150f); // Bottom water, west around the pit
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 96.461906f, -30.79f, 73.305093f, -36.929930f, -158.947485f, 150f); // Bottom water, southwest around the pit
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 94.130463f, -36.919930f, 82.638458f, -77.026337f, -158.947485f, 150f); // Bottom water, west coming up from south
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 94.192657f, -76.994118f, 91.834831f, -89.970016f, -158.947485f, 150f); // Bottom water, north around pit
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 93.893837f, -89.786736f, -0.248990f, -126.757332f, -147.937485f, 200f); // Water Pit Northeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 88.169029f, -55.670799f, -12.522690f, -89.796736f, -147.937485f, 200f); // Water pit, 1 step from northeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 75.674026f, -35.129551f, -49.419121f, -55.680799f, -147.937485f, 200f); // Water pit, 2 step from northeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 68.684937f, -28.673220f, -60.280651f, -35.139551f, -147.937485f, 200f); // Water pit, 3 step from northeast
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 50.179272f, -22.459141f, -12.933590f, -28.683220f, -147.937485f, 200f); // Water pit, 4 step from northeast (runs along northwest)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 6.849210f, -17.747561f, -12.315340f, -22.969141f, -147.937485f, 200f); // Water pit, 5 step from northeast (runs along southwest)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", 51f, -26.721331f, 50.179272f, -28.673220f, -147.937485f, 200f); // Water pit, northwest small corner fill-in
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -6.830730f, -88.576530f, -9.557570f, -94.195560f, -147.937485f, 200f); // Water pit, southwest small corner fill-in
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -0.238990f, -89.776736f, -7.250610f, -116.160797f, -147.937485f, 200f); // Water Pit southeast, more east
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -12.023590f, -55.575169f, -48.071800f, -86.330437f, -147.937485f, 200f); // Water Pit southeast, small fill-in
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -48.656361f, -34.435612f, -53.5f, -39.857670f, -147.937485f, 200f); // Water Pit south small fill-in
            AddLiquidVolume(LiquidType.Water, 95.977043f, -8.343080f, -69.914612f, -134.899902f, -168.947485f, -244.461487f); // Volume that covers water under the barrier between inner and outer center pool
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -32.244919f, 123.822372f, -81.219780f, 68.939758f, -42.968739f, 12.5f); // Water middle channel with bridge, upper section
            AddLiquidPlane(LiquidType.Water, "t50_agua1", -46.537670f, 68.945758f, -63.377460f, 62.118111f, -42.968739f, -45.999279f, LiquidSlantType.WestHighEastLow, 8f); // Water middle channel with bridge, incline)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -47.383591f, 62.128111f, -71.285881f, 29.986389f, -45.999279f, 8f); // Water middle channel with bridge, waterfall)
            AddLiquidPlaneZLevel(LiquidType.Water, "t50_agua1", -46.328320f, 30.177059f, -131.536011f, -113.023521f, -55.968750f, 12.2f); // Water middle channel with bridge, lower section
        }
    }
}
