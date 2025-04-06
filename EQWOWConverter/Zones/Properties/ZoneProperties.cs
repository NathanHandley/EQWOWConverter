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
        static private Dictionary<string, ZoneProperties> ZonePropertyListByShortName = new Dictionary<string, ZoneProperties>();
        static public ZoneEnvironmentSettings CommonOutdoorEnvironmentProperties = new ZoneEnvironmentSettings();

        public int DBCMapID;
        public int DBCMapDifficultyID;
        public UInt32 DBCWMOID;
        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;
        public ZoneContinentType Continent;
        public bool HasShadowBox = false;
        public Vector3 TelePosition = new Vector3();
        public float TeleOrientation = 0;
        public List<ZonePropertiesZoneLineBox> ZoneLineBoxes = new List<ZonePropertiesZoneLineBox>();  
        public List<ZoneLiquidGroup> LiquidGroups = new List<ZoneLiquidGroup>();
        public HashSet<string> AlwaysBrightMaterialsByName = new HashSet<string>();
        public ZoneEnvironmentSettings? CustomZonewideEnvironmentProperties = null;
        public double VertexColorIntensityOverride = -1;
        public ZoneArea DefaultZoneArea = new ZoneArea(string.Empty, string.Empty);
        public List<ZoneArea> ZoneAreas = new List<ZoneArea>();
        public HashSet<string> Enabled2DSoundInstancesByDaySoundName = new HashSet<string>();

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
            foreach(string daySoundName in daySoundNames)
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

        protected void SetZonewideEnvironmentAsOutdoorsWithSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType,float cloudDensity,
            float ambientBrightnessMod)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as outdoor with sky, but the zonewide environment settings were already set.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(fogRed, fogGreen, fogBlue, fogType, true, cloudDensity, ambientBrightnessMod, ZoneSkySpecialType.None);
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
            ZoneLiquid liquidVolume = new ZoneLiquid(liquidType, "", nwCornerX, nwCornerY, seCornerX, seCornerY, highZ, highZ-lowZ, ZoneLiquidShapeType.Volume);
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
        static public Dictionary<string, ZoneProperties> GetZonePropertyListByShortName()
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

        private static void AddZonePropertiesByShortName(List<Dictionary<string, string>> propertiesRows, string shortName, ZoneProperties zoneProperties)
        {
            foreach(Dictionary<string, string> propertiesRow in propertiesRows)
            {
                if (propertiesRow["ShortName"].Trim().ToLower() == shortName.Trim().ToLower())
                {
                    zoneProperties.ShortName = shortName;
                    zoneProperties.DBCMapID = int.Parse(propertiesRow["WOWMapID"]);
                    zoneProperties.DBCMapDifficultyID = int.Parse(propertiesRow["WOWMapDifficultyID"]);                    
                    zoneProperties.DescriptiveName = propertiesRow["DescriptiveName"];
                    zoneProperties.TelePosition.X = float.Parse(propertiesRow["TeleX"]);
                    zoneProperties.TelePosition.Y = float.Parse(propertiesRow["TeleY"]);
                    zoneProperties.TelePosition.Z = float.Parse(propertiesRow["TeleZ"]);
                    zoneProperties.TeleOrientation = float.Parse(propertiesRow["TeleOrientation"]);
                    zoneProperties.Continent = (ZoneContinentType)int.Parse(propertiesRow["ContinentID"]);
                    zoneProperties.DefaultZoneArea.DisplayName = propertiesRow["DescriptiveName"];
                    ZonePropertyListByShortName.Add(shortName, zoneProperties);
                    return;
                }
            }
            Logger.WriteError("Could not find the properties for short name '" + shortName + "' which should be in the ZoneProperties.csv file");
        }

        private static void PopulateZonePropertiesList()
        {
            // Load the file first
            string zonePropertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ZoneProperties.csv");
            Logger.WriteDebug("Populating Zone Properties list via file '" + zonePropertiesFile + "'");
            List<Dictionary<string, string>> zonePropertiesRows = FileTool.ReadAllRowsFromFileWithHeader(zonePropertiesFile, "|");

            AddZonePropertiesByShortName(zonePropertiesRows, "airplane", new PlaneOfSkyZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "akanon", new AkAnonZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "arena", new ArenaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "befallen", new BefallenZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "beholder", new GorgeOfKingXorbbZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "blackburrow", new BlackburrowZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "burningwood", new BurningWoodZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "butcher", new ButcherblockMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "cabeast", new CabilisEastZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "cabwest", new CabilisWestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "cauldron", new DagnorsCauldronZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "cazicthule", new CazicThuleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "charasis", new HowlingStonesZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "chardok", new ChardokZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "citymist", new CityOfMistZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "cobaltscar", new CobaltScarZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "commons", new WestCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "crushbone", new CrushboneZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "crystal", new CrystalCavernsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "dalnir", new DalnirZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "dreadlands", new DreadlandsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "droga", new TempleOfDrogaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "eastkarana", new EastKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "eastwastes", new EasternWastesZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "ecommons", new EastCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "emeraldjungle", new EmeraldJungleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "erudnext", new ErudinZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "erudnint", new ErudinPalaceZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "erudsxing", new ErudsCrossingZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "everfrost", new EverfrostZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "fearplane", new PlaneOfFearZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "feerrott", new FeerrottZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "felwithea", new NorthFelwitheZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "felwitheb", new SouthFelwitheZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "fieldofbone", new FieldOfBoneZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "firiona", new FirionaVieZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "freporte", new EastFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "freportn", new NorthFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "freportw", new WestFreeportZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "frontiermtns", new FrontierMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "frozenshadow", new TowerOfFrozenShadowZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "gfaydark", new GreaterFaydarkZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "greatdivide", new GreatDivideZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "grobb", new GrobbZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "growthplane", new PlaneOfGrowthZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "gukbottom", new GukBottomZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "guktop", new GukTopZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "halas", new HalasZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "hateplane", new PlaneOfHateZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "highkeep", new HighKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "highpass", new HighpassHoldZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "hole", new HoleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "iceclad", new IcecladOceanZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "innothule", new InnothuleSwampZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kael", new KaelDrakkalZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kaesora", new KaesoraZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kaladima", new SouthKaladimZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kaladimb", new NorthKaladimZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "karnor", new KarnorsCastleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kedge", new KedgeKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kerraridge", new KerraIsleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kithicor", new KithicorForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "kurn", new KurnsTowerZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "lakeofillomen", new LakeOfIllOmenZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "lakerathe", new LakeRathetearZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "lavastorm", new LavastormMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "lfaydark", new LesserFaydarkZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "load", new LoadingAreaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "mischiefplane", new PlaneOfMischiefZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "mistmoore", new CastleMistmooreZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "misty", new MistyThicketZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "najena", new NajenaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "necropolis", new DragonNecropolisZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "nektulos", new NektulosForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "neriaka", new NeriakForeignQuarterZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "neriakb", new NeriakCommonsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "neriakc", new NeriakThirdGateZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "northkarana", new NorthKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "nro", new NorthDesertOfRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "nurga", new MinesOfNurgaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "oasis", new OasisOfMarrZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "oggok", new OggokZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "oot", new OceanOfTearsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "overthere", new OverthereZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "paineel", new PaineelZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "paw", new LairOfTheSplitpawZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "permafrost", new PermafrostCavernsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qcat", new QeynosAqueductsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qey2hh1", new WestKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qeynos", new SouthQeynosZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qeynos2", new NorthQeynosZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qeytoqrg", new QeynosHillsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "qrg", new SurefallGladeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "rathemtn", new RatheMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "rivervale", new RivervaleZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "runnyeye", new RunnyeyeCitadelZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "sebilis", new OldSebilisZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "sirens", new SirensGrottoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "skyfire", new SkyfireMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "skyshrine", new SkyshrineZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "sleeper", new SleeperTombZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "soldunga", new SoluseksEyeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "soldungb", new NagafensLairZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "soltemple", new TempleOfSolusekRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "southkarana", new SouthKaranaZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "swampofnohope", new SwampOfNoHopeZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "sro", new SouthDesertOfRoZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "steamfont", new SteamfontMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "stonebrunt", new StonebruntMountainsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "templeveeshan", new TempleOfVeeshanZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "thurgadina", new ThurgadinZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "thurgadinb", new IcewellKeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "timorous", new TimorousDeepZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "tox", new ToxxuliaForestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "trakanon", new TrakanonsTeethZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "tutorial", new TutorialZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "veeshan", new VeeshansPeakZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "velketor", new VelketorsLabyrinthZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "unrest", new EstateOfUnrestZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "wakening", new WakeningLandZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "warrens", new WarrensZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "warslikswood", new WarsliksWoodsZoneProperties());
            AddZonePropertiesByShortName(zonePropertiesRows, "westwastes", new WesternWastesZoneProperties());
        }
    }
}
