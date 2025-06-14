﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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

namespace EQWOWConverter.Zones
{
    internal class ZoneEnvironmentSettings
    {
        internal class ZoneEnvironmentParameters
        {
            internal class ZoneEnvironmentParametersTimeSlice
            {
                public int HourTimestamp = 0; // 0-23 (hours since midnight)
                public float FogDistance = 0; // Should be the distance when everything is hidden
                public float FogMultiplier = -0.1f; // Fog Multiplier - Fog Distance * fogMultiplier = fog start distance. 0-0.999
                public float CelestialGlowThrough = 0; // 0-1, how much the moon or sun shows through the the clouds
                public float CloudDensity = 0; // 0-1 = clear-full
                public float UnknownFloat1 = 0.95f;
                public float UnknownFloat2 = 1f; // Everything seems to use this value
                public ColorRGBA SkyCastDiffuseLightColor = new ColorRGBA(); // Sunshine / Moonshine
                public ColorRGBA AmbientLightColor = new ColorRGBA();
                public ColorRGBA SkyTopColor = new ColorRGBA();
                public ColorRGBA SkyMiddleColor = new ColorRGBA();
                public ColorRGBA SkyMiddleToHorizonColor = new ColorRGBA();
                public ColorRGBA SkyAboveHorizonColor = new ColorRGBA();
                public ColorRGBA SkyHorizonColor = new ColorRGBA();
                public ColorRGBA FogColor = new ColorRGBA();
                public ColorRGBA Unknown1Color = new ColorRGBA();
                public ColorRGBA SunColor = new ColorRGBA();
                public ColorRGBA SunLargeHaloColor = new ColorRGBA();
                public ColorRGBA CloudColor = new ColorRGBA();
                public ColorRGBA CloudEdgeColor = new ColorRGBA();
                public ColorRGBA Unknown2Color = new ColorRGBA();
                public ColorRGBA OceanShallowColor = new ColorRGBA();
                public ColorRGBA OceanDeepColor = new ColorRGBA();
                public ColorRGBA RiverShallowColor = new ColorRGBA();
                public ColorRGBA RiverDeepColor = new ColorRGBA();

                public ZoneEnvironmentParametersTimeSlice(int hourTimestamp)
                {
                    HourTimestamp = hourTimestamp;
                }

                public void SetSkyboxElementsToSolidColor(ColorRGBA solidColor)
                {
                    ColorRGBA moddedColor = new ColorRGBA(solidColor).ApplyMod(0.8f);
                    SkyTopColor = moddedColor;
                    SkyMiddleColor = moddedColor;
                    SkyMiddleToHorizonColor = solidColor;
                    SkyAboveHorizonColor = solidColor;
                    SkyHorizonColor = solidColor;
                    SunColor = new ColorRGBA(0, 0, 0, 0);
                    SunLargeHaloColor = new ColorRGBA(0, 0, 0, 0);
                    CloudColor = moddedColor;
                    CloudEdgeColor = moddedColor;
                }
            }

            public List<ZoneEnvironmentParametersTimeSlice> ParametersTimeSlices = new List<ZoneEnvironmentParametersTimeSlice>();

            public void SetAllTimeSlicesCelestialGlowThrough(float celestialGlowThrough)
            {
                foreach (ZoneEnvironmentParametersTimeSlice timeSlice in ParametersTimeSlices)
                    timeSlice.CelestialGlowThrough = celestialGlowThrough;
            }

            public void SetAllTimeSlicesCloudColor(byte red, byte green, byte blue)
            {
                foreach (ZoneEnvironmentParametersTimeSlice timeSlice in ParametersTimeSlices)
                    timeSlice.CloudColor = new ColorRGBA(red, green, blue);
            }

            public void SetAllTimeSlicesFogDistance(float fogDistance)
            {
                foreach (ZoneEnvironmentParametersTimeSlice timeSlice in ParametersTimeSlices)
                    timeSlice.FogDistance = fogDistance;
            }

            public void SetAllTimeSlicesFogDistanceMultiplier(float fogDistanceMultiplier)
            {
                foreach (ZoneEnvironmentParametersTimeSlice timeSlice in ParametersTimeSlices)
                    timeSlice.FogMultiplier = fogDistanceMultiplier;
            }

            public void SetAllTimeSlicesCloudDensity(float cloudDensity)
            {
                foreach (ZoneEnvironmentParametersTimeSlice timeSlice in ParametersTimeSlices)
                    timeSlice.CloudDensity = cloudDensity;
            }

            // DBCIDs
            private static int CURRENT_LIGHTPARAMSID = Configuration.DBCID_LIGHTPARAMS_ID_START;
            private static readonly object LIGHTSPARAMSLock = new object();
            public int DBCLightParamsID;
            public float Glow = 0.5f;
            public int HighlightSky = 0; // Boolean, 1 or 0
            public int SkyboxID = 0;

            public ZoneEnvironmentParameters()
            {
                lock (LIGHTSPARAMSLock)
                {
                    DBCLightParamsID = CURRENT_LIGHTPARAMSID;
                    CURRENT_LIGHTPARAMSID++;
                }
            }        
        }

        // Parameters
        public float PositionX = 0;
        public float PositionY = 0;
        public float PositionZ = 0;
        public float FalloffStart = 0;
        public float FalloffEnd = 0;
        public ZoneEnvironmentParameters ParamatersClearWeather = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersClearWeatherUnderwater = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersStormyWeather = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersStormyWeatherUnderwater = new ZoneEnvironmentParameters();
        
        // References: MapID = 0, Light.dbc = 1, Clear LightParams = 12, Clear Underwater LightParams = 13, Stormy LightParams = 10, Stormy Underwater LightParams = 13
        public void SetAsEasternKingdomsOutdoors()
        {
            ParamatersClearWeather.ParametersTimeSlices.Clear();
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersStormyWeather.ParametersTimeSlices.Clear();
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Clear();

            // Clear Weather (Based on LightParams 12)
            ParamatersClearWeather.Glow = 0.65f;
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersClearWeather.ParametersTimeSlices[0].FogDistance = 500f;
            ParamatersClearWeather.ParametersTimeSlices[1].FogDistance = 500f;
            ParamatersClearWeather.ParametersTimeSlices[2].FogDistance = 444f;
            ParamatersClearWeather.ParametersTimeSlices[3].FogDistance = 500f;
            ParamatersClearWeather.ParametersTimeSlices[4].FogDistance = 472f;
            ParamatersClearWeather.ParametersTimeSlices[5].FogDistance = 481f;
            ParamatersClearWeather.ParametersTimeSlices[0].FogMultiplier = 0.25f;
            ParamatersClearWeather.ParametersTimeSlices[1].FogMultiplier = 0.25f;
            ParamatersClearWeather.ParametersTimeSlices[2].FogMultiplier = 0f;
            ParamatersClearWeather.ParametersTimeSlices[3].FogMultiplier = 0.25f;
            ParamatersClearWeather.ParametersTimeSlices[4].FogMultiplier = 0.25f;
            ParamatersClearWeather.ParametersTimeSlices[5].FogMultiplier = 0.25f;
            ParamatersClearWeather.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[1].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[2].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[3].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[4].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[5].CelestialGlowThrough = 1f;
            ParamatersClearWeather.ParametersTimeSlices[0].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[1].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[2].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[3].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[4].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[5].CloudDensity = 0.5f;
            ParamatersClearWeather.ParametersTimeSlices[0].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[1].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[2].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[3].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[4].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[5].UnknownFloat1 = 0.95f;
            ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(255, 103, 0);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(255, 136, 0);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(255, 109, 0);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162);
            ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(29, 60, 84);
            ParamatersClearWeather.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(29, 60, 84);
            ParamatersClearWeather.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(93, 125, 156);
            ParamatersClearWeather.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(104, 130, 154);
            ParamatersClearWeather.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(102, 92, 119);
            ParamatersClearWeather.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(29, 60, 84);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(35, 74, 84);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(0, 31, 73);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(47, 53, 62);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(68, 140, 128);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(58, 162, 207);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(107, 92, 128);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(210, 121, 72);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(153, 220, 245);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(148, 119, 101);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(16, 40, 72);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(255, 171, 64);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(175, 218, 224);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(181, 115, 66);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(49, 86, 123);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(49, 60, 99);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(255, 202, 76);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(180, 180, 180);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(199, 130, 43);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(49, 86, 123);
            ParamatersClearWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(0, 14, 33);
            ParamatersClearWeather.ParametersTimeSlices[1].FogColor = new ColorRGBA(0, 14, 33);
            ParamatersClearWeather.ParametersTimeSlices[2].FogColor = new ColorRGBA(93, 125, 109);
            ParamatersClearWeather.ParametersTimeSlices[3].FogColor = new ColorRGBA(77, 120, 143);
            ParamatersClearWeather.ParametersTimeSlices[4].FogColor = new ColorRGBA(91, 61, 75);
            ParamatersClearWeather.ParametersTimeSlices[5].FogColor = new ColorRGBA(0, 14, 33);
            ParamatersClearWeather.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(92, 92, 92);
            ParamatersClearWeather.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(92, 92, 92);
            ParamatersClearWeather.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(110, 110, 110);
            ParamatersClearWeather.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(77, 77, 77);
            ParamatersClearWeather.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(110, 110, 110);
            ParamatersClearWeather.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(92, 92, 92);
            ParamatersClearWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[1].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[2].SunColor = new ColorRGBA(255, 210, 150);
            ParamatersClearWeather.ParametersTimeSlices[3].SunColor = new ColorRGBA(255, 247, 222);
            ParamatersClearWeather.ParametersTimeSlices[4].SunColor = new ColorRGBA(255, 226, 169);
            ParamatersClearWeather.ParametersTimeSlices[5].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
            ParamatersClearWeather.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(58, 95, 128);
            ParamatersClearWeather.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(255, 179, 60);
            ParamatersClearWeather.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(255, 199, 138);
            ParamatersClearWeather.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(255, 212, 160);
            ParamatersClearWeather.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
            ParamatersClearWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(18, 56, 81);
            ParamatersClearWeather.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(19, 67, 81);
            ParamatersClearWeather.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(68, 162, 171);
            ParamatersClearWeather.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(43, 105, 132);
            ParamatersClearWeather.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(138, 93, 72);
            ParamatersClearWeather.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(18, 56, 81);
            ParamatersClearWeather.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[1].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[2].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[3].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[4].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[5].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 50, 75);
            ParamatersClearWeather.ParametersTimeSlices[1].Unknown2Color = new ColorRGBA(0, 50, 75);
            ParamatersClearWeather.ParametersTimeSlices[2].Unknown2Color = new ColorRGBA(32, 76, 111);
            ParamatersClearWeather.ParametersTimeSlices[3].Unknown2Color = new ColorRGBA(97, 130, 183);
            ParamatersClearWeather.ParametersTimeSlices[4].Unknown2Color = new ColorRGBA(93, 80, 120);
            ParamatersClearWeather.ParametersTimeSlices[5].Unknown2Color = new ColorRGBA(93, 80, 120);
            ParamatersClearWeather.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(33, 62, 47);
            ParamatersClearWeather.ParametersTimeSlices[1].OceanShallowColor = new ColorRGBA(33, 62, 47);
            ParamatersClearWeather.ParametersTimeSlices[2].OceanShallowColor = new ColorRGBA(91, 72, 66);
            ParamatersClearWeather.ParametersTimeSlices[3].OceanShallowColor = new ColorRGBA(17, 75, 89);
            ParamatersClearWeather.ParametersTimeSlices[4].OceanShallowColor = new ColorRGBA(90, 72, 70);
            ParamatersClearWeather.ParametersTimeSlices[5].OceanShallowColor = new ColorRGBA(33, 62, 47);
            ParamatersClearWeather.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersClearWeather.ParametersTimeSlices[1].OceanDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersClearWeather.ParametersTimeSlices[2].OceanDeepColor = new ColorRGBA(20, 48, 47);
            ParamatersClearWeather.ParametersTimeSlices[3].OceanDeepColor = new ColorRGBA(0, 29, 41);
            ParamatersClearWeather.ParametersTimeSlices[4].OceanDeepColor = new ColorRGBA(21, 34, 53);
            ParamatersClearWeather.ParametersTimeSlices[5].OceanDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersClearWeather.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(26, 60, 30);
            ParamatersClearWeather.ParametersTimeSlices[1].RiverShallowColor = new ColorRGBA(39, 66, 60);
            ParamatersClearWeather.ParametersTimeSlices[2].RiverShallowColor = new ColorRGBA(47, 58, 55);
            ParamatersClearWeather.ParametersTimeSlices[3].RiverShallowColor = new ColorRGBA(79, 93, 20);
            ParamatersClearWeather.ParametersTimeSlices[4].RiverShallowColor = new ColorRGBA(39, 66, 60);
            ParamatersClearWeather.ParametersTimeSlices[5].RiverShallowColor = new ColorRGBA(26, 60, 30);
            ParamatersClearWeather.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(39, 55, 47);
            ParamatersClearWeather.ParametersTimeSlices[1].RiverDeepColor = new ColorRGBA(36, 58, 60);
            ParamatersClearWeather.ParametersTimeSlices[2].RiverDeepColor = new ColorRGBA(46, 75, 77);
            ParamatersClearWeather.ParametersTimeSlices[3].RiverDeepColor = new ColorRGBA(51, 82, 85);
            ParamatersClearWeather.ParametersTimeSlices[4].RiverDeepColor = new ColorRGBA(36, 58, 60);
            ParamatersClearWeather.ParametersTimeSlices[5].RiverDeepColor = new ColorRGBA(39, 55, 47);

            // Clear Weather Underwater (Based on LightParams 13)
            ParamatersClearWeatherUnderwater.Glow = 1;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CelestialGlowThrough = 1f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudDensity = 0f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].UnknownFloat1 = 0.95f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].RiverShallowColor = new ColorRGBA(91, 72, 66);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].RiverShallowColor = new ColorRGBA(78, 63, 55);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].RiverShallowColor = new ColorRGBA(90, 72, 70);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].RiverDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].RiverDeepColor = new ColorRGBA(63, 85, 75);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].RiverDeepColor = new ColorRGBA(61, 94, 113);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].RiverDeepColor = new ColorRGBA(91, 61, 75);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].RiverDeepColor = new ColorRGBA(91, 61, 75);

            // Stormy Weather (Based on LightParams 10)
            ParamatersStormyWeather.Glow = 0.5f;
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeather.ParametersTimeSlices[0].FogDistance = 278f;
            ParamatersStormyWeather.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersStormyWeather.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudDensity = 0.95f;
            ParamatersStormyWeather.ParametersTimeSlices[0].UnknownFloat1 = 0f;
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(101, 101, 101);
            ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(78, 78, 95);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(111, 111, 111);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(109, 109, 109);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(111, 111, 111);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(103, 103, 103);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(82, 83, 82);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(82, 84, 82);
            ParamatersStormyWeather.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(56, 56, 56);
            ParamatersStormyWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(88, 88, 88);
            ParamatersStormyWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(195, 195, 181);
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(61, 63, 62);
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(77, 77, 77);
            ParamatersStormyWeather.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(67, 67, 67);
            ParamatersStormyWeather.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(77, 77, 77);
            ParamatersStormyWeather.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(67, 67, 67);

            // Stormy Weather Underwater (Based on LightParams 13)
            ParamatersStormyWeatherUnderwater.Glow = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CelestialGlowThrough = 1f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudDensity = 0f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].UnknownFloat1 = 0.95f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogColor = new ColorRGBA(20, 61, 61);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogColor = new ColorRGBA(37, 92, 92);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].OceanShallowColor = new ColorRGBA(37, 61, 76);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].OceanShallowColor = new ColorRGBA(64, 101, 125);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].OceanDeepColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].RiverShallowColor = new ColorRGBA(91, 72, 66);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].RiverShallowColor = new ColorRGBA(78, 63, 55);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].RiverShallowColor = new ColorRGBA(90, 72, 70);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].RiverShallowColor = new ColorRGBA(58, 55, 47);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].RiverDeepColor = new ColorRGBA(0, 12, 24);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].RiverDeepColor = new ColorRGBA(63, 85, 75);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].RiverDeepColor = new ColorRGBA(61, 94, 113);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].RiverDeepColor = new ColorRGBA(91, 61, 75);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].RiverDeepColor = new ColorRGBA(91, 61, 75);
        }

        // Used for outdoor zones that are foggy, but also have day/night cycles and potentially weather
        public void SetAsOutdoors(byte fogRedNoon, byte fogGreenNoon, byte fogBlueNoon, ZoneFogType fogType, bool isSkyVisible, float cloudDensity, 
            float ambientBrightnessMod, ZoneSkySpecialType skyTypeSpecial)
        {
            float fogDistance = 0;
            float fogDistanceMultiplier = 0;
            switch(fogType)
            {
                case ZoneFogType.Heavy:
                    {
                        fogDistance = 28f;
                        fogDistanceMultiplier = -0.95f;
                    } break;
                case ZoneFogType.Medium:
                    {
                        fogDistance = 250f;
                        fogDistanceMultiplier = -0.95f;
                    }
                    break;
                case ZoneFogType.Clear:
                    {
                        fogDistance = 500f;
                        fogDistanceMultiplier = 0.25f;
                    } break;
                default:
                    {
                        Logger.WriteError("Error in SetAsOutdoorFogg, as ZoneOutdoorFogType is '" + fogType + "'");
                    } break;
            }

            // Calculate the fog colors based on time of day, using the max as the base brightness
            float fogColorTime0BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_0) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime0 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime0BrightMod);
            float fogColorTime3BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_3) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime3 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime3BrightMod);
            float fogColorTime6BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_6) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime6 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime6BrightMod);
            float fogColorTime12BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime12 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime12BrightMod);
            float fogColorTime21BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_21) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime21 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime21BrightMod);
            float fogColorTime22BrightMod = Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_22) / Convert.ToSingle(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12);
            ColorRGBA fogColorTime22 = new ColorRGBA(fogRedNoon, fogGreenNoon, fogBlueNoon).ApplyMod(fogColorTime22BrightMod);

            // Clear Weather
            ParamatersClearWeather.ParametersTimeSlices.Clear();
            ParamatersClearWeather.Glow = Configuration.LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER;
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            if (isSkyVisible == true)
            {
                if (skyTypeSpecial == ZoneSkySpecialType.FearPlane)
                {
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(84, 74, 35);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(73, 31, 0);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(62, 53, 47);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(32, 12, 0);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(32, 12, 0);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(128, 140, 68);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(207, 162, 56);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(128, 92, 107);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(32, 12, 0);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(78, 40, 0);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(78, 40, 0);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(210, 121, 72);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(245, 220, 153);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(148, 119, 101);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(78, 40, 0);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(112, 70, 27);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(72, 40, 16);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(255, 171, 64);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(224, 218, 174);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(181, 115, 66);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(112, 70, 27);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(123, 86, 49);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(99, 60, 49);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(255, 202, 76);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(180, 180, 180);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(199, 130, 43);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(123, 86, 43);
                    ParamatersClearWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(255, 241, 232);
                    ParamatersClearWeather.ParametersTimeSlices[1].SunColor = new ColorRGBA(255, 241, 223);
                    ParamatersClearWeather.ParametersTimeSlices[2].SunColor = new ColorRGBA(255, 210, 150);
                    ParamatersClearWeather.ParametersTimeSlices[3].SunColor = new ColorRGBA(255, 247, 222);
                    ParamatersClearWeather.ParametersTimeSlices[4].SunColor = new ColorRGBA(255, 226, 169);
                    ParamatersClearWeather.ParametersTimeSlices[5].SunColor = new ColorRGBA(255, 241, 232);
                    ParamatersClearWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(132, 97, 50);
                    ParamatersClearWeather.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(128, 95, 53);
                    ParamatersClearWeather.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(255, 179, 60);
                    ParamatersClearWeather.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(255, 199, 138);
                    ParamatersClearWeather.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(255, 212, 160);
                    ParamatersClearWeather.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(132, 97, 50);
                    ParamatersClearWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(81, 56, 18);
                    ParamatersClearWeather.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(81, 67, 19);
                    ParamatersClearWeather.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(171, 162, 68);
                    ParamatersClearWeather.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(132, 105, 43);
                    ParamatersClearWeather.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(138, 93, 72);
                    ParamatersClearWeather.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(81, 56, 18);
                }
                else
                {
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(35, 74, 84);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(0, 31, 73);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(47, 53, 62);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(0, 0, 0);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(0, 12, 32);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(0, 12, 32);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(68, 140, 128);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(58, 162, 207);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(107, 92, 128);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(0, 12, 32);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(210, 121, 72);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(153, 220, 245);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(148, 119, 101);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(16, 40, 72);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(255, 171, 64);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(175, 218, 224);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(181, 115, 66);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
                    ParamatersClearWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(49, 86, 123);
                    ParamatersClearWeather.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(49, 60, 99);
                    ParamatersClearWeather.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(255, 202, 76);
                    ParamatersClearWeather.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(180, 180, 180);
                    ParamatersClearWeather.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(199, 130, 43);
                    ParamatersClearWeather.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(49, 86, 123);
                    ParamatersClearWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(232, 241, 255);
                    ParamatersClearWeather.ParametersTimeSlices[1].SunColor = new ColorRGBA(232, 241, 255);
                    ParamatersClearWeather.ParametersTimeSlices[2].SunColor = new ColorRGBA(255, 210, 150);
                    ParamatersClearWeather.ParametersTimeSlices[3].SunColor = new ColorRGBA(255, 247, 222);
                    ParamatersClearWeather.ParametersTimeSlices[4].SunColor = new ColorRGBA(255, 226, 169);
                    ParamatersClearWeather.ParametersTimeSlices[5].SunColor = new ColorRGBA(232, 241, 255);
                    ParamatersClearWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
                    ParamatersClearWeather.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(58, 95, 128);
                    ParamatersClearWeather.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(255, 179, 60);
                    ParamatersClearWeather.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(255, 199, 138);
                    ParamatersClearWeather.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(255, 212, 160);
                    ParamatersClearWeather.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
                    ParamatersClearWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(18, 56, 81);
                    ParamatersClearWeather.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(19, 67, 81);
                    ParamatersClearWeather.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(68, 162, 171);
                    ParamatersClearWeather.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(43, 105, 132);
                    ParamatersClearWeather.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(138, 93, 72);
                    ParamatersClearWeather.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(18, 56, 81);
                }
                ParamatersClearWeather.SetAllTimeSlicesCloudColor(0, 0, 0);
                ParamatersClearWeather.SetAllTimeSlicesCelestialGlowThrough(1f);
            }
            else
            {
                ParamatersClearWeather.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(fogColorTime0);
                ParamatersClearWeather.ParametersTimeSlices[1].SetSkyboxElementsToSolidColor(fogColorTime3);
                ParamatersClearWeather.ParametersTimeSlices[2].SetSkyboxElementsToSolidColor(fogColorTime6);
                ParamatersClearWeather.ParametersTimeSlices[3].SetSkyboxElementsToSolidColor(fogColorTime12);
                ParamatersClearWeather.ParametersTimeSlices[4].SetSkyboxElementsToSolidColor(fogColorTime21);
                ParamatersClearWeather.ParametersTimeSlices[5].SetSkyboxElementsToSolidColor(fogColorTime22);
                ParamatersClearWeather.SetAllTimeSlicesCelestialGlowThrough(0f);
            }
            ParamatersClearWeather.SetAllTimeSlicesCloudDensity(cloudDensity);
            ParamatersClearWeather.SetAllTimeSlicesFogDistance(fogDistance);
            ParamatersClearWeather.SetAllTimeSlicesFogDistanceMultiplier(fogDistanceMultiplier);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(128, 110, 113).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(128, 96, 64).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(128, 110, 113).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_0,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_0,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_0).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_3,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_3,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_3).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_6,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_6,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_6).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_12).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_21,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_21,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_21).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_22,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_22,
                                                                                             Configuration.LIGHT_OUTSIDE_AMBIENT_TIME_22).ApplyMod(ambientBrightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[0].FogColor = fogColorTime0;
            ParamatersClearWeather.ParametersTimeSlices[1].FogColor = fogColorTime3;
            ParamatersClearWeather.ParametersTimeSlices[2].FogColor = fogColorTime6;
            ParamatersClearWeather.ParametersTimeSlices[3].FogColor = fogColorTime12;
            ParamatersClearWeather.ParametersTimeSlices[4].FogColor = fogColorTime21;
            ParamatersClearWeather.ParametersTimeSlices[5].FogColor = fogColorTime22;
            ParamatersClearWeather.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(92, 92, 92);
            ParamatersClearWeather.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(92, 92, 92);
            ParamatersClearWeather.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(110, 110, 110);
            ParamatersClearWeather.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(77, 77, 77);
            ParamatersClearWeather.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(110, 110, 110);
            ParamatersClearWeather.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(92, 92, 92);            
            ParamatersClearWeather.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 50, 75);
            ParamatersClearWeather.ParametersTimeSlices[1].Unknown2Color = new ColorRGBA(0, 50, 75);
            ParamatersClearWeather.ParametersTimeSlices[2].Unknown2Color = new ColorRGBA(32, 76, 111);
            ParamatersClearWeather.ParametersTimeSlices[3].Unknown2Color = new ColorRGBA(97, 130, 183);
            ParamatersClearWeather.ParametersTimeSlices[4].Unknown2Color = new ColorRGBA(93, 80, 120);
            ParamatersClearWeather.ParametersTimeSlices[5].Unknown2Color = new ColorRGBA(93, 80, 120);

            // Clear Weather - Underwater
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersClearWeatherUnderwater.Glow = Configuration.LIGHT_OUTSIDE_GLOW_UNDERWATER;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistance(167f);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistanceMultiplier(-0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(fogColorTime0, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogColor = ColorRGBA.GetBlendedColor(fogColorTime3, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogColor = ColorRGBA.GetBlendedColor(fogColorTime6, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogColor = ColorRGBA.GetBlendedColor(fogColorTime12, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogColor = ColorRGBA.GetBlendedColor(fogColorTime21, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogColor = ColorRGBA.GetBlendedColor(fogColorTime22, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[1].AmbientLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[2].AmbientLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[3].AmbientLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[4].AmbientLightColor, 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[5].AmbientLightColor, 0.5f);
            if (isSkyVisible == true)
            {                
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersClearWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(1f);
            }
            else
            {
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime0, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime3, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime6, new ColorRGBA(126, 167, 167), 0.5f));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime12, new ColorRGBA(201, 242, 242), 0.5f));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime21, new ColorRGBA(154, 195, 195), 0.5f));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime22, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersClearWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(0f);
            }
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCloudDensity(0);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCloudColor(0, 0, 0);

            // Stormy Weather
            ParamatersStormyWeather.ParametersTimeSlices.Clear();
            ParamatersStormyWeather.Glow = Configuration.LIGHT_OUTSIDE_GLOW_STORMY_WEATHER;
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeather.ParametersTimeSlices[0].FogDistance = 278f;
            ParamatersStormyWeather.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersStormyWeather.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudDensity = 0.95f;
            ParamatersStormyWeather.ParametersTimeSlices[0].UnknownFloat1 = 0f;
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(101, 101, 101);
            ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(78, 78, 95);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(111, 111, 111);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(109, 109, 109);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(111, 111, 111);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(103, 103, 103);
            ParamatersStormyWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(82, 83, 82);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(82, 84, 82);
            ParamatersStormyWeather.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(56, 56, 56);
            ParamatersStormyWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(88, 88, 88);
            ParamatersStormyWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(195, 195, 181);
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(61, 63, 62);
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(77, 77, 77);
            ParamatersStormyWeather.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(67, 67, 67);
            ParamatersStormyWeather.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(77, 77, 77);
            ParamatersStormyWeather.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(67, 67, 67);

            // Stormy Weather - Underwater
            // TODO
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersStormyWeatherUnderwater.Glow = Configuration.LIGHT_OUTSIDE_GLOW_UNDERWATER;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersStormyWeatherUnderwater.SetAllTimeSlicesFogDistance(167f);
            ParamatersStormyWeatherUnderwater.SetAllTimeSlicesFogDistanceMultiplier(-0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(fogColorTime0, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogColor = ColorRGBA.GetBlendedColor(fogColorTime3, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogColor = ColorRGBA.GetBlendedColor(fogColorTime6, new ColorRGBA(20, 61, 61), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogColor = ColorRGBA.GetBlendedColor(fogColorTime12, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogColor = ColorRGBA.GetBlendedColor(fogColorTime21, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogColor = ColorRGBA.GetBlendedColor(fogColorTime22, new ColorRGBA(37, 92, 92), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[1].AmbientLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[2].AmbientLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[3].AmbientLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[4].AmbientLightColor, 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[5].AmbientLightColor, 0.5f);
            if (isSkyVisible == true)
            {
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
                ParamatersStormyWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(1f);
            }
            else
            {
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime0, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime3, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime6, new ColorRGBA(126, 167, 167), 0.5f));
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime12, new ColorRGBA(201, 242, 242), 0.5f));
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime21, new ColorRGBA(154, 195, 195), 0.5f));
                ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColorTime22, new ColorRGBA(20, 61, 61), 0.5f));
                ParamatersStormyWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(0f);
            }
            ParamatersStormyWeatherUnderwater.SetAllTimeSlicesCloudDensity(0);
            ParamatersStormyWeatherUnderwater.SetAllTimeSlicesCloudColor(0, 0, 0);
        }

        // Used for indoor zones that have no day/night cycle, no weather, and are obscured by fog
        public void SetAsIndoors(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, byte ambientRed, byte ambientGreen, byte ambientBlue)
        {
            float fogDistance = 0;
            float fogDistanceMultiplier = 0;
            switch (fogType)
            {
                case ZoneFogType.Heavy:
                    {
                        fogDistance = 28f;
                        fogDistanceMultiplier = -0.9f;
                    }
                    break;
                case ZoneFogType.Medium:
                    {
                        fogDistance = 250f;
                        fogDistanceMultiplier = -0.5f;
                    }
                    break;
                case ZoneFogType.Clear:
                    {
                        fogDistance = 500f;
                        fogDistanceMultiplier = 0.25f;
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Error in SetAsIndoors, as ZoneFogType is '" + fogType + "'");
                    }
                    break;
            }

            ParamatersClearWeather.ParametersTimeSlices.Clear();
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(ambientRed, ambientGreen, ambientBlue);
            ParamatersClearWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].FogDistance = fogDistance;
            ParamatersClearWeather.ParametersTimeSlices[0].FogMultiplier = fogDistanceMultiplier;
            ParamatersClearWeather.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersClearWeather.ParametersTimeSlices[0].CloudDensity = 1f;

            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(166, 201, 234), new ColorRGBA(ambientRed, ambientGreen, ambientBlue), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(new ColorRGBA(20, 61, 61), new ColorRGBA(fogRed, fogGreen, fogBlue), 0.5f);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 1f;

            ParamatersStormyWeather.ParametersTimeSlices.Clear();
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(ambientRed, ambientGreen, ambientBlue);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogDistance = fogDistance;
            ParamatersStormyWeather.ParametersTimeSlices[0].FogMultiplier = fogDistanceMultiplier;
            ParamatersStormyWeather.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudDensity = 1f;

            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(166, 201, 234), new ColorRGBA(ambientRed, ambientGreen, ambientBlue), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(new ColorRGBA(20, 61, 61), new ColorRGBA(fogRed, fogGreen, fogBlue), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 1f;
        }

        // Used for indoor zones that have a day/night cycle, no weather, are obscured by fog, but the sky should match normal outdoor sky
        public void SetAsIndoorsWithSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, byte ambientRed, byte ambientGreen, byte ambientBlue, float cloudDensity)
        {
            float fogDistance = 0;
            float fogDistanceMultiplier = 0;
            switch (fogType)
            {
                case ZoneFogType.Heavy:
                    {
                        fogDistance = 28f;
                        fogDistanceMultiplier = -0.9f;
                    }
                    break;
                case ZoneFogType.Medium:
                    {
                        fogDistance = 250f;
                        fogDistanceMultiplier = -0.5f;
                    }
                    break;
                case ZoneFogType.Clear:
                    {
                        fogDistance = 500f;
                        fogDistanceMultiplier = 0.25f;
                    }
                    break;
                default:
                    {
                        Logger.WriteError("Error in SetAsIndoorsWithSky, as ZoneFogType is '" + fogType + "'");
                    }
                    break;
            }

            ParamatersClearWeather.ParametersTimeSlices.Clear();
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            for (int i = 0; i < 6; i++)
            {
                ParamatersClearWeather.ParametersTimeSlices[i].AmbientLightColor = new ColorRGBA(ambientRed, ambientGreen, ambientBlue);
                ParamatersClearWeather.ParametersTimeSlices[i].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            }
            ParamatersClearWeather.SetAllTimeSlicesCloudColor(0, 0, 0);
            ParamatersClearWeather.SetAllTimeSlicesCelestialGlowThrough(1f);
            ParamatersClearWeather.SetAllTimeSlicesCloudDensity(cloudDensity);
            ParamatersClearWeather.SetAllTimeSlicesFogDistance(fogDistance);
            ParamatersClearWeather.SetAllTimeSlicesFogDistanceMultiplier(fogDistanceMultiplier);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(35, 74, 84);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(0, 31, 73);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(47, 53, 62);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(68, 140, 128);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(58, 162, 207);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(107, 92, 128);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(0, 12, 32);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(210, 121, 72);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(153, 220, 245);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(148, 119, 101);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(0, 40, 78);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(16, 40, 72);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(255, 171, 64);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(175, 218, 224);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(181, 115, 66);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(27, 70, 112);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(49, 86, 123);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(49, 60, 99);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(255, 202, 76);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(180, 180, 180);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(199, 130, 43);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(49, 86, 123);
            ParamatersClearWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[1].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[2].SunColor = new ColorRGBA(255, 210, 150);
            ParamatersClearWeather.ParametersTimeSlices[3].SunColor = new ColorRGBA(255, 247, 222);
            ParamatersClearWeather.ParametersTimeSlices[4].SunColor = new ColorRGBA(255, 226, 169);
            ParamatersClearWeather.ParametersTimeSlices[5].SunColor = new ColorRGBA(232, 241, 255);
            ParamatersClearWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
            ParamatersClearWeather.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(58, 95, 128);
            ParamatersClearWeather.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(255, 179, 60);
            ParamatersClearWeather.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(255, 199, 138);
            ParamatersClearWeather.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(255, 212, 160);
            ParamatersClearWeather.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(50, 97, 132);
            ParamatersClearWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(18, 56, 81);
            ParamatersClearWeather.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(19, 67, 81);
            ParamatersClearWeather.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(68, 162, 171);
            ParamatersClearWeather.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(43, 105, 132);
            ParamatersClearWeather.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(138, 93, 72);
            ParamatersClearWeather.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(18, 56, 81);

            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistance(167f);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistanceMultiplier(-0.5f);
            for (int i = 0; i < 6; i++)
            {
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[i].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(166, 201, 234), new ColorRGBA(ambientRed, ambientGreen, ambientBlue), 0.5f);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[i].FogColor = ColorRGBA.GetBlendedColor(new ColorRGBA(20, 61, 61), new ColorRGBA(fogRed, fogGreen, fogBlue), 0.5f);
            }
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(1f);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCloudDensity(cloudDensity);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCloudColor(0, 0, 0);

            ParamatersStormyWeather.ParametersTimeSlices.Clear();
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(ambientRed, ambientGreen, ambientBlue);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogDistance = fogDistance;
            ParamatersStormyWeather.ParametersTimeSlices[0].FogMultiplier = fogDistanceMultiplier;
            ParamatersStormyWeather.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersStormyWeather.ParametersTimeSlices[0].CloudDensity = 1f;

            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(166, 201, 234), new ColorRGBA(ambientRed, ambientGreen, ambientBlue), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(new ColorRGBA(20, 61, 61), new ColorRGBA(fogRed, fogGreen, fogBlue), 0.5f);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(new ColorRGBA(fogRed, fogGreen, fogBlue));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 1f;
        }
    }
}
