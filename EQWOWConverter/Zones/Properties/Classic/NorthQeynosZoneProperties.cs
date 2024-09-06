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
    internal class NorthQeynosZoneProperties : ZoneProperties
    {
        public NorthQeynosZoneProperties() : base()
        {
            SetBaseZoneProperties("qeynos2", "North Qeynos", 114f, 678f, 4f, 0, ZoneContinentType.Antonica);
            AddZoneArea("Kliknik Tunnel", 334.274689f, -136.490662f, 100f, 277.038788f, -184.821274f, -500f, "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "", "night", 0.2786121f);
            AddChildZoneArea("North Gate", "North Qeynos", 418.604004f, 230.328903f, 100f, 251.545822f, 7.306570f, -2f, "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "", "night", 0.2786121f);
            AddZoneArea("North Field", 2004.056274f, 1189.263184f, 100f, 383.942444f, -1105.253418f, -2f, "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "", "night", 0.2786121f);
            AddZoneArea("North Field", 2004.056274f, 1189.263184f, 100f, 156.620407f, 409.904510f, -2f, "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "", "night", 0.2786121f);
            AddZoneArea("North Field", 489.984772f, -106.924026f, 100f, 141.125656f, -660.864868f, -2f, "", "", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "", "night", 0.2786121f);
            AddZoneArea("North Underbelly", 239.557587f, 269.499146f, -1.580140f, -4.889460f, 7.821560f, -74.407204f);
            AddZoneArea("North Underbelly", 156.623184f, 295.676483f, -1.580140f, -4.889460f, 7.821560f, -74.407204f);
            AddChildZoneArea("Reflecting Pond", "North Qeynos", 225.163986f, 392.702484f, -4.225650f, 167.449249f, 279.318115f, -500f);
            AddChildZoneArea("The Cobbler", "North Qeynos", -53.441002f, 27.927370f, 100f, -83.798622f, -27.942450f, -1.547380f);
            AddChildZoneArea("The Cobbler", "North Qeynos", -53.441002f, 1.972830f, 100f, -97.868210f, -27.942450f, -1.547380f);
            AddChildZoneArea("Order of the Silent Fist", "North Qeynos", 387.976227f, 417.667633f, 100f, 252.011978f, 245.211823f, -1.547380f);
            AddZoneArea("Crows Pub and Casino", 125.749100f, 377.678131f, 100f, 28.148331f, 308.012634f, -100f);
            AddZoneArea("Crows Pub and Casino", 69.783287f, 377.678131f, 100f, 28.148331f, 280.053406f, -100f);
            AddChildZoneArea("Sneed Galliway's Trading Post", "North Qeynos", 153.636826f, 279.677948f, 100f, 98.028473f, 182.072983f, -1.547380f);
            AddChildZoneArea("Sneed Galliway's Trading Post", "North Qeynos", 153.636826f, 223.864426f, 100f, 84.053848f, 182.072983f, -1.547380f);
            AddChildZoneArea("Ironforge Estate", "North Qeynos", 157.945709f, 49.223141f, 100f, 40.169628f, -28.157061f, -1.547380f);
            AddChildZoneArea("Ironforge Estate", "North Qeynos", 157.945709f, 49.223141f, 100f, 69.815872f, -105.258453f, -1.547380f);
            AddChildZoneArea("Ironforge Estate", "North Qeynos", 157.945709f, 97.868721f, 100f, 14.391950f, 42.199970f, -1.547380f);
            AddChildZoneArea("Ironforge Estate", "North Qeynos", 144.467056f, 128.472595f, 100f, 35.919418f, 42.199970f, -1.547380f);
            AddZoneArea("North Qeynos", 383.006805f, 421.212250f, 100f, -210.144882f, -111.267601f, -320.804321f, "qeynos2-01", "qeynos2-01");
            AddChildZoneArea("Shrine of the Prime Healer", "Temple of Life", 16.545980f, -263.407410f, 100f, -263.607483f, -760.173279f, -400, "qeynos2-04", "qeynos2-04", Configuration.CONFIG_AUDIO_MUSIC_DEFAULT_VOLUME, "space", "space", 0.25f);
            AddZoneArea("Temple of Life", 16.545980f, -107.728653f, 100f, -263.607483f, -760.173279f, -400, "qeynos2-04", "qeynos2-04");
            AddZoneLineBox("qcat", 1056.423950f, -48.181938f, 199.885666f, ZoneLineOrientationType.South, 308.068420f, -153.744324f, -87.613121f, 293.681549f, -168.130386f, -126.259743f);
            AddZoneLineBox("qcat", 888.408264f, 216.199905f, 25.632490f, ZoneLineOrientationType.East, 196.099686f, 350.067566f, -106.426399f, 181.744537f, 335.681549f, -137.562592f);
            AddZoneLineBox("qcat", 636.627319f, 98.454399f, -41.968731f, ZoneLineOrientationType.East, 182.129868f, 77.711632f, -29.531000f, 167.744064f, 41.776329f, -42.468739f);
            AddZoneLineBox("qeynos", 587.979126f, -77.531120f, -0.999980f, ZoneLineOrientationType.South, -34.728909f, 364.036194f, 18.469000f, -56.161228f, 349.683868f, -1.498870f);
            AddZoneLineBox("qeynos", 461.973907f, -440.615234f, -0.999980f, ZoneLineOrientationType.South, -163.580154f, 0.193400f, 18.469000f, -182.130936f, -14.192770f, -1.500000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 1099.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1109.153198f, 200.000000f, 1500.794556f, 1089.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 1079.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1089.153198f, 200.000000f, 1500.794556f, 1069.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 1059.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1069.153198f, 200.000000f, 1500.794556f, 1049.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 1039.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1049.153198f, 200.000000f, 1500.794556f, 1029.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 1019.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1029.153198f, 200.000000f, 1500.794556f, 1009.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 999.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1009.153198f, 200.000000f, 1500.794556f, 989.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 979.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 989.153198f, 200.000000f, 1500.794556f, 969.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 959.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 969.153198f, 200.000000f, 1500.794556f, 949.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 939.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 949.153198f, 200.000000f, 1500.794556f, 929.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 919.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 929.153198f, 200.000000f, 1500.794556f, 909.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 899.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 909.153198f, 200.000000f, 1500.794556f, 889.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 879.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 889.153198f, 200.000000f, 1500.794556f, 869.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 859.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 869.153198f, 200.000000f, 1500.794556f, 849.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 839.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 849.153198f, 200.000000f, 1500.794556f, 829.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 819.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 829.153198f, 200.000000f, 1500.794556f, 809.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 799.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 809.153198f, 200.000000f, 1500.794556f, 789.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 779.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 789.153198f, 200.000000f, 1500.794556f, 769.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 759.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 769.153198f, 200.000000f, 1500.794556f, 749.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 739.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 749.153198f, 200.000000f, 1500.794556f, 729.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 719.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 729.153198f, 200.000000f, 1500.794556f, 709.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 699.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 709.153198f, 200.000000f, 1500.794556f, 689.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 679.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 689.153198f, 200.000000f, 1500.794556f, 669.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 659.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 669.153198f, 200.000000f, 1500.794556f, 649.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 639.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 649.153198f, 200.000000f, 1500.794556f, 629.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 619.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 629.153198f, 200.000000f, 1500.794556f, 609.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 599.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 609.153198f, 200.000000f, 1500.794556f, 589.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 579.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 589.153198f, 200.000000f, 1500.794556f, 569.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 559.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 569.153198f, 200.000000f, 1500.794556f, 549.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 539.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 549.153198f, 200.000000f, 1500.794556f, 529.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 519.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 529.153198f, 200.000000f, 1500.794556f, 509.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 499.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 509.153198f, 200.000000f, 1500.794556f, 489.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 479.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 489.153198f, 200.000000f, 1500.794556f, 469.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 459.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 469.153198f, 200.000000f, 1500.794556f, 449.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 439.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 449.153198f, 200.000000f, 1500.794556f, 429.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 419.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 429.153198f, 200.000000f, 1500.794556f, 409.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 399.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 409.153198f, 200.000000f, 1500.794556f, 389.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 379.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 389.153198f, 200.000000f, 1500.794556f, 369.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 359.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 369.153198f, 200.000000f, 1500.794556f, 349.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 339.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 349.153198f, 200.000000f, 1500.794556f, 329.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 319.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 329.153198f, 200.000000f, 1500.794556f, 309.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 299.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 309.153198f, 200.000000f, 1500.794556f, 289.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 279.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 289.153198f, 200.000000f, 1500.794556f, 269.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 259.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 269.153198f, 200.000000f, 1500.794556f, 249.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 239.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 249.153198f, 200.000000f, 1500.794556f, 229.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 219.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 229.153198f, 200.000000f, 1500.794556f, 209.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 199.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 209.153198f, 200.000000f, 1500.794556f, 189.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 179.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 189.153198f, 200.000000f, 1500.794556f, 169.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 159.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 169.153198f, 200.000000f, 1500.794556f, 149.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 139.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 149.153198f, 200.000000f, 1500.794556f, 129.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 119.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 129.153198f, 200.000000f, 1500.794556f, 109.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 99.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 109.153198f, 200.000000f, 1500.794556f, 89.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 79.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 89.153198f, 200.000000f, 1500.794556f, 69.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 59.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 69.153198f, 200.000000f, 1500.794556f, 49.153198f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 39.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 49.153198f, 200.000000f, 1500.794556f, 29.153200f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, 19.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 29.153200f, 200.000000f, 1500.794556f, 9.153200f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -0.407100f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 9.153200f, 200.000000f, 1500.794556f, -10.846800f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -20.407110f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -10.846800f, 200.000000f, 1500.794556f, -30.846800f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -40.407108f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -30.846800f, 200.000000f, 1500.794556f, -50.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -60.407108f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -50.846802f, 200.000000f, 1500.794556f, -70.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -80.407112f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -70.846802f, 200.000000f, 1500.794556f, -90.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -100.407097f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -90.846802f, 200.000000f, 1500.794556f, -110.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -120.407097f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -110.846802f, 200.000000f, 1500.794556f, -130.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -140.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -130.846802f, 200.000000f, 1500.794556f, -150.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -160.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -150.846802f, 200.000000f, 1500.794556f, -170.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -180.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -170.846802f, 200.000000f, 1500.794556f, -190.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -200.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -190.846802f, 200.000000f, 1500.794556f, -210.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -220.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -210.846802f, 200.000000f, 1500.794556f, -230.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -240.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -230.846802f, 200.000000f, 1500.794556f, -250.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -260.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -250.846802f, 200.000000f, 1500.794556f, -270.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -280.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -270.846802f, 200.000000f, 1500.794556f, -290.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -300.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -290.846802f, 200.000000f, 1500.794556f, -310.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -320.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -310.846802f, 200.000000f, 1500.794556f, -330.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -340.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -330.846802f, 200.000000f, 1500.794556f, -350.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -360.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -350.846802f, 200.000000f, 1500.794556f, -370.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -380.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -370.846802f, 200.000000f, 1500.794556f, -390.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -400.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -390.846802f, 200.000000f, 1500.794556f, -410.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -420.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -410.846802f, 200.000000f, 1500.794556f, -430.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -440.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -430.846802f, 200.000000f, 1500.794556f, -450.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -460.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -450.846802f, 200.000000f, 1500.794556f, -470.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -480.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -470.846802f, 200.000000f, 1500.794556f, -490.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -500.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -490.846802f, 200.000000f, 1500.794556f, -510.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -520.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -510.846802f, 200.000000f, 1500.794556f, -530.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -540.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -530.846802f, 200.000000f, 1500.794556f, -550.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -560.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -550.846802f, 200.000000f, 1500.794556f, -570.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -580.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -570.846802f, 200.000000f, 1500.794556f, -590.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -600.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -590.846802f, 200.000000f, 1500.794556f, -610.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -620.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -610.846802f, 200.000000f, 1500.794556f, -630.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -640.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -630.846802f, 200.000000f, 1500.794556f, -650.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -660.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -650.846802f, 200.000000f, 1500.794556f, -670.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -680.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -670.846802f, 200.000000f, 1500.794556f, -690.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -700.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -690.846802f, 200.000000f, 1500.794556f, -710.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -720.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -710.846802f, 200.000000f, 1500.794556f, -730.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -740.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -730.846802f, 200.000000f, 1500.794556f, -750.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -760.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -750.846802f, 200.000000f, 1500.794556f, -770.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -780.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -770.846802f, 200.000000f, 1500.794556f, -790.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -800.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -790.846802f, 200.000000f, 1500.794556f, -810.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -820.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -810.846802f, 200.000000f, 1500.794556f, -830.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -840.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -830.846802f, 200.000000f, 1500.794556f, -850.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -860.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -850.846802f, 200.000000f, 1500.794556f, -870.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -880.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -870.846802f, 200.000000f, 1500.794556f, -890.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -900.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -890.846802f, 200.000000f, 1500.794556f, -910.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -920.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -910.846802f, 200.000000f, 1500.794556f, -930.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -940.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -930.846802f, 200.000000f, 1500.794556f, -950.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -960.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -950.846802f, 200.000000f, 1500.794556f, -970.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -980.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -970.846802f, 200.000000f, 1500.794556f, -990.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1000.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -990.846802f, 200.000000f, 1500.794556f, -1010.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1020.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1010.846802f, 200.000000f, 1500.794556f, -1030.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1040.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1030.846802f, 200.000000f, 1500.794556f, -1050.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1060.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1050.846802f, 200.000000f, 1500.794556f, -1070.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1080.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1070.846802f, 200.000000f, 1500.794556f, -1090.846802f, -100.000000f);
            AddZoneLineBox("qeytoqrg", -310.758850f, -1099f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1090.846802f, 200.000000f, 1500.794556f, -1150f, -100.000000f);
            AddZoneLineBox("qeynos2", -155.322067f, -699.484131f, -14.999940f, ZoneLineOrientationType.NorthWest, -157.864014f, -472.939392f, 49.568951f, -164.179825f, -478.591766f, 32.480042f);  // To the saucer 1
            AddZoneLineBox("qeynos2", -155.322067f, -699.484131f, -14.999940f, ZoneLineOrientationType.NorthWest, -156.997330f, -474.081573f, 48.106071f, -165.076935f, -477.619629f, 32.480042f);  // To the saucer 2
            AddZoneLineBox("qeynos2", -155.322067f, -699.484131f, -14.999940f, ZoneLineOrientationType.NorthWest, -158.996887f, -471.783356f, 33.000019f, -162.633759f, -479.874207f, 32.480042f);  // To the saucer 3
            AddZoneLineBox("qeynos2", -29.628080f, -154.001953f, 6.000010f, ZoneLineOrientationType.South, -111.775909f, -657.557861f, -80.440407f, -154.130737f, -699.911682f, -120.813408f); // From saucer
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 227.730164f, 394.674011f, 165.504913f, 277.284241f, -1.999990f, 350f); // West water and sewer entry
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", 308.709290f, -152.874634f, 292.515320f, -169.049088f, -1.999990f, 350f); // North sewer entry
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "t50_w1", -41.123249f, -293.180542f, -239.001862f, -504.985931f, -2.009970f, 75f); // Water around paladin guild entry
        }
    }
}
