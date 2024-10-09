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
    internal class ErudsCrossingZoneProperties : ZoneProperties
    {
        public ErudsCrossingZoneProperties() : base()
        {
            // TODO: There's a boat that connects to erudnext and qeynos (south)
            SetBaseZoneProperties("erudsxing", "Erud's Crossing", 795f, -1766.9f, 12.36f, 0, ZoneContinentType.Odus);
            SetZonewideAmbienceSound("ocean", "ocean", 0.24445728f, 0.24445728f); // Reduced a bit from EQ (-0.15)

            AddZoneArea("Fishing Camp", "erudsxing-01", "erudsxing-01", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME * 0.8f);
            AddZoneAreaBox("Fishing Camp", -1387.844849f, 1039.357910f, 99.225990f, - 1675.524048f, 651.345337f, -107.792427f);

            AddZoneArea("Shipwreck");
            AddZoneAreaBox("Shipwreck", -1022.513428f, 2311.508545f, 169.701233f, -1509.418091f, 1992.726685f, -313.425903f);

            AddZoneArea("Island West", "erudsxing-00", "erudsxing-00");
            AddZoneAreaBox("Island West", -624.765015f, 1917.588013f, 213.023804f, -1934.694458f, 1432.076538f, -111.334282f);

            AddZoneArea("Island");
            AddZoneAreaBox("Island", -356.611389f, 2056.785889f, 388.188568f, -2012.822754f, 514.036499f, -340.110535f);
            
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3050.016846f, 5036.591309f, -4999.445801f, -3051.582520f, -20.062160f, 500);
        }
    }
}
