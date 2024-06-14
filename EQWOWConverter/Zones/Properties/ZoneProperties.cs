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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Common;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Zones
{
    internal class ZoneProperties
    {
        static private Dictionary<string, ZoneProperties> ZonePropertyListByShortName = new Dictionary<string, ZoneProperties>();

        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;
        public ZoneContinent Continent;
        public ColorRGBA FogColor = new ColorRGBA();
        public int FogMinClip = -1;
        public int FogMaxClip = -1;
        public bool DoShowSky = true;
        public Vector3 SafePosition = new Vector3();
        public float SafeOrientation = 0;
        public HashSet<string> NonCollisionMaterialNames = new HashSet<string>();
        public List<ZonePropertiesLineBox> ZoneLineBoxes = new List<ZonePropertiesLineBox>();
        public List<ZonePropertiesLiquidVolume> LiquidVolumes = new List<ZonePropertiesLiquidVolume>();
        public ZonePropertiesLiquidProperties LiquidProperties = new ZonePropertiesLiquidProperties();            

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        public void SetBaseZoneProperties(string shortName, string descriptiveName, float safeX, float safeY, float safeZ, float safeOrientation, ZoneContinent continent)
        {
            ShortName = shortName;
            DescriptiveName = descriptiveName;
            Continent = continent;
            SafePosition.X = safeX;
            SafePosition.Y = safeY;
            SafePosition.Z = safeZ;
            SafeOrientation = safeOrientation;
        }

        public void SetFogProperties(byte red, byte green, byte blue, int minClip, int maxClip)
        {
            FogColor.R = red;
            FogColor.G = green;
            FogColor.B = blue;
            FogMinClip = minClip;
            FogMaxClip = maxClip;
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        // The box is oriented when facing north (when using .gps, orientation = 0 and no tilt) since zone lines are axis aligned in EQ
        public void AddZoneLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY,
            float targetZonePositionZ, ZoneLineOrientationType targetZoneOrientation, float boxTopNorthwestX, float boxTopNorthwestY, 
            float boxTopNorthwestZ, float boxBottomSoutheastX, float boxBottomSoutheastY, float boxBottomSoutheastZ)
        {
            ZonePropertiesLineBox zoneLineBox = new ZonePropertiesLineBox(targetZoneShortName, targetZonePositionX, targetZonePositionY,
                targetZonePositionZ, targetZoneOrientation, boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ, boxBottomSoutheastX, 
                boxBottomSoutheastY, boxBottomSoutheastZ);
            ZoneLineBoxes.Add(zoneLineBox);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        public void AddTeleportPad(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY, float targetZonePositionZ, 
            ZoneLineOrientationType targetZoneOrientation, float padBottomCenterXPosition, float padBottomCenterYPosition, float padBottomCenterZPosition,
            float padWidth)
        {
            ZonePropertiesLineBox zoneLineBox = new ZonePropertiesLineBox(targetZoneShortName, targetZonePositionX, targetZonePositionY, targetZonePositionZ,
            targetZoneOrientation, padBottomCenterXPosition, padBottomCenterYPosition, padBottomCenterZPosition, padWidth);
            ZoneLineBoxes.Add(zoneLineBox);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        public void AddLiquidVolume(float bottomX, float bottomY, float bottomZ, float topX, float topY, float topZ)
        {
            ZonePropertiesLiquidVolume liquidVolume = new ZonePropertiesLiquidVolume(bottomX, bottomY, bottomZ, topX, topY, topZ);
            LiquidVolumes.Add(liquidVolume);
        }

        public void AddDisabledMaterialCollisionByNames(params string[] names)
        {
            foreach(string name in names)
                if (NonCollisionMaterialNames.Contains(name) == false)
                    NonCollisionMaterialNames.Add(name);
        }

        public void SetLiquidProperties(LiquidType liquidType, params string[] materialNames)
        {
            LiquidProperties = new ZonePropertiesLiquidProperties(liquidType, materialNames);
        }

        public static ZoneProperties GetZonePropertiesForZone(string zoneShortName)
        {
            if (ZonePropertyListByShortName.Count == 0)
                PopulateZonePropertiesList();
            if (ZonePropertyListByShortName.ContainsKey(zoneShortName) == false)
            {
                Logger.WriteError("Error.  Tried to pull Zone Properties for zone with shortname '" + zoneShortName + "' but non existed with that name");
                return new ZoneProperties();
            }
            else
                return ZonePropertyListByShortName[zoneShortName];
        }

        private static void PopulateZonePropertiesList()
        {
            // Classic
            ZonePropertyListByShortName.Add("airplane", GenerateZonePropertiesForZone("airplane"));
            ZonePropertyListByShortName.Add("akanon", GenerateZonePropertiesForZone("akanon"));
            ZonePropertyListByShortName.Add("arena", GenerateZonePropertiesForZone("arena"));
            ZonePropertyListByShortName.Add("befallen", GenerateZonePropertiesForZone("befallen"));
            ZonePropertyListByShortName.Add("beholder", GenerateZonePropertiesForZone("beholder"));
            ZonePropertyListByShortName.Add("blackburrow", GenerateZonePropertiesForZone("blackburrow"));
            ZonePropertyListByShortName.Add("butcher", GenerateZonePropertiesForZone("butcher"));
            ZonePropertyListByShortName.Add("cauldron", GenerateZonePropertiesForZone("cauldron"));
            ZonePropertyListByShortName.Add("cazicthule", GenerateZonePropertiesForZone("cazicthule"));
            ZonePropertyListByShortName.Add("commons", GenerateZonePropertiesForZone("commons"));
            ZonePropertyListByShortName.Add("crushbone", GenerateZonePropertiesForZone("crushbone"));
            ZonePropertyListByShortName.Add("eastkarana", GenerateZonePropertiesForZone("eastkarana"));
            ZonePropertyListByShortName.Add("ecommons", GenerateZonePropertiesForZone("ecommons"));
            ZonePropertyListByShortName.Add("erudnext", GenerateZonePropertiesForZone("erudnext"));
            ZonePropertyListByShortName.Add("erudnint", GenerateZonePropertiesForZone("erudnint"));
            ZonePropertyListByShortName.Add("erudsxing", GenerateZonePropertiesForZone("erudsxing"));
            ZonePropertyListByShortName.Add("everfrost", GenerateZonePropertiesForZone("everfrost"));
            ZonePropertyListByShortName.Add("fearplane", GenerateZonePropertiesForZone("fearplane"));
            ZonePropertyListByShortName.Add("feerrott", GenerateZonePropertiesForZone("feerrott"));
            ZonePropertyListByShortName.Add("felwithea", GenerateZonePropertiesForZone("felwithea"));
            ZonePropertyListByShortName.Add("felwitheb", GenerateZonePropertiesForZone("felwitheb"));
            ZonePropertyListByShortName.Add("freporte", GenerateZonePropertiesForZone("freporte"));
            ZonePropertyListByShortName.Add("freportn", GenerateZonePropertiesForZone("freportn"));
            ZonePropertyListByShortName.Add("freportw", GenerateZonePropertiesForZone("freportw"));
            ZonePropertyListByShortName.Add("gfaydark", GenerateZonePropertiesForZone("gfaydark"));
            ZonePropertyListByShortName.Add("grobb", GenerateZonePropertiesForZone("grobb"));
            ZonePropertyListByShortName.Add("gukbottom", GenerateZonePropertiesForZone("gukbottom"));
            ZonePropertyListByShortName.Add("guktop", GenerateZonePropertiesForZone("guktop"));
            ZonePropertyListByShortName.Add("halas", GenerateZonePropertiesForZone("halas"));
            ZonePropertyListByShortName.Add("hateplane", GenerateZonePropertiesForZone("hateplane"));
            ZonePropertyListByShortName.Add("highkeep", GenerateZonePropertiesForZone("highkeep"));
            ZonePropertyListByShortName.Add("highpass", GenerateZonePropertiesForZone("highpass"));
            ZonePropertyListByShortName.Add("hole", GenerateZonePropertiesForZone("hole"));
            ZonePropertyListByShortName.Add("innothule", GenerateZonePropertiesForZone("innothule"));
            ZonePropertyListByShortName.Add("kaladima", GenerateZonePropertiesForZone("kaladima"));
            ZonePropertyListByShortName.Add("kaladimb", GenerateZonePropertiesForZone("kaladimb"));
            ZonePropertyListByShortName.Add("kedge", GenerateZonePropertiesForZone("kedge"));
            ZonePropertyListByShortName.Add("kerraridge", GenerateZonePropertiesForZone("kerraridge"));
            ZonePropertyListByShortName.Add("kithicor", GenerateZonePropertiesForZone("kithicor"));
            ZonePropertyListByShortName.Add("lakerathe", GenerateZonePropertiesForZone("lakerathe"));
            ZonePropertyListByShortName.Add("lavastorm", GenerateZonePropertiesForZone("lavastorm"));
            ZonePropertyListByShortName.Add("lfaydark", GenerateZonePropertiesForZone("lfaydark"));
            ZonePropertyListByShortName.Add("load", GenerateZonePropertiesForZone("load"));
            ZonePropertyListByShortName.Add("mistmoore", GenerateZonePropertiesForZone("mistmoore"));
            ZonePropertyListByShortName.Add("misty", GenerateZonePropertiesForZone("misty"));
            ZonePropertyListByShortName.Add("najena", GenerateZonePropertiesForZone("najena"));
            ZonePropertyListByShortName.Add("nektulos", GenerateZonePropertiesForZone("nektulos"));
            ZonePropertyListByShortName.Add("neriaka", GenerateZonePropertiesForZone("neriaka"));
            ZonePropertyListByShortName.Add("neriakb", GenerateZonePropertiesForZone("neriakb"));
            ZonePropertyListByShortName.Add("neriakc", GenerateZonePropertiesForZone("neriakc"));
            ZonePropertyListByShortName.Add("northkarana", GenerateZonePropertiesForZone("northkarana"));
            ZonePropertyListByShortName.Add("nro", GenerateZonePropertiesForZone("nro"));
            ZonePropertyListByShortName.Add("oasis", GenerateZonePropertiesForZone("oasis"));
            ZonePropertyListByShortName.Add("oggok", GenerateZonePropertiesForZone("oggok"));
            ZonePropertyListByShortName.Add("oot", GenerateZonePropertiesForZone("oot"));
            ZonePropertyListByShortName.Add("paineel", GenerateZonePropertiesForZone("paineel"));
            ZonePropertyListByShortName.Add("paw", GenerateZonePropertiesForZone("paw"));
            ZonePropertyListByShortName.Add("permafrost", GenerateZonePropertiesForZone("permafrost"));
            ZonePropertyListByShortName.Add("qcat", GenerateZonePropertiesForZone("qcat"));
            ZonePropertyListByShortName.Add("qey2hh1", GenerateZonePropertiesForZone("qey2hh1"));
            ZonePropertyListByShortName.Add("qeynos", GenerateZonePropertiesForZone("qeynos"));
            ZonePropertyListByShortName.Add("qeynos2", GenerateZonePropertiesForZone("qeynos2"));
            ZonePropertyListByShortName.Add("qeytoqrg", GenerateZonePropertiesForZone("qeytoqrg"));
            ZonePropertyListByShortName.Add("qrg", GenerateZonePropertiesForZone("qrg"));
            ZonePropertyListByShortName.Add("rathemtn", GenerateZonePropertiesForZone("rathemtn"));
            ZonePropertyListByShortName.Add("rivervale", GenerateZonePropertiesForZone("rivervale"));
            ZonePropertyListByShortName.Add("runnyeye", GenerateZonePropertiesForZone("runnyeye"));
            ZonePropertyListByShortName.Add("soldunga", GenerateZonePropertiesForZone("soldunga"));
            ZonePropertyListByShortName.Add("soldungb", GenerateZonePropertiesForZone("soldungb"));
            ZonePropertyListByShortName.Add("soltemple", GenerateZonePropertiesForZone("soltemple"));
            ZonePropertyListByShortName.Add("southkarana", GenerateZonePropertiesForZone("southkarana"));
            ZonePropertyListByShortName.Add("sro", GenerateZonePropertiesForZone("sro"));
            ZonePropertyListByShortName.Add("steamfont", GenerateZonePropertiesForZone("steamfont"));
            ZonePropertyListByShortName.Add("stonebrunt", GenerateZonePropertiesForZone("stonebrunt"));
            ZonePropertyListByShortName.Add("tox", GenerateZonePropertiesForZone("tox"));
            ZonePropertyListByShortName.Add("tutorial", GenerateZonePropertiesForZone("tutorial"));
            ZonePropertyListByShortName.Add("unrest", GenerateZonePropertiesForZone("unrest"));
            ZonePropertyListByShortName.Add("warrens", GenerateZonePropertiesForZone("warrens"));

            // Kunark
            if (Configuration.CONFIG_GENERATE_KUNARK_ZONES == true)
            {
                ZonePropertyListByShortName.Add("burningwood", GenerateZonePropertiesForZone("burningwood"));
                ZonePropertyListByShortName.Add("cabeast", GenerateZonePropertiesForZone("cabeast"));
                ZonePropertyListByShortName.Add("cabwest", GenerateZonePropertiesForZone("cabwest"));
                ZonePropertyListByShortName.Add("charasis", GenerateZonePropertiesForZone("charasis"));
                ZonePropertyListByShortName.Add("chardok", GenerateZonePropertiesForZone("chardok"));
                ZonePropertyListByShortName.Add("citymist", GenerateZonePropertiesForZone("citymist"));
                ZonePropertyListByShortName.Add("dalnir", GenerateZonePropertiesForZone("dalnir"));
                ZonePropertyListByShortName.Add("dreadlands", GenerateZonePropertiesForZone("dreadlands"));
                ZonePropertyListByShortName.Add("droga", GenerateZonePropertiesForZone("droga"));
                ZonePropertyListByShortName.Add("emeraldjungle", GenerateZonePropertiesForZone("emeraldjungle"));
                ZonePropertyListByShortName.Add("fieldofbone", GenerateZonePropertiesForZone("fieldofbone"));
                ZonePropertyListByShortName.Add("firiona", GenerateZonePropertiesForZone("firiona"));
                ZonePropertyListByShortName.Add("frontiermtns", GenerateZonePropertiesForZone("frontiermtns"));
                ZonePropertyListByShortName.Add("kaesora", GenerateZonePropertiesForZone("kaesora"));
                ZonePropertyListByShortName.Add("karnor", GenerateZonePropertiesForZone("karnor"));
                ZonePropertyListByShortName.Add("kurn", GenerateZonePropertiesForZone("kurn"));
                ZonePropertyListByShortName.Add("lakeofillomen", GenerateZonePropertiesForZone("lakeofillomen"));
                ZonePropertyListByShortName.Add("nurga", GenerateZonePropertiesForZone("nurga"));
                ZonePropertyListByShortName.Add("overthere", GenerateZonePropertiesForZone("overthere"));
                ZonePropertyListByShortName.Add("sebilis", GenerateZonePropertiesForZone("sebilis"));
                ZonePropertyListByShortName.Add("skyfire", GenerateZonePropertiesForZone("skyfire"));
                ZonePropertyListByShortName.Add("swampofnohope", GenerateZonePropertiesForZone("swampofnohope"));
                ZonePropertyListByShortName.Add("timorous", GenerateZonePropertiesForZone("timorous"));
                ZonePropertyListByShortName.Add("trakanon", GenerateZonePropertiesForZone("trakanon"));
                ZonePropertyListByShortName.Add("veeshan", GenerateZonePropertiesForZone("veeshan"));
                ZonePropertyListByShortName.Add("wakening", GenerateZonePropertiesForZone("wakening"));
                ZonePropertyListByShortName.Add("warslikswood", GenerateZonePropertiesForZone("warslikswood"));
            }

            // Velious
            if (Configuration.CONFIG_GENERATE_VELIOUS_ZONES == true)
            {
                ZonePropertyListByShortName.Add("cobaltscar", GenerateZonePropertiesForZone("cobaltscar"));
                ZonePropertyListByShortName.Add("crystal", GenerateZonePropertiesForZone("crystal"));
                ZonePropertyListByShortName.Add("eastwastes", GenerateZonePropertiesForZone("eastwastes"));
                ZonePropertyListByShortName.Add("frozenshadow", GenerateZonePropertiesForZone("frozenshadow"));
                ZonePropertyListByShortName.Add("greatdivide", GenerateZonePropertiesForZone("greatdivide"));
                ZonePropertyListByShortName.Add("growthplane", GenerateZonePropertiesForZone("growthplane"));
                ZonePropertyListByShortName.Add("iceclad", GenerateZonePropertiesForZone("iceclad"));
                ZonePropertyListByShortName.Add("kael", GenerateZonePropertiesForZone("kael"));
                ZonePropertyListByShortName.Add("mischiefplane", GenerateZonePropertiesForZone("mischiefplane"));
                ZonePropertyListByShortName.Add("necropolis", GenerateZonePropertiesForZone("necropolis"));
                ZonePropertyListByShortName.Add("sirens", GenerateZonePropertiesForZone("sirens"));
                ZonePropertyListByShortName.Add("skyshrine", GenerateZonePropertiesForZone("skyshrine"));
                ZonePropertyListByShortName.Add("sleeper", GenerateZonePropertiesForZone("sleeper"));
                ZonePropertyListByShortName.Add("templeveeshan", GenerateZonePropertiesForZone("templeveeshan"));
                ZonePropertyListByShortName.Add("thurgadina", GenerateZonePropertiesForZone("thurgadina"));
                ZonePropertyListByShortName.Add("thurgadinb", GenerateZonePropertiesForZone("thurgadinb"));
                ZonePropertyListByShortName.Add("velketor", GenerateZonePropertiesForZone("velketor"));
                ZonePropertyListByShortName.Add("westwastes", GenerateZonePropertiesForZone("westwastes"));
            }
        }

        // Define what area should only be considered when generating the world geometry, to get rid of things like temp rooms and flying tree in lfay etc
        private static ZoneProperties GenerateZonePropertiesForZone(string zoneShortName)
        {
            ZoneProperties zoneProperties = new ZoneProperties();

            switch (zoneShortName)
            {
                //--------------------------------------------------------------------------------------------------------------------------
                // Classic Zones
                //--------------------------------------------------------------------------------------------------------------------------
                case "airplane":
                    {
                        // TODO: Add teleport pads
                        zoneProperties.SetBaseZoneProperties("airplane", "Plane of Sky", 542.45f, 1384.6f, -650f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("freporte", -363.75037f, -1778.4629f, 100f, ZoneLineOrientationType.West, 3000f, 3000f, -1000f, -3000f, -3000f, -1200f);
                    }
                    break;
                case "akanon":
                    {
                        zoneProperties.SetBaseZoneProperties("akanon", "Ak'Anon", -35f, 47f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(30, 60, 30, 10, 600);
                        zoneProperties.AddZoneLineBox("steamfont", -2059.579834f, 528.815857f, -111.126549f, ZoneLineOrientationType.North, 70.830750f, -69.220848f, 12.469000f, 60.770279f, -84.162193f, -0.500000f);
                    }
                    break;
                case "arena":
                    {
                        zoneProperties.SetBaseZoneProperties("arena", "The Arena", 460.9f, -41.4f, -7.38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 100, 100, 10, 1500);
                        zoneProperties.AddZoneLineBox("lakerathe", 2345.1172f, 2692.0679f, 92.193184f, ZoneLineOrientationType.East, -44.28722f, -845.03625f, 45.75025f, -74.66106f, -871.419f, 7.0403852f);
                    }
                    break;
                case "befallen":
                    {
                        zoneProperties.SetBaseZoneProperties("befallen", "Befallen", 35.22f, -75.27f, 2.19f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 30, 90, 10, 175);
                        zoneProperties.AddZoneLineBox("commons", -1155.6317f, 596.3344f, -42.280308f, ZoneLineOrientationType.North, -49.9417f, 42.162197f, 12.469f, -63.428658f, 27.86666f, -0.5000006f);
                    }
                    break;
                case "beholder":
                    {
                        zoneProperties.SetBaseZoneProperties("beholder", "Gorge of King Xorbb", -21.44f, -512.23f, 45.13f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(240, 180, 150, 10, 600);
                        zoneProperties.AddZoneLineBox("runnyeye", -111.2879f, -12.021315f, 0.000001f, ZoneLineOrientationType.East, 911.07355f, -1858.2123f, 15.469f, 894.673f, -1878.7317f, 0.50007594f);
                        zoneProperties.AddZoneLineBox("eastkarana", 3094.991f, -2315.8328f, 23.849806f, ZoneLineOrientationType.South,-1443.1708f, -542.2266f, 423.58218f, -1665.6155f, -747.2683f, 13f);
                    }
                    break;
                case "blackburrow":
                    {
                        zoneProperties.SetBaseZoneProperties("blackburrow", "Blackburrow", 38.92f, -158.97f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 100, 90, 10, 700);
                        zoneProperties.AddZoneLineBox("everfrost", -3027.1943f, -532.2794f, -113.18725f, ZoneLineOrientationType.North, 106.64458f, -329.8163f, 13.469f, 80.88026f, -358.2026f, -0.49926078f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", 3432.621094f, -1142.645020f, 0.000010f, ZoneLineOrientationType.East, -154.74507f, 20.123898f, 10.469f, -174.6326f, 10.831751f, -0.49996006f);
                    }
                    break;
                case "butcher":
                    {
                        // Note: There should be a boat to Firiona Vie [Timorous Deep] (NYI) and a boat to Freeport [Ocean of Tears] (NYI)
                        zoneProperties.SetBaseZoneProperties("butcher", "Butcherblock Mountains", -700f, 2550f, 2.9f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(150, 170, 140, 10, 1000);
                        zoneProperties.AddZoneLineBox("kaladima", -60.207775f, 41.798244f, 0.0010997541f, ZoneLineOrientationType.North, 3145.1406f, -173.6824f, 14.468006f, 3128.918f, -186.06715f, -0.4991133f);
                        zoneProperties.AddZoneLineBox("gfaydark", -1563.382568f, 2626.150391f, -0.126430f, ZoneLineOrientationType.North, -1180.5581f, -3073.2896f, 67.52528f, -1218.3838f, -3150f, -0.4993223f);
                        zoneProperties.AddZoneLineBox("cauldron", 2853.7092f, 264.44955f, 469.3444f, ZoneLineOrientationType.South, -2937.8154f, -317.8051f, 45.09004f, -2957.5332f, -351.47528f, -0.49968797f);
                    }
                    break;                
                case "cauldron":
                    {
                        zoneProperties.SetBaseZoneProperties("cauldron", "Dagnor's Cauldron", 320f, 2815f, 473f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 100, 140, 10, 1000);
                        zoneProperties.AddZoneLineBox("butcher", -2921.925537f, -335.659668f, 0.000200f, ZoneLineOrientationType.North, 2872.3113f, 280.6821f, 496.7702f, 2863.3867f, 247.66762f, 468.8444f);
                        zoneProperties.AddZoneLineBox("kedge", 129.834778f, 19.404051f, 320.322083f, ZoneLineOrientationType.West, -1160.462646f, -1000.650696f, -287.718506f, -1180.848267f, -1024.053711f, -334.875000f);
                        zoneProperties.AddZoneLineBox("unrest", 60.597672f, 329.112183f, 0.000000f, ZoneLineOrientationType.South, -2022.738403f, -616.401611f, 108.469002f, -2054.549072f, -636.787415f, 89.500183f);
                    }
                    break;
                case "cazicthule":
                    {
                        zoneProperties.SetBaseZoneProperties("cazicthule", "Lost Temple of Cazic-Thule", -80f, 80f, 5.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 80, 20, 10, 450);
                        zoneProperties.AddZoneLineBox("feerrott", -1460.633545f, -109.760483f, 47.935600f, ZoneLineOrientationType.North, 42.322739f, -55.775299f, 10.469000f, -0.193150f, -84.162201f, -0.500000f);
                    }
                    break;
                case "commons":
                    {
                        zoneProperties.SetBaseZoneProperties("commons", "West Commonlands", -1334.24f, 209.57f, -51.47f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("befallen", -67.97337f, 34.777237f, 0.0009409825f, ZoneLineOrientationType.South, -1161.5278f, 603.6031f, -29.81225f, -1176.3967f, 588.6972f, -42.781216f);
                        zoneProperties.AddZoneLineBox("kithicor", 1361.589966f, -1139.802246f, -52.093639f, ZoneLineOrientationType.South, 1026.848022f, 4180.347168f, 6.000250f, 987.942383f, 4119.968750f, -52.593540f);
                        zoneProperties.AddZoneLineBox("ecommons", 1158.777954f, 5081.237793f, 17.410320f, ZoneLineOrientationType.East, 1168.570435f, -1619.747314f, 200.000000f, 1148.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1138.777954f, 5081.237793f, 2.592890f, ZoneLineOrientationType.East, 1148.570435f, -1619.747314f, 200.000000f, 1128.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1118.777954f, 5081.237793f, -3.419240f, ZoneLineOrientationType.East, 1128.570435f, -1619.747314f, 200.000000f, 1108.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1098.777954f, 5081.237793f, -9.438890f, ZoneLineOrientationType.East, 1108.570435f, -1619.747314f, 200.000000f, 1088.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1078.777954f, 5081.237793f, -15.464940f, ZoneLineOrientationType.East, 1088.570435f, -1619.747314f, 200.000000f, 1068.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1058.777954f, 5081.237793f, -21.449539f, ZoneLineOrientationType.East, 1068.570435f, -1619.747314f, 200.000000f, 1048.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1038.777954f, 5081.237793f, -22.050051f, ZoneLineOrientationType.East, 1048.570435f, -1619.747314f, 200.000000f, 1028.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 1018.777893f, 5081.237793f, -28.476311f, ZoneLineOrientationType.East, 1028.570435f, -1619.747314f, 200.000000f, 1008.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 998.777893f, 5081.237793f, -34.902721f, ZoneLineOrientationType.East, 1008.570374f, -1619.747314f, 200.000000f, 988.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 978.777893f, 5081.237793f, -41.336720f, ZoneLineOrientationType.East, 988.570374f, -1619.747314f, 200.000000f, 968.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 958.777893f, 5081.237793f, -47.761551f, ZoneLineOrientationType.East, 968.570374f, -1619.747314f, 200.000000f, 948.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 938.777893f, 5081.237793f, -50.970051f, ZoneLineOrientationType.East, 948.570374f, -1619.747314f, 200.000000f, 928.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 918.777893f, 5081.237793f, -50.969872f, ZoneLineOrientationType.East, 928.570374f, -1619.747314f, 200.000000f, 908.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 898.777893f, 5081.237793f, -50.970100f, ZoneLineOrientationType.East, 908.570374f, -1619.747314f, 200.000000f, 888.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 878.777893f, 5081.237793f, -50.970169f, ZoneLineOrientationType.East, 888.570374f, -1619.747314f, 200.000000f, 868.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 858.777893f, 5081.237793f, -50.987450f, ZoneLineOrientationType.East, 868.570374f, -1619.747314f, 200.000000f, 848.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 838.777893f, 5081.237793f, -53.242039f, ZoneLineOrientationType.East, 848.570374f, -1619.747314f, 200.000000f, 828.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 818.777893f, 5081.237793f, -49.842571f, ZoneLineOrientationType.East, 828.570374f, -1619.747314f, 200.000000f, 808.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 798.777893f, 5081.237793f, -46.443378f, ZoneLineOrientationType.East, 808.570374f, -1619.747314f, 200.000000f, 788.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 778.777893f, 5081.237793f, -43.043781f, ZoneLineOrientationType.East, 788.570374f, -1619.747314f, 200.000000f, 768.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 758.777893f, 5081.237793f, -39.655670f, ZoneLineOrientationType.East, 768.570374f, -1619.747314f, 200.000000f, 748.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 738.777893f, 5081.237793f, -38.168171f, ZoneLineOrientationType.East, 748.570374f, -1619.747314f, 200.000000f, 728.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 718.777893f, 5081.237793f, -36.143929f, ZoneLineOrientationType.East, 728.570374f, -1619.747314f, 200.000000f, 708.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 698.777893f, 5081.237793f, -34.119610f, ZoneLineOrientationType.East, 708.570374f, -1619.747314f, 200.000000f, 688.570374f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 678.777893f, 5081.237793f, -32.100288f, ZoneLineOrientationType.East, 688.570374f, -1619.747314f, 200.000000f, 668.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 658.777954f, 5081.237793f, -30.106050f, ZoneLineOrientationType.East, 668.570435f, -1619.747314f, 200.000000f, 648.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 638.777954f, 5081.237793f, -31.591780f, ZoneLineOrientationType.East, 648.570435f, -1619.747314f, 200.000000f, 628.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 618.777954f, 5081.237793f, -33.615952f, ZoneLineOrientationType.East, 628.570435f, -1619.747314f, 200.000000f, 608.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 598.777954f, 5081.237793f, -35.639420f, ZoneLineOrientationType.East, 608.570435f, -1619.747314f, 200.000000f, 588.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 578.777954f, 5081.237793f, -37.663071f, ZoneLineOrientationType.East, 588.570435f, -1619.747314f, 200.000000f, 568.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 558.777954f, 5081.237793f, -39.682640f, ZoneLineOrientationType.East, 568.570435f, -1619.747314f, 200.000000f, 548.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 538.777954f, 5081.237793f, -41.615669f, ZoneLineOrientationType.East, 548.570435f, -1619.747314f, 200.000000f, 528.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 518.777954f, 5081.237793f, -45.022900f, ZoneLineOrientationType.East, 528.570435f, -1619.747314f, 200.000000f, 508.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 498.777954f, 5081.237793f, -48.430302f, ZoneLineOrientationType.East, 508.570435f, -1619.747314f, 200.000000f, 488.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 478.777954f, 5081.237793f, -51.837990f, ZoneLineOrientationType.East, 488.570435f, -1619.747314f, 200.000000f, 468.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 458.777954f, 5081.237793f, -55.218479f, ZoneLineOrientationType.East, 468.570435f, -1619.747314f, 200.000000f, 448.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 438.777954f, 5081.237793f, -55.218769f, ZoneLineOrientationType.East, 448.570435f, -1619.747314f, 200.000000f, 428.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 418.777954f, 5081.237793f, -55.218441f, ZoneLineOrientationType.East, 428.570435f, -1619.747314f, 200.000000f, 408.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 398.777954f, 5081.237793f, -55.218472f, ZoneLineOrientationType.East, 408.570435f, -1619.747314f, 200.000000f, 388.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 378.777954f, 5081.237793f, -55.218540f, ZoneLineOrientationType.East, 388.570435f, -1619.747314f, 200.000000f, 368.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 358.777954f, 5081.237793f, -55.218479f, ZoneLineOrientationType.East, 368.570435f, -1619.747314f, 200.000000f, 348.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 338.777954f, 5081.237793f, -55.217411f, ZoneLineOrientationType.East, 348.570435f, -1619.747314f, 200.000000f, 328.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 318.777954f, 5081.237793f, -55.218578f, ZoneLineOrientationType.East, 328.570435f, -1619.747314f, 200.000000f, 308.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 298.777954f, 5081.237793f, -55.218540f, ZoneLineOrientationType.East, 308.570435f, -1619.747314f, 200.000000f, 288.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 278.777954f, 5081.237793f, -55.218761f, ZoneLineOrientationType.East, 288.570435f, -1619.747314f, 200.000000f, 268.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 258.777954f, 5081.237793f, -55.218281f, ZoneLineOrientationType.East, 268.570435f, -1619.747314f, 200.000000f, 248.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 238.777954f, 5081.237793f, -55.218632f, ZoneLineOrientationType.East, 248.570435f, -1619.747314f, 200.000000f, 228.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 218.777954f, 5081.237793f, -55.218319f, ZoneLineOrientationType.East, 228.570435f, -1619.747314f, 200.000000f, 208.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 198.777954f, 5081.237793f, -55.218620f, ZoneLineOrientationType.East, 208.570435f, -1619.747314f, 200.000000f, 188.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 178.777954f, 5081.237793f, -55.218521f, ZoneLineOrientationType.East, 188.570435f, -1619.747314f, 200.000000f, 168.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 158.777954f, 5081.237793f, -55.248749f, ZoneLineOrientationType.East, 168.570435f, -1619.747314f, 200.000000f, 148.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 138.777954f, 5081.237793f, -59.047550f, ZoneLineOrientationType.East, 148.570435f, -1619.747314f, 200.000000f, 128.570435f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 118.777946f, 5081.237793f, -61.217190f, ZoneLineOrientationType.East, 128.570435f, -1619.747314f, 200.000000f, 108.570427f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 98.777946f, 5081.237793f, -63.389259f, ZoneLineOrientationType.East, 108.570427f, -1619.747314f, 200.000000f, 88.570427f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 78.777962f, 5081.237793f, -65.572212f, ZoneLineOrientationType.East, 88.570427f, -1619.747314f, 200.000000f, 68.570442f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 58.777962f, 5081.237793f, -67.756332f, ZoneLineOrientationType.East, 68.570442f, -1619.747314f, 200.000000f, 48.570438f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 28.777962f, 5081.237793f, -67.756332f, ZoneLineOrientationType.East, 48.570438f, -1619.747314f, 200.000000f, 28.570440f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 18.777960f, 5081.237793f, -71.677933f, ZoneLineOrientationType.East, 28.570440f, -1619.747314f, 200.000000f, 8.570430f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1.222040f, 5081.237793f, -73.329102f, ZoneLineOrientationType.East, 8.570430f, -1619.747314f, 200.000000f, -11.429570f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -21.222040f, 5081.237793f, -74.980499f, ZoneLineOrientationType.East, -11.429570f, -1619.747314f, 200.000000f, -31.429569f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -41.222038f, 5081.237793f, -76.600540f, ZoneLineOrientationType.East, -31.429569f, -1619.747314f, 200.000000f, -51.429569f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -61.222038f, 5081.237793f, -74.328918f, ZoneLineOrientationType.East, -51.429569f, -1619.747314f, 200.000000f, -71.429573f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -81.222038f, 5081.237793f, -72.677887f, ZoneLineOrientationType.East, -71.429573f, -1619.747314f, 200.000000f, -91.429573f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -101.222038f, 5081.237793f, -71.026268f, ZoneLineOrientationType.East, -91.429573f, -1619.747314f, 200.000000f, -111.429573f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -121.222038f, 5081.237793f, -69.375511f, ZoneLineOrientationType.East, -111.429573f, -1619.747314f, 200.000000f, -131.429565f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -141.222046f, 5081.237793f, -67.707352f, ZoneLineOrientationType.East, -131.429565f, -1619.747314f, 200.000000f, -151.429565f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -161.222046f, 5081.237793f, -63.881512f, ZoneLineOrientationType.East, -151.429565f, -1619.747314f, 200.000000f, -171.429565f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -181.222046f, 5081.237793f, -61.701981f, ZoneLineOrientationType.East, -171.429565f, -1619.747314f, 200.000000f, -191.429565f, -1649.747314f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -201.222046f, 5081.237793f, -59.530651f, ZoneLineOrientationType.East, -191.429565f, -1616.641235f, 200.000000f, -211.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -221.222046f, 5081.237793f, -57.366020f, ZoneLineOrientationType.East, -211.429565f, -1616.641235f, 200.000000f, -231.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -241.222046f, 5081.237793f, -55.238590f, ZoneLineOrientationType.East, -231.429565f, -1616.641235f, 200.000000f, -251.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -261.222046f, 5081.237793f, -57.779572f, ZoneLineOrientationType.East, -251.429565f, -1616.641235f, 200.000000f, -271.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -281.222046f, 5081.237793f, -62.237400f, ZoneLineOrientationType.East, -271.429565f, -1616.641235f, 200.000000f, -291.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -301.222046f, 5081.237793f, -66.696938f, ZoneLineOrientationType.East, -291.429565f, -1616.641235f, 200.000000f, -311.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -321.222046f, 5081.237793f, -71.156601f, ZoneLineOrientationType.East, -311.429565f, -1616.641235f, 200.000000f, -331.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -341.222046f, 5081.237793f, -75.596397f, ZoneLineOrientationType.East, -331.429565f, -1616.641235f, 200.000000f, -351.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -361.222046f, 5081.237793f, -77.522232f, ZoneLineOrientationType.East, -351.429565f, -1616.641235f, 200.000000f, -371.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -381.222046f, 5081.237793f, -80.169937f, ZoneLineOrientationType.East, -371.429565f, -1616.641235f, 200.000000f, -391.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -401.222046f, 5081.237793f, -82.816628f, ZoneLineOrientationType.East, -391.429565f, -1616.641235f, 200.000000f, -411.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -421.222046f, 5081.237793f, -85.463211f, ZoneLineOrientationType.East, -411.429565f, -1616.641235f, 200.000000f, -431.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -441.222046f, 5081.237793f, -88.071243f, ZoneLineOrientationType.East, -431.429565f, -1616.641235f, 200.000000f, -451.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -461.222046f, 5081.237793f, -86.145203f, ZoneLineOrientationType.East, -451.429565f, -1616.641235f, 200.000000f, -471.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -481.222046f, 5081.237793f, -83.486809f, ZoneLineOrientationType.East, -471.429565f, -1616.641235f, 200.000000f, -491.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -501.222046f, 5081.237793f, -80.836227f, ZoneLineOrientationType.East, -491.429565f, -1616.641235f, 200.000000f, -511.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -521.222046f, 5081.237793f, -78.198151f, ZoneLineOrientationType.East, -511.429565f, -1616.641235f, 200.000000f, -531.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -541.222046f, 5081.237793f, -75.560722f, ZoneLineOrientationType.East, -531.429565f, -1616.641235f, 200.000000f, -551.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -561.222046f, 5081.237793f, -72.992142f, ZoneLineOrientationType.East, -551.429565f, -1616.641235f, 200.000000f, -571.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -581.222046f, 5081.237793f, -68.537811f, ZoneLineOrientationType.East, -571.429565f, -1616.641235f, 200.000000f, -591.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -601.222046f, 5081.237793f, -64.086182f, ZoneLineOrientationType.East, -591.429565f, -1616.641235f, 200.000000f, -611.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -621.222046f, 5081.237793f, -59.634548f, ZoneLineOrientationType.East, -611.429565f, -1616.641235f, 200.000000f, -631.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -641.222046f, 5081.237793f, -55.166950f, ZoneLineOrientationType.East, -631.429565f, -1616.641235f, 200.000000f, -651.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -661.222046f, 5081.237793f, -48.647518f, ZoneLineOrientationType.East, -651.429565f, -1616.641235f, 200.000000f, -671.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -681.222107f, 5081.237793f, -37.260860f, ZoneLineOrientationType.East, -671.429626f, -1616.641235f, 200.000000f, -691.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -701.222107f, 5081.237793f, -25.872950f, ZoneLineOrientationType.East, -691.429626f, -1616.641235f, 200.000000f, -711.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -721.222107f, 5081.237793f, -14.485130f, ZoneLineOrientationType.East, -711.429626f, -1616.641235f, 200.000000f, -731.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -741.222107f, 5081.237793f, -3.148240f, ZoneLineOrientationType.East, -731.429626f, -1616.641235f, 200.000000f, -751.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -761.222107f, 5081.237793f, 1.779870f, ZoneLineOrientationType.East, -751.429626f, -1616.641235f, 200.000000f, -771.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -781.222107f, 5081.237793f, 8.531090f, ZoneLineOrientationType.East, -771.429626f, -1616.641235f, 200.000000f, -791.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -801.222107f, 5081.237793f, 15.288440f, ZoneLineOrientationType.East, -791.429626f, -1616.641235f, 200.000000f, -811.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -821.222107f, 5081.237793f, 22.050070f, ZoneLineOrientationType.East, -811.429626f, -1616.641235f, 200.000000f, -831.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -841.222107f, 5081.237793f, 28.719170f, ZoneLineOrientationType.East, -831.429626f, -1616.641235f, 200.000000f, -851.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -861.222107f, 5081.237793f, 23.791540f, ZoneLineOrientationType.East, -851.429626f, -1616.641235f, 200.000000f, -871.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -881.222107f, 5081.237793f, 17.033131f, ZoneLineOrientationType.East, -871.429626f, -1616.641235f, 200.000000f, -891.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -901.222107f, 5081.237793f, 10.274550f, ZoneLineOrientationType.East, -891.429626f, -1616.641235f, 200.000000f, -911.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -921.222107f, 5081.237793f, 3.515960f, ZoneLineOrientationType.East, -911.429626f, -1616.641235f, 200.000000f, -931.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -941.222107f, 5081.237793f, -3.184360f, ZoneLineOrientationType.East, -931.429626f, -1616.641235f, 200.000000f, -951.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -961.222107f, 5081.237793f, -2.896860f, ZoneLineOrientationType.East, -951.429626f, -1616.641235f, 200.000000f, -971.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -981.222107f, 5081.237793f, -4.134420f, ZoneLineOrientationType.East, -971.429626f, -1616.641235f, 200.000000f, -991.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1001.222107f, 5081.237793f, -5.372180f, ZoneLineOrientationType.East, -991.429626f, -1616.641235f, 200.000000f, -1011.429626f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1021.222107f, 5081.237793f, -6.611620f, ZoneLineOrientationType.East, -1011.429626f, -1616.641235f, 200.000000f, -1031.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1041.222046f, 5081.237793f, -7.814110f, ZoneLineOrientationType.East, -1031.429565f, -1616.641235f, 200.000000f, -1051.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1061.222046f, 5081.237793f, -1.553180f, ZoneLineOrientationType.East, -1051.429565f, -1616.641235f, 200.000000f, -1071.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1081.222046f, 5081.237793f, 6.107420f, ZoneLineOrientationType.East, -1071.429565f, -1616.641235f, 200.000000f, -1091.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1101.222046f, 5081.237793f, 13.762880f, ZoneLineOrientationType.East, -1091.429565f, -1616.641235f, 200.000000f, -1111.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -1121.222046f, 5081.237793f, 21.418200f, ZoneLineOrientationType.East, -1111.429565f, -1616.641235f, 200.000000f, -1151.429565f, -1646.641235f, -100.000000f);
                        zoneProperties.SetLiquidProperties(LiquidType.Water, "d_w1");
                    }
                    break;
                case "crushbone":
                    {
                        zoneProperties.SetBaseZoneProperties("crushbone", "Crushbone", 158f, -644f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(90, 90, 190, 10, 400);
                        zoneProperties.AddZoneLineBox("gfaydark", 2561.247803f, -52.142502f, 15.843880f, ZoneLineOrientationType.South, -640.919861f, 187.129715f, 39.221329f, -732.241028f, 141.981308f, -0.500000f);
                    }
                    break;                
                case "eastkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("eastkarana", "Eastern Plains of Karana", 0f, 0f, 3.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("beholder", -1385.247f, -659.5757f, 60.639446f, ZoneLineOrientationType.North, 3388.710449f, -2134.555420f, 322.495361f, 3160.392090f, -2401.121826f, -100);
                        zoneProperties.AddZoneLineBox("northkarana", 10.664860f, -3093.490234f, -37.343510f, ZoneLineOrientationType.West, 38.202431f, 1198.431396f, 32.241810f, -13.265930f, 1182.535156f, -37.843681f);
                        zoneProperties.AddZoneLineBox("highpass", -1014.530701f, 112.901894f, -0.000030f, ZoneLineOrientationType.East, -3062.753662f, -8301.240234f, 737.270081f, -3082.371826f, -8324.481445f, 689.406372f);
                    }
                    break;               
                case "ecommons":
                    {
                        zoneProperties.SetBaseZoneProperties("ecommons", "East Commonlands", -1485f, 9.2f, -51f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("nro", 2033.690186f, 1875.838257f, 0.000120f, ZoneLineOrientationType.East, -3004.062744f, -1183.421265f, 28.469000f, -3087.551270f, -1212.701660f, -0.499900f);
                        zoneProperties.AddZoneLineBox("nektulos", -2686.337891f, -529.951477f, -21.531050f, ZoneLineOrientationType.West, 1591.733643f, 696.248291f, 23.553110f, 1554.580811f, 679.187378f, -22.031260f);
                        zoneProperties.AddZoneLineBox("commons", 1158.570435f, -1599.747314f, 6.933440f, ZoneLineOrientationType.West, 1168.777954f, 5131.237793f, 200.000000f, 1148.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1138.570435f, -1599.747314f, 0.914720f, ZoneLineOrientationType.West, 1148.777954f, 5131.237793f, 200.000000f, 1128.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1118.570435f, -1599.747314f, -5.104220f, ZoneLineOrientationType.West, 1128.777954f, 5131.237793f, 200.000000f, 1108.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1098.570435f, -1599.747314f, -11.122930f, ZoneLineOrientationType.West, 1108.777954f, 5131.237793f, 200.000000f, 1088.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1078.570435f, -1599.747314f, -17.141710f, ZoneLineOrientationType.West, 1088.777954f, 5131.237793f, 200.000000f, 1068.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1058.570435f, -1599.747314f, -23.122900f, ZoneLineOrientationType.West, 1068.777954f, 5131.237793f, 200.000000f, 1048.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1038.570435f, -1599.747314f, -29.554140f, ZoneLineOrientationType.West, 1048.777954f, 5131.237793f, 200.000000f, 1028.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 1018.570374f, -1599.747314f, -35.985538f, ZoneLineOrientationType.West, 1028.777954f, 5131.237793f, 200.000000f, 1008.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 998.570374f, -1599.747314f, -42.416641f, ZoneLineOrientationType.West, 1008.777893f, 5131.237793f, 200.000000f, 988.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 978.570374f, -1599.747314f, -48.847809f, ZoneLineOrientationType.West, 988.777893f, 5131.237793f, 200.000000f, 968.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 958.570374f, -1599.747314f, -55.186001f, ZoneLineOrientationType.West, 968.777893f, 5131.237793f, 200.000000f, 948.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 938.570374f, -1599.747314f, -55.184929f, ZoneLineOrientationType.West, 948.777893f, 5131.237793f, 200.000000f, 928.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 918.570374f, -1599.747314f, -55.185928f, ZoneLineOrientationType.West, 928.777893f, 5131.237793f, 200.000000f, 908.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 898.570374f, -1599.747314f, -55.186050f, ZoneLineOrientationType.West, 908.777893f, 5131.237793f, 200.000000f, 888.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 878.570374f, -1599.747314f, -55.186008f, ZoneLineOrientationType.West, 888.777893f, 5131.237793f, 200.000000f, 868.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 858.570374f, -1599.747314f, -55.167400f, ZoneLineOrientationType.West, 868.777893f, 5131.237793f, 200.000000f, 848.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 838.570374f, -1599.747314f, -51.761181f, ZoneLineOrientationType.West, 848.777893f, 5131.237793f, 200.000000f, 828.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 818.570374f, -1599.747314f, -48.354771f, ZoneLineOrientationType.West, 828.777893f, 5131.237793f, 200.000000f, 808.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 798.570374f, -1599.747314f, -44.948689f, ZoneLineOrientationType.West, 808.777893f, 5131.237793f, 200.000000f, 788.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 778.570374f, -1599.747314f, -41.542381f, ZoneLineOrientationType.West, 788.777893f, 5131.237793f, 200.000000f, 768.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 758.570374f, -1599.747314f, -38.165630f, ZoneLineOrientationType.West, 768.777893f, 5131.237793f, 200.000000f, 748.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 738.570374f, -1599.747314f, -36.147209f, ZoneLineOrientationType.West, 748.777893f, 5131.237793f, 200.000000f, 728.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 718.570374f, -1599.747314f, -34.128349f, ZoneLineOrientationType.West, 728.777893f, 5131.237793f, 200.000000f, 708.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 698.570374f, -1599.747314f, -32.109650f, ZoneLineOrientationType.West, 708.777893f, 5131.237793f, 200.000000f, 688.777893f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 678.570374f, -1599.747314f, -30.090839f, ZoneLineOrientationType.West, 688.777893f, 5131.237793f, 200.000000f, 668.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 658.570435f, -1599.747314f, -28.141970f, ZoneLineOrientationType.West, 668.777954f, 5131.237793f, 200.000000f, 648.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 638.570435f, -1599.747314f, -30.156441f, ZoneLineOrientationType.West, 648.777954f, 5131.237793f, 200.000000f, 628.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 618.570435f, -1599.747314f, -32.170738f, ZoneLineOrientationType.West, 628.777954f, 5131.237793f, 200.000000f, 608.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 598.570435f, -1599.747314f, -34.185089f, ZoneLineOrientationType.West, 608.777954f, 5131.237793f, 200.000000f, 588.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 578.570435f, -1599.747314f, -36.207230f, ZoneLineOrientationType.West, 588.777954f, 5131.237793f, 200.000000f, 568.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 558.570435f, -1599.747314f, -38.249901f, ZoneLineOrientationType.West, 568.777954f, 5131.237793f, 200.000000f, 548.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 538.570435f, -1599.747314f, -41.656132f, ZoneLineOrientationType.West, 548.777954f, 5131.237793f, 200.000000f, 528.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 518.570435f, -1599.747314f, -45.062489f, ZoneLineOrientationType.West, 528.777954f, 5131.237793f, 200.000000f, 508.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 498.570435f, -1599.747314f, -48.468410f, ZoneLineOrientationType.West, 508.777954f, 5131.237793f, 200.000000f, 488.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 478.570435f, -1599.747314f, -51.874989f, ZoneLineOrientationType.West, 488.777954f, 5131.237793f, 200.000000f, 468.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 458.570435f, -1599.747314f, -55.217369f, ZoneLineOrientationType.West, 468.777954f, 5131.237793f, 200.000000f, 448.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 438.570435f, -1599.747314f, -55.218510f, ZoneLineOrientationType.West, 448.777954f, 5131.237793f, 200.000000f, 428.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 418.570435f, -1599.747314f, -55.217442f, ZoneLineOrientationType.West, 428.777954f, 5131.237793f, 200.000000f, 408.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 398.570435f, -1599.747314f, -55.218632f, ZoneLineOrientationType.West, 408.777954f, 5131.237793f, 200.000000f, 388.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 378.570435f, -1599.747314f, -55.218700f, ZoneLineOrientationType.West, 388.777954f, 5131.237793f, 200.000000f, 368.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 358.570435f, -1599.747314f, -55.218700f, ZoneLineOrientationType.West, 368.777954f, 5131.237793f, 200.000000f, 348.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 338.570435f, -1599.747314f, -55.218449f, ZoneLineOrientationType.West, 348.777954f, 5131.237793f, 200.000000f, 328.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 318.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 328.777954f, 5131.237793f, 200.000000f, 308.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 298.570435f, -1599.747314f, -55.218651f, ZoneLineOrientationType.West, 308.777954f, 5131.237793f, 200.000000f, 288.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 278.570435f, -1599.747314f, -55.217579f, ZoneLineOrientationType.West, 288.777954f, 5131.237793f, 200.000000f, 268.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 258.570435f, -1599.747314f, -55.218681f, ZoneLineOrientationType.West, 268.777954f, 5131.237793f, 200.000000f, 248.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 238.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 248.777954f, 5131.237793f, 200.000000f, 228.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 218.570435f, -1599.747314f, -55.218658f, ZoneLineOrientationType.West, 228.777954f, 5131.237793f, 200.000000f, 208.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 198.570435f, -1599.747314f, -55.218418f, ZoneLineOrientationType.West, 208.777954f, 5131.237793f, 200.000000f, 188.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 178.570435f, -1599.747314f, -55.218670f, ZoneLineOrientationType.West, 188.777954f, 5131.237793f, 200.000000f, 168.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 158.570435f, -1599.747314f, -55.271351f, ZoneLineOrientationType.West, 168.777954f, 5131.237793f, 200.000000f, 148.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 138.570435f, -1599.747314f, -57.446281f, ZoneLineOrientationType.West, 148.777954f, 5131.237793f, 200.000000f, 128.777954f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 118.570427f, -1599.747314f, -59.621250f, ZoneLineOrientationType.West, 128.777954f, 5131.237793f, 200.000000f, 108.777946f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 98.570427f, -1599.747314f, -61.796280f, ZoneLineOrientationType.West, 108.777946f, 5131.237793f, 200.000000f, 88.777946f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 78.570442f, -1599.747314f, -63.971249f, ZoneLineOrientationType.West, 88.777946f, 5131.237793f, 200.000000f, 68.777962f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 58.570438f, -1599.747314f, -66.141312f, ZoneLineOrientationType.West, 68.777962f, 5131.237793f, 200.000000f, 48.777962f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 38.570438f, -1599.747314f, -67.787682f, ZoneLineOrientationType.West, 48.777962f, 5131.237793f, 200.000000f, 28.777960f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", 18.570440f, -1599.747314f, -69.434021f, ZoneLineOrientationType.West, 28.777960f, 5131.237793f, 200.000000f, 8.777960f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1.429570f, -1599.747314f, -71.086342f, ZoneLineOrientationType.West, 8.777960f, 5131.237793f, 200.000000f, -11.222040f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -21.429569f, -1599.747314f, -72.738922f, ZoneLineOrientationType.West, -11.222040f, 5131.237793f, 200.000000f, -31.222040f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -41.429569f, -1599.747314f, -74.325768f, ZoneLineOrientationType.West, -31.222040f, 5131.237793f, 200.000000f, -51.222038f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -61.429569f, -1599.747314f, -72.675987f, ZoneLineOrientationType.West, -51.222038f, 5131.237793f, 200.000000f, -71.222038f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -81.429573f, -1599.747314f, -71.025978f, ZoneLineOrientationType.West, -71.222038f, 5131.237793f, 200.000000f, -91.222038f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -101.429573f, -1599.747314f, -69.375992f, ZoneLineOrientationType.West, -91.222038f, 5131.237793f, 200.000000f, -111.222038f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -121.429573f, -1599.747314f, -67.725998f, ZoneLineOrientationType.West, -111.222038f, 5131.237793f, 200.000000f, -131.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -141.429565f, -1599.747314f, -66.053642f, ZoneLineOrientationType.West, -131.222046f, 5131.237793f, 200.000000f, -151.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -161.429565f, -1599.747314f, -63.878689f, ZoneLineOrientationType.West, -151.222046f, 5131.237793f, 200.000000f, -171.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -181.429565f, -1599.747314f, -61.703491f, ZoneLineOrientationType.West, -171.222046f, 5131.237793f, 200.000000f, -191.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -201.235886f, -1594.215820f, -58.965080f, ZoneLineOrientationType.West, -191.222046f, 5131.237793f, 200.000000f, -211.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -221.429565f, -1596.641235f, -57.032490f, ZoneLineOrientationType.West, -211.222046f, 5131.237793f, 200.000000f, -231.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -241.429565f, -1596.641235f, -55.300110f, ZoneLineOrientationType.West, -231.222046f, 5131.237793f, 200.000000f, -251.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -261.429565f, -1596.641235f, -59.739422f, ZoneLineOrientationType.West, -251.222046f, 5131.237793f, 200.000000f, -271.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -281.429565f, -1596.641235f, -64.194572f, ZoneLineOrientationType.West, -271.222046f, 5131.237793f, 200.000000f, -291.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -301.429565f, -1596.641235f, -68.656754f, ZoneLineOrientationType.West, -291.222046f, 5131.237793f, 200.000000f, -311.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -321.429565f, -1596.641235f, -73.119377f, ZoneLineOrientationType.West, -311.222046f, 5131.237793f, 200.000000f, -331.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -341.429565f, -1596.641235f, -77.264351f, ZoneLineOrientationType.West, -331.222046f, 5131.237793f, 200.000000f, -351.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -361.429565f, -1596.641235f, -79.907959f, ZoneLineOrientationType.West, -351.222046f, 5131.237793f, 200.000000f, -371.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -381.429565f, -1596.641235f, -82.551781f, ZoneLineOrientationType.West, -371.222046f, 5131.237793f, 200.000000f, -391.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -401.429565f, -1596.641235f, -85.195572f, ZoneLineOrientationType.West, -391.222046f, 5131.237793f, 200.000000f, -411.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -421.429565f, -1596.641235f, -87.839287f, ZoneLineOrientationType.West, -411.222046f, 5131.237793f, 200.000000f, -431.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -441.429565f, -1596.641235f, -90.279900f, ZoneLineOrientationType.West, -431.222046f, 5131.237793f, 200.000000f, -451.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -461.429565f, -1596.641235f, -87.635872f, ZoneLineOrientationType.West, -451.222046f, 5131.237793f, 200.000000f, -471.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -481.429565f, -1596.641235f, -84.992378f, ZoneLineOrientationType.West, -471.222046f, 5131.237793f, 200.000000f, -491.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -501.429565f, -1596.641235f, -82.348679f, ZoneLineOrientationType.West, -491.222046f, 5131.237793f, 200.000000f, -511.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -521.429565f, -1596.641235f, -79.704811f, ZoneLineOrientationType.West, -511.222046f, 5131.237793f, 200.000000f, -531.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -541.429565f, -1596.641235f, -77.134331f, ZoneLineOrientationType.West, -531.222046f, 5131.237793f, 200.000000f, -551.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -561.429565f, -1596.641235f, -72.686951f, ZoneLineOrientationType.West, -551.222046f, 5131.237793f, 200.000000f, -571.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -581.429565f, -1596.641235f, -68.228188f, ZoneLineOrientationType.West, -571.222046f, 5131.237793f, 200.000000f, -591.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -601.429565f, -1596.641235f, -63.769482f, ZoneLineOrientationType.West, -591.222046f, 5131.237793f, 200.000000f, -611.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -621.429565f, -1596.641235f, -59.310661f, ZoneLineOrientationType.West, -611.222046f, 5131.237793f, 200.000000f, -631.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -641.429565f, -1596.641235f, -54.353611f, ZoneLineOrientationType.West, -631.222046f, 5131.237793f, 200.000000f, -651.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -661.429565f, -1596.641235f, -42.966110f, ZoneLineOrientationType.West, -651.222046f, 5131.237793f, 200.000000f, -671.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -681.429626f, -1596.641235f, -31.578560f, ZoneLineOrientationType.West, -671.222107f, 5131.237793f, 200.000000f, -691.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -701.429626f, -1596.641235f, -20.191050f, ZoneLineOrientationType.West, -691.222107f, 5131.237793f, 200.000000f, -711.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -721.429626f, -1596.641235f, -8.803610f, ZoneLineOrientationType.West, -711.222107f, 5131.237793f, 200.000000f, -731.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -741.429626f, -1596.641235f, 2.213060f, ZoneLineOrientationType.West, -731.222107f, 5131.237793f, 200.000000f, -751.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -761.429626f, -1596.641235f, 8.973560f, ZoneLineOrientationType.West, -751.222107f, 5131.237793f, 200.000000f, -771.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -781.429626f, -1596.641235f, 15.734280f, ZoneLineOrientationType.West, -771.222107f, 5131.237793f, 200.000000f, -791.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -801.429626f, -1596.641235f, 22.494841f, ZoneLineOrientationType.West, -791.222107f, 5131.237793f, 200.000000f, -811.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -821.429626f, -1596.641235f, 29.255489f, ZoneLineOrientationType.West, -811.222107f, 5131.237793f, 200.000000f, -831.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -841.429626f, -1596.641235f, 34.901020f, ZoneLineOrientationType.West, -831.222107f, 5131.237793f, 200.000000f, -851.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -861.429626f, -1596.641235f, 28.144600f, ZoneLineOrientationType.West, -851.222107f, 5131.237793f, 200.000000f, -871.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -881.429626f, -1596.641235f, 21.388330f, ZoneLineOrientationType.West, -871.222107f, 5131.237793f, 200.000000f, -891.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -901.429626f, -1596.641235f, 14.632090f, ZoneLineOrientationType.West, -891.222107f, 5131.237793f, 200.000000f, -911.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -921.429626f, -1596.641235f, 7.875830f, ZoneLineOrientationType.West, -911.222107f, 5131.237793f, 200.000000f, -931.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -941.429626f, -1596.641235f, 0.970300f, ZoneLineOrientationType.West, -931.222107f, 5131.237793f, 200.000000f, -951.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -961.429626f, -1596.641235f, -0.273860f, ZoneLineOrientationType.West, -951.222107f, 5131.237793f, 200.000000f, -971.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -981.429626f, -1596.641235f, -1.517720f, ZoneLineOrientationType.West, -971.222107f, 5131.237793f, 200.000000f, -991.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1001.429626f, -1596.641235f, -2.761340f, ZoneLineOrientationType.West, -991.222107f, 5131.237793f, 200.000000f, -1011.222107f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1021.429626f, -1596.641235f, -4.005160f, ZoneLineOrientationType.West, -1011.222107f, 5131.237793f, 200.000000f, -1031.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1041.429565f, -1596.641235f, -4.855840f, ZoneLineOrientationType.West, -1031.222046f, 5131.237793f, 200.000000f, -1051.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1061.429565f, -1596.641235f, 2.797780f, ZoneLineOrientationType.West, -1051.222046f, 5131.237793f, 200.000000f, -1071.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1081.429565f, -1596.641235f, 10.450930f, ZoneLineOrientationType.West, -1071.222046f, 5131.237793f, 200.000000f, -1091.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1101.429565f, -1596.641235f, 18.104589f, ZoneLineOrientationType.West, -1091.222046f, 5131.237793f, 200.000000f, -1111.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1121.429565f, -1596.641235f, 25.758080f, ZoneLineOrientationType.West, -1111.222046f, 5131.237793f, 200.000000f, -1131.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("commons", -1141.061279f, -1596.579956f, 33.031120f, ZoneLineOrientationType.West, -1131.222046f, 5131.237793f, 200.000000f, -1151.222046f, 5101.237793f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 577.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 587.000000f, -1600.000000f, 200.000000f, 567.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 557.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 567.000000f, -1600.000000f, 200.000000f, 547.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 537.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 547.000000f, -1600.000000f, 200.000000f, 527.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 517.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 527.000000f, -1600.000000f, 200.000000f, 507.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 497.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 507.000000f, -1600.000000f, 200.000000f, 487.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 477.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 487.000000f, -1600.000000f, 200.000000f, 467.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 457.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 467.000000f, -1600.000000f, 200.000000f, 447.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 437.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 447.000000f, -1600.000000f, 200.000000f, 427.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 417.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 427.000000f, -1600.000000f, 200.000000f, 407.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 397.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 407.000000f, -1600.000000f, 200.000000f, 387.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 377.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 387.000000f, -1600.000000f, 200.000000f, 367.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 357.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 367.000000f, -1600.000000f, 200.000000f, 347.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 337.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 347.000000f, -1600.000000f, 200.000000f, 327.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 317.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 327.000000f, -1600.000000f, 200.000000f, 307.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 297.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 307.000000f, -1600.000000f, 200.000000f, 287.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 277.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 287.000000f, -1600.000000f, 200.000000f, 267.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 257.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 267.000000f, -1600.000000f, 200.000000f, 247.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 237.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 247.000000f, -1600.000000f, 200.000000f, 227.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 217.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 227.000000f, -1600.000000f, 200.000000f, 207.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 197.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 207.000000f, -1600.000000f, 200.000000f, 187.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 177.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 187.000000f, -1600.000000f, 200.000000f, 167.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 157.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 167.000000f, -1600.000000f, 200.000000f, 147.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 137.772156f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 147.000000f, -1600.000000f, 200.000000f, 127.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 117.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 127.000000f, -1600.000000f, 200.000000f, 107.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 97.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 107.000000f, -1600.000000f, 200.000000f, 87.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 77.772163f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 87.000000f, -1600.000000f, 200.000000f, 67.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 57.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 67.000000f, -1600.000000f, 200.000000f, 47.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 37.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 47.000000f, -1600.000000f, 200.000000f, 27.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", 17.772160f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 27.000000f, -1600.000000f, 200.000000f, 7.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -2.227840f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, 7.000000f, -1600.000000f, 200.000000f, -13.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -22.227850f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -13.000000f, -1600.000000f, 200.000000f, -33.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -42.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -33.000000f, -1600.000000f, 200.000000f, -53.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -62.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -53.000000f, -1600.000000f, 200.000000f, -73.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -82.227852f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -73.000000f, -1600.000000f, 200.000000f, -93.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -102.227837f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -93.000000f, -1600.000000f, 200.000000f, -113.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -122.227837f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -113.000000f, -1600.000000f, 200.000000f, -133.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -142.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -133.000000f, -1600.000000f, 200.000000f, -153.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -162.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -153.000000f, -1600.000000f, 200.000000f, -173.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -182.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -173.000000f, -1600.000000f, 200.000000f, -193.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -202.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -193.000000f, -1600.000000f, 200.000000f, -213.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -222.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -213.000000f, -1600.000000f, 200.000000f, -233.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -242.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -233.000000f, -1600.000000f, 200.000000f, -253.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -262.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -253.000000f, -1600.000000f, 200.000000f, -273.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -282.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -273.000000f, -1600.000000f, 200.000000f, -293.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -302.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -293.000000f, -1600.000000f, 200.000000f, -313.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -322.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -313.000000f, -1600.000000f, 200.000000f, -333.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -342.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -333.000000f, -1600.000000f, 200.000000f, -353.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -362.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -353.000000f, -1600.000000f, 200.000000f, -373.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -382.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -373.000000f, -1600.000000f, 200.000000f, -393.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -402.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -393.000000f, -1600.000000f, 200.000000f, -413.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -422.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -413.000000f, -1600.000000f, 200.000000f, -433.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -442.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -433.000000f, -1600.000000f, 200.000000f, -453.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -462.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -453.000000f, -1600.000000f, 200.000000f, -473.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -482.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -473.000000f, -1600.000000f, 200.000000f, -493.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -502.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -493.000000f, -1600.000000f, 200.000000f, -513.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -522.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -513.000000f, -1600.000000f, 200.000000f, -533.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -542.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -533.000000f, -1600.000000f, 200.000000f, -553.000000f, -1630.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -562.227844f, 791.873230f, -27.999950f, ZoneLineOrientationType.East, -553.000000f, -1600.000000f, 200.000000f, -593.000000f, -1630.000000f, -100.000000f);
                    }
                    break;                
                case "erudnext":
                    {
                        zoneProperties.SetBaseZoneProperties("erudnext", "Erudin", -309.75f, 109.64f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 550);
                        zoneProperties.AddZoneLineBox("tox", 2543.662842f, 297.415588f, -48.407711f, ZoneLineOrientationType.South, -1559.726196f, -175.747467f, -17.531000f, -1584.182617f, -211.414566f, -48.468529f);
                        zoneProperties.AddTeleportPad("erudnext", -1410.466431f, -184.863327f, 34.000938f, ZoneLineOrientationType.North, -1392.625977f, -254.981995f, -42.968651f, 6.0f);
                        zoneProperties.AddTeleportPad("erudnext", -1410.336670f, -310.649994f, -45.968410f, ZoneLineOrientationType.South, -1410.323730f, -310.856049f, 37.000172f, 6.0f);
                        zoneProperties.AddTeleportPad("erudnint", 711.753357f, 805.382568f, 18.000059f, ZoneLineOrientationType.East, -644.927917f, -183.935837f, 75.968788f, 6.0f);
                        zoneProperties.AddTeleportPad("erudnext", -396.419495f, -8.040450f, 38.000011f, ZoneLineOrientationType.North, -644.701294f, -278.909973f, 68.968857f, 6.0f);
                        zoneProperties.AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.673065f, -1.365350f, 37.998589f, 11f);
                        zoneProperties.AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.673065f, -8.182305f, 37.998589f, 11f);
                        zoneProperties.AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, -327.821869f, -14.999260f, 37.998589f, 11f);
                    }
                    break;
                case "erudnint":
                    {
                        zoneProperties.SetBaseZoneProperties("erudnint", "Erudin Palace", 807f, 712f, 22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                        zoneProperties.AddTeleportPad("erudnext", -773.795898f, -183.949753f, 50.968781f, ZoneLineOrientationType.North, 711.744934f, 806.283813f, 5.000060f, 12.0f);
                    }
                    break;
                case "erudsxing":
                    {
                        // TODO: There's a boat that connects to erudnext and qeynos (south)
                        zoneProperties.SetBaseZoneProperties("erudsxing", "Erud's Crossing", 795f, -1766.9f, 12.36f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "everfrost":
                    {
                        zoneProperties.SetBaseZoneProperties("everfrost", "Everfrost", 682.74f, 3139.01f, -60.16f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 500);
                        zoneProperties.AddZoneLineBox("blackburrow", 64.26508f, -340.1918f, 0.00073920796f, ZoneLineOrientationType.South, -3054.6953f, -515.55963f, -99.7185f, -3094.8235f, -547f, -113.68753f);
                        zoneProperties.AddZoneLineBox("halas", -647.768616f, -75.159027f, 0.000020f, ZoneLineOrientationType.North, 3756.428467f, 397.611786f, 38.469002f, 3706.500488f, 347.150665f, -0.499760f);
                        zoneProperties.AddZoneLineBox("permafrost", -61.690048f, 84.215889f, 0.000010f, ZoneLineOrientationType.East, 2040.192261f, -7055.080078f, -8.999750f, 1989.364502f, -7120.806641f, -64.344040f);
                    }
                    break;
                case "fearplane":
                    {
                        zoneProperties.SetBaseZoneProperties("fearplane", "Plane of Fear", 1282.09f, -1139.03f, 1.67f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 1000);
                        zoneProperties.AddZoneLineBox("feerrott", -2347.395752f, 2604.589111f, 10.280410f, ZoneLineOrientationType.North, -790.410828f, 1052.103638f, 150.821121f, -803.796631f, 1015.684509f, 105.875198f);
                    }
                    break;
                case "feerrott":
                    {
                        zoneProperties.SetBaseZoneProperties("feerrott", "The Feerrott", 902.6f, 1091.7f, 28f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(60, 90, 30, 10, 175);
                        zoneProperties.AddZoneLineBox("cazicthule", 55.471420f, -67.975937f, 0.000000f, ZoneLineOrientationType.North, -1469.255859f, -100.275429f, 58.405380f, -1499.662231f, -120.661491f, 47.437580f);
                        zoneProperties.AddZoneLineBox("oggok", -373.311127f, -102.846184f, -0.000000f, ZoneLineOrientationType.North, 1700.901245f, 832.210693f, 110.609047f, 1669.091797f, 786.900452f, 56.781330f);
                        zoneProperties.AddZoneLineBox("innothule", -1120.934570f, 1876.716309f, -12.343200f, ZoneLineOrientationType.East, -1053.738770f, -3064.860107f, 34.236019f, -1118.701904f, -3134.157959f, -12.843610f);
                        zoneProperties.AddZoneLineBox("rathemtn", 654.660095f, -3116.889893f, 0.000320f, ZoneLineOrientationType.North, 391.610870f, 3485.147949f, 64.094902f, 348.915161f, 3365.229736f, -0.499940f);
                        zoneProperties.AddZoneLineBox("fearplane", -817.476501f, 1032.365723f, 102.129517f, ZoneLineOrientationType.South, -2374.944580f, 2635.523193f, 98.296158f, -2399.710449f, 2569.650391f, 18.406269f);
                    }
                    break;
                case "felwithea":
                    {
                        zoneProperties.SetBaseZoneProperties("felwithea", "Northern Felwithe", 94f, -25f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                        zoneProperties.AddZoneLineBox("felwitheb", 251.268646f, -832.815125f, -13.999020f, ZoneLineOrientationType.North, 364.650452f, -711.921509f, -1.531000f, 342.316345f, -727.911865f, -14.499750f);
                        zoneProperties.AddZoneLineBox("gfaydark", -1931.678101f, -2613.879639f, 20.406450f, ZoneLineOrientationType.West, 56.161152f, 242.410782f, 26.469000f, 27.806530f, 193.596893f, -0.500000f);
                    }
                    break;
                case "felwitheb":
                    {
                        // Bug: Hole in the doorway into the teleport room (bottom).  Might be scale related
                        zoneProperties.SetBaseZoneProperties("felwitheb", "Southern Felwithe", -790f, 320f, -10.25f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                        zoneProperties.AddZoneLineBox("felwithea", 336.521210f, -720.996582f, -13.999750f, ZoneLineOrientationType.South, 245.892227f, -825.463867f, -1.531000f, 218.101257f, -839.849731f, -14.500020f);
                        zoneProperties.AddTeleportPad("felwitheb", 503.797028f, -496.463074f, -5.001940f, ZoneLineOrientationType.West, 435.755615f, -584.819824f, 31.000059f, 6.0f);
                        zoneProperties.AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 503.520294f, -338.805695f, 3.999970f, 6.0f);
                        zoneProperties.AddTeleportPad("felwitheb", 603.829346f, -580.765015f, 0.000040f, ZoneLineOrientationType.North, 458.797363f, -601.881165f, 31.000561f, 6.0f);
                        zoneProperties.AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 745.727112f, -532.876404f, 4.000100f, 6.0f);
                        zoneProperties.AddTeleportPad("felwitheb", 552.740112f, -743.948242f, 0.000020f, ZoneLineOrientationType.East, 435.665283f, -618.718323f, 31.000561f, 6.0f);
                        zoneProperties.AddTeleportPad("felwitheb", 303.943054f, -581.065125f, -13.999980f, ZoneLineOrientationType.North, 553.732544f, -919.604370f, 4.000090f, 6.0f);
                    }
                    break;
                case "freporte":
                    {
                        // TODO: There is a boat that goes to ocean of tears (OOT)
                        zoneProperties.SetBaseZoneProperties("freporte", "East Freeport", -648f, -1097f, -52.2f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 905.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -98.602051f, 200.000000f, -1366.303711f, -138.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 885.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -138.602051f, 200.000000f, -1366.303711f, -158.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 865.000000f, -28.031231f, ZoneLineOrientationType.South, -1336.303711f, -158.602051f, 200.000000f, -1366.303711f, -178.602066f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 845.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -178.602066f, 200.000000f, -1366.303711f, -198.602066f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 825.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -198.602066f, 200.000000f, -1366.303711f, -218.602066f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 805.000000f, -28.031151f, ZoneLineOrientationType.South, -1336.303711f, -218.602066f, 200.000000f, -1366.303711f, -238.602066f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 785.000000f, -28.031059f, ZoneLineOrientationType.South, -1336.303711f, -238.602066f, 200.000000f, -1366.303711f, -258.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 765.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -258.602051f, 200.000000f, -1366.303711f, -278.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 745.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -278.602051f, 200.000000f, -1366.303711f, -298.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 725.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -298.602051f, 200.000000f, -1366.303711f, -318.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 705.000000f, -28.030741f, ZoneLineOrientationType.South, -1336.303711f, -318.602051f, 200.000000f, -1366.303711f, -338.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 685.000000f, -28.030991f, ZoneLineOrientationType.South, -1336.303711f, -338.602051f, 200.000000f, -1366.303711f, -358.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 665.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -358.602051f, 200.000000f, -1366.303711f, -378.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 645.000000f, -28.031090f, ZoneLineOrientationType.South, -1336.303711f, -378.602051f, 200.000000f, -1366.303711f, -398.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 625.000000f, -28.031290f, ZoneLineOrientationType.South, -1336.303711f, -398.602051f, 200.000000f, -1366.303711f, -418.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 605.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -418.602051f, 200.000000f, -1366.303711f, -438.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 585.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -438.602051f, 200.000000f, -1366.303711f, -458.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 565.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -458.602051f, 200.000000f, -1366.303711f, -478.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 545.000000f, -28.031281f, ZoneLineOrientationType.South, -1336.303711f, -478.602051f, 200.000000f, -1366.303711f, -498.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 525.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -498.602051f, 200.000000f, -1366.303711f, -518.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 505.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -518.602051f, 200.000000f, -1366.303711f, -538.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 485.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -538.602051f, 200.000000f, -1366.303711f, -558.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 465.000000f, -28.031019f, ZoneLineOrientationType.South, -1336.303711f, -558.602051f, 200.000000f, -1366.303711f, -578.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 445.000000f, -28.030939f, ZoneLineOrientationType.South, -1336.303711f, -578.602051f, 200.000000f, -1366.303711f, -598.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 425.000000f, -28.031099f, ZoneLineOrientationType.South, -1336.303711f, -598.602051f, 200.000000f, -1366.303711f, -618.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 405.000000f, -28.031139f, ZoneLineOrientationType.South, -1336.303711f, -618.602051f, 200.000000f, -1366.303711f, -638.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 385.000000f, -28.030809f, ZoneLineOrientationType.South, -1336.303711f, -638.602051f, 200.000000f, -1366.303711f, -658.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 365.000000f, -28.030390f, ZoneLineOrientationType.South, -1336.303711f, -658.602051f, 200.000000f, -1366.303711f, -678.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 345.000000f, -28.031111f, ZoneLineOrientationType.South, -1336.303711f, -678.602051f, 200.000000f, -1366.303711f, -698.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 325.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -698.602051f, 200.000000f, -1366.303711f, -718.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 305.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -718.602051f, 200.000000f, -1366.303711f, -738.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 285.000000f, -28.030560f, ZoneLineOrientationType.South, -1336.303711f, -738.602051f, 200.000000f, -1366.303711f, -758.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 265.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -758.602051f, 200.000000f, -1366.303711f, -778.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 245.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -778.602051f, 200.000000f, -1366.303711f, -798.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 225.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -798.602051f, 200.000000f, -1366.303711f, -818.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 205.000000f, -28.031170f, ZoneLineOrientationType.South, -1336.303711f, -818.602051f, 200.000000f, -1366.303711f, -838.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 185.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -838.602051f, 200.000000f, -1366.303711f, -858.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 165.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -858.602051f, 200.000000f, -1366.303711f, -878.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 145.000000f, -28.031010f, ZoneLineOrientationType.South, -1336.303711f, -878.602051f, 200.000000f, -1366.303711f, -898.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 125.000000f, -28.031120f, ZoneLineOrientationType.South, -1336.303711f, -898.602051f, 200.000000f, -1366.303711f, -918.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 105.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -918.602051f, 200.000000f, -1366.303711f, -938.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 85.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -938.602051f, 200.000000f, -1366.303711f, -958.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 65.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -958.602051f, 200.000000f, -1366.303711f, -978.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 45.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -978.602051f, 200.000000f, -1366.303711f, -998.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 25.000000f, -28.031071f, ZoneLineOrientationType.South, -1336.303711f, -998.602051f, 200.000000f, -1366.303711f, -1018.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, 5.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1018.602051f, 200.000000f, -1366.303711f, -1038.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -15.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1038.602051f, 200.000000f, -1366.303711f, -1058.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -35.000000f, -28.031040f, ZoneLineOrientationType.South, -1336.303711f, -1058.602051f, 200.000000f, -1366.303711f, -1078.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -55.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1078.602051f, 200.000000f, -1366.303711f, -1098.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -75.000000f, -28.031031f, ZoneLineOrientationType.South, -1336.303711f, -1098.602051f, 200.000000f, -1366.303711f, -1118.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -95.000000f, -28.031179f, ZoneLineOrientationType.South, -1336.303711f, -1118.602051f, 200.000000f, -1366.303711f, -1138.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -115.000000f, -28.031050f, ZoneLineOrientationType.South, -1336.303711f, -1138.602051f, 200.000000f, -1366.303711f, -1158.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -135.000000f, -28.031250f, ZoneLineOrientationType.South, -1336.303711f, -1158.602051f, 200.000000f, -1366.303711f, -1178.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -155.000000f, -28.031130f, ZoneLineOrientationType.South, -1336.303711f, -1178.602051f, 200.000000f, -1366.303711f, -1198.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -175.000000f, -28.031000f, ZoneLineOrientationType.South, -1336.303711f, -1198.602051f, 200.000000f, -1366.303711f, -1218.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -195.000000f, -28.031219f, ZoneLineOrientationType.South, -1336.303711f, -1218.602051f, 200.000000f, -1366.303711f, -1238.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -215.000000f, -28.030870f, ZoneLineOrientationType.South, -1336.303711f, -1238.602051f, 200.000000f, -1366.303711f, -1258.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("nro", 4152.241699f, -230.000000f, -28.031080f, ZoneLineOrientationType.South, -1336.303711f, -1258.602051f, 200.000000f, -1366.303711f, -1298.602051f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freportw", -82.741112f, -951.859192f, -27.999960f, ZoneLineOrientationType.East, 462.006012f, -343.977173f, -0.311040f, 433.619080f, -420.036346f, -28.500010f);
                        zoneProperties.AddZoneLineBox("freportw", -392.460449f, -622.734680f, -28.000040f, ZoneLineOrientationType.North, 154.989761f, -55.088539f, 0.501000f, 93.445801f, -70.162163f, -28.499990f);
                        zoneProperties.AddZoneLineBox("freportw", -740.355530f, -1630.233276f, -97.968758f, ZoneLineOrientationType.South, -164.879898f, 350.068451f, -85.501068f, -196.100616f, 335.683228f, -98.468712f);
                    }
                    break;
                case "freportn":
                    {
                        zoneProperties.SetBaseZoneProperties("freportn", "North Freeport", 211f, -296f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.AddZoneLineBox("freportw", 1588.414673f, -278.419495f, 0.000050f, ZoneLineOrientationType.East, 378.034851f, 718.198425f, -1.531000f, 361.772491f, 697.030884f, -14.499990f);
                        zoneProperties.AddZoneLineBox("freportw", 728.335388f, -581.244812f, -20.999701f, ZoneLineOrientationType.South, -15.071440f, -433.618988f, -11.531000f, -34.966301f, -454.098175f, -50f);
                        zoneProperties.AddZoneLineBox("freportw", 211.309326f, -124.670799f, -14.000000f, ZoneLineOrientationType.South, -429.537323f, 504.799438f, 14.500150f, -490.004974f, 475.620117f, -14.500010f);
                        zoneProperties.AddZoneLineBox("freportw", 252.782593f, -698.531494f, -27.999969f, ZoneLineOrientationType.South, -378.454254f, -67.828621f, 0.500040f, -448.004974f, -98.161171f, -28.499950f);
                    }
                    break;
                case "freportw":
                    {
                        zoneProperties.SetBaseZoneProperties("freportw", "West Freeport", 181f, 335f, -24f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.AddZoneLineBox("ecommons", 577.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 587.772156f, 841.873230f, 200.000000f, 567.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 557.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 567.772156f, 841.873230f, 200.000000f, 547.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 537.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 547.772156f, 841.873230f, 200.000000f, 527.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 517.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 527.772156f, 841.873230f, 200.000000f, 507.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 497.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, 507.772156f, 841.873230f, 200.000000f, 487.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 477.000000f, -1580.000000f, -54.468620f, ZoneLineOrientationType.West, 487.772156f, 841.873230f, 200.000000f, 467.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 457.000000f, -1580.000000f, -54.468651f, ZoneLineOrientationType.West, 467.772156f, 841.873230f, 200.000000f, 447.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 437.000000f, -1580.000000f, -54.468689f, ZoneLineOrientationType.West, 447.772156f, 841.873230f, 200.000000f, 427.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 417.000000f, -1580.000000f, -54.467621f, ZoneLineOrientationType.West, 427.772156f, 841.873230f, 200.000000f, 407.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 397.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, 407.772156f, 841.873230f, 200.000000f, 387.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 377.000000f, -1580.000000f, -54.468578f, ZoneLineOrientationType.West, 387.772156f, 841.873230f, 200.000000f, 367.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 357.000000f, -1580.000000f, -52.021858f, ZoneLineOrientationType.West, 367.772156f, 841.873230f, 200.000000f, 347.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 337.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 347.772156f, 841.873230f, 200.000000f, 327.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 317.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 327.772156f, 841.873230f, 200.000000f, 307.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 297.000000f, -1580.000000f, -54.467548f, ZoneLineOrientationType.West, 307.772156f, 841.873230f, 200.000000f, 287.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 277.000000f, -1580.000000f, -54.468739f, ZoneLineOrientationType.West, 287.772156f, 841.873230f, 200.000000f, 267.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 257.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, 267.772156f, 841.873230f, 200.000000f, 247.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 237.000000f, -1580.000000f, -54.468578f, ZoneLineOrientationType.West, 247.772156f, 841.873230f, 200.000000f, 227.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 217.000000f, -1580.000000f, -54.467510f, ZoneLineOrientationType.West, 227.772156f, 841.873230f, 200.000000f, 207.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 197.000000f, -1580.000000f, -54.468491f, ZoneLineOrientationType.West, 207.772156f, 841.873230f, 200.000000f, 187.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 177.000000f, -1580.000000f, -54.468739f, ZoneLineOrientationType.West, 187.772156f, 841.873230f, 200.000000f, 167.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 157.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 167.772156f, 841.873230f, 200.000000f, 147.772156f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 137.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, 147.772156f, 841.873230f, 200.000000f, 127.772163f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 117.000000f, -1580.000000f, -54.468521f, ZoneLineOrientationType.West, 127.772163f, 841.873230f, 200.000000f, 107.772163f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 97.000000f, -1580.000000f, -54.467442f, ZoneLineOrientationType.West, 107.772163f, 841.873230f, 200.000000f, 87.772163f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 77.000000f, -1580.000000f, -54.468540f, ZoneLineOrientationType.West, 87.772163f, 841.873230f, 200.000000f, 67.772163f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 57.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 67.772163f, 841.873230f, 200.000000f, 47.772160f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 37.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, 47.772160f, 841.873230f, 200.000000f, 27.772160f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", 17.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, 27.772160f, 841.873230f, 200.000000f, 7.772160f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -3.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, 7.772160f, 841.873230f, 200.000000f, -12.227850f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -23.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -12.227840f, 841.873230f, 200.000000f, -32.227852f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -43.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -32.227852f, 841.873230f, 200.000000f, -52.227852f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -63.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -52.227852f, 841.873230f, 200.000000f, -72.227852f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -83.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -72.227852f, 841.873230f, 200.000000f, -92.227837f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -103.000000f, -1580.000000f, -54.468651f, ZoneLineOrientationType.West, -92.227837f, 841.873230f, 200.000000f, -112.227837f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -123.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -112.227837f, 841.873230f, 200.000000f, -132.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -143.000000f, -1580.000000f, -54.468540f, ZoneLineOrientationType.West, -132.227844f, 841.873230f, 200.000000f, -152.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -163.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -152.227844f, 841.873230f, 200.000000f, -172.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -183.000000f, -1580.000000f, -54.468410f, ZoneLineOrientationType.West, -172.227844f, 841.873230f, 200.000000f, -192.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -203.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -192.227844f, 841.873230f, 200.000000f, -212.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -223.000000f, -1580.000000f, -54.468441f, ZoneLineOrientationType.West, -212.227844f, 841.873230f, 200.000000f, -232.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -243.000000f, -1580.000000f, -54.468639f, ZoneLineOrientationType.West, -232.227844f, 841.873230f, 200.000000f, -252.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -263.000000f, -1580.000000f, -54.467571f, ZoneLineOrientationType.West, -252.227844f, 841.873230f, 200.000000f, -272.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -283.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -272.227844f, 841.873230f, 200.000000f, -292.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -303.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -292.227844f, 841.873230f, 200.000000f, -312.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -323.000000f, -1580.000000f, -54.468700f, ZoneLineOrientationType.West, -312.227844f, 841.873230f, 200.000000f, -332.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -343.000000f, -1580.000000f, -54.468681f, ZoneLineOrientationType.West, -332.227844f, 841.873230f, 200.000000f, -352.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -363.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -352.227844f, 841.873230f, 200.000000f, -372.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -383.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -372.227844f, 841.873230f, 200.000000f, -392.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -403.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -392.227844f, 841.873230f, 200.000000f, -412.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -423.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -412.227844f, 841.873230f, 200.000000f, -432.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -443.000000f, -1580.000000f, -54.468658f, ZoneLineOrientationType.West, -432.227844f, 841.873230f, 200.000000f, -452.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -463.000000f, -1580.000000f, -54.468719f, ZoneLineOrientationType.West, -452.227844f, 841.873230f, 200.000000f, -472.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -483.000000f, -1580.000000f, -54.467651f, ZoneLineOrientationType.West, -472.227844f, 841.873230f, 200.000000f, -492.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -503.000000f, -1580.000000f, -54.468670f, ZoneLineOrientationType.West, -492.227844f, 841.873230f, 200.000000f, -512.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -523.000000f, -1580.000000f, -54.468712f, ZoneLineOrientationType.West, -512.227844f, 841.873230f, 200.000000f, -532.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -543.000000f, -1580.000000f, -54.468632f, ZoneLineOrientationType.West, -532.227844f, 841.873230f, 200.000000f, -552.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -563.000000f, -1580.000000f, -54.468601f, ZoneLineOrientationType.West, -552.227844f, 841.873230f, 200.000000f, -592.227844f, 811.873230f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", 447.153564f, -338.147949f, -27.999809f, ZoneLineOrientationType.West, -68.449120f, -867.598389f, 0.500060f, -98.163116f, -942.147095f, -28.499960f);
                        zoneProperties.AddZoneLineBox("freporte", 84.504356f, -62.710468f, -27.999990f, ZoneLineOrientationType.South, -401.208527f, -615.295532f, 0.940550f, -462.005890f, -629.942322f, -28.499929f);
                        zoneProperties.AddZoneLineBox("freporte", -154.759933f, 342.932068f, -97.968491f, ZoneLineOrientationType.North, -699.512451f, -1623.163940f, -85.500092f, -734.874329f, -1637.536377f, -98.468758f);
                        zoneProperties.AddZoneLineBox("freportn", 370.832550f, 726.575989f, -13.999940f, ZoneLineOrientationType.West, 1597.559326f, -249.103378f, 12.469000f, 1581.182617f, -270.030487f, -0.499950f);
                        zoneProperties.AddZoneLineBox("freportn", -2.907860f, -440.593567f, -20.999920f, ZoneLineOrientationType.North, 758.867493f, -571.213928f, -12.532780f, 742.214172f, -587.942444f, -50f);
                        zoneProperties.AddZoneLineBox("freportn", -408.099854f, 489.939026f, -13.999160f, ZoneLineOrientationType.North, 265.374237f, -110.341187f, 14.500020f, 221.547180f, -140.130920f, -14.500000f);
                        zoneProperties.AddZoneLineBox("freportn", -366.081055f, -82.489418f, -28.000010f, ZoneLineOrientationType.North, 307.515747f, -684.160217f, 0.500130f, 265.184326f, -713.913147f, -28.499969f);
                        zoneProperties.AddTeleportPad("freportw", 146.800308f, -681.771179f, -12.999480f, ZoneLineOrientationType.East, 97.993584f, -657.753784f, -40.968651f, 7.7f);
                        zoneProperties.AddTeleportPad("freportw", 12.084580f, -655.863647f, -54.968719f, ZoneLineOrientationType.North, 157.920013f, -715.959045f, -12.000000f, 7.7f);
                        zoneProperties.AddDisabledMaterialCollisionByNames("t50_w1", "t25_m0004");
                    }
                    break;
                case "gfaydark": // One More Lift
                    {
                        // TODO: Lifts for Kelethin
                        zoneProperties.SetBaseZoneProperties("gfaydark", "Greater Faydark", 10f, -20f, 0f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(0, 128, 64, 10, 300);
                        zoneProperties.AddZoneLineBox("butcher", -1164.1454f, -3082.1367f, 0.00028176606f, ZoneLineOrientationType.North, -1636.052856f, 2614.448242f, 80.942001f, -1604.046753f, 2657.645264f, -0.499690f);
                        zoneProperties.AddZoneLineBox("crushbone", -625.626038f, 163.201843f, 0.000070f, ZoneLineOrientationType.North, 2670.067139f, -28.324280f, 56.295769f, 2579.850830f, -75.045639f, 15.343880f);
                        zoneProperties.AddZoneLineBox("felwithea", 41.148460f, 183.167984f, 0.000000f, ZoneLineOrientationType.East, -1917.227173f, -2623.463623f, 46.844002f, -1945.600464f, -2663.089355f, 19.906750f);
                        zoneProperties.AddZoneLineBox("lfaydark", 2164.083984f, -1199.626953f, 0.000040f, ZoneLineOrientationType.South, -2623.411133f, -1084.083862f, 114.320740f, -2650.334229f, -1130.060669f, -0.499900f);
                        
                        // These three teleports are temp until the lifts are put in
                        zoneProperties.AddTeleportPad("gfaydark", 946.531250f, 222.323135f, 73.968826f, ZoneLineOrientationType.South, 988.554749f, 220.978745f, -24.697590f, 10.0f);
                        zoneProperties.AddTeleportPad("gfaydark", 138.929764f, 275.177704f, 73.969337f, ZoneLineOrientationType.West, 136.997269f, 234.183487f, 5.157810f, 10.0f);
                        zoneProperties.AddTeleportPad("gfaydark", -16.362190f, -136.277435f, 73.968750f, ZoneLineOrientationType.South, 26.329359f, -138.125824f, 5.380220f, 10.0f);
                        ///
                    }
                    break;               
                case "grobb":
                    {
                        zoneProperties.SetBaseZoneProperties("grobb", "Grobb", 0f, -100f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("innothule", -2795.355469f, -654.658081f, -34.562538f, ZoneLineOrientationType.East, -169.745117f, 26.887341f, 28.469000f, -192.243027f, 9.193430f, -0.499990f);
                    }
                    break;                
                case "gukbottom":
                    {
                        // TODO: Ladders
                        zoneProperties.SetBaseZoneProperties("gukbottom", "Ruins of Old Guk", -217f, 1197f, -81.78f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 45, 20, 10, 140);
                        zoneProperties.AddZoneLineBox("guktop", 1113.605835f, 617.183350f, -88.333542f, ZoneLineOrientationType.East, 1161.719360f, 662.774170f, -81.499748f, 1143.830933f, 656.943542f, -110f);
                        zoneProperties.AddZoneLineBox("innothule", 144.032776f, -821.548645f, -11.500000f, ZoneLineOrientationType.West, -123.419243f, 84.161140f, -225.437256f, -140f, 69.775558f, -238.406235f);
                        zoneProperties.AddZoneLineBox("guktop", 1620.083008f, 181.952133f, -88.660629f, ZoneLineOrientationType.West, 1675.066772f, -37.624660f, -70f, 1648.329590f, -92.907097f, -138.851685f);
                        zoneProperties.AddZoneLineBox("guktop", 1555.745972f, -121.623947f, -91.073799f, ZoneLineOrientationType.West, 1506.506348f, 73.868462f, -80f, 1485.213745f, 14.151250f, -105f);
                        zoneProperties.AddZoneLineBox("guktop", 1196.247681f, -197.502167f, -83.967888f, ZoneLineOrientationType.West, 1203.723999f, -181.743942f, -71.499748f, 1189.337769f, -204.274963f, -84.468781f);
                    }
                    break;
                case "guktop":
                    {
                        // TODO: Ladders
                        zoneProperties.SetBaseZoneProperties("guktop", "Guk", 7f, -36f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(40, 45, 20, 10, 140);
                        zoneProperties.AddZoneLineBox("gukbottom", 1154.039917f, 670.316589f, -93.968727f, ZoneLineOrientationType.West, 1122.238281f, 644.556519f, -77.740372f, 1105.369995f, 629.647583f, -95.468483f);
                        zoneProperties.AddZoneLineBox("gukbottom", 1665.729126f, -107.982651f, -102.307808f, ZoneLineOrientationType.East, 1623.884277f, 142.214523f, -60f, 1563.454590f, 117.747520f, -110f);
                        zoneProperties.AddZoneLineBox("gukbottom", 1493.752930f, -1.347960f, -91.878059f, ZoneLineOrientationType.East, 1575f, -205.689896f, -80f, 1550.644043f, -155.016663f, -115f);
                        zoneProperties.AddZoneLineBox("gukbottom", 1195.318848f, -209.319427f, -83.968781f, ZoneLineOrientationType.East, 1203.724121f, -205.721344f, -71.500748f, 1185.029785f, -210.714722f, -84.468697f);
                        zoneProperties.AddZoneLineBox("innothule", 144.032776f, -821.548645f, -11.500000f, ZoneLineOrientationType.West, -53.083141f, 56.776360f, 12.469000f, -70.161201f, 49.388599f, -0.499990f);
                    }
                    break;
                case "halas":
                    {
                        zoneProperties.SetBaseZoneProperties("halas", "Halas", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 300);
                        zoneProperties.AddZoneLineBox("everfrost", 3682.792725f, 372.904633f, 0.000240f, ZoneLineOrientationType.South, -664.463196f, -50.776440f, 37.469002f, -744.483093f, -101.162247f, -0.499990f);
                    }
                    break;
                case "hateplane":
                    {
                        // TODO: Need to identify a new zone in / zone out for this zone
                        zoneProperties.SetBaseZoneProperties("hateplane", "Plane of Hate", -353.08f, -374.8f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(128, 128, 128, 30, 200);
                    }
                    break;
                case "highkeep":
                    {
                        zoneProperties.SetBaseZoneProperties("highkeep", "High Keep", 88f, -16f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                        zoneProperties.AddZoneLineBox("highpass", 62.824429f, -112.595383f, 0.000000f, ZoneLineOrientationType.West, 70.162773f, 126.130470f, 12.469000f, 55.775291f, 104.252892f, -0.499970f);
                        zoneProperties.AddZoneLineBox("highpass", -90.567039f, -112.659950f, -0.000010f, ZoneLineOrientationType.West, -82.355392f, 112.775299f, 12.469000f, -98.161209f, 104.758774f, -0.500000f);
                    }
                    break;
                case "highpass":
                    {
                        zoneProperties.SetBaseZoneProperties("highpass", "Highpass Hold", -104f, -14f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 200, 10, 400);
                        zoneProperties.AddZoneLineBox("eastkarana", -3069.264893f, -8291.038086f, 689.907410f, ZoneLineOrientationType.West, -1000.400269f, 153.409576f, 25.578859f, -1021.786133f, 121.336189f, -0.500030f);
                        zoneProperties.AddZoneLineBox("highkeep", -90.607208f, 98.531219f, 0.000000f, ZoneLineOrientationType.East, -83.776314f, -118.791763f, 12.469000f, -98.162193f, -140.129593f, -0.500000f);
                        zoneProperties.AddZoneLineBox("highkeep", 62.486179f, 97.604347f, 0.000030f, ZoneLineOrientationType.East, 70.161171f, -118.140022f, 12.469000f, 53.453629f, -126.744057f, -0.500010f);
                        zoneProperties.AddZoneLineBox("kithicor", 552.036682f, 4892.523438f, 689.904907f, ZoneLineOrientationType.South, -986.189697f, 98.161331f, 38.800350f, -1007.820984f, 83.809853f, -0.499890f);
                    }
                    break;
                case "hole":
                    {
                        zoneProperties.SetBaseZoneProperties("hole", "The Hole", -1049.98f, 640.04f, -77.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(10, 10, 10, 200, 500);
                        zoneProperties.AddZoneLineBox("paineel", 588.502197f, -941.292969f, -93.159729f, ZoneLineOrientationType.South, 608.765930f, -935.432007f, -82.499748f, 580.660583f, -947.818420f, -98.468742f);
                        // TODO: Make portal geometry not obstruct
                        // TODO: Change these to circle with radius
                        zoneProperties.AddZoneLineBox("neriakc", 480.001648f, -809.905090f, -55.968712f, ZoneLineOrientationType.North, 75.090286f, 356.037201f, -375.374756f, 67.145378f, 341.312317f, -386.343719f);
                        zoneProperties.AddZoneLineBox("paineel", 588.502197f, -941.292969f, -93.159729f, ZoneLineOrientationType.South, 55.819328f, 375.380615f, -375.374756f, 41.268639f, 367.254913f, -386.343750f);
                        zoneProperties.AddZoneLineBox("erudnext", -1552.149292f, -184.036606f, -47.968700f, ZoneLineOrientationType.North, 52.396881f, 326.834320f, -375.374756f, 37.806911f, 318.680603f, -386.343323f);
                    }
                    break;                
                case "innothule":
                    {
                        zoneProperties.SetBaseZoneProperties("innothule", "Innothule Swamp", -588f, -2192f, -25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(170, 160, 90, 10, 500);
                        zoneProperties.AddZoneLineBox("feerrott", -1020.344177f, -3092.292236f, -12.343540f, ZoneLineOrientationType.North, -1110.918945f, 1900.790283f, 9.191510f, -1156.486450f, 1899.104858f, -12.843200f);
                        zoneProperties.AddZoneLineBox("grobb", -179.500046f, 39.101452f, -0.000000f, ZoneLineOrientationType.West, -2781.871094f, -625.318726f, -16.126810f, -2804.662109f, -646.227112f, -35.062538f);
                        zoneProperties.AddZoneLineBox("guktop", -62.457378f, 42.394871f, 0.000010f, ZoneLineOrientationType.East, 150.598709f, -828.381348f, 0.967340f, 136.212891f, -843.098694f, -11.999980f);
                        zoneProperties.AddZoneLineBox("sro", -3168.635742f, 1032.933105f, -26.814310f, ZoneLineOrientationType.North, 2800f, 1250f, 19.084551f, 2554.791748f, 1120f, -35f);
                    }
                    break;
                case "kaladima":
                    {
                        zoneProperties.SetBaseZoneProperties("kaladima", "South Kaladim", -2f, -18f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                        zoneProperties.AddZoneLineBox("butcher", 3121.1667f, -179.98013f, 0.00088672107f, ZoneLineOrientationType.South, -66.545395f, 47.896313f, 14.469f, -85.64434f, 34.009415f, -0.49999186f);
                        zoneProperties.AddZoneLineBox("kaladimb", 409.332306f, 340.759308f, -24.000509f, ZoneLineOrientationType.North, 334.304260f, 252.005707f, 16.310989f, 317.203705f, 225.868561f, 0.608990f);
                        zoneProperties.AddZoneLineBox("kaladimb", 394.005920f, -270.823303f, 0.000210f, ZoneLineOrientationType.North, 414.648987f, -209.715607f, 22.469000f, 405.986603f, -280f, -0.499960f);
                    }
                    break;
                case "kaladimb":
                    {
                        zoneProperties.SetBaseZoneProperties("kaladimb", "North Kaladim", -267f, 414f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                        zoneProperties.AddZoneLineBox("kaladima", 306.093964f, 231.490326f, 0.020500f, ZoneLineOrientationType.South, 394.649292f, 346.066956f, -1.531000f, 397.138519f, 312.694366f, -24.499941f);
                        zoneProperties.AddZoneLineBox("kaladima", 393.919128f, -263.472565f, 0.000040f, ZoneLineOrientationType.South, 384.053192f, -259.715820f, 22.414330f, 373.654907f, -272.101318f, -0.499970f);
                    }
                    break;
                case "kedge":
                    {
                        zoneProperties.SetBaseZoneProperties("kedge", "Kedge Keep", 99.96f, 14.02f, 31.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(10, 10, 10, 25, 25);
                        zoneProperties.AddZoneLineBox("cauldron", -1170.507080f, -1030.383179f, -315.376831f, ZoneLineOrientationType.East, 140.130951f, 14.514380f, 348.342682f, 119.745049f, -10.192420f, 299.375000f);
                    }
                    break;
                case "kerraridge":
                    {
                        zoneProperties.SetBaseZoneProperties("kerraridge", "Kerra Isle", -859.97f, 474.96f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 220, 200, 10, 600);
                        zoneProperties.AddZoneLineBox("tox", -510.562134f, 2635.008545f, -38.249962f, ZoneLineOrientationType.East, 430.005493f, -948.882141f, 38.436760f, 399.657959f, -979.802734f, 19.500050f);
                    }
                    break;
                case "kithicor":
                    {
                        zoneProperties.SetBaseZoneProperties("kithicor", "Kithicor Forest", 3828f, 1889f, 459f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(120, 140, 100, 10, 200);
                        zoneProperties.AddZoneLineBox("commons", 1032.412720f, 4154.744629f, -52.093071f, ZoneLineOrientationType.North, 1408.693237f, -1098.195190f, 55.470139f, 1378.633545f, -1153.891724f, -52.593639f);
                        zoneProperties.AddZoneLineBox("highpass", -980.394165f, 90.663696f, -0.000010f, ZoneLineOrientationType.North, 569.884521f, 4903.054199f, 742.436829f, 558.181274f, 4885.024414f, 689.404907f);
                        zoneProperties.AddZoneLineBox("rivervale", -371.955841f, -282.273224f, 0.000020f, ZoneLineOrientationType.North, 2028.557495f, 3831.161621f, 472.718994f, 2020.498779f, 3818.663086f, 461.750427f);
                    }
                    break;                
                case "lakerathe":
                    {
                        zoneProperties.SetBaseZoneProperties("lakerathe", "Lake Rathetear", 1213f, 4183f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("arena", -56.940857f, -835.9014f, 7.882746f, ZoneLineOrientationType.West, 2360.1794f, 2708.7017f, 130.344f, 2329.8247f, 2699.243f, 92.11265f);
                        zoneProperties.AddZoneLineBox("southkarana", -8541.681641f, 1158.678223f, 0.000370f, ZoneLineOrientationType.North, 4392.966797f, 1200f, 38.467892f, 4366.503906f, 1132.421143f, -0.500990f);
                        zoneProperties.AddZoneLineBox("rathemtn", 3533.836426f, 2945.927734f, -3.874240f, ZoneLineOrientationType.North, 2647.961426f, -2217.051025f, 62.953671f, 2538.218994f, -2290.238770f, 1.250070f);      
                    }
                    break;
                case "lavastorm":
                    {
                        zoneProperties.SetBaseZoneProperties("lavastorm", "Lavastorm Mountains", 153.45f, -1842.79f, -16.37f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 800);
                        zoneProperties.AddZoneLineBox("soltemple", 255.801758f, 55.676472f, -0.999090f, ZoneLineOrientationType.North, 1381.412109f, 336.848877f, 156.155457f, 1356.645630f, 324.494568f, 145.188034f);
                        zoneProperties.AddZoneLineBox("soldunga", -449.347717f, -524.520508f, 69.968758f, ZoneLineOrientationType.South, 784.385925f, 231.909103f, 139.531494f, 763.337830f, 221.400375f, 126.562843f);
                        zoneProperties.AddZoneLineBox("soldungb", -419.581055f, -264.690491f, -111.967888f, ZoneLineOrientationType.South, 901.472107f, 489.983673f, 62.156502f, 880.400269f, 479.244751f, 51.187592f);
                        zoneProperties.AddZoneLineBox("najena", -16.450621f, 870.293030f, 0.000150f, ZoneLineOrientationType.East, -921.776184f, -1060.107300f, 61.094002f, -961.185852f, -1075.276733f, 12.125720f);
                        zoneProperties.AddZoneLineBox("nektulos", 3052.935791f, 312.635284f, -19.294090f, ZoneLineOrientationType.South, -2100.800537f, -115.948547f, 129.457657f, -2171.145996f, -253.399704f, -20.001289f);
                    }
                    break;
                case "lfaydark":
                    {
                        zoneProperties.SetBaseZoneProperties("lfaydark", "Lesser Faydark", -1769.93f, -108.08f, -1.11f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(230, 255, 200, 10, 300);
                        zoneProperties.AddZoneLineBox("gfaydark", -2612.000732f, -1113.000000f, 0.000290f, ZoneLineOrientationType.North, 2195.666504f, -1174.378906f, 67.384300f, 2176.618164f, -1215.322021f, -0.499960f);
                        zoneProperties.AddZoneLineBox("mistmoore", -295.757965f, 160.095764f, -181.936813f, ZoneLineOrientationType.West, -1153.577759f, 3291.550049f, 110.469002f, -1182.255737f, 3372.130859f, -0.499820f);
                        zoneProperties.AddZoneLineBox("steamfont", 590.807617f, 2193.784424f, -113.249947f, ZoneLineOrientationType.East, 940.560425f, -2182.093262f, 77.329933f, 889.527710f, -2186.912109f, -5.281170f);
                    }
                    break;
                case "mistmoore":
                    {
                        zoneProperties.SetBaseZoneProperties("mistmoore", "Castle Mistmoore", 123f, -295f, -177f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(60, 30, 90, 10, 250);
                        zoneProperties.AddZoneLineBox("lfaydark", -1166.805908f, 3263.892578f, 0.000850f, ZoneLineOrientationType.East, -279.682556f, 141.644180f, -78.362358f, -339.412628f, 108.218033f, -182.437500f);
                    }
                    break;
                case "misty":
                    {
                        zoneProperties.SetBaseZoneProperties("misty", "Misty Thicket", 0f, 0f, 2.43f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 120, 50, 10, 500);
                        zoneProperties.AddZoneLineBox("runnyeye", 231.871094f, 150.141602f, 1.001060f, ZoneLineOrientationType.South, -826.740295f, 1443.224487f, 3.532040f, -840.195496f, 1415.736206f, -11.249970f);
                        zoneProperties.AddZoneLineBox("rivervale", -83.344131f, 97.216301f, -0.000000f, ZoneLineOrientationType.East, 418.880157f, -2588.360596f, 11.531500f, 394.495270f, -2613.019531f, -11.249990f);
                    }
                    break;
                case "najena":
                    {
                        zoneProperties.SetBaseZoneProperties("najena", "Najena", -22.6f, 229.1f, -41.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 0, 40, 10, 110);
                        zoneProperties.AddZoneLineBox("lavastorm", -937.992371f, -1044.653320f, 12.625020f, ZoneLineOrientationType.West, 0.193110f, 929.818542f, 48.437752f, -30.192530f, 883.758789f, -0.499850f);
                    }
                    break;                
                case "nektulos":
                    {
                        zoneProperties.SetBaseZoneProperties("nektulos", "Nektulos Forest", -259f, -1201f, -5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(80, 90, 70, 10, 400);
                        zoneProperties.AddZoneLineBox("ecommons", 1569.311157f, 667.254028f, -21.531260f, ZoneLineOrientationType.East, -2666.610596f, -550.025208f, 31.661320f, -2707.922119f, -636.140076f, -22.031050f);
                        zoneProperties.AddZoneLineBox("lavastorm", -2075.322998f, -189.802826f, -19.598631f, ZoneLineOrientationType.North, 3164.449707f, 385.575653f, 151.052231f, 3094.654785f, 237.925507f, -19.999571f);
                        zoneProperties.AddZoneLineBox("neriaka", -0.739280f, 113.977829f, 28.000000f, ZoneLineOrientationType.South, 2270.015381f, -1092.749023f, 12.469000f, 2210.318115f, -1149.777344f, -0.499900f);
                    }
                    break;
                case "neriaka":
                    {
                        zoneProperties.SetBaseZoneProperties("neriaka", "Neriak Foreign Quarter", 156.92f, -2.94f, 31.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.AddZoneLineBox("nektulos", 2294.104980f, -1105.768066f, 0.000190f, ZoneLineOrientationType.North, 27.909149f, 168.129883f, 40.197109f, -14.193390f, 134.459396f, 27.500010f);
                        zoneProperties.AddZoneLineBox("neriakb", 83.471588f, -404.715454f, -14.000000f, ZoneLineOrientationType.East, 98.162216f, -339.024811f, 12.469000f, 64.296402f, -406.345734f, -14.499970f);
                        zoneProperties.AddZoneLineBox("neriakb", -237.565536f, -455.336853f, 14.000020f, ZoneLineOrientationType.North, -223.713684f, -447.620209f, 26.468010f, -238.099426f, -489.933807f, 13.500010f);
                    }
                    break;
                case "neriakb":
                    {
                        zoneProperties.SetBaseZoneProperties("neriakb", "Neriak Commons", -499.91f, 2.97f, -10.25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.AddZoneLineBox("neriaka", 83.959953f, -322.479065f, -14.000000f, ZoneLineOrientationType.West, 98.161079f, -305.681519f, 12.467630f, 69.775436f, -384.671295f, -14.500000f);
                        zoneProperties.AddZoneLineBox("neriaka", -252.560760f, -455.675934f, 14.000010f, ZoneLineOrientationType.South, -252.075302f, -447.619110f, 26.454840f, -267.196991f, -490.619293f, 13.499990f);
                        zoneProperties.AddZoneLineBox("neriakc", 209.334473f, -853.563110f, -41.968079f, ZoneLineOrientationType.North, 210.713379f, -844.618347f, -31.532860f, 203.079483f, -860.849731f, -42.468700f);
                    }
                    break;
                case "neriakc":
                    {
                        zoneProperties.SetBaseZoneProperties("neriakc", "Neriak Third Gate", -968.96f, 891.92f, -52.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                        zoneProperties.AddZoneLineBox("neriakb", 196.809433f, -853.183411f, -41.968700f, ZoneLineOrientationType.South, 203.418655f, -846.463745f, -31.531000f, 181.745132f, -860.847778f, -42.468739f);
                    }
                    break;
                case "northkarana": 
                    {
                        zoneProperties.SetBaseZoneProperties("northkarana", "Northern Plains of Karana", -382f, -284f, -7f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("eastkarana", 13.189000f, 1166.973999f, -37.343681f, ZoneLineOrientationType.East, 38.130348f, -3110.049316f, 42.091228f, -17.969540f, -3238.555908f, -37.988708f);
                        zoneProperties.AddZoneLineBox("southkarana", 2777.505371f, 906.202576f, -34.406231f, ZoneLineOrientationType.South, -4530.517090f, 1250.382690f, 59.137932f, -4598.474609f, 1171.919800f, -36.060040f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4429.286621f, -15935.629883f, -69.124321f, ZoneLineOrientationType.West, -4417.983887f, 3744.417480f, 200.000000f, -5000.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4409.286621f, -15935.629883f, -69.124352f, ZoneLineOrientationType.West, -4397.983887f, 3744.417480f, 200.000000f, -4417.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4389.286621f, -15935.629883f, -69.122940f, ZoneLineOrientationType.West, -4377.983887f, 3744.417480f, 200.000000f, -4397.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4369.286621f, -15935.629883f, -69.124352f, ZoneLineOrientationType.West, -4357.983887f, 3744.417480f, 200.000000f, -4377.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4349.286621f, -15935.629883f, -69.124130f, ZoneLineOrientationType.West, -4337.983887f, 3744.417480f, 200.000000f, -4357.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4329.286621f, -15935.629883f, -69.124962f, ZoneLineOrientationType.West, -4317.983887f, 3744.417480f, 200.000000f, -4337.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4309.286621f, -15935.629883f, -69.125008f, ZoneLineOrientationType.West, -4297.983887f, 3744.417480f, 200.000000f, -4317.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4289.286621f, -15935.629883f, -69.124153f, ZoneLineOrientationType.West, -4277.983887f, 3744.417480f, 200.000000f, -4297.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4269.286621f, -15935.629883f, -69.124008f, ZoneLineOrientationType.West, -4257.983887f, 3744.417480f, 200.000000f, -4277.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4249.286621f, -15935.629883f, -69.124062f, ZoneLineOrientationType.West, -4237.983887f, 3744.417480f, 200.000000f, -4257.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4229.286621f, -15935.629883f, -66.292328f, ZoneLineOrientationType.West, -4217.983887f, 3744.417480f, 200.000000f, -4237.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4209.286621f, -15935.629883f, -61.747200f, ZoneLineOrientationType.West, -4197.983887f, 3744.417480f, 200.000000f, -4217.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4189.286621f, -15935.629883f, -57.202209f, ZoneLineOrientationType.West, -4177.983887f, 3744.417480f, 200.000000f, -4197.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4169.286621f, -15935.629883f, -52.656712f, ZoneLineOrientationType.West, -4157.983887f, 3744.417480f, 200.000000f, -4177.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4149.286621f, -15935.629883f, -48.113232f, ZoneLineOrientationType.West, -4137.983887f, 3744.417480f, 200.000000f, -4157.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4129.286621f, -15935.629883f, -43.567230f, ZoneLineOrientationType.West, -4117.983887f, 3744.417480f, 200.000000f, -4137.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4109.286621f, -15935.629883f, -39.022579f, ZoneLineOrientationType.West, -4097.983887f, 3744.417480f, 200.000000f, -4117.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4089.286621f, -15935.629883f, -34.477600f, ZoneLineOrientationType.West, -4077.983887f, 3744.417480f, 200.000000f, -4097.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4069.286621f, -15935.629883f, -29.932230f, ZoneLineOrientationType.West, -4057.983887f, 3744.417480f, 200.000000f, -4077.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4049.286621f, -15935.629883f, -25.385630f, ZoneLineOrientationType.West, -4037.983887f, 3744.417480f, 200.000000f, -4057.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4029.286621f, -15935.629883f, -20.839479f, ZoneLineOrientationType.West, -4017.983887f, 3744.417480f, 200.000000f, -4037.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -4009.286621f, -15935.629883f, -16.298170f, ZoneLineOrientationType.West, -3997.983887f, 3744.417480f, 200.000000f, -4017.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3989.286621f, -15935.629883f, -13.606570f, ZoneLineOrientationType.West, -3977.983887f, 3744.417480f, 200.000000f, -3997.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3969.286621f, -15935.629883f, -12.278990f, ZoneLineOrientationType.West, -3957.983887f, 3744.417480f, 200.000000f, -3977.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3949.286621f, -15935.629883f, -10.799740f, ZoneLineOrientationType.West, -3937.983887f, 3744.417480f, 200.000000f, -3957.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3929.286621f, -15935.629883f, -9.324460f, ZoneLineOrientationType.West, -3917.983887f, 3744.417480f, 200.000000f, -3937.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3909.286621f, -15935.629883f, -7.845920f, ZoneLineOrientationType.West, -3897.983887f, 3744.417480f, 200.000000f, -3917.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3889.286621f, -15935.629883f, -6.369650f, ZoneLineOrientationType.West, -3877.983887f, 3744.417480f, 200.000000f, -3897.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3869.286621f, -15935.629883f, -4.892200f, ZoneLineOrientationType.West, -3857.983887f, 3744.417480f, 200.000000f, -3877.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3849.286621f, -15935.629883f, -3.936970f, ZoneLineOrientationType.West, -3837.983887f, 3744.417480f, 200.000000f, -3857.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3829.286621f, -15935.629883f, -3.936510f, ZoneLineOrientationType.West, -3817.983887f, 3744.417480f, 200.000000f, -3837.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3809.286621f, -15935.629883f, -3.936450f, ZoneLineOrientationType.West, -3797.983887f, 3744.417480f, 200.000000f, -3817.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3789.286621f, -15935.629883f, -3.937420f, ZoneLineOrientationType.West, -3777.983887f, 3744.417480f, 200.000000f, -3797.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3769.286621f, -15935.629883f, -3.936300f, ZoneLineOrientationType.West, -3757.983887f, 3744.417480f, 200.000000f, -3777.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3749.286621f, -15935.629883f, -3.936550f, ZoneLineOrientationType.West, -3737.983887f, 3744.417480f, 200.000000f, -3757.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3729.286621f, -15935.629883f, -3.934840f, ZoneLineOrientationType.West, -3717.983887f, 3744.417480f, 200.000000f, -3737.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3709.286621f, -15935.629883f, -3.936970f, ZoneLineOrientationType.West, -3697.983887f, 3744.417480f, 200.000000f, -3717.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3689.286621f, -15935.629883f, -3.936690f, ZoneLineOrientationType.West, -3677.983887f, 3744.417480f, 200.000000f, -3697.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3669.286621f, -15935.629883f, -3.936980f, ZoneLineOrientationType.West, -3657.983887f, 3744.417480f, 200.000000f, -3677.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3649.286621f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -3637.983887f, 3744.417480f, 200.000000f, -3657.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3629.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -3617.983887f, 3744.417480f, 200.000000f, -3637.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3609.286621f, -15935.629883f, -3.937040f, ZoneLineOrientationType.West, -3597.983887f, 3744.417480f, 200.000000f, -3617.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3589.286621f, -15935.629883f, -3.935510f, ZoneLineOrientationType.West, -3577.983887f, 3744.417480f, 200.000000f, -3597.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3569.286621f, -15935.629883f, -3.935760f, ZoneLineOrientationType.West, -3557.983887f, 3744.417480f, 200.000000f, -3577.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3549.286621f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -3537.983887f, 3744.417480f, 200.000000f, -3557.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3529.286621f, -15935.629883f, -3.937000f, ZoneLineOrientationType.West, -3517.983887f, 3744.417480f, 200.000000f, -3537.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3509.286621f, -15935.629883f, -3.935910f, ZoneLineOrientationType.West, -3497.983887f, 3744.417480f, 200.000000f, -3517.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3489.286621f, -15935.629883f, -3.936570f, ZoneLineOrientationType.West, -3477.983887f, 3744.417480f, 200.000000f, -3497.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3469.286621f, -15935.629883f, -3.936660f, ZoneLineOrientationType.West, -3457.983887f, 3744.417480f, 200.000000f, -3477.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3449.286621f, -15935.629883f, -3.936750f, ZoneLineOrientationType.West, -3437.983887f, 3744.417480f, 200.000000f, -3457.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3429.286621f, -15935.629883f, -3.936850f, ZoneLineOrientationType.West, -3417.983887f, 3744.417480f, 200.000000f, -3437.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3409.286621f, -15935.629883f, -3.936940f, ZoneLineOrientationType.West, -3397.983887f, 3744.417480f, 200.000000f, -3417.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3389.286621f, -15935.629883f, -3.937030f, ZoneLineOrientationType.West, -3377.983887f, 3744.417480f, 200.000000f, -3397.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3369.286621f, -15935.629883f, -3.935940f, ZoneLineOrientationType.West, -3357.983887f, 3744.417480f, 200.000000f, -3377.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3349.286621f, -15935.629883f, -3.936970f, ZoneLineOrientationType.West, -3337.983887f, 3744.417480f, 200.000000f, -3357.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3329.286621f, -15935.629883f, -3.935250f, ZoneLineOrientationType.West, -3317.983887f, 3744.417480f, 200.000000f, -3337.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3309.286621f, -15935.629883f, -3.936850f, ZoneLineOrientationType.West, -3297.983887f, 3744.417480f, 200.000000f, -3317.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3289.286621f, -15935.629883f, -3.935900f, ZoneLineOrientationType.West, -3277.983887f, 3744.417480f, 200.000000f, -3297.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3269.286621f, -15935.629883f, -3.936870f, ZoneLineOrientationType.West, -3257.983887f, 3744.417480f, 200.000000f, -3277.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3249.286621f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -3237.983887f, 3744.417480f, 200.000000f, -3257.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3229.286621f, -15935.629883f, -3.936600f, ZoneLineOrientationType.West, -3217.983887f, 3744.417480f, 200.000000f, -3237.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3209.286621f, -15935.629883f, -3.936690f, ZoneLineOrientationType.West, -3197.983887f, 3744.417480f, 200.000000f, -3217.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3189.286621f, -15935.629883f, -3.936250f, ZoneLineOrientationType.West, -3177.983887f, 3744.417480f, 200.000000f, -3197.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3169.286621f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -3157.983887f, 3744.417480f, 200.000000f, -3177.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3149.286621f, -15935.629883f, -3.935880f, ZoneLineOrientationType.West, -3137.983887f, 3744.417480f, 200.000000f, -3157.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3129.286621f, -15935.629883f, -3.936910f, ZoneLineOrientationType.West, -3117.983887f, 3744.417480f, 200.000000f, -3137.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3109.286621f, -15935.629883f, -3.935590f, ZoneLineOrientationType.West, -3097.983887f, 3744.417480f, 200.000000f, -3117.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3089.286621f, -15935.629883f, -3.937250f, ZoneLineOrientationType.West, -3077.983887f, 3744.417480f, 200.000000f, -3097.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3069.286621f, -15935.629883f, -3.936240f, ZoneLineOrientationType.West, -3057.983887f, 3744.417480f, 200.000000f, -3077.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3049.286621f, -15935.629883f, -3.936800f, ZoneLineOrientationType.West, -3037.983887f, 3744.417480f, 200.000000f, -3057.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3029.286621f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -3017.983887f, 3744.417480f, 200.000000f, -3037.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -3009.286621f, -15935.629883f, -3.937170f, ZoneLineOrientationType.West, -2997.983887f, 3744.417480f, 200.000000f, -3017.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2989.286621f, -15935.629883f, -3.936590f, ZoneLineOrientationType.West, -2977.983887f, 3744.417480f, 200.000000f, -2997.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2969.286621f, -15935.629883f, -3.936680f, ZoneLineOrientationType.West, -2957.983887f, 3744.417480f, 200.000000f, -2977.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2949.286621f, -15935.629883f, -3.937260f, ZoneLineOrientationType.West, -2937.983887f, 3744.417480f, 200.000000f, -2957.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2929.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -2917.983887f, 3744.417480f, 200.000000f, -2937.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2909.286621f, -15935.629883f, -3.936970f, ZoneLineOrientationType.West, -2897.983887f, 3744.417480f, 200.000000f, -2917.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2889.286621f, -15935.629883f, -3.935150f, ZoneLineOrientationType.West, -2877.983887f, 3744.417480f, 200.000000f, -2897.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2869.286621f, -15935.629883f, -3.936860f, ZoneLineOrientationType.West, -2857.983887f, 3744.417480f, 200.000000f, -2877.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2849.286621f, -15935.629883f, -3.936090f, ZoneLineOrientationType.West, -2837.983887f, 3744.417480f, 200.000000f, -2857.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2829.286621f, -15935.629883f, -3.936650f, ZoneLineOrientationType.West, -2817.983887f, 3744.417480f, 200.000000f, -2837.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2809.286621f, -15935.629883f, -3.936390f, ZoneLineOrientationType.West, -2797.983887f, 3744.417480f, 200.000000f, -2817.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2789.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -2777.983887f, 3744.417480f, 200.000000f, -2797.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2769.286621f, -15935.629883f, -3.936970f, ZoneLineOrientationType.West, -2757.983887f, 3744.417480f, 200.000000f, -2777.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2749.286621f, -15935.629883f, -3.936490f, ZoneLineOrientationType.West, -2737.983887f, 3744.417480f, 200.000000f, -2757.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2729.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -2717.983887f, 3744.417480f, 200.000000f, -2737.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2709.286621f, -15935.629883f, -3.936570f, ZoneLineOrientationType.West, -2697.983887f, 3744.417480f, 200.000000f, -2717.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2689.286621f, -15935.629883f, -3.937060f, ZoneLineOrientationType.West, -2677.983887f, 3744.417480f, 200.000000f, -2697.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2669.286621f, -15935.629883f, -3.935750f, ZoneLineOrientationType.West, -2657.983887f, 3744.417480f, 200.000000f, -2677.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2649.286621f, -15935.629883f, -3.936820f, ZoneLineOrientationType.West, -2637.983887f, 3744.417480f, 200.000000f, -2657.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2629.286621f, -15935.629883f, -3.936020f, ZoneLineOrientationType.West, -2617.983887f, 3744.417480f, 200.000000f, -2637.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2609.286621f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -2597.983887f, 3744.417480f, 200.000000f, -2617.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2589.286621f, -15935.629883f, -3.936360f, ZoneLineOrientationType.West, -2577.983887f, 3744.417480f, 200.000000f, -2597.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2569.286621f, -15935.629883f, -3.936620f, ZoneLineOrientationType.West, -2557.983887f, 3744.417480f, 200.000000f, -2577.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2549.286621f, -15935.629883f, -3.936670f, ZoneLineOrientationType.West, -2537.983887f, 3744.417480f, 200.000000f, -2557.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2529.286621f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -2517.983887f, 3744.417480f, 200.000000f, -2537.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2509.286621f, -15935.629883f, -3.937020f, ZoneLineOrientationType.West, -2497.983887f, 3744.417480f, 200.000000f, -2517.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2489.286621f, -15935.629883f, -3.935200f, ZoneLineOrientationType.West, -2477.983887f, 3744.417480f, 200.000000f, -2497.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2469.286621f, -15935.629883f, -3.935450f, ZoneLineOrientationType.West, -2457.983887f, 3744.417480f, 200.000000f, -2477.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2449.286621f, -15935.629883f, -3.935680f, ZoneLineOrientationType.West, -2437.983887f, 3744.417480f, 200.000000f, -2457.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2429.286621f, -15935.629883f, -3.937290f, ZoneLineOrientationType.West, -2417.983887f, 3744.417480f, 200.000000f, -2437.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2409.286621f, -15935.629883f, -3.935920f, ZoneLineOrientationType.West, -2397.983887f, 3744.417480f, 200.000000f, -2417.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2389.286621f, -15935.629883f, -3.936990f, ZoneLineOrientationType.West, -2377.983887f, 3744.417480f, 200.000000f, -2397.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2369.286621f, -15935.629883f, -3.936830f, ZoneLineOrientationType.West, -2357.983887f, 3744.417480f, 200.000000f, -2377.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2349.286621f, -15935.629883f, -3.936820f, ZoneLineOrientationType.West, -2337.983887f, 3744.417480f, 200.000000f, -2357.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2329.286621f, -15935.629883f, -3.936910f, ZoneLineOrientationType.West, -2317.983887f, 3744.417480f, 200.000000f, -2337.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2309.286621f, -15935.629883f, -3.936720f, ZoneLineOrientationType.West, -2297.983887f, 3744.417480f, 200.000000f, -2317.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2289.286621f, -15935.629883f, -3.937450f, ZoneLineOrientationType.West, -2277.983887f, 3744.417480f, 200.000000f, -2297.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2269.286621f, -15935.629883f, -3.935630f, ZoneLineOrientationType.West, -2257.983887f, 3744.417480f, 200.000000f, -2277.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2249.286621f, -15935.629883f, -3.936650f, ZoneLineOrientationType.West, -2237.983887f, 3744.417480f, 200.000000f, -2257.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2229.286621f, -15935.629883f, -3.936140f, ZoneLineOrientationType.West, -2217.983887f, 3744.417480f, 200.000000f, -2237.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2209.286621f, -15935.629883f, -3.936390f, ZoneLineOrientationType.West, -2197.983887f, 3744.417480f, 200.000000f, -2217.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2189.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -2177.983887f, 3744.417480f, 200.000000f, -2197.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2169.286621f, -15935.629883f, -3.936940f, ZoneLineOrientationType.West, -2157.983887f, 3744.417480f, 200.000000f, -2177.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2149.286621f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -2137.983887f, 3744.417480f, 200.000000f, -2157.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2129.286621f, -15935.629883f, -3.936570f, ZoneLineOrientationType.West, -2117.983887f, 3744.417480f, 200.000000f, -2137.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2109.286621f, -15935.629883f, -3.936550f, ZoneLineOrientationType.West, -2097.983887f, 3744.417480f, 200.000000f, -2117.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2089.286621f, -15935.629883f, -3.936620f, ZoneLineOrientationType.West, -2077.983887f, 3744.417480f, 200.000000f, -2097.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2069.286621f, -15935.629883f, -3.937110f, ZoneLineOrientationType.West, -2057.983887f, 3744.417480f, 200.000000f, -2077.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2049.286621f, -15935.629883f, -3.936330f, ZoneLineOrientationType.West, -2037.983887f, 3744.417480f, 200.000000f, -2057.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2029.286499f, -15935.629883f, -3.936820f, ZoneLineOrientationType.West, -2017.983887f, 3744.417480f, 200.000000f, -2037.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -2009.286499f, -15935.629883f, -3.936080f, ZoneLineOrientationType.West, -1997.983887f, 3744.417480f, 200.000000f, -2017.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1989.286499f, -15935.629883f, -3.936640f, ZoneLineOrientationType.West, -1977.983887f, 3744.417480f, 200.000000f, -1997.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1969.286499f, -15935.629883f, -3.935490f, ZoneLineOrientationType.West, -1957.983887f, 3744.417480f, 200.000000f, -1977.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1949.286499f, -15935.629883f, -3.935750f, ZoneLineOrientationType.West, -1937.983887f, 3744.417480f, 200.000000f, -1957.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1929.286499f, -15935.629883f, -3.935010f, ZoneLineOrientationType.West, -1917.983887f, 3744.417480f, 200.000000f, -1937.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1909.286499f, -15935.629883f, -3.935260f, ZoneLineOrientationType.West, -1897.983887f, 3744.417480f, 200.000000f, -1917.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1889.286499f, -15935.629883f, -3.935520f, ZoneLineOrientationType.West, -1877.983887f, 3744.417480f, 200.000000f, -1897.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1869.286499f, -15935.629883f, -3.936540f, ZoneLineOrientationType.West, -1857.983887f, 3744.417480f, 200.000000f, -1877.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1849.286499f, -15935.629883f, -3.936610f, ZoneLineOrientationType.West, -1837.983887f, 3744.417480f, 200.000000f, -1857.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1829.286499f, -15935.629883f, -3.936400f, ZoneLineOrientationType.West, -1817.983887f, 3744.417480f, 200.000000f, -1837.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1809.286499f, -15935.629883f, -3.936650f, ZoneLineOrientationType.West, -1797.983887f, 3744.417480f, 200.000000f, -1817.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1789.286499f, -15935.629883f, -3.936440f, ZoneLineOrientationType.West, -1777.983887f, 3744.417480f, 200.000000f, -1797.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1769.286499f, -15935.629883f, -3.937000f, ZoneLineOrientationType.West, -1757.983887f, 3744.417480f, 200.000000f, -1777.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1749.286499f, -15935.629883f, -3.935850f, ZoneLineOrientationType.West, -1737.983887f, 3744.417480f, 200.000000f, -1757.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1729.286499f, -15935.629883f, -3.936980f, ZoneLineOrientationType.West, -1717.983887f, 3744.417480f, 200.000000f, -1737.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1709.286499f, -15935.629883f, -3.936710f, ZoneLineOrientationType.West, -1697.983887f, 3744.417480f, 200.000000f, -1717.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1689.286499f, -15935.629883f, -3.936500f, ZoneLineOrientationType.West, -1677.983887f, 3744.417480f, 200.000000f, -1697.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1669.286499f, -15935.629883f, -3.936590f, ZoneLineOrientationType.West, -1657.983887f, 3744.417480f, 200.000000f, -1677.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1649.286499f, -15935.629883f, -3.936380f, ZoneLineOrientationType.West, -1637.983887f, 3744.417480f, 200.000000f, -1657.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1629.286499f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -1617.983887f, 3744.417480f, 200.000000f, -1637.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1609.286499f, -15935.629883f, -3.934760f, ZoneLineOrientationType.West, -1597.983887f, 3744.417480f, 200.000000f, -1617.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1589.286499f, -15935.629883f, -3.937010f, ZoneLineOrientationType.West, -1577.983887f, 3744.417480f, 200.000000f, -1597.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1569.286499f, -15935.629883f, -3.936360f, ZoneLineOrientationType.West, -1557.983887f, 3744.417480f, 200.000000f, -1577.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1549.286499f, -15935.629883f, -3.936820f, ZoneLineOrientationType.West, -1537.983887f, 3744.417480f, 200.000000f, -1557.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1529.286499f, -15935.629883f, -3.936700f, ZoneLineOrientationType.West, -1517.983887f, 3744.417480f, 200.000000f, -1537.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1509.286499f, -15935.629883f, -3.936490f, ZoneLineOrientationType.West, -1497.983887f, 3744.417480f, 200.000000f, -1517.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1489.286499f, -15935.629883f, -3.936480f, ZoneLineOrientationType.West, -1477.983887f, 3744.417480f, 200.000000f, -1497.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1469.286499f, -15935.629883f, -3.934760f, ZoneLineOrientationType.West, -1457.983887f, 3744.417480f, 200.000000f, -1477.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1449.286499f, -15935.629883f, -3.937320f, ZoneLineOrientationType.West, -1437.983887f, 3744.417480f, 200.000000f, -1457.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1429.286499f, -15935.629883f, -3.935050f, ZoneLineOrientationType.West, -1417.983887f, 3744.417480f, 200.000000f, -1437.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1409.286499f, -15935.629883f, -3.936630f, ZoneLineOrientationType.West, -1397.983887f, 3744.417480f, 200.000000f, -1417.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1389.286499f, -15935.629883f, -3.936580f, ZoneLineOrientationType.West, -1377.983887f, 3744.417480f, 200.000000f, -1397.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1369.286499f, -15935.629883f, -3.936840f, ZoneLineOrientationType.West, -1357.983887f, 3744.417480f, 200.000000f, -1377.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1349.286499f, -15935.629883f, -3.936550f, ZoneLineOrientationType.West, -1337.983887f, 3744.417480f, 200.000000f, -1357.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1329.286499f, -15935.629883f, -3.935810f, ZoneLineOrientationType.West, -1317.983887f, 3744.417480f, 200.000000f, -1337.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1309.286499f, -15935.629883f, -3.937400f, ZoneLineOrientationType.West, -1297.983887f, 3744.417480f, 200.000000f, -1317.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1289.286499f, -15935.629883f, -3.936660f, ZoneLineOrientationType.West, -1277.983887f, 3744.417480f, 200.000000f, -1297.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1269.286499f, -15935.629883f, -3.936640f, ZoneLineOrientationType.West, -1257.983887f, 3744.417480f, 200.000000f, -1277.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1249.286499f, -15935.629883f, -3.936490f, ZoneLineOrientationType.West, -1237.983887f, 3744.417480f, 200.000000f, -1257.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1229.286499f, -15935.629883f, -3.936740f, ZoneLineOrientationType.West, -1217.983887f, 3744.417480f, 200.000000f, -1237.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1209.286499f, -15935.629883f, -3.937000f, ZoneLineOrientationType.West, -1197.983887f, 3744.417480f, 200.000000f, -1217.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1189.286499f, -15935.629883f, -3.936260f, ZoneLineOrientationType.West, -1177.983887f, 3744.417480f, 200.000000f, -1197.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1169.286499f, -15935.629883f, -3.937210f, ZoneLineOrientationType.West, -1157.983887f, 3744.417480f, 200.000000f, -1177.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1149.286499f, -15935.629883f, -3.936470f, ZoneLineOrientationType.West, -1137.983887f, 3744.417480f, 200.000000f, -1157.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1129.286499f, -15935.629883f, -3.936960f, ZoneLineOrientationType.West, -1117.983887f, 3744.417480f, 200.000000f, -1137.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1109.286499f, -15935.629883f, -3.935710f, ZoneLineOrientationType.West, -1097.983887f, 3744.417480f, 200.000000f, -1117.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1089.286499f, -15935.629883f, -3.937050f, ZoneLineOrientationType.West, -1077.983887f, 3744.417480f, 200.000000f, -1097.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1069.286499f, -15935.629883f, -3.936600f, ZoneLineOrientationType.West, -1057.983887f, 3744.417480f, 200.000000f, -1077.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1049.286499f, -15935.629883f, -3.936770f, ZoneLineOrientationType.West, -1037.983887f, 3744.417480f, 200.000000f, -1057.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1029.286499f, -15935.629883f, -3.936860f, ZoneLineOrientationType.West, -1017.983948f, 3744.417480f, 200.000000f, -1037.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -1009.286499f, -15935.629883f, -3.936120f, ZoneLineOrientationType.West, -997.983948f, 3744.417480f, 200.000000f, -1017.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -989.286499f, -15935.629883f, -3.936780f, ZoneLineOrientationType.West, -977.983948f, 3744.417480f, 200.000000f, -997.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -969.286499f, -15935.629883f, -3.935530f, ZoneLineOrientationType.West, -957.983948f, 3744.417480f, 200.000000f, -977.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -949.286499f, -15935.629883f, -3.936600f, ZoneLineOrientationType.West, -937.983948f, 3744.417480f, 200.000000f, -957.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -929.286499f, -15935.629883f, -3.936430f, ZoneLineOrientationType.West, -917.983948f, 3744.417480f, 200.000000f, -937.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -909.286499f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -897.983948f, 3744.417480f, 200.000000f, -917.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -889.286499f, -15935.629883f, -3.936540f, ZoneLineOrientationType.West, -877.983948f, 3744.417480f, 200.000000f, -897.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -869.286499f, -15935.629883f, -3.935390f, ZoneLineOrientationType.West, -857.983948f, 3744.417480f, 200.000000f, -877.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -849.286499f, -15935.629883f, -3.935650f, ZoneLineOrientationType.West, -837.983948f, 3744.417480f, 200.000000f, -857.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -829.286499f, -15935.629883f, -3.935900f, ZoneLineOrientationType.West, -817.983948f, 3744.417480f, 200.000000f, -837.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -809.286499f, -15935.629883f, -3.936570f, ZoneLineOrientationType.West, -797.983948f, 3744.417480f, 200.000000f, -817.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -789.286499f, -15935.629883f, -3.936010f, ZoneLineOrientationType.West, -777.983948f, 3744.417480f, 200.000000f, -797.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -769.286499f, -15935.629883f, -3.936570f, ZoneLineOrientationType.West, -757.983948f, 3744.417480f, 200.000000f, -777.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -749.286499f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -737.983948f, 3744.417480f, 200.000000f, -757.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -729.286499f, -15935.629883f, -3.936930f, ZoneLineOrientationType.West, -717.983948f, 3744.417480f, 200.000000f, -737.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -709.286499f, -15935.629883f, -3.936640f, ZoneLineOrientationType.West, -697.983948f, 3744.417480f, 200.000000f, -717.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -689.286499f, -15935.629883f, -3.936430f, ZoneLineOrientationType.West, -677.983948f, 3744.417480f, 200.000000f, -697.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -669.286499f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -657.983887f, 3744.417480f, 200.000000f, -677.983948f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -649.286499f, -15935.629883f, -3.935270f, ZoneLineOrientationType.West, -637.983887f, 3744.417480f, 200.000000f, -657.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -629.286499f, -15935.629883f, -3.937150f, ZoneLineOrientationType.West, -617.983887f, 3744.417480f, 200.000000f, -637.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -609.286499f, -15935.629883f, -3.936590f, ZoneLineOrientationType.West, -597.983887f, 3744.417480f, 200.000000f, -617.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -589.286499f, -15935.629883f, -3.937060f, ZoneLineOrientationType.West, -577.983887f, 3744.417480f, 200.000000f, -597.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -569.286499f, -15935.629883f, -3.937320f, ZoneLineOrientationType.West, -557.983887f, 3744.417480f, 200.000000f, -577.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -549.286499f, -15935.629883f, -3.934710f, ZoneLineOrientationType.West, -537.983887f, 3744.417480f, 200.000000f, -557.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -529.286499f, -15935.629883f, -3.937260f, ZoneLineOrientationType.West, -517.983887f, 3744.417480f, 200.000000f, -537.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -509.286469f, -15935.629883f, -3.936520f, ZoneLineOrientationType.West, -497.983887f, 3744.417480f, 200.000000f, -517.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -489.286469f, -15935.629883f, -3.936610f, ZoneLineOrientationType.West, -477.983887f, 3744.417480f, 200.000000f, -497.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -469.286469f, -15935.629883f, -3.936400f, ZoneLineOrientationType.West, -457.983887f, 3744.417480f, 200.000000f, -477.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -449.286469f, -15935.629883f, -3.936490f, ZoneLineOrientationType.West, -437.983887f, 3744.417480f, 200.000000f, -457.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -429.286469f, -15935.629883f, -3.936470f, ZoneLineOrientationType.West, -417.983887f, 3744.417480f, 200.000000f, -437.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -409.286469f, -15935.629883f, -3.936730f, ZoneLineOrientationType.West, -397.983887f, 3744.417480f, 200.000000f, -417.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -389.286469f, -15935.629883f, -3.936590f, ZoneLineOrientationType.West, -377.983887f, 3744.417480f, 200.000000f, -397.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -369.286469f, -15935.629883f, -3.935850f, ZoneLineOrientationType.West, -357.983887f, 3744.417480f, 200.000000f, -377.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -349.286469f, -15935.629883f, -3.936790f, ZoneLineOrientationType.West, -337.983887f, 3744.417480f, 200.000000f, -357.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -329.286499f, -15935.629883f, -3.935650f, ZoneLineOrientationType.West, -317.983887f, 3744.417480f, 200.000000f, -337.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -309.286499f, -15935.629883f, -3.937160f, ZoneLineOrientationType.West, -297.983887f, 3744.417480f, 200.000000f, -317.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -289.286499f, -15935.629883f, -3.935160f, ZoneLineOrientationType.West, -277.983887f, 3744.417480f, 200.000000f, -297.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -269.286499f, -15935.629883f, -3.937010f, ZoneLineOrientationType.West, -257.983887f, 3744.417480f, 200.000000f, -277.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -249.286499f, -15935.629883f, -3.937080f, ZoneLineOrientationType.West, -237.983887f, 3744.417480f, 200.000000f, -257.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -229.286499f, -15935.629883f, -3.937160f, ZoneLineOrientationType.West, -217.983887f, 3744.417480f, 200.000000f, -237.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -209.286499f, -15935.629883f, -3.936710f, ZoneLineOrientationType.West, -197.983887f, 3744.417480f, 200.000000f, -217.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -189.286499f, -15935.629883f, -3.936690f, ZoneLineOrientationType.West, -177.983887f, 3744.417480f, 200.000000f, -197.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -169.286499f, -15935.629883f, -3.937480f, ZoneLineOrientationType.West, -157.983887f, 3744.417480f, 200.000000f, -177.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -149.001923f, -15935.786133f, -3.896160f, ZoneLineOrientationType.West, -137.983887f, 3744.417480f, 200.000000f, -157.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -129.001923f, -15935.786133f, -3.936490f, ZoneLineOrientationType.West, -117.983887f, 3744.417480f, 200.000000f, -137.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -109.001923f, -15935.786133f, -3.936410f, ZoneLineOrientationType.West, -97.983887f, 3744.417480f, 200.000000f, -117.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -89.001923f, -15935.786133f, -3.936550f, ZoneLineOrientationType.West, -77.983887f, 3744.417480f, 200.000000f, -97.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -69.001930f, -15935.786133f, -3.937040f, ZoneLineOrientationType.West, -57.983891f, 3744.417480f, 200.000000f, -77.983887f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -49.001930f, -15935.786133f, -3.937170f, ZoneLineOrientationType.West, -37.983891f, 3744.417480f, 200.000000f, -57.983891f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -29.001921f, -15935.786133f, -3.936370f, ZoneLineOrientationType.West, -17.983891f, 3744.417480f, 200.000000f, -37.983891f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", -9.001920f, -15935.786133f, -3.936780f, ZoneLineOrientationType.West, 2.016110f, 3744.417480f, 200.000000f, -17.983891f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 10.998080f, -15935.786133f, -3.936910f, ZoneLineOrientationType.West, 22.016109f, 3744.417480f, 200.000000f, 2.016110f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 30.998079f, -15935.786133f, -3.937440f, ZoneLineOrientationType.West, 42.016121f, 3744.417480f, 200.000000f, 22.016109f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 50.998081f, -15935.786133f, -3.935660f, ZoneLineOrientationType.West, 62.016121f, 3744.417480f, 200.000000f, 42.016121f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 70.998077f, -15935.786133f, -3.936810f, ZoneLineOrientationType.West, 82.016121f, 3744.417480f, 200.000000f, 62.016121f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 90.998077f, -15935.786133f, -3.936860f, ZoneLineOrientationType.West, 102.016113f, 3744.417480f, 200.000000f, 82.016121f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 110.998077f, -15935.786133f, -3.936910f, ZoneLineOrientationType.West, 122.016113f, 3744.417480f, 200.000000f, 102.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 130.998077f, -15935.786133f, -3.936480f, ZoneLineOrientationType.West, 142.016113f, 3744.417480f, 200.000000f, 122.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 150.998077f, -15935.786133f, -3.936610f, ZoneLineOrientationType.West, 162.016113f, 3744.417480f, 200.000000f, 142.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 170.998077f, -15935.786133f, -3.937170f, ZoneLineOrientationType.West, 182.016113f, 3744.417480f, 200.000000f, 162.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 190.998077f, -15935.786133f, -3.936390f, ZoneLineOrientationType.West, 202.016113f, 3744.417480f, 200.000000f, 182.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 210.998077f, -15935.786133f, -3.935840f, ZoneLineOrientationType.West, 222.016113f, 3744.417480f, 200.000000f, 202.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 230.998077f, -15935.786133f, -3.936090f, ZoneLineOrientationType.West, 242.016113f, 3744.417480f, 200.000000f, 222.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 250.998077f, -15935.786133f, -3.936630f, ZoneLineOrientationType.West, 262.016113f, 3744.417480f, 200.000000f, 242.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 270.998077f, -15935.786133f, -3.935360f, ZoneLineOrientationType.West, 282.016113f, 3744.417480f, 200.000000f, 262.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 290.998077f, -15935.786133f, -3.936770f, ZoneLineOrientationType.West, 302.016113f, 3744.417480f, 200.000000f, 282.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 310.998077f, -15935.786133f, -3.936680f, ZoneLineOrientationType.West, 322.016113f, 3744.417480f, 200.000000f, 302.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 330.998077f, -15935.786133f, -3.936730f, ZoneLineOrientationType.West, 342.016113f, 3744.417480f, 200.000000f, 322.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 350.998077f, -15935.786133f, -3.935060f, ZoneLineOrientationType.West, 362.016113f, 3744.417480f, 200.000000f, 342.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 370.998077f, -15935.786133f, -3.936680f, ZoneLineOrientationType.West, 382.016113f, 3744.417480f, 200.000000f, 362.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 390.998077f, -15935.786133f, -3.936490f, ZoneLineOrientationType.West, 402.016113f, 3744.417480f, 200.000000f, 382.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 410.998077f, -15935.786133f, -3.936550f, ZoneLineOrientationType.West, 422.016113f, 3744.417480f, 200.000000f, 402.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 430.998077f, -15935.786133f, -3.934870f, ZoneLineOrientationType.West, 442.016113f, 3744.417480f, 200.000000f, 422.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 450.998077f, -15935.786133f, -3.936730f, ZoneLineOrientationType.West, 462.016113f, 3744.417480f, 200.000000f, 442.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 470.998077f, -15935.786133f, -3.936800f, ZoneLineOrientationType.West, 482.016113f, 3744.417480f, 200.000000f, 462.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 490.998077f, -15935.786133f, -3.935620f, ZoneLineOrientationType.West, 502.016113f, 3744.417480f, 200.000000f, 482.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 510.998077f, -15935.786133f, -3.937000f, ZoneLineOrientationType.West, 522.016113f, 3744.417480f, 200.000000f, 502.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 530.998047f, -15935.786133f, -3.936480f, ZoneLineOrientationType.West, 542.016113f, 3744.417480f, 200.000000f, 522.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 550.998047f, -15935.786133f, -3.934700f, ZoneLineOrientationType.West, 562.016113f, 3744.417480f, 200.000000f, 542.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 570.998047f, -15935.786133f, -3.936530f, ZoneLineOrientationType.West, 582.016113f, 3744.417480f, 200.000000f, 562.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 590.998047f, -15935.786133f, -3.936560f, ZoneLineOrientationType.West, 602.016113f, 3744.417480f, 200.000000f, 582.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 610.998047f, -15935.786133f, -3.935290f, ZoneLineOrientationType.West, 622.016113f, 3744.417480f, 200.000000f, 602.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 630.998047f, -15935.786133f, -3.936610f, ZoneLineOrientationType.West, 642.016113f, 3744.417480f, 200.000000f, 622.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 650.998047f, -15935.786133f, -3.937180f, ZoneLineOrientationType.West, 662.016113f, 3744.417480f, 200.000000f, 642.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 670.998047f, -15935.786133f, -3.934940f, ZoneLineOrientationType.West, 682.016052f, 3744.417480f, 200.000000f, 662.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 690.998108f, -15935.786133f, -3.935190f, ZoneLineOrientationType.West, 702.016052f, 3744.417480f, 200.000000f, 682.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 710.998108f, -15935.786133f, -3.936540f, ZoneLineOrientationType.West, 722.016052f, 3744.417480f, 200.000000f, 702.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 730.998108f, -15935.786133f, -3.934760f, ZoneLineOrientationType.West, 742.016052f, 3744.417480f, 200.000000f, 722.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 750.998108f, -15935.786133f, -3.936730f, ZoneLineOrientationType.West, 762.016052f, 3744.417480f, 200.000000f, 742.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 770.998108f, -15935.786133f, -3.936770f, ZoneLineOrientationType.West, 782.016052f, 3744.417480f, 200.000000f, 762.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 790.998108f, -15935.786133f, -3.934990f, ZoneLineOrientationType.West, 802.016052f, 3744.417480f, 200.000000f, 782.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 810.998108f, -15935.786133f, -3.936610f, ZoneLineOrientationType.West, 822.016052f, 3744.417480f, 200.000000f, 802.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 830.998108f, -15935.786133f, -3.934930f, ZoneLineOrientationType.West, 842.016052f, 3744.417480f, 200.000000f, 822.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 850.998108f, -15935.786133f, -3.935720f, ZoneLineOrientationType.West, 862.016052f, 3744.417480f, 200.000000f, 842.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 870.998108f, -15935.786133f, -3.936740f, ZoneLineOrientationType.West, 882.016052f, 3744.417480f, 200.000000f, 862.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 890.998108f, -15935.786133f, -3.936550f, ZoneLineOrientationType.West, 902.016052f, 3744.417480f, 200.000000f, 882.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 910.998108f, -15935.786133f, -3.936690f, ZoneLineOrientationType.West, 922.016052f, 3744.417480f, 200.000000f, 902.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 930.998108f, -15935.786133f, -3.936670f, ZoneLineOrientationType.West, 942.016052f, 3744.417480f, 200.000000f, 922.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 950.998108f, -15935.786133f, -3.936570f, ZoneLineOrientationType.West, 962.016052f, 3744.417480f, 200.000000f, 942.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 970.998108f, -15935.786133f, -3.935300f, ZoneLineOrientationType.West, 982.016052f, 3744.417480f, 200.000000f, 962.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 990.998108f, -15935.786133f, -3.935570f, ZoneLineOrientationType.West, 1002.016052f, 3744.417480f, 200.000000f, 982.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1010.998108f, -15935.786133f, -3.937030f, ZoneLineOrientationType.West, 1022.016052f, 3744.417480f, 200.000000f, 1002.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1030.998047f, -15935.786133f, -3.936560f, ZoneLineOrientationType.West, 1042.016113f, 3744.417480f, 200.000000f, 1022.016052f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1050.998047f, -15935.786133f, -3.936580f, ZoneLineOrientationType.West, 1062.016113f, 3744.417480f, 200.000000f, 1042.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1070.998047f, -15935.786133f, -3.936630f, ZoneLineOrientationType.West, 1082.016113f, 3744.417480f, 200.000000f, 1062.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1090.998047f, -15935.786133f, -3.936830f, ZoneLineOrientationType.West, 1102.016113f, 3744.417480f, 200.000000f, 1082.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1110.998047f, -15935.786133f, 2.664960f, ZoneLineOrientationType.West, 1122.016113f, 3744.417480f, 200.000000f, 1102.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1130.998047f, -15935.786133f, 13.462410f, ZoneLineOrientationType.West, 1142.016113f, 3744.417480f, 200.000000f, 1122.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1150.998047f, -15935.786133f, 20.513180f, ZoneLineOrientationType.West, 1162.016113f, 3744.417480f, 200.000000f, 1142.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1170.998047f, -15935.786133f, 27.565720f, ZoneLineOrientationType.West, 1182.016113f, 3744.417480f, 200.000000f, 1162.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1190.998047f, -15935.786133f, 34.615608f, ZoneLineOrientationType.West, 1202.016113f, 3744.417480f, 200.000000f, 1182.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1210.998047f, -15935.786133f, 41.667130f, ZoneLineOrientationType.West, 1222.016113f, 3744.417480f, 200.000000f, 1202.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1230.998047f, -15935.786133f, 48.718231f, ZoneLineOrientationType.West, 1242.016113f, 3744.417480f, 200.000000f, 1222.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1250.998047f, -15935.786133f, 55.769260f, ZoneLineOrientationType.West, 1262.016113f, 3744.417480f, 300.000000f, 1242.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1270.998047f, -15935.786133f, 62.820889f, ZoneLineOrientationType.West, 1282.016113f, 3744.417480f, 300.000000f, 1262.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1290.998047f, -15935.786133f, 69.871483f, ZoneLineOrientationType.West, 1302.016113f, 3744.417480f, 300.000000f, 1282.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1310.998047f, -15935.786133f, 76.923477f, ZoneLineOrientationType.West, 1322.016113f, 3744.417480f, 300.000000f, 1302.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1330.998047f, -15935.786133f, 83.973763f, ZoneLineOrientationType.West, 1342.016113f, 3744.417480f, 300.000000f, 1322.016113f, 3714.417480f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 1350.998047f, -15935.786133f, 91.028137f, ZoneLineOrientationType.West, 1500.016113f, 3744.417480f, 300.000000f, 1342.016113f, 3714.417480f, -100.000000f);
                    }
                    break;
                case "nro":
                    {
                        zoneProperties.SetBaseZoneProperties("nro", "Northern Desert of Ro", 299.12f, 3537.9f, -24.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -128.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 935.000000f, 200.000000f, 4172.241699f, 895.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -148.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 895.000000f, 200.000000f, 4172.241699f, 875.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -168.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 875.000000f, 200.000000f, 4172.241699f, 855.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -188.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 855.000000f, 200.000000f, 4172.241699f, 835.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -208.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 835.000000f, 200.000000f, 4172.241699f, 815.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -228.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 815.000000f, 200.000000f, 4172.241699f, 795.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -248.602066f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 795.000000f, 200.000000f, 4172.241699f, 775.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -268.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 775.000000f, 200.000000f, 4172.241699f, 755.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -288.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 755.000000f, 200.000000f, 4172.241699f, 735.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -308.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 735.000000f, 200.000000f, 4172.241699f, 715.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -328.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 715.000000f, 200.000000f, 4172.241699f, 695.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -348.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 695.000000f, 200.000000f, 4172.241699f, 675.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -368.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 675.000000f, 200.000000f, 4172.241699f, 655.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -388.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 655.000000f, 200.000000f, 4172.241699f, 635.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -408.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 635.000000f, 200.000000f, 4172.241699f, 615.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -428.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 615.000000f, 200.000000f, 4172.241699f, 595.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -448.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 595.000000f, 200.000000f, 4172.241699f, 575.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -468.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 575.000000f, 200.000000f, 4172.241699f, 555.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -488.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 555.000000f, 200.000000f, 4172.241699f, 535.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -508.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 535.000000f, 200.000000f, 4172.241699f, 515.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -528.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 515.000000f, 200.000000f, 4172.241699f, 495.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -548.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 495.000000f, 200.000000f, 4172.241699f, 475.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -568.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 475.000000f, 200.000000f, 4172.241699f, 455.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -588.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 455.000000f, 200.000000f, 4172.241699f, 435.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -608.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 435.000000f, 200.000000f, 4172.241699f, 415.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -628.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 415.000000f, 200.000000f, 4172.241699f, 395.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -648.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 395.000000f, 200.000000f, 4172.241699f, 375.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -668.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 375.000000f, 200.000000f, 4172.241699f, 355.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -688.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 355.000000f, 200.000000f, 4172.241699f, 335.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -708.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 335.000000f, 200.000000f, 4172.241699f, 315.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -728.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 315.000000f, 200.000000f, 4172.241699f, 295.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -748.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 295.000000f, 200.000000f, 4172.241699f, 275.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -768.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 275.000000f, 200.000000f, 4172.241699f, 255.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -788.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 255.000000f, 200.000000f, 4172.241699f, 235.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -808.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 235.000000f, 200.000000f, 4172.241699f, 215.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -828.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 215.000000f, 200.000000f, 4172.241699f, 195.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -848.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 195.000000f, 200.000000f, 4172.241699f, 175.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -868.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 175.000000f, 200.000000f, 4172.241699f, 155.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -888.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 155.000000f, 200.000000f, 4172.241699f, 135.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -908.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 135.000000f, 200.000000f, 4172.241699f, 115.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -928.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 115.000000f, 200.000000f, 4172.241699f, 95.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -948.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 95.000000f, 200.000000f, 4172.241699f, 75.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -968.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 75.000000f, 200.000000f, 4172.241699f, 55.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -988.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 55.000000f, 200.000000f, 4172.241699f, 35.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1008.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 35.000000f, 200.000000f, 4172.241699f, 15.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1028.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, 15.000000f, 200.000000f, 4172.241699f, -5.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1048.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -5.000000f, 200.000000f, 4172.241699f, -25.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1068.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -25.000000f, 200.000000f, 4172.241699f, -45.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1088.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -45.000000f, 200.000000f, 4172.241699f, -65.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1108.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -65.000000f, 200.000000f, 4172.241699f, -85.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1128.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -85.000000f, 200.000000f, 4172.241699f, -105.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1148.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -105.000000f, 200.000000f, 4172.241699f, -125.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1168.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -125.000000f, 200.000000f, 4172.241699f, -145.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1188.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -145.000000f, 200.000000f, 4172.241699f, -165.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1208.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -165.000000f, 200.000000f, 4172.241699f, -185.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1228.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -185.000000f, 200.000000f, 4172.241699f, -205.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1248.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -205.000000f, 200.000000f, 4172.241699f, -225.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("freporte", -1316.303711f, -1263.602051f, -55.968739f, ZoneLineOrientationType.North, 4202.241699f, -225.000000f, 200.000000f, 4172.241699f, -265.000000f, -100.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1239.252441f, 21.636360f, ZoneLineOrientationType.South, -1878f, 1500.229980f, 300.000000f, -1900f, 1231.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1219.252441f, 22.805220f, ZoneLineOrientationType.South, -1878f, 1231.229980f, 300.000000f, -1900f, 1211.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1199.252441f, 23.974270f, ZoneLineOrientationType.South, -1878f, 1211.229980f, 300.000000f, -1900f, 1191.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1179.252441f, 25.147091f, ZoneLineOrientationType.South, -1878f, 1191.229980f, 300.000000f, -1900f, 1171.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1159.252441f, 25.584009f, ZoneLineOrientationType.South, -1878f, 1171.229980f, 300.000000f, -1900f, 1151.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1139.252441f, 25.984921f, ZoneLineOrientationType.South, -1878f, 1151.229980f, 300.000000f, -1900f, 1131.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1119.252441f, 26.372841f, ZoneLineOrientationType.South, -1878f, 1131.229980f, 300.000000f, -1900f, 1111.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1099.252441f, 26.760839f, ZoneLineOrientationType.South, -1878f, 1111.229980f, 300.000000f, -1900f, 1091.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1079.252441f, 27.148649f, ZoneLineOrientationType.South, -1878f, 1091.229980f, 300.000000f, -1900f, 1071.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1059.252441f, 27.167200f, ZoneLineOrientationType.South, -1878f, 1071.229980f, 300.000000f, -1900f, 1051.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1039.252441f, 26.993990f, ZoneLineOrientationType.South, -1878f, 1051.229980f, 300.000000f, -1900f, 1031.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 1019.252502f, 26.600121f, ZoneLineOrientationType.South, -1878f, 1031.229980f, 300.000000f, -1900f, 1011.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 999.252502f, 26.203390f, ZoneLineOrientationType.South, -1878f, 1011.229980f, 300.000000f, -1900f, 991.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 979.252502f, 25.806890f, ZoneLineOrientationType.South, -1878f, 991.229980f, 300.000000f, -1900f, 971.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 959.252502f, 25.788059f, ZoneLineOrientationType.South, -1878f, 971.229980f, 300.000000f, -1900f, 951.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 939.252502f, 24.566401f, ZoneLineOrientationType.South, -1878f, 951.229980f, 300.000000f, -1900f, 931.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 919.252502f, 21.840040f, ZoneLineOrientationType.South, -1878f, 931.229980f, 300.000000f, -1900f, 911.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 899.252502f, 19.114100f, ZoneLineOrientationType.South, -1878f, 911.229980f, 300.000000f, -1900f, 891.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 879.252502f, 16.380091f, ZoneLineOrientationType.South, -1878f, 891.229980f, 300.000000f, -1900f, 871.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 859.252502f, 16.249411f, ZoneLineOrientationType.South, -1878f, 871.229980f, 300.000000f, -1900f, 851.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 839.252502f, 15.835400f, ZoneLineOrientationType.South, -1878f, 851.229980f, 300.000000f, -1900f, 831.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 819.252502f, 14.938530f, ZoneLineOrientationType.South, -1878f, 831.229980f, 300.000000f, -1900f, 811.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 799.252502f, 14.043360f, ZoneLineOrientationType.South, -1878f, 811.229980f, 300.000000f, -1900f, 791.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 779.252502f, 13.148440f, ZoneLineOrientationType.South, -1878f, 791.229980f, 300.000000f, -1900f, 771.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 759.252502f, 11.505960f, ZoneLineOrientationType.South, -1878f, 771.229980f, 300.000000f, -1900f, 751.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 739.252502f, 10.579040f, ZoneLineOrientationType.South, -1878f, 751.229980f, 300.000000f, -1900f, 731.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 719.252502f, 10.579120f, ZoneLineOrientationType.South, -1878f, 731.229980f, 300.000000f, -1900f, 711.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 699.252502f, 10.579510f, ZoneLineOrientationType.South, -1878f, 711.229980f, 300.000000f, -1900f, 691.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 679.252502f, 10.579230f, ZoneLineOrientationType.South, -1878f, 691.229980f, 300.000000f, -1900f, 671.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 659.252441f, 4.989020f, ZoneLineOrientationType.South, -1878f, 671.229980f, 300.000000f, -1900f, 651.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 639.252441f, 1.750520f, ZoneLineOrientationType.South, -1878f, 651.229980f, 300.000000f, -1900f, 631.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 619.252441f, 1.750570f, ZoneLineOrientationType.South, -1878f, 631.229980f, 300.000000f, -1900f, 611.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 599.252441f, 1.750080f, ZoneLineOrientationType.South, -1878f, 611.229980f, 300.000000f, -1900f, 591.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 579.252441f, 1.750100f, ZoneLineOrientationType.South, -1878f, 591.229980f, 300.000000f, -1900f, 571.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 559.252441f, 1.750250f, ZoneLineOrientationType.South, -1878f, 571.229980f, 300.000000f, -1900f, 551.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 539.252441f, 1.407590f, ZoneLineOrientationType.South, -1878f, 551.229980f, 300.000000f, -1900f, 531.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 519.252441f, 0.632410f, ZoneLineOrientationType.South, -1878f, 531.229980f, 300.000000f, -1900f, 511.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 499.252441f, -0.142320f, ZoneLineOrientationType.South, -1878f, 511.230011f, 300.000000f, -1900f, 491.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 479.252441f, -0.921910f, ZoneLineOrientationType.South, -1878f, 491.230011f, 300.000000f, -1900f, 471.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 459.252441f, -0.588930f, ZoneLineOrientationType.South, -1878f, 471.230011f, 300.000000f, -1900f, 451.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 439.252441f, -2.299280f, ZoneLineOrientationType.South, -1878f, 451.230011f, 300.000000f, -1900f, 431.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 419.252441f, -6.598600f, ZoneLineOrientationType.South, -1878f, 431.230011f, 300.000000f, -1900f, 411.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 399.252441f, -10.900240f, ZoneLineOrientationType.South, -1878f, 411.230011f, 300.000000f, -1900f, 391.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 379.252441f, -15.201820f, ZoneLineOrientationType.South, -1878f, 391.230011f, 300.000000f, -1900f, 371.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 359.252441f, -13.067110f, ZoneLineOrientationType.South, -1878f, 371.230011f, 300.000000f, -1900f, 351.230011f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 339.252441f, -11.130530f, ZoneLineOrientationType.South, -1878f, 351.230011f, 300.000000f, -1900f, 331.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 319.252441f, -9.835140f, ZoneLineOrientationType.South, -1878f, 331.229980f, 300.000000f, -1900f, 311.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 299.252441f, -8.539930f, ZoneLineOrientationType.South, -1878f, 311.229980f, 300.000000f, -1900f, 291.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 279.252441f, -7.249940f, ZoneLineOrientationType.South, -1878f, 291.229980f, 300.000000f, -1900f, 271.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 259.252441f, -4.595450f, ZoneLineOrientationType.South, -1878f, 271.229980f, 300.000000f, -1900f, 251.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 239.252441f, 1.599700f, ZoneLineOrientationType.South, -1878f, 251.229980f, 300.000000f, -1900f, 231.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 219.252441f, 12.023810f, ZoneLineOrientationType.South, -1878f, 231.229980f, 300.000000f, -1900f, 211.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 199.252441f, 22.448219f, ZoneLineOrientationType.South, -1878f, 211.229980f, 300.000000f, -1900f, 191.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 179.252441f, 32.874660f, ZoneLineOrientationType.South, -1878f, 191.229980f, 300.000000f, -1900f, 171.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 159.252441f, 38.845650f, ZoneLineOrientationType.South, -1878f, 171.229980f, 300.000000f, -1900f, 151.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 139.252441f, 42.769871f, ZoneLineOrientationType.South, -1878f, 151.229980f, 300.000000f, -1900f, 131.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 119.252441f, 44.451469f, ZoneLineOrientationType.South, -1878f, 131.229980f, 300.000000f, -1900f, 111.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 99.252441f, 46.133499f, ZoneLineOrientationType.South, -1878f, 111.229980f, 300.000000f, -1900f, 91.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 79.252441f, 47.815441f, ZoneLineOrientationType.South, -1878f, 91.229980f, 300.000000f, -1900f, 71.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 59.252441f, 47.895851f, ZoneLineOrientationType.South, -1878f, 71.229980f, 300.000000f, -1900f, 51.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 39.252441f, 47.666088f, ZoneLineOrientationType.South, -1878f, 51.229980f, 300.000000f, -1900f, 31.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, 19.252439f, 47.153721f, ZoneLineOrientationType.South, -1878f, 31.229980f, 300.000000f, -1900f, 11.229980f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -0.747560f, 46.641392f, ZoneLineOrientationType.South, -1878f, 11.229980f, 300.000000f, -1900f, -8.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -20.747561f, 46.129292f, ZoneLineOrientationType.South, -1878f, -8.770020f, 300.000000f, -1900f, -28.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -40.747559f, 46.222469f, ZoneLineOrientationType.South, -1878f, -28.770020f, 300.000000f, -1900f, -48.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -60.747559f, 45.824760f, ZoneLineOrientationType.South, -1878f, -48.770020f, 300.000000f, -1900f, -68.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -80.747559f, 44.768810f, ZoneLineOrientationType.South, -1878f, -68.770020f, 300.000000f, -1900f, -88.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -100.747559f, 43.713421f, ZoneLineOrientationType.South, -1878f, -88.770020f, 300.000000f, -1900f, -108.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -120.747559f, 42.660069f, ZoneLineOrientationType.South, -1878f, -108.770020f, 300.000000f, -1900f, -128.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -140.747559f, 43.350559f, ZoneLineOrientationType.South, -1878f, -128.770020f, 300.000000f, -1900f, -148.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -160.747559f, 43.900349f, ZoneLineOrientationType.South, -1878f, -148.770020f, 300.000000f, -1900f, -168.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -180.747559f, 44.170551f, ZoneLineOrientationType.South, -1878f, -168.770020f, 300.000000f, -1900f, -188.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -200.747559f, 44.441021f, ZoneLineOrientationType.South, -1878f, -188.770020f, 300.000000f, -1900f, -208.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -220.747559f, 44.711781f, ZoneLineOrientationType.South, -1878f, -208.770020f, 300.000000f, -1900f, -228.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -240.747559f, 44.724800f, ZoneLineOrientationType.South, -1878f, -228.770020f, 300.000000f, -1900f, -248.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -260.747559f, 44.551331f, ZoneLineOrientationType.South, -1878f, -248.770020f, 300.000000f, -1900f, -268.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -280.747559f, 44.164749f, ZoneLineOrientationType.South, -1878f, -268.770020f, 300.000000f, -1900f, -288.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -300.747559f, 43.778198f, ZoneLineOrientationType.South, -1878f, -288.770020f, 300.000000f, -1900f, -308.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -320.747559f, 43.390808f, ZoneLineOrientationType.South, -1878f, -308.770020f, 300.000000f, -1900f, -328.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -340.747559f, 42.884041f, ZoneLineOrientationType.South, -1878f, -328.770020f, 300.000000f, -1900f, -348.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -360.747559f, 42.472370f, ZoneLineOrientationType.South, -1878f, -348.769989f, 300.000000f, -1900f, -368.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -380.747559f, 42.204800f, ZoneLineOrientationType.South, -1878f, -368.769989f, 300.000000f, -1900f, -388.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -400.747559f, 41.936619f, ZoneLineOrientationType.South, -1878f, -388.769989f, 300.000000f, -1900f, -408.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -420.747559f, 41.664650f, ZoneLineOrientationType.South, -1878f, -408.769989f, 300.000000f, -1900f, -428.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -440.747559f, 39.041630f, ZoneLineOrientationType.South, -1878f, -428.769989f, 300.000000f, -1900f, -448.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -460.747559f, 35.604820f, ZoneLineOrientationType.South, -1878f, -448.769989f, 300.000000f, -1900f, -468.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -480.747559f, 31.312210f, ZoneLineOrientationType.South, -1878f, -468.769989f, 300.000000f, -1900f, -488.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -500.747559f, 27.020281f, ZoneLineOrientationType.South, -1878f, -488.769989f, 300.000000f, -1900f, -508.769989f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -520.747559f, 22.728399f, ZoneLineOrientationType.South, -1878f, -508.769989f, 300.000000f, -1900f, -528.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -540.747559f, 17.421190f, ZoneLineOrientationType.South, -1878f, -528.770020f, 300.000000f, -1900f, -548.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -560.747559f, 13.013620f, ZoneLineOrientationType.South, -1878f, -548.770020f, 300.000000f, -1900f, -568.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -580.747559f, 9.775330f, ZoneLineOrientationType.South, -1878f, -568.770020f, 300.000000f, -1900f, -588.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -600.747559f, 6.536830f, ZoneLineOrientationType.South, -1878f, -588.770020f, 300.000000f, -1900f, -608.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -620.747559f, 3.294970f, ZoneLineOrientationType.South, -1878f, -608.770020f, 300.000000f, -1900f, -628.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -640.747559f, 2.011570f, ZoneLineOrientationType.South, -1878f, -628.770020f, 300.000000f, -1900f, -648.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -660.747559f, 1.708660f, ZoneLineOrientationType.South, -1878f, -648.770020f, 300.000000f, -1900f, -668.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -680.747498f, 2.490760f, ZoneLineOrientationType.South, -1878f, -668.770020f, 300.000000f, -1900f, -688.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -700.747498f, 3.273800f, ZoneLineOrientationType.South, -1878f, -688.770020f, 300.000000f, -1900f, -708.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -720.747498f, 4.056780f, ZoneLineOrientationType.South, -1878f, -708.770020f, 300.000000f, -1900f, -728.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -740.747498f, 1.383310f, ZoneLineOrientationType.South, -1878f, -728.770020f, 300.000000f, -1900f, -748.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -760.747498f, -1.239650f, ZoneLineOrientationType.South, -1878f, -748.770020f, 300.000000f, -1900f, -768.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -780.747498f, -3.587200f, ZoneLineOrientationType.South, -1878f, -768.770020f, 300.000000f, -1900f, -788.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -800.747498f, -5.934350f, ZoneLineOrientationType.South, -1878f, -788.770020f, 300.000000f, -1900f, -808.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -820.747498f, -6.406100f, ZoneLineOrientationType.South, -1878f, -808.770020f, 300.000000f, -1900f, -828.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -840.747498f, -6.406000f, ZoneLineOrientationType.South, -1878f, -828.770020f, 300.000000f, -1900f, -848.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("oasis", 2540.233154f, -860.747498f, -6.406110f, ZoneLineOrientationType.South, -1878f, -848.770020f, 300.000000f, -1900f, -1200.770020f, -200.000000f);
                        zoneProperties.AddZoneLineBox("ecommons", -3023.223633f, -1147.192261f, 0.000050f, ZoneLineOrientationType.West, 2077.083984f, 1928.101074f, 28.065140f, 2007.522705f, 1900.196045f, -0.499880f);
                    }
                    break;
                case "oasis":
                    {
                        zoneProperties.SetBaseZoneProperties("oasis", "Oasis of Marr", 903.98f, 490.03f, 6.4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1241.229980f, 30.033600f, ZoneLineOrientationType.North, 2590f, 1500.252441f, 300.000000f, 2560f, 1229.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1221.229980f, 31.204430f, ZoneLineOrientationType.North, 2590f, 1229.252441f, 300.000000f, 2560f, 1209.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1201.229980f, 31.854610f, ZoneLineOrientationType.North, 2590f, 1209.252441f, 300.000000f, 2560f, 1189.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1181.229980f, 32.248322f, ZoneLineOrientationType.North, 2590f, 1189.252441f, 300.000000f, 2560f, 1169.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1161.229980f, 32.642101f, ZoneLineOrientationType.North, 2590f, 1169.252441f, 300.000000f, 2560f, 1149.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1141.229980f, 33.036030f, ZoneLineOrientationType.North, 2590f, 1149.252441f, 300.000000f, 2560f, 1129.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1121.229980f, 33.429749f, ZoneLineOrientationType.North, 2590f, 1129.252441f, 300.000000f, 2560f, 1109.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1101.229980f, 33.559669f, ZoneLineOrientationType.North, 2590f, 1109.252441f, 300.000000f, 2560f, 1089.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1081.229980f, 33.559719f, ZoneLineOrientationType.North, 2590f, 1089.252441f, 300.000000f, 2560f, 1069.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1061.229980f, 33.559502f, ZoneLineOrientationType.North, 2590f, 1069.252441f, 300.000000f, 2560f, 1049.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1041.229980f, 33.423931f, ZoneLineOrientationType.North, 2590f, 1049.252441f, 300.000000f, 2560f, 1029.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1021.229980f, 33.030449f, ZoneLineOrientationType.North, 2590f, 1029.252441f, 300.000000f, 2560f, 1009.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1001.229980f, 32.896721f, ZoneLineOrientationType.North, 2590f, 1009.252502f, 300.000000f, 2560f, 989.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 981.229980f, 32.896290f, ZoneLineOrientationType.North, 2590f, 989.252502f, 300.000000f, 2560f, 969.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 961.229980f, 32.895741f, ZoneLineOrientationType.North, 2590f, 969.252502f, 300.000000f, 2560f, 949.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 941.229980f, 31.942690f, ZoneLineOrientationType.North, 2590f, 949.252502f, 300.000000f, 2560f, 929.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 921.229980f, 29.214920f, ZoneLineOrientationType.North, 2590f, 929.252502f, 300.000000f, 2560f, 909.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 901.229980f, 28.314159f, ZoneLineOrientationType.North, 2590f, 909.252502f, 300.000000f, 2560f, 889.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 881.229980f, 28.314159f, ZoneLineOrientationType.North, 2590f, 889.252502f, 300.000000f, 2560f, 869.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 861.229980f, 28.314199f, ZoneLineOrientationType.North, 2590f, 869.252502f, 300.000000f, 2560f, 849.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 841.229980f, 27.997549f, ZoneLineOrientationType.North, 2590f, 849.252502f, 300.000000f, 2560f, 829.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 821.229980f, 27.092230f, ZoneLineOrientationType.North, 2590f, 829.252502f, 300.000000f, 2560f, 809.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 801.229980f, 25.666780f, ZoneLineOrientationType.North, 2590f, 809.252502f, 300.000000f, 2560f, 789.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 781.229980f, 23.984989f, ZoneLineOrientationType.North, 2590f, 789.252502f, 300.000000f, 2560f, 769.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 761.229980f, 22.306400f, ZoneLineOrientationType.North, 2590f, 769.252502f, 300.000000f, 2560f, 749.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 741.229980f, 21.218809f, ZoneLineOrientationType.North, 2590f, 749.252502f, 300.000000f, 2560f, 729.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 721.229980f, 21.217211f, ZoneLineOrientationType.North, 2590f, 729.252502f, 300.000000f, 2560f, 709.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 701.229980f, 17.291059f, ZoneLineOrientationType.North, 2590f, 709.252502f, 300.000000f, 2560f, 689.252502f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 681.229980f, 11.429590f, ZoneLineOrientationType.North, 2590f, 689.252502f, 300.000000f, 2560f, 669.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 661.229980f, 5.566800f, ZoneLineOrientationType.North, 2590f, 669.252441f, 300.000000f, 2560f, 649.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 641.229980f, 1.750140f, ZoneLineOrientationType.North, 2590f, 649.252441f, 300.000000f, 2560f, 629.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 621.229980f, 1.750060f, ZoneLineOrientationType.North, 2590f, 629.252441f, 300.000000f, 2560f, 609.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 601.229980f, 1.750310f, ZoneLineOrientationType.North, 2590f, 609.252441f, 300.000000f, 2560f, 589.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 581.229980f, 1.749970f, ZoneLineOrientationType.North, 2590f, 589.252441f, 300.000000f, 2560f, 569.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 561.229980f, 1.750130f, ZoneLineOrientationType.North, 2590f, 569.252441f, 300.000000f, 2560f, 549.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 541.229980f, 1.477440f, ZoneLineOrientationType.North, 2590f, 549.252441f, 300.000000f, 2560f, 529.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 521.229980f, 0.696480f, ZoneLineOrientationType.North, 2590f, 529.252441f, 300.000000f, 2560f, 509.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 501.230011f, 0.702600f, ZoneLineOrientationType.North, 2590f, 509.252441f, 300.000000f, 2560f, 489.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 481.230011f, 1.096270f, ZoneLineOrientationType.North, 2590f, 489.252441f, 300.000000f, 2560f, 469.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 461.230011f, 1.489190f, ZoneLineOrientationType.North, 2590f, 469.252441f, 300.000000f, 2560f, 449.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 441.230011f, 0.239070f, ZoneLineOrientationType.North, 2590f, 449.252441f, 300.000000f, 2560f, 429.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 421.230011f, -4.065280f, ZoneLineOrientationType.North, 2590f, 429.252441f, 300.000000f, 2560f, 409.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 401.230011f, -3.834260f, ZoneLineOrientationType.North, 2590f, 409.252441f, 300.000000f, 2560f, 389.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 381.230011f, -1.367880f, ZoneLineOrientationType.North, 2590f, 389.252441f, 300.000000f, 2560f, 369.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 361.230011f, 1.099570f, ZoneLineOrientationType.North, 2590f, 369.252441f, 300.000000f, 2560f, 349.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 341.230011f, 3.158530f, ZoneLineOrientationType.North, 2590f, 349.252441f, 300.000000f, 2560f, 329.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 321.229980f, 4.450130f, ZoneLineOrientationType.North, 2590f, 329.252441f, 300.000000f, 2560f, 309.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 301.229980f, 6.698310f, ZoneLineOrientationType.North, 2590f, 309.252441f, 300.000000f, 2560f, 289.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 281.229980f, 9.426860f, ZoneLineOrientationType.North, 2590f, 289.252441f, 300.000000f, 2560f, 269.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 261.229980f, 12.158010f, ZoneLineOrientationType.North, 2590f, 269.252441f, 300.000000f, 2560f, 249.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 241.229980f, 17.582541f, ZoneLineOrientationType.North, 2590f, 249.252441f, 300.000000f, 2560f, 229.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 221.229980f, 28.020960f, ZoneLineOrientationType.North, 2590f, 229.252441f, 300.000000f, 2560f, 209.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 201.229980f, 35.314270f, ZoneLineOrientationType.North, 2590f, 209.252441f, 300.000000f, 2560f, 189.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 181.229980f, 41.057659f, ZoneLineOrientationType.North, 2590f, 189.252441f, 300.000000f, 2560f, 169.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 161.229980f, 46.801510f, ZoneLineOrientationType.North, 2590f, 169.252441f, 300.000000f, 2560f, 149.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 141.229980f, 51.119049f, ZoneLineOrientationType.North, 2590f, 149.252441f, 300.000000f, 2560f, 129.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 121.229980f, 52.788261f, ZoneLineOrientationType.North, 2590f, 129.252441f, 300.000000f, 2560f, 109.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 101.229980f, 53.347881f, ZoneLineOrientationType.North, 2590f, 109.252441f, 300.000000f, 2560f, 89.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 81.229980f, 53.352081f, ZoneLineOrientationType.North, 2590f, 89.252441f, 300.000000f, 2560f, 69.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 61.229980f, 53.355190f, ZoneLineOrientationType.North, 2590f, 69.252441f, 300.000000f, 2560f, 49.252441f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 41.229980f, 53.176750f, ZoneLineOrientationType.North, 2590f, 49.252441f, 300.000000f, 2560f, 29.252439f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 21.229980f, 52.665791f, ZoneLineOrientationType.North, 2590f, 29.252439f, 300.000000f, 2560f, 9.252440f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, 1.229980f, 52.576851f, ZoneLineOrientationType.North, 2590f, 9.252440f, 300.000000f, 2560f, -10.747560f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -18.770020f, 52.695461f, ZoneLineOrientationType.North, 2590f, -10.747560f, 300.000000f, 2560f, -30.747561f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -38.770020f, 52.815609f, ZoneLineOrientationType.North, 2590f, -30.747561f, 300.000000f, 2560f, -50.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -58.770020f, 52.526031f, ZoneLineOrientationType.North, 2590f, -50.747559f, 300.000000f, 2560f, -70.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -78.770020f, 51.465820f, ZoneLineOrientationType.North, 2590f, -70.747559f, 300.000000f, 2560f, -90.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -98.770020f, 51.639549f, ZoneLineOrientationType.North, 2590f, -90.747559f, 300.000000f, 2560f, -110.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -118.770020f, 52.421059f, ZoneLineOrientationType.North, 2590f, -110.747559f, 300.000000f, 2560f, -130.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -138.770020f, 53.202438f, ZoneLineOrientationType.North, 2590f, -130.747559f, 300.000000f, 2560f, -150.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -158.770020f, 53.810509f, ZoneLineOrientationType.North, 2590f, -150.747559f, 300.000000f, 2560f, -170.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -178.770020f, 54.081211f, ZoneLineOrientationType.North, 2590f, -170.747559f, 300.000000f, 2560f, -190.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -198.770020f, 54.174801f, ZoneLineOrientationType.North, 2590f, -190.747559f, 300.000000f, 2560f, -210.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -218.770020f, 54.177811f, ZoneLineOrientationType.North, 2590f, -210.747559f, 300.000000f, 2560f, -230.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -238.770020f, 54.180111f, ZoneLineOrientationType.North, 2590f, -230.747559f, 300.000000f, 2560f, -250.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -258.770020f, 54.042179f, ZoneLineOrientationType.North, 2590f, -250.747559f, 300.000000f, 2560f, -270.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -278.770020f, 53.646809f, ZoneLineOrientationType.North, 2590f, -270.747559f, 300.000000f, 2560f, -290.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -298.770020f, 53.171070f, ZoneLineOrientationType.North, 2590f, -290.747559f, 300.000000f, 2560f, -310.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -318.770020f, 52.657700f, ZoneLineOrientationType.North, 2590f, -310.747559f, 300.000000f, 2560f, -330.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -338.769989f, 52.145191f, ZoneLineOrientationType.North, 2590f, -330.747559f, 300.000000f, 2560f, -350.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -358.769989f, 51.717529f, ZoneLineOrientationType.North, 2590f, -350.747559f, 300.000000f, 2560f, -370.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -378.769989f, 51.447670f, ZoneLineOrientationType.North, 2590f, -370.747559f, 300.000000f, 2560f, -390.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -398.769989f, 49.524891f, ZoneLineOrientationType.North, 2590f, -390.747559f, 300.000000f, 2560f, -410.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -418.769989f, 46.792030f, ZoneLineOrientationType.North, 2590f, -410.747559f, 300.000000f, 2560f, -430.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -438.769989f, 44.060749f, ZoneLineOrientationType.North, 2590f, -430.747559f, 300.000000f, 2560f, -450.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -458.769989f, 40.781170f, ZoneLineOrientationType.North, 2590f, -450.747559f, 300.000000f, 2560f, -470.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -478.769989f, 36.477970f, ZoneLineOrientationType.North, 2590f, -470.747559f, 300.000000f, 2560f, -490.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -498.769989f, 31.476549f, ZoneLineOrientationType.North, 2590f, -490.747559f, 300.000000f, 2560f, -510.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -518.770020f, 26.128920f, ZoneLineOrientationType.North, 2590f, -510.747559f, 300.000000f, 2560f, -530.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -538.770020f, 20.779930f, ZoneLineOrientationType.North, 2590f, -530.747559f, 300.000000f, 2560f, -550.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -558.770020f, 16.167030f, ZoneLineOrientationType.North, 2590f, -550.747559f, 300.000000f, 2560f, -570.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -578.770020f, 12.928620f, ZoneLineOrientationType.North, 2590f, -570.747559f, 300.000000f, 2560f, -590.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -598.770020f, 11.071880f, ZoneLineOrientationType.North, 2590f, -590.747559f, 300.000000f, 2560f, -610.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -618.770020f, 9.896990f, ZoneLineOrientationType.North, 2590f, -610.747559f, 300.000000f, 2560f, -630.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -638.770020f, 8.722420f, ZoneLineOrientationType.North, 2590f, -630.747559f, 300.000000f, 2560f, -650.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -658.770020f, 8.231810f, ZoneLineOrientationType.North, 2590f, -650.747559f, 300.000000f, 2560f, -670.747559f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -678.770020f, 9.012640f, ZoneLineOrientationType.North, 2590f, -670.747559f, 300.000000f, 2560f, -690.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -698.770020f, 7.365450f, ZoneLineOrientationType.North, 2590f, -690.747498f, 300.000000f, 2560f, -710.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -718.770020f, 4.516680f, ZoneLineOrientationType.North, 2590f, -710.747498f, 300.000000f, 2560f, -730.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -738.770020f, 1.666720f, ZoneLineOrientationType.North, 2590f, -730.747498f, 300.000000f, 2560f, -750.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -758.770020f, -1.020910f, ZoneLineOrientationType.North, 2590f, -750.747498f, 300.000000f, 2560f, -770.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -778.770020f, -3.347860f, ZoneLineOrientationType.North, 2590f, -770.747498f, 300.000000f, 2560f, -790.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -798.770020f, -6.148300f, ZoneLineOrientationType.North, 2590f, -790.747498f, 300.000000f, 2560f, -810.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -818.770020f, -6.406220f, ZoneLineOrientationType.North, 2590f, -810.747498f, 300.000000f, 2560f, -830.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -838.770020f, -6.406150f, ZoneLineOrientationType.North, 2590f, -830.747498f, 300.000000f, 2560f, -850.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("nro", -1858.000000f, -858.770020f, -6.406100f, ZoneLineOrientationType.North, 2590f, -850.747498f, 300.000000f, 2560f, -1200.747498f, -200.000000f);
                        zoneProperties.AddZoneLineBox("sro", 1433.793579f, 244.703186f, -13.301640f, ZoneLineOrientationType.South, -1890.072144f, 301.506622f, 119.807327f, -1933.566406f, 58.167610f, 1.784040f);
                    }
                    break;
                case "oggok":
                    {
                        zoneProperties.SetBaseZoneProperties("oggok", "Oggok", -99f, -345f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(130, 140, 80, 10, 300);
                        zoneProperties.AddZoneLineBox("feerrott", 1652.742065f, 811.823181f, 57.281330f, ZoneLineOrientationType.South, -399.834625f, -77.776642f, 56.437752f, -462.005951f, -120.130768f, -0.500000f);
                    }
                    break;
                case "oot":
                    {
                        // TODO: Boat connecting east freeport and butcherblock
                        zoneProperties.SetBaseZoneProperties("oot", "Ocean of Tears", -9200f, 390f, 6f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;                
                case "paineel":
                    {
                        // TODO: Teleporters after zone collision is mapped
                        zoneProperties.SetBaseZoneProperties("paineel", "Paineel", 200f, 800f, 3.39f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(150, 150, 150, 200, 850);
                        zoneProperties.AddZoneLineBox("hole", 633.865723f, -942.076172f, -93.062523f, ZoneLineOrientationType.North, 640.945190f, -935.434082f, -87.500748f, 605.060547f, -947.819336f, -98.468681f);
                        zoneProperties.AddZoneLineBox("hole", 645.839417f, 246.516739f, -327.142517f, ZoneLineOrientationType.North, 932.554138f, 434.162994f, -151.438705f, 242.766006f, 88.558594f, -332.241425f);
                        zoneProperties.AddZoneLineBox("tox", -2592.465576f, -418.976410f, -45.092499f, ZoneLineOrientationType.North, 872.845337f, 187.732834f, 17.467010f, 831.879700f, 133.200150f, -1.499920f);
                        zoneProperties.AddZoneLineBox("warrens", 740.468933f, -881.437256f, -36.999771f, ZoneLineOrientationType.North, 748.881470f, -874.462463f, -26.586519f, 732.721863f, -888.849670f, -37.499989f);
                    }
                    break;
                case "paw":
                    {
                        zoneProperties.SetBaseZoneProperties("paw", "Lair of the Splitpaw", -7.9f, -79.3f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 25, 10, 10, 180);
                        zoneProperties.AddZoneLineBox("southkarana", -3118.534424f, 908.824341f, -11.938860f, ZoneLineOrientationType.West, -95.775307f, 64.159088f, 14.467540f, -112.163208f, 29.530199f, -0.499950f);
                    }
                    break;
                case "permafrost":
                    {
                        zoneProperties.SetBaseZoneProperties("permafrost", "Permafrost Caverns", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(25, 35, 45, 10, 180);
                        zoneProperties.AddZoneLineBox("everfrost", 2019.599976f, -7040.121094f, -63.843819f, ZoneLineOrientationType.West, -39.775318f, 172.344788f, 38.435791f, -80.162201f, 102.044090f, -0.499990f);
                    }
                    break;
                case "qcat":
                    {
                        // TODO: Secret pot to Tox
                        zoneProperties.SetBaseZoneProperties("qcat", "Qeynos Aqueduct System", -315f, 214f, -38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("qeynos2", 301.114655f, -161.613953f, -63.449379f, ZoneLineOrientationType.North, 1063.755981f, -41.776192f, 262.653015f, 1049.400269f, -56.161572f, 215f);
                        zoneProperties.AddZoneLineBox("qeynos2", 187.834518f, 341.638733f, -87.992813f, ZoneLineOrientationType.West, 895.849731f, 224.099167f, 61.134998f, 881.462830f, 209.713654f, 39.701309f);
                        zoneProperties.AddZoneLineBox("qeynos2", 175.706985f, 97.447029f, -41.968739f, ZoneLineOrientationType.West, 645.282104f, 140.744995f, -29.533369f, 629.556519f, 111.476059f, -42.468731f);
                        zoneProperties.AddZoneLineBox("qeynos", 273.965027f, -553.077881f, -107.459396f, ZoneLineOrientationType.West, 350.068390f, -167.744415f, 73.340729f, 335.681610f, -182.130219f, 42.217911f);
                        zoneProperties.AddZoneLineBox("qeynos", -147.118271f, -602.695679f, -27.999969f, ZoneLineOrientationType.West, 224.098785f, -251.712753f, -29.532080f, 209.713898f, -294.294037f, -42.468761f);
                        zoneProperties.AddZoneLineBox("qeynos", 174.606125f, -482.169281f, -81.858551f, ZoneLineOrientationType.North, 238.099686f, -55.775669f, 89.590897f, 223.712814f, -70.160507f, 55.560329f);
                        zoneProperties.AddZoneLineBox("qeynos", -188.236420f, 78.551971f, -91.583946f, ZoneLineOrientationType.West, -167.744095f, 314.996979f, -71.500740f, -182.130966f, 286.822327f, -84.468750f);
                        zoneProperties.AddDisabledMaterialCollisionByNames("t75_w1", "d_m0001", "t75_m0007", "t75_m0008");
                    }
                    break;
                case "qey2hh1":
                    {
                        zoneProperties.SetBaseZoneProperties("qey2hh1", "Western Plains of Karana", -638f, 12f, -4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("qeytoqrg", 1513.821777f, -2144.436768f, -4.343770f, ZoneLineOrientationType.West, 75.848831f, -542.542114f, 69.329170f, -31.425360f, -581.946533f, -7.499270f);
                        zoneProperties.AddZoneLineBox("northkarana", -4427.983887f, 3694.417480f, -69.281387f, ZoneLineOrientationType.East, -4419.286621f, -15945, 200.000000f, -5000.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4407.983887f, 3694.417480f, -69.280312f, ZoneLineOrientationType.East, -4399.286621f, -15945, 200.000000f, -4419.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4387.983887f, 3694.417480f, -69.281021f, ZoneLineOrientationType.East, -4379.286621f, -15945, 200.000000f, -4399.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4367.983887f, 3694.417480f, -69.281113f, ZoneLineOrientationType.East, -4359.286621f, -15945, 200.000000f, -4379.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4347.983887f, 3694.417480f, -69.281219f, ZoneLineOrientationType.East, -4339.286621f, -15945, 200.000000f, -4359.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4327.983887f, 3694.417480f, -69.280998f, ZoneLineOrientationType.East, -4319.286621f, -15945, 200.000000f, -4339.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4307.983887f, 3694.417480f, -69.281090f, ZoneLineOrientationType.East, -4299.286621f, -15945, 200.000000f, -4319.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4287.983887f, 3694.417480f, -69.281067f, ZoneLineOrientationType.East, -4279.286621f, -15945, 200.000000f, -4299.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4267.983887f, 3694.417480f, -69.281120f, ZoneLineOrientationType.East, -4259.286621f, -15945, 200.000000f, -4279.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4247.983887f, 3694.417480f, -69.281021f, ZoneLineOrientationType.East, -4239.286621f, -15945, 200.000000f, -4259.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4227.983887f, 3694.417480f, -65.794540f, ZoneLineOrientationType.East, -4219.286621f, -15945, 200.000000f, -4239.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4207.983887f, 3694.417480f, -61.161800f, ZoneLineOrientationType.East, -4199.286621f, -15945, 200.000000f, -4219.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4187.983887f, 3694.417480f, -56.529270f, ZoneLineOrientationType.East, -4179.286621f, -15945, 200.000000f, -4199.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4167.983887f, 3694.417480f, -51.893570f, ZoneLineOrientationType.East, -4159.286621f, -15945, 200.000000f, -4179.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4147.983887f, 3694.417480f, -47.257912f, ZoneLineOrientationType.East, -4139.286621f, -15945, 200.000000f, -4159.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4127.983887f, 3694.417480f, -42.622330f, ZoneLineOrientationType.East, -4119.286621f, -15945, 200.000000f, -4139.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4107.983887f, 3694.417480f, -37.987011f, ZoneLineOrientationType.East, -4099.286621f, -15945, 200.000000f, -4119.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4087.983887f, 3694.417480f, -33.355850f, ZoneLineOrientationType.East, -4079.286621f, -15945, 200.000000f, -4099.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4067.983887f, 3694.417480f, -28.725941f, ZoneLineOrientationType.East, -4059.286621f, -15945, 200.000000f, -4079.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4047.983887f, 3694.417480f, -24.095470f, ZoneLineOrientationType.East, -4039.286621f, -15945, 200.000000f, -4059.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4027.983887f, 3694.417480f, -19.465460f, ZoneLineOrientationType.East, -4019.286621f, -15945, 200.000000f, -4039.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -4007.983887f, 3694.417480f, -14.835540f, ZoneLineOrientationType.East, -3999.286621f, -15945, 200.000000f, -4019.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3987.983887f, 3694.417480f, -12.321260f, ZoneLineOrientationType.East, -3979.286621f, -15945, 200.000000f, -3999.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3967.983887f, 3694.417480f, -11.047800f, ZoneLineOrientationType.East, -3959.286621f, -15945, 200.000000f, -3979.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3947.983887f, 3694.417480f, -9.774160f, ZoneLineOrientationType.East, -3939.286621f, -15945, 200.000000f, -3959.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3927.983887f, 3694.417480f, -8.500100f, ZoneLineOrientationType.East, -3919.286621f, -15945, 200.000000f, -3939.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3907.983887f, 3694.417480f, -7.226280f, ZoneLineOrientationType.East, -3899.286621f, -15945, 200.000000f, -3919.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3887.983887f, 3694.417480f, -5.952450f, ZoneLineOrientationType.East, -3879.286621f, -15945, 200.000000f, -3899.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3867.983887f, 3694.417480f, -4.678950f, ZoneLineOrientationType.East, -3859.286621f, -15945, 200.000000f, -3879.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3847.983887f, 3694.417480f, -3.937270f, ZoneLineOrientationType.East, -3839.286621f, -15945, 200.000000f, -3859.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3827.983887f, 3694.417480f, -3.936940f, ZoneLineOrientationType.East, -3819.286621f, -15945, 200.000000f, -3839.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3807.983887f, 3694.417480f, -3.937120f, ZoneLineOrientationType.East, -3799.286621f, -15945, 200.000000f, -3819.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3787.983887f, 3694.417480f, -3.937180f, ZoneLineOrientationType.East, -3779.286621f, -15945, 200.000000f, -3799.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3767.983887f, 3694.417480f, -3.937140f, ZoneLineOrientationType.East, -3759.286621f, -15945, 200.000000f, -3779.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3747.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -3739.286621f, -15945, 200.000000f, -3759.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3727.983887f, 3694.417480f, -3.937550f, ZoneLineOrientationType.East, -3719.286621f, -15945, 200.000000f, -3739.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3707.983887f, 3694.417480f, -3.937300f, ZoneLineOrientationType.East, -3699.286621f, -15945, 200.000000f, -3719.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3687.983887f, 3694.417480f, -3.937290f, ZoneLineOrientationType.East, -3679.286621f, -15945, 200.000000f, -3699.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3667.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -3659.286621f, -15945, 200.000000f, -3679.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3647.983887f, 3694.417480f, -3.937010f, ZoneLineOrientationType.East, -3639.286621f, -15945, 200.000000f, -3659.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3627.983887f, 3694.417480f, -3.936940f, ZoneLineOrientationType.East, -3619.286621f, -15945, 200.000000f, -3639.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3607.983887f, 3694.417480f, -3.936890f, ZoneLineOrientationType.East, -3599.286621f, -15945, 200.000000f, -3619.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3587.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -3579.286621f, -15945, 200.000000f, -3599.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3567.983887f, 3694.417480f, -3.937030f, ZoneLineOrientationType.East, -3559.286621f, -15945, 200.000000f, -3579.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3547.983887f, 3694.417480f, -3.937020f, ZoneLineOrientationType.East, -3539.286621f, -15945, 200.000000f, -3559.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3527.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -3519.286621f, -15945, 200.000000f, -3539.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3507.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -3499.286621f, -15945, 200.000000f, -3519.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3487.983887f, 3694.417480f, -3.937550f, ZoneLineOrientationType.East, -3479.286621f, -15945, 200.000000f, -3499.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3467.983887f, 3694.417480f, -3.937290f, ZoneLineOrientationType.East, -3459.286621f, -15945, 200.000000f, -3479.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3447.983887f, 3694.417480f, -3.937330f, ZoneLineOrientationType.East, -3439.286621f, -15945, 200.000000f, -3459.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3427.983887f, 3694.417480f, -3.937460f, ZoneLineOrientationType.East, -3419.286621f, -15945, 200.000000f, -3439.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3407.983887f, 3694.417480f, -3.937550f, ZoneLineOrientationType.East, -3399.286621f, -15945, 200.000000f, -3419.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3387.983887f, 3694.417480f, -3.936840f, ZoneLineOrientationType.East, -3379.286621f, -15945, 200.000000f, -3399.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3367.983887f, 3694.417480f, -3.936900f, ZoneLineOrientationType.East, -3359.286621f, -15945, 200.000000f, -3379.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3347.983887f, 3694.417480f, -3.937120f, ZoneLineOrientationType.East, -3339.286621f, -15945, 200.000000f, -3359.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3327.983887f, 3694.417480f, -3.937370f, ZoneLineOrientationType.East, -3319.286621f, -15945, 200.000000f, -3339.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3307.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -3299.286621f, -15945, 200.000000f, -3319.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3287.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -3279.286621f, -15945, 200.000000f, -3299.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3267.983887f, 3694.417480f, -3.937400f, ZoneLineOrientationType.East, -3259.286621f, -15945, 200.000000f, -3279.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3247.983887f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -3239.286621f, -15945, 200.000000f, -3259.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3227.983887f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -3219.286621f, -15945, 200.000000f, -3239.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3207.983887f, 3694.417480f, -3.937120f, ZoneLineOrientationType.East, -3199.286621f, -15945, 200.000000f, -3219.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3187.983887f, 3694.417480f, -3.937070f, ZoneLineOrientationType.East, -3179.286621f, -15945, 200.000000f, -3199.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3167.983887f, 3694.417480f, -3.936950f, ZoneLineOrientationType.East, -3159.286621f, -15945, 200.000000f, -3179.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3147.983887f, 3694.417480f, -3.936900f, ZoneLineOrientationType.East, -3139.286621f, -15945, 200.000000f, -3159.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3127.983887f, 3694.417480f, -3.937130f, ZoneLineOrientationType.East, -3119.286621f, -15945, 200.000000f, -3139.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3107.983887f, 3694.417480f, -3.937430f, ZoneLineOrientationType.East, -3099.286621f, -15945, 200.000000f, -3119.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3087.983887f, 3694.417480f, -3.937240f, ZoneLineOrientationType.East, -3079.286621f, -15945, 200.000000f, -3099.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3067.983887f, 3694.417480f, -3.937390f, ZoneLineOrientationType.East, -3059.286621f, -15945, 200.000000f, -3079.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3047.983887f, 3694.417480f, -3.937480f, ZoneLineOrientationType.East, -3039.286621f, -15945, 200.000000f, -3059.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3027.983887f, 3694.417480f, -3.937290f, ZoneLineOrientationType.East, -3019.286621f, -15945, 200.000000f, -3039.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -3007.983887f, 3694.417480f, -3.937320f, ZoneLineOrientationType.East, -2999.286621f, -15945, 200.000000f, -3019.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2987.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -2979.286621f, -15945, 200.000000f, -2999.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2967.983887f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -2959.286621f, -15945, 200.000000f, -2979.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2947.983887f, 3694.417480f, -3.937080f, ZoneLineOrientationType.East, -2939.286621f, -15945, 200.000000f, -2959.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2927.983887f, 3694.417480f, -3.936950f, ZoneLineOrientationType.East, -2919.286621f, -15945, 200.000000f, -2939.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2907.983887f, 3694.417480f, -3.937220f, ZoneLineOrientationType.East, -2899.286621f, -15945, 200.000000f, -2919.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2887.983887f, 3694.417480f, -3.937040f, ZoneLineOrientationType.East, -2879.286621f, -15945, 200.000000f, -2899.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2867.983887f, 3694.417480f, -3.937370f, ZoneLineOrientationType.East, -2859.286621f, -15945, 200.000000f, -2879.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2847.983887f, 3694.417480f, -3.937370f, ZoneLineOrientationType.East, -2839.286621f, -15945, 200.000000f, -2859.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2827.983887f, 3694.417480f, -3.937520f, ZoneLineOrientationType.East, -2819.286621f, -15945, 200.000000f, -2839.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2807.983887f, 3694.417480f, -3.937270f, ZoneLineOrientationType.East, -2799.286621f, -15945, 200.000000f, -2819.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2787.983887f, 3694.417480f, -3.936910f, ZoneLineOrientationType.East, -2779.286621f, -15945, 200.000000f, -2799.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2767.983887f, 3694.417480f, -3.936900f, ZoneLineOrientationType.East, -2759.286621f, -15945, 200.000000f, -2779.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2747.983887f, 3694.417480f, -3.937180f, ZoneLineOrientationType.East, -2739.286621f, -15945, 200.000000f, -2759.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2727.983887f, 3694.417480f, -3.936850f, ZoneLineOrientationType.East, -2719.286621f, -15945, 200.000000f, -2739.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2707.983887f, 3694.417480f, -3.936840f, ZoneLineOrientationType.East, -2699.286621f, -15945, 200.000000f, -2719.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2687.983887f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -2679.286621f, -15945, 200.000000f, -2699.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2667.983887f, 3694.417480f, -3.937230f, ZoneLineOrientationType.East, -2659.286621f, -15945, 200.000000f, -2679.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2647.983887f, 3694.417480f, -3.937500f, ZoneLineOrientationType.East, -2639.286621f, -15945, 200.000000f, -2659.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2627.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -2619.286621f, -15945, 200.000000f, -2639.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2607.983887f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -2599.286621f, -15945, 200.000000f, -2619.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2587.983887f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -2579.286621f, -15945, 200.000000f, -2599.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2567.983887f, 3694.417480f, -3.937480f, ZoneLineOrientationType.East, -2559.286621f, -15945, 200.000000f, -2579.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2547.983887f, 3694.417480f, -3.937020f, ZoneLineOrientationType.East, -2539.286621f, -15945, 200.000000f, -2559.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2527.983887f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -2519.286621f, -15945, 200.000000f, -2539.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2507.983887f, 3694.417480f, -3.937090f, ZoneLineOrientationType.East, -2499.286621f, -15945, 200.000000f, -2519.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2487.983887f, 3694.417480f, -3.937150f, ZoneLineOrientationType.East, -2479.286621f, -15945, 200.000000f, -2499.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2467.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -2459.286621f, -15945, 200.000000f, -2479.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2447.983887f, 3694.417480f, -3.936880f, ZoneLineOrientationType.East, -2439.286621f, -15945, 200.000000f, -2459.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2427.983887f, 3694.417480f, -3.937530f, ZoneLineOrientationType.East, -2419.286621f, -15945, 200.000000f, -2439.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2407.983887f, 3694.417480f, -3.937440f, ZoneLineOrientationType.East, -2399.286621f, -15945, 200.000000f, -2419.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2387.983887f, 3694.417480f, -3.937330f, ZoneLineOrientationType.East, -2379.286621f, -15945, 200.000000f, -2399.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2367.983887f, 3694.417480f, -3.937510f, ZoneLineOrientationType.East, -2359.286621f, -15945, 200.000000f, -2379.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2347.983887f, 3694.417480f, -3.937520f, ZoneLineOrientationType.East, -2339.286621f, -15945, 200.000000f, -2359.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2327.983887f, 3694.417480f, -3.936880f, ZoneLineOrientationType.East, -2319.286621f, -15945, 200.000000f, -2339.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2307.983887f, 3694.417480f, -3.936950f, ZoneLineOrientationType.East, -2299.286621f, -15945, 200.000000f, -2319.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2287.983887f, 3694.417480f, -3.936900f, ZoneLineOrientationType.East, -2279.286621f, -15945, 200.000000f, -2299.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2267.983887f, 3694.417480f, -3.936950f, ZoneLineOrientationType.East, -2259.286621f, -15945, 200.000000f, -2279.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2247.983887f, 3694.417480f, -3.937140f, ZoneLineOrientationType.East, -2239.286621f, -15945, 200.000000f, -2259.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2227.983887f, 3694.417480f, -3.937400f, ZoneLineOrientationType.East, -2219.286621f, -15945, 200.000000f, -2239.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2207.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -2199.286621f, -15945, 200.000000f, -2219.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2187.983887f, 3694.417480f, -3.937420f, ZoneLineOrientationType.East, -2179.286621f, -15945, 200.000000f, -2199.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2167.983887f, 3694.417480f, -3.937430f, ZoneLineOrientationType.East, -2159.286621f, -15945, 200.000000f, -2179.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2147.983887f, 3694.417480f, -3.937040f, ZoneLineOrientationType.East, -2139.286621f, -15945, 200.000000f, -2159.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2127.983887f, 3694.417480f, -3.937440f, ZoneLineOrientationType.East, -2119.286621f, -15945, 200.000000f, -2139.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2107.983887f, 3694.417480f, -3.936790f, ZoneLineOrientationType.East, -2099.286621f, -15945, 200.000000f, -2119.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2087.983887f, 3694.417480f, -3.937320f, ZoneLineOrientationType.East, -2079.286621f, -15945, 200.000000f, -2099.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2067.983887f, 3694.417480f, -3.937410f, ZoneLineOrientationType.East, -2059.286621f, -15945, 200.000000f, -2079.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2047.983887f, 3694.417480f, -3.937510f, ZoneLineOrientationType.East, -2039.286499f, -15945, 200.000000f, -2059.286621f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2027.983887f, 3694.417480f, -3.936830f, ZoneLineOrientationType.East, -2019.286499f, -15945, 200.000000f, -2039.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -2007.983887f, 3694.417480f, -3.937360f, ZoneLineOrientationType.East, -1999.286499f, -15945, 200.000000f, -2019.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1987.983887f, 3694.417480f, -3.937040f, ZoneLineOrientationType.East, -1979.286499f, -15945, 200.000000f, -1999.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1967.983887f, 3694.417480f, -3.936880f, ZoneLineOrientationType.East, -1959.286499f, -15945, 200.000000f, -1979.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1947.983887f, 3694.417480f, -3.936950f, ZoneLineOrientationType.East, -1939.286499f, -15945, 200.000000f, -1959.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1927.983887f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -1919.286499f, -15945, 200.000000f, -1939.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1907.983887f, 3694.417480f, -3.937270f, ZoneLineOrientationType.East, -1899.286499f, -15945, 200.000000f, -1919.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1887.983887f, 3694.417480f, -3.937020f, ZoneLineOrientationType.East, -1879.286499f, -15945, 200.000000f, -1899.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1867.983887f, 3694.417480f, -3.936970f, ZoneLineOrientationType.East, -1859.286499f, -15945, 200.000000f, -1879.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1847.983887f, 3694.417480f, -3.937480f, ZoneLineOrientationType.East, -1839.286499f, -15945, 200.000000f, -1859.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1827.983887f, 3694.417480f, -3.937170f, ZoneLineOrientationType.East, -1819.286499f, -15945, 200.000000f, -1839.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1807.983887f, 3694.417480f, -3.937180f, ZoneLineOrientationType.East, -1799.286499f, -15945, 200.000000f, -1819.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1787.983887f, 3694.417480f, -3.936870f, ZoneLineOrientationType.East, -1779.286499f, -15945, 200.000000f, -1799.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1767.983887f, 3694.417480f, -3.937270f, ZoneLineOrientationType.East, -1759.286499f, -15945, 200.000000f, -1779.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1747.983887f, 3694.417480f, -3.937110f, ZoneLineOrientationType.East, -1739.286499f, -15945, 200.000000f, -1759.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1727.983887f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -1719.286499f, -15945, 200.000000f, -1739.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1707.983887f, 3694.417480f, -3.936880f, ZoneLineOrientationType.East, -1699.286499f, -15945, 200.000000f, -1719.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1687.983887f, 3694.417480f, -3.937400f, ZoneLineOrientationType.East, -1679.286499f, -15945, 200.000000f, -1699.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1667.983887f, 3694.417480f, -3.937150f, ZoneLineOrientationType.East, -1659.286499f, -15945, 200.000000f, -1679.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1647.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -1639.286499f, -15945, 200.000000f, -1659.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1627.983887f, 3694.417480f, -3.937220f, ZoneLineOrientationType.East, -1619.286499f, -15945, 200.000000f, -1639.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1607.983887f, 3694.417480f, -3.937340f, ZoneLineOrientationType.East, -1599.286499f, -15945, 200.000000f, -1619.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1587.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -1579.286499f, -15945, 200.000000f, -1599.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1567.983887f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -1559.286499f, -15945, 200.000000f, -1579.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1547.983887f, 3694.417480f, -3.937140f, ZoneLineOrientationType.East, -1539.286499f, -15945, 200.000000f, -1559.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1527.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -1519.286499f, -15945, 200.000000f, -1539.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1507.983887f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -1499.286499f, -15945, 200.000000f, -1519.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1487.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -1479.286499f, -15945, 200.000000f, -1499.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1467.983887f, 3694.417480f, -3.937020f, ZoneLineOrientationType.East, -1459.286499f, -15945, 200.000000f, -1479.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1447.983887f, 3694.417480f, -3.937130f, ZoneLineOrientationType.East, -1439.286499f, -15945, 200.000000f, -1459.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1427.983887f, 3694.417480f, -3.936890f, ZoneLineOrientationType.East, -1419.286499f, -15945, 200.000000f, -1439.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1407.983887f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -1399.286499f, -15945, 200.000000f, -1419.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1387.983887f, 3694.417480f, -3.937070f, ZoneLineOrientationType.East, -1379.286499f, -15945, 200.000000f, -1399.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1367.983887f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -1359.286499f, -15945, 200.000000f, -1379.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1347.983887f, 3694.417480f, -3.937090f, ZoneLineOrientationType.East, -1339.286499f, -15945, 200.000000f, -1359.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1327.983887f, 3694.417480f, -3.937160f, ZoneLineOrientationType.East, -1319.286499f, -15945, 200.000000f, -1339.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1307.983887f, 3694.417480f, -3.936970f, ZoneLineOrientationType.East, -1299.286499f, -15945, 200.000000f, -1319.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1287.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -1279.286499f, -15945, 200.000000f, -1299.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1267.983887f, 3694.417480f, -3.937220f, ZoneLineOrientationType.East, -1259.286499f, -15945, 200.000000f, -1279.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1247.983887f, 3694.417480f, -3.937440f, ZoneLineOrientationType.East, -1239.286499f, -15945, 200.000000f, -1259.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1227.983887f, 3694.417480f, -3.937220f, ZoneLineOrientationType.East, -1219.286499f, -15945, 200.000000f, -1239.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1207.983887f, 3694.417480f, -3.936990f, ZoneLineOrientationType.East, -1199.286499f, -15945, 200.000000f, -1219.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1187.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -1179.286499f, -15945, 200.000000f, -1199.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1167.983887f, 3694.417480f, -3.937250f, ZoneLineOrientationType.East, -1159.286499f, -15945, 200.000000f, -1179.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1147.983887f, 3694.417480f, -3.937060f, ZoneLineOrientationType.East, -1139.286499f, -15945, 200.000000f, -1159.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1127.983887f, 3694.417480f, -3.936930f, ZoneLineOrientationType.East, -1119.286499f, -15945, 200.000000f, -1139.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1107.983887f, 3694.417480f, -3.937260f, ZoneLineOrientationType.East, -1099.286499f, -15945, 200.000000f, -1119.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1087.983887f, 3694.417480f, -3.937020f, ZoneLineOrientationType.East, -1079.286499f, -15945, 200.000000f, -1099.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1067.983887f, 3694.417480f, -3.936880f, ZoneLineOrientationType.East, -1059.286499f, -15945, 200.000000f, -1079.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1047.983887f, 3694.417480f, -3.937170f, ZoneLineOrientationType.East, -1039.286499f, -15945, 200.000000f, -1059.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1027.983887f, 3694.417480f, -3.937430f, ZoneLineOrientationType.East, -1019.286499f, -15945, 200.000000f, -1039.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -1007.983948f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -999.286499f, -15945, 200.000000f, -1019.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -987.983948f, 3694.417480f, -3.937010f, ZoneLineOrientationType.East, -979.286499f, -15945, 200.000000f, -999.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -967.983948f, 3694.417480f, -3.937280f, ZoneLineOrientationType.East, -959.286499f, -15945, 200.000000f, -979.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -947.983948f, 3694.417480f, -3.937390f, ZoneLineOrientationType.East, -939.286499f, -15945, 200.000000f, -959.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -927.983948f, 3694.417480f, -3.937310f, ZoneLineOrientationType.East, -919.286499f, -15945, 200.000000f, -939.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -907.983948f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -899.286499f, -15945, 200.000000f, -919.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -887.983948f, 3694.417480f, -3.937160f, ZoneLineOrientationType.East, -879.286499f, -15945, 200.000000f, -899.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -867.983948f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -859.286499f, -15945, 200.000000f, -879.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -847.983948f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -839.286499f, -15945, 200.000000f, -859.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -827.983948f, 3694.417480f, -3.937350f, ZoneLineOrientationType.East, -819.286499f, -15945, 200.000000f, -839.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -807.983948f, 3694.417480f, -3.937220f, ZoneLineOrientationType.East, -799.286499f, -15945, 200.000000f, -819.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -787.983948f, 3694.417480f, -3.937120f, ZoneLineOrientationType.East, -779.286499f, -15945, 200.000000f, -799.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -767.983948f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -759.286499f, -15945, 200.000000f, -779.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -747.983948f, 3694.417480f, -3.937140f, ZoneLineOrientationType.East, -739.286499f, -15945, 200.000000f, -759.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -727.983948f, 3694.417480f, -3.936910f, ZoneLineOrientationType.East, -719.286499f, -15945, 200.000000f, -739.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -707.983948f, 3694.417480f, -3.937170f, ZoneLineOrientationType.East, -699.286499f, -15945, 200.000000f, -719.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -687.983948f, 3694.417480f, -3.937380f, ZoneLineOrientationType.East, -679.286499f, -15945, 200.000000f, -699.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -667.983887f, 3694.417480f, -3.937240f, ZoneLineOrientationType.East, -659.286499f, -15945, 200.000000f, -679.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -647.983887f, 3694.417480f, -3.937000f, ZoneLineOrientationType.East, -639.286499f, -15945, 200.000000f, -659.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -627.983887f, 3694.417480f, -3.937270f, ZoneLineOrientationType.East, -619.286499f, -15945, 200.000000f, -639.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -607.983887f, 3694.417480f, -3.937440f, ZoneLineOrientationType.East, -599.286499f, -15945, 200.000000f, -619.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -587.983887f, 3694.417480f, -3.937200f, ZoneLineOrientationType.East, -579.286499f, -15945, 200.000000f, -599.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -567.983887f, 3694.417480f, -3.936970f, ZoneLineOrientationType.East, -559.286499f, -15945, 200.000000f, -579.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -547.983887f, 3694.417480f, -3.937300f, ZoneLineOrientationType.East, -539.286499f, -15945, 200.000000f, -559.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -527.983887f, 3694.417480f, -3.937470f, ZoneLineOrientationType.East, -519.286499f, -15945, 200.000000f, -539.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -507.983887f, 3694.417480f, -3.936820f, ZoneLineOrientationType.East, -499.286469f, -15945, 200.000000f, -519.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -487.983887f, 3694.417480f, -3.937150f, ZoneLineOrientationType.East, -479.286469f, -15945, 200.000000f, -499.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -467.983887f, 3694.417480f, -3.937360f, ZoneLineOrientationType.East, -459.286469f, -15945, 200.000000f, -479.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -447.983887f, 3694.417480f, -3.826400f, ZoneLineOrientationType.East, -439.286469f, -15945, 200.000000f, -459.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -427.983887f, 3694.417480f, -2.500250f, ZoneLineOrientationType.East, -419.286469f, -15945, 200.000000f, -439.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -407.983887f, 3694.417480f, -1.174630f, ZoneLineOrientationType.East, -399.286469f, -15945, 200.000000f, -419.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -387.983887f, 3694.417480f, 0.150980f, ZoneLineOrientationType.East, -379.286469f, -15945, 200.000000f, -399.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -367.983887f, 3694.417480f, 1.476650f, ZoneLineOrientationType.East, -359.286469f, -15945, 200.000000f, -379.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -347.983887f, 3694.417480f, 2.802750f, ZoneLineOrientationType.East, -339.286469f, -15945, 200.000000f, -359.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -327.983887f, 3694.417480f, 4.128600f, ZoneLineOrientationType.East, -319.286499f, -15945, 200.000000f, -339.286469f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -307.983887f, 3694.417480f, 4.489570f, ZoneLineOrientationType.East, -299.286499f, -15945, 200.000000f, -319.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -287.983887f, 3694.417480f, 4.489320f, ZoneLineOrientationType.East, -279.286499f, -15945, 200.000000f, -299.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -267.983887f, 3694.417480f, 4.489390f, ZoneLineOrientationType.East, -259.286499f, -15945, 200.000000f, -279.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -247.983887f, 3694.417480f, 4.489580f, ZoneLineOrientationType.East, -239.286499f, -15945, 200.000000f, -259.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -227.983887f, 3694.417480f, 4.489460f, ZoneLineOrientationType.East, -219.286499f, -15945, 200.000000f, -239.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -207.983887f, 3694.417480f, 4.489200f, ZoneLineOrientationType.East, -199.286499f, -15945, 200.000000f, -219.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -187.983887f, 3694.417480f, 4.489550f, ZoneLineOrientationType.East, -179.286499f, -15945, 200.000000f, -199.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -167.983887f, 3694.417480f, 4.489290f, ZoneLineOrientationType.East, -159.286499f, -15945, 200.000000f, -179.286499f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -147.983887f, 3694.417480f, 4.489180f, ZoneLineOrientationType.East, -139.001923f, -15945, 200.000000f, -159.001923f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -127.983887f, 3694.417480f, 4.489320f, ZoneLineOrientationType.East, -119.001923f, -15945, 200.000000f, -139.001923f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -107.983887f, 3694.417480f, 4.489390f, ZoneLineOrientationType.East, -99.001923f, -15945, 200.000000f, -119.001923f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -87.983887f, 3694.417480f, 4.489280f, ZoneLineOrientationType.East, -79.001930f, -15945, 200.000000f, -99.001923f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -67.983887f, 3694.417480f, 4.489490f, ZoneLineOrientationType.East, -59.001930f, -15945, 200.000000f, -79.001930f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -47.983891f, 3694.417480f, 4.489500f, ZoneLineOrientationType.East, -39.001919f, -15945, 200.000000f, -59.001930f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -27.983891f, 3694.417480f, 4.489520f, ZoneLineOrientationType.East, -19.001921f, -15945, 200.000000f, -39.001919f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", -7.983890f, 3694.417480f, 4.489260f, ZoneLineOrientationType.East, 0.998080f, -15945, 200.000000f, -19.001921f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 12.016110f, 3694.417480f, 4.489850f, ZoneLineOrientationType.East, 20.998079f, -15945, 200.000000f, 0.998080f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 32.016109f, 3694.417480f, 4.489460f, ZoneLineOrientationType.East, 40.998081f, -15945, 200.000000f, 20.998079f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 52.016121f, 3694.417480f, 4.489540f, ZoneLineOrientationType.East, 60.998081f, -15945, 200.000000f, 40.998081f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 72.016121f, 3694.417480f, 4.489720f, ZoneLineOrientationType.East, 80.998077f, -15945, 200.000000f, 60.998081f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 92.016113f, 3694.417480f, 4.489790f, ZoneLineOrientationType.East, 100.998077f, -15945, 200.000000f, 80.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 112.016113f, 3694.417480f, 4.489450f, ZoneLineOrientationType.East, 120.998077f, -15945, 200.000000f, 100.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 132.016113f, 3694.417480f, 4.489280f, ZoneLineOrientationType.East, 140.998077f, -15945, 200.000000f, 120.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 152.016113f, 3694.417480f, 4.489520f, ZoneLineOrientationType.East, 160.998077f, -15945, 200.000000f, 140.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 172.016113f, 3694.417480f, 4.489210f, ZoneLineOrientationType.East, 180.998077f, -15945, 200.000000f, 160.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 192.016113f, 3694.417480f, 4.489490f, ZoneLineOrientationType.East, 200.998077f, -15945, 200.000000f, 180.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 212.016113f, 3694.417480f, 4.489530f, ZoneLineOrientationType.East, 220.998077f, -15945, 200.000000f, 200.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 232.016113f, 3694.417480f, 4.489440f, ZoneLineOrientationType.East, 240.998077f, -15945, 200.000000f, 220.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 252.016113f, 3694.417480f, 4.489230f, ZoneLineOrientationType.East, 260.998077f, -15945, 200.000000f, 240.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 272.016113f, 3694.417480f, 4.489370f, ZoneLineOrientationType.East, 280.998077f, -15945, 200.000000f, 260.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 292.016113f, 3694.417480f, 4.489200f, ZoneLineOrientationType.East, 300.998077f, -15945, 200.000000f, 280.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 312.016113f, 3694.417480f, 4.489360f, ZoneLineOrientationType.East, 320.998077f, -15945, 200.000000f, 300.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 332.016113f, 3694.417480f, 4.489490f, ZoneLineOrientationType.East, 340.998077f, -15945, 200.000000f, 320.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 352.016113f, 3694.417480f, 4.489260f, ZoneLineOrientationType.East, 360.998077f, -15945, 200.000000f, 340.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 372.016113f, 3694.417480f, 4.489400f, ZoneLineOrientationType.East, 380.998077f, -15945, 200.000000f, 360.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 392.016113f, 3694.417480f, 4.489530f, ZoneLineOrientationType.East, 400.998077f, -15945, 200.000000f, 380.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 412.016113f, 3694.417480f, 4.489520f, ZoneLineOrientationType.East, 420.998077f, -15945, 200.000000f, 400.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 432.016113f, 3694.417480f, 4.489260f, ZoneLineOrientationType.East, 440.998077f, -15945, 200.000000f, 420.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 452.016113f, 3694.417480f, 4.489410f, ZoneLineOrientationType.East, 460.998077f, -15945, 200.000000f, 440.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 472.016113f, 3694.417480f, 4.489590f, ZoneLineOrientationType.East, 480.998077f, -15945, 200.000000f, 460.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 492.016113f, 3694.417480f, 4.489770f, ZoneLineOrientationType.East, 500.998077f, -15945, 200.000000f, 480.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 512.016113f, 3694.417480f, 4.489390f, ZoneLineOrientationType.East, 520.998047f, -15945, 200.000000f, 500.998077f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 532.016113f, 3694.417480f, 4.489400f, ZoneLineOrientationType.East, 540.998047f, -15945, 200.000000f, 520.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 552.016113f, 3694.417480f, 4.489570f, ZoneLineOrientationType.East, 560.998047f, -15945, 200.000000f, 540.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 572.016113f, 3694.417480f, 4.489750f, ZoneLineOrientationType.East, 580.998047f, -15945, 200.000000f, 560.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 592.016113f, 3694.417480f, 4.489420f, ZoneLineOrientationType.East, 600.998047f, -15945, 200.000000f, 580.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 612.016113f, 3694.417480f, 4.489190f, ZoneLineOrientationType.East, 620.998047f, -15945, 200.000000f, 600.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 632.016113f, 3694.417480f, 4.489770f, ZoneLineOrientationType.East, 640.998047f, -15945, 200.000000f, 620.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 652.016113f, 3694.417480f, 4.489850f, ZoneLineOrientationType.East, 660.998047f, -15945, 200.000000f, 640.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 672.016052f, 3694.417480f, 4.489460f, ZoneLineOrientationType.East, 680.998108f, -15945, 200.000000f, 660.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 692.016052f, 3694.417480f, 4.489200f, ZoneLineOrientationType.East, 700.998108f, -15945, 200.000000f, 680.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 712.016052f, 3694.417480f, 4.489460f, ZoneLineOrientationType.East, 720.998108f, -15945, 200.000000f, 700.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 732.016052f, 3694.417480f, 4.489640f, ZoneLineOrientationType.East, 740.998108f, -15945, 200.000000f, 720.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 752.016052f, 3694.417480f, 4.489710f, ZoneLineOrientationType.East, 760.998108f, -15945, 200.000000f, 740.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 772.016052f, 3694.417480f, 4.489400f, ZoneLineOrientationType.East, 780.998108f, -15945, 200.000000f, 760.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 792.016052f, 3694.417480f, 4.489360f, ZoneLineOrientationType.East, 800.998108f, -15945, 200.000000f, 780.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 812.016052f, 3694.417480f, 4.489430f, ZoneLineOrientationType.East, 820.998108f, -15945, 200.000000f, 800.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 832.016052f, 3694.417480f, 4.489230f, ZoneLineOrientationType.East, 840.998108f, -15945, 200.000000f, 820.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 852.016052f, 3694.417480f, 4.489410f, ZoneLineOrientationType.East, 860.998108f, -15945, 200.000000f, 840.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 872.016052f, 3694.417480f, 4.489370f, ZoneLineOrientationType.East, 880.998108f, -15945, 200.000000f, 860.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 892.016052f, 3694.417480f, 4.489500f, ZoneLineOrientationType.East, 900.998108f, -15945, 200.000000f, 880.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 912.016052f, 3694.417480f, 4.489250f, ZoneLineOrientationType.East, 920.998108f, -15945, 200.000000f, 900.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 932.016052f, 3694.417480f, 4.489440f, ZoneLineOrientationType.East, 940.998108f, -15945, 200.000000f, 920.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 952.016052f, 3694.417480f, 4.489300f, ZoneLineOrientationType.East, 960.998108f, -15945, 200.000000f, 940.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 972.016052f, 3694.417480f, 4.489430f, ZoneLineOrientationType.East, 980.998108f, -15945, 200.000000f, 960.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 992.016052f, 3694.417480f, 4.489310f, ZoneLineOrientationType.East, 1000.998108f, -15945, 200.000000f, 980.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1012.016052f, 3694.417480f, 4.489410f, ZoneLineOrientationType.East, 1020.998108f, -15945, 200.000000f, 1000.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1032.016113f, 3694.417480f, 4.489210f, ZoneLineOrientationType.East, 1040.998047f, -15945, 200.000000f, 1020.998108f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1052.016113f, 3694.417480f, 4.489790f, ZoneLineOrientationType.East, 1060.998047f, -15945, 200.000000f, 1040.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1072.016113f, 3694.417480f, 4.489360f, ZoneLineOrientationType.East, 1080.998047f, -15945, 200.000000f, 1060.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1092.016113f, 3694.417480f, 4.489500f, ZoneLineOrientationType.East, 1100.998047f, -15945, 200.000000f, 1080.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1112.016113f, 3694.417480f, 9.215130f, ZoneLineOrientationType.East, 1120.998047f, -15945, 200.000000f, 1100.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1132.016113f, 3694.417480f, 16.373960f, ZoneLineOrientationType.East, 1140.998047f, -15945, 200.000000f, 1120.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1152.016113f, 3694.417480f, 23.532040f, ZoneLineOrientationType.East, 1160.998047f, -15945, 200.000000f, 1140.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1172.016113f, 3694.417480f, 30.690269f, ZoneLineOrientationType.East, 1180.998047f, -15945, 200.000000f, 1160.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1192.016113f, 3694.417480f, 37.848419f, ZoneLineOrientationType.East, 1200.998047f, -15945, 200.000000f, 1180.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1212.016113f, 3694.417480f, 45.007149f, ZoneLineOrientationType.East, 1220.998047f, -15945, 200.000000f, 1200.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1232.016113f, 3694.417480f, 52.132851f, ZoneLineOrientationType.East, 1240.998047f, -15945, 200.000000f, 1220.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1252.016113f, 3694.417480f, 59.185081f, ZoneLineOrientationType.East, 1260.998047f, -15945, 200.000000f, 1240.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1272.016113f, 3694.417480f, 66.236954f, ZoneLineOrientationType.East, 1280.998047f, -15945, 200.000000f, 1260.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1292.016113f, 3694.417480f, 73.288948f, ZoneLineOrientationType.East, 1300.998047f, -15945, 300.000000f, 1280.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1312.016113f, 3694.417480f, 80.340080f, ZoneLineOrientationType.East, 1320.998047f, -15945, 300.000000f, 1300.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1332.016113f, 3694.417480f, 87.391762f, ZoneLineOrientationType.East, 1340.998047f, -15945, 300.000000f, 1320.998047f, -15975, -100.000000f);
                        zoneProperties.AddZoneLineBox("northkarana", 1352.016113f, 3694.417480f, 94.442497f, ZoneLineOrientationType.East, 1650.998047f, -15945, 300.000000f, 1340.998047f, -15975, -100.000000f);
                    }
                    break;
                case "qeynos":
                    {
                        // TODO: Boat to Erudes Crossing
                        zoneProperties.SetBaseZoneProperties("qeynos", "South Qeynos", 186.46f, 14.29f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 210, 10, 450);
                        zoneProperties.AddZoneLineBox("qcat", 342.931549f, -174.301727f, 20.630989f, ZoneLineOrientationType.South, 280.068512f, -545.588013f, -130.403152f, 265.713104f, -559.974731f, -173.822586f);
                        zoneProperties.AddZoneLineBox("qcat", 215.878342f, -307.922394f, -41.968761f, ZoneLineOrientationType.East, -139.744003f, -621.613892f, -15.531000f, -156.270111f, -644.556519f, -28.499399f);
                        zoneProperties.AddZoneLineBox("qcat", 231.812836f, -63.370232f, 37.164181f, ZoneLineOrientationType.South, 182.130966f, -475.619812f, -112.237106f, 167.745056f, -490.005890f, -150.894775f);
                        zoneProperties.AddZoneLineBox("qcat", -175.662354f, 267.444336f, -77.394470f, ZoneLineOrientationType.East, -181.744064f, 46.723820f, -85.501877f, -196.098679f, 25.135241f, -98.468697f);
                        zoneProperties.AddZoneLineBox("qeynos2", -28.415890f, 357.134766f, -1.000000f, ZoneLineOrientationType.North, 602.557495f, -68.215889f, 18.468479f, 595.287170f, -84.163147f, -1.499980f);
                        zoneProperties.AddZoneLineBox("qeynos2", -28.292761f, 356.805267f, -1.000020f, ZoneLineOrientationType.South, 476.619080f, -430.347809f, 18.467279f, 468.250488f, -448.006866f, -1.499980f);
                    }
                    break;
                case "qeynos2":
                    {
                        // TODO: Teleporters after zone collision is implemented
                        zoneProperties.SetBaseZoneProperties("qeynos2", "North Qeynos", 114f, 678f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 450);
                        zoneProperties.AddZoneLineBox("qcat", 1056.423950f, -48.181938f, 199.885666f, ZoneLineOrientationType.South, 308.068420f, -153.744324f, -87.613121f, 293.681549f, -168.130386f, -126.259743f);
                        zoneProperties.AddZoneLineBox("qcat", 888.408264f, 216.199905f, 25.632490f, ZoneLineOrientationType.East, 196.099686f, 350.067566f, -106.426399f, 181.744537f, 335.681549f, -137.562592f);
                        zoneProperties.AddZoneLineBox("qcat", 636.627319f, 98.454399f, -41.968731f, ZoneLineOrientationType.East, 182.129868f, 77.711632f, -29.531000f, 167.744064f, 41.776329f, -42.468739f);
                        zoneProperties.AddZoneLineBox("qeynos", 587.979126f, -77.531120f, -0.999980f, ZoneLineOrientationType.South, -34.728909f, 364.036194f, 18.469000f, -56.161228f, 349.683868f, -1.498870f);
                        zoneProperties.AddZoneLineBox("qeynos", 461.973907f, -440.615234f, -0.999980f, ZoneLineOrientationType.South, -34.504589f, 364.036255f, 18.469000f, -56.161228f, 349.683929f, -1.499930f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 1099.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1109.153198f, 200.000000f, 1500.794556f, 1089.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 1079.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1089.153198f, 200.000000f, 1500.794556f, 1069.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 1059.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1069.153198f, 200.000000f, 1500.794556f, 1049.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 1039.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1049.153198f, 200.000000f, 1500.794556f, 1029.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 1019.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1029.153198f, 200.000000f, 1500.794556f, 1009.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 999.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 1009.153198f, 200.000000f, 1500.794556f, 989.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 979.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 989.153198f, 200.000000f, 1500.794556f, 969.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 959.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 969.153198f, 200.000000f, 1500.794556f, 949.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 939.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 949.153198f, 200.000000f, 1500.794556f, 929.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 919.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 929.153198f, 200.000000f, 1500.794556f, 909.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 899.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 909.153198f, 200.000000f, 1500.794556f, 889.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 879.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 889.153198f, 200.000000f, 1500.794556f, 869.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 859.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 869.153198f, 200.000000f, 1500.794556f, 849.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 839.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 849.153198f, 200.000000f, 1500.794556f, 829.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 819.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 829.153198f, 200.000000f, 1500.794556f, 809.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 799.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 809.153198f, 200.000000f, 1500.794556f, 789.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 779.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 789.153198f, 200.000000f, 1500.794556f, 769.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 759.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 769.153198f, 200.000000f, 1500.794556f, 749.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 739.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 749.153198f, 200.000000f, 1500.794556f, 729.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 719.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 729.153198f, 200.000000f, 1500.794556f, 709.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 699.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 709.153198f, 200.000000f, 1500.794556f, 689.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 679.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 689.153198f, 200.000000f, 1500.794556f, 669.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 659.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 669.153198f, 200.000000f, 1500.794556f, 649.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 639.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 649.153198f, 200.000000f, 1500.794556f, 629.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 619.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 629.153198f, 200.000000f, 1500.794556f, 609.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 599.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 609.153198f, 200.000000f, 1500.794556f, 589.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 579.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 589.153198f, 200.000000f, 1500.794556f, 569.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 559.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 569.153198f, 200.000000f, 1500.794556f, 549.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 539.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 549.153198f, 200.000000f, 1500.794556f, 529.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 519.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 529.153198f, 200.000000f, 1500.794556f, 509.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 499.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 509.153198f, 200.000000f, 1500.794556f, 489.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 479.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 489.153198f, 200.000000f, 1500.794556f, 469.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 459.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 469.153198f, 200.000000f, 1500.794556f, 449.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 439.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 449.153198f, 200.000000f, 1500.794556f, 429.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 419.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 429.153198f, 200.000000f, 1500.794556f, 409.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 399.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 409.153198f, 200.000000f, 1500.794556f, 389.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 379.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 389.153198f, 200.000000f, 1500.794556f, 369.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 359.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 369.153198f, 200.000000f, 1500.794556f, 349.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 339.592865f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 349.153198f, 200.000000f, 1500.794556f, 329.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 319.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 329.153198f, 200.000000f, 1500.794556f, 309.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 299.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 309.153198f, 200.000000f, 1500.794556f, 289.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 279.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 289.153198f, 200.000000f, 1500.794556f, 269.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 259.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 269.153198f, 200.000000f, 1500.794556f, 249.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 239.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 249.153198f, 200.000000f, 1500.794556f, 229.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 219.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 229.153198f, 200.000000f, 1500.794556f, 209.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 199.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 209.153198f, 200.000000f, 1500.794556f, 189.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 179.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 189.153198f, 200.000000f, 1500.794556f, 169.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 159.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 169.153198f, 200.000000f, 1500.794556f, 149.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 139.592896f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 149.153198f, 200.000000f, 1500.794556f, 129.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 119.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 129.153198f, 200.000000f, 1500.794556f, 109.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 99.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 109.153198f, 200.000000f, 1500.794556f, 89.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 79.592903f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 89.153198f, 200.000000f, 1500.794556f, 69.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 59.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 69.153198f, 200.000000f, 1500.794556f, 49.153198f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 39.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 49.153198f, 200.000000f, 1500.794556f, 29.153200f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, 19.592899f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 29.153200f, 200.000000f, 1500.794556f, 9.153200f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -0.407100f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, 9.153200f, 200.000000f, 1500.794556f, -10.846800f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -20.407110f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -10.846800f, 200.000000f, 1500.794556f, -30.846800f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -40.407108f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -30.846800f, 200.000000f, 1500.794556f, -50.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -60.407108f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -50.846802f, 200.000000f, 1500.794556f, -70.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -80.407112f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -70.846802f, 200.000000f, 1500.794556f, -90.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -100.407097f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -90.846802f, 200.000000f, 1500.794556f, -110.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -120.407097f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -110.846802f, 200.000000f, 1500.794556f, -130.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -140.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -130.846802f, 200.000000f, 1500.794556f, -150.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -160.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -150.846802f, 200.000000f, 1500.794556f, -170.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -180.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -170.846802f, 200.000000f, 1500.794556f, -190.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -200.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -190.846802f, 200.000000f, 1500.794556f, -210.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -220.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -210.846802f, 200.000000f, 1500.794556f, -230.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -240.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -230.846802f, 200.000000f, 1500.794556f, -250.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -260.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -250.846802f, 200.000000f, 1500.794556f, -270.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -280.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -270.846802f, 200.000000f, 1500.794556f, -290.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -300.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -290.846802f, 200.000000f, 1500.794556f, -310.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -320.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -310.846802f, 200.000000f, 1500.794556f, -330.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -340.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -330.846802f, 200.000000f, 1500.794556f, -350.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -360.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -350.846802f, 200.000000f, 1500.794556f, -370.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -380.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -370.846802f, 200.000000f, 1500.794556f, -390.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -400.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -390.846802f, 200.000000f, 1500.794556f, -410.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -420.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -410.846802f, 200.000000f, 1500.794556f, -430.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -440.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -430.846802f, 200.000000f, 1500.794556f, -450.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -460.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -450.846802f, 200.000000f, 1500.794556f, -470.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -480.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -470.846802f, 200.000000f, 1500.794556f, -490.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -500.407135f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -490.846802f, 200.000000f, 1500.794556f, -510.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -520.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -510.846802f, 200.000000f, 1500.794556f, -530.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -540.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -530.846802f, 200.000000f, 1500.794556f, -550.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -560.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -550.846802f, 200.000000f, 1500.794556f, -570.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -580.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -570.846802f, 200.000000f, 1500.794556f, -590.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -600.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -590.846802f, 200.000000f, 1500.794556f, -610.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -620.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -610.846802f, 200.000000f, 1500.794556f, -630.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -640.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -630.846802f, 200.000000f, 1500.794556f, -650.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -660.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -650.846802f, 200.000000f, 1500.794556f, -670.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -680.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -670.846802f, 200.000000f, 1500.794556f, -690.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -700.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -690.846802f, 200.000000f, 1500.794556f, -710.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -720.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -710.846802f, 200.000000f, 1500.794556f, -730.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -740.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -730.846802f, 200.000000f, 1500.794556f, -750.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -760.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -750.846802f, 200.000000f, 1500.794556f, -770.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -780.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -770.846802f, 200.000000f, 1500.794556f, -790.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -800.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -790.846802f, 200.000000f, 1500.794556f, -810.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -820.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -810.846802f, 200.000000f, 1500.794556f, -830.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -840.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -830.846802f, 200.000000f, 1500.794556f, -850.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -860.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -850.846802f, 200.000000f, 1500.794556f, -870.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -880.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -870.846802f, 200.000000f, 1500.794556f, -890.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -900.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -890.846802f, 200.000000f, 1500.794556f, -910.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -920.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -910.846802f, 200.000000f, 1500.794556f, -930.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -940.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -930.846802f, 200.000000f, 1500.794556f, -950.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -960.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -950.846802f, 200.000000f, 1500.794556f, -970.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -980.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -970.846802f, 200.000000f, 1500.794556f, -990.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1000.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -990.846802f, 200.000000f, 1500.794556f, -1010.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1020.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1010.846802f, 200.000000f, 1500.794556f, -1030.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1040.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1030.846802f, 200.000000f, 1500.794556f, -1050.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1060.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1050.846802f, 200.000000f, 1500.794556f, -1070.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1080.407104f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1070.846802f, 200.000000f, 1500.794556f, -1090.846802f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", -310.758850f, -1099f, -7.843740f, ZoneLineOrientationType.North, 1530.794556f, -1090.846802f, 200.000000f, 1500.794556f, -1150f, -100.000000f);                   
                    }
                    break;
                case "qeytoqrg":
                    {
                        zoneProperties.SetBaseZoneProperties("qeytoqrg", "Qeynos Hills", 196.7f, 5100.9f, -1f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("blackburrow", -163.06775f, 29.47728f, 0.000014f, ZoneLineOrientationType.West, 3442.5054f, -1124.6694f, 11.548047f, 3424.3691f, -1135.8118f, -0.4999545f);
                        zoneProperties.AddZoneLineBox("qey2hh1", 16.735029f, -634.390564f, -7.000000f, ZoneLineOrientationType.East, 1511.327637f, -2226.687500f, 67.544861f, 1317.927246f, -2336.197754f, -4.843780f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 1099.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1109.592896f, 200.000000f, -360.758850f, 1089.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 1079.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1089.592896f, 200.000000f, -360.758850f, 1069.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 1059.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1069.592896f, 200.000000f, -360.758850f, 1049.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 1039.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1049.592896f, 200.000000f, -360.758850f, 1029.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 1019.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1029.592896f, 200.000000f, -360.758850f, 1009.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 999.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 1009.592896f, 200.000000f, -360.758850f, 989.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 979.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 989.592896f, 200.000000f, -360.758850f, 969.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 959.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 969.592896f, 200.000000f, -360.758850f, 949.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 939.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 949.592896f, 200.000000f, -360.758850f, 929.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 919.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 929.592896f, 200.000000f, -360.758850f, 909.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 899.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 909.592896f, 200.000000f, -360.758850f, 889.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 879.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 889.592896f, 200.000000f, -360.758850f, 869.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 859.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 869.592896f, 200.000000f, -360.758850f, 849.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 839.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 849.592896f, 200.000000f, -360.758850f, 829.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 819.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 829.592896f, 200.000000f, -360.758850f, 809.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 799.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 809.592896f, 200.000000f, -360.758850f, 789.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 779.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 789.592896f, 200.000000f, -360.758850f, 769.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 759.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 769.592896f, 200.000000f, -360.758850f, 749.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 739.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 749.592896f, 200.000000f, -360.758850f, 729.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 719.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 729.592896f, 200.000000f, -360.758850f, 709.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 699.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 709.592896f, 200.000000f, -360.758850f, 689.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 679.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 689.592896f, 200.000000f, -360.758850f, 669.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 659.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 669.592896f, 200.000000f, -360.758850f, 649.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 639.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 649.592896f, 200.000000f, -360.758850f, 629.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 619.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 629.592896f, 200.000000f, -360.758850f, 609.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 599.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 609.592896f, 200.000000f, -360.758850f, 589.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 579.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 589.592896f, 200.000000f, -360.758850f, 569.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 559.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 569.592896f, 200.000000f, -360.758850f, 549.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 539.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 549.592896f, 200.000000f, -360.758850f, 529.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 519.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 529.592896f, 200.000000f, -360.758850f, 509.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 499.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 509.592865f, 200.000000f, -360.758850f, 489.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 479.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 489.592865f, 200.000000f, -360.758850f, 469.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 459.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 469.592865f, 200.000000f, -360.758850f, 449.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 439.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 449.592865f, 200.000000f, -360.758850f, 429.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 419.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 429.592865f, 200.000000f, -360.758850f, 409.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 399.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 409.592865f, 200.000000f, -360.758850f, 389.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 379.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 389.592865f, 200.000000f, -360.758850f, 369.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 359.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 369.592865f, 200.000000f, -360.758850f, 349.592865f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 339.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 349.592865f, 200.000000f, -360.758850f, 329.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 319.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 329.592896f, 200.000000f, -360.758850f, 309.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 299.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 309.592896f, 200.000000f, -360.758850f, 289.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 279.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 289.592896f, 200.000000f, -360.758850f, 269.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 259.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 269.592896f, 200.000000f, -360.758850f, 249.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 239.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 249.592896f, 200.000000f, -360.758850f, 229.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 219.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 229.592896f, 200.000000f, -360.758850f, 209.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 199.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 209.592896f, 200.000000f, -360.758850f, 189.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 179.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 189.592896f, 200.000000f, -360.758850f, 169.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 159.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 169.592896f, 200.000000f, -360.758850f, 149.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 139.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 149.592896f, 200.000000f, -360.758850f, 129.592896f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 119.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 129.592896f, 200.000000f, -360.758850f, 109.592903f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 99.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 109.592903f, 200.000000f, -360.758850f, 89.592903f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 79.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 89.592903f, 200.000000f, -360.758850f, 69.592903f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 59.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 69.592903f, 200.000000f, -360.758850f, 49.592899f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 39.153198f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 49.592899f, 200.000000f, -360.758850f, 29.592899f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, 19.153200f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 29.592899f, 200.000000f, -360.758850f, 9.592900f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -0.846800f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, 9.592900f, 200.000000f, -360.758850f, -10.407100f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -20.846800f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -10.407100f, 200.000000f, -360.758850f, -30.407110f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -40.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -30.407110f, 200.000000f, -360.758850f, -50.407108f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -60.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -50.407108f, 200.000000f, -360.758850f, -70.407112f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -80.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -70.407112f, 200.000000f, -360.758850f, -90.407097f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -100.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -90.407097f, 200.000000f, -360.758850f, -110.407097f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -120.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -110.407097f, 200.000000f, -360.758850f, -130.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -140.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -130.407104f, 200.000000f, -360.758850f, -150.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -160.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -150.407104f, 200.000000f, -360.758850f, -170.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -180.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -170.407104f, 200.000000f, -360.758850f, -190.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -200.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -190.407104f, 200.000000f, -360.758850f, -210.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -220.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -210.407104f, 200.000000f, -360.758850f, -230.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -240.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -230.407104f, 200.000000f, -360.758850f, -250.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -260.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -250.407104f, 200.000000f, -360.758850f, -270.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -280.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -270.407104f, 200.000000f, -360.758850f, -290.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -300.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -290.407104f, 200.000000f, -360.758850f, -310.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -320.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -310.407104f, 200.000000f, -360.758850f, -330.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -340.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -330.407104f, 200.000000f, -360.758850f, -350.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -360.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -350.407135f, 200.000000f, -360.758850f, -370.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -380.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -370.407135f, 200.000000f, -360.758850f, -390.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -400.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -390.407135f, 200.000000f, -360.758850f, -410.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -420.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -410.407135f, 200.000000f, -360.758850f, -430.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -440.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -430.407135f, 200.000000f, -360.758850f, -450.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -460.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -450.407135f, 200.000000f, -360.758850f, -470.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -480.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -470.407135f, 200.000000f, -360.758850f, -490.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -500.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -490.407135f, 200.000000f, -360.758850f, -510.407135f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -520.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -510.407135f, 200.000000f, -360.758850f, -530.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -540.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -530.407104f, 200.000000f, -360.758850f, -550.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -560.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -550.407104f, 200.000000f, -360.758850f, -570.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -580.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -570.407104f, 200.000000f, -360.758850f, -590.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -600.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -590.407104f, 200.000000f, -360.758850f, -610.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -620.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -610.407104f, 200.000000f, -360.758850f, -630.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -640.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -630.407104f, 200.000000f, -360.758850f, -650.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -660.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -650.407104f, 200.000000f, -360.758850f, -670.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -680.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -670.407104f, 200.000000f, -360.758850f, -690.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -700.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -690.407104f, 200.000000f, -360.758850f, -710.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -720.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -710.407104f, 200.000000f, -360.758850f, -730.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -740.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -730.407104f, 200.000000f, -360.758850f, -750.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -760.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -750.407104f, 200.000000f, -360.758850f, -770.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -780.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -770.407104f, 200.000000f, -360.758850f, -790.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -800.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -790.407104f, 200.000000f, -360.758850f, -810.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -820.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -810.407104f, 200.000000f, -360.758850f, -830.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -840.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -830.407104f, 200.000000f, -360.758850f, -850.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -860.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -850.407104f, 200.000000f, -360.758850f, -870.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -880.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -870.407104f, 200.000000f, -360.758850f, -890.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -900.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -890.407104f, 200.000000f, -360.758850f, -910.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -920.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -910.407104f, 200.000000f, -360.758850f, -930.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -940.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -930.407104f, 200.000000f, -360.758850f, -950.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -960.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -950.407104f, 200.000000f, -360.758850f, -970.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -980.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -970.407104f, 200.000000f, -360.758850f, -990.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1000.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -990.407104f, 200.000000f, -360.758850f, -1010.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1020.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1010.407104f, 200.000000f, -360.758850f, -1030.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1040.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1030.407104f, 200.000000f, -360.758850f, -1050.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1060.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1050.407104f, 200.000000f, -360.758850f, -1070.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1080.846802f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1070.407104f, 200.000000f, -360.758850f, -1090.407104f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qeynos2", 1480.794556f, -1099f, 0.000010f, ZoneLineOrientationType.South, -330.758850f, -1090.407104f, 200.000000f, -360.758850f, -1150f, -100.000000f);
                        zoneProperties.AddZoneLineBox("qrg", -631.004761f, 137.129745f, 0.000030f, ZoneLineOrientationType.East, 5189.661133f, 143.432114f, 7.875250f, 5173.275391f, 103.197861f, -7.093250f);
                    }
                    break;
                case "qrg":
                    {
                        zoneProperties.SetBaseZoneProperties("qrg", "Surefall Glade", 136.9f, -65.9f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 175, 183, 10, 450);
                        zoneProperties.AddZoneLineBox("qeytoqrg", 5180.557617f, 161.911987f, -6.594880f, ZoneLineOrientationType.West, -623.557495f, 168.640945f, 0.500030f, -639.942505f, 150.659027f, -0.499970f);
                    }
                    break;
                case "rathemtn":
                    {
                        zoneProperties.SetBaseZoneProperties("rathemtn", "Rathe Mountains", 1831f, 3825f, 29.03f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("feerrott", 313.833893f, 3388.136230f, 0.000060f, ZoneLineOrientationType.South, 607.850098f, -3069.240234f, 77.677612f, 564.785278f, -3162.108887f, -0.499980f);
                        zoneProperties.AddZoneLineBox("lakerathe", 2707.150635f, -2236.831299f, 1.750170f, ZoneLineOrientationType.North, 3495.903564f, 2999.350830f, 72.285400f, 3401.946777f, 2973.988281f, -4.374810f);
                    }
                    break;
                case "rivervale":
                    {
                        zoneProperties.SetBaseZoneProperties("rivervale", "Rivervale", 45.3f, 1.6f, 3.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 210, 200, 10, 400);
                        zoneProperties.AddZoneLineBox("kithicor", 2012.985229f, 3825.189209f, 462.250427f, ZoneLineOrientationType.South, -384.065887f, -275.682556f, 22.469000f, -396.650330f, -290.013977f, -0.499910f);
                        zoneProperties.AddZoneLineBox("misty", 407.486847f, -2571.641357f, -10.749720f, ZoneLineOrientationType.West, -69.729698f, 134.790482f, 22.466999f, -96.162209f, 113.427109f, -0.500000f);
                    }
                    break;
                case "runnyeye":
                    {
                        zoneProperties.SetBaseZoneProperties("runnyeye", "Runnyeye Citadel", -21.85f, -108.88f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(75, 150, 25, 10, 600);
                        zoneProperties.AddZoneLineBox("beholder", 903.2041f, -1850.1808f, 1.0001143f, ZoneLineOrientationType.West, -102.775955f, 12.901143f, 15.468005f, -119.129944f, -8.304958f, -0.49999338f);
                        zoneProperties.AddZoneLineBox("misty", -816.631531f, 1427.580444f, -10.751390f, ZoneLineOrientationType.North, 271.099457f, 170f, 15.469000f, 250.497299f, 135.744324f, 0.501060f);
                    }
                    break;
                case "soldunga":
                    {
                        zoneProperties.SetBaseZoneProperties("soldunga", "Solusek's Eye", -485.77f, -476.04f, 73.72f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 100);
                        zoneProperties.AddZoneLineBox("lavastorm", 792.794373f, 226.540787f, 127.062599f, ZoneLineOrientationType.North, -429.005951f, -518.254517f, 82.437752f, -440.369812f, -529.974792f, 69.468758f); // Works
                        zoneProperties.AddZoneLineBox("soldungb", -165.640060f, -595.953247f, 14.000010f, ZoneLineOrientationType.East, -158.745377f, -582.988464f, 25.937571f, -173.130524f, -600.847412f, 13.500000f);
                        zoneProperties.AddZoneLineBox("soldungb", -275.436981f, -507.896454f, 22.000071f, ZoneLineOrientationType.North, -267.713684f, -499.620789f, 32.469002f, -286.491913f, -514.974121f, 21.500681f);
                        zoneProperties.AddZoneLineBox("soldungb", -364.385254f, -406.963348f, 8.000610f, ZoneLineOrientationType.East, -357.650269f, -400.866211f, 18.469000f, -377.148956f, -409.100372f, 6.500000f);
                        zoneProperties.AddZoneLineBox("soldungb", -1081.872803f, -525.067810f, -3.999330f, ZoneLineOrientationType.West, -1068.534180f, -521.974548f, 8.468000f, -1087.309937f, -541.027771f, -4.500000f);
                    }
                    break;
                case "soldungb":
                    {
                        zoneProperties.SetBaseZoneProperties("soldungb", "Nagafen's Lair", -262.7f, -423.99f, -108.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 350);
                        zoneProperties.AddZoneLineBox("lavastorm", 909.788574f, 484.493713f, 51.688461f, ZoneLineOrientationType.North, -399.037048f, -259.033051f, -101.499748f, -410.349213f, -270.100739f, -112.467888f);
                        zoneProperties.AddZoneLineBox("soldunga", -166.265717f, -572.437744f, 15.365680f, ZoneLineOrientationType.West, -158.744904f, -540.487061f, 35.468010f, -173.130875f, -581.914795f, 13.500010f);
                        zoneProperties.AddZoneLineBox("soldunga", -299.452057f, -508.939087f, 22.000681f, ZoneLineOrientationType.South, -285.998016f, -499.999878f, 32.469002f, -300.556580f, -514.974731f, 21.500320f);
                        zoneProperties.AddZoneLineBox("soldunga", -364.548492f, -391.234467f, 8.000000f, ZoneLineOrientationType.West, -350.692047f, -379.634491f, 19.469000f, -371.417236f, -399.513031f, 7.500610f);
                        zoneProperties.AddZoneLineBox("soldunga", -1082.416138f, -549.995605f, -4.000000f, ZoneLineOrientationType.East, -1076.369019f, -535.692444f, 15.149850f, -1089.673218f, -559.933105f, -4.499780f);
                    }
                    break;
                case "soltemple":
                    {
                        zoneProperties.SetBaseZoneProperties("soltemple", "Temple of Solusek Ro", 7.5f, 268.8f, 3f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 5, 5, 50, 500);
                        zoneProperties.AddZoneLineBox("lavastorm", 1346.515381f, 330.955505f, 146.188034f, ZoneLineOrientationType.South, 244.129364f, 62.161572f, 9.468010f, 219.713821f, 44.408550f, -1.500000f);
                    }
                    break;
                case "southkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("southkarana", "Southern Plains of Karana", 1293.66f, 2346.69f, -5.77f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("lakerathe", 4352.154297f, 1158.142578f, -0.000990f, ZoneLineOrientationType.South, -8555.652344f, 1180.041138f, 43.965542f, -8569.452148f, 1132.577637f, -0.499510f);
                        zoneProperties.AddZoneLineBox("northkarana", -4472.277344f, 1208.014893f, -34.406212f, ZoneLineOrientationType.North, 2900.742432f, 943.823730f, 17.628691f, 2821.058350f, 862.661682f, -36.353588f);
                        zoneProperties.AddZoneLineBox("paw", -103.683167f, 16.824860f, 0.000050f, ZoneLineOrientationType.East, -3110.107910f, 895.748901f, 2.515520f, -3126.174805f, 861.375854f, -12.438860f);
                    }
                    break;
                case "sro":
                    {
                        zoneProperties.SetBaseZoneProperties("sro", "Southern Desert of Ro", 286f, 1265f, 79f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                        zoneProperties.AddZoneLineBox("innothule", 2537.843262f, 1157.335449f, -28.670191f, ZoneLineOrientationType.South, -3172.916504f, 1030f, 38.835121f, -3225.501709f, 1057.282593f, -30f);
                        zoneProperties.AddZoneLineBox("oasis", -1859.231567f, 182.460098f, 2.406740f, ZoneLineOrientationType.North, 1526.327637f, 9.256500f, 131.793716f, 1478.424438f, 292.955048f, 1.148580f);
                    }
                    break;
                case "steamfont":
                    {
                        zoneProperties.SetBaseZoneProperties("steamfont", "Steamfont Mountains", -272.86f, 159.86f, -21.4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("akanon", 57.052101f, -77.213501f, 0.000010f, ZoneLineOrientationType.South, -2064.9805f, 535.8183f, -98.656f, -2077.9038f, 521.43134f, -111.624886f);
                        zoneProperties.AddZoneLineBox("lfaydark", 930.675537f, -2166.410400f, -4.781320f, ZoneLineOrientationType.West, 608.013672f, 2214.515625f, 26.767950f, 559.319153f, 2202.571045f, -113.749878f);
                    }
                    break;
                case "stonebrunt":
                    {
                        zoneProperties.SetBaseZoneProperties("stonebrunt", "Stonebrunt Mountains", -1643.01f, -3427.84f, -6.57f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(235, 235, 235, 10, 800);
                        zoneProperties.AddZoneLineBox("warrens", -100.582893f, 1145.348877f, -110.968758f, ZoneLineOrientationType.North, -3674.369385f, 2932.535400f, -22.218500f, -3707.896240f, 2908.150146f, -40.187389f);
                    }
                    break;                
                case "tox":
                    {
                        zoneProperties.SetBaseZoneProperties("tox", "Toxxulia Forest", 203f, 2295f, -45f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 200, 30, 50, 250);
                        zoneProperties.AddZoneLineBox("erudnext", -1552.149292f, -184.036606f, -47.968700f, ZoneLineOrientationType.North, 2574.356934f, 305.599121f, -33.937248f, 2550.955078f, 289.213013f, -48.907711f);
                        zoneProperties.AddZoneLineBox("kerraridge", 416.010834f, -930.879211f, 20.000179f, ZoneLineOrientationType.West, -495.140961f, 2684.400635f, -19.784010f, -527.409973f, 2655.238281f, -38.749310f);
                        zoneProperties.AddZoneLineBox("paineel", 852.573181f, 196.109207f, 0.000050f, ZoneLineOrientationType.West, -2613.365479f, -417.686676f, -26.624750f, -2628.005371f, -470f, -45.593510f);
                    }
                    break;
                case "unrest":
                    {
                        zoneProperties.SetBaseZoneProperties("unrest", "Estate of Unrest", 52f, -38f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(40, 10, 60, 10, 300);
                        zoneProperties.AddZoneLineBox("cauldron", -2014.301880f, -627.332886f, 90.001083f, ZoneLineOrientationType.North, 113.163170f, 340.068451f, 18.469000f, 72.315872f, 319.681549f, -0.500000f);
                    }
                    break;
                case "warrens":
                    {
                        zoneProperties.SetBaseZoneProperties("warrens", "The Warrens", -930f, 748f, -37.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                        zoneProperties.AddZoneLineBox("paineel", 721.470337f, -881.333069f, -36.999989f, ZoneLineOrientationType.South, 734.975342f, -874.463745f, -26.531000f, 713.524292f, -888.849182f, -37.499908f);
                        zoneProperties.AddZoneLineBox("stonebrunt", -3720.441895f, 2921.923096f, -39.687389f, ZoneLineOrientationType.South, -119.277092f, 1159.723511f, -93.500740f, -145.652954f, 1130f, -111.468353f);
                    }
                    break;
                //--------------------------------------------------------------------------------------------------------------------------
                // Kunark Zones
                //--------------------------------------------------------------------------------------------------------------------------
                case "burningwood":
                    {
                        zoneProperties.SetBaseZoneProperties("burningwood", "Burning Wood", -820f, -4942f, 200.31f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 60, 400);
                        zoneProperties.AddZoneLineBox("skyfire", -5439.923f, 1772.0016f, -162.23752f, ZoneLineOrientationType.North,
                            5302.864258f, 1900f, -110.222794f, 5118.025879f, 1400.658936f, -170.772964f);
                        zoneProperties.AddZoneLineBox("chardok", -2.107779f, 858.89545f, 99.968796f, ZoneLineOrientationType.North,
                            7339.295f, -4128.179f, -197.4685f, 7297.6826f, -4168.443f, -236.43733f);
                        zoneProperties.AddZoneLineBox("frontiermtns", -2404.3184f, 4043.9534f, -457.12997f, ZoneLineOrientationType.East,
                            -2845.6619f, -4620.881f, 5.9380794f, -2976.7217f, -4548.8438f, -48.21263f);
                        zoneProperties.AddZoneLineBox("dreadlands", 2797.4214f, -814.58563f, 195.71893f, ZoneLineOrientationType.South,
                            -4431.6133f, -509.9672f, 457.6962f, -4849.6196f, -1052.6932f, 176.06267f);
                    }
                    break;
                case "cabeast":
                    {
                        zoneProperties.SetBaseZoneProperties("cabeast", "Cabilis East", -416f, 1343f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                        zoneProperties.AddZoneLineBox("fieldofbone", -2557.7278f, 3688.0273f, 4.093815f, ZoneLineOrientationType.East,
                            1377.6309f, -455.81412f, 97.201485f, 1346.7754f, -497.1183f, -0.49989557f);
                        zoneProperties.AddZoneLineBox("fieldofbone", -2747.7383f, 3530.195f, 4.093984f, ZoneLineOrientationType.North,
                            1236.0558f, -605.5564f, 128.95297f, 1192.7297f, -635.9432f, -0.4994932f);
                        zoneProperties.AddZoneLineBox("cabwest", -13.886450f, 314.975342f, 0.000000f, ZoneLineOrientationType.North,
                            -3.434140f, 322.059662f, 12.469000f, -21.590590f, 307.681549f, -0.499970f);
                        zoneProperties.AddZoneLineBox("cabwest", -13.976320f, 338.086029f, -24.860001f, ZoneLineOrientationType.North,
                            -6.287930f, 350.279877f, 12.469000f, -18.972679f, 321.680542f, -42.468731f);
                        zoneProperties.AddZoneLineBox("cabwest", -14.334330f, 371.205414f, 0.000030f, ZoneLineOrientationType.North,
                            -13.192510f, 378.441284f, 12.469000f, -21.719170f, 349.526428f, -0.499940f);
                        zoneProperties.AddZoneLineBox("swampofnohope", 3122.768066f, 3056.380127f, 0.125070f, ZoneLineOrientationType.South,
                            -111.601692f, -643.556580f, 77.611252f, -154.608215f, -673.912231f, -0.499930f);
                        zoneProperties.AddZoneLineBox("swampofnohope", 2972.800537f, 3241.029053f, 0.124830f, ZoneLineOrientationType.East,
                            -296.067413f, -489.889130f, 72.299011f, -267.712738f, -533.045593f, -0.499910f);
                    }
                    break;
                case "cabwest":
                    {
                        zoneProperties.SetBaseZoneProperties("cabwest", "Cabilis West", 790f, 165f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(150, 120, 80, 40, 300);
                        zoneProperties.AddZoneLineBox("cabeast", -28.278749f, 314.920990f, 0.000030f, ZoneLineOrientationType.South,
                            -20.735310f, 322.030548f, 12.469000f, -33.827209f, 302.649109f, -0.499990f);
                        zoneProperties.AddZoneLineBox("cabeast", -28.944679f, 335.877106f, -24.860720f, ZoneLineOrientationType.South,
                            -20.975170f, 350.067322f, 12.469000f, -41.966270f, 321.681580f, -42.468739f);
                        zoneProperties.AddZoneLineBox("cabeast", -28.406759f, 357.039429f, 0.000260f, ZoneLineOrientationType.South,
                            -27.676720f, 364.034607f, 12.469000f, -49.269180f, 349.616089f, -0.500000f);
                        zoneProperties.AddZoneLineBox("warslikswood", -2253.605225f, -1121.567871f, 262.812622f, ZoneLineOrientationType.West,
                            887.849365f, 1192.889526f, 64.138229f, 857.462646f, 1153.048340f, -0.499980f);
                        zoneProperties.AddZoneLineBox("warslikswood", -2410.033447f, -934.157043f, 262.812653f, ZoneLineOrientationType.North,
                            739.584961f, 1343.662231f, 99.151367f, 698.854492f, 1313.275391f, -0.499970f);
                        zoneProperties.AddZoneLineBox("lakeofillomen", 6520.699707f, -6630.659180f, 35.093719f, ZoneLineOrientationType.South,
                            -810.963440f, 783.879944f, 129.847549f, -860.993652f, 753.494934f, -0.500040f);
                        zoneProperties.AddZoneLineBox("lakeofillomen", 6331.367676f, -6786.975586f, 35.093800f, ZoneLineOrientationType.West,
                            -971.431702f, 642.097107f, 166.406494f, -1001.788269f, 595.321289f, -0.499620f);
                    }
                    break;
                case "charasis":
                    {
                        zoneProperties.SetBaseZoneProperties("charasis", "Howling Stones", 0f, 0f, -4.25f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(160, 180, 200, 50, 400);
                        zoneProperties.AddZoneLineBox("overthere", -83.674156f, 825.339172f, -506.624969f, ZoneLineOrientationType.East, // Consider moving to the stone?
                            -2.369990f, -722.148010f, 34.469002f, -15.200140f, -734.827942f, 17.500111f);
                    }
                    break;
                case "chardok":
                    {
                        zoneProperties.SetBaseZoneProperties("chardok", "Chardok", 859f, 119f, 106f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 53, 6, 30, 300);
                        zoneProperties.AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                            -20.012981f, 879.84973f, 137.60643f, -70.907234f, 839.5071f, 99.46923f);
                        zoneProperties.AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                            220.71272f, 895.73254f, 138.4065f, 157.77734f, 839.54913f, 99.468735f);
                    }
                    break;
                case "citymist":
                    {
                        // TODO: Any in-zone teleports?
                        zoneProperties.SetBaseZoneProperties("citymist", "City of Mist", -734f, 28f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 110, 60, 50, 275);
                        zoneProperties.AddZoneLineBox("emeraldjungle", 0.121500f, -774.691650f, 0.000000f, ZoneLineOrientationType.West,
                            309.691193f, -1730.243408f, -300.343506f, 291.030334f, -1789.959473f, -335.468658f);
                    }
                    break;
                case "dalnir":
                    {
                        zoneProperties.SetBaseZoneProperties("dalnir", "Dalnir", 90f, 8f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 25, 30, 210);
                    }
                    break;
                case "dreadlands":
                    {
                        zoneProperties.SetBaseZoneProperties("dreadlands", "Dreadlands", 9565.05f, 2806.04f, 1045.2f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 600);
                        zoneProperties.AddZoneLineBox("burningwood", -4247.9624f, -712.7452f, 245.30704f, ZoneLineOrientationType.North,
                            3057.91f, -414.8485f, 319.16867f, 2988.2588f, -1083.3096f, 240.4023f);
                    }
                    break;
                case "droga":
                    {
                        zoneProperties.SetBaseZoneProperties("droga", "Temple of Droga", 294.11f, 1371.43f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "emeraldjungle":
                    {
                        zoneProperties.SetBaseZoneProperties("emeraldjungle", "Emerald Jungle", 4648.06f, -1222.97f, 0f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(200, 235, 210, 60, 200);
                        zoneProperties.AddZoneLineBox("citymist", 300.490265f, -1799.661743f, -334.968658f, ZoneLineOrientationType.East,
                            10.193290f, -783.147522f, 34.308090f, -10.191010f, -844.774231f, -0.500000f);
                    }
                    break;
                case "fieldofbone":
                    {
                        zoneProperties.SetBaseZoneProperties("fieldofbone", "Field of Bone", 1617f, -1684f, -54.78f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("cabeast", 1359.3015f, -435.72766f, 0.000174f, ZoneLineOrientationType.West,
                            -2541.8613f, 3747.162f, 50.543545f, -2570.4119f, 3699.717f, 3.5938148f);
                        zoneProperties.AddZoneLineBox("cabeast", 1179.1279f, -619.062f, 0.000174f, ZoneLineOrientationType.South,
                            -2768.011f, 3545.4978f, 86.73899f, -2829.281f, 3514.2957f, 3.5937567f);
                    }
                    break;
                case "firiona":
                    {
                        zoneProperties.SetBaseZoneProperties("firiona", "Firiona Vie", 1439.96f, -2392.06f, -2.65f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "frontiermtns":
                    {
                        zoneProperties.SetBaseZoneProperties("frontiermtns", "Frontier Mountains", -4262f, -633f, 113.24f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("burningwood", -2965.3167f, -4515.809f, -51.462868f, ZoneLineOrientationType.West,
                            -2312.331f, 4184.5947f, -433.798f, -2418.7312f, 4063.2607f, -472.19543f);
                    }
                    break;
                case "kaesora":
                    {
                        zoneProperties.SetBaseZoneProperties("kaesora", "Kaesora", 40f, 370f, 99.72f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 10, 0, 20, 200);
                    }
                    break;
                case "karnor":
                    {
                        zoneProperties.SetBaseZoneProperties("karnor", "Karnor's Castle", 0f, 0f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 20, 20, 10, 350);
                    }
                    break;
                case "kurn":
                    {
                        zoneProperties.SetBaseZoneProperties("kurn", "Kurn's Tower", 77.72f, -277.64f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 50, 20, 10, 200);
                    }
                    break;
                case "lakeofillomen":
                    {
                        zoneProperties.SetBaseZoneProperties("lakeofillomen", "Lake of Ill Omen", -5383.07f, 5747.14f, 68.27f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("cabwest", -802.654480f, 767.458740f, -0.000070f, ZoneLineOrientationType.North,
                            6577.715820f, -6613.837891f, 145.213730f, 6533.130859f, -6645.066895f, 34.593719f);
                        zoneProperties.AddZoneLineBox("cabwest", -985.943787f, 584.806458f, 0.000380f, ZoneLineOrientationType.East,
                            6344.193848f, -6799.043945f, 182.103806f, 6315.685547f, -6843.227051f, 34.595600f);
                    }
                    break;
                case "nurga":
                    {
                        zoneProperties.SetBaseZoneProperties("nurga", "Mines of Nurga", 150f, -1062f, -107f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "overthere":
                    {
                        // TODO: There's a clicky teleport to Chardok which should drop you at 0, 0, -8f facing North
                        zoneProperties.SetBaseZoneProperties("overthere", "The Overthere", -4263f, -241f, 235f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "sebilis":
                    {
                        zoneProperties.SetBaseZoneProperties("sebilis", "Old Sebilis", 0f, 235f, 40f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 60, 50, 400);
                    }
                    break;
                case "skyfire":
                    {
                        zoneProperties.SetBaseZoneProperties("skyfire", "Skyfire Mountains", -3931.32f, -1139.25f, 39.76f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 200, 200, 200, 600);
                        zoneProperties.AddZoneLineBox("burningwood", 5087.0146f, 1740.0859f, -163.56395f, ZoneLineOrientationType.South,
                            -5623.817f, 1910.7054f, -56.840195f, -5703.1704f, 1580.5497f, -164.28036f); // Zone-in had no geometery
                    }
                    break;
                case "swampofnohope":
                    {
                        zoneProperties.SetBaseZoneProperties("swampofnohope", "Swamp of No Hope", -1830f, -1259.9f, 27.1f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 200, 210, 60, 400);
                        zoneProperties.AddZoneLineBox("cabeast", -98.067253f, -657.688232f, 0.000070f, ZoneLineOrientationType.North,
                            3172.572266f, 3068.755859f, 43.239300f, 3137.161865f, 3040.213135f, -0.374930f);
                        zoneProperties.AddZoneLineBox("cabeast", -280.219482f, -476.267853f, 0.000010f, ZoneLineOrientationType.West,
                            2955.181396f, 3256.399658f, -0.375170f, 2955.181396f, 3256.399658f, -0.375170f);
                    }
                    break;
                case "timorous":
                    {
                        zoneProperties.SetBaseZoneProperties("timorous", "Timorous Deep", 2194f, -5392f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(225, 225, 230, 100, 700);
                    }
                    break;
                case "trakanon":
                    {
                        zoneProperties.SetBaseZoneProperties("trakanon", "Trakanon's Teeth", 1485.86f, 3868.29f, -340.59f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 213, 60, 250);
                    }
                    break;
                case "veeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("veeshan", "Veeshan's Peak", 1682f, 41f, 28f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 0, 0, 100, 1200);
                    }
                    break;
                case "wakening":
                    {
                        zoneProperties.SetBaseZoneProperties("wakening", "Wakening Land", -5000f, -673f, -195f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(254, 254, 254, 60, 600);
                    }
                    break;
                case "warslikswood":
                    {
                        zoneProperties.SetBaseZoneProperties("warslikswood", "Warslik's Woods", -467.95f, -1428.95f, 197.31f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                        zoneProperties.AddZoneLineBox("cabwest", 870.207581f, 1143.751831f, 0.000020f, ZoneLineOrientationType.East,
                            -2237.151123f, -1135.133423f, 381.612640f, -2268.348633f, -1180.958496f, 262.312653f);
                        zoneProperties.AddZoneLineBox("cabwest", 688.666626f, 1327.751099f, 0.000030f, ZoneLineOrientationType.South,
                            -2420.843750f, -917.836975f, 399.112671f, -2473.554932f, -946.380981f, 262.313660f);
                    }
                    break;
                //--------------------------------------------------------------------------------------------------------------------------
                // Velious Zones
                //--------------------------------------------------------------------------------------------------------------------------
                case "cobaltscar":
                    {
                        // TODO: Portal to skyshrine is a clicky in the tower
                        zoneProperties.SetBaseZoneProperties("cobaltscar", "Cobalt Scar", 895f, -939f, 318f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(180, 180, 180, 200, 1800);
                        zoneProperties.AddZoneLineBox("sirens", -595.916992f, 73.038841f, -96.968727f, ZoneLineOrientationType.North,
                            1604.295898f, 1636.723511f, 87.406502f, 1588.378052f, 1616.337891f, 62.437771f);
                    }
                    break;
                case "crystal":
                    {
                        zoneProperties.SetBaseZoneProperties("crystal", "Crystal Caverns", 303f, 487f, -74f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "eastwastes":
                    {
                        zoneProperties.SetBaseZoneProperties("eastwastes", "Eastern Wastes", -4296f, -5049f, 147f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "frozenshadow":
                    {
                        zoneProperties.SetBaseZoneProperties("frozenshadow", "Tower of Frozen Shadow", 200f, 120f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 10, 350);
                    }
                    break;
                case "greatdivide":
                    {
                        zoneProperties.SetBaseZoneProperties("greatdivide", "The Great Divide", -965f, -7720f, -557f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(160, 160, 172, 200, 1800);
                    }
                    break;
                case "growthplane":
                    {
                        zoneProperties.SetBaseZoneProperties("growthplane", "Plane of Growth", 3016f, -2522f, -19f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 50, 100, 60, 1200);
                    }
                    break;
                case "iceclad":
                    {
                        zoneProperties.SetBaseZoneProperties("iceclad", "Iceclad Ocean", 340f, 5330f, -17f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "kael":
                    {
                        zoneProperties.SetBaseZoneProperties("kael", "Kael Drakkal", -633f, -47f, 128f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 10, 50, 20, 500);
                    }
                    break;
                case "mischiefplane":
                    {
                        zoneProperties.SetBaseZoneProperties("mischiefplane", "Plane of Mischief", -395f, -1410f, 115f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                    }
                    break;
                case "necropolis":
                    {
                        zoneProperties.SetBaseZoneProperties("necropolis", "Dragon Necropolis", 2000f, -100f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(35, 50, 35, 10, 2000);
                    }
                    break;
                case "sirens":
                    {
                        zoneProperties.SetBaseZoneProperties("sirens", "Siren's Grotto", -33f, 196f, 4f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(30, 100, 130, 10, 500);
                        zoneProperties.AddZoneLineBox("cobaltscar", 1584.026611f, 1626.080811f, 62.937771f, ZoneLineOrientationType.South,
                            -600.025208f, 83.160942f, -72.500954f, -625.650818f, 62.775249f, -97.468727f);
                    }
                    break;
                case "skyshrine":
                    {
                        zoneProperties.SetBaseZoneProperties("skyshrine", "Skyshrine", -730f, -210f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(50, 0, 200, 100, 600);
                    }
                    break;
                case "sleeper":
                    {
                        zoneProperties.SetBaseZoneProperties("sleeper", "Sleeper's Tomb", 0f, 0f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(80, 80, 220, 200, 800);
                    }
                    break;
                case "templeveeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("templeveeshan", "Temple of Veeshan", -499f, -2086f, -36f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(60, 10, 10, 30, 300);
                    }
                    break;
                case "thurgadina":
                    {
                        zoneProperties.SetBaseZoneProperties("thurgadina", "Thurgadin", 0f, -1222f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "thurgadinb":
                    {
                        zoneProperties.SetBaseZoneProperties("thurgadinb", "Icewell Keep", 0f, 250f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "velketor":
                    {
                        zoneProperties.SetBaseZoneProperties("velketor", "Velketor's Labyrinth", -65f, 581f, -152f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 130, 130, 10, 500);
                    }
                    break;
                case "westwastes":
                    {
                        zoneProperties.SetFogProperties(128, 128, 160, 200, 1800);
                        zoneProperties.SetBaseZoneProperties("westwastes", "Western Wastes", -3499f, -4099f, -16.66f, 0, ZoneContinent.Velious);
                    }
                    break;
                //--------------------------------------------------------------------------------------------------------------------------
                // Miscellaneous Zones
                //--------------------------------------------------------------------------------------------------------------------------
                case "load":
                    {
                        zoneProperties.SetBaseZoneProperties("load", "Loading Area", -316f, 5f, 8.2f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "tutorial":
                    {
                        zoneProperties.SetBaseZoneProperties("tutorial", "Tutorial", 0f, 0f, 0f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                default:
                    {
                        Logger.WriteError("GetZonePropertiesForZone error!  No known short name of '" + zoneShortName + "'");
                    }
                    break;
            }

            return zoneProperties;
        }
    }
}
