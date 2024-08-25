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
    internal class RatheMountainsZoneProperties : ZoneProperties
    {
        public RatheMountainsZoneProperties() : base()
        {
            SetBaseZoneProperties("rathemtn", "Rathe Mountains", 1831f, 3825f, 29.03f, 0, ZoneContinentType.Antonica);
            //AddValidMusicInstanceTrackIndexes(2);
            AddZoneLineBox("feerrott", 313.833893f, 3388.136230f, 0.000060f, ZoneLineOrientationType.South, 607.850098f, -3069.240234f, 77.677612f, 564.785278f, -3162.108887f, -0.499980f);
            AddZoneLineBox("lakerathe", 2707.150635f, -2236.831299f, 1.750170f, ZoneLineOrientationType.North, 3495.903564f, 2999.350830f, 72.285400f, 3401.946777f, 2973.988281f, -4.374810f);
        }
    }
}
