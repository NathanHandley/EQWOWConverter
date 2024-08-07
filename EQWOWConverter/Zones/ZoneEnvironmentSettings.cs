using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            private static int CURRENT_LIGHTPARAMSID = Configuration.CONFIG_DBCID_LIGHTPARAMS_START;
            public int DBCLightParamsID;
            public float Glow = 0.5f;
            public int HighlightSky = 0; // Boolean, 1 or 0
            public int SkyboxID = 0;

            public ZoneEnvironmentParameters()
            {
                DBCLightParamsID = CURRENT_LIGHTPARAMSID;
                CURRENT_LIGHTPARAMSID++;
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
        public void SetAsOutdoors(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, bool isSkyVisible, float cloudDensity, float brightnessMod, float ambientOnFogInfluenceMod)
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

            ColorRGBA fogColor = new ColorRGBA(fogRed, fogGreen, fogBlue);

            // Clear Weather
            ParamatersClearWeather.ParametersTimeSlices.Clear();
            ParamatersClearWeather.Glow = Configuration.CONFIG_LIGHT_OUTSIDE_GLOW_CLEAR_WEATHER;
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            if (isSkyVisible == false)
            {
                ParamatersClearWeather.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.ParametersTimeSlices[1].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.ParametersTimeSlices[2].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(164, 164, 164), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.ParametersTimeSlices[3].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(239, 239, 239), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.ParametersTimeSlices[4].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(192, 192, 192), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.ParametersTimeSlices[5].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeather.SetAllTimeSlicesCelestialGlowThrough(0f);
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
                ParamatersClearWeather.SetAllTimeSlicesCloudColor(0, 0, 0);
                ParamatersClearWeather.SetAllTimeSlicesCelestialGlowThrough(1f);
            }
            ParamatersClearWeather.SetAllTimeSlicesCloudDensity(cloudDensity);
            ParamatersClearWeather.SetAllTimeSlicesFogDistance(fogDistance);
            ParamatersClearWeather.SetAllTimeSlicesFogDistanceMultiplier(fogDistanceMultiplier);
            ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(128, 110, 113).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(128, 96, 64).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(128, 110, 113).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(97, 130, 162).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(58, 58, 58).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(58, 58, 58).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(164, 164, 164).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(239, 239, 239).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(192, 192, 192).ApplyMod(brightnessMod);
            ParamatersClearWeather.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(58, 58, 58).ApplyMod(brightnessMod);            
            ParamatersClearWeather.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
            ParamatersClearWeather.ParametersTimeSlices[1].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
            ParamatersClearWeather.ParametersTimeSlices[2].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(164, 164, 164), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
            ParamatersClearWeather.ParametersTimeSlices[3].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(239, 239, 239), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
            ParamatersClearWeather.ParametersTimeSlices[4].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(192, 192, 192), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
            ParamatersClearWeather.ParametersTimeSlices[5].FogColor = ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(58, 58, 58), ambientOnFogInfluenceMod).ApplyMod(brightnessMod); // Fog + Ambient
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
            ParamatersClearWeatherUnderwater.Glow = Configuration.CONFIG_LIGHT_OUTSIDE_GLOW_UNDERWATER;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistance(167f);
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesFogDistanceMultiplier(-0.5f);
            if (fogType == ZoneFogType.Heavy)
            {
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[1].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[2].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[3].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[4].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = ColorRGBA.GetBlendedColor(new ColorRGBA(66, 101, 134), ParamatersClearWeather.ParametersTimeSlices[5].SkyCastDiffuseLightColor, 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(20, 61, 61), 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(20, 61, 61), 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(20, 61, 61), 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(37, 92, 92), 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(37, 92, 92), 0.5f).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogColor = ColorRGBA.GetBlendedColorWithMaintainedBColorIntensity(fogColor, new ColorRGBA(37, 92, 92), 0.5f).ApplyMod(brightnessMod);
            }
            else
            {
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134).ApplyMod(brightnessMod);                
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(20, 61, 61).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].FogColor = new ColorRGBA(20, 61, 61).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].FogColor = new ColorRGBA(20, 61, 61).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].FogColor = new ColorRGBA(37, 92, 92).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].FogColor = new ColorRGBA(37, 92, 92).ApplyMod(brightnessMod);
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].FogColor = new ColorRGBA(37, 92, 92).ApplyMod(brightnessMod);
            }
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
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(20, 61, 61), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(20, 61, 61), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(126, 167, 167), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(201, 242, 242), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(154, 195, 195), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].SetSkyboxElementsToSolidColor(ColorRGBA.GetBlendedColor(fogColor, new ColorRGBA(20, 61, 61), ambientOnFogInfluenceMod).ApplyMod(brightnessMod));
                ParamatersClearWeatherUnderwater.SetAllTimeSlicesCelestialGlowThrough(0f);
            }
            ParamatersClearWeatherUnderwater.SetAllTimeSlicesCloudDensity(0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[1].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[2].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[3].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[4].CloudColor = new ColorRGBA(0, 0, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[5].CloudColor = new ColorRGBA(0, 0, 0);

            // Stormy Weather
            // TODO
            ParamatersStormyWeather.ParametersTimeSlices.Clear();
            ParamatersStormyWeather.Glow = Configuration.CONFIG_LIGHT_OUTSIDE_GLOW_STORMY_WEATHER;
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            //ParamatersStormyWeather.ParametersTimeSlices[0].FogDistance = 278f;
            //ParamatersStormyWeather.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            //ParamatersStormyWeather.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            //ParamatersStormyWeather.ParametersTimeSlices[0].CloudDensity = 0.95f;
            //ParamatersStormyWeather.ParametersTimeSlices[0].UnknownFloat1 = 0f;
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(101, 101, 101);
            //ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(78, 78, 95);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(111, 111, 111);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(109, 109, 109);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(111, 111, 111);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(103, 103, 103);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(82, 83, 82);
            //ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(82, 84, 82);
            //ParamatersStormyWeather.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(56, 56, 56);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SunColor = new ColorRGBA(88, 88, 88);
            //ParamatersStormyWeather.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(195, 195, 181);
            //ParamatersStormyWeather.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(61, 63, 62);
            //ParamatersStormyWeather.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeather.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeather.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(77, 77, 77);
            //ParamatersStormyWeather.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(67, 67, 67);
            //ParamatersStormyWeather.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(77, 77, 77);
            //ParamatersStormyWeather.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(67, 67, 67);

            // Stormy Weather - Underwater
            // TODO
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Clear();
            ParamatersStormyWeatherUnderwater.Glow = Configuration.CONFIG_LIGHT_OUTSIDE_GLOW_UNDERWATER;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(3));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(6));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(12));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(21));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(22));
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogDistance = 167f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogMultiplier = -0.5f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CelestialGlowThrough = 1f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudDensity = 0f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].UnknownFloat1 = 0.95f;
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyCastDiffuseLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].AmbientLightColor = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyTopColor = new ColorRGBA(30, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleColor = new ColorRGBA(53, 107, 136);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyMiddleToHorizonColor = new ColorRGBA(54, 112, 142);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyAboveHorizonColor = new ColorRGBA(56, 113, 141);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SkyHorizonColor = new ColorRGBA(66, 132, 166);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(20, 61, 61);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].FogColor = new ColorRGBA(20, 61, 61);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].FogColor = new ColorRGBA(20, 61, 61);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].FogColor = new ColorRGBA(37, 92, 92);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].FogColor = new ColorRGBA(37, 92, 92);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].FogColor = new ColorRGBA(37, 92, 92);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].Unknown1Color = new ColorRGBA(66, 101, 134);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunColor = new ColorRGBA(91, 128, 158);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].SunLargeHaloColor = new ColorRGBA(82, 106, 123);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudEdgeColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].CloudColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].Unknown2Color = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].OceanShallowColor = new ColorRGBA(37, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].OceanShallowColor = new ColorRGBA(37, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].OceanShallowColor = new ColorRGBA(37, 61, 76);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].OceanShallowColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].OceanShallowColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].OceanShallowColor = new ColorRGBA(64, 101, 125);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].OceanDeepColor = new ColorRGBA(0, 0, 0);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].RiverShallowColor = new ColorRGBA(58, 55, 47);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].RiverShallowColor = new ColorRGBA(58, 55, 47);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].RiverShallowColor = new ColorRGBA(91, 72, 66);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].RiverShallowColor = new ColorRGBA(78, 63, 55);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].RiverShallowColor = new ColorRGBA(90, 72, 70);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].RiverShallowColor = new ColorRGBA(58, 55, 47);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].RiverDeepColor = new ColorRGBA(0, 12, 24);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[1].RiverDeepColor = new ColorRGBA(0, 12, 24);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[2].RiverDeepColor = new ColorRGBA(63, 85, 75);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[3].RiverDeepColor = new ColorRGBA(61, 94, 113);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[4].RiverDeepColor = new ColorRGBA(91, 61, 75);
            //ParamatersStormyWeatherUnderwater.ParametersTimeSlices[5].RiverDeepColor = new ColorRGBA(91, 61, 75);
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
    }
}
