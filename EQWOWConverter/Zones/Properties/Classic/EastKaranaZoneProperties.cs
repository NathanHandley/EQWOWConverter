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
    internal class EastKaranaZoneProperties : ZoneProperties
    {
        public EastKaranaZoneProperties() : base()
        {
            // TODO: Add more zone areas
            SetZonewideAmbienceSound("silence", "darkwds2");
            Enable2DSoundInstances("wind_lp3");

            AddZoneArea("North Karana Bridge", "eastkarana-01", "eastkarana-01");
            AddZoneAreaBox("North Karana Bridge", 447.861389f, 1756.020508f, 338.753632f, -259.226746f, 225.282013f, -307.166260f);

            AddZoneArea("High Hold Gorge", "", "", false, "wind_lp4", "wind_lp4");
            AddZoneAreaBox("High Hold Gorge", 199.264359f, -3199.790283f, 527.914368f, -3156.146973f, -8060.119141f, -151.800049f);
            AddZoneAreaBox("High Hold Gorge", -2822.352539f, -3329.555420f, 850.699219f, -3373.271240f, -8467.166992f, -126.344742f);

            AddZoneArea("Gorge of King Xorbb Entry", "", "", false, "wind_lp2", "wind_lp2");
            AddZoneAreaBox("Gorge of King Xorbb Entry", 5282.254395f, -1306.117920f, 699.679443f, 2186.515137f, -2606.609863f, -105.504097f);

            AddZoneLineBox("beholder", -1385.247f, -659.5757f, 60.639446f, ZoneLineOrientationType.North, 3388.710449f, -2134.555420f, 322.495361f, 3160.392090f, -2401.121826f, -100);
            AddZoneLineBox("northkarana", 10.664860f, -3093.490234f, -37.343510f, ZoneLineOrientationType.West, 38.202431f, 1198.431396f, 32.241810f, -13.265930f, 1182.535156f, -37.843681f);
            AddZoneLineBox("highpass", 818.462402f, 132.797134f, 0.000120f, ZoneLineOrientationType.West, -3062.753662f, -8301.240234f, 737.270081f, -3082.371826f, -8324.481445f, 689.406372f);
           
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3007.819092f, 1837.666504f, -3782.756836f, 551.661438f, -74.156052f, 500f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -3772.680420f, 1837.766504f, -5798.433105f, -4512.786133f, -74.156052f, 500f);
        }
    }
}
