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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones.Properties
{
    internal class PlaneOfFearZoneProperties : ZoneProperties
    {
        public PlaneOfFearZoneProperties() : base()
        {
            // TODO: Set more zone areas
            // TODO: Use audio track 3
            // TODO: Bug: Fetid House area is broken, fix it
            SetBaseZoneProperties("fearplane", "Plane of Fear", 1282.09f, -1139.03f, 1.67f, 0, ZoneContinentType.Antonica);
            SetZonewideEnvironmentAsPlaneOfFear();
            SetZonewideAmbienceSound("", "night");
            Enable2DSoundInstances("darkwds1", "wind_lp4");

            AddZoneArea("Portal", "fearplane-00", "fearplane-00");
            AddZoneAreaBox("Portal", -701.058350f, 1154.465820f, 215.094101f, -904.929810f, 922.794373f, -45.228722f);

            AddZoneArea("Graveyard", "fearplane-02", "fearplane-02");
            AddZoneAreaBox("Graveyard", 250.347687f, 756.466614f, 264.420441f, 7.684500f, -71.233391f, -45.455978f);
            AddZoneAreaBox("Graveyard", 356.578247f, 555.858459f, 351.132385f, 7.684500f, -71.233391f, -45.455978f);
            AddZoneAreaBox("Graveyard", 393.642639f, 553.420593f, 280.953339f, -392.336639f, -502.222717f, -140.454239f);

            AddZoneArea("Castle", "fearplane-01", "fearplane-01");
            AddZoneAreaBox("Castle", 1061.392822f, 662.627136f, 653.066223f, 508.359253f, 171.546707f, -255.437500f);

            AddZoneArea("Fetid House", "fearplane-06", "fearplane-06");
            AddZoneAreaBox("Fetid House", -32.407242f, 1362.433228f, 10f, -193.475601f, 1192.469238f, -15.045580f);

            AddZoneArea("Boogie House", "fearplane-05", "fearplane-05");
            AddZoneAreaBox("Boogie House", -850.897034f, 526.810608f, 164.968903f, -1123.206177f, 272.315460f, -200f);

            AddZoneLineBox("feerrott", -2347.395752f, 2604.589111f, 10.280410f, ZoneLineOrientationType.North, -790.410828f, 1052.103638f, 150.821121f, -803.796631f, 1015.684509f, 105.875198f);
        }

        protected void SetZonewideEnvironmentAsPlaneOfFear()
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as Plane of Fear, but the zonewide environment settings were already set");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(167, 33, 7, ZoneFogType.Medium, false, 0.75f, 1f, ZoneSkySpecialType.FearPlane);
        }
    }
}
