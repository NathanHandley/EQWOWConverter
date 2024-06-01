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
    internal class ZonePropertiesLineBox
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

        public ZonePropertiesLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY,
            float targetZonePositionZ, ZoneLineOrientationType targetZoneOrientation, float boxTopNorthwestX, float boxTopNorthwestY,
            float boxTopNorthwestZ, float boxBottomSoutheastX, float boxBottomSoutheastY, float boxBottomSoutheastZ)
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();

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
            TargetZoneShortName = targetZoneShortName;
            TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch (targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: TargetZoneOrientation = 0; break;
                case ZoneLineOrientationType.South: TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            BoxPosition = zoneLineBoxBounding.GetCenter();
            BoxWidth = zoneLineBoxBounding.GetYDistance();
            BoxLength = zoneLineBoxBounding.GetXDistance();
            BoxHeight = zoneLineBoxBounding.GetZDistance();
        }

        public ZonePropertiesLineBox(string targetZoneShortName, float targetZonePositionX, float targetZonePositionY, float targetZonePositionZ,
            ZoneLineOrientationType targetZoneOrientation, float padBottomCenterXPosition, float padBottomCenterYPosition, float padBottomCenterZPosition,
            float padWidth)
        {
            AreaTriggerID = AreaTriggerDBC.GetGeneratedAreaTriggerID();

            // Scale input values
            targetZonePositionX *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            targetZonePositionY *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            targetZonePositionZ *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            padBottomCenterXPosition *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            padBottomCenterYPosition *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            padBottomCenterZPosition *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;
            padWidth *= Configuration.CONFIG_EQTOWOW_WORLD_SCALE;

            // Create the box base values
            TargetZoneShortName = targetZoneShortName;
            TargetZonePosition = new Vector3(targetZonePositionX, targetZonePositionY, targetZonePositionZ);
            switch (targetZoneOrientation)
            {
                case ZoneLineOrientationType.North: TargetZoneOrientation = 0; break;
                case ZoneLineOrientationType.South: TargetZoneOrientation = Convert.ToSingle(Math.PI); break;
                case ZoneLineOrientationType.West: TargetZoneOrientation = Convert.ToSingle(Math.PI * 0.5); break;
                case ZoneLineOrientationType.East: TargetZoneOrientation = Convert.ToSingle(Math.PI * 1.5); break;
            }

            // Calculate the dimensions in the form needed by a wow trigger zone
            float boxBottomSoutheastX = padBottomCenterXPosition - (padWidth / 2);
            float boxBottomSoutheastY = padBottomCenterYPosition - (padWidth / 2);
            float boxBottomSoutheastZ = padBottomCenterZPosition - (0.25f * Configuration.CONFIG_EQTOWOW_WORLD_SCALE);
            float boxTopNorthwestX = boxBottomSoutheastX + padWidth;
            float boxTopNorthwestY = boxBottomSoutheastY + padWidth;
            float boxTopNorthwestZ = padBottomCenterZPosition + (5.0f * Configuration.CONFIG_EQTOWOW_WORLD_SCALE);
            BoundingBox zoneLineBoxBounding = new BoundingBox(boxBottomSoutheastX, boxBottomSoutheastY, boxBottomSoutheastZ,
                boxTopNorthwestX, boxTopNorthwestY, boxTopNorthwestZ);
            BoxPosition = zoneLineBoxBounding.GetCenter();
            BoxWidth = zoneLineBoxBounding.GetYDistance();
            BoxLength = zoneLineBoxBounding.GetXDistance();
            BoxHeight = zoneLineBoxBounding.GetZDistance();
        }
    }
}
