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
using EQWOWConverter.Zones.Properties;

namespace EQWOWConverter.Zones
{
    internal class ZoneProperties
    {
        private static Dictionary<string, ZoneProperties> ZonePropertyListByShortName = new Dictionary<string, ZoneProperties>();
        public static ZoneEnvironmentSettings CommonOutdoorEnvironmentProperties = new ZoneEnvironmentSettings();

        public int DBCMapID;
        public int DBCMapDifficultyID;
        public UInt32 DBCWMOID;
        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;
        public ZoneContinentType Continent;
        public bool HasShadowBox = false;
        public Vector3 TelePosition = new Vector3();
        public float TeleOrientation = 0;
        public int ExpansionID = 0;
        public List<ZonePropertiesZoneLineBox> ZoneLineBoxes = new List<ZonePropertiesZoneLineBox>();
        public List<ZoneLiquidGroup> LiquidGroups = new List<ZoneLiquidGroup>();
        public HashSet<string> AlwaysBrightMaterialsByName = new HashSet<string>();
        public ZoneEnvironmentSettings? CustomZonewideEnvironmentProperties = null;
        public double VertexColorIntensityOverride = -1;
        public ZoneArea DefaultZoneArea = new ZoneArea(string.Empty, string.Empty);
        public List<ZoneArea> ZoneAreas = new List<ZoneArea>();
        public HashSet<string> Enabled2DSoundInstancesByDaySoundName = new HashSet<string>();
        public bool IsRestingZoneWide = false;
        public int RainChanceWinter = 0;
        public int RainChanceSpring = 0;
        public int RainChanceSummer = 0;
        public int RainChanceFall = 0;
        public int SnowChanceWinter = 0;
        public int SnowChanceSpring = 0;
        public int SnowChanceSummer = 0;
        public int SnowChanceFall = 0;
        public float CollisionMaxZ = 0;
        public List<BoundingBox> DiscardGeometryBoxes = new List<BoundingBox>();

        private static readonly object ListReadLock = new object();
        private static readonly object DBCWMOIDLock = new object();

        // DBCIDs
        private static UInt32 CURRENT_WMOID = Configuration.DBCID_WMOAREATABLE_WMOID_START;

        public ZoneProperties()
        {
            DBCWMOID = GenerateDBCWMOID();
        }

        public static UInt32 GenerateDBCWMOID()
        {
            lock (DBCWMOIDLock)
            {
                UInt32 dbcWMOID = CURRENT_WMOID;
                CURRENT_WMOID++;
                return dbcWMOID;
            }
        }

        protected void DisableSunlight()
        {
            HasShadowBox = true;
        }

        protected void Enable2DSoundInstances(params string[] daySoundNames)
        {
            foreach (string daySoundName in daySoundNames)
            {
                if (Enabled2DSoundInstancesByDaySoundName.Contains(daySoundName) == false)
                    Enabled2DSoundInstancesByDaySoundName.Add(daySoundName);
            }
        }

        protected void SetZonewideEnvironmentAsIndoors(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as indoor foggy, but the zonewide environment settings were already set. There could be issues.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsIndoors(fogRed, fogGreen, fogBlue, fogType,
                Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE,
                Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE,
                Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE);
        }

        protected void SetZonewideEnvironmentAsIndoors(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, byte ambientRed, byte ambientGreen, byte ambientBlue)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as indoor foggy, but the zonewide environment settings were already set. There could be issues.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsIndoors(fogRed, fogGreen, fogBlue, fogType, ambientRed, ambientGreen, ambientBlue);
        }

        protected void SetZonewideEnvironmentAsOutdoorsWithSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, float cloudDensity,
            float ambientBrightnessMod)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as outdoor with sky, but the zonewide environment settings were already set.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(fogRed, fogGreen, fogBlue, fogType, true, cloudDensity, ambientBrightnessMod, ZoneSkySpecialType.None);
        }

        protected void SetZonewideEnvironmentAsIndoorWithSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, float cloudDensity)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as indoor with sky, but the zonewide environment settings were already set.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsIndoorsWithSky(fogRed, fogGreen, fogBlue, fogType, Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE,
                Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE, Configuration.LIGHT_DEFAULT_INDOOR_AMBIENCE, cloudDensity);
        }

        protected void SetZonewideEnvironmentAsOutdoorsNoSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, float ambientBrightnessMod)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as outdoor no sky, but the zonewide environment settings were already set");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(fogRed, fogGreen, fogBlue, fogType, false, 1f, ambientBrightnessMod, ZoneSkySpecialType.None);
        }

        protected void OverrideVertexColorIntensity(double overrideIntensityAmount)
        {
            VertexColorIntensityOverride = overrideIntensityAmount;
        }

        // These areas must be made in descending priority order, as every area will isolate its own geometry
        // Ambient sound of "silence" will override a parent's ambience with no sound
        protected void AddZoneArea(string name, string musicFileNameNoExtDay = "", string musicFileNameNoExtNight = "", bool loopMusic = true,
            string ambientSoundFileNameNoExtDay = "", string ambientSoundFileNameNoExtNight = "", float musicVolume = 1f)
        {
            AddChildZoneArea(name, string.Empty, musicFileNameNoExtDay, musicFileNameNoExtNight, loopMusic, ambientSoundFileNameNoExtDay,
                ambientSoundFileNameNoExtNight, musicVolume);
        }

        // These areas must be made in descending priority order, as every area will isolate its own geometry
        // IMPORTANT: These must be defined before the parent area if they share geometry
        // Ambient sound of "silence" will override a parent's ambience with no sound
        protected void AddChildZoneArea(string name, string parentName, string musicFileNameNoExtDay = "", string musicFileNameNoExtNight = "", bool loopMusic = true,
            string ambientSoundFileNameNoExtDay = "", string ambientSoundFileNameNoExtNight = "", float musicVolume = 1f)
        {
            // Create it
            ZoneArea newZoneArea = new ZoneArea(name, parentName);
            ZoneAreas.Add(newZoneArea);

            // Music
            if (musicFileNameNoExtDay != "" || musicFileNameNoExtNight != "")
                newZoneArea.SetMusic(musicFileNameNoExtDay, musicFileNameNoExtNight, loopMusic, musicVolume);

            // Ambient Sounds
            if (ambientSoundFileNameNoExtDay != "" || ambientSoundFileNameNoExtNight != "")
                newZoneArea.SetAmbientSound(ambientSoundFileNameNoExtDay, ambientSoundFileNameNoExtNight);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddZoneAreaBox(string zoneAreaName, float nwCornerX, float nwCornerY, float nwCornerZ, float seCornerX, float seCornerY, float seCornerZ)
        {
            BoundingBox boundingBox = new BoundingBox(seCornerX, seCornerY, seCornerZ, nwCornerX, nwCornerY, nwCornerZ);

            // Add to an existing zone area if there's a match
            foreach (ZoneArea zoneArea in ZoneAreas)
            {
                if (zoneArea.DisplayName == zoneAreaName)
                {
                    zoneArea.AddBoundingBox(boundingBox, true);
                    return;
                }
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddZoneAreaOctagonBox(string zoneAreaName, float northEdgeX, float southEdgeX, float westEdgeY, float eastEdgeY, float northWestY, float northEastY,
            float southWestY, float southEastY, float westNorthX, float westSouthX, float eastNorthX, float eastSouthX, float topZ, float bottomZ)
        {
            // TODO: Consider moving this to a config
            float stepSize = 0.3f;

            // Boundary Control (very limited)
            if (northEdgeX < southEdgeX)
            {
                Logger.WriteError("AddOctagonChildZoneArea error for zone '" + ShortName + "' as north x < south x");
                return;
            }
            if (westEdgeY < eastEdgeY)
            {
                Logger.WriteError("AddOctagonChildZoneArea error for zone '" + ShortName + "' as west x < east x");
                return;
            }

            // Start at the north X and walk down to the south X
            float curXTop = northEdgeX;
            bool moreXToWalk = true;
            while (moreXToWalk == true)
            {
                // If in the middle block, fill it in
                if (curXTop <= westNorthX && curXTop <= eastNorthX && curXTop >= westSouthX && curXTop >= eastSouthX)
                {
                    float highestSouthEastX = MathF.Max(westSouthX, eastSouthX);
                    AddZoneAreaBox(zoneAreaName, curXTop, westEdgeY, topZ, highestSouthEastX, eastEdgeY, bottomZ);
                    curXTop = highestSouthEastX;
                }

                // Calculate the bottom edge, and align bottom edge if extends
                float curXBottom = curXTop - stepSize;
                if (curXBottom < southEdgeX)
                {
                    curXBottom = southEdgeX;
                    moreXToWalk = false;
                }

                // Determine NW position
                float nwX = curXTop;
                float nwY = 0;
                if (curXTop > westNorthX)
                    nwY = GetYOnLineAtX(northEdgeX, northWestY, westNorthX, westEdgeY, curXTop);
                else
                    nwY = GetYOnLineAtX(southEdgeX, southWestY, westSouthX, westEdgeY, curXTop);

                // Determine SE position
                float seX = curXBottom;
                float seY = 0;
                if (curXBottom > eastNorthX)
                    seY = GetYOnLineAtX(northEdgeX, northEastY, eastNorthX, eastEdgeY, curXBottom);
                else
                    seY = GetYOnLineAtX(southEdgeX, southEastY, eastSouthX, eastEdgeY, curXBottom);

                // Constrain in bounds
                nwX = MathF.Min(nwX, northEdgeX);
                nwY = MathF.Min(nwY, westEdgeY);
                seX = MathF.Max(seX, southEdgeX);
                seY = MathF.Max(seY, eastEdgeY);

                // Add the box if the bounds are good
                if (nwX > seX && nwY > seY)
                {
                    AddZoneAreaBox(zoneAreaName, nwX, nwY, topZ, seX, seY, bottomZ);
                }

                // Set new top factoring for overlap
                curXTop = curXBottom;
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        // Coordinates flip due to world <-> wmo space
        protected void AddDiscardGeometryBox(float nwCornerX, float nwCornerY, float nwCornerZ, float seCornerX, float seCornerY, float seCornerZ)
        {
            BoundingBox preScaleBox = new BoundingBox(seCornerX, seCornerY, seCornerZ, nwCornerX, nwCornerY, nwCornerZ);
            BoundingBox postScaleBox = new BoundingBox();

            postScaleBox.TopCorner.X = preScaleBox.BottomCorner.X * -Configuration.GENERATE_WORLD_SCALE;
            postScaleBox.TopCorner.Y = preScaleBox.BottomCorner.Y * -Configuration.GENERATE_WORLD_SCALE;
            postScaleBox.TopCorner.Z = preScaleBox.TopCorner.Z * Configuration.GENERATE_WORLD_SCALE;
            postScaleBox.BottomCorner.X = preScaleBox.TopCorner.X * -Configuration.GENERATE_WORLD_SCALE;
            postScaleBox.BottomCorner.Y = preScaleBox.TopCorner.Y * -Configuration.GENERATE_WORLD_SCALE;
            postScaleBox.BottomCorner.Z = preScaleBox.BottomCorner.Z * Configuration.GENERATE_WORLD_SCALE;

            DiscardGeometryBoxes.Add(postScaleBox);
        }

        protected void SetZonewideMusic(string musicFileNameNoExtDay, string musicFileNameNoExtNight, bool loop = true, float musicVolume = 1f)
        {
            DefaultZoneArea.SetMusic(musicFileNameNoExtDay, musicFileNameNoExtNight, loop, musicVolume);
        }

        protected void SetZonewideAmbienceSound(string ambienceFileNameNoExtDay, string ambienceFileNameNoExtNight)
        {
            DefaultZoneArea.SetAmbientSound(ambienceFileNameNoExtDay, ambienceFileNameNoExtNight);
        }

        protected void AddAlwaysBrightMaterial(string materialName)
        {
            if (AlwaysBrightMaterialsByName.Contains(materialName) == false)
                AlwaysBrightMaterialsByName.Add(materialName);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        // The box is oriented when facing north (when using .gps, orientation = 0 and no tilt) since zone lines are axis aligned in EQ
        protected void AddZoneLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY,
            float targetZonePositionZ, ZoneLineOrientationType targetZoneOrientation, float boxTopNorthwestX, float boxTopNorthwestY,
            float boxTopNorthwestZ, float boxBottomSoutheastX, float boxBottomSoutheastY, float boxBottomSoutheastZ)
        {
            ZonePropertiesZoneLineBox zoneLineBox = new ZonePropertiesZoneLineBox(targetZoneShortName, targetZonePositionX, targetZonePositionY,
                targetZonePositionZ, targetZoneOrientation, boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ, boxBottomSoutheastX,
                boxBottomSoutheastY, boxBottomSoutheastZ);
            ZoneLineBoxes.Add(zoneLineBox);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddTeleportPad(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY, float targetZonePositionZ,
            ZoneLineOrientationType targetZoneOrientation, float padBottomCenterXPosition, float padBottomCenterYPosition, float padBottomCenterZPosition,
            float padWidth)
        {
            ZonePropertiesZoneLineBox zoneLineBox = new ZonePropertiesZoneLineBox(targetZoneShortName, targetZonePositionX, targetZonePositionY, targetZonePositionZ,
            targetZoneOrientation, padBottomCenterXPosition, padBottomCenterYPosition, padBottomCenterZPosition, padWidth);
            ZoneLineBoxes.Add(zoneLineBox);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddLiquidVolume(ZoneLiquidType liquidType, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ, int liquidGroupID = -1)
        {
            ZoneLiquid liquidVolume = new ZoneLiquid(liquidType, "", nwCornerX, nwCornerY, seCornerX, seCornerY, highZ, highZ - lowZ, ZoneLiquidShapeType.Volume);
            if (liquidGroupID == -1)
            {
                ZoneLiquidGroup newLiquidGroup = new ZoneLiquidGroup();
                newLiquidGroup.AddLiquidChunk(liquidVolume);
                LiquidGroups.Add(newLiquidGroup);
            }
            else
                LiquidGroups[liquidGroupID].AddLiquidChunk(liquidVolume);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddLiquidPlane(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ, ZoneLiquidSlantType slantType, float minDepth, int liquidGroupID = -1, string forcedAreaAlignment = "")
        {
            ZoneLiquid liquidPlane = new ZoneLiquid(liquidType, materialName, nwCornerX, nwCornerY, seCornerX, seCornerY,
                highZ, lowZ, slantType, minDepth);
            if (liquidGroupID == -1)
            {
                ZoneLiquidGroup newLiquidGroup = new ZoneLiquidGroup();
                newLiquidGroup.AddLiquidChunk(liquidPlane);
                if (forcedAreaAlignment.Length > 0)
                    newLiquidGroup.ForcedAreaAssignmentName = forcedAreaAlignment;
                LiquidGroups.Add(newLiquidGroup);
            }
            else
            {
                if (forcedAreaAlignment.Length > 0)
                    Logger.WriteError("Unhandled forced alignment of liquid group to an area");
                LiquidGroups[liquidGroupID].AddLiquidChunk(liquidPlane);
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddLiquidPlaneZLevel(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float fixedZ, float minDepth, int liquidGroupID = -1)
        {
            ZoneLiquid liquidPlane = new ZoneLiquid(liquidType, materialName, nwCornerX, nwCornerY, seCornerX,
                seCornerY, fixedZ, minDepth, ZoneLiquidShapeType.Plane);
            if (liquidGroupID == -1)
            {
                ZoneLiquidGroup newLiquidGroup = new ZoneLiquidGroup();
                newLiquidGroup.AddLiquidChunk(liquidPlane);
                LiquidGroups.Add(newLiquidGroup);
            }
            else
                LiquidGroups[liquidGroupID].AddLiquidChunk(liquidPlane);
        }

        protected float GetYOnLineAtX(float point1X, float point1Y, float point2X, float point2Y, float testX)
        {
            // Handle horizontal y (flat line)
            if (MathF.Abs(point1X - point2X) < float.Epsilon)
            {
                Logger.WriteError("Unable to determine y on line at x becasue point 1 x and point 2 x were the same (straight line)");
                return 0f;
            }

            // Calculate and return the y
            float m = (point2Y - point1Y) / (point2X - point1X);
            float c = point1Y - m * point1X;
            return m * testX + c;
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType liquidType, string materialName, float northMostX, float northMostY, float westMostX, float westMostY,
            float southMostX, float southMostY, float eastMostX, float eastMostY, float allCornersZ, float minDepth, float northXLimit, float westYLimit,
            float southXLimit, float eastYLimit, float stepSize)
        {
            // Boundary Control (very limited)
            if (northMostX < southMostX)
            {
                Logger.WriteError("AddQuadrilateralLiquidShapeZLevel error for zone '" + ShortName + "' and material '" + materialName + "' as north x < south x");
                return;
            }
            if (westMostY < eastMostY)
            {
                Logger.WriteError("AddQuadrilateralLiquidShapeZLevel error for zone '" + ShortName + "' and material '" + materialName + "' as west x < east x");
                return;
            }

            // Generate a new group
            int curLiquidGroupID = LiquidGroups.Count;
            LiquidGroups.Add(new ZoneLiquidGroup());

            // Start at the north X and walk down to the south X
            float curXTop = northMostX;
            bool moreXToWalk = true;
            while (moreXToWalk == true)
            {
                // Calculate the bottom edge, and align bottom edge if extends
                float curXBottom = curXTop - stepSize;
                if (curXBottom < southMostX)
                {
                    curXBottom = southMostX;
                    moreXToWalk = false;
                }

                // Determine NW position
                float nwX = curXTop;
                float nwY = 0;
                if (curXTop > westMostX)
                    nwY = GetYOnLineAtX(northMostX, northMostY, westMostX, westMostY, curXTop);
                else
                    nwY = GetYOnLineAtX(westMostX, westMostY, southMostX, southMostY, curXTop);

                // Determine SE position
                float seX = curXBottom;
                float seY = 0;
                if (curXBottom > eastMostX)
                    seY = GetYOnLineAtX(northMostX, northMostY, eastMostX, eastMostY, curXBottom);
                else
                    seY = GetYOnLineAtX(eastMostX, eastMostY, southMostX, southMostY, curXBottom);

                // Constrain in bounds
                nwX = MathF.Min(nwX, northXLimit);
                nwY = MathF.Min(nwY, westYLimit);
                seX = MathF.Max(seX, southXLimit);
                seY = MathF.Max(seY, eastYLimit);

                // Add the plane if the bounds are good
                if (nwX > seX && nwY > seY)
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornersZ, minDepth, curLiquidGroupID);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddTriangleLiquidShapeSouthEdgeAligned(ZoneLiquidType liquidType, string materialName, float northX, float northY, float southEdgeX, float southWestY,
            float southEastY, float allCornerZ, float minDepth, float stepSize)
        {
            float curXTop = northX;
            bool moreXToWalk = true;
            float xDelta = southEdgeX - northX;
            float yWestDelta = northY - southWestY;
            float yEastDelta = northY - southEastY;
            float northToSouthWestDistance = MathF.Sqrt(xDelta * xDelta + yWestDelta * yWestDelta);
            float northToSouthEastDistance = MathF.Sqrt(xDelta * xDelta + yEastDelta * yEastDelta);

            // Generate a new group
            int curLiquidGroupID = LiquidGroups.Count;
            LiquidGroups.Add(new ZoneLiquidGroup());

            while (moreXToWalk == true)
            {
                // Calculate the bottom edge, and align bottom edge if extends
                float curXBottom = curXTop - stepSize;
                if (curXBottom < southEdgeX)
                {
                    curXBottom = southEdgeX;
                    moreXToWalk = false;
                }

                // Determine NW position
                float nwX = curXTop;
                float nwY = GetYOnLineAtX(northX, northY, southEdgeX, southWestY, curXTop);

                // Determine SE position
                float seX = curXBottom;
                float seY = GetYOnLineAtX(northX, northY, southEdgeX, southEastY, curXBottom);

                // Add the plane if the bounds are good
                if (nwX > seX && nwY > seY)
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornerZ, minDepth, curLiquidGroupID);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        // TODO: BUG: The plane may be inverted, but seems to work properly in WoW
        protected void AddLiquidCylinder(ZoneLiquidType liquidType, string materialName, float centerX, float centerY, float radius, float topZ,
            float height, float maxX, float maxY, float minX, float minY, float stepSize)
        {
            // Generate a new group
            int curLiquidGroupID = LiquidGroups.Count;
            LiquidGroups.Add(new ZoneLiquidGroup());

            // Step down through all text X positions based on radius
            float relativeWorkingX = radius - float.Epsilon;
            float relativeEndX = -(radius - float.Epsilon);
            while (relativeWorkingX > relativeEndX)
            {
                // Calculate the base core y coordinate
                float yBase = radius * radius - (relativeWorkingX * relativeWorkingX);
                float yDistance = MathF.Sqrt(yBase);

                // Create the coordinates for this liquid plane
                float nwX = centerX + relativeWorkingX;
                float nwY = centerY + yDistance;
                float seX = centerX - relativeWorkingX;
                float seY = centerY - yDistance;

                // TODO: Restrain the bounds
                nwX = MathF.Min(nwX, maxX);
                nwY = MathF.Min(nwY, maxY);
                seX = MathF.Max(seX, minX);
                seY = MathF.Max(seY, minY);

                // Make the plane
                AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, topZ, height, curLiquidGroupID);

                // Step
                relativeWorkingX -= stepSize;
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType liquidType, string materialName, float northEdgeX, float southEdgeX, float northWestY, float northEastY,
            float southWestY, float southEastY, float topZ, float height, float stepSize)
        {
            // Generate a new group
            int curLiquidGroupID = LiquidGroups.Count;
            LiquidGroups.Add(new ZoneLiquidGroup());

            float curXTop = northEdgeX;
            bool moreXToWalk = true;
            while (moreXToWalk == true)
            {
                // Calculate the bottom edge, and align bottom edge if extends
                float curXBottom = curXTop - stepSize;
                if (curXBottom < southEdgeX)
                {
                    curXBottom = southEdgeX;
                    moreXToWalk = false;
                }

                // Determine NW position
                float nwX = curXTop;
                float nwY = GetYOnLineAtX(northEdgeX, northWestY, southEdgeX, southWestY, curXTop);

                // Determine SE position
                float seX = curXBottom;
                float seY = GetYOnLineAtX(northEdgeX, northEastY, southEdgeX, southEastY, curXBottom);

                // Add the plane if the bounds are good
                if (nwX > seX && nwY > seY)
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, topZ, height, curLiquidGroupID);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType liquidType, string materialName, float northMostX, float northMostY, float westMostX, float westMostY,
            float southMostX, float southMostY, float eastMostX, float eastMostY, float allCornersZ, float minDepth)
        {
            AddQuadrilateralLiquidShapeZLevel(liquidType, materialName, northMostX, northMostY, westMostX, westMostY, southMostX, southMostY,
                eastMostX, eastMostY, allCornersZ, minDepth, northMostX, westMostY, southMostX, eastMostY, Configuration.LIQUID_QUADGEN_EDGE_WALK_SIZE);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddOctagonLiquidShape(ZoneLiquidType liquidType, string materialName, float northEdgeX, float southEdgeX, float westEdgeY, float eastEdgeY, float northWestY, float northEastY,
            float southWestY, float southEastY, float westNorthX, float westSouthX, float eastNorthX, float eastSouthX, float allCornersZ, float minDepth)
        {
            AddOctagonLiquidShape(liquidType, materialName, northEdgeX, southEdgeX, westEdgeY, eastEdgeY, northWestY, northEastY, southWestY, southEastY, westNorthX,
                westSouthX, eastNorthX, eastSouthX, allCornersZ, minDepth, Configuration.LIQUID_QUADGEN_EDGE_WALK_SIZE);
        }

        // Values should be pre-Scaling (before * EQTOWOW_WORLD_SCALE)
        protected void AddOctagonLiquidShape(ZoneLiquidType liquidType, string materialName, float northEdgeX, float southEdgeX, float westEdgeY, float eastEdgeY, float northWestY, float northEastY,
            float southWestY, float southEastY, float westNorthX, float westSouthX, float eastNorthX, float eastSouthX, float allCornersZ, float minDepth, float stepSize)
        {
            // Boundary Control (very limited)
            if (northEdgeX < southEdgeX)
            {
                Logger.WriteError("AddQuadrilateralLiquidShape error for zone '" + ShortName + "' and material '" + materialName + "' as north x < south x");
                return;
            }
            if (westEdgeY < eastEdgeY)
            {
                Logger.WriteError("AddQuadrilateralLiquidShape error for zone '" + ShortName + "' and material '" + materialName + "' as west x < east x");
                return;
            }

            // Generate a new group
            int curLiquidGroupID = LiquidGroups.Count;
            LiquidGroups.Add(new ZoneLiquidGroup());

            // Start at the north X and walk down to the south X
            float curXTop = northEdgeX;
            bool moreXToWalk = true;
            while (moreXToWalk == true)
            {
                // If in the middle block, fill it in
                if (curXTop <= westNorthX && curXTop <= eastNorthX && curXTop >= westSouthX && curXTop >= eastSouthX)
                {
                    float highestSouthEastX = MathF.Max(westSouthX, eastSouthX);
                    AddLiquidPlaneZLevel(liquidType, materialName, curXTop, westEdgeY, highestSouthEastX, eastEdgeY, allCornersZ, minDepth, curLiquidGroupID);
                    curXTop = highestSouthEastX;
                }

                // Calculate the bottom edge, and align bottom edge if extends
                float curXBottom = curXTop - stepSize;
                if (curXBottom < southEdgeX)
                {
                    curXBottom = southEdgeX;
                    moreXToWalk = false;
                }

                // Determine NW position
                float nwX = curXTop;
                float nwY = 0;
                if (curXTop > westNorthX)
                    nwY = GetYOnLineAtX(northEdgeX, northWestY, westNorthX, westEdgeY, curXTop);
                else
                    nwY = GetYOnLineAtX(southEdgeX, southWestY, westSouthX, westEdgeY, curXTop);

                // Determine SE position
                float seX = curXBottom;
                float seY = 0;
                if (curXBottom > eastNorthX)
                    seY = GetYOnLineAtX(northEdgeX, northEastY, eastNorthX, eastEdgeY, curXBottom);
                else
                    seY = GetYOnLineAtX(southEdgeX, southEastY, eastSouthX, eastEdgeY, curXBottom);

                // Constrain in bounds
                nwX = MathF.Min(nwX, northEdgeX);
                nwY = MathF.Min(nwY, westEdgeY);
                seX = MathF.Max(seX, southEdgeX);
                seY = MathF.Max(seY, eastEdgeY);

                // Add the plane if the bounds are good
                if (nwX > seX && nwY > seY)
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornersZ, minDepth, curLiquidGroupID);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        public static Dictionary<string, ZoneProperties> GetZonePropertyListByShortName()
        {
            lock (ListReadLock)
            {
                if (ZonePropertyListByShortName.Count == 0)
                    PopulateZonePropertiesList();
                return ZonePropertyListByShortName;
            }
        }

        public static ZoneProperties GetZonePropertiesForZone(string zoneShortName)
        {
            lock (ListReadLock)
            {
                if (ZonePropertyListByShortName.Count == 0)
                    PopulateZonePropertiesList();
                if (ZonePropertyListByShortName.ContainsKey(zoneShortName) == false)
                {
                    Logger.WriteError("Error.  Tried to pull Zone Properties for zone with shortname '" + zoneShortName + "' but non existed with that name");
                    return ZonePropertyListByShortName["load"];
                }
                else
                    return ZonePropertyListByShortName[zoneShortName];
            }
        }

        private static void AddZonePropertiesByShortName(Dictionary<string, Dictionary<string, string>> zonePropertiesByShortName, string shortName, ZoneProperties zoneProperties)
        {
            if (zonePropertiesByShortName.ContainsKey(shortName) == true)
            {
                Dictionary<string, string> propertiesRow = zonePropertiesByShortName[shortName];

                // Skip if the expansion doesn't line up, and it wasn't an explicit add
                zoneProperties.ExpansionID = int.Parse(propertiesRow["ExpansionID"]);

                // If this is an explict add, respond accordingly
                if (Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES.Count != 0)
                {
                    bool foundShortname = false;
                    foreach (string generateShortName in Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES)
                    {
                        if (generateShortName.Trim().ToLower() == shortName.ToLower())
                        {
                            foundShortname = true;
                            break;
                        }
                    }
                    if (foundShortname == false)
                    {
                        Logger.WriteDebug(string.Concat("Skipping zone with shortname '", shortName, "' since the Configuration.GENERATE_ONLY_LISTED_ZONE_SHORTNAMES was populated but did not include this shortname"));
                        return;
                    }
                }

                else if (Configuration.GENERATE_EQ_EXPANSION_ID_ZONES < zoneProperties.ExpansionID)
                {
                    Logger.WriteDebug(string.Concat("Skipping zone with shortname '", shortName, "' since the expansionID is > the configured expansion ID"));
                    return;
                }

                zoneProperties.ShortName = shortName;
                zoneProperties.DBCMapID = int.Parse(propertiesRow["WOWMapID"]);
                zoneProperties.DBCMapDifficultyID = int.Parse(propertiesRow["WOWMapDifficultyID"]);                    
                zoneProperties.DescriptiveName = propertiesRow["DescriptiveName"];
                zoneProperties.TelePosition.X = float.Parse(propertiesRow["TeleX"]) * Configuration.GENERATE_WORLD_SCALE;
                zoneProperties.TelePosition.Y = float.Parse(propertiesRow["TeleY"]) * Configuration.GENERATE_WORLD_SCALE;
                zoneProperties.TelePosition.Z = float.Parse(propertiesRow["TeleZ"]) * Configuration.GENERATE_WORLD_SCALE;
                zoneProperties.TeleOrientation = float.Parse(propertiesRow["TeleOrientation"]);
                zoneProperties.Continent = (ZoneContinentType)int.Parse(propertiesRow["ContinentID"]);
                zoneProperties.ExpansionID = int.Parse(propertiesRow["ExpansionID"]);
                zoneProperties.DefaultZoneArea.DisplayName = propertiesRow["DescriptiveName"];
                zoneProperties.IsRestingZoneWide = int.Parse(propertiesRow["RestZoneWide"]) == 1 ? true : false;
                zoneProperties.RainChanceWinter = int.Parse(propertiesRow["rain_chance_winter"]);
                zoneProperties.RainChanceSpring = int.Parse(propertiesRow["rain_chance_spring"]);
                zoneProperties.RainChanceSummer = int.Parse(propertiesRow["rain_chance_summer"]);
                zoneProperties.RainChanceFall = int.Parse(propertiesRow["rain_chance_fall"]);
                zoneProperties.SnowChanceWinter = int.Parse(propertiesRow["snow_chance_winter"]);
                zoneProperties.SnowChanceSpring = int.Parse(propertiesRow["snow_chance_spring"]);
                zoneProperties.SnowChanceSummer = int.Parse(propertiesRow["snow_chance_summer"]);
                zoneProperties.SnowChanceFall = int.Parse(propertiesRow["snow_chance_fall"]);
                zoneProperties.CollisionMaxZ = float.Parse(propertiesRow["CollisionGeometryMaxZ"]);
                ZonePropertyListByShortName.Add(shortName, zoneProperties);
            }
            else
                Logger.WriteError("Could not find the properties for short name '" + shortName + "' which should be in the ZoneProperties.csv file");
        }

        private static void PopulateZonePropertiesList()
        {
            // Load the file first
            string zonePropertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneProperties.csv");
            Logger.WriteDebug("Populating Zone Properties list via file '" + zonePropertiesFile + "'");
            List<Dictionary<string, string>> zonePropertiesRows = FileTool.ReadAllRowsFromFileWithHeader(zonePropertiesFile, "|");
            
            // Remap the rows for quicker lookups
            Dictionary<string, Dictionary<string, string>> zonePropertiesByShortName = new Dictionary<string, Dictionary<string, string>>();
            foreach (Dictionary<string, string> propertiesRow in zonePropertiesRows)
                zonePropertiesByShortName.Add(propertiesRow["ShortName"].Trim().ToLower(), propertiesRow);

            // Add any valid properties
            AddZonePropertiesByShortName(zonePropertiesByShortName, "airplane", new PlaneOfSkyZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "akanon", new AkAnonZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "arena", new ArenaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "befallen", new BefallenZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "beholder", new GorgeOfKingXorbbZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "blackburrow", new BlackburrowZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "burningwood", new BurningWoodZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "butcher", new ButcherblockMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "cabeast", new CabilisEastZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "cabwest", new CabilisWestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "cauldron", new DagnorsCauldronZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "cazicthule", new CazicThuleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "charasis", new HowlingStonesZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "chardok", new ChardokZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "citymist", new CityOfMistZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "cobaltscar", new CobaltScarZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "commons", new WestCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "crushbone", new CrushboneZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "crystal", new CrystalCavernsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "dalnir", new DalnirZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "dreadlands", new DreadlandsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "droga", new TempleOfDrogaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "eastkarana", new EastKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "eastwastes", new EasternWastesZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "ecommons", new EastCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "emeraldjungle", new EmeraldJungleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "erudnext", new ErudinZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "erudnint", new ErudinPalaceZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "erudsxing", new ErudsCrossingZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "everfrost", new EverfrostZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "fearplane", new PlaneOfFearZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "feerrott", new FeerrottZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "felwithea", new NorthFelwitheZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "felwitheb", new SouthFelwitheZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "fieldofbone", new FieldOfBoneZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "firiona", new FirionaVieZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "freporte", new EastFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "freportn", new NorthFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "freportw", new WestFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "frontiermtns", new FrontierMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "frozenshadow", new TowerOfFrozenShadowZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "gfaydark", new GreaterFaydarkZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "greatdivide", new GreatDivideZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "grobb", new GrobbZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "growthplane", new PlaneOfGrowthZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "gukbottom", new GukBottomZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "guktop", new GukTopZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "halas", new HalasZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "hateplane", new PlaneOfHateZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "highkeep", new HighKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "highpass", new HighpassHoldZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "hole", new HoleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "iceclad", new IcecladOceanZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "innothule", new InnothuleSwampZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kael", new KaelDrakkalZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kaesora", new KaesoraZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kaladima", new SouthKaladimZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kaladimb", new NorthKaladimZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "karnor", new KarnorsCastleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kedge", new KedgeKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kerraridge", new KerraIsleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kithicor", new KithicorForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "kurn", new KurnsTowerZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "lakeofillomen", new LakeOfIllOmenZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "lakerathe", new LakeRathetearZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "lavastorm", new LavastormMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "lfaydark", new LesserFaydarkZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "load", new LoadingAreaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "mischiefplane", new PlaneOfMischiefZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "mistmoore", new CastleMistmooreZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "misty", new MistyThicketZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "najena", new NajenaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "necropolis", new DragonNecropolisZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "nektulos", new NektulosForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "neriaka", new NeriakForeignQuarterZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "neriakb", new NeriakCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "neriakc", new NeriakThirdGateZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "northkarana", new NorthKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "nro", new NorthDesertOfRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "nurga", new MinesOfNurgaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "oasis", new OasisOfMarrZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "oggok", new OggokZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "oot", new OceanOfTearsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "overthere", new OverthereZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "paineel", new PaineelZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "paw", new LairOfTheSplitpawZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "permafrost", new PermafrostCavernsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qcat", new QeynosAqueductsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qey2hh1", new WestKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qeynos", new SouthQeynosZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qeynos2", new NorthQeynosZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qeytoqrg", new QeynosHillsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "qrg", new SurefallGladeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "rathemtn", new RatheMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "rivervale", new RivervaleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "runnyeye", new RunnyeyeCitadelZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "sebilis", new OldSebilisZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "sirens", new SirensGrottoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "skyfire", new SkyfireMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "skyshrine", new SkyshrineZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "sleeper", new SleeperTombZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "soldunga", new SoluseksEyeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "soldungb", new NagafensLairZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "soltemple", new TempleOfSolusekRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "southkarana", new SouthKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "swampofnohope", new SwampOfNoHopeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "sro", new SouthDesertOfRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "steamfont", new SteamfontMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "stonebrunt", new StonebruntMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "templeveeshan", new TempleOfVeeshanZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "thurgadina", new ThurgadinZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "thurgadinb", new IcewellKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "timorous", new TimorousDeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "tox", new ToxxuliaForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "trakanon", new TrakanonsTeethZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "tutorial", new TutorialZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "veeshan", new VeeshansPeakZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "velketor", new VelketorsLabyrinthZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "unrest", new EstateOfUnrestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "wakening", new WakeningLandZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "warrens", new WarrensZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "warslikswood", new WarsliksWoodsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesByShortName, "westwastes", new WesternWastesZoneProperties());
        }
    }
}
