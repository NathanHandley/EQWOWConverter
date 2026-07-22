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

using System.Text;

namespace EQWOWConverter
{
    internal class IDGenerationTool
    {
        private class IDGenerationCategory
        {
            public string Name = string.Empty;
            public List<int> RangeStartIDs = new List<int>();
            public List<int> RangeEndIDs = new List<int>();
            public int CurrentRangeIndex = 0;
            public int CurrentID = 0;
            public Dictionary<string, int> SavedIDsByContextKey = new Dictionary<string, int>();

            public int EndID { get { return RangeEndIDs[RangeEndIDs.Count - 1]; } }
        }

        private static readonly object IDGenerationLock = new object();
        private static Dictionary<string, IDGenerationCategory> CategoriesByName = new Dictionary<string, IDGenerationCategory>();
        private static bool SavedIDsAreLoaded = false;

        public static int GenerateID(string idName, params string[] contextKeyParts)
        {
            lock (IDGenerationLock)
            {
                InitializeIfNeeded();
                if (CategoriesByName.ContainsKey(idName) == false)
                    throw new Exception("IDGenerationTool error, unknown ID of name '" + idName + "'");
                if (contextKeyParts.Length == 0)
                    throw new Exception("IDGenerationTool error, for name '" + idName + "' there were no contextKeyParts");
                IDGenerationCategory category = CategoriesByName[idName];

                string contextKey = GenerateContextKey(contextKeyParts);
                if (category.SavedIDsByContextKey.ContainsKey(contextKey) == true)
                    return category.SavedIDsByContextKey[contextKey];

                MoveCurrentIDIntoValidRange(category);
                int returnID = category.CurrentID;
                if (returnID > category.EndID)
                    Logger.WriteError("Generated '" + idName + "' ID of '" + returnID.ToString() + "' is above the category end ID of '" + category.EndID.ToString() + "' which will cause issues");
                category.CurrentID++;
                category.SavedIDsByContextKey.Add(contextKey, returnID);
                AppendSavedIDToFile(category, contextKey, returnID);
                return returnID;
            }
        }

        private static void InitializeIfNeeded()
        {
            if (CategoriesByName.Count > 0)
                return;

            // DBC IDs
            Initialize("AreaBit", Configuration.DBCID_AREATABLE_AREABIT_BLOCK_1_START, Configuration.DBCID_AREATABLE_AREABIT_BLOCK_1_END,
                Configuration.DBCID_AREATABLE_AREABIT_BLOCK_2_START, Configuration.DBCID_AREATABLE_AREABIT_BLOCK_2_END,
                Configuration.DBCID_AREATABLE_AREABIT_BLOCK_3_START, Configuration.DBCID_AREATABLE_AREABIT_BLOCK_3_END);
            Initialize("CreatureDisplayInfoID", Configuration.DBCID_CREATUREDISPLAYINFO_ID_START, Configuration.DBCID_CREATUREDISPLAYINFO_ID_END);
            Initialize("CreatureFootstepID", Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_CREATUREFOOTSTEPID_START, int.MaxValue);
            Initialize("CreatureModelDataID", Configuration.DBCID_CREATUREMODELDATA_ID_START, int.MaxValue);
            Initialize("CreatureSoundDataID", Configuration.DBCID_CREATURESOUNDDATA_ID_START, int.MaxValue);
            Initialize("FootstepTerrainLookupID", Configuration.DBCID_FOOTSTEPTERRAINLOOKUP_ID_START, int.MaxValue);
            Initialize("GameObjectDisplayInfoID", Configuration.DBCID_GAMEOBJECTDISPLAYINFO_ID_START, int.MaxValue);
            Initialize("ItemDisplayInfoID", Configuration.DBCID_ITEMDISPLAYINFO_START, int.MaxValue);
            Initialize("LightID", Configuration.DBCID_LIGHT_ID_START, int.MaxValue);
            Initialize("LightParamsID", Configuration.DBCID_LIGHTPARAMS_ID_START, int.MaxValue);
            Initialize("LockID", Configuration.DBCID_LOCK_ID_START, int.MaxValue);
            Initialize("SkillLineID", Configuration.DBCID_SKILLLINE_ID_START, int.MaxValue);
            Initialize("SkillLineAbilityID", Configuration.DBCID_SKILLLINEABILITY_ID_START, int.MaxValue);
            Initialize("SkillRaceClassInfoID", Configuration.DBCID_SKILLRACECLASSINFO_ID_START, int.MaxValue);
            Initialize("SoundAmbienceID", Configuration.DBCID_SOUNDAMBIENCE_ID_START, int.MaxValue);
            Initialize("SoundEntryID", Configuration.DBCID_SOUNDENTRIES_ID_START, int.MaxValue);
            Initialize("SpellID", Configuration.DBCID_SPELL_ID_GENERATED_START, Configuration.DBCID_SPELL_ID_GENERATED_END);
            Initialize("SpellCastTimeID", Configuration.DBCID_SPELLCASTTIME_ID_START, int.MaxValue);
            Initialize("SpellCategoryID", Configuration.DBCID_SPELLCATEGORY_ID_START, int.MaxValue);
            Initialize("SpellItemEnchantmentID", Configuration.DBCID_SPELLITEMENCHANTMENT_ID_START, int.MaxValue);
            Initialize("SpellRadiusID", Configuration.DBCID_SPELLRADIUS_ID_START, int.MaxValue);
            Initialize("SpellRangeID", Configuration.DBCID_SPELLRANGE_ID_START, int.MaxValue);
            Initialize("SpellVisualID", Configuration.DBCID_SPELLVISUAL_ID_START, int.MaxValue);
            Initialize("SpellVisualKitID", Configuration.DBCID_SPELLVISUALKIT_ID_START, int.MaxValue);
            Initialize("SpellVisualEffectNameID", Configuration.DBCID_SPELLVISUALEFFECTNAME_ID_START, int.MaxValue);
            Initialize("SummonPropertiesID", Configuration.DBCID_SUMMONPROPERTIES_ID_START, int.MaxValue);
            Initialize("TaxiPathID", Configuration.DBCID_TAXIPATH_ID_START, int.MaxValue);
            Initialize("TaxiPathNodeID", Configuration.DBCID_TAXIPATHNODE_ID_START, int.MaxValue);
            Initialize("TotemCategoryID", Convert.ToInt32(Configuration.DBCID_TOTEMCATEGORY_ID_START), int.MaxValue);
            Initialize("TransportAnimationID", Configuration.DBCID_TRANSPORTANIMATION_ID_START, int.MaxValue);
            Initialize("WMOAreaTableID", Configuration.DBCID_WMOAREATABLE_ID_START, int.MaxValue);
            Initialize("ZoneMusicID", Configuration.DBCID_ZONEMUSIC_START, int.MaxValue);
            Initialize("ZoneWMOGroupID", Convert.ToInt32(Configuration.DBCID_WMOAREATABLE_WMOGROUPID_START), int.MaxValue);

            // SQL IDs
            Initialize("BroadcastTextID", Configuration.SQL_BROADCASTTEXT_ID_START, Configuration.SQL_BROADCASTTEXT_ID_END);
            if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                Initialize("CreatureGUID", Configuration.CONFIGONLY_SQL_CREATURE_GUID_DEBUG_LOW, Configuration.SQL_CREATURE_GUID_HIGH);
            else
                Initialize("CreatureGUID", Configuration.SQL_CREATURE_GUID_LOW, Configuration.SQL_CREATURE_GUID_HIGH);
            Initialize("CreatureImmunitiesID", Configuration.SQL_CREATUREIMMUNITIES_ID_START, Configuration.SQL_CREATUREIMMUNITIES_ID_END);
            Initialize("CreatureTemplateID", Configuration.SQL_CREATURETEMPLATE_GENERATED_START_ID, Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH);
            Initialize("GameEventID", Configuration.SQL_GAME_EVENTS_ID_START, Configuration.SQL_GAME_EVENTS_ID_END);
            Initialize("GameObjectGUID", Configuration.SQL_GAMEOBJECT_GUID_ID_START, Configuration.SQL_GAMEOBJECT_GUID_ID_END);
            Initialize("GameTeleRowID", Configuration.SQL_GAMETELE_ROWID_START, Configuration.SQL_GAMETELE_ROWID_END);
            Initialize("GossipMenuID", Configuration.SQL_GOSSIPMENU_MENUID_START, Configuration.SQL_GOSSIPMENU_MENUID_END);
            Initialize("NPCTextID", Configuration.SQL_NPCTEXT_ID_START, Configuration.SQL_NPCTEXT_ID_END);
            Initialize("PageTextID", Configuration.SQL_PAGETEXT_ID_START, Configuration.SQL_PAGETEXT_ID_END);
            Initialize("PetNameGenerationID", Configuration.SQL_PETNAMEGENERATION_ID_START, int.MaxValue);
            Initialize("PoolTemplateID", Configuration.SQL_POOL_TEMPLATE_ID_START, Configuration.SQL_POOL_TEMPLATE_ID_END);
            Initialize("ReferenceLootTemplateID", Configuration.SQL_REFERENCE_LOOT_TEMPLATE_ID_START, Configuration.SQL_REFERENCE_LOOT_TEMPLATE_ID_END);
            Initialize("SpellGroupID", Configuration.SQL_SPELL_GROUP_ID_START, Configuration.SQL_SPELL_GROUP_ID_END);
            Initialize("TrainerID", Configuration.SQL_TRAINER_ID_START, Configuration.SQL_TRAINER_ID_END);
            Initialize("TransportGUID", Configuration.SQL_TRANSPORTS_GUID_START, Configuration.SQL_TRANSPORTS_GUID_END);
            LoadSavedIDs();
        }

        private static void Initialize(string idName, params int[] rangeStartAndEndIDPairs)
        {
            if (CategoriesByName.ContainsKey(idName) == true)
            {
                Logger.WriteError("IDGenerationTool tried to initialize ID name '" + idName + "' more than once");
                return;
            }
            if (rangeStartAndEndIDPairs.Length == 0 || rangeStartAndEndIDPairs.Length % 2 != 0)
                throw new Exception("IDGenerationTool error, ID name '" + idName + "' was initialized without full start and end ID pairs");
            IDGenerationCategory category = new IDGenerationCategory();
            category.Name = idName;
            for (int i = 0; i < rangeStartAndEndIDPairs.Length; i += 2)
            {
                category.RangeStartIDs.Add(rangeStartAndEndIDPairs[i]);
                category.RangeEndIDs.Add(rangeStartAndEndIDPairs[i + 1]);
            }
            category.CurrentID = category.RangeStartIDs[0];
            CategoriesByName.Add(idName, category);
        }

        private static void MoveCurrentIDIntoValidRange(IDGenerationCategory category)
        {
            // Jump to the next range if the current one is used up.  If all ranges are used up, IDs run past the final range end and GenerateID logs the error
            while (category.CurrentRangeIndex < category.RangeStartIDs.Count - 1 && category.CurrentID > category.RangeEndIDs[category.CurrentRangeIndex])
            {
                category.CurrentRangeIndex++;
                if (category.CurrentID < category.RangeStartIDs[category.CurrentRangeIndex])
                    category.CurrentID = category.RangeStartIDs[category.CurrentRangeIndex];
            }
        }

        private static string GenerateContextKey(string[] contextKeyParts)
        {
            StringBuilder keySB = new StringBuilder();
            for (int i = 0; i < contextKeyParts.Length; i++)
            {
                if (i > 0)
                    keySB.Append("~");
                keySB.Append(contextKeyParts[i]);
            }

            // Control for things that'll screw up the format
            keySB.Replace("|", "_");
            keySB.Replace("\r", "_");
            keySB.Replace("\n", "_");

            // Trailing blank parts are trimmed so optional context key parts don't change the key
            return keySB.ToString().TrimEnd('~');
        }

        private static string GetSavedIDsFilePath()
        {
            return Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", Configuration.IDGENERATION_FILE_NAME);
        }

        private static void LoadSavedIDs()
        {
            if (SavedIDsAreLoaded == true)
                return;
            SavedIDsAreLoaded = true;

            string savedIDsFilePath = GetSavedIDsFilePath();
            if (File.Exists(savedIDsFilePath) == false)
            {
                Logger.WriteDebug("No saved generated IDs file found at '" + savedIDsFilePath + "', so all IDs will be newly generated");
                return;
            }

            Logger.WriteDebug("Loading saved generated IDs via file '" + savedIDsFilePath + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(savedIDsFilePath, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                string categoryName = columns["category"];
                if (CategoriesByName.ContainsKey(categoryName) == false)
                {
                    Logger.WriteError("Unknown category '" + categoryName + "' found in '" + savedIDsFilePath + "', skipping the row");
                    continue;
                }
                IDGenerationCategory category = CategoriesByName[categoryName];

                string contextKey = columns["contextkey"].TrimEnd('~');
                if (category.SavedIDsByContextKey.ContainsKey(contextKey) == true)
                {
                    Logger.WriteError("Duplicate '" + categoryName + "' context key '" + contextKey + "' found in '" + savedIDsFilePath + "', skipping the duplicate row");
                    continue;
                }

                int savedID = int.Parse(columns["id"]);
                category.SavedIDsByContextKey.Add(contextKey, savedID);

                // Ensure newly generated IDs never collide with previously saved ones
                if (savedID >= category.CurrentID)
                    category.CurrentID = savedID + 1;
            }
        }

        private static void AppendSavedIDToFile(IDGenerationCategory category, string contextKey, int id)
        {
            Dictionary<string, string> rowValues = new Dictionary<string, string>();
            rowValues.Add("category", category.Name);
            rowValues.Add("contextkey", contextKey);
            rowValues.Add("id", id.ToString());
            FileTool.AppendRowToFileWithHeader(GetSavedIDsFilePath(), "|", rowValues);
        }
    }
}
