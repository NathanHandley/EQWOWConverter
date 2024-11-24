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
    internal class CreatureRaceSounds
    {
        private static List<CreatureRaceSounds> CreatureRaceSoundsList = new List<CreatureRaceSounds>();
        public static Dictionary<string, Sound> SoundsBySoundName = new Dictionary<string, Sound>();

        public int RaceID;
        public int VariantID;
        public CreatureGenderType Gender = CreatureGenderType.Neutral;
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
        public string SoundSpecialAttackName = string.Empty;
        public string SoundTechnicalAttackName = string.Empty;

        public static CreatureRaceSounds GetSoundsByRaceIDAndGender(int id, CreatureGenderType gender)
        {
            if (CreatureRaceSoundsList.Count == 0)
                PopulateCreatureRaceList();

            // Look for gender match
            foreach (CreatureRaceSounds creatureRaceSounds in CreatureRaceSoundsList)
                if (creatureRaceSounds.RaceID == id && creatureRaceSounds.Gender == gender)
                    return creatureRaceSounds;

            // Fall back to neutral
            foreach (CreatureRaceSounds creatureRaceSounds in CreatureRaceSoundsList)
                if (creatureRaceSounds.RaceID == id && creatureRaceSounds.Gender == CreatureGenderType.Neutral)
                    return creatureRaceSounds;

            // Error if nothing, and return blank record
            Logger.WriteError("Failed to find a CreatureRaceSound record that matches RaceID = '" + id.ToString() + "' and Gender = '" + gender.ToString() + "'");
            return new CreatureRaceSounds();
        }

        public static void GenerateAllSounds()
        {
            if (CreatureRaceSoundsList.Count == 0)
                PopulateCreatureRaceList();
            foreach(CreatureRaceSounds creatureRaceSounds in CreatureRaceSoundsList)
            {
                GenerateSoundIfUnique(creatureRaceSounds.SoundLoopName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundIdle1Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundIdle2Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundJumpName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundHit1Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundHit2Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundHit3Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundHit4Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundGasp1Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundGasp2Name);
                GenerateSoundIfUnique(creatureRaceSounds.SoundDeathName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundDrownName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundWalkingName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundRunningName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundAttackName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundSpecialAttackName);
                GenerateSoundIfUnique(creatureRaceSounds.SoundTechnicalAttackName);
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
            Sound newSound = new Sound(soundName, soundName, SoundType.NPCCombat, 8, 45, false);
            SoundsBySoundName[soundName] = newSound;
        }

        private static void PopulateCreatureRaceList()
        {
            CreatureRaceSoundsList.Clear();

            // Load in base sound data
            string soundDataFileName = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureRaceSounds.csv");
            Logger.WriteDetail("Populating CreatureRaceSoundsList via file '" + soundDataFileName + "'");
            string inputData = File.ReadAllText(soundDataFileName);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureRaceSounds list via file '" + soundDataFileName + "' did not have enough lines");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Make sure data size is correct
                string[] rowBlocks = row.Split(",");
                if (rowBlocks.Length < 20)
                {
                    Logger.WriteError("CreatureRaceSound list via file '" + soundDataFileName + "' has too few rows");
                    continue;
                }

                // Load the row
                CreatureRaceSounds creatureRaceSounds = new CreatureRaceSounds();
                creatureRaceSounds.RaceID = int.Parse(rowBlocks[0]);
                creatureRaceSounds.VariantID = int.Parse(rowBlocks[1]);
                string genderCode = rowBlocks[2];
                switch (genderCode.Trim().ToUpper())
                {
                    case "M": creatureRaceSounds.Gender = CreatureGenderType.Male; break;
                    case "F": creatureRaceSounds.Gender = CreatureGenderType.Female; break;
                    case "N": creatureRaceSounds.Gender = CreatureGenderType.Neutral; break;
                    default:
                        {
                            Logger.WriteError("CreatureRaceSound list invalid gender code of '" + genderCode.Trim().ToUpper() + "' for race '" + creatureRaceSounds.RaceID.ToString() + "'");
                            continue;
                        }
                }
                creatureRaceSounds.SoundLoopName = rowBlocks[3];
                creatureRaceSounds.SoundIdle1Name = rowBlocks[4];
                creatureRaceSounds.SoundIdle2Name = rowBlocks[5];
                creatureRaceSounds.SoundJumpName = rowBlocks[6];
                creatureRaceSounds.SoundHit1Name = rowBlocks[7];
                creatureRaceSounds.SoundHit2Name = rowBlocks[8];
                creatureRaceSounds.SoundHit3Name = rowBlocks[9];
                creatureRaceSounds.SoundHit4Name = rowBlocks[10];
                creatureRaceSounds.SoundGasp1Name = rowBlocks[11];
                creatureRaceSounds.SoundGasp2Name = rowBlocks[12];
                creatureRaceSounds.SoundDeathName = rowBlocks[13];
                creatureRaceSounds.SoundDrownName = rowBlocks[14];
                creatureRaceSounds.SoundWalkingName = rowBlocks[15];
                creatureRaceSounds.SoundRunningName = rowBlocks[16];
                creatureRaceSounds.SoundAttackName = rowBlocks[17];
                creatureRaceSounds.SoundSpecialAttackName = rowBlocks[18];
                creatureRaceSounds.SoundTechnicalAttackName = rowBlocks[19];
                CreatureRaceSoundsList.Add(creatureRaceSounds);
            }
        }
    }
}
