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
using EQWOWConverter.EQFiles;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.Spells
{
    internal class SpellVisual
    {
        private static EQSpellsEFF? EQSpellsEFF = null;
        private static readonly object SpellVisualLock = new object();
        private static Dictionary<SpellVisualType, List<SpellVisual>> SpellVisualsByType = new Dictionary<SpellVisualType, List<SpellVisual>>();
        private static List<ObjectModel> AllEmitterObjectModels = new List<ObjectModel>();
        public static Dictionary<string, Sound> SoundsByFileNameNoExt = new Dictionary<string, Sound>();

        public int SpellVisualDBCID = 0;
        public int[] SpellVisualKitDBCIDsInStage = new int[3];
        public AnimationType[] AnimationTypeInStage = new AnimationType[3];
        public int[] SoundEntryDBCIDInStage = new int[3];
        public int EQVisualEffectIndex = 0;
        public Dictionary<SpellEmitterModelAttachLocationType, ObjectModel> PrecastEmitterObjectModelByAttachLocation = new Dictionary<SpellEmitterModelAttachLocationType, ObjectModel>();
        public Dictionary<SpellEmitterModelAttachLocationType, ObjectModel> CastEmitterObjectModelByAttachLocation = new Dictionary<SpellEmitterModelAttachLocationType, ObjectModel>();
        public Dictionary<SpellEmitterModelAttachLocationType, ObjectModel> ImpactEmitterObjectModelByAttachLocation = new Dictionary<SpellEmitterModelAttachLocationType, ObjectModel>();

        private static void LoadEQSpellVisualEffectsData()
        {
            Logger.WriteDebug("Loading EQ Spell Visual Effects Data...");
            string spellsEFFFileFullPath = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "clientdata", "spells.eff");
            if (Path.Exists(spellsEFFFileFullPath) == false)
            {
                Logger.WriteError("Could not find spells.eff data that should be at ", spellsEFFFileFullPath, ", did you not run the conditioner step?");
                return;
            }
            string sourceTextureFolder = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "equipment", "Textures");
            EQSpellsEFF = new EQSpellsEFF();
            EQSpellsEFF.LoadFromDisk(spellsEFFFileFullPath, sourceTextureFolder);
            Logger.WriteDebug("Loading EQ Spell Visual Effects Data complete...");
        }

        public static List<SpellVisual> GetAllSpellVisuals()
        {
            lock (SpellVisualLock)
            {
                if (SpellVisualsByType.Count == 0)
                    GenerateWOWSpellVisualData();
                List<SpellVisual> combinedSpellVisuals = new List<SpellVisual>();
                combinedSpellVisuals.AddRange(SpellVisualsByType[SpellVisualType.Beneficial]);
                combinedSpellVisuals.AddRange(SpellVisualsByType[SpellVisualType.Detrimental]);
                combinedSpellVisuals.AddRange(SpellVisualsByType[SpellVisualType.BardTick]);
                return combinedSpellVisuals;
            }
        }

        public static List<ObjectModel> GetAllEmitterObjectModels()
        {
            lock (SpellVisualLock)
                return AllEmitterObjectModels;
        }

        public static SpellVisual GetSpellVisual(int effectID, SpellVisualType spellVisualType)
        {
            lock (SpellVisualLock)
            {
                if (SpellVisualsByType.Count == 0)
                    GenerateWOWSpellVisualData();
                return SpellVisualsByType[spellVisualType][effectID];
            }
        }

        public static void GenerateWOWSpellVisualData()
        {
            Logger.WriteDebug("Generating wow spell visual data started...");
            SpellVisualsByType.Clear();
            SpellVisualsByType.Add(SpellVisualType.Beneficial, new List<SpellVisual>());
            SpellVisualsByType.Add(SpellVisualType.Detrimental, new List<SpellVisual>());
            SpellVisualsByType.Add(SpellVisualType.BardTick, new List<SpellVisual>());

            lock (SpellVisualLock)
            {
                // Load the EQ spell data
                if (EQSpellsEFF != null)
                {
                    Logger.WriteError("Attempted to generate spell visual data twice.");
                    return;
                }
                LoadEQSpellVisualEffectsData();
                if (EQSpellsEFF == null)
                    return;

                // Create spell visual data for each of the EQ spell data
                for (int i = 0; i < EQSpellsEFF.SpellEffects.Count; i++)
                {
                    // Two copies for good vs bad
                    for (int j = 0; j < 3; j++)
                    {
                        SpellVisualType spellVisualType = (SpellVisualType)j;
                        EQSpellsEFF.EQSpellEffect spellEffect = EQSpellsEFF.SpellEffects[i];
                        SpellVisual spellVisual = new SpellVisual();
                        spellVisual.EQVisualEffectIndex = i;
                        spellVisual.SpellVisualDBCID = SpellVisualDBC.GenerateID();
                        ConvertStageVisualData(ref spellVisual, spellEffect, SpellVisualStageType.Precast, spellVisualType);
                        ConvertStageVisualData(ref spellVisual, spellEffect, SpellVisualStageType.Cast, spellVisualType);
                        ConvertStageVisualData(ref spellVisual, spellEffect, SpellVisualStageType.Impact, spellVisualType);
                        SpellVisualsByType[spellVisualType].Add(spellVisual);
                    }
                }
            }
            Logger.WriteDebug("Generating wow spell visual data complete.");
        }

        private static void ConvertStageVisualData(ref SpellVisual spellVisual, EQSpellsEFF.EQSpellEffect spellEffect, SpellVisualStageType stageType, SpellVisualType spellVisualType)
        {
            // ID
            spellVisual.SpellVisualKitDBCIDsInStage[(int)stageType] = SpellVisualKitDBC.GenerateID();

            // Stage-specific logic
            switch (stageType)
            {
                case SpellVisualStageType.Precast:
                    {
                        switch (spellVisualType)
                        {
                            case SpellVisualType.Beneficial: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.ReadySpellOmni; break;
                            case SpellVisualType.Detrimental: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.ReadySpellDirected; break;
                            default: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.None; break;
                        }
                        spellVisual.SoundEntryDBCIDInStage[(int)stageType] = ProcessSoundAndReturnDBCID(spellEffect.SourceSoundID, stageType);
                    } break;
                case SpellVisualStageType.Cast:
                    {
                        switch (spellVisualType)
                        {
                            case SpellVisualType.Beneficial: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.SpellCastOmni; break;
                            case SpellVisualType.Detrimental: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.SpellCastDirected; break;
                            default: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.None; break;
                        }
                    } break;
                case SpellVisualStageType.Impact:
                    {
                        switch (spellVisualType)
                        {
                            default: spellVisual.AnimationTypeInStage[(int)stageType] = AnimationType.None; break;
                        }
                        spellVisual.SoundEntryDBCIDInStage[(int)stageType] = ProcessSoundAndReturnDBCID(spellEffect.TargetSoundID, stageType);
                    } break;
                default: Logger.WriteError("Unhandled stagetype in ConvertStageVisualData"); break;
            }

            // Model
            GenerateEmitterModels(ref spellVisual, spellEffect, stageType, spellVisualType);

            // If there is a projectile, create that
            //if (spellEffect.)
        }

        private static int ProcessSoundAndReturnDBCID(int effectSoundID, SpellVisualStageType stageType)
        {
            if (effectSoundID == -1)
                return 0;
            string soundFileNameNoExt = GetSoundFileNameNoExtFromSoundID(effectSoundID);
            if (soundFileNameNoExt != string.Empty)
            {
                if (SoundsByFileNameNoExt.ContainsKey(soundFileNameNoExt) == false)
                {
                    string name = string.Concat("EQ Spell ", soundFileNameNoExt);
                    Sound sound = new Sound(name, soundFileNameNoExt, SoundType.Spell, 8, 45, false);
                    SoundsByFileNameNoExt.Add(soundFileNameNoExt, sound);
                }
                return SoundsByFileNameNoExt[soundFileNameNoExt].DBCID;
            }
            else
                return 0;
        }

        private ObjectModel? GetObjectModelInStageAtAttachLocation(SpellVisualStageType stage, SpellEmitterModelAttachLocationType attachLocation)
        {
            switch (stage)
            {
                case SpellVisualStageType.Precast:
                    {
                        if (PrecastEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == true)
                            return PrecastEmitterObjectModelByAttachLocation[attachLocation];
                        else
                            return null;
                    }
                case SpellVisualStageType.Cast:
                    {
                        if (CastEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == true)
                            return CastEmitterObjectModelByAttachLocation[attachLocation];
                        else
                            return null;
                    }
                case SpellVisualStageType.Impact:
                    {
                        if (ImpactEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == true)
                            return ImpactEmitterObjectModelByAttachLocation[attachLocation];
                        else
                            return null;
                    }
                default: return null;
            }
        }

        private void AddObjectToStageAtAttachLocation(SpellVisualStageType stage, SpellEmitterModelAttachLocationType attachLocation, ObjectModel emitterObject)
        {
            switch (stage)
            {
                case SpellVisualStageType.Precast:
                    {
                        if (PrecastEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == false)
                            PrecastEmitterObjectModelByAttachLocation.Add(attachLocation, emitterObject);
                        else
                            Logger.WriteError("Attempted to add an emitter object to SpellVisual Precast when one existed already.");
                    } break;
                case SpellVisualStageType.Cast:
                    {
                        if (CastEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == false)
                            CastEmitterObjectModelByAttachLocation.Add(attachLocation, emitterObject);
                        else
                            Logger.WriteError("Attempted to add an emitter object to SpellVisual Cast when one existed already.");
                    } break;
                case SpellVisualStageType.Impact:
                    {
                        if (ImpactEmitterObjectModelByAttachLocation.ContainsKey(attachLocation) == false)
                            ImpactEmitterObjectModelByAttachLocation.Add(attachLocation, emitterObject);
                        else
                            Logger.WriteError("Attempted to add an emitter object to SpellVisual Impact when one existed already.");
                    } break;
                default: break;
            }
        }

        private static void GenerateEmitterModels(ref SpellVisual spellVisual, EQSpellsEFF.EQSpellEffect spellEffect, SpellVisualStageType stageType,
            SpellVisualType spellVisualType)
        {
            // There are no 'cast' models
            if (stageType == SpellVisualStageType.Cast)
                return;

            // Sprite List Effects are added as model data, so create those first
            List<EQSpellsEFF.EFFSpellSpriteListEffect> spriteListEffects = new List<EQSpellsEFF.EFFSpellSpriteListEffect>();
            if (stageType == SpellVisualStageType.Precast)
                spriteListEffects = spellEffect.CasterUnitSpriteListEffects;
            else if (stageType == SpellVisualStageType.Impact)
                spriteListEffects = spellEffect.TargetUnitSpriteListEffects;
            if (spriteListEffects.Count != 0)
            {
                string objectName = string.Concat("eqemitter_", spellVisual.SpellVisualDBCID.ToString(), "_", stageType.ToString(), "_Chest");
                ObjectModelProperties objectProperties = new ObjectModelProperties();
                objectProperties.SpellVisualEffectNameDBCID = SpellVisualEffectNameDBC.GenerateID();
                objectProperties.SpellVisualType = spellVisualType;
                ObjectModel objectModel = new ObjectModel(objectName, objectProperties, ObjectModelType.ParticleEmitter, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                objectModel.Load(new List<Material>(), new MeshData(), new List<Vector3>(), new List<TriangleFace>(), spriteListEffects);
                spellVisual.AddObjectToStageAtAttachLocation(stageType, SpellEmitterModelAttachLocationType.Chest, objectModel);
                AllEmitterObjectModels.Add(objectModel);
            }

            // Generate the object particle emitters for unit targets
            List<ObjectModelParticleEmitter> modelParticleEmitters = new List<ObjectModelParticleEmitter>();
            foreach (var emitter in spellEffect.Emitters)
            {
                // Only process stage-aligned unit emitter targets
                if (stageType == SpellVisualStageType.Precast && emitter.TargetType != EQSpellEffectTargetType.Caster)
                    continue;
                if (stageType == SpellVisualStageType.Impact && emitter.TargetType != EQSpellEffectTargetType.Target)
                    continue;

                ObjectModelParticleEmitter particleEmitter = new ObjectModelParticleEmitter();
                particleEmitter.Load(emitter, stageType, spellVisualType);
                modelParticleEmitters.Add(particleEmitter);

                // It seems that emitters with type 5 (disc at player center) ALSO create emitters on hands and on the ground
                if (emitter.EmissionTypeID == 5)
                {
                    // While potentially accurate, it looks odd in WoW to have this happen
                    //ObjectModelParticleEmitter particleEmitterHands = new ObjectModelParticleEmitter();
                    //particleEmitterHands.Load(effSectionData, i, SpellVisualEmitterSpawnPatternType.FromHands);
                    //emitters.Add(particleEmitterHands);

                    ObjectModelParticleEmitter particleEmitterGround = new ObjectModelParticleEmitter();
                    particleEmitterGround.Load(emitter, stageType, spellVisualType, SpellVisualEmitterSpawnPatternType.DiscOnGround);
                    modelParticleEmitters.Add(particleEmitterGround);
                }
            }

            // Add the emitters to new or existing emitters
            foreach (ObjectModelParticleEmitter emitter in modelParticleEmitters)
            {
                ObjectModel? existingModel = spellVisual.GetObjectModelInStageAtAttachLocation(stageType, emitter.EmissionLocation);
                if (existingModel != null)
                    existingModel.Properties.SingleSpriteSpellParticleEmitters.Add(emitter);
                else
                {
                    // Make new
                    string objectName = string.Concat("eqemitter_", spellVisual.SpellVisualDBCID.ToString(), "_", stageType.ToString(), "_", emitter.EmissionLocation.ToString());
                    ObjectModelProperties objectProperties = new ObjectModelProperties();
                    objectProperties.SpellVisualEffectNameDBCID = SpellVisualEffectNameDBC.GenerateID();
                    objectProperties.SingleSpriteSpellParticleEmitters.Add(emitter);
                    objectProperties.SpellVisualEffectStageType = stageType;
                    objectProperties.SpellVisualType = spellVisualType;
                    if (emitter.EmissionPattern == SpellVisualEmitterSpawnPatternType.FromHands)
                        objectProperties.SpellEmitterSpraysFromHands = true;
                    ObjectModel objectModel = new ObjectModel(objectName, objectProperties, ObjectModelType.ParticleEmitter, Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE);
                    objectModel.Load(new List<Material>(), new MeshData(), new List<Vector3>(), new List<TriangleFace>());
                    spellVisual.AddObjectToStageAtAttachLocation(stageType, emitter.EmissionLocation, objectModel);
                    AllEmitterObjectModels.Add(objectModel);
                    existingModel = objectModel;
                }

                // Also add the textures, manually (if needed)
                if (existingModel != null)
                    emitter.TextureID = AddTextureToModelAndReturnID(existingModel, emitter.SpriteSheetFileNameNoExt);
            }
        }

        private static int AddTextureToModelAndReturnID(ObjectModel? existingModel, string spriteSheetFileNameNoExt)
        {
            if (existingModel == null)
                return -1;

            int modelTextureID = -1;
            for (int i = 0; i < existingModel.ModelTextures.Count; i++)
            {
                if (existingModel.ModelTextures[i].TextureName == spriteSheetFileNameNoExt)
                {
                    modelTextureID = i;
                    break;
                }
            }
            if (modelTextureID == -1)
            {
                ObjectModelTexture newModelTexture = new ObjectModelTexture();
                newModelTexture.TextureName = spriteSheetFileNameNoExt;
                existingModel.ModelTextures.Add(newModelTexture);
                modelTextureID = existingModel.ModelTextures.Count - 1;
            }
            return modelTextureID;
        }

        private static string GetSoundFileNameNoExtFromSoundID(int soundID)
        {
            switch (soundID)
            {
                case 103: return "spell_1"; // TODO: Confirm
                case 104: return "spell_2";
                case 105: return "spell_3";
                case 106: return "spell_4";
                case 107: return "spell_5";
                case 108: return "spelcast";
                case 109: return "spelgdht";
                case 110: return "spelhit1"; // TODO: Confirm
                case 111: return "spelhit2";
                case 112: return "spelhit3"; // TODO: Confirm
                case 113: return "spelhit4";
                default: return string.Empty;
            }
        }

        public int GetVisualIDForAttachLocationStage(SpellEmitterModelAttachLocationType attachLocation, SpellVisualStageType stage)
        {
            ObjectModel? objectModel = GetObjectModelInStageAtAttachLocation(stage, attachLocation);
            if (objectModel == null)
                return 0;
            return objectModel.Properties.SpellVisualEffectNameDBCID;
        }
    }
}
