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
    internal class EmeraldJungleZoneProperties : ZoneProperties
    {
        public EmeraldJungleZoneProperties() : base()
        {
            SetZonewideEnvironmentAsOutdoorsNoSky(22, 26, 23, ZoneFogType.Heavy, 1f);
            SetZonewideAmbienceSound("", "darkwds2");
            Enable2DSoundInstances("swmp1", "swmp3", "wind_lp3", "wtr_pool");

            AddZoneLineBox("fieldofbone", -1234.403564f, -767.976746f, -8.242520f, ZoneLineOrientationType.West,
                -536.343384f, 5581.730957f, 293.219910f, -1628.799805f, 4909.223145f, -143.907333f);
            AddZoneLineBox("citymist", -0.518530f, -774.348938f, 0.000000f, ZoneLineOrientationType.West,
                313.793121f, -1718.521484f, -293.553589f, 255.748840f, -1765.517334f, -340.562408f);
            AddZoneLineBox("trakanon", 3958.170654f, 1500.808472f, -344.466949f, ZoneLineOrientationType.South,
                -3444.631104f, 1653.905151f, -278.644684f, -3532.577637f, 1392.652588f, -369.231049f);

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1b", 4096.996094f, 99.285179f, 3197.614746f, -1874.321289f,
                -364.780731f, 200f);
        }
    }
}
