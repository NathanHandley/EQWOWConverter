//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelParticleCloudProperties
    {
        private static Dictionary<(string, string), ObjectModelParticleCloudProperties> PropertiesByObjectAndCloudName = new Dictionary<(string, string), ObjectModelParticleCloudProperties>();
        private static readonly object PropertiesLock = new object();

        public string ObjectName = string.Empty; 
        public string ParticleCloudName = string.Empty;
        public float VelocityMod = 1f;
        public float ScaleMod = 1f;
        public float EmitterAddX = 0f;
        public float EmitterAddY = 0f;
        public float EmitterAddZ = 0f;
        public int TransparencyPercentOverride = -1;
        public bool DoFadeOut = true;
        public float RotateW = 0f;
        public float RotateX = 0f;
        public float RotateY = 0f;
        public float RotateZ = 0f;
        public float SpawnRateMod = 1f;
        public bool PersistInWorldSpace = false;
        public float RadiusMod = 1f;
        public float LifespanMod = 1f;
        public int BurstAmount = 1;
        public int BurstDelayInMS = 0;
        public bool ForceStatic = false;

        public ObjectModelParticleCloudProperties() { }

        public static ObjectModelParticleCloudProperties GetPropertiesForObjectCloud(string objectName, string particleCloudName)
        {
            objectName = objectName.Replace("_npc", "");
            lock (PropertiesLock)
            {
                if (PropertiesByObjectAndCloudName.Count == 0)
                    PopulateProperties();
                if (PropertiesByObjectAndCloudName.ContainsKey((objectName, particleCloudName)) == false)
                    return new ObjectModelParticleCloudProperties();
                else
                    return PropertiesByObjectAndCloudName[(objectName, particleCloudName)];
            }
        }

        private static void PopulateProperties()
        {
            lock (PropertiesLock)
            {
                PropertiesByObjectAndCloudName.Clear();

                string propertiesFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ObjectModelParticleCloudProperties.csv");
                Logger.WriteDebug("Populating Object Model Particle Cloud Properties list via file '" + propertiesFileName + "'");
                List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(propertiesFileName, "|");
                foreach (Dictionary<string, string> columns in rows)
                {
                    ObjectModelParticleCloudProperties newProperties = new ObjectModelParticleCloudProperties();
                    newProperties.ObjectName = columns["ObjectName"];
                    newProperties.ParticleCloudName = columns["ParticleCloudName"];
                    newProperties.VelocityMod = float.Parse(columns["VelocityMod"]);
                    newProperties.ScaleMod = float.Parse(columns["ScaleMod"]);
                    newProperties.EmitterAddX = float.Parse(columns["EmitterAddX"]);
                    newProperties.EmitterAddY = float.Parse(columns["EmitterAddY"]);
                    newProperties.EmitterAddZ = float.Parse(columns["EmitterAddZ"]);
                    newProperties.TransparencyPercentOverride = int.Parse(columns["TransparencyPercentOverride"]);
                    newProperties.DoFadeOut = columns["FadeOut"].Trim() == "1" ? true : false;
                    newProperties.RotateW = float.Parse(columns["RotateW"]);
                    newProperties.RotateX = float.Parse(columns["RotateX"]);
                    newProperties.RotateY = float.Parse(columns["RotateY"]);
                    newProperties.RotateZ = float.Parse(columns["RotateZ"]);
                    newProperties.SpawnRateMod = float.Parse(columns["SpawnRateMod"]);
                    newProperties.PersistInWorldSpace = columns["PersistInWorldSpace"].Trim() == "1" ? true : false;
                    newProperties.RadiusMod = float.Parse(columns["RadiusMod"]);
                    newProperties.LifespanMod = float.Parse(columns["LifespanMod"]);
                    newProperties.BurstAmount = int.Parse(columns["BurstAmount"]);
                    newProperties.BurstDelayInMS = int.Parse(columns["BurstDelayMS"]);
                    newProperties.ForceStatic = columns["ForceStatic"].Trim() == "1" ? true : false;
                    PropertiesByObjectAndCloudName.Add((newProperties.ObjectName, newProperties.ParticleCloudName), newProperties);
                }
            }
        }
    }
}
