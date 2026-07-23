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

using EQWOWConverter.Items;

namespace EQWOWConverter.Creatures
{
    internal class CreatureCompanionPet
    {
        // Considered putting this in Configuration.cs, but unlikely to change
        public const int SUMMON_PROPERTIES_COMPANION_DBC_ID = 41; // SummonProperties.dbc row used by all wow vanity pet summons (Type = SUMMON_TYPE_MINIPET)
        public const int SPELL_ICON_DBC_ID = 1522; // Stock SpellIcon.dbc row for the icon that  the wow cat carriers use
        public const string ITEM_ICON_FILE_NAME_NO_EXT = "INV_Box_PetCarrier_01";
        public const int ITEM_LEARNING_SPELL_ID = 55884; // Stock spell "Learning"
        public const int SKILL_LINE_COMPANIONS_ID = 778; // Needed to show in the client's companions pet UI

        private static readonly object CompanionPetLock = new object();
        private static SortedDictionary<int, CreatureCompanionPet> EnabledCompanionPetsByID = new SortedDictionary<int, CreatureCompanionPet>();
        private static bool IsLoaded = false;

        public int ID = 0;
        public int WOWItemID = 0;
        public int WOWSpellID = 0;
        public CreatureCompanionDropRateType DropRateType = CreatureCompanionDropRateType.Low;
        public string Name = string.Empty;
        public float SizeMod = 1f;
        public int RaceID = 0;
        public int MatchGender = -1;
        public int MatchTexture = -1;
        public int MatchHelmTexture = -1;
        public int MatchFace = -1;
        public CreatureGenderType DisplayGender = CreatureGenderType.Neutral;
        public int DisplayTexture = 0;
        public int DisplayHelmTexture = 0;
        public int DisplayFace = 0;
        public CreatureTemplate? CreatureTemplate = null;
        public ItemTemplate? ItemTemplate = null;
        public int SpellVisualDBCID = 0;
        public int SpellVisualKitDBCID = 0;
        public int SummonSoundEntryDBCID = 0;

        public static SortedDictionary<int, CreatureCompanionPet> GetEnabledCompanionPetsByID()
        {
            lock (CompanionPetLock)
            {
                if (IsLoaded == false)
                    LoadCompanionPetData();
                return EnabledCompanionPetsByID;
            }
        }

        private static void LoadCompanionPetData()
        {
            string companionPetsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CompanionPetsMap.csv");
            Logger.WriteDebug(string.Concat("Loading companion pets via file '", companionPetsFile, "'"));
            List<Dictionary<string, string>> companionPetRows = FileTool.ReadAllRowsFromFileWithHeader(companionPetsFile, "|");
            foreach (Dictionary<string, string> columns in companionPetRows)
            {
                // Skip disabled rows
                if (columns["enabled"] == "0")
                    continue;

                CreatureCompanionPet companionPet = new CreatureCompanionPet();
                companionPet.ID = Convert.ToInt32(columns["id"]);
                companionPet.WOWItemID = Convert.ToInt32(columns["wow_itemid"]);
                companionPet.WOWSpellID = Convert.ToInt32(columns["wow_spelltemplateid"]);
                string dropRate = columns["drop_rate"].Trim().ToLower();
                switch (dropRate)
                {
                    case "low": companionPet.DropRateType = CreatureCompanionDropRateType.Low; break;
                    case "high": companionPet.DropRateType = CreatureCompanionDropRateType.High; break;
                    case "always": companionPet.DropRateType = CreatureCompanionDropRateType.Always; break;
                    default:
                        {
                            Logger.WriteError("CompanionPet with id '", companionPet.ID.ToString(), "' has an unhandled drop_rate of '", dropRate, "', so 'low' will be used");
                            companionPet.DropRateType = CreatureCompanionDropRateType.Low;
                        } break;
                }                    
                companionPet.Name = columns["name"];
                companionPet.SizeMod = Convert.ToSingle(columns["size_mod"]);
                companionPet.RaceID = Convert.ToInt32(columns["race_id"]);
                companionPet.MatchGender = Convert.ToInt32(columns["match_gender"]);
                companionPet.MatchTexture = Convert.ToInt32(columns["match_texture"]);
                companionPet.MatchHelmTexture = Convert.ToInt32(columns["match_helmtexture"]);
                companionPet.MatchFace = Convert.ToInt32(columns["match_face"]);
                switch (Convert.ToInt32(columns["display_gender"]))
                {
                    case 0: companionPet.DisplayGender = CreatureGenderType.Male; break;
                    case 1: companionPet.DisplayGender = CreatureGenderType.Female; break;
                    default: companionPet.DisplayGender = CreatureGenderType.Neutral; break;
                }
                companionPet.DisplayTexture = Convert.ToInt32(columns["display_texture"]);
                companionPet.DisplayHelmTexture = Convert.ToInt32(columns["display_helmtexture"]);
                companionPet.DisplayFace = Convert.ToInt32(columns["display_face"]);
                EnabledCompanionPetsByID.Add(companionPet.ID, companionPet);
            }
            IsLoaded = true;
            Logger.WriteDebug(string.Concat("Loading companion pets complete"));
        }

        public static CreatureCompanionPet? GetCompanionPetForCreatureTemplate(CreatureTemplate creatureTemplate)
        {
            lock (CompanionPetLock)
            {
                if (IsLoaded == false)
                    LoadCompanionPetData();

                int creatureGender = Convert.ToInt32(creatureTemplate.GenderType);
                CreatureCompanionPet? bestCompanionPet = null;
                int bestSpecificity = -1;
                foreach (CreatureCompanionPet companionPet in EnabledCompanionPetsByID.Values)
                {
                    if (companionPet.RaceID != creatureTemplate.Race.ID)
                        continue;
                    if (companionPet.MatchGender != -1 && companionPet.MatchGender != creatureGender)
                        continue;
                    if (companionPet.MatchTexture != -1 && companionPet.MatchTexture != creatureTemplate.TextureID)
                        continue;
                    if (companionPet.MatchHelmTexture != -1 && companionPet.MatchHelmTexture != creatureTemplate.HelmTextureID)
                        continue;
                    if (companionPet.MatchFace != -1 && companionPet.MatchFace != creatureTemplate.FaceID)
                        continue;

                    // -1 is a wildcard, so the more non-wildcard columns that matched the more specific the row is
                    int specificity = 0;
                    if (companionPet.MatchGender != -1)
                        specificity++;
                    if (companionPet.MatchTexture != -1)
                        specificity++;
                    if (companionPet.MatchHelmTexture != -1)
                        specificity++;
                    if (companionPet.MatchFace != -1)
                        specificity++;
                    if (specificity > bestSpecificity)
                    {
                        bestSpecificity = specificity;
                        bestCompanionPet = companionPet;
                    }
                }
                return bestCompanionPet;
            }
        }
    }
}
