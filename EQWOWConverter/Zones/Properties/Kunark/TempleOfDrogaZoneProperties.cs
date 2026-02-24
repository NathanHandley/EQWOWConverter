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
    internal class TempleOfDrogaZoneProperties : ZoneProperties
    {
        public TempleOfDrogaZoneProperties() : base()
        {
            AddZoneLineBox("frontiermtns", 3289.730469f, 2965.254150f, 341.723389f, ZoneLineOrientationType.North, 1446.159546f, 394.336365f, 25.083500f, 1425.303467f, 313.316986f, -9.347970f);
            AddZoneLineBox("nurga", -1039.550415f, 104.378319f, -111.968697f, ZoneLineOrientationType.South, -1404.654907f, 291.338409f, -173.331894f, -1477.731323f, 141.998917f, -200.277145f, "West");
            AddZoneLineBox("nurga", -847.560303f, -918.770935f, -175.937393f, ZoneLineOrientationType.South, -1330.795044f, -802.670654f, -31.326920f, -1392.724854f, -946.488586f, -86.248444f, "East");

            AddOctagonLiquidShape(ZoneLiquidType.Blood, "t50_redwat01", 546.104858f, 493.395386f, 1986.771484f, 1916.085938f, 1968.937866f, 1933.504761f, 1968.937866f, 1933.504761f, 529.461609f, 509.902405f, 529.461609f, 509.902405f, -269.906158f, 15f);

            AddDiscardGeometryBox(1564.696899f, 319.372986f, 29.451771f, 1499.377930f, 233.304977f, -1.483220f, "Cat head area at entry"); // Cat head area at entry
            AddDiscardGeometryBox(40.813339f, 26.727400f, 53.282471f, -39.816051f, -119.195923f, -4.358200f, "Floating 0 0 0"); // Floating 0 0 0
        }
    }
}
