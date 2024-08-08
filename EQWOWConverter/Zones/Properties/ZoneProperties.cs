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
using EQWOWConverter.Zones.Properties;

namespace EQWOWConverter.Zones
{
    internal class ZoneProperties
    {
        static public Dictionary<string, ZoneProperties> ZonePropertyListByShortName = new Dictionary<string, ZoneProperties>();
        static public ZoneEnvironmentSettings CommonOutdoorEnvironmentProperties = new ZoneEnvironmentSettings();

        public string ShortName = string.Empty;
        public string DescriptiveName = string.Empty;
        public ZoneContinentType Continent;
        public bool IsCompletelyInLiquid = false;
        public bool HasShadowBox = false;
        public ZoneLiquidType CompletelyInLiquidType = ZoneLiquidType.None;
        public Vector3 SafePosition = new Vector3();
        public float SafeOrientation = 0;
        public List<ZonePropertiesZoneLineBox> ZoneLineBoxes = new List<ZonePropertiesZoneLineBox>();
        public List<ZoneLiquidVolume> LiquidVolumes = new List<ZoneLiquidVolume>();       
        public List<ZoneLiquidPlane> LiquidPlanes = new List<ZoneLiquidPlane>();

        // Environment Properties
        public ZoneEnvironmentSettings? CustomZonewideEnvironmentProperties = null;
        public double VertexColorIntensityOverride = -1;

        // DBCIDs
        private static int CURRENT_MAPID = Configuration.CONFIG_DBCID_MAPID_START;
        private static UInt32 CURRENT_WMOID = Configuration.CONFIG_DBCID_WMOID_START;
        private static UInt32 CURRENT_AREAID = Configuration.CONFIG_DBCID_AREAID_START;
        private static int CURRENT_ROWID = Configuration.CONFIG_DBCID_MAPDIFFICULTYID_START;
        public int DBCMapID;
        public int DBCMapDifficultyID;
        public UInt32 DBCAreaID;
        public UInt32 DBCWMOID;

        protected ZoneProperties()
        {
            // Generate zone-specific IDs
            DBCMapID = CURRENT_MAPID;
            CURRENT_MAPID++;
            DBCAreaID = CURRENT_AREAID;
            CURRENT_AREAID++;
            DBCWMOID = CURRENT_WMOID;
            CURRENT_WMOID++;
            DBCMapDifficultyID = CURRENT_ROWID;
            CURRENT_ROWID++;
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void SetBaseZoneProperties(string shortName, string descriptiveName, float safeX, float safeY, float safeZ, float safeOrientation, ZoneContinentType continent)
        {
            ShortName = shortName;
            DescriptiveName = descriptiveName;
            Continent = continent;
            SafePosition.X = safeX;
            SafePosition.Y = safeY;
            SafePosition.Z = safeZ;
            SafeOrientation = safeOrientation;
        }

        protected void SetFogProperties(byte red, byte green, byte blue, int minClip, int maxClip)
        {
            // Does nothing for now
            //FogColor.R = red;
            //FogColor.G = green;
            //FogColor.B = blue;
            //FogMinClip = minClip;
            //FogMaxClip = maxClip;
        }

        protected void DisableSunlight()
        {
            HasShadowBox = true;
        }

        protected void SetZonewideEnvironmentAsIndoors(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as indoor foggy, but the zonewide environment settings were already set. There could be issues.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsIndoors(fogRed, fogGreen, fogBlue, fogType,
                Configuration.CONFIG_LIGHT_DEFAULT_INDOOR_AMBIENCE,
                Configuration.CONFIG_LIGHT_DEFAULT_INDOOR_AMBIENCE,
                Configuration.CONFIG_LIGHT_DEFAULT_INDOOR_AMBIENCE);
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
                Logger.WriteInfo("Warning: Environment set as outdoors foggy, but the zonewide environment settings were already set. There could be issues.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(fogRed, fogGreen, fogBlue, fogType, true, cloudDensity, ambientBrightnessMod);
        }

        protected void SetZonewideEnvironmentAsOutdoorsNoSky(byte fogRed, byte fogGreen, byte fogBlue, ZoneFogType fogType, float ambientBrightnessMod)
        {
            if (CustomZonewideEnvironmentProperties != null)
                Logger.WriteInfo("Warning: Environment set as outdoors foggy, but the zonewide environment settings were already set. There could be issues.");
            CustomZonewideEnvironmentProperties = new ZoneEnvironmentSettings();
            CustomZonewideEnvironmentProperties.SetAsOutdoors(fogRed, fogGreen, fogBlue, fogType, false, 1f, ambientBrightnessMod);
        }

        protected void OverrideVertexColorIntensity(double overrideIntensityAmount)
        {
            VertexColorIntensityOverride = overrideIntensityAmount;
        }

        protected void SetIsCompletelyInLiquid(ZoneLiquidType liquidType)
        {
            IsCompletelyInLiquid = true;
            CompletelyInLiquidType = liquidType;
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
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

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddTeleportPad(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY, float targetZonePositionZ, 
            ZoneLineOrientationType targetZoneOrientation, float padBottomCenterXPosition, float padBottomCenterYPosition, float padBottomCenterZPosition,
            float padWidth)
        {
            ZonePropertiesZoneLineBox zoneLineBox = new ZonePropertiesZoneLineBox(targetZoneShortName, targetZonePositionX, targetZonePositionY, targetZonePositionZ,
            targetZoneOrientation, padBottomCenterXPosition, padBottomCenterYPosition, padBottomCenterZPosition, padWidth);
            ZoneLineBoxes.Add(zoneLineBox);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddLiquidVolume(ZoneLiquidType liquidType, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ)
        {
            ZoneLiquidVolume liquidVolume = new ZoneLiquidVolume(liquidType, nwCornerX, nwCornerY, seCornerX, seCornerY, highZ, lowZ);
            LiquidVolumes.Add(liquidVolume);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddLiquidPlane(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ, ZoneLiquidSlantType slantType, float minDepth)
        {
            ZoneLiquidPlane liquidPlane = new ZoneLiquidPlane(liquidType, materialName, nwCornerX, nwCornerY, seCornerX, seCornerY,
                highZ, lowZ, slantType, minDepth);
            LiquidPlanes.Add(liquidPlane);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddLiquidPlaneZLevel(ZoneLiquidType liquidType, string materialName, float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float fixedZ, float minDepth)
        {
            ZoneLiquidPlane liquidPlane = new ZoneLiquidPlane(liquidType, materialName, nwCornerX, nwCornerY, seCornerX,
                seCornerY, fixedZ, minDepth);
            LiquidPlanes.Add(liquidPlane);
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

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
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
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornersZ, minDepth);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
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
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornerZ, minDepth);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddLiquidCylinder(ZoneLiquidType liquidType, string materialName, float centerX, float centerY, float radius, float topZ,
            float height, float maxX, float maxY, float minX, float minY, float stepSize)
        {
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
                AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, topZ, height);

                // Step
                relativeWorkingX -= stepSize;
            }
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddTrapezoidLiquidAxisAlignedZLevelShape(ZoneLiquidType liquidType, string materialName, float northEdgeX, float southEdgeX, float northWestY, float northEastY,
            float southWestY, float southEastY, float topZ, float height, float stepSize)
        {
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
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, topZ, height);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddQuadrilateralLiquidShapeZLevel(ZoneLiquidType liquidType, string materialName, float northMostX, float northMostY, float westMostX, float westMostY,
            float southMostX, float southMostY, float eastMostX, float eastMostY, float allCornersZ, float minDepth)
        {
            AddQuadrilateralLiquidShapeZLevel(liquidType, materialName, northMostX, northMostY, westMostX, westMostY, southMostX, southMostY,
                eastMostX, eastMostY, allCornersZ, minDepth, northMostX, westMostY, southMostX, eastMostY, Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_EDGE_WALK_SIZE);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
        protected void AddOctagonLiquidShape(ZoneLiquidType liquidType, string materialName, float northEdgeX, float southEdgeX, float westEdgeY, float eastEdgeY, float northWestY, float northEastY,
            float southWestY, float southEastY, float westNorthX, float westSouthX, float eastNorthX, float eastSouthX, float allCornersZ, float minDepth)
        {
            AddOctagonLiquidShape(liquidType, materialName, northEdgeX, southEdgeX, westEdgeY, eastEdgeY, northWestY, northEastY, southWestY, southEastY, westNorthX,
                westSouthX, eastNorthX, eastSouthX, allCornersZ, minDepth, Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_EDGE_WALK_SIZE);
        }

        // Values should be pre-Scaling (before * CONFIG_EQTOWOW_WORLD_SCALE)
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

            // Start at the north X and walk down to the south X
            float curXTop = northEdgeX;
            bool moreXToWalk = true;
            while (moreXToWalk == true)
            {
                // If in the middle block, fill it in
                if (curXTop <= westNorthX && curXTop <= eastNorthX && curXTop >= westSouthX && curXTop >= eastSouthX)
                {
                    float highestSouthEastX = MathF.Max(westSouthX, eastSouthX);
                    AddLiquidPlaneZLevel(liquidType, materialName, curXTop, westEdgeY, highestSouthEastX, eastEdgeY, allCornersZ, minDepth);
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
                    AddLiquidPlaneZLevel(liquidType, materialName, nwX, nwY, seX, seY, allCornersZ, minDepth);

                // Set new top factoring for overlap
                curXTop = curXBottom -= Configuration.CONFIG_EQTOWOW_LIQUID_QUADGEN_PLANE_OVERLAP_SIZE;
            }
        }

        public static ZoneProperties GetZonePropertiesForZone(string zoneShortName)
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

        private static void PopulateZonePropertiesList()
        {
            // Classic
            ZonePropertyListByShortName.Add("airplane", new PlaneOfSkyZoneProperties());
            ZonePropertyListByShortName.Add("akanon", new AkAnonZoneProperties());
            ZonePropertyListByShortName.Add("arena", new ArenaZoneProperties());
            ZonePropertyListByShortName.Add("befallen", new BefallenZoneProperties());
            ZonePropertyListByShortName.Add("beholder", new GorgeOfKingXorbbZoneProperties());
            ZonePropertyListByShortName.Add("blackburrow", new BlackburrowZoneProperties());
            ZonePropertyListByShortName.Add("butcher", new ButcherblockMountainsZoneProperties());
            ZonePropertyListByShortName.Add("cauldron", new DagnorsCauldronZoneProperties());
            ZonePropertyListByShortName.Add("cazicthule", new CazicThuleZoneProperties());
            ZonePropertyListByShortName.Add("commons", new WestCommonsZoneProperties());
            ZonePropertyListByShortName.Add("crushbone", new CrushboneZoneProperties());
            ZonePropertyListByShortName.Add("eastkarana", new EastKaranaZoneProperties());
            ZonePropertyListByShortName.Add("ecommons", new EastCommonsZoneProperties());
            ZonePropertyListByShortName.Add("erudnext", new ErudinZoneProperties());
            ZonePropertyListByShortName.Add("erudnint", new ErudinPalaceZoneProperties());
            ZonePropertyListByShortName.Add("erudsxing", new ErudsCrossingZoneProperties());
            ZonePropertyListByShortName.Add("everfrost", new EverfrostZoneProperties());
            ZonePropertyListByShortName.Add("fearplane", new PlaneOfFearZoneProperties());
            ZonePropertyListByShortName.Add("feerrott", new FeerrottZoneProperties());
            ZonePropertyListByShortName.Add("felwithea", new NorthFelwitheZoneProperties());
            ZonePropertyListByShortName.Add("felwitheb", new SouthFelwitheZoneProperties());
            ZonePropertyListByShortName.Add("freporte", new EastFreeportZoneProperties());
            ZonePropertyListByShortName.Add("freportn", new NorthFreeportZoneProperties());
            ZonePropertyListByShortName.Add("freportw", new WestFreeportZoneProperties());
            ZonePropertyListByShortName.Add("gfaydark", new GreaterFaydarkZoneProperties());
            ZonePropertyListByShortName.Add("grobb", new GrobbZoneProperties());
            ZonePropertyListByShortName.Add("gukbottom", new GukBottomZoneProperties());
            ZonePropertyListByShortName.Add("guktop", new GukTopZoneProperties());
            ZonePropertyListByShortName.Add("halas", new HalasZoneProperties());
            ZonePropertyListByShortName.Add("hateplane", new PlaneOfHateZoneProperties());
            ZonePropertyListByShortName.Add("highkeep", new HighKeepZoneProperties());
            ZonePropertyListByShortName.Add("highpass", new HighpassHoldZoneProperties());
            ZonePropertyListByShortName.Add("hole", new HoleZoneProperties());
            ZonePropertyListByShortName.Add("innothule", new InnothuleSwampZoneProperties());
            ZonePropertyListByShortName.Add("kaladima", new SouthKaladimZoneProperties());
            ZonePropertyListByShortName.Add("kaladimb", new NorthKaladimZoneProperties());
            ZonePropertyListByShortName.Add("kedge", new KedgeKeepZoneProperties());
            ZonePropertyListByShortName.Add("kerraridge", new KerraIsleZoneProperties());
            ZonePropertyListByShortName.Add("kithicor", new KithicorForestZoneProperties());
            ZonePropertyListByShortName.Add("lakerathe", new LakeRathetearZoneProperties());
            ZonePropertyListByShortName.Add("lavastorm", new LavastormMountainsZoneProperties());
            ZonePropertyListByShortName.Add("lfaydark", new LesserFaydarkZoneProperties());
            ZonePropertyListByShortName.Add("mistmoore", new CastleMistmooreZoneProperties());
            ZonePropertyListByShortName.Add("misty", new MistyThicketZoneProperties());
            ZonePropertyListByShortName.Add("najena", new NajenaZoneProperties());
            ZonePropertyListByShortName.Add("nektulos", new NektulosForestZoneProperties());
            ZonePropertyListByShortName.Add("neriaka", new NeriakForeignQuarterZoneProperties());
            ZonePropertyListByShortName.Add("neriakb", new NeriakCommonsZoneProperties());
            ZonePropertyListByShortName.Add("neriakc", new NeriakThirdGateZoneProperties());
            ZonePropertyListByShortName.Add("northkarana", new NorthKaranaZoneProperties());
            ZonePropertyListByShortName.Add("nro", new NorthDesertOfRoZoneProperties());
            ZonePropertyListByShortName.Add("oasis", new OasisOfMarrZoneProperties());
            ZonePropertyListByShortName.Add("oggok", new OggokZoneProperties());
            ZonePropertyListByShortName.Add("oot", new OceanOfTearsZoneProperties());
            ZonePropertyListByShortName.Add("paineel", new PaineelZoneProperties());
            ZonePropertyListByShortName.Add("paw", new LairOfTheSplitpawZoneProperties());
            ZonePropertyListByShortName.Add("permafrost", new PermafrostCavernsZoneProperties());
            ZonePropertyListByShortName.Add("qcat", new QeynosAqueductsZoneProperties());
            ZonePropertyListByShortName.Add("qey2hh1", new WestKaranaZoneProperties());
            ZonePropertyListByShortName.Add("qeynos", new SouthQeynosZoneProperties());
            ZonePropertyListByShortName.Add("qeynos2", new NorthQeynosZoneProperties());
            ZonePropertyListByShortName.Add("qeytoqrg", new QeynosHillsZoneProperties());
            ZonePropertyListByShortName.Add("qrg", new SurefallGladeZoneProperties());
            ZonePropertyListByShortName.Add("rathemtn", new RatheMountainsZoneProperties());
            ZonePropertyListByShortName.Add("rivervale", new RivervaleZoneProperties());
            ZonePropertyListByShortName.Add("runnyeye", new RunnyeyeCitadelZoneProperties());
            ZonePropertyListByShortName.Add("soldunga", new SoluseksEyeZoneProperties());
            ZonePropertyListByShortName.Add("soldungb", new NagafensLairZoneProperties());
            ZonePropertyListByShortName.Add("soltemple", new TempleOfSolusekRoZoneProperties());
            ZonePropertyListByShortName.Add("southkarana", new SouthKaranaZoneProperties());
            ZonePropertyListByShortName.Add("sro", new SouthDesertOfRoZoneProperties());
            ZonePropertyListByShortName.Add("steamfont", new SteamfontMountainsZoneProperties());
            ZonePropertyListByShortName.Add("stonebrunt", new StonebruntMountainsZoneProperties());
            ZonePropertyListByShortName.Add("tox", new ToxxuliaForestZoneProperties());
            ZonePropertyListByShortName.Add("unrest", new EstateOfUnrestZoneProperties());
            ZonePropertyListByShortName.Add("warrens", new WarrensZoneProperties());

            // Misc
            ZonePropertyListByShortName.Add("load", new LoadingAreaZoneProperties());
            ZonePropertyListByShortName.Add("tutorial", new TutorialZoneProperties());

            // Kunark
            ZonePropertyListByShortName.Add("burningwood", new BurningWoodZoneProperties());
            ZonePropertyListByShortName.Add("cabeast", new CabilisEastZoneProperties());
            ZonePropertyListByShortName.Add("cabwest", new CabilisWestZoneProperties());
            ZonePropertyListByShortName.Add("charasis", new HowlingStonesZoneProperties());
            ZonePropertyListByShortName.Add("chardok", new ChardokZoneProperties());
            ZonePropertyListByShortName.Add("citymist", new CityOfMistZoneProperties());
            ZonePropertyListByShortName.Add("dalnir", new DalnirZoneProperties());
            ZonePropertyListByShortName.Add("dreadlands", new DreadlandsZoneProperties());
            ZonePropertyListByShortName.Add("droga", new TempleOfDrogaZoneProperties());
            ZonePropertyListByShortName.Add("emeraldjungle", new EmeraldJungleZoneProperties());
            ZonePropertyListByShortName.Add("fieldofbone", new FieldOfBoneZoneProperties());
            ZonePropertyListByShortName.Add("firiona", new FirionaVieZoneProperties());
            ZonePropertyListByShortName.Add("frontiermtns", new FrontierMountainsZoneProperties());
            ZonePropertyListByShortName.Add("kaesora", new KaesoraZoneProperties());
            ZonePropertyListByShortName.Add("karnor", new KarnorsCastleZoneProperties());
            ZonePropertyListByShortName.Add("kurn", new KurnsTowerZoneProperties());
            ZonePropertyListByShortName.Add("lakeofillomen", new LakeOfIllOmenZoneProperties());
            ZonePropertyListByShortName.Add("nurga", new MinesOfNurgaZoneProperties());
            ZonePropertyListByShortName.Add("overthere", new OverthereZoneProperties());
            ZonePropertyListByShortName.Add("sebilis", new OldSebilisZoneProperties());
            ZonePropertyListByShortName.Add("skyfire", new SkyfireMountainsZoneProperties());
            ZonePropertyListByShortName.Add("swampofnohope", new SwampOfNoHopeZoneProperties());
            ZonePropertyListByShortName.Add("timorous", new TimorousDeepZoneProperties());
            ZonePropertyListByShortName.Add("trakanon", new TrakanonsTeethZoneProperties());
            ZonePropertyListByShortName.Add("veeshan", new VeeshansPeakZoneProperties());
            ZonePropertyListByShortName.Add("wakening", new WakeningLandZoneProperties());
            ZonePropertyListByShortName.Add("warslikswood", new WarsliksWoodsZoneProperties());

            // Velious
            ZonePropertyListByShortName.Add("cobaltscar", new CobaltScarZoneProperties());
            ZonePropertyListByShortName.Add("crystal", new CrystalCavernsZoneProperties());
            ZonePropertyListByShortName.Add("eastwastes", new EasternWastesZoneProperties());
            ZonePropertyListByShortName.Add("frozenshadow", new TowerOfFrozenShadowZoneProperties());
            ZonePropertyListByShortName.Add("greatdivide", new GreatDivideZoneProperties());
            ZonePropertyListByShortName.Add("growthplane", new PlaneOfGrowthZoneProperties());
            ZonePropertyListByShortName.Add("iceclad", new IcecladOceanZoneProperties());
            ZonePropertyListByShortName.Add("kael", new KaelDrakkalZoneProperties());
            ZonePropertyListByShortName.Add("mischiefplane", new PlaneOfMischiefZoneProperties());
            ZonePropertyListByShortName.Add("necropolis", new DragonNecropolisZoneProperties());
            ZonePropertyListByShortName.Add("sirens", new SirensGrottoZoneProperties());
            ZonePropertyListByShortName.Add("skyshrine", new SkyshrineZoneProperties());
            ZonePropertyListByShortName.Add("sleeper", new SleeperTombZoneProperties());
            ZonePropertyListByShortName.Add("templeveeshan", new TempleOfVeeshanZoneProperties());
            ZonePropertyListByShortName.Add("thurgadina", new ThurgadinZoneProperties());
            ZonePropertyListByShortName.Add("thurgadinb", new IcewellKeepZoneProperties());
            ZonePropertyListByShortName.Add("velketor", new VelketorsLabyrinthZoneProperties());
            ZonePropertyListByShortName.Add("westwastes", new WesternWastesZoneProperties());
        }
    }
}
