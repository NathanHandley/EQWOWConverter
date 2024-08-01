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
                public float CloudDensity = 0; // 0-1 = clear-full
                public ColorRGBA SkyCastDiffuseLight = new ColorRGBA(); // Sunshine / Moonshine
                public ColorRGBA AmbientLight = new ColorRGBA();
                public ColorRGBA SkyTopColor = new ColorRGBA();
                public ColorRGBA SkyMiddleColor = new ColorRGBA();
                public ColorRGBA SkyMiddleToHorizonColor = new ColorRGBA();
                public ColorRGBA SkyAboveHorizonColor = new ColorRGBA();
                public ColorRGBA SkyHorizonColor = new ColorRGBA();
                public ColorRGBA FogColor = new ColorRGBA();
                public ColorRGBA SunColor = new ColorRGBA();
                public ColorRGBA SunLargeHaloColor = new ColorRGBA();
                public ColorRGBA CloudColor = new ColorRGBA();

                public ZoneEnvironmentParametersTimeSlice(int hourTimestamp)
                {
                    HourTimestamp = hourTimestamp;
                }

                public void SetSkyboxElementsToSameColor(byte red, byte green, byte blue)
                {
                    CloudDensity = 1f;
                    SkyTopColor = new ColorRGBA(red, green, blue, 0);
                    SkyMiddleColor = new ColorRGBA(red, green, blue, 0);
                    SkyMiddleToHorizonColor = new ColorRGBA(red, green, blue, 0);
                    SkyAboveHorizonColor = new ColorRGBA(red, green, blue, 0);
                    SkyHorizonColor = new ColorRGBA(red, green, blue, 0);
                    SunColor = new ColorRGBA(red, green, blue, 0);
                    SunLargeHaloColor = new ColorRGBA(red, green, blue, 0);
                    CloudColor = new ColorRGBA(red, green, blue, 0);
                }
            }

            public bool ShowCelestialObjects = true; // Moon, Sun, Stars
            public List<ZoneEnvironmentParametersTimeSlice> ParametersTimeSlices = new List<ZoneEnvironmentParametersTimeSlice>();

            // DBCIDs
            private static int CURRENT_LIGHTPARAMSID = Configuration.CONFIG_DBCID_LIGHTPARAMS_START;
            private int DBCLightParamsID;

            public ZoneEnvironmentParameters()
            {
                DBCLightParamsID = CURRENT_LIGHTPARAMSID;
                CURRENT_LIGHTPARAMSID++;
            }        
        }

        // Parameters
        public ZoneEnvironmentParameters ParamatersClearWeather = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersClearWeatherUnderwater = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersStormyWeather = new ZoneEnvironmentParameters();
        public ZoneEnvironmentParameters ParamatersStormyWeatherUnderwater = new ZoneEnvironmentParameters();

        // DBCIDs
        private static int CURRENT_LIGHTID = Configuration.CONFIG_DBCID_LIGHT_START;
        private int DBCLightID;

        public ZoneEnvironmentSettings()
        {
            DBCLightID = CURRENT_LIGHTID;
            CURRENT_LIGHTID++;
        }

        // Used for indoor zones that have no day/night cycle, no weather, and are obscured by fog
        public void SetAsFoggyIndoors(byte fogRed, byte fogGreen, byte fogBlue, float fogDistance, float fogDistanceMultiplier,
            byte ambientRed, byte ambientGreen, byte ambientBlue)
        {
            ParamatersClearWeather.ShowCelestialObjects = false;
            ParamatersClearWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeather.ParametersTimeSlices[0].AmbientLight = new ColorRGBA(ambientRed, ambientGreen, ambientBlue, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersClearWeather.ParametersTimeSlices[0].SetSkyboxElementsToSameColor(fogRed, fogGreen, fogBlue);

            ParamatersClearWeatherUnderwater.ShowCelestialObjects = false;
            ParamatersClearWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].AmbientLight = new ColorRGBA(ambientRed, ambientGreen, ambientBlue, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersClearWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSameColor(fogRed, fogGreen, fogBlue);

            ParamatersStormyWeather.ShowCelestialObjects = false;
            ParamatersStormyWeather.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeather.ParametersTimeSlices[0].AmbientLight = new ColorRGBA(ambientRed, ambientGreen, ambientBlue, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersStormyWeather.ParametersTimeSlices[0].SetSkyboxElementsToSameColor(fogRed, fogGreen, fogBlue);

            ParamatersStormyWeatherUnderwater.ShowCelestialObjects = false;
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices.Add(new ZoneEnvironmentParameters.ZoneEnvironmentParametersTimeSlice(0));
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].AmbientLight = new ColorRGBA(ambientRed, ambientGreen, ambientBlue, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].FogColor = new ColorRGBA(fogRed, fogGreen, fogBlue, 0);
            ParamatersStormyWeatherUnderwater.ParametersTimeSlices[0].SetSkyboxElementsToSameColor(fogRed, fogGreen, fogBlue);
        }
    }
}
