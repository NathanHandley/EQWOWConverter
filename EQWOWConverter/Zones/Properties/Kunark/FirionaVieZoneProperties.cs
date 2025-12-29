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
    internal class FirionaVieZoneProperties : ZoneProperties
    {
        public FirionaVieZoneProperties() : base()
        {
            AddZoneLineBox("dreadlands", 344.637085f, -6136.062500f, 135.156677f, ZoneLineOrientationType.North,
                435.862000f, 6081.367676f, -34.554569f, 264.265533f, 5902.338379f, -100.851601f);
            AddZoneLineBox("lakeofillomen", 1294.271118f, -4322.280273f, 208.467178f, ZoneLineOrientationType.West,
                1434.877808f, 4066.415771f, 549.152283f, 1029.942871f, 3403.977295f, 143.228012f);
            AddZoneLineBox("swampofnohope", -3529.421875f, 2375.426025f, 16.446251f, ZoneLineOrientationType.North,
                5011.051758f, 4177.166016f, 751.968750f, 2959.246338f, 1712.352905f, -120.101540f); // West
            AddZoneLineBox("swampofnohope", -2535.899170f, -2158.673340f, 44.126610f, ZoneLineOrientationType.North,
                4814.504395f, -1717.879517f, 1012.971863f, 3438.907471f, -3290.471924f, -278.979065f); // East

            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_w1", -355.170837f, 4835.860840f, -10012.150391f, -5163.290039f, -126.468178f, 500f); // Ocean

            // River that runs north-south
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -644.519869f, 2555.889810f, -653.466255f, 2516.106562f, -71.672652f, -126.474779f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -635.364531f, 2556.487966f, -644.554686f, 2517.582807f, -69.784779f, -71.674483f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -624.379631f, 2559.122379f, -635.398566f, 2520.677100f, -60.283366f, -69.785779f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -613.128631f, 2559.407693f, -624.413414f, 2522.665314f, -56.616100f, -60.286483f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -594.951820f, 2562.027849f, -613.162807f, 2522.688014f, -51.298228f, -56.620214f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -583.596076f, 2564.535700f, -594.951724f, 2524.105459f, -45.786683f, -51.301662f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -564.406076f, 2562.069179f, -583.596031f, 2519.958217f, -39.937455f, -45.791321f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -544.496683f, 2556.539383f, -564.406031f, 2517.423128f, -38.844128f, -39.940904f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -534.429276f, 2562.331676f, -544.496683f, 2510.674269f, -36.718862f, -38.845369f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -516.428776f, 2555.323093f, -534.429431f, 2510.898490f, -36.243638f, -36.721748f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -487.739593f, 2562.412059f, -516.428776f, 2518.144400f, -29.926755f, -36.245690f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -479.260679f, 2562.301369f, -487.739583f, 2517.991165f, -28.906183f, -29.929159f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -423.327348f, 2558.623128f, -479.260655f, 2516.522642f, -28.905721f, -28.909976f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -404.226648f, 2564.616362f, -423.327338f, 2517.430428f, -28.889193f, -28.909583f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -373.852079f, 2567.060700f, -404.226648f, 2518.180690f, -22.913555f, -28.891107f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -333.665614f, 2559.995159f, -373.852069f, 2514.080083f, -22.864221f, -22.915700f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -320.156134f, 2551.326455f, -333.665614f, 2506.142166f, -20.950666f, -22.867669f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -289.272652f, 2564.695393f, -320.156134f, 2503.106562f, -20.875428f, -20.953362f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -268.537931f, 2564.652662f, -289.272690f, 2516.552159f, -18.897169f, -20.877959f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -253.903438f, 2556.501797f, -268.537966f, 2507.266628f, -16.874586f, -18.900690f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -245.146531f, 2558.168073f, -253.903466f, 2508.721876f, -16.873793f, -16.876552f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -233.802045f, 2561.379145f, -245.146538f, 2510.162390f, -14.874566f, -16.876552f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -224.054459f, 2565.242379f, -233.802038f, 2508.997076f, -14.830321f, -14.877528f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -213.311200f, 2563.195862f, -224.054486f, 2511.554476f, -12.847338f, -14.833607f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -194.267076f, 2564.534186f, -213.311207f, 2521.613724f, -12.843638f, -12.850058f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -184.298455f, 2564.431092f, -194.267076f, 2521.999369f, -10.218862f, -12.843986f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -143.386886f, 2563.430786f, -184.298455f, 2510.065338f, -10.218538f, -10.221428f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -104.028179f, 2564.856421f, -143.386886f, 2504.204352f, -10.173138f, -10.222373f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", -23.293080f, 2563.214007f, -104.028179f, 2510.915807f, -8.812021f, -10.177007f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 68.304162f, 2670.919214f, -23.293080f, 2510.803438f, -8.812145f, -8.813993f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 146.548048f, 2775.823400f, 68.304162f, 2613.102283f, -8.812593f, -8.815459f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 435.685614f, 2776.444493f, 146.548048f, 2705.582676f, -8.812355f, -8.814983f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 446.421190f, 2763.589666f, 435.685586f, 2709.011234f, -0.347207f, -8.814400f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 476.414376f, 2770.808738f, 446.421190f, 2710.638228f, 5.700593f, -0.350652f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 517.180993f, 2769.648503f, 476.414376f, 2701.333079f, 11.324507f, 5.697145f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 531.498252f, 2767.673717f, 517.180993f, 2705.434524f, 12.346362f, 11.321369f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 545.130655f, 2764.079428f, 531.498252f, 2706.092424f, 14.140548f, 12.342755f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 576.681641f, 2763.599514f, 545.130655f, 2709.780179f, 14.325793f, 14.137214f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 579.383445f, 2765.599679f, 576.681641f, 2717.511662f, 14.750000f, 14.322759f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);          
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 604.705531f, 2764.375369f, 579.383445f, 2725.955262f, 19.566862f, 15.026690f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);           
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 616.146183f, 2763.553614f, 604.705531f, 2723.086131f, 21.028179f, 19.563434f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 626.411724f, 2763.401386f, 616.146183f, 2717.468431f, 21.091145f, 21.024421f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 629.190700f, 2758.554386f, 626.411724f, 2716.590107f, 23.484779f, 21.088862f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 636.259938f, 2756.831628f, 629.190700f, 2716.273173f, 23.522379f, 23.481676f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 654.441390f, 2754.614555f, 636.259938f, 2715.223938f, 24.736969f, 23.519966f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 711.679772f, 2763.527586f, 654.441390f, 2708.625655f, 28.609038f, 24.732007f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 746.662176f, 2762.075355f, 711.679772f, 2718.664717f, 34.313600f, 28.605841f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 828.557348f, 2858.656207f, 746.662176f, 2724.065997f, 34.313379f, 34.309014f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 847.261141f, 2871.326493f, 828.557348f, 2788.509228f, 34.313379f, 34.308931f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 945.643483f, 2970.153093f, 847.261141f, 2810.764562f, 34.313379f, 34.308931f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1206.920938f, 3026.967276f, 945.643483f, 2909.871856f, 34.313379f, 34.308931f, ZoneLiquidSlantType.NorthHighSouthLow, 250f);

            // River that runs east-west
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1210.697144f, 2994.419434f, 1176.712891f, 2984.353516f, 65.847672f, 39.704868f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1210.797729f, 3003.371582f, 1176.212158f, 2994.385254f, 70.799759f, 65.844231f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1210.909790f, 3024.302490f, 1176.049194f, 3003.336914f, 76.924057f, 70.796310f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1204.353516f, 3044.781006f, 1168.063232f, 3024.268066f, 106.259354f, 76.920609f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1204.303223f, 3054.189209f, 1169.068848f, 3044.746582f, 110.059418f, 106.255913f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1210.260620f, 3064.219727f, 1168.584351f, 3054.154541f, 110.062668f, 110.055969f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1210.704590f, 3074.491455f, 1174.065308f, 3064.185547f, 117.526970f, 110.059219f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1211.830811f, 3084.275635f, 1174.812012f, 3074.456543f, 117.531441f, 117.523529f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1216.796143f, 3099.723633f, 1176.600342f, 3084.241211f, 138.267792f, 117.528000f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1221.795166f, 3119.274658f, 1180.494141f, 3099.688965f, 149.633560f, 138.264343f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1212.247681f, 3144.248535f, 1168.536743f, 3119.239746f, 167.837494f, 149.630112f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1212.872192f, 3169.298584f, 1168.843628f, 3144.214111f, 180.019485f, 167.834061f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1220.096680f, 3184.447754f, 1175.122192f, 3169.264404f, 183.844193f, 180.016037f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1324.041748f, 3236.672852f, 1176.891479f, 3184.413086f, 183.844055f, 183.840744f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1326.717651f, 3285.305908f, 1184.643188f, 3236.638184f, 183.854233f, 183.840607f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1316.119263f, 3297.705566f, 1272.364746f, 3285.271240f, 186.594055f, 183.850784f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1314.703613f, 3346.599609f, 1263.879272f, 3297.671143f, 186.593933f, 186.590607f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1313.885376f, 3367.751465f, 1267.215210f, 3346.565186f, 194.340973f, 186.590485f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1317.844238f, 3408.286377f, 1255.528320f, 3367.717041f, 194.343842f, 194.337555f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1321.732300f, 3435.921875f, 1262.405640f, 3408.251953f, 197.544418f, 194.340393f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1324.668823f, 3486.756836f, 1268.861572f, 3435.887451f, 197.756058f, 197.540955f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1315.292847f, 3496.706787f, 1273.908813f, 3486.722168f, 202.656433f, 197.752594f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1316.940552f, 3505.984863f, 1272.021851f, 3496.672607f, 202.656540f, 202.652985f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1313.632202f, 3516.552246f, 1271.579956f, 3505.950439f, 206.156479f, 202.653107f, ZoneLiquidSlantType.WestHighEastLow, 250f);
            AddLiquidPlane(ZoneLiquidType.Water, "d_stwt01", 1312.970093f, 3545.086670f, 1266.212036f, 3516.517334f, 206.156433f, 206.153046f, ZoneLiquidSlantType.WestHighEastLow, 250f);
        }
    }
}
