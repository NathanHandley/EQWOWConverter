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
using EQWOWConverter.Items;
using EQWOWConverter.Spells;

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
        public List<ObjectModelParticleEmitter> SingleSpriteSpellParticleEmitters = new List<ObjectModelParticleEmitter>();
        public int SpellVisualEffectNameDBCID;
        public SpellVisualStageType SpellVisualEffectStageType = SpellVisualStageType.None;
        public SpellVisualType SpellVisualType = SpellVisualType.Beneficial;
        public bool SpellEmitterSpraysFromHands = false;
        public bool RenderingEnabled = true; // Note: This is also makes it non-interactive (non-clickable)
        public ItemEquipUnitType EquipUnitType = ItemEquipUnitType.Player;
        public bool IncludeInMinimapGeneration = false;

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
            SingleSpriteSpellParticleEmitters.AddRange(other.SingleSpriteSpellParticleEmitters);
            SpellVisualEffectNameDBCID = other.SpellVisualEffectNameDBCID;
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
            lock (ObjectModelsLock)
            {
                ObjectPropertiesByByName.Clear();

                string objectModelPropertiesFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ObjectModelProperties.csv");
                Logger.WriteDebug("Populating Object Model Properties list via file '" + objectModelPropertiesFileName + "'");
                List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(objectModelPropertiesFileName, "|");
                foreach (Dictionary<string, string> columns in rows)
                {
                    ObjectModelProperties newObjectModelProperties = new ObjectModelProperties();
                    newObjectModelProperties.Name = columns["Name"];
                    switch (columns["SpecialCollisionType"])
                    {
                        case "ladder": newObjectModelProperties.SetCustomCollisionType(ObjectModelCustomCollisionType.Ladder); break;
                        case "ladder_right_angle": newObjectModelProperties.SetCustomCollisionType(ObjectModelCustomCollisionType.LadderRightAngle); break;
                        default:break;
                    }
                    string alwaysBrightMaterialNames = columns["AlwaysBrightMaterials"];
                    if (alwaysBrightMaterialNames.Length > 0)
                    {
                        string[] materialNames = alwaysBrightMaterialNames.Split(',');
                        foreach (string materialName in materialNames)
                            newObjectModelProperties.AddAlwaysBrightMaterial(materialName);
                    }
                    newObjectModelProperties.IncludeInMinimapGeneration = columns["IncludeInMinimap"] == "1" ? true : false;
                    ObjectPropertiesByByName.Add(newObjectModelProperties.Name, newObjectModelProperties);
                }
            }
        }

        private void PopulateAllMaterialAlphaBlendMaterials()
        {
            AlphaBlendMaterialsByName.Clear();
            AlphaBlendMaterialsByName.Add("d_ub5"); // Treetops that should 'fade into the sky'
            AlphaBlendMaterialsByName.Add("clear"); // Transparent should be completely alpha
        }
    }
}
