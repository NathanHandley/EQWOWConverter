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

using EQWOWConverter.Common;
using EQWOWConverter.ObjectModels;

namespace EQWOWConverter.Creatures
{
    internal class CreatureIllusionCharacterRegistry
    {
        public class IllusionCharacterEntry
        {
            public CreatureRace Race;
            public CreatureGenderType GenderType;
            public float Scale = 1;
            public CreatureModelTemplate? ModelTemplate = null;
            public int ChrRacesID = 0;
            public int AltCreatureDisplayID = 0; // Second display of the same model, for the same-model display flip that re-applies mirror image data
            public int AltCreatureModelDataID = 0;
            public int CorpseCreatureDisplayID = 0;
            public int CorpseCreatureModelDataID = 0;
            public bool IsRobeCapable = false;
            public List<int> ValidFaceIndexes = new List<int>();
            public ObjectModelCharacterComposite? CharacterComposite = null; // Set once the model files generate; NPC skins of this race bake through it
            public string HelmTextureRelativePathBase = string.Empty; // Helm index ("01"-"03"), "C", tint index and ".blp" get appended
            public List<int> HelmVariantIndexes = new List<int>();
            public int HelmTintCount = 0;

            public string GetHelmTextureRelativePath(int helmIndex, int tintIndex)
            {
                return string.Concat(HelmTextureRelativePathBase, helmIndex.ToString("00"), "C", tintIndex.ToString(), ".blp");
            }
            public string BodyTextureRelativePath = string.Empty;
            public string FaceLowerTextureRelativePathBase = string.Empty; // Face index ("00" - "09") and ".blp" get appended
            public string FaceUpperTextureRelativePathBase = string.Empty; // Face index ("00" - "09") and ".blp" get appended

            public IllusionCharacterEntry(CreatureRace race, CreatureGenderType genderType, float scale)
            {
                Race = race;
                GenderType = genderType;
                Scale = scale;
            }

            public string GetFaceLowerTextureRelativePath(int faceIndex)
            {
                return string.Concat(FaceLowerTextureRelativePathBase, faceIndex.ToString("00"), ".blp");
            }

            public string GetFaceUpperTextureRelativePath(int faceIndex)
            {
                return string.Concat(FaceUpperTextureRelativePathBase, faceIndex.ToString("00"), ".blp");
            }
        }

        private static Dictionary<string, IllusionCharacterEntry> EntriesByRaceAndGender = new Dictionary<string, IllusionCharacterEntry>();
        private static readonly object IllusionCharacterLock = new object();

        private static string GetEntryKey(int raceID, CreatureGenderType genderType)
        {
            return string.Concat(raceID.ToString(), "_", Convert.ToInt32(genderType).ToString());
        }

        public static IllusionCharacterEntry? GetEntry(int raceID, CreatureGenderType genderType)
        {
            lock (IllusionCharacterLock)
            {
                string entryKey = GetEntryKey(raceID, genderType);
                if (EntriesByRaceAndGender.ContainsKey(entryKey) == false)
                    return null;
                return EntriesByRaceAndGender[entryKey];
            }
        }

        public static int GetChrRacesID(int raceID, CreatureGenderType genderType)
        {
            lock (IllusionCharacterLock)
            {
                string entryKey = GetEntryKey(raceID, genderType);
                if (EntriesByRaceAndGender.ContainsKey(entryKey) == false)
                    return 0;
                return EntriesByRaceAndGender[entryKey].ChrRacesID;
            }
        }

        public static string GetBlankTextureRelativePath()
        {
            return string.Concat("Character\\EverQuest\\", ObjectModelCharacterComposite.BLANK_TEXTURE_NAME, ".blp");
        }

        // The client only runs its character customization systems for models at Character\{ClientFileString}\{Male|Female}\, where
        // ClientFileString matches the model's ChrRaces row, so every EQ race gets a unique one
        public static string GetClientFileStringForRace(CreatureRace race)
        {
            string raceName = race.Name.Trim();
            raceName = raceName.Replace("/", "And");
            raceName = raceName.Replace(" ", "");
            raceName = raceName.Replace("-", "");
            return string.Concat("EQ", raceName);
        }

        public static string GetGenderFolderName(CreatureGenderType genderType)
        {
            if (genderType == CreatureGenderType.Female)
                return "Female";
            return "Male";
        }

        // Creates the player character model templates for every race+gender flagged CanShowEquipInIllusion, and assigns their IDs.
        // Must run after CreateCreatureModelTemplatesFromCreatureTemplates (which clears the shared template lists) and before the
        // creature model files generate
        public static void CreateModelTemplatesForAllEquipShowingRaces()
        {
            lock (IllusionCharacterLock)
            {
                EntriesByRaceAndGender.Clear();
                List<int> raceIDs = new List<int>();
                HashSet<int> allowedRaceIDs = new HashSet<int>();
                foreach (CreatureRace race in CreatureRace.GetAllCreatureRaces())
                {
                    if (race.CanShowEquipInIllusion == false || race.VariantID != 0)
                        continue;
                    if (allowedRaceIDs.Count > 0 && allowedRaceIDs.Contains(race.ID) == false)
                        continue;
                    if (race.Gender != CreatureGenderType.Male && race.Gender != CreatureGenderType.Female)
                        continue;
                    string entryKey = GetEntryKey(race.ID, race.Gender);
                    if (EntriesByRaceAndGender.ContainsKey(entryKey) == true)
                        continue;

                    // Same scale calculation the illusion form spells use, so the model renders at the same size in game
                    float scale = race.Height * race.SpawnSizeMod * (Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);
                    IllusionCharacterEntry entry = new IllusionCharacterEntry(race, race.Gender, scale);
                    entry.IsRobeCapable = (race.ID == 1 || race.ID == 3 || race.ID == 5 || race.ID == 6 || race.ID == 12 || race.ID == 128);
                    string raceIDString = race.ID.ToString();
                    string genderIDString = Convert.ToInt32(race.Gender).ToString();
                    entry.AltCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "playercharacteralt", raceIDString, genderIDString);
                    entry.AltCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "playercharacteralt", raceIDString, genderIDString);
                    entry.CorpseCreatureModelDataID = IDGenerationTool.GenerateID("CreatureModelDataID", "playercharactercorpse", raceIDString, genderIDString);
                    entry.CorpseCreatureDisplayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "playercharactercorpse", raceIDString, genderIDString);
                    entry.ModelTemplate = CreatureModelTemplate.GetOrCreateCreatureModelTemplate(race, race.Gender, 0, 0, 0, 0, scale, false, false, false, isPlayerCharacterVersion: true);
                    EntriesByRaceAndGender.Add(entryKey, entry);
                    if (raceIDs.Contains(race.ID) == false)
                        raceIDs.Add(race.ID);
                }

                // ChrRaces rows are one per race (male and female entries share it), IDs assigned in EQ race ID order
                raceIDs.Sort();
                foreach (var entryByKey in EntriesByRaceAndGender)
                    entryByKey.Value.ChrRacesID = Configuration.DBCID_CHRRACES_ID_START + raceIDs.IndexOf(entryByKey.Value.Race.ID);
            }
        }

        // Called during creature model file generation to store the composite output details (face list and texture paths)
        public static void SetModelOutputData(int raceID, CreatureGenderType genderType, ObjectModelCharacterComposite characterComposite)
        {
            lock (IllusionCharacterLock)
            {
                string entryKey = GetEntryKey(raceID, genderType);
                if (EntriesByRaceAndGender.ContainsKey(entryKey) == false)
                {
                    Logger.WriteError(string.Concat("CreatureIllusionCharacterRegistry has no entry for race ", raceID.ToString(), " gender ", genderType.ToString()));
                    return;
                }
                IllusionCharacterEntry entry = EntriesByRaceAndGender[entryKey];
                entry.CharacterComposite = characterComposite;
                entry.ValidFaceIndexes = new List<int>(characterComposite.ValidFaceIndexes);
                string skeletonNameUpper = characterComposite.SkeletonName.ToUpper();
                string modelFolder = string.Concat("Character\\", GetClientFileStringForRace(entry.Race), "\\", GetGenderFolderName(entry.GenderType), "\\");
                entry.BodyTextureRelativePath = string.Concat(modelFolder, characterComposite.GetBodyTextureName(), ".blp");
                entry.FaceLowerTextureRelativePathBase = string.Concat(modelFolder, "EQ", skeletonNameUpper, "FaceL");
                entry.FaceUpperTextureRelativePathBase = string.Concat(modelFolder, "EQ", skeletonNameUpper, "FaceU");
                entry.HelmTextureRelativePathBase = string.Concat(modelFolder, "EQ", skeletonNameUpper, "Helm");
                entry.HelmVariantIndexes = new List<int>(characterComposite.HelmVariantIndexes);
                entry.HelmTintCount = characterComposite.GetHelmTintCount();
            }
        }

        private static int CompareEntriesByRaceIDAndGender(IllusionCharacterEntry entry1, IllusionCharacterEntry entry2)
        {
            if (entry1.Race.ID != entry2.Race.ID)
                return entry1.Race.ID.CompareTo(entry2.Race.ID);
            return Convert.ToInt32(entry1.GenderType).CompareTo(Convert.ToInt32(entry2.GenderType));
        }

        // The stock death skeleton (bones) each EQ race uses
        public static string GetDeathSkeletonSourceName(CreatureRace race)
        {
            return "Human";
        }

        public static List<IllusionCharacterEntry> GetEntries()
        {
            lock (IllusionCharacterLock)
            {
                List<IllusionCharacterEntry> entries = new List<IllusionCharacterEntry>();
                foreach (var entryByKey in EntriesByRaceAndGender)
                    entries.Add(entryByKey.Value);
                entries.Sort(CompareEntriesByRaceIDAndGender);
                return entries;
            }
        }
    }
}
