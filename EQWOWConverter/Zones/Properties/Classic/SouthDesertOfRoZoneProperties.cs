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
    internal class SouthDesertOfRoZoneProperties : ZoneProperties
    {
        public SouthDesertOfRoZoneProperties() : base()
        {
            // TODO: Add more zone areas
            AddZoneArea("Desert", "", "", false, "", "silence");
            AddZoneAreaBox("Desert", 2655.683594f, 1829.046021f, 396.438385f, -924.193787f, -1038.153442f, -238.829483f);

            AddZoneLineBox("innothule", 2537.843262f, 1157.335449f, -28.670191f, ZoneLineOrientationType.South, -3172.916504f, 1030f, 38.835121f, -3225.501709f, 1057.282593f, -30f);
            AddZoneLineBox("oasis", -1859.231567f, 182.460098f, 2.406740f, ZoneLineOrientationType.North, 1526.327637f, 9.256500f, 131.793716f, 1478.424438f, 292.955048f, 1.148580f);

            AddDiscardGeometryBoxMapGenOnly(3307.842773f, 2282.771484f, 1356.119629f, -3674.315430f, 1544.682129f, -200.446121f, "West wall"); // West wall
            AddDiscardGeometryBoxMapGenOnly(3597.069336f, 2262.191650f, 1164.133789f, 2182.963623f, -1836.698120f, -267.743073f, "North edge"); // North edge
            AddDiscardGeometryBoxMapGenOnly(3630.897705f, -955.310730f, 2998.388428f, -3670.284424f, -1793.413086f, -381.375549f, "East edge"); // East edge
        }
    }
}
