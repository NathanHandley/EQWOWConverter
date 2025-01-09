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
using EQWOWConverter.ObjectModels.Properties;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureRace
    {
        private static List<CreatureRace> CreatureRaces = new List<CreatureRace>();

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

        public static Dictionary<string, Sound> SoundsBySoundName = new Dictionary<string, Sound>();
        public static Dictionary<string, int> FootstepIDBySoundName = new Dictionary<string, int>();
        public static Dictionary<int, int> FootstepIDBySoundID = new Dictionary<int, int>();
        private static int CUR_CREATURE_FOOTSTEP_ID = Configuration.CONFIG_DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START;

        public static void GenerateAllSounds()
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

        public static int GetSoundIDForSound(string soundName)
        {
            if (SoundsBySoundName.ContainsKey(soundName))
                return SoundsBySoundName[soundName].DBCID;
            else
                return 0;

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
            if (CreatureRaces.Count == 0)
                PopulateCreatureRaceList();
            return CreatureRaces;
        }

        public static CreatureRace GetCreatureRace(int raceID, CreatureGenderType gender, int variantID)
        {
            foreach(CreatureRace race in GetAllCreatureRaces())
            {
                if (race.ID == raceID && race.Gender == gender && race.VariantID == variantID)
                    return race;
            }

            // By default, return the first result
            Logger.WriteError("Could not find a race as id = '" + raceID.ToString() + "', gender = '" + gender.ToString() + "', variant = '" + variantID.ToString() + "'.  Returning human male.");
            return CreatureRaces[0];
        }

        private static void PopulateCreatureRaceList()
        {
            CreatureRaces.Clear();

            // Load in base race data
            string raceDataFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureRaces.csv");
            Logger.WriteDetail("Populating CreatureRace list via file '" + raceDataFileName + "'");
            string inputData = File.ReadAllText(raceDataFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureRace list via file '" + raceDataFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach(string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split("|");
                CreatureRace newCreatureRace = new CreatureRace();
                newCreatureRace.ID = int.Parse(rowBlocks[0]);
                newCreatureRace.Gender = (CreatureGenderType)int.Parse(rowBlocks[1]);
                newCreatureRace.VariantID = int.Parse(rowBlocks[2]);
                newCreatureRace.Name = rowBlocks[3].Trim();
                newCreatureRace.SkeletonName = rowBlocks[4].Trim();
                newCreatureRace.Skeleton2Name = rowBlocks[5].Trim();
                newCreatureRace.Lift = float.Parse(rowBlocks[6]);
                newCreatureRace.ModelScale = float.Parse(rowBlocks[7]);
                newCreatureRace.Height = float.Parse(rowBlocks[8]);
                newCreatureRace.SpawnSizeMod = float.Parse(rowBlocks[9]);
                newCreatureRace.BoundaryRadius = float.Parse(rowBlocks[10]);
                newCreatureRace.BoundaryHeight = float.Parse(rowBlocks[11]);
                newCreatureRace.SoundLoopName = rowBlocks[12];
                newCreatureRace.SoundIdle1Name = rowBlocks[13];
                newCreatureRace.SoundIdle2Name = rowBlocks[14];
                newCreatureRace.SoundJumpName = rowBlocks[15];
                newCreatureRace.SoundHit1Name = rowBlocks[16];
                newCreatureRace.SoundHit2Name = rowBlocks[17];
                newCreatureRace.SoundHit3Name = rowBlocks[18];
                newCreatureRace.SoundHit4Name = rowBlocks[19];
                newCreatureRace.SoundGasp1Name = rowBlocks[20];
                newCreatureRace.SoundGasp2Name = rowBlocks[21];
                newCreatureRace.SoundDeathName = rowBlocks[22];
                newCreatureRace.SoundDrownName = rowBlocks[23];
                newCreatureRace.SoundWalkingName = rowBlocks[24];
                newCreatureRace.SoundRunningName = rowBlocks[25];
                newCreatureRace.SoundAttackName = rowBlocks[26];
                newCreatureRace.SoundSpellAttackName = rowBlocks[27];
                newCreatureRace.SoundTechnicalAttackName = rowBlocks[28];
                float cameraPositionModX = float.Parse(rowBlocks[29]);
                float cameraPositionModY = float.Parse(rowBlocks[30]);
                float cameraPositionModZ = float.Parse(rowBlocks[31]);
                newCreatureRace.CameraPositionMod = new Vector3(cameraPositionModX, cameraPositionModY, cameraPositionModZ);
                float cameraTargetPositionModX = float.Parse(rowBlocks[32]);
                float cameraTargetPositionModY = float.Parse(rowBlocks[33]);
                float cameraTargetPositionModZ = float.Parse(rowBlocks[34]);
                newCreatureRace.CameraTargetPositionMod = new Vector3(cameraTargetPositionModX, cameraTargetPositionModY, cameraTargetPositionModZ);

                CreatureRaces.Add(newCreatureRace);
            }
        }
    }
}
