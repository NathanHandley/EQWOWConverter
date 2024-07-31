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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class CabilisWestZoneProperties : ZoneProperties
    {
        public CabilisWestZoneProperties() : base()
        {
            SetBaseZoneProperties("cabwest", "Cabilis West", 790f, 165f, 3.75f, 0, ZoneContinent.Kunark);
            SetFogProperties(150, 120, 80, 40, 300);
            AddZoneLineBox("cabeast", -28.278749f, 314.920990f, 0.000030f, ZoneLineOrientationType.South,
                -20.735310f, 322.030548f, 12.469000f, -33.827209f, 302.649109f, -0.499990f);
            AddZoneLineBox("cabeast", -28.944679f, 335.877106f, -24.860720f, ZoneLineOrientationType.South,
                -20.975170f, 350.067322f, 12.469000f, -41.966270f, 321.681580f, -42.468739f);
            AddZoneLineBox("cabeast", -28.406759f, 357.039429f, 0.000260f, ZoneLineOrientationType.South,
                -27.676720f, 364.034607f, 12.469000f, -49.269180f, 349.616089f, -0.500000f);
            AddZoneLineBox("warslikswood", -2253.605225f, -1121.567871f, 262.812622f, ZoneLineOrientationType.West,
                887.849365f, 1192.889526f, 64.138229f, 857.462646f, 1153.048340f, -0.499980f);
            AddZoneLineBox("warslikswood", -2410.033447f, -934.157043f, 262.812653f, ZoneLineOrientationType.North,
                739.584961f, 1343.662231f, 99.151367f, 698.854492f, 1313.275391f, -0.499970f);
            AddZoneLineBox("lakeofillomen", 6520.699707f, -6630.659180f, 35.093719f, ZoneLineOrientationType.South,
                -810.963440f, 783.879944f, 129.847549f, -860.993652f, 753.494934f, -0.500040f);
            AddZoneLineBox("lakeofillomen", 6331.367676f, -6786.975586f, 35.093800f, ZoneLineOrientationType.West,
                -971.431702f, 642.097107f, 166.406494f, -1001.788269f, 595.321289f, -0.499620f);
        }
    }
}
