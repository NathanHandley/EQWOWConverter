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

using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQAmbientLight
    {
        public ColorRGBA AmbientLight = new ColorRGBA();

        public bool LoadFromDisk(string fileFullPath)
        {
            Logger.WriteDetail(" - Reading EQ Object Instances Data from '" + fileFullPath + "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find object instances file that should be at '" + fileFullPath + "'");
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

                // 3-block is the light
                string[] blocks = inputRow.Split(",");
                if (blocks.Length != 3)
                {
                    Logger.WriteError("- Ambient light data must be in 3 components");
                    return false;
                }
                AmbientLight.R = byte.Parse(blocks[0]);
                AmbientLight.G = byte.Parse(blocks[1]);
                AmbientLight.B = byte.Parse(blocks[2]);
                return true;
            }

            Logger.WriteDetail(" - Done reading ambient light data from '" + fileFullPath + "'");
            return true;
        }
    }
}
