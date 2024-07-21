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
    internal class CabilisEastZoneProperties : ZoneProperties
    {
        public CabilisEastZoneProperties()
        {
            SetBaseZoneProperties("cabeast", "Cabilis East", -416f, 1343f, 4f, 0, ZoneContinent.Kunark);
            SetFogProperties(150, 120, 80, 40, 300);
            AddZoneLineBox("fieldofbone", -2557.7278f, 3688.0273f, 4.093815f, ZoneLineOrientationType.East,
                1377.6309f, -455.81412f, 97.201485f, 1346.7754f, -497.1183f, -0.49989557f);
            AddZoneLineBox("fieldofbone", -2747.7383f, 3530.195f, 4.093984f, ZoneLineOrientationType.North,
                1236.0558f, -605.5564f, 128.95297f, 1192.7297f, -635.9432f, -0.4994932f);
            AddZoneLineBox("cabwest", -13.886450f, 314.975342f, 0.000000f, ZoneLineOrientationType.North,
                -3.434140f, 322.059662f, 12.469000f, -21.590590f, 307.681549f, -0.499970f);
            AddZoneLineBox("cabwest", -13.976320f, 338.086029f, -24.860001f, ZoneLineOrientationType.North,
                -6.287930f, 350.279877f, 12.469000f, -18.972679f, 321.680542f, -42.468731f);
            AddZoneLineBox("cabwest", -14.334330f, 371.205414f, 0.000030f, ZoneLineOrientationType.North,
                -13.192510f, 378.441284f, 12.469000f, -21.719170f, 349.526428f, -0.499940f);
            AddZoneLineBox("swampofnohope", 3122.768066f, 3056.380127f, 0.125070f, ZoneLineOrientationType.South,
                -111.601692f, -643.556580f, 77.611252f, -154.608215f, -673.912231f, -0.499930f);
            AddZoneLineBox("swampofnohope", 2972.800537f, 3241.029053f, 0.124830f, ZoneLineOrientationType.East,
                -296.067413f, -489.889130f, 72.299011f, -267.712738f, -533.045593f, -0.499910f);
        }
    }
}
