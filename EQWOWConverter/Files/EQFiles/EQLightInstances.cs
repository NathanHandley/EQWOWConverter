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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQLightInstances
    {
        public List<LightInstance> LightInstances = new List<LightInstance>();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Light Instances Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find light instances file that should be at '" + fileFullPath + "'");
                return false;
            }

            // Load the data
            string inputData = File.ReadAllText(fileFullPath);
            string[] inputRows = inputData.Split(Environment.NewLine);
            foreach (string inputRow in inputRows)
            {
                // Nothing for blank lines
                if (inputRow.Length == 0)
                    continue;

                // # = comment
                else if (inputRow.StartsWith("#"))
                    continue;

                // 7-blocks is a light instance
                else
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 7)
                    {
                        Logger.WriteError("- Light instance data is 7 components");
                        continue;
                    }
                    LightInstance newLightInstance = new LightInstance();
                    newLightInstance.Position.X = float.Parse(blocks[0]);
                    newLightInstance.Position.Y = float.Parse(blocks[1]);
                    newLightInstance.Position.Z = float.Parse(blocks[2]);
                    newLightInstance.Radius = float.Parse(blocks[3]);
                    newLightInstance.SetColor(float.Parse(blocks[4]), float.Parse(blocks[5]), float.Parse(blocks[6]));
                    LightInstances.Add(newLightInstance);
                }
            }

            Logger.WriteDetail(" - Done reading EQ Light Instances from '" + fileFullPath + "'");
            return true;
        }
    }
}
