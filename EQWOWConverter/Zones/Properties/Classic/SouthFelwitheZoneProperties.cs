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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class SouthFelwitheZoneProperties : ZoneProperties
    {
        public SouthFelwitheZoneProperties() : base()
        {
            // TODO: Add more zone areas
            // Bug: Hole in the doorway into the teleport room (bottom).  Might be scale related -- It's also this way on live!  Test after collision changes.
            SetBaseZoneProperties("felwitheb", "Southern Felwithe", -790f, 320f, -10.25f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsOutdoorsWithSky(58, 75, 58, ZoneFogType.Medium, 0.5f, 1f);

            // This track normally plays in north felwithe
            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                SetZonewideMusic("felwithea-02", "felwithea-02", true);

            AddZoneLineBox("felwithea", 336.521210f, -720.996582f, -13.999750f, ZoneLineOrientationType.South, 245.892227f, -825.463867f, -1.531000f, 218.101257f, -839.849731f, -14.500020f);
            AddTeleportPad("felwitheb", 503.797028f, -496.463074f, -5.001940f, ZoneLineOrientationType.West, 435.755615f, -584.819824f, 31.000059f, 6.0f);
            AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 503.520294f, -338.805695f, 3.999970f, 6.0f);
            AddTeleportPad("felwitheb", 603.829346f, -580.765015f, 0.000040f, ZoneLineOrientationType.North, 458.797363f, -601.881165f, 31.000561f, 6.0f);
            AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 745.727112f, -532.876404f, 4.000100f, 6.0f);
            AddTeleportPad("felwitheb", 552.740112f, -743.948242f, 0.000020f, ZoneLineOrientationType.East, 435.665283f, -618.718323f, 31.000561f, 6.0f);
            AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 553.732544f, -919.604370f, 4.000090f, 6.0f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t25_agua1", 567.282104f, -512.480042f, 322.703186f, -713.834900f, -27.999870f, 300f);
        }
    }
}
