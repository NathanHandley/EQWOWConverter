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
    internal class EastFreeportZoneProperties : ZoneProperties
    {
        public EastFreeportZoneProperties() : base()
        {
            // TODO: There is a boat that goes to ocean of tears (OOT)
            SetBaseZoneProperties("freporte", "East Freeport", -648f, -1097f, -52.2f, 0, ZoneContinentType.Antonica);
            AddZoneArea("Field North Camp", -341.816437f, -60.168308f, 200, -540.543701f, -266.745239f, -60f, "freporte-04", "freporte-04", 0f, "", "night", 0.2786121f);
            AddZoneArea("South Gate", -342.407654f, -411.425812f, 100f, -564.648193f, -887.039978f, -60f, "", "", 0f, "", "night", 0.2786121f);
            AddZoneArea("South Gate", -280.112610f, -560.106567f, 100f, -350.181793f, -770.032776f, -60f, "", "", 0f, "", "night", 0.2786121f);
            AddZoneArea("South Field", -342.360107f, -65.000290f, -55f, -2010.536743f, -966.472961f, -56.5f, "", "", 0f, "", "night", 0.2786121f);
            AddZoneArea("South Field", -348.634613f, -926.667175f, -55f, -2010.536743f, -990.134460f, -56.5f, "", "", 0f, "", "night", 0.2786121f);
            AddZoneArea("South Field", -412.658569f, -990.025208f, -55f, -2010.536743f, -1281.146729f, -56.5f, "", "", 0f, "", "night", 0.2786121f);
            AddZoneArea("Grub n Grog Tavern", 111.895752f, -657.588135f, 100f, 42.035801f, -700.445862f, -60f);
            AddZoneArea("Grub n Grog Tavern", 112.624428f, -699.819946f, 100f, -27.890490f, -769.804749f, -60f);
            AddZoneArea("Freeport Inn", 29.813869f, -600.123047f, 100f, -126.564362f, -770.024475f, -60f, "freporte-02", "freporte-01");
            AddZoneArea("The Dark Corner", -219.618500f, -893.643677f, -81.657059f, -351.893951f, -979.860413f, -121.632477f, "freporte-07", "freporte-07");
            AddZoneArea("The Dark Corner", -172.636154f, -784.387573f, -88.136513f, -363.913147f, -979.860413f, -120.643372f, "freporte-07", "freporte-07");
            AddZoneArea("The Dark Corner", -263.556030f, -730.539856f, -90.290993f, -357.307373f, -877.569580f, -112.743217f, "freporte-07", "freporte-07");
            AddZoneArea("Portside Tunnel", -261.986359f, -960.989624f, -69.464958f, -345.707611f, -1026.206177f, -111, "freporte-06", "freporte-06");
            AddZoneArea("Portside Tunnel", -294.081604f, -984.719849f, -69.464958f, -405.236694f, -1039.854858f, -111, "freporte-06", "freporte-06");
            AddZoneArea("Portside Tunnel", -308.852722f, -987.433044f, -62.899090f, -491.365112f, -1049.549805f, -111, "freporte-06", "freporte-06");
            AddZoneArea("Portside Tunnel", -476.421814f, -989.723145f, -56.559040f, -593.941284f, -1119.387939f, -111, "freporte-06", "freporte-06");
            AddZoneArea("Portside Tunnel", -416.091278f, -1020.019226f, -60.956261f, -423.911926f, -1035.786377f, -111, "freporte-06", "freporte-06");
            AddZoneArea("Undermarket", -111.269089f, -164.807983f, -93.928459f, -250.684006f, -331.247040f, -112.361656f, "freporte-07", "freporte-07");
            AddZoneArea("Undermarket", -14.607160f, -167.825241f, -84.865463f, -112.624840f, -307.052856f, -99.015457f, "freporte-07", "freporte-07");
            AddZoneArea("Central Tunnel", -194.367798f, -79.156219f, - 111.427818f, -353.227386f, -794.283813f, -146.810425f);
            AddZoneArea("Central Tunnel", -152.643631f, -74.156021f, -111.076973f, -225.211441f, -112.659233f, -146.810425f);
            AddZoneArea("Shady Market", -83.636902f, -41.591999f, 50f, -342.550507f, -357.456238f, -84.468719f, "freporte-06", "freporte-06");
            AddZoneArea("Shady Market", 20.045830f, -188.963806f, 50f, -146.404694f, -413.418121f, -84.468719f, "freporte-06", "freporte-06");
            AddZoneArea("South Tunnels", -22.523890f, 414.279907f, -85.259438f, -265.123016f, -168.912506f, -150f, "freporte-06", "freporte-06");
            AddZoneArea("South Tunnels", -337.013611f, 55.357231f, -53.954090f, -392.767914f, -68.238281f, -150f, "freporte-06", "freporte-06");
            AddZoneArea("South Tunnels", -59.927891f, 300.814484f, -53.954090f, -392.767914f, -68.238281f, -150f, "freporte-06", "freporte-06");
            AddZoneArea("Seafarer's Roost", -182.084061f, -797.880615f, 100f, -265.703278f, -909.308350f, -84.705994f, "freporte-03", "freporte-03");
            AddZoneArea("Armor by Ikthar", 83.689796f, -154.161804f, 2.793000f, 28.222139f, -251.543106f, -100f);
            AddZoneArea("Trader's Holiday", 167.701202f, -238.067795f, 2.793000f, 112.369164f, -349.568359f, -100f);
            AddZoneArea("Trader's Holiday", 167.701202f, -238.067795f, 2.793000f, 82.095016f, -279.520264f, -100f);
            AddZoneArea("Velithe & Bardo's", 167.565765f, -364.155640f, 2.793000f, 56.067902f, -419.576172f, -100f);
            AddZoneArea("Velithe & Bardo's", 111.770233f, -392.137573f, 2.793000f, 56.067902f, -503.620117f, -100f);
            AddZoneArea("Leather and Hide", 97.780190f, -532.112793f, 2.793000f, 28.198851f, -587.479797f, -100f);
            AddZoneArea("Hallard's Resales", -14.271970f, -434.044525f, 50f, -125.575272f, -503.412567f, -150f);
            AddZoneArea("Hallard's Resales", -14.271970f, -434.044525f, 50f, -153.549210f, -475.407990f, -150f);
            AddChildZoneArea("Port Authority", "Freeport Port", 111.771873f, -797.825439f, -11.962730f, 42.381168f, -909.340637f, -57.018581f);
            AddZoneArea("Freeport Port", 1064.131104f, -810.813171f, 100f, 118.151123f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", 172.718567f, -810.813171f, 100f, 109.435837f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", 114.289108f, -775.425415f, 100f, -264.093933f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", -261.120789f, -1010.070984f, 100f, -527.188660f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", -521.252380f, -1115.864502f, 100f, -602.331726f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", -593.839050f, -1200.171387f, 100f, -642.278320f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneArea("Freeport Port", -632.776367f, -1298.205444f, 100f, -1406.162598f, -3000f, -200f, "freporte-00", "freporte-00");
            AddZoneLineBox("nro", 4152.241699f, 905.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -98.602051f, 200.000000f, -1366.303711f, -138.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 885.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -138.602051f, 200.000000f, -1366.303711f, -158.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 865.000000f, -28.031231f, ZoneLineOrientationType.South, -1336.303711f, -158.602051f, 200.000000f, -1366.303711f, -178.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 845.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -178.602066f, 200.000000f, -1366.303711f, -198.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 825.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -198.602066f, 200.000000f, -1366.303711f, -218.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 805.000000f, -28.031151f, ZoneLineOrientationType.South, -1336.303711f, -218.602066f, 200.000000f, -1366.303711f, -238.602066f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 785.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -238.602066f, 200.000000f, -1366.303711f, -258.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 765.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -258.602051f, 200.000000f, -1366.303711f, -278.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 745.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -278.602051f, 200.000000f, -1366.303711f, -298.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 725.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -298.602051f, 200.000000f, -1366.303711f, -318.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 705.000000f, -28.030741f, ZoneLineOrientationType.South, -1336.303711f, -318.602051f, 200.000000f, -1366.303711f, -338.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 685.000000f, -28.030991f, ZoneLineOrientationType.South, -1336.303711f, -338.602051f, 200.000000f, -1366.303711f, -358.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 665.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -358.602051f, 200.000000f, -1366.303711f, -378.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 645.000000f, -28.031090f, ZoneLineOrientationType.South, -1336.303711f, -378.602051f, 200.000000f, -1366.303711f, -398.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 625.000000f, -28.031290f, ZoneLineOrientationType.South, -1336.303711f, -398.602051f, 200.000000f, -1366.303711f, -418.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 605.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -418.602051f, 200.000000f, -1366.303711f, -438.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 585.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -438.602051f, 200.000000f, -1366.303711f, -458.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 565.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -458.602051f, 200.000000f, -1366.303711f, -478.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 545.000000f, -28.031281f, ZoneLineOrientationType.South, -1336.303711f, -478.602051f, 200.000000f, -1366.303711f, -498.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 525.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -498.602051f, 200.000000f, -1366.303711f, -518.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 505.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -518.602051f, 200.000000f, -1366.303711f, -538.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 485.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -538.602051f, 200.000000f, -1366.303711f, -558.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 465.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -558.602051f, 200.000000f, -1366.303711f, -578.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 445.000000f, -28.030939f, ZoneLineOrientationType.South, -1336.303711f, -578.602051f, 200.000000f, -1366.303711f, -598.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 425.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -598.602051f, 200.000000f, -1366.303711f, -618.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 405.000000f, -28.031139f, ZoneLineOrientationType.South, -1336.303711f, -618.602051f, 200.000000f, -1366.303711f, -638.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 385.000000f, -28.030809f, ZoneLineOrientationType.South, -1336.303711f, -638.602051f, 200.000000f, -1366.303711f, -658.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 365.000000f, -28.030390f, ZoneLineOrientationType.South, -1336.303711f, -658.602051f, 200.000000f, -1366.303711f, -678.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 345.000000f, -28.031111f, ZoneLineOrientationType.South, -1336.303711f, -678.602051f, 200.000000f, -1366.303711f, -698.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 325.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -698.602051f, 200.000000f, -1366.303711f, -718.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 305.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -718.602051f, 200.000000f, -1366.303711f, -738.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 285.000000f, -28.030560f, ZoneLineOrientationType.South, -1336.303711f, -738.602051f, 200.000000f, -1366.303711f, -758.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 265.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -758.602051f, 200.000000f, -1366.303711f, -778.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 245.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -778.602051f, 200.000000f, -1366.303711f, -798.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 225.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -798.602051f, 200.000000f, -1366.303711f, -818.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 205.000000f, -28.031170f, ZoneLineOrientationType.South, -1336.303711f, -818.602051f, 200.000000f, -1366.303711f, -838.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 185.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -838.602051f, 200.000000f, -1366.303711f, -858.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 165.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -858.602051f, 200.000000f, -1366.303711f, -878.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 145.000000f, -28.031010f, ZoneLineOrientationType.South, -1336.303711f, -878.602051f, 200.000000f, -1366.303711f, -898.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 125.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -898.602051f, 200.000000f, -1366.303711f, -918.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 105.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -918.602051f, 200.000000f, -1366.303711f, -938.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 85.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -938.602051f, 200.000000f, -1366.303711f, -958.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 65.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -958.602051f, 200.000000f, -1366.303711f, -978.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 45.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -978.602051f, 200.000000f, -1366.303711f, -998.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 25.000000f, -28.031071f, ZoneLineOrientationType.South, -1336.303711f, -998.602051f, 200.000000f, -1366.303711f, -1018.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, 5.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1018.602051f, 200.000000f, -1366.303711f, -1038.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -15.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1038.602051f, 200.000000f, -1366.303711f, -1058.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -35.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1058.602051f, 200.000000f, -1366.303711f, -1078.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -55.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1078.602051f, 200.000000f, -1366.303711f, -1098.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -75.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1098.602051f, 200.000000f, -1366.303711f, -1118.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -95.000000f, -28.031179f, ZoneLineOrientationType.South, -1336.303711f, -1118.602051f, 200.000000f, -1366.303711f, -1138.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -115.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1138.602051f, 200.000000f, -1366.303711f, -1158.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -135.000000f, -28.031250f, ZoneLineOrientationType.South, -1336.303711f, -1158.602051f, 200.000000f, -1366.303711f, -1178.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -155.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -1178.602051f, 200.000000f, -1366.303711f, -1198.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -175.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -1198.602051f, 200.000000f, -1366.303711f, -1218.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -195.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -1218.602051f, 200.000000f, -1366.303711f, -1238.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -215.000000f, -28.030870f, ZoneLineOrientationType.South, -1336.303711f, -1238.602051f, 200.000000f, -1366.303711f, -1258.602051f, -100.000000f);
            AddZoneLineBox("nro", 4152.241699f, -230.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -1258.602051f, 200.000000f, -1366.303711f, -1298.602051f, -100.000000f);
            AddZoneLineBox("freportw", -82.741112f, -951.859192f, -27.999960f, ZoneLineOrientationType.East, 462.006012f, -343.977173f, -0.311040f, 433.619080f, -420.036346f, -28.500010f);
            AddZoneLineBox("freportw", -392.460449f, -622.734680f, -28.000040f, ZoneLineOrientationType.North, 154.989761f, -55.088539f, 0.501000f, 93.445801f, -70.162163f, -28.499990f);
            AddZoneLineBox("freportw", -740.355530f, -1630.233276f, -97.968758f, ZoneLineOrientationType.South, -164.879898f, 350.068451f, -85.501068f, -196.100616f, 335.683228f, -98.468712f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_ow1", 1052.497925f, -1188.711670f, -1303.691772f, -2912.643799f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_ow1", -405.173492f, -1016.167297f, -613.165588f, -1201.003418f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_ow1", 391.193787f, -828.030029f, -177.646881f, -1195.420410f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_ow1", -212.893402f, -1035.786987f, -483.346436f, -1188.991699f, -69.966743f, 300f);
            AddLiquidPlaneZLevel(ZoneLiquidType.Water, "d_ow1", -176.359528f, -962.933838f, -213.906982f, -1189.269043f, -69.966743f, 300f);
        }
    }
}
