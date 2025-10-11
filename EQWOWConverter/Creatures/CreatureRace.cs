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

namespace EQWOWConverter.Creatures
{
    internal class CreatureRace
    {
        private static List<CreatureRace> CreatureRaces = new List<CreatureRace>();
        private static readonly object CreatureLock = new object();

        public int ID;
        public CreatureGenderType Gender = CreatureGenderType.Neutral;
        public int VariantID;
        public string Name = string.Empty;
        public string SkeletonName = string.Empty;
        public string Skeleton2Name = string.Empty;
        public float BoundaryRadius;
        public float BoundaryHeight;
        public float Lift = 0;
        public float ModelScale = 1;
        public float Height = 1;
        public float SpawnSizeMod = 0.2f;
        public bool CanHoldWeapons = false;
        public string SoundLoopName = string.Empty;
        public string SoundIdle1Name = string.Empty;
        public string SoundIdle2Name = string.Empty;
        public string SoundJumpName = string.Empty;
        public string SoundHit1Name = string.Empty;
        public string SoundHit2Name = string.Empty;
        public string SoundHit3Name = string.Empty;
        public string SoundHit4Name = string.Empty;
        public string SoundGasp1Name = string.Empty;
        public string SoundGasp2Name = string.Empty;
        public string SoundDeathName = string.Empty;
        public string SoundDrownName = string.Empty;
        public string SoundWalkingName = string.Empty;
        public string SoundRunningName = string.Empty;
        public string SoundAttackName = string.Empty;
        public string SoundSpellAttackName = string.Empty;
        public string SoundTechnicalAttackName = string.Empty;
        public Vector3 CameraPositionMod = new Vector3();
        public Vector3 CameraTargetPositionMod = new Vector3();
        public float GeoboxInradius = 0;
        public float NameplateAddedHeight = 0;
        public int WOWCreatureFamily = 0;
        public int WOWCreatureType = 0;

        public static Dictionary<string, Sound> SoundsBySoundName = new Dictionary<string, Sound>();
        public static Dictionary<string, int> FootstepIDBySoundName = new Dictionary<string, int>();
        public static Dictionary<int, int> FootstepIDBySoundID = new Dictionary<int, int>();
        private static int CUR_CREATURE_FOOTSTEP_ID = Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;

        public static void GenerateAllSounds()
        {
            lock (CreatureLock)
            {
                if (CreatureRaces.Count == 0)
                {
                    PopulateCreatureRaceList();

                    // Set the default 'blank' sound for footstep
                    FootstepIDBySoundName.Add("null24.wav", 0);
                }

                foreach (CreatureRace creatureRace in CreatureRaces)
                {
                    GenerateSoundIfUnique(creatureRace.SoundLoopName);
                    GenerateSoundIfUnique(creatureRace.SoundIdle1Name);
                    GenerateSoundIfUnique(creatureRace.SoundIdle2Name);
                    GenerateSoundIfUnique(creatureRace.SoundJumpName);
                    GenerateSoundIfUnique(creatureRace.SoundHit1Name);
                    GenerateSoundIfUnique(creatureRace.SoundHit2Name);
                    GenerateSoundIfUnique(creatureRace.SoundHit3Name);
                    GenerateSoundIfUnique(creatureRace.SoundHit4Name);
                    GenerateSoundIfUnique(creatureRace.SoundGasp1Name);
                    GenerateSoundIfUnique(creatureRace.SoundGasp2Name);
                    GenerateSoundIfUnique(creatureRace.SoundDeathName);
                    GenerateSoundIfUnique(creatureRace.SoundDrownName);
                    GenerateSoundIfUnique(creatureRace.SoundWalkingName);
                    GenerateSoundIfUnique(creatureRace.SoundAttackName);
                    GenerateSoundIfUnique(creatureRace.SoundSpellAttackName);
                    GenerateSoundIfUnique(creatureRace.SoundTechnicalAttackName);
                    GenerateSoundIfUnique(creatureRace.SoundRunningName);

                    if (FootstepIDBySoundName.ContainsKey(creatureRace.SoundWalkingName) == false)
                    {
                        FootstepIDBySoundName.Add(creatureRace.SoundWalkingName, CUR_CREATURE_FOOTSTEP_ID);
                        FootstepIDBySoundID.Add(GetSoundIDForSound(creatureRace.SoundWalkingName), CUR_CREATURE_FOOTSTEP_ID);
                        CUR_CREATURE_FOOTSTEP_ID++;
                    }
                }
            }
        }

        public static int GetSoundIDForSound(string soundName)
        {
            lock (CreatureLock)
            {
                if (SoundsBySoundName.ContainsKey(soundName))
                    return SoundsBySoundName[soundName].DBCID;
                else
                    return 0;
            }
        }

        private static void GenerateSoundIfUnique(string soundName)
        {
            if (SoundsBySoundName.ContainsKey(soundName))
                return;
            if (soundName == "null24.wav")
                return;
            Sound newSound = new Sound(soundName, soundName, SoundType.NPCCombat, 8, 20, false);
            newSound.NoOverlap = true;
            SoundsBySoundName[soundName] = newSound;
        }

        public static List<CreatureRace> GetAllCreatureRaces()
        {
            lock (CreatureLock)
            {
                if (CreatureRaces.Count == 0)
                    PopulateCreatureRaceList();
                return CreatureRaces;
            }
        }

        public static CreatureRace? GetRaceForRaceGenderVariant(int raceID, CreatureGenderType gender, int variant, bool fallbackToMaleOrNeutral = false)
        {
            lock (CreatureLock)
            {
                if (CreatureRaces.Count == 0)
                    PopulateCreatureRaceList();
                foreach (CreatureRace race in CreatureRaces)
                    if (race.ID == raceID && race.Gender == gender && race.VariantID == variant)
                        return race;
                if (fallbackToMaleOrNeutral == true)
                {
                    foreach (CreatureRace race in CreatureRaces)
                        if (race.ID == raceID && race.Gender == CreatureGenderType.Neutral && race.VariantID == variant)
                            return race;
                    foreach (CreatureRace race in CreatureRaces)
                        if (race.ID == raceID && race.Gender == CreatureGenderType.Male && race.VariantID == variant)
                            return race;
                }
                Logger.WriteError(string.Concat("Could not find a creature race with ID ", raceID, ", gender ", gender, ", varient ", variant));
                return null;
            }
        }

        private static void PopulateCreatureRaceList()
        {
            CreatureRaces.Clear();

            // Load in base race data
            string raceDataFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureRaces.csv");
            Logger.WriteDebug("Populating CreatureRace list via file '" + raceDataFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(raceDataFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Load the row
                CreatureRace newCreatureRace = new CreatureRace();
                newCreatureRace.ID = int.Parse(columns["RaceID"]);
                newCreatureRace.Gender = (CreatureGenderType)int.Parse(columns["Gender"]);
                newCreatureRace.VariantID = int.Parse(columns["Variant"]);
                newCreatureRace.Name = columns["RaceName"].Trim();
                newCreatureRace.SkeletonName = columns["Anim"].Trim();
                newCreatureRace.Skeleton2Name = columns["Anim2"].Trim();
                newCreatureRace.Lift = float.Parse(columns["Lift"]);
                newCreatureRace.ModelScale = float.Parse(columns["ModelScale"]);
                newCreatureRace.Height = float.Parse(columns["Height"]);
                newCreatureRace.SpawnSizeMod = float.Parse(columns["SpawnSizeMod"]);
                newCreatureRace.BoundaryRadius = float.Parse(columns["BoundRadius"]);
                newCreatureRace.BoundaryHeight = float.Parse(columns["BoundHeight"]);
                newCreatureRace.CanHoldWeapons = columns["CanHoldWeapons"].Trim() == "1";
                newCreatureRace.SoundLoopName = columns["SndLoop"];
                newCreatureRace.SoundIdle1Name = columns["SndIdle1"];
                newCreatureRace.SoundIdle2Name = columns["SndIdle2"];
                newCreatureRace.SoundJumpName = columns["SndJump"];
                newCreatureRace.SoundHit1Name = columns["SndGetHit1"];
                newCreatureRace.SoundHit2Name = columns["SndGetHit2"];
                newCreatureRace.SoundHit3Name = columns["SndGetHit3"];
                newCreatureRace.SoundHit4Name = columns["SndGetHit4"];
                newCreatureRace.SoundGasp1Name = columns["SndGasp1"];
                newCreatureRace.SoundGasp2Name = columns["SndGasp2"];
                newCreatureRace.SoundDeathName = columns["SndDeath"];
                newCreatureRace.SoundDrownName = columns["SndDrown"];
                newCreatureRace.SoundWalkingName = columns["SndWalking"];
                newCreatureRace.SoundRunningName = columns["SndRunning"];
                newCreatureRace.SoundAttackName = columns["SndAttack"];
                newCreatureRace.SoundSpellAttackName = columns["SndSAttack"];
                newCreatureRace.SoundTechnicalAttackName = columns["SndTAttack"];
                float cameraPositionModX = float.Parse(columns["CamPosModX"]) * Configuration.GENERATE_CREATURE_SCALE;
                float cameraPositionModY = float.Parse(columns["CamPosModY"]) * Configuration.GENERATE_CREATURE_SCALE;
                float cameraPositionModZ = float.Parse(columns["CamPosModZ"]) * Configuration.GENERATE_CREATURE_SCALE;
                newCreatureRace.CameraPositionMod = new Vector3(cameraPositionModX, cameraPositionModY, cameraPositionModZ);
                float cameraTargetPositionModX = float.Parse(columns["CamTarModX"]) * Configuration.GENERATE_CREATURE_SCALE;
                float cameraTargetPositionModY = float.Parse(columns["CamTarModY"]) * Configuration.GENERATE_CREATURE_SCALE;
                float cameraTargetPositionModZ = float.Parse(columns["CamTarModZ"]) * Configuration.GENERATE_CREATURE_SCALE;
                newCreatureRace.CameraTargetPositionMod = new Vector3(cameraTargetPositionModX, cameraTargetPositionModY, cameraTargetPositionModZ);
                newCreatureRace.GeoboxInradius = float.Parse(columns["GeoboxInradius"]) * Configuration.GENERATE_CREATURE_SCALE;
                newCreatureRace.NameplateAddedHeight = float.Parse(columns["NamePlateHeightAdd"]) * Configuration.GENERATE_CREATURE_SCALE;
                newCreatureRace.WOWCreatureFamily = int.Parse(columns["WOWFamily"]);
                newCreatureRace.WOWCreatureType = int.Parse(columns["WOWType"]);
                CreatureRaces.Add(newCreatureRace);
            }
        }
    }
}
