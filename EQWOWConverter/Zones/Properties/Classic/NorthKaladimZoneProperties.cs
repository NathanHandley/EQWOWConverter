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
    internal class NorthKaladimZoneProperties : ZoneProperties
    {
        public NorthKaladimZoneProperties() : base()
        {
            SetBaseZoneProperties("kaladimb", "North Kaladim", -267f, 414f, 3.75f, 0, ZoneContinentType.Faydwer);
            SetZonewideEnvironmentAsIndoors(31, 22, 09, ZoneFogType.Heavy);
            OverrideVertexColorIntensity(0.4);

            AddZoneArea("Gemstone Mine");
            AddZoneAreaBox("Gemstone Mine", 795.009949f, 457.298615f, -38.483971f, 520.949097f, 140.156342f, -133.599564f);

            if (Configuration.CONFIG_AUDIO_USE_ALTERNATE_TRACKS == true)
                AddZoneArea("Temple of Brell Serilis", "kaladima-02", "kaladima-02");
            else
                AddZoneArea("Temple of Brell Serilis");
            AddZoneAreaBox("Temple of Brell Serilis", 1483.537109f, 206.665359f, 92.547607f, 657.100952f, 67.384972f, -2.300760f);

            AddZoneArea("Greybloom Farms");
            AddZoneAreaBox("Greybloom Farms", 700.961914f, -55.230789f, 58.052711f, 569.156677f, -215.507767f, -37.635891f);

            AddZoneArea("Everhot Forge");
            AddZoneAreaBox("Everhot Forge", 406.560242f, -144.992920f, 52.823269f, 340.473755f, -212.804214f, -5.368430f);

            AddZoneArea("Southwest Tunnel", "", "", false, "wind_lp2", "wind_lp2");
            AddZoneAreaBox("Southwest Tunnel", 473.453552f, 358.147095f, 24.729561f, 336.825073f, 256.194977f, -27.863100f);
            AddZoneAreaBox("Southwest Tunnel", 454.195770f, -214.330215f, 44.012981f, 316.785950f, -288.690063f, -27.863100f);

            AddZoneArea("Central Tunnel", "", "", false, "wind_lp4", "wind_lp4");
            AddZoneAreaBox("Central Tunnel", 562.377991f, 182.709274f, 32.009251f, 392.926910f, -35.701130f, -35.316898f);

            AddZoneArea("Northwest Tunnel", "", "", false, "wind_lp4", "wind_lp4");
            AddZoneAreaBox("Northwest Tunnel", 715.138184f, 313.482269f, 102.215286f, 626.388977f, -8.161750f, -38.159229f);
            AddZoneAreaBox("Northwest Tunnel", 715.138184f, 177.230865f, 102.215286f, 557.901489f, -8.161750f, -38.159229f);
            AddZoneAreaBox("Northwest Tunnel", 646.349792f, -178.952515f, 73.669640f, 471.682922f, -286.248322f, -9.044160f);

            AddZoneLineBox("kaladima", 306.093964f, 231.490326f, 0.020500f, ZoneLineOrientationType.South, 394.649292f, 346.066956f, -1.531000f, 397.138519f, 312.694366f, -24.499941f);
            AddZoneLineBox("kaladima", 393.919128f, -263.472565f, 0.000040f, ZoneLineOrientationType.South, 384.053192f, -259.715820f, 22.414330f, 373.654907f, -272.101318f, -0.499970f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 789.445374f, 379.175079f, 736.143677f, 226.058517f, -75.968742f, 50f); // NW Rail Pool
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 1203.536499f, 188.962967f, 1120.689331f, 76.613777f, 22.000019f, 50f); // Outside north temple area
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 628.413330f, -26.542490f, 443.050323f, -200.405060f, -3.999960f, 50f); // Large dock area, north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_agua1", 460.282043f, -42.519150f, 330.709229f, -153.390045f, -3.999960f, 50f); // Large dock area, south
        }
    }
}
