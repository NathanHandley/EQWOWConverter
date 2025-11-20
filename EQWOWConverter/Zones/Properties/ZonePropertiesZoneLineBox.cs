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
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Zones
{
    internal class ZonePropertiesZoneLineBox
    {
        public int AreaTriggerID;
        public string TargetZoneShortName = string.Empty;
        public Vector3 TargetZonePosition = new Vector3();
        public float TargetZoneOrientation = 0f;
        public Vector3 BoxPosition = new Vector3();
        public float BoxLength;
        public float BoxWidth;
        public float BoxHeight;
        public float BoxOrientation = 0.0f;
        public float NorthEdgeWorldScaled = 0.0f; // Used for displayed world maps
        public float SouthEdgeWorldScaled = 0.0f; // Used for displayed world maps
        public float WestEdgeWorldScaled = 0.0f; // Used for displayed world maps
        public float EastEdgeWorldScaled = 0.0f; // Used for displayed world maps

        public ZonePropertiesZoneLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY,
            float targetZonePositionZ, ZoneLineOrientationType targetZoneOrientation, float boxTopNorthwestX, float boxTopNorthwestY,
            float boxTopNorthwestZ, float boxBottomSoutheastX, float boxBottomSoutheastY, float boxBottomSoutheastZ)
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();

            // Scale input values
            targetZonePositionX *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionY *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionZ *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestX *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestY *= Configuration.GENERATE_WORLD_SCALE;
            boxTopNorthwestZ *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastX *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastY *= Configuration.GENERATE_WORLD_SCALE;
            boxBottomSoutheastZ *= Configuration.GENERATE_WORLD_SCALE;

            // Save values for the display map
            // Many were entered in incorrectly, so fixing it here
            NorthEdgeWorldScaled = Math.Max(boxTopNorthwestX, boxBottomSoutheastX);
            WestEdgeWorldScaled = Math.Max(boxTopNorthwestY, boxBottomSoutheastY);
            SouthEdgeWorldScaled = Math.Min(boxBottomSoutheastX, boxTopNorthwestX);
            EastEdgeWorldScaled = Math.Min(boxBottomSoutheastY, boxTopNorthwestY);

            // Create the box base values
            TargetZoneShortName = targetZoneShortName;
            TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch (targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: TargetZoneOrientation = 0; break;
                case ZoneLineOrientationType.South: TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
                case ZoneLineOrientationType.NorthWest: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.25); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            BoxPosition = zoneLineBoxBounding.GetCenter();
            BoxWidth = zoneLineBoxBounding.GetYDistance();
            BoxLength = zoneLineBoxBounding.GetXDistance();
            BoxHeight = zoneLineBoxBounding.GetZDistance();
        }

        public ZonePropertiesZoneLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY, float targetZonePositionZ,
            ZoneLineOrientationType targetZoneOrientation, float padBottomCenterXPosition, float padBottomCenterYPosition, float padBottomCenterZPosition,
            float padWidth)
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();

            // Scale input values
            targetZonePositionX *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionY *= Configuration.GENERATE_WORLD_SCALE;
            targetZonePositionZ *= Configuration.GENERATE_WORLD_SCALE;
            padBottomCenterXPosition *= Configuration.GENERATE_WORLD_SCALE;
            padBottomCenterYPosition *= Configuration.GENERATE_WORLD_SCALE;
            padBottomCenterZPosition *= Configuration.GENERATE_WORLD_SCALE;
            padWidth *= Configuration.GENERATE_WORLD_SCALE;

            // Create the box base values
            TargetZoneShortName = targetZoneShortName;
            TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch (targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: TargetZoneOrientation = 0; break;
                case ZoneLineOrientationType.South: TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
                case ZoneLineOrientationType.NorthWest: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.25); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            float boxBottomSoutheastX = padBottomCenterXPosition - (padWidth / 2);
            float boxBottomSoutheastY = padBottomCenterYPosition - (padWidth / 2);
            float boxBottomSoutheastZ = padBottomCenterZPosition - (0.25f * Configuration.GENERATE_WORLD_SCALE);
            float boxTopNorthwestX = boxBottomSoutheastX + padWidth;
            float boxTopNorthwestY = boxBottomSoutheastY + padWidth;
            float boxTopNorthwestZ = padBottomCenterZPosition + (5.0f * Configuration.GENERATE_WORLD_SCALE);
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            BoxPosition = zoneLineBoxBounding.GetCenter();
            BoxWidth = zoneLineBoxBounding.GetYDistance();
            BoxLength = zoneLineBoxBounding.GetXDistance();
            BoxHeight = zoneLineBoxBounding.GetZDistance();

            // Save values for the display map
            // Many were entered in incorrectly, so fixing it here
            NorthEdgeWorldScaled = Math.Max(boxTopNorthwestX, boxBottomSoutheastX);
            WestEdgeWorldScaled = Math.Max(boxTopNorthwestY, boxBottomSoutheastY);
            SouthEdgeWorldScaled = Math.Min(boxBottomSoutheastX, boxTopNorthwestX);
            EastEdgeWorldScaled = Math.Min(boxBottomSoutheastY, boxTopNorthwestY);
        }
    }
}
