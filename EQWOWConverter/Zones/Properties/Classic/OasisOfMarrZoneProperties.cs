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
    internal class OasisOfMarrZoneProperties : ZoneProperties
    {
        public OasisOfMarrZoneProperties() : base()
        {
            // TODO: Add Zone Areas
            SetBaseZoneProperties("oasis", "Oasis of Marr", 903.98f, 490.03f, 6.4f, 0, ZoneContinentType.Antonica);
            Enable2DSoundInstances("ocean", "wind_lp4", "oceanwav");
            AddZoneLineBox("nro", -1858.000000f, 1241.229980f, 30.033600f, ZoneLineOrientationType.North, 2590f, 1500.252441f, 300.000000f, 2560f, 1229.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1221.229980f, 31.204430f, ZoneLineOrientationType.North, 2590f, 1229.252441f, 300.000000f, 2560f, 1209.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1201.229980f, 31.854610f, ZoneLineOrientationType.North, 2590f, 1209.252441f, 300.000000f, 2560f, 1189.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1181.229980f, 32.248322f, ZoneLineOrientationType.North, 2590f, 1189.252441f, 300.000000f, 2560f, 1169.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1161.229980f, 32.642101f, ZoneLineOrientationType.North, 2590f, 1169.252441f, 300.000000f, 2560f, 1149.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1141.229980f, 33.036030f, ZoneLineOrientationType.North, 2590f, 1149.252441f, 300.000000f, 2560f, 1129.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1121.229980f, 33.429749f, ZoneLineOrientationType.North, 2590f, 1129.252441f, 300.000000f, 2560f, 1109.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1101.229980f, 33.559669f, ZoneLineOrientationType.North, 2590f, 1109.252441f, 300.000000f, 2560f, 1089.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1081.229980f, 33.559719f, ZoneLineOrientationType.North, 2590f, 1089.252441f, 300.000000f, 2560f, 1069.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1061.229980f, 33.559502f, ZoneLineOrientationType.North, 2590f, 1069.252441f, 300.000000f, 2560f, 1049.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1041.229980f, 33.423931f, ZoneLineOrientationType.North, 2590f, 1049.252441f, 300.000000f, 2560f, 1029.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1021.229980f, 33.030449f, ZoneLineOrientationType.North, 2590f, 1029.252441f, 300.000000f, 2560f, 1009.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1001.229980f, 32.896721f, ZoneLineOrientationType.North, 2590f, 1009.252502f, 300.000000f, 2560f, 989.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 981.229980f, 32.896290f, ZoneLineOrientationType.North, 2590f, 989.252502f, 300.000000f, 2560f, 969.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 961.229980f, 32.895741f, ZoneLineOrientationType.North, 2590f, 969.252502f, 300.000000f, 2560f, 949.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 941.229980f, 31.942690f, ZoneLineOrientationType.North, 2590f, 949.252502f, 300.000000f, 2560f, 929.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 921.229980f, 29.214920f, ZoneLineOrientationType.North, 2590f, 929.252502f, 300.000000f, 2560f, 909.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 901.229980f, 28.314159f, ZoneLineOrientationType.North, 2590f, 909.252502f, 300.000000f, 2560f, 889.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 881.229980f, 28.314159f, ZoneLineOrientationType.North, 2590f, 889.252502f, 300.000000f, 2560f, 869.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 861.229980f, 28.314199f, ZoneLineOrientationType.North, 2590f, 869.252502f, 300.000000f, 2560f, 849.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 841.229980f, 27.997549f, ZoneLineOrientationType.North, 2590f, 849.252502f, 300.000000f, 2560f, 829.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 821.229980f, 27.092230f, ZoneLineOrientationType.North, 2590f, 829.252502f, 300.000000f, 2560f, 809.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 801.229980f, 25.666780f, ZoneLineOrientationType.North, 2590f, 809.252502f, 300.000000f, 2560f, 789.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 781.229980f, 23.984989f, ZoneLineOrientationType.North, 2590f, 789.252502f, 300.000000f, 2560f, 769.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 761.229980f, 22.306400f, ZoneLineOrientationType.North, 2590f, 769.252502f, 300.000000f, 2560f, 749.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 741.229980f, 21.218809f, ZoneLineOrientationType.North, 2590f, 749.252502f, 300.000000f, 2560f, 729.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 721.229980f, 21.217211f, ZoneLineOrientationType.North, 2590f, 729.252502f, 300.000000f, 2560f, 709.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 701.229980f, 17.291059f, ZoneLineOrientationType.North, 2590f, 709.252502f, 300.000000f, 2560f, 689.252502f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 681.229980f, 11.429590f, ZoneLineOrientationType.North, 2590f, 689.252502f, 300.000000f, 2560f, 669.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 661.229980f, 5.566800f, ZoneLineOrientationType.North, 2590f, 669.252441f, 300.000000f, 2560f, 649.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 641.229980f, 1.750140f, ZoneLineOrientationType.North, 2590f, 649.252441f, 300.000000f, 2560f, 629.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 621.229980f, 1.750060f, ZoneLineOrientationType.North, 2590f, 629.252441f, 300.000000f, 2560f, 609.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 601.229980f, 1.750310f, ZoneLineOrientationType.North, 2590f, 609.252441f, 300.000000f, 2560f, 589.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 581.229980f, 1.749970f, ZoneLineOrientationType.North, 2590f, 589.252441f, 300.000000f, 2560f, 569.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 561.229980f, 1.750130f, ZoneLineOrientationType.North, 2590f, 569.252441f, 300.000000f, 2560f, 549.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 541.229980f, 1.477440f, ZoneLineOrientationType.North, 2590f, 549.252441f, 300.000000f, 2560f, 529.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 521.229980f, 0.696480f, ZoneLineOrientationType.North, 2590f, 529.252441f, 300.000000f, 2560f, 509.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 501.230011f, 0.702600f, ZoneLineOrientationType.North, 2590f, 509.252441f, 300.000000f, 2560f, 489.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 481.230011f, 1.096270f, ZoneLineOrientationType.North, 2590f, 489.252441f, 300.000000f, 2560f, 469.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 461.230011f, 1.489190f, ZoneLineOrientationType.North, 2590f, 469.252441f, 300.000000f, 2560f, 449.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 441.230011f, 0.239070f, ZoneLineOrientationType.North, 2590f, 449.252441f, 300.000000f, 2560f, 429.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 421.230011f, -4.065280f, ZoneLineOrientationType.North, 2590f, 429.252441f, 300.000000f, 2560f, 409.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 401.230011f, -3.834260f, ZoneLineOrientationType.North, 2590f, 409.252441f, 300.000000f, 2560f, 389.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 381.230011f, -1.367880f, ZoneLineOrientationType.North, 2590f, 389.252441f, 300.000000f, 2560f, 369.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 361.230011f, 1.099570f, ZoneLineOrientationType.North, 2590f, 369.252441f, 300.000000f, 2560f, 349.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 341.230011f, 3.158530f, ZoneLineOrientationType.North, 2590f, 349.252441f, 300.000000f, 2560f, 329.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 321.229980f, 4.450130f, ZoneLineOrientationType.North, 2590f, 329.252441f, 300.000000f, 2560f, 309.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 301.229980f, 6.698310f, ZoneLineOrientationType.North, 2590f, 309.252441f, 300.000000f, 2560f, 289.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 281.229980f, 9.426860f, ZoneLineOrientationType.North, 2590f, 289.252441f, 300.000000f, 2560f, 269.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 261.229980f, 12.158010f, ZoneLineOrientationType.North, 2590f, 269.252441f, 300.000000f, 2560f, 249.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 241.229980f, 17.582541f, ZoneLineOrientationType.North, 2590f, 249.252441f, 300.000000f, 2560f, 229.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 221.229980f, 28.020960f, ZoneLineOrientationType.North, 2590f, 229.252441f, 300.000000f, 2560f, 209.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 201.229980f, 35.314270f, ZoneLineOrientationType.North, 2590f, 209.252441f, 300.000000f, 2560f, 189.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 181.229980f, 41.057659f, ZoneLineOrientationType.North, 2590f, 189.252441f, 300.000000f, 2560f, 169.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 161.229980f, 46.801510f, ZoneLineOrientationType.North, 2590f, 169.252441f, 300.000000f, 2560f, 149.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 141.229980f, 51.119049f, ZoneLineOrientationType.North, 2590f, 149.252441f, 300.000000f, 2560f, 129.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 121.229980f, 52.788261f, ZoneLineOrientationType.North, 2590f, 129.252441f, 300.000000f, 2560f, 109.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 101.229980f, 53.347881f, ZoneLineOrientationType.North, 2590f, 109.252441f, 300.000000f, 2560f, 89.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 81.229980f, 53.352081f, ZoneLineOrientationType.North, 2590f, 89.252441f, 300.000000f, 2560f, 69.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 61.229980f, 53.355190f, ZoneLineOrientationType.North, 2590f, 69.252441f, 300.000000f, 2560f, 49.252441f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 41.229980f, 53.176750f, ZoneLineOrientationType.North, 2590f, 49.252441f, 300.000000f, 2560f, 29.252439f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 21.229980f, 52.665791f, ZoneLineOrientationType.North, 2590f, 29.252439f, 300.000000f, 2560f, 9.252440f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, 1.229980f, 52.576851f, ZoneLineOrientationType.North, 2590f, 9.252440f, 300.000000f, 2560f, -10.747560f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -18.770020f, 52.695461f, ZoneLineOrientationType.North, 2590f, -10.747560f, 300.000000f, 2560f, -30.747561f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -38.770020f, 52.815609f, ZoneLineOrientationType.North, 2590f, -30.747561f, 300.000000f, 2560f, -50.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -58.770020f, 52.526031f, ZoneLineOrientationType.North, 2590f, -50.747559f, 300.000000f, 2560f, -70.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -78.770020f, 51.465820f, ZoneLineOrientationType.North, 2590f, -70.747559f, 300.000000f, 2560f, -90.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -98.770020f, 51.639549f, ZoneLineOrientationType.North, 2590f, -90.747559f, 300.000000f, 2560f, -110.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -118.770020f, 52.421059f, ZoneLineOrientationType.North, 2590f, -110.747559f, 300.000000f, 2560f, -130.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -138.770020f, 53.202438f, ZoneLineOrientationType.North, 2590f, -130.747559f, 300.000000f, 2560f, -150.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -158.770020f, 53.810509f, ZoneLineOrientationType.North, 2590f, -150.747559f, 300.000000f, 2560f, -170.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -178.770020f, 54.081211f, ZoneLineOrientationType.North, 2590f, -170.747559f, 300.000000f, 2560f, -190.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -198.770020f, 54.174801f, ZoneLineOrientationType.North, 2590f, -190.747559f, 300.000000f, 2560f, -210.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -218.770020f, 54.177811f, ZoneLineOrientationType.North, 2590f, -210.747559f, 300.000000f, 2560f, -230.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -238.770020f, 54.180111f, ZoneLineOrientationType.North, 2590f, -230.747559f, 300.000000f, 2560f, -250.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -258.770020f, 54.042179f, ZoneLineOrientationType.North, 2590f, -250.747559f, 300.000000f, 2560f, -270.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -278.770020f, 53.646809f, ZoneLineOrientationType.North, 2590f, -270.747559f, 300.000000f, 2560f, -290.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -298.770020f, 53.171070f, ZoneLineOrientationType.North, 2590f, -290.747559f, 300.000000f, 2560f, -310.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -318.770020f, 52.657700f, ZoneLineOrientationType.North, 2590f, -310.747559f, 300.000000f, 2560f, -330.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -338.769989f, 52.145191f, ZoneLineOrientationType.North, 2590f, -330.747559f, 300.000000f, 2560f, -350.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -358.769989f, 51.717529f, ZoneLineOrientationType.North, 2590f, -350.747559f, 300.000000f, 2560f, -370.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -378.769989f, 51.447670f, ZoneLineOrientationType.North, 2590f, -370.747559f, 300.000000f, 2560f, -390.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -398.769989f, 49.524891f, ZoneLineOrientationType.North, 2590f, -390.747559f, 300.000000f, 2560f, -410.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -418.769989f, 46.792030f, ZoneLineOrientationType.North, 2590f, -410.747559f, 300.000000f, 2560f, -430.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -438.769989f, 44.060749f, ZoneLineOrientationType.North, 2590f, -430.747559f, 300.000000f, 2560f, -450.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -458.769989f, 40.781170f, ZoneLineOrientationType.North, 2590f, -450.747559f, 300.000000f, 2560f, -470.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -478.769989f, 36.477970f, ZoneLineOrientationType.North, 2590f, -470.747559f, 300.000000f, 2560f, -490.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -498.769989f, 31.476549f, ZoneLineOrientationType.North, 2590f, -490.747559f, 300.000000f, 2560f, -510.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -518.770020f, 26.128920f, ZoneLineOrientationType.North, 2590f, -510.747559f, 300.000000f, 2560f, -530.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -538.770020f, 20.779930f, ZoneLineOrientationType.North, 2590f, -530.747559f, 300.000000f, 2560f, -550.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -558.770020f, 16.167030f, ZoneLineOrientationType.North, 2590f, -550.747559f, 300.000000f, 2560f, -570.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -578.770020f, 12.928620f, ZoneLineOrientationType.North, 2590f, -570.747559f, 300.000000f, 2560f, -590.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -598.770020f, 11.071880f, ZoneLineOrientationType.North, 2590f, -590.747559f, 300.000000f, 2560f, -610.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -618.770020f, 9.896990f, ZoneLineOrientationType.North, 2590f, -610.747559f, 300.000000f, 2560f, -630.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -638.770020f, 8.722420f, ZoneLineOrientationType.North, 2590f, -630.747559f, 300.000000f, 2560f, -650.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -658.770020f, 8.231810f, ZoneLineOrientationType.North, 2590f, -650.747559f, 300.000000f, 2560f, -670.747559f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -678.770020f, 9.012640f, ZoneLineOrientationType.North, 2590f, -670.747559f, 300.000000f, 2560f, -690.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -698.770020f, 7.365450f, ZoneLineOrientationType.North, 2590f, -690.747498f, 300.000000f, 2560f, -710.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -718.770020f, 4.516680f, ZoneLineOrientationType.North, 2590f, -710.747498f, 300.000000f, 2560f, -730.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -738.770020f, 1.666720f, ZoneLineOrientationType.North, 2590f, -730.747498f, 300.000000f, 2560f, -750.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -758.770020f, -1.020910f, ZoneLineOrientationType.North, 2590f, -750.747498f, 300.000000f, 2560f, -770.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -778.770020f, -3.347860f, ZoneLineOrientationType.North, 2590f, -770.747498f, 300.000000f, 2560f, -790.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -798.770020f, -6.148300f, ZoneLineOrientationType.North, 2590f, -790.747498f, 300.000000f, 2560f, -810.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -818.770020f, -6.406220f, ZoneLineOrientationType.North, 2590f, -810.747498f, 300.000000f, 2560f, -830.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -838.770020f, -6.406150f, ZoneLineOrientationType.North, 2590f, -830.747498f, 300.000000f, 2560f, -850.747498f, -200.000000f);
            AddZoneLineBox("nro", -1858.000000f, -858.770020f, -6.406100f, ZoneLineOrientationType.North, 2590f, -850.747498f, 300.000000f, 2560f, -1200.747498f, -200.000000f);
            AddZoneLineBox("sro", 1433.793579f, 244.703186f, -13.301640f, ZoneLineOrientationType.South, -1890.072144f, 301.506622f, 119.807327f, -1933.566406f, 58.167610f, 1.784040f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 3204.784424f, -761.411926f, -2522.721680f, -1661.221313f, -6.405980f, 100f); // Ocean line
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", 324.968842f, 928.367065f, -592.296204f, -231.830185f, -41.718739f, 300f); // Oasis, north
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -592.196204f, 962.252136f, -1073.070679f, 135.639008f, -41.718739f, 300f); // Oasis, south west
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -592.196204f, 135.739008f, -1130.410278f, -507.798889f, -41.718739f, 300f); // Oasis, south east
        }
    }
}
