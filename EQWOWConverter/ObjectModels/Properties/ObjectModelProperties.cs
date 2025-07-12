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
using EQWOWConverter.Creatures;

namespace EQWOWConverter.ObjectModels.Properties
{
    internal class ObjectModelProperties
    {
        private static Dictionary<string, ObjectModelProperties> ObjectPropertiesByByName = new Dictionary<string, ObjectModelProperties>();
        private static readonly object ObjectModelsLock = new object();

        public string Name = string.Empty;
        public ObjectModelCustomCollisionType CustomCollisionType = ObjectModelCustomCollisionType.None;
        public HashSet<string> AlwaysBrightMaterialsByName = new HashSet<string>();
        public HashSet<string> AlphaBlendMaterialsByName = new HashSet<string>();
        public float ModelScalePreWorldScale = 1f;
        public float ModelLiftPreWorldScale = 0f;
        public CreatureModelTemplate? CreatureModelTemplate = null;
        public ActiveDoodadAnimType? ActiveDoodadAnimationType = null;
        public float ActiveDoodadAnimSlideValue = 0; 
        public int ActiveDoodadAnimTimeInMS = 0;
        public bool DoGenerateCollisionFromMeshData = true;
        public ObjectModelParticleEmitter? ParticleEmitter = null;
        public bool RenderingEnabled = true; // Note: This is also makes it non-interactive (non-clickable)

        public ObjectModelProperties() { }
        public ObjectModelProperties(ObjectModelProperties other)
        {
            Name = other.Name;
            CustomCollisionType = other.CustomCollisionType;
            AlwaysBrightMaterialsByName = new HashSet<string>(other.AlwaysBrightMaterialsByName);
            AlphaBlendMaterialsByName = new HashSet<string>(other.AlphaBlendMaterialsByName);
            ModelScalePreWorldScale = other.ModelScalePreWorldScale;
            ModelLiftPreWorldScale = other.ModelLiftPreWorldScale;
            CreatureModelTemplate = other.CreatureModelTemplate;
            ActiveDoodadAnimationType = other.ActiveDoodadAnimationType;
            ActiveDoodadAnimSlideValue = other.ActiveDoodadAnimSlideValue;
            ActiveDoodadAnimTimeInMS = other.ActiveDoodadAnimTimeInMS;
            DoGenerateCollisionFromMeshData = other.DoGenerateCollisionFromMeshData;
            ParticleEmitter = other.ParticleEmitter;
            RenderingEnabled = other.RenderingEnabled;
        }

        public ObjectModelProperties(ActiveDoodadAnimType? activeDoodadAnimationType, float activeDoodadAnimSlideValue, int activeDoodadAnimTimeInMS, bool hasCollision,
            bool renderingEnabled)
        {
            ActiveDoodadAnimationType = activeDoodadAnimationType;
            ActiveDoodadAnimSlideValue = activeDoodadAnimSlideValue;
            ActiveDoodadAnimTimeInMS = activeDoodadAnimTimeInMS;
            DoGenerateCollisionFromMeshData = hasCollision;
            RenderingEnabled = renderingEnabled;
        }

        protected ObjectModelProperties(string name)
        {
            Name = name;
            PopulateAllMaterialAlphaBlendMaterials();
        }

        protected void SetCustomCollisionType(ObjectModelCustomCollisionType customCollisionType)
        {
            CustomCollisionType = customCollisionType;
        }

        protected void AddAlwaysBrightMaterial(string materialName)
        {
            if (AlwaysBrightMaterialsByName.Contains(materialName) == false)
                AlwaysBrightMaterialsByName.Add(materialName);
        }

        public static ObjectModelProperties GetObjectPropertiesForObject(string objectName)
        {
            lock (ObjectModelsLock)
            {
                if (ObjectPropertiesByByName.Count == 0)
                    PopulateObjectPropertiesList();
                if (ObjectPropertiesByByName.ContainsKey(objectName) == false)
                    return new ObjectModelProperties(objectName);
                else
                    return ObjectPropertiesByByName[objectName];
            }            
        }

        private static void PopulateObjectPropertiesList()
        {
            ObjectPropertiesByByName.Clear();
            ObjectPropertiesByByName.Add("ladder14", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder20", new Ladder20ObjectProperties());
            ObjectPropertiesByByName.Add("ladder28", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder42", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder60", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("slbraz101", new SLBraz101ObjectPreperties());
            ObjectPropertiesByByName.Add("slfountain101", new SLFountain101ObjectProperties());
            ObjectPropertiesByByName.Add("sltorch101", new SLTorch101ObjectProperties());
        }

        private void PopulateAllMaterialAlphaBlendMaterials()
        {
            AlphaBlendMaterialsByName.Clear();
            AlphaBlendMaterialsByName.Add("d_ub5"); // Treetops that should 'fade into the sky'
            AlphaBlendMaterialsByName.Add("clear"); // Transparent should be completely alpha
        }
    }
}
