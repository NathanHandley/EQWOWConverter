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

        public class ZoneLineBox
        {
            public int AreaTriggerID;
            public string TargetZoneShortName = string.Empty;
            public Vector3 TargetZonePosition = new Vector3();
            public float TargetZoneOrientation = 0f;
            public Vector3 BoxPosition = new Vector3();
            public float BoxLength;
            public float BoxWidth;
            public float BoxHeight;
            public float BoxOrientation;

            public ZoneLineBox()
            {
                AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();
            }
        }

        public string ShortName = string.Empty;
        public ZoneContinent Continent;
        public ColorRGB FogColor = new ColorRGB();
        public int FogMinClip = -1;
        public int FogMaxClip = -1;
        public bool DoShowSky = true;
        public Vector3 SafePosition = new Vector3();
        public float SafeOrientation = 0;
        public List<ZoneLineBox> ZoneLineBoxes = new List<ZoneLineBox>();

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        public void SetBaseZoneProperties(string shortName, float safeX, float safeY, float safeZ, float safeOrientation, ZoneContinent continent)
        {
            ShortName = shortName;
            Continent = continent;
            SafePosition.X = safeX;
            SafePosition.Y = safeY;
            SafePosition.Z = safeZ;
            SafeOrientation = safeOrientation;
        }

        public void SetFogProperties(int red, int green, int blue, int minClip, int maxClip)
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
            // Scale input values
            targetZonePositionX *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            targetZonePositionY *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            targetZonePositionZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxTopNorthwestX *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxTopNorthwestY *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxTopNorthwestZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxBottomSoutheastX *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxBottomSoutheastY *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            boxBottomSoutheastZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

            // Create the box base values
            ZoneLineBox zoneLineBox = new ZoneLineBox();
            zoneLineBox.TargetZoneShortName = targetZoneShortName;
            zoneLineBox.TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch(targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: zoneLineBox.TargetZoneOrientation = 0;  break;
                case ZoneLineOrientationType.South: zoneLineBox.TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: zoneLineBox.TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: zoneLineBox.TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            zoneLineBox.BoxPosition = zoneLineBoxBounding.GetCenter();
            zoneLineBox.BoxWidth = zoneLineBoxBounding.GetYDistance();
            zoneLineBox.BoxLength = zoneLineBoxBounding.GetXDistance();
            zoneLineBox.BoxHeight = zoneLineBoxBounding.GetZDistance();
            ZoneLineBoxes.Add(zoneLineBox);
        }

        public static ZoneProperties GetZonePropertiesForZone(string zoneShortName)
        {
            if (ZonePropertyListByShortName.Count == 0)
                PopulateZonePropertiesList();
            if (ZonePropertyListByShortName.ContainsKey(zoneShortName) == false)
            {
                Logger.WriteLine("Error.  Tried to pull Zone Properties for zone with shortname '" + zoneShortName + "' but non existed with that name");
                return new ZoneProperties();
            }
            else
                return ZonePropertyListByShortName[zoneShortName];
        }

        private static void PopulateZonePropertiesList()
        {
            ZonePropertyListByShortName.Add("airplane", GenerateZonePropertiesForZone("airplane"));
            ZonePropertyListByShortName.Add("akanon", GenerateZonePropertiesForZone("akanon"));
            ZonePropertyListByShortName.Add("arena", GenerateZonePropertiesForZone("arena"));
            ZonePropertyListByShortName.Add("befallen", GenerateZonePropertiesForZone("befallen"));
            ZonePropertyListByShortName.Add("beholder", GenerateZonePropertiesForZone("beholder"));
            ZonePropertyListByShortName.Add("blackburrow", GenerateZonePropertiesForZone("blackburrow"));
            ZonePropertyListByShortName.Add("burningwood", GenerateZonePropertiesForZone("burningwood"));
            ZonePropertyListByShortName.Add("butcher", GenerateZonePropertiesForZone("butcher"));
            ZonePropertyListByShortName.Add("cabeast", GenerateZonePropertiesForZone("cabeast"));
            ZonePropertyListByShortName.Add("cabwest", GenerateZonePropertiesForZone("cabwest"));
            ZonePropertyListByShortName.Add("cauldron", GenerateZonePropertiesForZone("cauldron"));
            ZonePropertyListByShortName.Add("cazicthule", GenerateZonePropertiesForZone("cazicthule"));
            ZonePropertyListByShortName.Add("charasis", GenerateZonePropertiesForZone("charasis"));
            ZonePropertyListByShortName.Add("chardok", GenerateZonePropertiesForZone("chardok"));
            ZonePropertyListByShortName.Add("citymist", GenerateZonePropertiesForZone("citymist"));
            ZonePropertyListByShortName.Add("cobaltscar", GenerateZonePropertiesForZone("cobaltscar"));
            ZonePropertyListByShortName.Add("commons", GenerateZonePropertiesForZone("commons"));
            ZonePropertyListByShortName.Add("crushbone", GenerateZonePropertiesForZone("crushbone"));
            ZonePropertyListByShortName.Add("crystal", GenerateZonePropertiesForZone("crystal"));
            ZonePropertyListByShortName.Add("dalnir", GenerateZonePropertiesForZone("dalnir"));
            ZonePropertyListByShortName.Add("dreadlands", GenerateZonePropertiesForZone("dreadlands"));
            ZonePropertyListByShortName.Add("droga", GenerateZonePropertiesForZone("droga"));
            ZonePropertyListByShortName.Add("eastkarana", GenerateZonePropertiesForZone("eastkarana"));
            ZonePropertyListByShortName.Add("eastwastes", GenerateZonePropertiesForZone("eastwastes"));
            ZonePropertyListByShortName.Add("ecommons", GenerateZonePropertiesForZone("ecommons"));
            ZonePropertyListByShortName.Add("emeraldjungle", GenerateZonePropertiesForZone("emeraldjungle"));
            ZonePropertyListByShortName.Add("erudnext", GenerateZonePropertiesForZone("erudnext"));
            ZonePropertyListByShortName.Add("erudnint", GenerateZonePropertiesForZone("erudnint"));
            ZonePropertyListByShortName.Add("erudsxing", GenerateZonePropertiesForZone("erudsxing"));
            ZonePropertyListByShortName.Add("everfrost", GenerateZonePropertiesForZone("everfrost"));
            ZonePropertyListByShortName.Add("fearplane", GenerateZonePropertiesForZone("fearplane"));
            ZonePropertyListByShortName.Add("feerrott", GenerateZonePropertiesForZone("feerrott"));
            ZonePropertyListByShortName.Add("felwithea", GenerateZonePropertiesForZone("felwithea"));
            ZonePropertyListByShortName.Add("felwitheb", GenerateZonePropertiesForZone("felwitheb"));
            ZonePropertyListByShortName.Add("fieldofbone", GenerateZonePropertiesForZone("fieldofbone"));
            ZonePropertyListByShortName.Add("firiona", GenerateZonePropertiesForZone("firiona"));
            ZonePropertyListByShortName.Add("freporte", GenerateZonePropertiesForZone("freporte"));
            ZonePropertyListByShortName.Add("freportn", GenerateZonePropertiesForZone("freportn"));
            ZonePropertyListByShortName.Add("freportw", GenerateZonePropertiesForZone("freportw"));
            ZonePropertyListByShortName.Add("frontiermtns", GenerateZonePropertiesForZone("frontiermtns"));
            ZonePropertyListByShortName.Add("frozenshadow", GenerateZonePropertiesForZone("frozenshadow"));
            ZonePropertyListByShortName.Add("gfaydark", GenerateZonePropertiesForZone("gfaydark"));
            ZonePropertyListByShortName.Add("greatdivide", GenerateZonePropertiesForZone("greatdivide"));
            ZonePropertyListByShortName.Add("grobb", GenerateZonePropertiesForZone("grobb"));
            ZonePropertyListByShortName.Add("growthplane", GenerateZonePropertiesForZone("growthplane"));
            ZonePropertyListByShortName.Add("gukbottom", GenerateZonePropertiesForZone("gukbottom"));
            ZonePropertyListByShortName.Add("guktop", GenerateZonePropertiesForZone("guktop"));
            ZonePropertyListByShortName.Add("halas", GenerateZonePropertiesForZone("halas"));
            ZonePropertyListByShortName.Add("hateplane", GenerateZonePropertiesForZone("hateplane"));
            ZonePropertyListByShortName.Add("highkeep", GenerateZonePropertiesForZone("highkeep"));
            ZonePropertyListByShortName.Add("highpass", GenerateZonePropertiesForZone("highpass"));
            ZonePropertyListByShortName.Add("hole", GenerateZonePropertiesForZone("hole"));
            ZonePropertyListByShortName.Add("iceclad", GenerateZonePropertiesForZone("iceclad"));
            ZonePropertyListByShortName.Add("innothule", GenerateZonePropertiesForZone("innothule"));
            ZonePropertyListByShortName.Add("kael", GenerateZonePropertiesForZone("kael"));
            ZonePropertyListByShortName.Add("kaesora", GenerateZonePropertiesForZone("kaesora"));
            ZonePropertyListByShortName.Add("kaladima", GenerateZonePropertiesForZone("kaladima"));
            ZonePropertyListByShortName.Add("kaladimb", GenerateZonePropertiesForZone("kaladimb"));
            ZonePropertyListByShortName.Add("karnor", GenerateZonePropertiesForZone("karnor"));
            ZonePropertyListByShortName.Add("kedge", GenerateZonePropertiesForZone("kedge"));
            ZonePropertyListByShortName.Add("kerraridge", GenerateZonePropertiesForZone("kerraridge"));
            ZonePropertyListByShortName.Add("kithicor", GenerateZonePropertiesForZone("kithicor"));
            ZonePropertyListByShortName.Add("kurn", GenerateZonePropertiesForZone("kurn"));
            ZonePropertyListByShortName.Add("lakeofillomen", GenerateZonePropertiesForZone("lakeofillomen"));
            ZonePropertyListByShortName.Add("lakerathe", GenerateZonePropertiesForZone("lakerathe"));
            ZonePropertyListByShortName.Add("lavastorm", GenerateZonePropertiesForZone("lavastorm"));
            ZonePropertyListByShortName.Add("lfaydark", GenerateZonePropertiesForZone("lfaydark"));
            ZonePropertyListByShortName.Add("load", GenerateZonePropertiesForZone("load"));
            ZonePropertyListByShortName.Add("mischiefplane", GenerateZonePropertiesForZone("mischiefplane"));
            ZonePropertyListByShortName.Add("mistmoore", GenerateZonePropertiesForZone("mistmoore"));
            ZonePropertyListByShortName.Add("misty", GenerateZonePropertiesForZone("misty"));
            ZonePropertyListByShortName.Add("najena", GenerateZonePropertiesForZone("najena"));
            ZonePropertyListByShortName.Add("necropolis", GenerateZonePropertiesForZone("necropolis"));
            ZonePropertyListByShortName.Add("nektulos", GenerateZonePropertiesForZone("nektulos"));
            ZonePropertyListByShortName.Add("neriaka", GenerateZonePropertiesForZone("neriaka"));
            ZonePropertyListByShortName.Add("neriakb", GenerateZonePropertiesForZone("neriakb"));
            ZonePropertyListByShortName.Add("neriakc", GenerateZonePropertiesForZone("neriakc"));
            ZonePropertyListByShortName.Add("northkarana", GenerateZonePropertiesForZone("northkarana"));
            ZonePropertyListByShortName.Add("nro", GenerateZonePropertiesForZone("nro"));
            ZonePropertyListByShortName.Add("nurga", GenerateZonePropertiesForZone("nurga"));
            ZonePropertyListByShortName.Add("oasis", GenerateZonePropertiesForZone("oasis"));
            ZonePropertyListByShortName.Add("oggok", GenerateZonePropertiesForZone("oggok"));
            ZonePropertyListByShortName.Add("oot", GenerateZonePropertiesForZone("oot"));
            ZonePropertyListByShortName.Add("overthere", GenerateZonePropertiesForZone("overthere"));
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
            ZonePropertyListByShortName.Add("sebilis", GenerateZonePropertiesForZone("sebilis"));
            ZonePropertyListByShortName.Add("sirens", GenerateZonePropertiesForZone("sirens"));
            ZonePropertyListByShortName.Add("skyfire", GenerateZonePropertiesForZone("skyfire"));
            ZonePropertyListByShortName.Add("skyshrine", GenerateZonePropertiesForZone("skyshrine"));
            ZonePropertyListByShortName.Add("sleeper", GenerateZonePropertiesForZone("sleeper"));
            ZonePropertyListByShortName.Add("soldunga", GenerateZonePropertiesForZone("soldunga"));
            ZonePropertyListByShortName.Add("soldungb", GenerateZonePropertiesForZone("soldungb"));
            ZonePropertyListByShortName.Add("soltemple", GenerateZonePropertiesForZone("soltemple"));
            ZonePropertyListByShortName.Add("southkarana", GenerateZonePropertiesForZone("southkarana"));
            ZonePropertyListByShortName.Add("sro", GenerateZonePropertiesForZone("sro"));
            ZonePropertyListByShortName.Add("steamfont", GenerateZonePropertiesForZone("steamfont"));
            ZonePropertyListByShortName.Add("stonebrunt", GenerateZonePropertiesForZone("stonebrunt"));
            ZonePropertyListByShortName.Add("swampofnohope", GenerateZonePropertiesForZone("swampofnohope"));
            ZonePropertyListByShortName.Add("templeveeshan", GenerateZonePropertiesForZone("templeveeshan"));
            ZonePropertyListByShortName.Add("thurgadina", GenerateZonePropertiesForZone("thurgadina"));
            ZonePropertyListByShortName.Add("thurgadinb", GenerateZonePropertiesForZone("thurgadinb"));
            ZonePropertyListByShortName.Add("timorous", GenerateZonePropertiesForZone("timorous"));
            ZonePropertyListByShortName.Add("tox", GenerateZonePropertiesForZone("tox"));
            ZonePropertyListByShortName.Add("trakanon", GenerateZonePropertiesForZone("trakanon"));
            ZonePropertyListByShortName.Add("tutorial", GenerateZonePropertiesForZone("tutorial"));
            ZonePropertyListByShortName.Add("unrest", GenerateZonePropertiesForZone("unrest"));
            ZonePropertyListByShortName.Add("veeshan", GenerateZonePropertiesForZone("veeshan"));
            ZonePropertyListByShortName.Add("velketor", GenerateZonePropertiesForZone("velketor"));
            ZonePropertyListByShortName.Add("wakening", GenerateZonePropertiesForZone("wakening"));
            ZonePropertyListByShortName.Add("warrens", GenerateZonePropertiesForZone("warrens"));
            ZonePropertyListByShortName.Add("warslikswood", GenerateZonePropertiesForZone("warslikswood"));
            ZonePropertyListByShortName.Add("westwastes", GenerateZonePropertiesForZone("westwastes"));
        }

        private static ZoneProperties GenerateZonePropertiesForZone(string zoneShortName)
        {
            ZoneProperties zoneProperties = new ZoneProperties();

            switch (zoneShortName)
            {
                case "airplane": // Done - Tested
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Sky", 542.45f, 1384.6f, -650f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("freporte", -363.75037f, -1778.4629f, 100f, ZoneLineOrientationType.West,
                            3000f, 3000f, -1000f, -3000f, -3000f, -1200f);
                    }
                    break;
                case "akanon": // Done - Needs Test
                    {
                        zoneProperties.SetBaseZoneProperties("Ak'Anon", -35f, 47f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(30, 60, 30, 10, 600);
                        zoneProperties.AddZoneLineBox("steamfont", -2059.579834f, 528.815857f, -111.126549f, ZoneLineOrientationType.North,
                            70.830750f, -69.220848f, 12.469000f, 60.770279f, -84.162193f, -0.500000f);
                    }
                    break;
                case "arena": // Done - Tested
                    {
                        zoneProperties.SetBaseZoneProperties("The Arena", 460.9f, -41.4f, -7.38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 100, 100, 10, 1500);
                        zoneProperties.AddZoneLineBox("lakerathe", 2345.1172f, 2692.0679f, 92.193184f, ZoneLineOrientationType.East,
                            -44.28722f, -845.03625f, 45.75025f, -74.66106f, -871.419f, 7.0403852f);
                    }
                    break;
                case "befallen": // Done - Tested
                    {
                        zoneProperties.SetBaseZoneProperties("Befallen", 35.22f, -75.27f, 2.19f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 30, 90, 10, 175);
                        zoneProperties.AddZoneLineBox("commons", -1155.6317f, 596.3344f, -42.280308f, ZoneLineOrientationType.North,
                            -49.9417f, 42.162197f, 12.469f, -63.428658f, 27.86666f, -0.5000006f);
                    }
                    break;
                case "beholder": // Done - Tested - Needs Runnyeye Retested
                    {
                        zoneProperties.SetBaseZoneProperties("Gorge of King Xorbb", -21.44f, -512.23f, 45.13f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(240, 180, 150, 10, 600);
                        zoneProperties.AddZoneLineBox("runnyeye", -111.2879f, -12.021315f, 0.000001f, ZoneLineOrientationType.East,
                            911.07355f, -1858.2123f, 15.469f, 894.673f, -1878.7317f, 0.50007594f);
                        zoneProperties.AddZoneLineBox("eastkarana", 3094.991f, -2315.8328f, 23.849806f, ZoneLineOrientationType.South,
                            -1443.1708f, -542.2266f, 423.58218f, -1665.6155f, -747.2683f, 13f);
                    }
                    break;
                case "blackburrow": // Done - Tested - Needs retest to qeynos hills (fixed)
                    {
                        zoneProperties.SetBaseZoneProperties("Blackburrow", 38.92f, -158.97f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 100, 90, 10, 700);
                        zoneProperties.AddZoneLineBox("everfrost", -3027.1943f, -532.2794f, -113.18725f, ZoneLineOrientationType.North,
                            106.64458f, -329.8163f, 13.469f, 80.88026f, -358.2026f, -0.49926078f);
                        zoneProperties.AddZoneLineBox("qeytoqrg", 3432.621094f, -1142.645020f, 0.000010f, ZoneLineOrientationType.East,
                            -154.74507f, 20.123898f, 10.469f, -174.6326f, 10.831751f, -0.49996006f); 
                    }
                    break;
                case "burningwood": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("Burning Wood", -820f, -4942f, 200.31f, 0, ZoneContinent.Kunark);
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
                case "butcher": // Done
                    {
                        // Note: There should be a boat to Firiona Vie [Timorous Deep] (NYI) and a boat to Freeport [Ocean of Tears] (NYI)
                        zoneProperties.SetBaseZoneProperties("Butcherblock Mountains", -700f, 2550f, 2.9f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(150, 170, 140, 10, 1000);
                        zoneProperties.AddZoneLineBox("kaladima", -60.207775f, 41.798244f, 0.0010997541f, ZoneLineOrientationType.North,
                            3145.1406f, -173.6824f, 14.468006f, 3128.918f, -186.06715f, -0.4991133f);
                        zoneProperties.AddZoneLineBox("gfaydark", -1563.382568f, 2626.150391f, -0.126430f, ZoneLineOrientationType.North,
                            -1180.5581f, -3073.2896f, 67.52528f, -1218.3838f, -3150f, -0.4993223f);
                        zoneProperties.AddZoneLineBox("cauldron", 2853.7092f, 264.44955f, 469.3444f, ZoneLineOrientationType.South,
                            -2937.8154f, -317.8051f, 45.09004f, -2957.5332f, -351.47528f, -0.49968797f);
                    }
                    break;
                case "cabeast": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("East Cabilis", -416f, 1343f, 4f, 0, ZoneContinent.Kunark);
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
                case "cabwest": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("West Cabilis", 790f, 165f, 3.75f, 0, ZoneContinent.Kunark);
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
                case "cauldron": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("Dagnor's Cauldron", 320f, 2815f, 473f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 100, 140, 10, 1000);
                        zoneProperties.AddZoneLineBox("butcher", -2921.925537f, -335.659668f, 0.000200f, ZoneLineOrientationType.North,
                            2872.3113f, 280.6821f, 496.7702f, 2863.3867f, 247.66762f, 468.8444f);
                        zoneProperties.AddZoneLineBox("kedge", 129.834778f, 19.404051f, 320.322083f, ZoneLineOrientationType.West,
                            -1160.462646f, -1000.650696f, -287.718506f, -1180.848267f, -1024.053711f, -334.875000f);
                        zoneProperties.AddZoneLineBox("unrest", 60.597672f, 329.112183f, 0.000000f, ZoneLineOrientationType.South,
                            -2022.738403f, -616.401611f, 108.469002f, -2054.549072f, -636.787415f, 89.500183f);
                    }
                    break;
                case "cazicthule": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("Cazic Thule", -80f, 80f, 5.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 80, 20, 10, 450);
                        zoneProperties.AddZoneLineBox("feerrott", -1460.633545f, -109.760483f, 47.935600f, ZoneLineOrientationType.North,
                            42.322739f, -55.775299f, 10.469000f, -0.193150f, -84.162201f, -0.500000f);
                    }
                    break;
                case "charasis": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("Howling Stones", 0f, 0f, -4.25f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(160, 180, 200, 50, 400);
                        zoneProperties.AddZoneLineBox("overthere", -83.674156f, 825.339172f, -506.624969f, ZoneLineOrientationType.East, // Consider moving to the stone?
                            -2.369990f, -722.148010f, 34.469002f, -15.200140f, -734.827942f, 17.500111f);
                    }
                    break;
                case "chardok": // Done
                    {
                        zoneProperties.SetBaseZoneProperties("Chardok", 859f, 119f, 106f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 53, 6, 30, 300);
                        zoneProperties.AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                            -20.012981f, 879.84973f, 137.60643f, -70.907234f, 839.5071f, 99.46923f);
                        zoneProperties.AddZoneLineBox("burningwood", 7357.6494f, -4147.4604f, -235.93742f, ZoneLineOrientationType.North,
                            220.71272f, 895.73254f, 138.4065f, 157.77734f, 839.54913f, 99.468735f);
                    }
                    break;
                case "citymist":
                    {
                        zoneProperties.SetBaseZoneProperties("City of Mist", -734f, 28f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(90, 110, 60, 50, 275);
                    }
                    break;
                case "cobaltscar":
                    {
                        zoneProperties.SetBaseZoneProperties("Cobalt Scar", 895f, -939f, 318f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(180, 180, 180, 200, 1800);
                    }
                    break;
                case "commons":
                    {
                        zoneProperties.SetBaseZoneProperties("West Commonlands", -1334.24f, 209.57f, -51.47f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("befallen", -67.97337f, 34.777237f, 0.0009409825f, ZoneLineOrientationType.South,
                            -1161.5278f, 603.6031f, -29.81225f, -1176.3967f, 588.6972f, -42.781216f); // Tested, works fine
                    }
                    break;
                case "crushbone":
                    {
                        zoneProperties.SetBaseZoneProperties("Crushbone", 158f, -644f, 4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(90, 90, 190, 10, 400);
                    }
                    break;
                case "crystal":
                    {
                        zoneProperties.SetBaseZoneProperties("Crystal Caverns", 303f, 487f, -74f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "dalnir":
                    {
                        zoneProperties.SetBaseZoneProperties("Dalnir", 90f, 8f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 25, 30, 210);
                    }
                    break;
                case "dreadlands":
                    {
                        zoneProperties.SetBaseZoneProperties("Dreadlands", 9565.05f, 2806.04f, 1045.2f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 600);
                        zoneProperties.AddZoneLineBox("burningwood", -4247.9624f, -712.7452f, 245.30704f, ZoneLineOrientationType.North,
                            3057.91f, -414.8485f, 319.16867f, 2988.2588f, -1083.3096f, 240.4023f);
                    }
                    break;
                case "droga":
                    {
                        zoneProperties.SetBaseZoneProperties("Temple of Droga", 294.11f, 1371.43f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "eastkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Eastern Karana", 0f, 0f, 3.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("beholder", -1385.247f, -659.5757f, 60.639446f, ZoneLineOrientationType.North,
                            3302.396f, -2173.8193f, 330.6518f, 3189.4185f, -2385.883f, 42.886936f); // Issue, can slip around and hard to find.  Redo
                    }
                    break;
                case "eastwastes":
                    {
                        zoneProperties.SetBaseZoneProperties("Eastern Wastes", -4296f, -5049f, 147f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "ecommons":
                    {
                        zoneProperties.SetBaseZoneProperties("East Commonlands", -1485f, 9.2f, -51f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "emeraldjungle":
                    {
                        zoneProperties.SetBaseZoneProperties("Emerald Jungle", 4648.06f, -1222.97f, 0f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(200, 235, 210, 60, 200);
                    }
                    break;
                case "erudnext":
                    {
                        zoneProperties.SetBaseZoneProperties("Erudin Docks", -309.75f, 109.64f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 550);
                    }
                    break;
                case "erudnint":
                    {
                        zoneProperties.SetBaseZoneProperties("Erudin Palace", 807f, 712f, 22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "erudsxing":
                    {
                        zoneProperties.SetBaseZoneProperties("Erud's Crossing", 795f, -1766.9f, 12.36f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "everfrost":
                    {
                        zoneProperties.SetBaseZoneProperties("Everfrost Peaks", 682.74f, 3139.01f, -60.16f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 500);
                        zoneProperties.AddZoneLineBox("blackburrow", 64.26508f, -340.1918f, 0.00073920796f, ZoneLineOrientationType.South,
                            -3054.6953f, -515.55963f, -99.7185f, -3094.8235f, -547f, -113.68753f); // Works properly
                    }
                    break;
                case "fearplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Fear", 1282.09f, -1139.03f, 1.67f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 1000);
                    }
                    break;
                case "feerrott":
                    {
                        zoneProperties.SetBaseZoneProperties("The Feerrott", 902.6f, 1091.7f, 28f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(60, 90, 30, 10, 175);
                        zoneProperties.AddZoneLineBox("cazicthule", 55.471420f, -67.975937f, 0.000000f, ZoneLineOrientationType.North,
                            -1468.307251f, -100.275467f, 58.406502f, -1498.661133f, -119.661331f, 48.437538f);
                    }
                    break;
                case "felwithea":
                    {
                        zoneProperties.SetBaseZoneProperties("North Felwithe", 94f, -25f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                    }
                    break;
                case "felwitheb":
                    {
                        zoneProperties.SetBaseZoneProperties("South Felwithe", -790f, 320f, -10.25f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(100, 130, 100, 10, 300);
                    }
                    break;
                case "fieldofbone":
                    {
                        zoneProperties.SetBaseZoneProperties("Field of Bone", 1617f, -1684f, -54.78f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("cabeast", 1359.3015f, -435.72766f, 0.000174f, ZoneLineOrientationType.West,
                            -2541.8613f, 3747.162f, 50.543545f, -2570.4119f, 3699.717f, 3.5938148f);
                        zoneProperties.AddZoneLineBox("cabeast", 1179.1279f, -619.062f, 0.000174f, ZoneLineOrientationType.South,
                            -2768.011f, 3545.4978f, 86.73899f, -2829.281f, 3514.2957f, 3.5937567f);
                    }
                    break;
                case "firiona":
                    {
                        zoneProperties.SetBaseZoneProperties("Firiona Vie", 1439.96f, -2392.06f, -2.65f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "freporte":
                    {
                        zoneProperties.SetBaseZoneProperties("East Freeport", -648f, -1097f, -52.2f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                    }
                    break;
                case "freportn":
                    {
                        zoneProperties.SetBaseZoneProperties("North Freeport", 211f, -296f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.AddZoneLineBox("freportw", 217.17102f, -124.4742f, -14.000001f, ZoneLineOrientationType.South,
                            -420.53745f, 504.00485f, 14.968492f, -458.10336f, 475.61905f, -14.499974f);
                    }
                    break;
                case "freportw":
                    {
                        zoneProperties.SetBaseZoneProperties("West Freeport", 181f, 335f, -24f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(230, 200, 200, 10, 450);
                        zoneProperties.AddZoneLineBox("freportn", -410.58734f, 489.82828f, -13.999962f, ZoneLineOrientationType.North,
                            266.0717f, -111.73723f, 14.0062895f, 224.31366f, -140.08673f, -14.499674f);
                    }
                    break;
                case "frontiermtns":
                    {
                        zoneProperties.SetBaseZoneProperties("Frontier Mountains", -4262f, -633f, 113.24f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("burningwood", -2965.3167f, -4515.809f, -51.462868f, ZoneLineOrientationType.West,
                            -2312.331f, 4184.5947f, -433.798f, -2418.7312f, 4063.2607f, -472.19543f);
                    }
                    break;
                case "frozenshadow":
                    {
                        zoneProperties.SetBaseZoneProperties("Tower of Frozen Shadow", 200f, 120f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 10, 350);
                    }
                    break;
                case "gfaydark":
                    {
                        zoneProperties.SetBaseZoneProperties("Greater Faydark", 10f, -20f, 0f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(0, 128, 64, 10, 300);
                        zoneProperties.AddZoneLineBox("butcher", -1164.1454f, -3082.1367f, 0.00028176606f, ZoneLineOrientationType.North,
                            -1636.052856f, 2614.448242f, 80.942001f, -1604.046753f, 2657.645264f, -0.499690f); // Pre-tested
                    }
                    break;
                case "greatdivide":
                    {
                        zoneProperties.SetBaseZoneProperties("The Great Divide", -965f, -7720f, -557f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(160, 160, 172, 200, 1800);
                    }
                    break;
                case "grobb":
                    {
                        zoneProperties.SetBaseZoneProperties("Grobb", 0f, -100f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "growthplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Growth", 3016f, -2522f, -19f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(0, 50, 100, 60, 1200);
                    }
                    break;
                case "gukbottom":
                    {
                        zoneProperties.SetBaseZoneProperties("Lower Guk", -217f, 1197f, -81.78f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(50, 45, 20, 10, 140);
                    }
                    break;
                case "guktop":
                    {
                        zoneProperties.SetBaseZoneProperties("Upper Guk", 7f, -36f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(40, 45, 20, 10, 140);
                    }
                    break;
                case "halas":
                    {
                        zoneProperties.SetBaseZoneProperties("Halas", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 230, 255, 10, 300);
                    }
                    break;
                case "hateplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Hate", -353.08f, -374.8f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(128, 128, 128, 30, 200);
                    }
                    break;
                case "highkeep":
                    {
                        zoneProperties.SetBaseZoneProperties("High Keep", 88f, -16f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 0, 0);
                    }
                    break;
                case "highpass":
                    {
                        zoneProperties.SetBaseZoneProperties("Highpass Hold", -104f, -14f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 200, 10, 400);
                    }
                    break;
                case "hole":
                    {
                        zoneProperties.SetBaseZoneProperties("The Hole", -1049.98f, 640.04f, -77.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(10, 10, 10, 200, 500);
                    }
                    break;
                case "iceclad":
                    {
                        zoneProperties.SetBaseZoneProperties("Iceclad Ocean", 340f, 5330f, -17f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(200, 200, 200, 200, 1800);
                    }
                    break;
                case "innothule":
                    {
                        zoneProperties.SetBaseZoneProperties("Innothule Swamp", -588f, -2192f, -25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(170, 160, 90, 10, 500);
                    }
                    break;
                case "kael":
                    {
                        zoneProperties.SetBaseZoneProperties("Kael Drakkal", -633f, -47f, 128f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 10, 50, 20, 500);
                    }
                    break;
                case "kaesora":
                    {
                        zoneProperties.SetBaseZoneProperties("Kaesora", 40f, 370f, 99.72f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 10, 0, 20, 200);
                    }
                    break;
                case "kaladima":
                    {
                        zoneProperties.SetBaseZoneProperties("South Kaladim", -2f, -18f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                        zoneProperties.AddZoneLineBox("butcher", 3121.1667f, -179.98013f, 0.00088672107f, ZoneLineOrientationType.South,
                            -66.545395f, 47.896313f, 14.469f, -85.64434f, 34.009415f, -0.49999186f); // pre-check
                    }
                    break;
                case "kaladimb":
                    {
                        zoneProperties.SetBaseZoneProperties("North Kaladim", -267f, 414f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(70, 50, 20, 10, 175);
                    }
                    break;
                case "karnor":
                    {
                        zoneProperties.SetBaseZoneProperties("Karnor's Castle", 0f, 0f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 20, 20, 10, 350);
                    }
                    break;
                case "kedge":
                    {
                        zoneProperties.SetBaseZoneProperties("Kedge Keep", 99.96f, 14.02f, 31.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(10, 10, 10, 25, 25);
                        zoneProperties.AddZoneLineBox("cauldron", -1170.507080f, -1030.383179f, -315.376831f, ZoneLineOrientationType.East,
                            140.130951f, 14.514380f, 348.342682f, 119.745049f, -10.192420f, 299.375000f);
                    }
                    break;
                case "kerraridge":
                    {
                        zoneProperties.SetBaseZoneProperties("Kerra Island", -859.97f, 474.96f, 23.75f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 220, 200, 10, 600);
                    }
                    break;
                case "kithicor":
                    {
                        zoneProperties.SetBaseZoneProperties("Kithicor Forest", 3828f, 1889f, 459f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(120, 140, 100, 10, 200);
                    }
                    break;
                case "kurn":
                    {
                        zoneProperties.SetBaseZoneProperties("Kurn's Tower", 77.72f, -277.64f, 3.75f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(50, 50, 20, 10, 200);
                    }
                    break;
                case "lakeofillomen":
                    {
                        zoneProperties.SetBaseZoneProperties("Lake of Ill Omen", -5383.07f, 5747.14f, 68.27f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                        zoneProperties.AddZoneLineBox("cabwest", -802.654480f, 767.458740f, -0.000070f, ZoneLineOrientationType.North,
                            6577.715820f, -6613.837891f, 145.213730f, 6533.130859f, -6645.066895f, 34.593719f);
                        zoneProperties.AddZoneLineBox("cabwest", -985.943787f, 584.806458f, 0.000380f, ZoneLineOrientationType.East,
                            6344.193848f, -6799.043945f, 182.103806f, 6315.685547f, -6843.227051f, 34.595600f);
                    }
                    break;
                case "lakerathe":
                    {
                        zoneProperties.SetBaseZoneProperties("Lake Rathetear", 1213f, 4183f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("arena", -56.940857f, -835.9014f, 7.882746f, ZoneLineOrientationType.West,
                            2360.1794f, 2708.7017f, 130.344f, 2329.8247f, 2699.243f, 92.11265f); // Tested and works
                    }
                    break;
                case "lavastorm":
                    {
                        zoneProperties.SetBaseZoneProperties("Lavastorm Mountains", 153.45f, -1842.79f, -16.37f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(255, 50, 10, 10, 800);
                    }
                    break;
                case "lfaydark":
                    {
                        zoneProperties.SetBaseZoneProperties("Lesser Faydark", -1769.93f, -108.08f, -1.11f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(230, 255, 200, 10, 300);
                    }
                    break;
                case "load":
                    {
                        zoneProperties.SetBaseZoneProperties("Loading Area", -316f, 5f, 8.2f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "mischiefplane":
                    {
                        zoneProperties.SetBaseZoneProperties("Plane of Mischief", -395f, -1410f, 115f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                    }
                    break;
                case "mistmoore":
                    {
                        zoneProperties.SetBaseZoneProperties("Mistmoore Castle", 123f, -295f, -177f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(60, 30, 90, 10, 250);
                    }
                    break;
                case "misty":
                    {
                        zoneProperties.SetBaseZoneProperties("Misty Thicket", 0f, 0f, 2.43f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(100, 120, 50, 10, 500);
                    }
                    break;
                case "najena":
                    {
                        zoneProperties.SetBaseZoneProperties("Najena", -22.6f, 229.1f, -41.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 0, 40, 10, 110);
                    }
                    break;
                case "necropolis":
                    {
                        zoneProperties.SetBaseZoneProperties("Dragon Necropolis", 2000f, -100f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(35, 50, 35, 10, 2000);
                    }
                    break;
                case "nektulos":
                    {
                        zoneProperties.SetBaseZoneProperties("Nektulos Forest", -259f, -1201f, -5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(80, 90, 70, 10, 400);
                    }
                    break;
                case "neriaka":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Foreign Quarter", 156.92f, -2.94f, 31.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "neriakb":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Commons", -499.91f, 2.97f, -10.25f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "neriakc":
                    {
                        zoneProperties.SetBaseZoneProperties("Neriak Third Gate", -968.96f, 891.92f, -52.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(10, 0, 60, 10, 250);
                    }
                    break;
                case "northkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Northern Karana", -382f, -284f, -7f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "nro":
                    {
                        zoneProperties.SetBaseZoneProperties("Northern Desert of Ro", 299.12f, 3537.9f, -24.5f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "nurga":
                    {
                        zoneProperties.SetBaseZoneProperties("Mines of Nurga", 150f, -1062f, -107f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "oasis":
                    {
                        zoneProperties.SetBaseZoneProperties("Oasis of Marr", 903.98f, 490.03f, 6.4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "oggok":
                    {
                        zoneProperties.SetBaseZoneProperties("Oggok", -99f, -345f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(130, 140, 80, 10, 300);
                    }
                    break;
                case "oot":
                    {
                        zoneProperties.SetBaseZoneProperties("Ocean of Tears", -9200f, 390f, 6f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "overthere":
                    {
                        // TODO: There's a clicky teleport to Chardok which should drop you at 0, 0, -8f facing North
                        zoneProperties.SetBaseZoneProperties("The Overthere", -4263f, -241f, 235f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 235, 235, 200, 800);
                    }
                    break;
                case "paineel":
                    {
                        zoneProperties.SetBaseZoneProperties("Paineel", 200f, 800f, 3.39f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(150, 150, 150, 200, 850);
                    }
                    break;
                case "paw":
                    {
                        zoneProperties.SetBaseZoneProperties("Splitpaw Lair", -7.9f, -79.3f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(30, 25, 10, 10, 180);
                    }
                    break;
                case "permafrost":
                    {
                        zoneProperties.SetBaseZoneProperties("Permafrost", 0f, 0f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(25, 35, 45, 10, 180);
                    }
                    break;
                case "qcat":
                    {
                        zoneProperties.SetBaseZoneProperties("Qeynos Catacombs", -315f, 214f, -38f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "qey2hh1":
                    {
                        zoneProperties.SetBaseZoneProperties("Western Karana", -638f, 12f, -4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "qeynos":
                    {
                        zoneProperties.SetBaseZoneProperties("South Qeynos", 186.46f, 14.29f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 210, 10, 450);
                    }
                    break;
                case "qeynos2":
                    {
                        zoneProperties.SetBaseZoneProperties("North Qeynos", 114f, 678f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 450);
                    }
                    break;
                case "qeytoqrg":
                    {
                        zoneProperties.SetBaseZoneProperties("Qeynos Hills", 196.7f, 5100.9f, -1f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                        zoneProperties.AddZoneLineBox("blackburrow", -163.06775f, 29.47728f, 1.1399389f, ZoneLineOrientationType.West,
                            3442.5054f, -1124.6694f, 11.548047f, 3424.3691f, -1135.8118f, -0.4999545f); // Redo Z target
                    }
                    break;
                case "qrg":
                    {
                        zoneProperties.SetBaseZoneProperties("Surefall Glade", 136.9f, -65.9f, 4f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 175, 183, 10, 450);
                    }
                    break;
                case "rathemtn":
                    {
                        zoneProperties.SetBaseZoneProperties("Rathe Mountains", 1831f, 3825f, 29.03f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "rivervale":
                    {
                        zoneProperties.SetBaseZoneProperties("Rivervale", 45.3f, 1.6f, 3.8f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 210, 200, 10, 400);
                    }
                    break;
                case "runnyeye":
                    {
                        zoneProperties.SetBaseZoneProperties("Runnyeye Citadel", -21.85f, -108.88f, 3.75f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(75, 150, 25, 10, 600);
                        zoneProperties.AddZoneLineBox("beholder", 903.2041f, -1850.1808f, 1.0001143f, ZoneLineOrientationType.West,
                            -102.775955f, 12.901143f, 15.468005f, -119.129944f, -8.304958f, -0.49999338f); // Tested, works fine
                    }
                    break;
                case "sebilis":
                    {
                        zoneProperties.SetBaseZoneProperties("Old Sebilis", 0f, 235f, 40f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 10, 60, 50, 400);
                    }
                    break;
                case "sirens":
                    {
                        zoneProperties.SetBaseZoneProperties("Siren's Grotto", -33f, 196f, 4f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(30, 100, 130, 10, 500);
                    }
                    break;
                case "skyfire":
                    {
                        zoneProperties.SetBaseZoneProperties("Skyfire Mountains", -3931.32f, -1139.25f, 39.76f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(235, 200, 200, 200, 600);
                        zoneProperties.AddZoneLineBox("burningwood", 5087.0146f, 1740.0859f, -163.56395f, ZoneLineOrientationType.South,
                            -5623.817f, 1910.7054f, -56.840195f, -5703.1704f, 1580.5497f, -164.28036f); // Zone-in had no geometery
                    }
                    break;
                case "skyshrine":
                    {
                        zoneProperties.SetBaseZoneProperties("Skyshrine", -730f, -210f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(50, 0, 200, 100, 600);
                    }
                    break;
                case "sleeper":
                    {
                        zoneProperties.SetBaseZoneProperties("Sleeper's Tomb", 0f, 0f, 5f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(80, 80, 220, 200, 800);
                    }
                    break;
                case "soldunga":
                    {
                        zoneProperties.SetBaseZoneProperties("Solusek's Eye", -485.77f, -476.04f, 73.72f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 100);
                    }
                    break;
                case "soldungb":
                    {
                        zoneProperties.SetBaseZoneProperties("Nagafen's Lair", -262.7f, -423.99f, -108.22f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 30, 30, 10, 350);
                    }
                    break;
                case "soltemple":
                    {
                        zoneProperties.SetBaseZoneProperties("The Temple of Solusek Ro", 7.5f, 268.8f, 3f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(180, 5, 5, 50, 500);
                    }
                    break;
                case "southkarana":
                    {
                        zoneProperties.SetBaseZoneProperties("Southern Karana", 1293.66f, 2346.69f, -5.77f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                    }
                    break;
                case "sro":
                    {
                        zoneProperties.SetBaseZoneProperties("Southern Desert of Ro", 286f, 1265f, 79f, 0, ZoneContinent.Antonica);
                        zoneProperties.SetFogProperties(250, 250, 180, 10, 800);
                    }
                    break;
                case "steamfont":
                    {
                        zoneProperties.SetBaseZoneProperties("Steamfont Mountains", -272.86f, 159.86f, -21.4f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(200, 200, 220, 10, 800);
                        zoneProperties.AddZoneLineBox("akanon", 57.052101f, -77.213501f, 0.000010f, ZoneLineOrientationType.South,
                            -2064.9805f, 535.8183f, -98.656f, -2077.9038f, 521.43134f, -111.624886f);
                    }
                    break;
                case "stonebrunt":
                    {
                        zoneProperties.SetBaseZoneProperties("Stonebrunt Mountains", -1643.01f, -3427.84f, -6.57f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(235, 235, 235, 10, 800);
                    }
                    break;
                case "swampofnohope":
                    {
                        zoneProperties.SetBaseZoneProperties("Swamp of No Hope", -1830f, -1259.9f, 27.1f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 200, 210, 60, 400);
                        zoneProperties.AddZoneLineBox("cabeast", -98.067253f, -657.688232f, 0.000070f, ZoneLineOrientationType.North,
                            3172.572266f, 3068.755859f, 43.239300f, 3137.161865f, 3040.213135f, -0.374930f);
                        zoneProperties.AddZoneLineBox("cabeast", -280.219482f, -476.267853f, 0.000010f, ZoneLineOrientationType.West,
                            2955.181396f, 3256.399658f, -0.375170f, 2955.181396f, 3256.399658f, -0.375170f);
                    }
                    break;
                case "templeveeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("Temple of Veeshan", -499f, -2086f, -36f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(60, 10, 10, 30, 300);
                    }
                    break;
                case "thurgadina":
                    {
                        zoneProperties.SetBaseZoneProperties("Thurgadin", 0f, -1222f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "thurgadinb":
                    {
                        zoneProperties.SetBaseZoneProperties("Icewell Keep", 0f, 250f, 0f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(25, 25, 25, 100, 300);
                    }
                    break;
                case "timorous":
                    {
                        zoneProperties.SetBaseZoneProperties("Timorous Deep", 2194f, -5392f, 4f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(225, 225, 230, 100, 700);
                    }
                    break;
                case "tox":
                    {
                        zoneProperties.SetBaseZoneProperties("Toxxulia Forest", 203f, 2295f, -45f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(220, 200, 30, 50, 250);
                    }
                    break;
                case "trakanon":
                    {
                        zoneProperties.SetBaseZoneProperties("Trakanon's Teeth", 1485.86f, 3868.29f, -340.59f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 213, 60, 250);
                    }
                    break;
                case "tutorial":
                    {
                        zoneProperties.SetBaseZoneProperties("Tutorial", 0f, 0f, 0f, 0, ZoneContinent.Development);
                        zoneProperties.SetFogProperties(0, 0, 0, 500, 2000);
                    }
                    break;
                case "unrest":
                    {
                        zoneProperties.SetBaseZoneProperties("The Estate of Unrest", 52f, -38f, 3.75f, 0, ZoneContinent.Faydwer);
                        zoneProperties.SetFogProperties(40, 10, 60, 10, 300);
                        zoneProperties.AddZoneLineBox("cauldron", -2014.301880f, -627.332886f, 90.001083f, ZoneLineOrientationType.North,
                            113.163170f, 340.068451f, 18.469000f, 72.315872f, 319.681549f, -0.500000f);
                    }
                    break;
                case "veeshan":
                    {
                        zoneProperties.SetBaseZoneProperties("Veeshan's Peak", 1682f, 41f, 28f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(20, 0, 0, 100, 1200);
                    }
                    break;
                case "velketor":
                    {
                        zoneProperties.SetBaseZoneProperties("Velketor's Labyrinth", -65f, 581f, -152f, 0, ZoneContinent.Velious);
                        zoneProperties.SetFogProperties(10, 130, 130, 10, 500);
                    }
                    break;
                case "wakening":
                    {
                        zoneProperties.SetBaseZoneProperties("Wakening Land", -5000f, -673f, -195f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(254, 254, 254, 60, 600);
                    }
                    break;
                case "warrens":
                    {
                        zoneProperties.SetBaseZoneProperties("The Warrens", -930f, 748f, -37.22f, 0, ZoneContinent.Odus);
                        zoneProperties.SetFogProperties(0, 15, 0, 100, 300);
                    }
                    break;
                case "warslikswood":
                    {
                        zoneProperties.SetBaseZoneProperties("Warslik's Woods", -467.95f, -1428.95f, 197.31f, 0, ZoneContinent.Kunark);
                        zoneProperties.SetFogProperties(210, 235, 210, 60, 600);
                        zoneProperties.AddZoneLineBox("cabwest", 870.207581f, 1143.751831f, 0.000020f, ZoneLineOrientationType.East,
                            -2237.151123f, -1135.133423f, 381.612640f, -2268.348633f, -1180.958496f, 262.312653f);
                        zoneProperties.AddZoneLineBox("cabwest", 688.666626f, 1327.751099f, 0.000030f, ZoneLineOrientationType.South,
                            -2420.843750f, -917.836975f, 399.112671f, -2473.554932f, -946.380981f, 262.313660f);
                    }
                    break;
                case "westwastes":
                    {
                        zoneProperties.SetFogProperties(128, 128, 160, 200, 1800);
                        zoneProperties.SetBaseZoneProperties("Western Wastes", -3499f, -4099f, -16.66f, 0, ZoneContinent.Velious);
                    }
                    break;
                default:
                    {
                        Logger.WriteLine("GetZonePropertiesForZone error!  No known short name of '" + zoneShortName + "'");
                    }
                    break;
            }

            return zoneProperties;
        }
    }
}
