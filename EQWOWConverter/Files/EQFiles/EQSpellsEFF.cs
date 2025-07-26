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

namespace EQWOWConverter.EQFiles
{
    // Much of this structure data was from research from "Stolistic" on the Project Latern discord (posted 12/30/2022 in #lantern-general)
    internal class EQSpellsEFF
    {
        internal struct EFFSpellEmitter
        {
            public EQSpellEffectTargetType TargetType;
            public int VisualEffectIndex;
            public string SpriteName = string.Empty;
            public int LocationID;
            public int EmissionTypeID;
            public ColorRGBA Color = new ColorRGBA();
            public float Gravity;
            public float SpawnX;
            public float SpawnY;
            public float SpawnZ;
            public float Radius;
            public float Angle;
            public int ParticleLifespan; // Is this actually emitter lifespan?
            public float Velocity;
            public int SpawnRate;
            public float Scale;

            public EFFSpellEmitter(EQSpellEffectTargetType targetType, int visualEffectIndex, string spriteName, int locationID, int emissionTypeID, ColorRGBA color, float gravity, float spawnX, 
                float spawnY, float spawnZ, float radius, float angle, int particleLifespan, float velocity, int spawnRate, float scale)
            {
                TargetType = targetType;
                VisualEffectIndex = visualEffectIndex;
                SpriteName = spriteName;
                LocationID = locationID;
                EmissionTypeID = emissionTypeID;
                Color = color;
                Gravity = gravity;
                SpawnX = spawnX;
                SpawnY = spawnY;
                SpawnZ = spawnZ;
                Radius = radius;
                Angle = angle;
                ParticleLifespan = particleLifespan;
                Velocity = velocity;
                SpawnRate = spawnRate;
                Scale = scale;
            }
        }

        internal struct EFFSpellSpriteListParticle
        {
            public string SpriteName = string.Empty;
            public short CircularShift;
            public short VerticalForce;
            public float Radius;
            public short Movement;
            public float Scale;

            public EFFSpellSpriteListParticle(string spriteName, short circularShift, short verticalForce, float radius, short movement, float scale)
            {
                SpriteName = spriteName;
                CircularShift = circularShift;
                VerticalForce = verticalForce;
                Radius = radius;
                Movement = movement;
                Scale = scale;
            }
        }

        internal struct EFFSpellSpriteListEffect
        {
            public EQSpellEffectTargetType TargetType;
            public EQSpellListEffectType EffectType;
            public int VisualEffectIndex = 0;
            public List<EFFSpellSpriteListParticle> Particles= new List<EFFSpellSpriteListParticle>();
            public Dictionary<string, List<string>> SpriteChainsBySpriteRoot = new Dictionary<string, List<string>>();

            public EFFSpellSpriteListEffect(EQSpellEffectTargetType targetType, EQSpellListEffectType effectType, int visualEffectIndex)
            {
                TargetType = targetType;
                EffectType = effectType;
                VisualEffectIndex = visualEffectIndex;
            }
        }

        internal class EFFSourceSectionData
        {
            public int VisualEffectIndex = 0;
            public string[] SpriteNames = new string[3];
            public string TypeString = string.Empty;
            public int[] LocationIDs = new int[3]; // -1 = None(?), 0 = Body Center, 1 = Head, 2 = Right Hand, 3 = Left Hand, 4 = Right Foot, 5 = Left Foot, 6+ = Also center of body
            public int[] EmissionTypeIDs = new int[3]; // -1 = None, 0 = Hands, 1 = reverse sphere?, 2 = Sphere around unit, 3 = Disc on the ground, 4 = Column from the ground, 5 = Disc at the player center.  Note: 5 also gives a 2 and a 3 emitter.
            public string[] SpriteListNames = new string[12]; // Up to 12 sprites used for animating particles. Loop through until the end and start at begining. Particles oriented in a circle, with each 30 degrees from the next for a total of 360 degrees.
            public int SpriteListEffect; // 1 = Projectile, 2 = Disc pulsating outward and then inward
            public int SoundID = 0; // Values look to be between 103-113. -1 for no sound
            public ColorRGBA[] EmitterColors = new ColorRGBA[3]; // BGRX formation, where X (otherwise alpha) is unused
            public float[] EmitterGravities = new float[3];
            public float[] EmitterSpawnXs = new float[3]; // #1 can be 1 or -1, #2 can be 1, #3 can be 1 or -1 (?)
            public float[] EmitterSpawnYs = new float[3]; // #1 can be 1 or -1, #2 can be -1, #3 can be 1 (?)
            public float[] EmitterSpawnZs = new float[3]; // #1 and #2 can be 1 or -1 or -2, #3 can be 1 or -1
            public float[] EmitterSpawnRadii = new float[3]; // Radius of the particles
            public float[] EmitterSpawnAngles = new float[3]; // Angle of the particles (unsure on orientation)
            public int[] EmitterSpawnLifespans = new int[3]; // Lifespawn of the particles in milliseconds
            public float[] EmitterSpawnVelocities = new float[3]; // Velocity of the particles
            public int[] EmitterSpawnRates = new int[3]; // Spawn rates of the particles (in what?)
            public float[] EmitterSpawnScale = new float[3]; // Scale of the particles
            public int[] UnknownData = new int[9];  // 9 unknown values that is always zero
            public float[] SpriteListUnknown = new float[12]; // Unsure.  Can be 10, 15, 20, or 30
            public short[] SpriteListCircularShifts = new short[12]; // How many rotational shifts particles will travel during animation.  15 shifts is one full rotation counterclockwise.  Negative goes clockwise.  Ignored if movement is stationary.
            public short[] SpriteListVerticalForces = new short[12]; // Verticle force applied to the particles.  -3 and +3 are common.  3 = Upper Top, 2 = Upper Middle, 1 = Upper Bottom, 0 = Player Center, -1 = Lower Top, -2 = Lower Middle, -3 = Lower Bottom
            public float[] SpriteListRadii = new float[12]; // Radius from playre's center for the particles
            public short[] SpriteListMovements = new short[12]; // Determines if the particle moves.  1 = Stationary, 2 = Moves
            public float[] SpriteListScales = new float[12]; // The scale of the particles

            public void LoadFromBytes(List<byte> bytes, ref int byteCursor)
            {
                // TODO: The 12 element ones are actually 4 blocks of 3, so break into parts
                for (int i = 0; i < 3; i++)
                {
                    SpriteNames[i] = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                    SpriteNames[i] = SpriteNames[i].Replace("GENE00", "GENE01"); // Fix invalid texture association
                    SpriteNames[i] = SpriteNames[i].Replace("_SPRITE", ""); // Remove "_SPRITE" that appears sometimes
                }
                TypeString = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                for (int i = 0; i < 3; i++)
                    LocationIDs[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmissionTypeIDs[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                {
                    SpriteListNames[i] = ByteTool.ReadStringFromBytes(bytes, ref byteCursor, 32);
                    SpriteListNames[i] = SpriteListNames[i].Replace("GENE00", "GENE01"); // Fix invalid texture association
                    SpriteListNames[i] = SpriteListNames[i].Replace("_SPRITE", ""); // Remove "_SPRITE" that appears sometimes
                }
                SpriteListEffect = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                SoundID = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterColors[i] = ByteTool.ReadColorBGRAFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterGravities[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                {
                    EmitterSpawnXs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                    EmitterSpawnYs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                    EmitterSpawnZs[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                }
                for (int i = 0; i < 3; i++)
                    EmitterSpawnRadii[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnAngles[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnLifespans[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnVelocities[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnRates[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 3; i++)
                    EmitterSpawnScale[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 9; i++)
                    UnknownData[i] = ByteTool.ReadInt32FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListUnknown[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListCircularShifts[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListVerticalForces[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListRadii[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListMovements[i] = ByteTool.ReadInt16FromBytes(bytes, ref byteCursor);
                for (int i = 0; i < 12; i++)
                    SpriteListScales[i] = ByteTool.ReadFloatFromBytes(bytes, ref byteCursor);
            }
        }

        internal class EQSpellEffect
        {
            public int Field01;
            public int Field02;
            public int VisualEffectIndex = 0;
            public EFFSourceSectionData[] RawSectionDatas = new EFFSourceSectionData[3]; // Always 3, sometimes blank
            public List<EFFSpellEmitter> Emitters = new List<EFFSpellEmitter>();
            public List<EFFSpellSpriteListEffect> CasterUnitSpriteListEffects = new List<EFFSpellSpriteListEffect>();
            public List<EFFSpellSpriteListEffect> TargetUnitSpriteListEffects = new List<EFFSpellSpriteListEffect>();
            public List<EFFSpellSpriteListEffect> ProjectileSpriteListEffects = new List<EFFSpellSpriteListEffect>();
            public int SourceSoundID = -1;
            public int TargetSoundID = -1;
        }        

        public List<EQSpellEffect> SpellEffects = new List<EQSpellEffect>();
        public HashSet<string> UniqueSpriteNames = new HashSet<string>();
        public Dictionary<string, List<ColorRGBA>> ColorTintsBySpriteNames = new Dictionary<string, List<ColorRGBA>>();
        public Dictionary<string, List<string>> SpriteChainsBySpriteRoot = new Dictionary<string, List<string>>();

        public bool LoadFromDisk(string fileFullPath, string sourceTextureFolder)
        {
            Logger.WriteDebug(" - Reading Spell Effects Data from '", fileFullPath, "'...");
            if (File.Exists(fileFullPath) == false)
            {
                Logger.WriteError("- Could not find Spell Effects file that should be at '", fileFullPath, "'");
                return false;
            }

            // Load in all the data
            List<byte> fileBytes = FileTool.GetFileBytes(fileFullPath);

            //  There are 255 spell effects in the file, but looks like only 52 (0-51) are populated with anything
            int byteCursor = 0;
            for (int i = 0; i < 52; i++)
            {
                EQSpellEffect curEffect = new EQSpellEffect();
                curEffect.Field01 = ByteTool.ReadInt32FromBytes(fileBytes, ref byteCursor);
                curEffect.Field02 = ByteTool.ReadInt32FromBytes(fileBytes, ref byteCursor);
                curEffect.VisualEffectIndex = i;

                // Always three sections
                for (int j = 0; j < 3; j++)
                {
                    curEffect.RawSectionDatas[j] = new EFFSourceSectionData();
                    curEffect.RawSectionDatas[j].VisualEffectIndex = i;
                    curEffect.RawSectionDatas[j].LoadFromBytes(fileBytes, ref byteCursor);
                }

                SpellEffects.Add(curEffect);
            }

            // Extract out all of the unique texture names
            foreach (EQSpellEffect spellEffect in SpellEffects)
            {
                foreach (EFFSourceSectionData sectionData in spellEffect.RawSectionDatas)
                {
                    foreach (string spriteName in sectionData.SpriteNames)
                    {
                        if (spriteName.Length > 0 && UniqueSpriteNames.Contains(spriteName) == false)
                            UniqueSpriteNames.Add(spriteName);
                    }
                    foreach (string spriteName in sectionData.SpriteListNames)
                    {
                        if (spriteName.Length > 0 && UniqueSpriteNames.Contains(spriteName) == false)
                            UniqueSpriteNames.Add(spriteName);
                    }
                }
            }

            // Create a list of sprite chains
            foreach (string rootSpriteName in UniqueSpriteNames)
            {
                // Skip if this file doesn't exist, since there wouldn't be subsequent anyway
                string spriteFileNameFullPath = Path.Combine(sourceTextureFolder, string.Concat(rootSpriteName, ".png"));
                if (File.Exists(spriteFileNameFullPath) == false)
                    continue;

                // Separate the number from the text
                string rootTextName = rootSpriteName.Substring(0, rootSpriteName.Length - 2);
                string rootNumberPartString = rootSpriteName.Substring(rootSpriteName.Length - 2);
                int rootNumber = int.Parse(rootNumberPartString);

                // Start a chain of sprites by going through in sequence until a sprite isn't found
                SpriteChainsBySpriteRoot.Add(rootSpriteName, new List<string>());
                SpriteChainsBySpriteRoot[rootSpriteName].Add(rootSpriteName);
                bool spriteFound = true;
                while (spriteFound)
                {
                    rootNumber++;
                    string nextSpriteName = string.Concat(rootTextName, rootNumber.ToString());
                    if (rootNumber < 10)
                        nextSpriteName = string.Concat(rootTextName, "0", rootNumber.ToString());
                    string nextSpriteFullFileName = Path.Combine(sourceTextureFolder, string.Concat(nextSpriteName, ".png"));
                    if (File.Exists(nextSpriteFullFileName) == false)
                    {
                        spriteFound = false;
                        continue;
                    }
                    SpriteChainsBySpriteRoot[rootSpriteName].Add(nextSpriteName);
                }
            }

            // Extract out any sprites that have a color tinting
            foreach (EQSpellEffect spellEffect in SpellEffects)
            {
                foreach (EFFSourceSectionData sectionData in spellEffect.RawSectionDatas)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        // Skip invalid sprite names
                        if (sectionData.SpriteNames[i].Trim().Length == 0)
                            continue;

                        // Skip Non-populated colors
                        if (sectionData.EmitterColors[i].R == 0 && sectionData.EmitterColors[i].G == 0 && sectionData.EmitterColors[i].B == 0)
                            continue;
                        if (sectionData.EmitterColors[i].R == 255 && sectionData.EmitterColors[i].G == 255 && sectionData.EmitterColors[i].B == 255)
                            continue;

                        // Skip if a matching already exists
                        bool colorExistsForTexture = false;
                        foreach (var colorTintListBySpriteName in ColorTintsBySpriteNames)
                        {
                            foreach (ColorRGBA colorTint in colorTintListBySpriteName.Value)
                            {
                                if (colorTint == sectionData.EmitterColors[i])
                                {
                                    colorExistsForTexture = true;
                                    break;
                                }
                            }
                            if (colorExistsForTexture == true)
                                continue;
                        }
                        if (colorExistsForTexture == true)
                            continue;

                        // Add it
                        if (ColorTintsBySpriteNames.ContainsKey(sectionData.SpriteNames[i]) == false)
                            ColorTintsBySpriteNames.Add(sectionData.SpriteNames[i], new List<ColorRGBA>());
                        ColorTintsBySpriteNames[sectionData.SpriteNames[i]].Add(sectionData.EmitterColors[i]);
                    }
                    foreach (string spriteName in sectionData.SpriteNames)
                    {
                        if (spriteName.Length > 0 && UniqueSpriteNames.Contains(spriteName) == false)
                            UniqueSpriteNames.Add(spriteName);
                    }
                }
            }

            // Convert all the section datas into appropriate emitter and spell list objects
            foreach (EQSpellEffect spellEffect in SpellEffects)
            {
                for (int sectionDataIter = 0; sectionDataIter < 3;  sectionDataIter++)
                {                     
                    EFFSourceSectionData sectionData = spellEffect.RawSectionDatas[sectionDataIter];

                    // Grab the sound IDs
                    if (sectionDataIter == 0)
                        spellEffect.SourceSoundID = sectionData.SoundID;
                    else if (sectionDataIter == 2)
                        spellEffect.TargetSoundID = sectionData.SoundID;

                    // Convert sprite list effect
                    if (sectionData.SpriteListEffect != -1)
                    {
                        // Skip invalid sprite list effects
                        if (sectionData.SpriteListEffect == -1)
                            continue;

                        // Determine effect type
                        EQSpellListEffectType effectType;
                        switch (sectionData.SpriteListEffect)
                        {
                            case 0: effectType = EQSpellListEffectType.Static; break;
                            case 1: effectType = EQSpellListEffectType.Projectile; break;
                            case 2: effectType = EQSpellListEffectType.Pulsating; break;
                            default: effectType = EQSpellListEffectType.Static; break;
                        }

                        // Determine the target type
                        EQSpellEffectTargetType targetType = EQSpellEffectTargetType.Caster;
                        if (effectType == EQSpellListEffectType.Projectile)
                            targetType = EQSpellEffectTargetType.Projectile;
                        else if (effectType == EQSpellListEffectType.Static)
                            targetType = EQSpellEffectTargetType.Caster;
                        else if (effectType == EQSpellListEffectType.Pulsating)
                        {
                            if (sectionDataIter == 0)
                                targetType = EQSpellEffectTargetType.Caster;
                            // TODO: There doesn't appear to be any in position "1" that fall into this
                            else if (sectionDataIter == 2)
                                targetType = EQSpellEffectTargetType.Target;
                        }

                        // Create the sprite list effect
                        EFFSpellSpriteListEffect newSpriteListEffect = new EFFSpellSpriteListEffect(targetType, effectType, spellEffect.VisualEffectIndex);
                        for (int i = 0; i < 12; i++)
                        {
                            if (sectionData.SpriteListNames[i].Trim().Length > 0)
                            {
                                EFFSpellSpriteListParticle newParticle = new EFFSpellSpriteListParticle(sectionData.SpriteListNames[i], sectionData.SpriteListCircularShifts[i],
                                    sectionData.SpriteListVerticalForces[i], sectionData.SpriteListRadii[i], sectionData.SpriteListMovements[i], sectionData.SpriteListScales[i]);
                                newSpriteListEffect.Particles.Add(newParticle);
                            }
                        }

                        // Associate any sprite chains (can't find projectile textures yet)
                        if (targetType != EQSpellEffectTargetType.Projectile)
                            foreach (EFFSpellSpriteListParticle particle in newSpriteListEffect.Particles)
                            {
                                if (newSpriteListEffect.SpriteChainsBySpriteRoot.ContainsKey(particle.SpriteName) == false)
                                    newSpriteListEffect.SpriteChainsBySpriteRoot.Add(particle.SpriteName, SpriteChainsBySpriteRoot[particle.SpriteName]);
                            }

                        // Add it
                        if (targetType == EQSpellEffectTargetType.Projectile)
                            spellEffect.ProjectileSpriteListEffects.Add(newSpriteListEffect);
                        else if (targetType == EQSpellEffectTargetType.Caster)
                            spellEffect.CasterUnitSpriteListEffects.Add(newSpriteListEffect);
                        else if (targetType == EQSpellEffectTargetType.Target)
                            spellEffect.TargetUnitSpriteListEffects.Add(newSpriteListEffect);
                    }

                    // Convert any emitters
                    for (int emitterIter = 0; emitterIter < 3; emitterIter++)
                    {
                        // Skip non emission types
                        if (sectionData.EmissionTypeIDs[emitterIter] == -1)
                            continue;

                        // Skip invalid sprite names
                        if (sectionData.SpriteNames[emitterIter].Length == 0)
                            continue;

                        // Determine target type
                        EQSpellEffectTargetType targetType = EQSpellEffectTargetType.Caster;
                        if (sectionDataIter == 0) // TODO: Confirm if we need to check for "Source" in the TypeID
                            targetType = EQSpellEffectTargetType.Caster;
                        else if (sectionDataIter == 1)
                            targetType = EQSpellEffectTargetType.Caster;
                        else if (sectionDataIter == 2) // TODO: Confirm if we need to check for "Target" in the TypeID
                            targetType = EQSpellEffectTargetType.Target;

                        // Create the emitter
                        EFFSpellEmitter newEmitter = new EFFSpellEmitter(targetType, spellEffect.VisualEffectIndex, sectionData.SpriteNames[emitterIter], sectionData.LocationIDs[emitterIter],
                            sectionData.EmissionTypeIDs[emitterIter], sectionData.EmitterColors[emitterIter], sectionData.EmitterGravities[emitterIter], sectionData.EmitterSpawnXs[emitterIter],
                            sectionData.EmitterSpawnYs[emitterIter], sectionData.EmitterSpawnZs[emitterIter], sectionData.EmitterSpawnRadii[emitterIter], sectionData.EmitterSpawnAngles[emitterIter],
                            sectionData.EmitterSpawnLifespans[emitterIter], sectionData.EmitterSpawnVelocities[emitterIter], sectionData.EmitterSpawnRates[emitterIter], sectionData.EmitterSpawnScale[emitterIter]);
                        spellEffect.Emitters.Add(newEmitter);
                    }
                }
            }

            Logger.WriteDebug(" - Done reading Spell Effects from '", fileFullPath, "'");
            return true;
        }
    }
}
