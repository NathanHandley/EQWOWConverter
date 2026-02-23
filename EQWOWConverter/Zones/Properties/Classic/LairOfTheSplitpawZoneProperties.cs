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
    internal class LairOfTheSplitpawZoneProperties : ZoneProperties
    {
        public LairOfTheSplitpawZoneProperties() : base()
        {
            AddZoneArea("Entry", "paw-01", "paw-01");
            AddZoneAreaBox("Entry", 304.315002f, 164.917114f, 76.783112f, -248.294434f, -242.120102f, -14.874890f);

            AddZoneArea("Inner Pass", "paw-00", "paw-00");
            AddZoneAreaBox("Inner Pass", 414.866638f, 160.217361f, 60.530499f, 283.930664f, -10.214720f, -1.216620f);

            AddZoneArea("Prison", "paw-02", "paw-02");
            AddZoneAreaBox("Prison", 533.833862f, 355.961914f, 15.6f, 374.045227f, 120.173988f, -4.906780f);
            AddZoneAreaBox("Prison", 435.937653f, 357.812988f, 15.6f, 264.710632f, 193.642593f, 5.684790f);
            AddZoneAreaBox("Prison", 581.031372f, 241.429214f, 15.6f, 542.096375f, 181.379074f, 8.723260f);
            AddZoneAreaBox("Prison", 550.940308f, 217.146118f, 12.983020f, 502.867828f, 180.879517f, 8.999980f);

            AddZoneLineBox("southkarana", -3118.534424f, 908.824341f, -11.938860f, ZoneLineOrientationType.West, -95.775307f, 64.159088f, 14.467540f, -112.163208f, 29.530199f, -0.499950f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 1171.132324f, 159.264618f, 1098.533813f, 89.405617f, -5.999900f, 100f); // Western water (higher) - Northmost
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 1098.633813f, 198.262665f, 931.602183f, 82.047173f, -5.999900f, 100f); // Western water (higher) - Southmost
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 931.612183f, 223.869247f, 679.216797f, 20.784540f, -31.999950f, 100f); // Western water (lower) - Southwest
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 918.777344f, 20.794540f, 784.150146f, -90.593658f, -31.999950f, 100f); // Western water (lower) - Eastern
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 711.465454f, -170.612061f, 618.156555f, -243.180054f, -0.009000f, 100f); // Southeast Water, upper column and part swim under
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 723.807068f, -112.854370f, 661.784668f, -173.061676f, -40.978719f, 100f); // Southeast Water, middle
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 727.727112f, 19.525690f, 616.720337f, -117.570297f, -40.978719f, 100f); // Southeast Water, southwest
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 769.085815f, -69.982788f, 727.627112f, -117.570297f, -40.978719f, 100f); // Southeast Water, southwest
        }
    }
}
