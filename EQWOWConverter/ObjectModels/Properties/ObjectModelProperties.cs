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
using EQWOWConverter.Creatures;
using EQWOWConverter.EQFiles;
using EQWOWConverter.Items;
using EQWOWConverter.Spells;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelProperties
    {
        private static Dictionary<string, ObjectModelProperties> ObjectPropertiesByName = new Dictionary<string, ObjectModelProperties>();
        private static readonly object PropertiesLock = new object();

        public string Name = string.Empty;
        public ObjectModelCustomCollisionType CustomCollisionType = ObjectModelCustomCollisionType.None;
        public HashSet<string> AlwaysBrightMaterialsByName = new HashSet<string>();
        public float AdditionalScaleMultiplier = 1.0f;
        public EQAnimationType StandAnimEQAnimOverride = EQAnimationType.Unknown;
        public HashSet<string> AlphaBlendMaterialsByName = new HashSet<string>();
        public float ModelScalePreWorldScale = 1f;
        public float ModelLiftPreWorldScale = 0f;
        public CreatureModelTemplate? CreatureModelTemplate = null;
        public ActiveDoodadAnimType? ActiveDoodadAnimationType = null;
        public float ActiveDoodadAnimSlideValue = 0; 
        public int ActiveDoodadAnimTimeInMS = 0;
        public bool DoGenerateCollisionFromMeshData = true;
        public List<ObjectModelParticleEmitter> ParticleEmitters = new List<ObjectModelParticleEmitter>();
        public int SpellVisualEffectNameDBCID;
        public SpellVisualStageType SpellVisualEffectStageType = SpellVisualStageType.None;
        public SpellVisualType SpellVisualType = SpellVisualType.Beneficial;
        public bool SpellEmitterSpraysFromHands = false;
        public bool RenderingEnabled = true; // Note: This is also makes it non-interactive (non-clickable)
        public ItemEquipUnitType EquipUnitType = ItemEquipUnitType.Player;
        public bool IncludeInMinimapGeneration = false;
        public string AlternateModelSwapName = string.Empty;
        public string CustomMaterialListLine = string.Empty;
        public List<string> TransportNonCollideMaterialNames = new List<string>();
        public bool ApplyCollisionIfTransparent = false;
        public int MaterialTransparencyPercentOverride = -1;

        public ObjectModelProperties() { }
        public ObjectModelProperties(ObjectModelProperties other)
        {
            Name = other.Name;
            CustomCollisionType = other.CustomCollisionType;
            AlwaysBrightMaterialsByName = new HashSet<string>(other.AlwaysBrightMaterialsByName);
            AlphaBlendMaterialsByName = new HashSet<string>(other.AlphaBlendMaterialsByName);
            AdditionalScaleMultiplier = other.AdditionalScaleMultiplier;
            StandAnimEQAnimOverride = other.StandAnimEQAnimOverride;
            ModelScalePreWorldScale = other.ModelScalePreWorldScale;
            ModelLiftPreWorldScale = other.ModelLiftPreWorldScale;
            CreatureModelTemplate = other.CreatureModelTemplate;
            ActiveDoodadAnimationType = other.ActiveDoodadAnimationType;
            ActiveDoodadAnimSlideValue = other.ActiveDoodadAnimSlideValue;
            ActiveDoodadAnimTimeInMS = other.ActiveDoodadAnimTimeInMS;
            DoGenerateCollisionFromMeshData = other.DoGenerateCollisionFromMeshData;
            ParticleEmitters.AddRange(other.ParticleEmitters);
            SpellVisualEffectNameDBCID = other.SpellVisualEffectNameDBCID;
            SpellVisualEffectStageType = other.SpellVisualEffectStageType;
            SpellVisualType = other.SpellVisualType;
            SpellEmitterSpraysFromHands = other.SpellEmitterSpraysFromHands;
            RenderingEnabled = other.RenderingEnabled;
            EquipUnitType = other.EquipUnitType;
            IncludeInMinimapGeneration = other.IncludeInMinimapGeneration;
            AlternateModelSwapName = other.AlternateModelSwapName;
            CustomMaterialListLine = other.CustomMaterialListLine;
            TransportNonCollideMaterialNames = new List<string>(other.TransportNonCollideMaterialNames);
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
            objectName = objectName.Replace("_npc", "");
            lock (PropertiesLock)
            {
                if (ObjectPropertiesByName.Count == 0)
                    PopulateObjectPropertiesList();
                if (ObjectPropertiesByName.ContainsKey(objectName) == false)
                    return new ObjectModelProperties(objectName);
                else
                    return ObjectPropertiesByName[objectName];
            }            
        }

        private static void PopulateObjectPropertiesList()
        {
            lock (PropertiesLock)
            {
                ObjectPropertiesByName.Clear();

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
                        case "ladder_wall_attached": newObjectModelProperties.SetCustomCollisionType(ObjectModelCustomCollisionType.LadderWallAttached); break;
                        default:break;
                    }
                    string alwaysBrightMaterialNames = columns["AlwaysBrightMaterials"];
                    if (alwaysBrightMaterialNames.Length > 0)
                    {
                        string[] materialNames = alwaysBrightMaterialNames.Split(',');
                        foreach (string materialName in materialNames)
                            newObjectModelProperties.AddAlwaysBrightMaterial(materialName);
                    }
                    newObjectModelProperties.AdditionalScaleMultiplier = float.Parse(columns["AdditionalScaleMultiplier"]);
                    string standAnimEQAnimOverrideString = columns["StandAnimEQAnimOverride"].Trim().ToLower();
                    if (standAnimEQAnimOverrideString.Length > 0)
                    {
                        switch (standAnimEQAnimOverrideString)
                        {
                            case "o01standidle": newObjectModelProperties.StandAnimEQAnimOverride = EQAnimationType.o01StandIdle; break;
                            case "o02standarmstoside": newObjectModelProperties.StandAnimEQAnimOverride = EQAnimationType.o02StandArmsToSide; break;
                            case "p01standpassive": newObjectModelProperties.StandAnimEQAnimOverride = EQAnimationType.p01StandPassive; break;
                            case "posstandpose": newObjectModelProperties.StandAnimEQAnimOverride = EQAnimationType.posStandPose; break;
                            default:
                                {
                                    Logger.WriteError("Error determining StandAnimEQAnimOverride for ", newObjectModelProperties.Name, " as it is unhandled");
                                } break;
                        }
                    }
                    newObjectModelProperties.AlternateModelSwapName = columns["AlternateModelSwap"].Trim();
                    newObjectModelProperties.CustomMaterialListLine = columns["CustomMaterialListLine"].Trim();
                    newObjectModelProperties.IncludeInMinimapGeneration = columns["IncludeInMinimap"] == "1" ? true : false;
                    string transportNonCollideMaterials = columns["TransportNonCollideMaterials"].Trim();
                    if (transportNonCollideMaterials.Length > 0)
                    {
                        string[] materials = transportNonCollideMaterials.Split(",");
                        foreach (string material in materials)
                            newObjectModelProperties.TransportNonCollideMaterialNames.Add(material.Trim());
                    }
                    newObjectModelProperties.ApplyCollisionIfTransparent = columns["ApplyCollisionIfTransparent"] == "1" ? true : false;
                    newObjectModelProperties.MaterialTransparencyPercentOverride = Convert.ToInt32(columns["MaterialTransparencyPercentOverride"]);
                    ObjectPropertiesByName.Add(newObjectModelProperties.Name, newObjectModelProperties);
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
