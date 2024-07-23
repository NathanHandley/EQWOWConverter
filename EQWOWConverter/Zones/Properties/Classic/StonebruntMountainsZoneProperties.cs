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
    internal class StonebruntMountainsZoneProperties : ZoneProperties
    {
        public StonebruntMountainsZoneProperties()
        {
            // TODO: Bad objects in map
            SetBaseZoneProperties("stonebrunt", "Stonebrunt Mountains", -1643.01f, -3427.84f, -6.57f, 0, ZoneContinent.Odus);
            SetFogProperties(235, 235, 235, 10, 800);
            AddZoneLineBox("warrens", -100.582893f, 1145.348877f, -110.968758f, ZoneLineOrientationType.North, -3674.369385f, 2932.535400f, -22.218500f, -3707.896240f, 2908.150146f, -40.187389f);
            AddLiquidPlaneZLevel(LiquidType.Water, "d_w1", -3974.402832f, 5999.776855f, -8973.957031f, -5999.862305f, -399.999573f, 300f); // Ocean
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -4089.780762f, -2618.106445f, -4174.711426f, -2654.849121f, -240.560013f, -399.999725f, LiquidSlantType.NorthHighSouthLow, 250f); // River, starting south going north (from mouth to source)
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -4080.608398f, -2627.503418f, -4089.790762f, -2657.807861f, -229.763794f, -240.560013f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3981.018311f, -2620.621826f, -4080.618398f, -2664.068848f, -141.579254f, -229.764794f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3962.614746f, -2623.791260f, -3981.314209f, -2658.096680f, -136.950165f, -141.850555f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3913.138672f, -2618.743652f, -3962.624756f, -2654.123291f, -123.690201f, -136.951172f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3880.507812f, -2618.375732f, -3913.148682f, -2657.647949f, -115.152710f, -123.691200f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3851.830811f, -2625.285889f, -3880.987305f, -2661.344482f, -112.728111f, -115.223022f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3774.035156f, -2618.516113f, -3851.841064f, -2658.202637f, -107.231216f, -112.729111f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3648.049805f, -2617.774414f, -3774.045166f, -2663.897217f, -96.859993f, -107.232224f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3583.988281f, -2618.612549f, -3648.059814f, -2660.474365f, -90.616226f, -96.860977f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3539.107910f, -2617.342529f, -3583.998047f, -2658.466309f, -76.625137f, -90.617233f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3480.861816f, -2619.671875f, -3539.117676f, -2658.342773f, -54.654530f, -76.626137f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3400.221680f, -2618.359131f, -3480.871582f, -2665.584717f, -54.282051f, -54.655529f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3307.027832f, -2617.618164f, -3400.231689f, -2662.062256f, -48.919472f, -54.283051f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3244.170654f, -2618.382568f, -3307.037842f, -2667.891846f, -45.239109f, -48.920471f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3182.210449f, -2618.400635f, -3244.180420f, -2662.465576f, -41.852558f, -45.240108f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3106.682129f, -2617.072510f, -3182.220703f, -2662.097656f, -31.859449f, -41.853561f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3070.901123f, -2621.034912f, -3107.093994f, -2662.242432f, -27.514740f, -31.912910f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -3024.943604f, -2614.100342f, -3070.911133f, -2664.892822f, -25.367399f, -27.515739f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2978.302979f, -2618.652100f, -3024.953613f, -2660.659668f, -23.584881f, -25.368401f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2936.417480f, -2621.033691f, -2978.312988f, -2660.311523f, -17.651979f, -23.585880f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2895.461426f, -2619.434326f, -2936.427490f, -2655.937256f, -11.548420f, -17.652981f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2850.172363f, -2623.666992f, -2895.471436f, -2662.303223f, -5.464480f, -11.549420f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2808.487061f, -2620.823975f, -2850.182617f, -2660.080811f, -1.107560f, -5.465480f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2788.628418f, -2622.487793f, -2808.497070f, -2656.828857f, 1.117390f, -1.108560f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2752.642578f, -2616.562256f, -2788.638428f, -2661.417236f, 9.804530f, 1.116390f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2699.196045f, -2617.749268f, -2752.652588f, -2654.399658f, 24.110350f, 9.803530f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2679.821533f, -2623.379639f, -2699.206299f, -2659.557617f, 30.077009f, 24.109350f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2648.678711f, -2624.119873f, -2679.831543f, -2667.212402f, 35.807678f, 30.076010f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2586.912354f, -2618.079834f, -2648.688721f, -2666.959717f, 46.733551f, 35.806679f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2541.197754f, -2621.827637f, -2587.356689f, -2658.158936f, 54.022141f, 46.649120f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2495.466553f, -2616.282959f, -2541.207764f, -2664.701660f, 60.852600f, 54.021141f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2451.046387f, -2621.494873f, -2495.476562f, -2665.750244f, 67.936920f, 60.851601f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2410.277832f, -2617.565918f, -2451.056396f, -2668.116943f, 74.031860f, 67.935921f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2386.424072f, -2618.297607f, -2410.287842f, -2662.251221f, 77.505371f, 74.030861f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2342.368896f, -2620.448242f, -2386.434082f, -2662.428467f, 90.354897f, 77.504372f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2311.118896f, -2618.427002f, -2342.378906f, -2656.483398f, 100.508522f, 90.353897f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2280.060791f, -2616.784912f, -2311.128906f, -2666.918945f, 109.957718f, 100.507523f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2245.596191f, -2625.292236f, -2280.070801f, -2665.314209f, 115.192543f, 109.956718f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2212.304199f, -2619.879150f, -2245.606201f, -2661.091309f, 120.448273f, 115.191544f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2185.872314f, -2619.548340f, -2212.314209f, -2659.202881f, 124.896027f, 120.447273f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2152.742188f, -2617.307129f, -2185.882324f, -2664.039551f, 132.571167f, 124.895027f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2106.827393f, -2615.335693f, -2152.752197f, -2662.431885f, 142.337570f, 132.570160f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2075.673340f, -2620.463623f, -2107.213623f, -2663.254639f, 148.978165f, 142.258469f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2053.830566f, -2625.434082f, -2075.683350f, -2665.038330f, 153.179398f, 148.977158f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -2010.718872f, -2618.761230f, -2053.840576f, -2666.657227f, 161.556290f, 153.178391f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1981.852905f, -2617.047119f, -2010.729004f, -2662.668457f, 167.118134f, 161.555283f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1944.471313f, -2618.636719f, -1981.862915f, -2660.250732f, 179.729736f, 167.117126f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1909.095459f, -2620.152832f, -1944.481323f, -2655.091064f, 192.251022f, 179.728714f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1881.718994f, -2620.285645f, -1909.105591f, -2659.541504f, 202.430954f, 192.250015f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1850.718506f, -2623.256836f, -1881.729126f, -2666.062256f, 204.144485f, 202.429962f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1812.166382f, -2617.580566f, -1850.728516f, -2665.861328f, 205.817474f, 204.143463f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1770.397095f, -2617.758301f, -1812.176270f, -2662.278076f, 208.177444f, 205.816483f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1733.140015f, -2616.410645f, -1770.406982f, -2660.315674f, 211.452118f, 208.176437f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1690.075562f, -2617.472900f, -1733.150024f, -2661.624512f, 215.237534f, 211.451126f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1649.064331f, -2621.083008f, -1690.085571f, -2667.390625f, 219.722824f, 215.236542f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1608.585327f, -2615.617676f, -1649.074341f, -2669.679199f, 223.961884f, 219.721817f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1577.601807f, -2616.954590f, -1608.595215f, -2663.372803f, 227.766373f, 223.960876f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1555.604614f, -2626.827881f, -1577.611694f, -2684.968262f, 230.815796f, 227.765366f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1534.433472f, -2637.890137f, -1555.614502f, -2708.599609f, 233.463165f, 230.814774f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1504.335815f, -2654.895264f, -1534.443481f, -2745.604248f, 237.576096f, 233.462158f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1470.759888f, -2704.236084f, -1504.345703f, -2760.171875f, 241.978638f, 237.575073f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1416.989746f, -2716.711670f, -1471.102295f, -2761.514648f, 246.686676f, 241.941315f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1380.878418f, -2716.568848f, -1416.999634f, -2767.531250f, 249.934387f, 246.685654f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1340.413086f, -2718.835449f, -1380.888428f, -2763.892578f, 255.695786f, 249.933380f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1299.599121f, -2719.226807f, -1340.423096f, -2758.354248f, 261.435150f, 255.694763f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1290.953369f, -2718.104980f, -1299.609131f, -2760.415771f, 262.656921f, 261.434143f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1221.065186f, -2712.682617f, -1290.963379f, -2766.467529f, 277.216248f, 262.655914f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1168.302734f, -2714.414795f, -1221.075195f, -2758.406494f, 287.413574f, 277.215240f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1133.334961f, -2712.912109f, -1168.312744f, -2765.770508f, 294.499603f, 287.412567f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1070.510254f, -2711.978760f, -1133.344971f, -2762.202148f, 307.323639f, 294.498596f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -1013.800293f, -2711.414062f, -1070.520264f, -2763.667236f, 315.781830f, 307.322632f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -980.653870f, -2715.934814f, -1013.810425f, -2761.598633f, 320.791962f, 315.780823f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -924.475464f, -2708.155273f, -980.663818f, -2768.097656f, 323.416260f, 320.790955f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -879.396851f, -2713.716797f, -924.485413f, -2761.882812f, 325.632172f, 323.415253f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -830.394348f, -2712.874512f, -879.406860f, -2763.661865f, 338.874786f, 325.631165f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -781.786987f, -2714.538330f, -830.404297f, -2758.021240f, 352.175873f, 338.873749f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -738.163025f, -2703.703125f, -781.796936f, -2767.928711f, 352.562714f, 352.174866f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -691.605103f, -2715.341553f, -738.173035f, -2765.906006f, 352.586212f, 352.561707f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -668.181274f, -2721.933838f, -691.615112f, -2763.689697f, 354.483948f, 352.585175f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -615.935547f, -2731.735596f, -668.191284f, -2840.651855f, 356.730347f, 354.482971f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -558.809631f, -2791.171631f, -615.945557f, -2865.253662f, 358.953186f, 356.729370f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -504.826172f, -2813.828125f, -559.566162f, -2860.765381f, 361.317230f, 358.920441f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -480.896881f, -2816.817627f, -504.836151f, -2865.117432f, 362.624908f, 361.316254f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -438.060425f, -2809.294922f, -480.906891f, -2869.053223f, 372.192230f, 362.623932f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -399.257568f, -2813.171631f, -438.070465f, -2864.957520f, 380.771637f, 372.191254f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -357.660706f, -2812.222168f, -399.267548f, -2863.639160f, 389.431549f, 380.770630f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -314.509705f, -2814.044434f, -357.670715f, -2862.945801f, 397.810394f, 389.430573f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -280.781281f, -2810.888916f, -314.519714f, -2861.467773f, 404.551575f, 397.809387f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -233.099197f, -2802.924561f, -280.791290f, -2868.737061f, 405.647614f, 404.550507f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -180.152893f, -2810.301025f, -233.109207f, -2865.964111f, 406.838959f, 405.646545f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -117.909492f, -2806.558838f, -180.162903f, -2867.300781f, 413.026123f, 406.837952f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -57.808300f, -2811.264404f, -117.919487f, -2861.925049f, 418.979553f, 413.025116f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", -22.269360f, -2809.887451f, -57.818298f, -2874.794678f, 422.086517f, 418.978546f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 20.126051f, -2809.756592f, -22.279360f, -2862.245850f, 425.750366f, 422.085510f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 65.053284f, -2810.480225f, 20.116051f, -2868.475098f, 425.750793f, 425.749359f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 119.188957f, -2810.104004f, 65.043282f, -2861.843750f, 425.690430f, 425.749786f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 148.748184f, -2818.407471f, 119.198957f, -2866.031006f, 429.747833f, 425.690430f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 218.506927f, -2813.279297f, 148.738190f, -2872.738770f, 439.916168f, 429.746826f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 234.074875f, -2818.871094f, 218.496933f, -2868.757812f, 440.322906f, 439.915161f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 319.347870f, -2830.769287f, 234.064880f, -2957.451904f, 440.739532f, 440.321930f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 368.027954f, -2904.849854f, 319.337860f, -2964.127441f, 450.456085f, 440.738525f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 418.233612f, -2910.860107f, 368.017975f, -2963.746582f, 460.135803f, 450.455078f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 467.872070f, -2912.190674f, 418.223602f, -2970.934082f, 460.125214f, 460.134796f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 518.974731f, -2908.742676f, 467.862091f, -2966.251953f, 460.063232f, 460.124146f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 576.072571f, -2909.015381f, 518.964722f, -2969.000977f, 471.068604f, 460.062225f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 632.152954f, -2916.925537f, 575.641357f, -2965.936523f, 482.605591f, 470.979492f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 671.578857f, -2916.782715f, 632.142944f, -2970.213867f, 490.640686f, 482.604614f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 706.714294f, -2915.372559f, 671.568909f, -2961.813965f, 497.795288f, 490.639679f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 772.346619f, -2912.666992f, 706.704346f, -2964.593506f, 513.829712f, 497.794281f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 801.969055f, -2910.493408f, 772.336670f, -2960.027100f, 521.210632f, 513.828735f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 819.860291f, -2911.392090f, 801.959045f, -2968.762451f, 525.984436f, 521.209656f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 889.874023f, -2907.662354f, 819.850342f, -2971.105713f, 552.690491f, 525.983459f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 920.052551f, -2911.710938f, 889.864014f, -2967.290771f, 565.309082f, 552.689514f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 987.972107f, -2912.949219f, 920.042542f, -2966.139160f, 584.528748f, 565.308105f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1019.722717f, -2907.418945f, 987.961975f, -2959.240234f, 594.519348f, 584.527771f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1067.578613f, -2915.862305f, 1019.712708f, -2968.380127f, 597.636780f, 594.518372f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1119.793579f, -2915.357666f, 1067.568604f, -2960.057861f, 600.940613f, 597.635803f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1160.965210f, -2914.784424f, 1119.783569f, -2966.214600f, 611.443542f, 600.939636f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1214.010620f, -2912.607910f, 1160.955200f, -2967.173828f, 625.002625f, 611.442566f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1261.377441f, -2913.531494f, 1214.000610f, -2968.704102f, 637.013428f, 625.001648f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1309.207153f, -2912.212891f, 1261.367432f, -2966.672852f, 649.372375f, 637.012451f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1342.815796f, -2922.927734f, 1309.197144f, -2973.235107f, 656.987793f, 649.371399f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1352.788696f, -2938.175293f, 1342.652100f, -2988.824219f, 659.100769f, 656.955627f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1362.769653f, -2942.566162f, 1352.778564f, -3023.530029f, 661.216003f, 659.099792f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1367.647827f, -2950.374756f, 1362.403320f, -3011.889648f, 662.265015f, 661.149902f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1377.514282f, -2953.017334f, 1367.637817f, -3030.000000f, 664.054810f, 662.264038f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1384.252686f, -2963.877441f, 1377.504150f, -3046.480225f, 665.510315f, 664.053833f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1393.648682f, -2987.812500f, 1384.242554f, -3043.690918f, 667.536926f, 665.509338f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1404.434937f, -2991.024902f, 1393.164795f, -3059.312256f, 669.922852f, 667.430359f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1419.431641f, -3009.501221f, 1404.424805f, -3068.442871f, 673.584167f, 669.921875f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1457.303223f, -3017.948486f, 1419.369629f, -3067.382812f, 677.947815f, 673.576477f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1497.098511f, -3014.421143f, 1457.293335f, -3063.130615f, 682.118530f, 677.946899f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1542.612671f, -3018.024414f, 1497.088501f, -3070.014160f, 687.222290f, 682.117493f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1587.342529f, -3012.524902f, 1542.602661f, -3075.695068f, 691.924011f, 687.221252f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1618.223389f, -3016.144043f, 1587.332520f, -3065.490967f, 695.294983f, 691.923035f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1642.276611f, -3026.022461f, 1618.213501f, -3096.578857f, 696.722107f, 695.294006f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1668.203491f, -3035.243408f, 1642.266602f, -3117.191895f, 698.097656f, 696.721069f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1693.273804f, -3052.572266f, 1668.193481f, -3147.494629f, 699.671692f, 698.096619f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1727.363403f, -3096.843262f, 1693.263794f, -3162.751953f, 701.787354f, 699.670715f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1778.611572f, -3112.801514f, 1727.353394f, -3167.680420f, 706.685120f, 701.786377f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1817.098877f, -3111.739746f, 1778.601440f, -3168.264648f, 710.495911f, 706.684143f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1858.354370f, -3111.848877f, 1817.088745f, -3168.629639f, 716.659912f, 710.494934f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1888.760620f, -3114.855469f, 1858.344360f, -3169.289062f, 721.209229f, 716.658936f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1918.264526f, -3117.059570f, 1888.750610f, -3163.902832f, 725.879944f, 721.208252f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 1971.451782f, -3119.121338f, 1918.254517f, -3168.446289f, 749.570862f, 725.878967f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2018.026855f, -3113.209229f, 1971.441895f, -3157.965332f, 770.882263f, 749.569946f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2055.551270f, -3114.986328f, 2018.016846f, -3164.545166f, 782.663818f, 770.881226f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2093.246826f, -3114.832031f, 2055.541260f, -3169.414062f, 794.177979f, 782.662903f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2118.999268f, -3114.485840f, 2093.236816f, -3164.153076f, 801.878784f, 794.177063f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2137.918701f, -3118.262939f, 2118.989258f, -3182.478516f, 806.407410f, 801.877869f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2163.684814f, -3131.644531f, 2137.908691f, -3219.468262f, 811.734009f, 806.406494f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2219.063232f, -3142.017822f, 2163.674805f, -3260.433350f, 822.781738f, 811.733032f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2261.715088f, -3211.639160f, 2219.053223f, -3267.953369f, 845.086670f, 822.780823f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2304.876465f, -3213.013428f, 2261.705078f, -3263.336670f, 867.508423f, 845.085693f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2319.434570f, -3215.175781f, 2304.866455f, -3261.643311f, 873.454102f, 867.507385f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2348.642578f, -3223.873047f, 2318.664551f, -3264.899414f, 876.655457f, 873.150146f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2389.334717f, -3217.087158f, 2348.632568f, -3264.670410f, 880.929749f, 876.654480f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2419.717529f, -3218.552734f, 2389.324707f, -3260.298584f, 884.150452f, 880.928772f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2439.438721f, -3225.317871f, 2419.707520f, -3275.764893f, 888.680664f, 884.149536f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2451.607178f, -3233.134033f, 2439.428711f, -3297.254639f, 891.491272f, 888.679626f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2463.645752f, -3239.042236f, 2451.597168f, -3312.831055f, 894.264771f, 891.490295f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2483.661621f, -3247.936279f, 2463.635742f, -3343.289551f, 899.060669f, 894.263855f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2510.357178f, -3283.812744f, 2483.651611f, -3358.101318f, 905.162292f, 899.059692f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2541.883789f, -3318.065918f, 2510.347168f, -3359.448730f, 912.238342f, 905.161255f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2587.736572f, -3314.293213f, 2541.873779f, -3364.447998f, 921.151550f, 912.237366f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2620.843262f, -3316.479736f, 2587.726562f, -3362.730469f, 927.316467f, 921.150574f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2632.888672f, -3322.711914f, 2620.604248f, -3373.875732f, 934.308411f, 927.271301f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2653.815918f, -3322.178467f, 2632.878662f, -3372.153809f, 946.536316f, 934.307434f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2676.135742f, -3315.385010f, 2653.805908f, -3369.485107f, 959.657776f, 946.535339f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2691.292725f, -3316.888428f, 2676.125732f, -3359.584229f, 968.589600f, 959.656799f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2707.708496f, -3321.574463f, 2691.282471f, -3360.282227f, 978.702271f, 968.588623f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2719.770996f, -3324.635498f, 2707.698486f, -3360.439697f, 986.788696f, 978.701294f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2740.960938f, -3323.223145f, 2719.760742f, -3361.264404f, 993.669434f, 986.787659f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2773.593018f, -3318.190674f, 2740.388428f, -3360.999756f, 1003.628723f, 993.497925f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2809.927246f, -3316.468262f, 2773.583008f, -3359.766602f, 1014.757751f, 1003.627747f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2844.262451f, -3323.226562f, 2809.917480f, -3366.243164f, 1024.869873f, 1014.756775f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2859.947510f, -3321.327881f, 2844.252441f, -3370.218018f, 1029.221680f, 1024.868896f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2893.027100f, -3317.726074f, 2859.937500f, -3364.785645f, 1038.384155f, 1029.220703f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2919.475098f, -3321.856689f, 2892.660156f, -3359.501221f, 1045.156128f, 1038.286377f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2949.743652f, -3323.900879f, 2919.464844f, -3360.048096f, 1051.021240f, 1045.155151f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 2975.173340f, -3316.475830f, 2949.733643f, -3357.200439f, 1055.734131f, 1051.020264f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3000.567139f, -3313.836182f, 2975.163574f, -3353.527588f, 1060.429810f, 1055.733154f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3018.312988f, -3321.983154f, 3000.557129f, -3359.880371f, 1063.886719f, 1060.428833f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3049.306152f, -3323.345215f, 3018.302979f, -3370.594971f, 1073.814331f, 1063.885742f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3087.033691f, -3316.677490f, 3048.803467f, -3366.509033f, 1086.426758f, 1073.647583f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3106.625244f, -3316.580078f, 3087.023438f, -3364.579102f, 1092.987061f, 1086.425781f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3119.572266f, -3322.431396f, 3106.615479f, -3362.386475f, 1098.101807f, 1092.986084f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3148.788818f, -3320.261230f, 3119.562012f, -3361.094971f, 1106.393921f, 1098.100830f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3180.839355f, -3317.486816f, 3148.778809f, -3359.352295f, 1115.864868f, 1106.392944f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3219.168213f, -3316.917480f, 3180.829346f, -3364.450439f, 1126.916626f, 1115.863892f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3256.671143f, -3321.506836f, 3219.158447f, -3369.697266f, 1131.128052f, 1126.915649f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3286.655029f, -3316.897949f, 3256.138184f, -3367.358154f, 1134.276001f, 1131.070557f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3318.741211f, -3313.639160f, 3286.645020f, -3366.002441f, 1137.656006f, 1134.275024f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3334.924072f, -3318.731201f, 3318.730957f, -3366.458984f, 1143.326660f, 1137.655029f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3362.152100f, -3313.359131f, 3334.914307f, -3363.476562f, 1158.293457f, 1143.325684f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3385.317871f, -3313.538818f, 3362.141846f, -3362.046143f, 1170.605225f, 1158.292480f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3391.398682f, -3317.497070f, 3384.878174f, -3356.647461f, 1173.841431f, 1170.373657f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3403.278809f, -3320.152100f, 3391.388916f, -3359.859619f, 1180.156616f, 1173.840454f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3419.686279f, -3320.491211f, 3403.268555f, -3362.502197f, 1187.091675f, 1180.155640f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3440.931885f, -3323.353271f, 3419.676514f, -3361.234619f, 1188.415771f, 1187.090698f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3465.671387f, -3318.050781f, 3440.145508f, -3369.928955f, 1189.921875f, 1188.367920f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3496.247559f, -3319.329834f, 3465.661377f, -3365.091797f, 1191.786499f, 1189.920898f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3526.294678f, -3316.955078f, 3496.237549f, -3366.149658f, 1194.536987f, 1191.785522f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3564.016602f, -3312.586670f, 3526.284912f, -3362.833984f, 1199.619263f, 1194.536011f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3592.764893f, -3309.061035f, 3564.006836f, -3357.571777f, 1203.390869f, 1199.618286f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3619.196777f, -3314.522461f, 3592.754883f, -3361.519775f, 1207.329834f, 1203.389893f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3648.465332f, -3309.475830f, 3619.186768f, -3373.250244f, 1218.804688f, 1207.328857f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3666.671631f, -3316.888428f, 3648.455322f, -3371.713867f, 1226.348999f, 1218.803711f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3688.787109f, -3314.074219f, 3666.661865f, -3360.305664f, 1235.504150f, 1226.348022f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3718.658203f, -3311.240723f, 3688.776855f, -3365.878418f, 1247.998169f, 1235.503174f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3758.287598f, -3313.494629f, 3718.648438f, -3368.171143f, 1248.094360f, 1247.997192f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3800.054932f, -3315.355225f, 3758.277344f, -3357.985840f, 1245.167114f, 1248.093384f, LiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3802.063721f, -3312.545166f, 3789.618652f, -3318.837646f, 1246.597412f, 1245.415894f, LiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(LiquidType.Water, "t50_sbw1", 3802.124512f, -3353.900879f, 3783.853027f, -3368.494385f, 1247.042480f, 1245.473145f, LiquidSlantType.EastHighWestLow, 250f);
        }
    }
}
