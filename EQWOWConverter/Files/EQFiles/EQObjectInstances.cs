using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.EQFiles
{
    internal class EQObjectInstances
    {
        public List<ObjectInstance> ObjectInstances = new List<ObjectInstance>();

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

                //11-blocks is an object instance
                else
                {
                    string[] blocks = inputRow.Split(",");
                    if (blocks.Length != 11)
                    {
                        Logger.WriteError("- Object instance data is 11 components");
                        continue;
                    }

                    ObjectInstance newObjectInstance = new ObjectInstance();
                    newObjectInstance.ModelName = blocks[0];
                    newObjectInstance.Position.X = float.Parse(blocks[1]);
                    newObjectInstance.Position.Y = float.Parse(blocks[2]);
                    newObjectInstance.Position.Z = float.Parse(blocks[3]);
                    newObjectInstance.Rotation.X = float.Parse(blocks[4]);
                    newObjectInstance.Rotation.Y = float.Parse(blocks[5]);
                    newObjectInstance.Rotation.Z = float.Parse(blocks[6]);
                    newObjectInstance.Scale.X = float.Parse(blocks[7]);
                    newObjectInstance.Scale.Y = float.Parse(blocks[8]);
                    newObjectInstance.Scale.Z = float.Parse(blocks[9]);
                    newObjectInstance.ColorIndex = Int32.Parse(blocks[10]);
                    ObjectInstances.Add(newObjectInstance);
                }
            }

            Logger.WriteDetail(" - Done reading EQ Object Instances from '" + fileFullPath + "'");
            return true;
        }
    }
}
