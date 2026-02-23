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
    internal class GreaterFaydarkZoneProperties : ZoneProperties
    {
        public GreaterFaydarkZoneProperties() : base()
        {       
            AddZoneArea("Kelethin", "gfaydark-04", "gfaydark-04", true, "silence", "silence");
            AddZoneAreaBox("Kelethin", 1218.981567f, 858.808472f, 486.386993f, -736.565674f, -711.635437f, -136.266785f);

            AddZoneArea("Felwithe", "", "", false, "silence", "silence");
            AddZoneAreaBox("Felwithe", -1901.312500f, -2402.857666f, 39.048130f, -2122.025879f, -2712.875732f, 9.969560f);

            AddZoneArea("Outside Felwithe", "gfaydark-02", "gfaydark-00", false);
            AddZoneAreaBox("Outside Felwithe", -1760.329102f, -2114.716309f, 302.008179f, -2357.157959f, -2723.451904f, 4.988010f);

            AddZoneArea("Wizard Spires", "gfaydark-00", "gfaydark-03", false);
            AddZoneAreaBox("Wizard Spires", -1776.816528f, -325.939972f, 328.057770f, -2292.352051f, -551.731445f, -38.319908f);

            AddZoneArea("Crushbone", "gfaydark-05", "gfaydark-05");
            AddZoneAreaBox("Crushbone", 2734.106934f, 276.152039f, 186.932007f, 2227.108154f, -127.627853f, -46.127869f);
            
            AddZoneLineBox("butcher", -1164.1454f, -3082.1367f, 0.00028176606f, ZoneLineOrientationType.North, -1636.052856f, 2614.448242f, 80.942001f, -1604.046753f, 2657.645264f, -0.499690f);
            AddZoneLineBox("crushbone", -625.626038f, 163.201843f, 0.000070f, ZoneLineOrientationType.North, 2670.067139f, -28.324280f, 56.295769f, 2579.850830f, -75.045639f, 15.343880f);
            AddZoneLineBox("felwithea", 41.148460f, 183.167984f, 0.000000f, ZoneLineOrientationType.East, -1917.227173f, -2623.463623f, 46.844002f, -1945.600464f, -2663.089355f, 19.906750f);
            AddZoneLineBox("lfaydark", 2164.083984f, -1199.626953f, 0.000040f, ZoneLineOrientationType.South, -2623.411133f, -1084.083862f, 114.320740f, -2650.334229f, -1130.060669f, -0.499900f);
        }
    }
}
