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
using EQWOWConverter.GameObjects;
using EQWOWConverter.Items;
using EQWOWConverter.Player;
using EQWOWConverter.Quests;
using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;
using MySql.Data.MySqlClient;

namespace EQWOWConverter
{
    internal class SQLScriptWorker
    {
        // Characters
        private CharacterAuraSQL characterAuraSQL = new CharacterAuraSQL();
        private ModEverquestCharacterHomebindSQL modEverquestCharacterHomebindSQL = new ModEverquestCharacterHomebindSQL();
        // World
        private AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
        private AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();
        private BroadcastTextSQL broadcastTextSQL = new BroadcastTextSQL();
        private CreatureSQL creatureSQL = new CreatureSQL();
        private CreatureAddonSQL creatureAddonSQL = new CreatureAddonSQL();
        private CreatureLootTemplateSQL creatureLootTableSQL = new CreatureLootTemplateSQL();
        private CreatureModelInfoSQL creatureModelInfoSQL = new CreatureModelInfoSQL();
        private CreatureQuestEnderSQL creatureQuestEnderSQL = new CreatureQuestEnderSQL();
        private CreatureQuestStarterSQL creatureQuestStarterSQL = new CreatureQuestStarterSQL();
        private CreatureTemplateSQL creatureTemplateSQL = new CreatureTemplateSQL();
        private CreatureTemplateModelSQL creatureTemplateModelSQL = new CreatureTemplateModelSQL();
        private CreatureTemplateSpellSQL creatureTemplateSpellSQL = new CreatureTemplateSpellSQL();
        private CreatureTextSQL creatureTextSQL = new CreatureTextSQL();
        private GameEventSQL gameEventSQL = new GameEventSQL();
        private GameGraveyardSQL gameGraveyardSQL = new GameGraveyardSQL();
        private GameTeleSQL gameTeleSQL = new GameTeleSQL();
        private GameWeatherSQL gameWeatherSQL = new GameWeatherSQL();
        private GossipMenuSQL gossipMenuSQL = new GossipMenuSQL();
        private GossipMenuOptionSQL gossipMenuOptionSQL = new GossipMenuOptionSQL();
        private GraveyardZoneSQL graveyardZoneSQL = new GraveyardZoneSQL();
        private GameObjectSQL gameObjectSQL = new GameObjectSQL();
        private GameObjectTemplateSQL gameObjectTemplateSQL = new GameObjectTemplateSQL();
        private GameObjectTemplateAddonSQL gameObjectTemplateAddonSQL = new GameObjectTemplateAddonSQL();
        private InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
        private ItemLootTemplateSQL itemLootTemplateSQL = new ItemLootTemplateSQL();
        private ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
        private ModEverquestCreatureOnkillReputationSQL modEverquestCreatureOnkillReputationSQL = new ModEverquestCreatureOnkillReputationSQL();
        private ModEverquestPetSQL modEverquestPetSQL = new ModEverquestPetSQL();
        private ModEverquestSpellSQL modEverquestSpellSQL = new ModEverquestSpellSQL();
        private ModEverquestSystemConfigsSQL modEverquestSystemConfigsSQL = new ModEverquestSystemConfigsSQL();
        private ModEverquestQuestCompleteReputationSQL modEverquestQuestCompleteReputationSQL = new ModEverquestQuestCompleteReputationSQL();
        private NPCTextSQL npcTextSQL = new NPCTextSQL();
        private NPCTrainerSQL npcTrainerSQL = new NPCTrainerSQL();
        private NPCVendorSQL npcVendorSQL = new NPCVendorSQL();
        private PlayerCreateInfoSQL playerCreateInfoSQL = new PlayerCreateInfoSQL();
        private PlayerCreateInfoSpellCustomSQL playerCreateInfoSpellCustomSQL = new PlayerCreateInfoSpellCustomSQL();
        private PoolCreatureSQL poolCreatureSQL = new PoolCreatureSQL();
        private PoolPoolSQL poolPoolSQL = new PoolPoolSQL();
        private PoolTemplateSQL poolTemplateSQL = new PoolTemplateSQL();
        private QuestTemplateSQL questTemplateSQL = new QuestTemplateSQL();
        private QuestTemplateAddonSQL questTemplateAddonSQL = new QuestTemplateAddonSQL();
        private SmartScriptsSQL smartScriptsSQL = new SmartScriptsSQL();
        private SpellGroupSQL spellGroupSQL = new SpellGroupSQL();
        private SpellGroupStackRulesSQL spellGroupStackRulesSQL = new SpellGroupStackRulesSQL();
        private SpellLinkedSpellSQL spellLinkedSpellSQL = new SpellLinkedSpellSQL();
        private SpellScriptNamesSQL spellScriptNamesSQL = new SpellScriptNamesSQL();
        private SpellTargetPositionSQL spellTargetPositionSQL = new SpellTargetPositionSQL();
        private TransportsSQL transportsSQL = new TransportsSQL();
        private WaypointDataSQL waypointDataSQL = new WaypointDataSQL();

        public void CreateSQLScripts(List<Zone> zones, List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID, List<QuestTemplate> questTemplates,
            List<TradeskillRecipe> tradeskillRecipes, List<SpellTemplate> spellTemplates)
        {
            Logger.WriteInfo("Creating SQL Scripts...");

            // Clean the folder
            string sqlScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts");
            if (Directory.Exists(sqlScriptFolder))
                Directory.Delete(sqlScriptFolder, true);

            // System configs
            PopulateSystemConfigs();

            // Zones
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            Dictionary<string, int> mapIDsByShortName;
            PopulateZoneData(zones, zonePropertiesByShortName, out mapIDsByShortName);

            // Creatures
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
            PopulateCreatureData(creatureTemplates, creatureModelTemplates, creatureSpawnPools, spellTemplatesByEQID);

            // Game Events
            DateTime eventEnd = new DateTime(2025, 12, 30, 23, 0, 0);
            DateTime dayStart = new DateTime(2012, 10, 29, 6, 0, 0);
            gameEventSQL.AddRow(Configuration.SQL_GAMEEVENT_ID_DAY, dayStart, eventEnd, 1440, 840, "EQ Day");
            DateTime nightStart = new DateTime(2016, 10, 28, 20, 0, 0);
            gameEventSQL.AddRow(Configuration.SQL_GAMEEVENT_ID_NIGHT, nightStart, eventEnd, 1440, 600, "EQ Night");

            // Items
            PopulateItemData(itemLootTemplatesByCreatureTemplateID, spellTemplatesByEQID);

            // Player start properties
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true && zonePropertiesByShortName.Count > 0)
                PopulatePlayerStartData(zones, mapIDsByShortName);

            // Quests
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            PopulateQuestData(questTemplates, itemTemplatesByWOWEntryID);

            // Spells
            PopulateSpellAndTradeskillData(spellTemplates, tradeskillRecipes, itemTemplatesByWOWEntryID);

            // Trainer Abilities
            PopulateTrainerAbilityListData();

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
                PopulateTransportSQLData(zones, mapIDsByShortName);

            // Output them
            OutputSQLScriptsToDisk();
        }

        private void PopulateSystemConfigs()
        {
            modEverquestSystemConfigsSQL.AddRow("BardMaxConcurrentSongs", Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS.ToString());
            modEverquestSystemConfigsSQL.AddRow("DayEventID", Configuration.SQL_GAMEEVENT_ID_DAY.ToString());
            modEverquestSystemConfigsSQL.AddRow("NightEventID", Configuration.SQL_GAMEEVENT_ID_NIGHT.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMin", Configuration.DBCID_MAP_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMax", Configuration.DBCID_MAP_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMin", Configuration.DBCID_SPELL_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMax", Configuration.DBCID_SPELL_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDDayPhaseAura", Configuration.SPELLS_DAYPHASE_SPELLDBC_ID.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDNightPhaseAura", Configuration.SPELLS_NIGHTPHASE_SPELLDBC_ID.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMin", Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMax", Configuration.SQL_QUEST_TEMPLATE_ID_END.ToString());
        }

        private void PopulateCreatureData(List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, SpellTemplate> spellTemplatesByEQID)
        {
            // Pre-generate class trainer menus
            Dictionary<ClassType, int> classTrainerMenuIDs = new Dictionary<ClassType, int>();
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;

                // Base menu
                int gossipMenuID = GossipMenuSQL.GenerateUniqueMenuID();
                classTrainerMenuIDs.Add(classType, gossipMenuID);
                gossipMenuSQL.AddRow(gossipMenuID, Configuration.CREATURE_CLASS_TRAINER_NPC_TEXT_ID);

                // Menu options
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 0, 3, "I would like to train.",
                    Configuration.CREATURE_CLASS_TRAINER_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 1, 0, "I wish to unlearn my talents",
                    Configuration.CREATURE_CLASS_TRAINER_UNLEARN_BROADCAST_TEXT_ID, 16, 16, Configuration.CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID);
                gossipMenuOptionSQL.AddRowForClassTrainer(gossipMenuID, 2, 0, "I wish to know about Dual Talent Specialization.",
                    Configuration.CREATURE_CLASS_TRAINER_DUALTALENT_BROADCAST_TEXT_ID, 20, 1, Configuration.CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID);
            }

            // Creature Templates
            Dictionary<int, List<CreatureVendorItem>> vendorItems = CreatureVendorItem.GetCreatureVendorItemsByMerchantIDs();
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                // Skip invalid creatures
                if (creatureTemplate.ModelTemplate == null)
                {
                    Logger.WriteError("Error generating azeroth core scripts since model template was null for creature template '" + creatureTemplate.Name + "'");
                    continue;
                }                

                // Class Trainers
                if (creatureTemplate.ClassTrainerType != ClassType.All && creatureTemplate.ClassTrainerType != ClassType.None)
                {
                    // Trainers need a line in the npc trainers table
                    npcTrainerSQL.AddRowForTrainerReference(SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWClassTrainer(creatureTemplate.ClassTrainerType), creatureTemplate.WOWCreatureTemplateID);

                    // Associate the menu
                    creatureTemplate.GossipMenuID = classTrainerMenuIDs[creatureTemplate.ClassTrainerType];
                }

                // Profession Trainers
                if (creatureTemplate.TradeskillTrainerType != TradeskillType.None && creatureTemplate.TradeskillTrainerType != TradeskillType.Unknown)
                {
                    // Trainers need a line in the npc trainers table
                    npcTrainerSQL.AddRowForTrainerReference(SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer(creatureTemplate.TradeskillTrainerType), creatureTemplate.WOWCreatureTemplateID);
                }

                // Riding trainers
                if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true && creatureTemplate.IsRidingTrainer == true)
                {
                    npcTrainerSQL.AddRowForTrainerReference(202010, creatureTemplate.WOWCreatureTemplateID); // Same as Binjy Featherwhistle
                    if (Configuration.CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY == true)
                        npcTrainerSQL.AddRowForTrainerReference(202011, creatureTemplate.WOWCreatureTemplateID); // Same as Hargen Bronzewing
                }

                // Determine the display id
                int displayID = creatureTemplate.ModelTemplate.DBCCreatureDisplayID;
                if (creatureTemplate.IsNonNPC == true)
                    displayID = 11686; // Dranei totem

                // Create the records
                float scale = creatureTemplate.Size * creatureTemplate.Race.SpawnSizeMod;
                creatureTemplateSQL.AddRow(creatureTemplate, scale);
                creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, displayID, scale);

                // If it's a vendor, add the vendor records too
                if (creatureTemplate.MerchantID != 0 && vendorItems.ContainsKey(creatureTemplate.MerchantID))
                {
                    int curSlotNum = 0;
                    foreach (CreatureVendorItem vendorItem in vendorItems[creatureTemplate.MerchantID])
                    {
                        if (vendorItem.WOWItemID != -1)
                        {
                            npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, vendorItem.WOWItemID, curSlotNum);
                            curSlotNum++;
                        }
                        else
                        {
                            if (ItemTemplate.GetItemTemplatesByEQDBIDs().ContainsKey(vendorItem.EQItemID) == false)
                            {
                                Logger.WriteError("Attempted to add a merchant item with EQItemID '" + vendorItem.EQItemID + "' to merchant '" + creatureTemplate.MerchantID + "', but the EQItemID did not exist");
                                continue;
                            }
                            ItemTemplate itemTemplate = ItemTemplate.GetItemTemplatesByEQDBIDs()[vendorItem.EQItemID];

                            // Some vendor items are spell scrolls, and if so then there will be a vendor item row for each one
                            if (Configuration.SPELLS_LEARNABLE_FROM_ITEMS_ENABLED == false || spellTemplatesByEQID.ContainsKey(itemTemplate.EQScrollSpellID) == false)
                            {
                                npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, itemTemplate.WOWEntryID, curSlotNum);
                                curSlotNum++;
                            }
                            else
                            {
                                SpellTemplate spellTemplate = spellTemplatesByEQID[itemTemplate.EQScrollSpellID];
                                foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByClassType)
                                {
                                    npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, scrollPropertiesByClassType.Value.WOWItemTemplateID, curSlotNum);
                                    curSlotNum++;
                                }
                            }
                        }
                    }
                }

                // Kill rewards
                foreach (CreatureFactionKillReward creatureFactionKillReward in creatureTemplate.CreatureFactionKillRewards)
                    modEverquestCreatureOnkillReputationSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureFactionKillReward);

                // Spell scripts
                if (creatureTemplate.CreatureSpellListID > 0)
                {
                    // Add spell events for every heal entry
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesHeal)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        string comment = string.Concat("EQ Heal ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateInCombatHealCast(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, curSpellTemplate.WOWSpellID, curSpellTemplate.SpellRange, comment);
                    }

                    // Add spell events for every combat entry
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesCombat)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        string comment = string.Concat("EQ In Combat ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateInCombatSpellCast(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, curSpellTemplate.WOWSpellID, comment);
                    }

                    // Add spell events for every buff entry
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesOutOfCombatBuff)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        string comment = string.Concat("EQ Out of Combat Buffs ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateOutOfCombatBuffCastSelf(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, curSpellTemplate.WOWSpellID, comment);
                    }

                    // Spell on Attack
                    foreach (var eqSpellIDAndProcChance in creatureTemplate.AttackEQSpellIDAndProcChance)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[eqSpellIDAndProcChance.Item1];
                        string comment = string.Concat("EQ Attack Proc ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateApplySpellOnDamageDone(creatureTemplate.WOWCreatureTemplateID, eqSpellIDAndProcChance.Item2,
                            curSpellTemplate.WOWSpellID, comment);
                    }
                }

                // Creature spell associations for pet creatures
                if (creatureTemplate.IsPet == true)
                {
                    int curIndex = 0;
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesOutOfCombatBuff)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, curSpellTemplate.WOWSpellID);
                        curIndex++;
                    }
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesHeal)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, curSpellTemplate.WOWSpellID);
                        curIndex++;
                    }
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesCombat)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, curSpellTemplate.WOWSpellID);
                        curIndex++;
                    }
                }
            }

            // Creature models
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
                creatureModelInfoSQL.AddRow(creatureModelTemplate.DBCCreatureDisplayID, Convert.ToInt32(creatureModelTemplate.GenderType));

            // Creature and Spawn Pools
            foreach (CreatureSpawnPool spawnPool in creatureSpawnPools)
            {
                // For single element single location pools, just create a creature record
                if (spawnPool.CreatureSpawnInstances.Count == 1 && spawnPool.CreatureTemplates.Count == 1)
                {
                    CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[0];
                    CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[0];
                    int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                    string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                    if (pathGridEntries.Count > 0)
                    {
                        int waypointGUID = creatureGUID * 1000;
                        creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                        foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                            waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path,
                            spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                    }
                    else
                    {
                        creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                        CreatureMovementType movementType = CreatureMovementType.None;
                        if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                            movementType = CreatureMovementType.Random;
                        creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                            spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                            spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                    }
                }

                // Create a pool pools if there are multiple locations
                else if (spawnPool.CreatureSpawnInstances.Count > 1)
                {
                    // Create the mother pool template
                    int motherPoolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                    List<string> motherPoolNames = new List<string>();
                    foreach (CreatureTemplate template in spawnPool.CreatureTemplates)
                        if (motherPoolNames.Contains(template.Name) == false)
                            motherPoolNames.Add(template.Name);
                    string motherPoolDescription = "(" + spawnPool.CreatureSpawnGroup.ID + ")";
                    foreach (string name in motherPoolNames)
                        motherPoolDescription += ", " + name;
                    poolTemplateSQL.AddRow(motherPoolTemplateID, motherPoolDescription + " - Master Pool", spawnPool.GetMaxSpawnCount());

                    // Create by instance groups
                    for (int spawnInstanceIndex = 0; spawnInstanceIndex < spawnPool.CreatureSpawnInstances.Count; spawnInstanceIndex++)
                    {
                        CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[spawnInstanceIndex];

                        // Create the pool pool
                        int poolPoolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                        string poolPoolDescription = motherPoolDescription + " - " + spawnInstanceIndex.ToString();
                        poolTemplateSQL.AddRow(poolPoolTemplateID, poolPoolDescription, 1);
                        poolPoolSQL.AddRow(poolPoolTemplateID, motherPoolTemplateID, 0, poolPoolDescription);

                        // Create the creature records
                        for (int creatureTemplateIndex = 0; creatureTemplateIndex < spawnPool.CreatureTemplates.Count; creatureTemplateIndex++)
                        {
                            CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[creatureTemplateIndex];
                            int chance = spawnPool.CreatureTemplateChances[creatureTemplateIndex];
                            int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                            poolCreatureSQL.AddRow(creatureGUID, poolPoolTemplateID, chance, creatureTemplate.Name);
                            List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                            string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                            if (pathGridEntries.Count > 0)
                            {
                                int waypointGUID = creatureGUID * 1000;
                                creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                                foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                                    waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path,
                                    spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                            }
                            else
                            {
                                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                                CreatureMovementType movementType = CreatureMovementType.None;
                                if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                                    movementType = CreatureMovementType.Random;
                                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                    spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                                    spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                            }
                        }
                    }
                }

                // No mother pool needed
                else if (spawnPool.CreatureSpawnInstances.Count == 1 && spawnPool.CreatureTemplates.Count > 0)
                {
                    CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[0];

                    // Make the pool description
                    List<string> poolNames = new List<string>();
                    foreach (CreatureTemplate template in spawnPool.CreatureTemplates)
                        if (poolNames.Contains(template.Name) == false)
                            poolNames.Add(template.Name);
                    string poolDescription = "(" + spawnPool.CreatureSpawnGroup.ID + ")";
                    foreach (string name in poolNames)
                        poolDescription += ", " + name;

                    // Create the pool template
                    int poolTemplateID = CreatureSpawnPool.GetPoolTemplateSQLID();
                    poolTemplateSQL.AddRow(poolTemplateID, poolDescription, spawnPool.GetMaxSpawnCount());

                    // Create the creature records
                    for (int creatureTemplateIndex = 0; creatureTemplateIndex < spawnPool.CreatureTemplates.Count; creatureTemplateIndex++)
                    {
                        CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[creatureTemplateIndex];
                        int chance = spawnPool.CreatureTemplateChances[creatureTemplateIndex];
                        int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                        poolCreatureSQL.AddRow(creatureGUID, poolTemplateID, chance, creatureTemplate.Name);
                        List<CreaturePathGridEntry> pathGridEntries = spawnInstance.GetPathGridEntries();
                        string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.CreatureSpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                        if (pathGridEntries.Count > 0)
                        {
                            int waypointGUID = creatureGUID * 1000;
                            creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                            foreach (CreaturePathGridEntry pathGridEntry in pathGridEntries)
                                waypointDataSQL.AddRow(waypointGUID, pathGridEntry.Number, pathGridEntry.NodeX, pathGridEntry.NodeY, pathGridEntry.NodeZ, pathGridEntry.PauseInSec * 1000);
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, CreatureMovementType.Path,
                                spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                        }
                        else
                        {
                            creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                            CreatureMovementType movementType = CreatureMovementType.None;
                            if (spawnPool.CreatureSpawnGroup.RoamDistance > 1)
                                movementType = CreatureMovementType.Random;
                            creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID,
                                spawnInstance.SpawnXPosition, spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType,
                                spawnPool.CreatureSpawnGroup.RoamDistance, spawnInstance.SpawnDay, spawnInstance.SpawnNight, comment);
                        }
                    }
                }
            }
        }

        private void PopulateItemData(Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID, Dictionary<int, SpellTemplate> spellTemplatesByEQID)
        {
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;

                // Associate spells if it's a learnable item
                if (itemTemplate.DoesTeachSpell == true && itemTemplate.EQScrollSpellID != 0)
                {
                    // Make items "junk" if there is no spell to learn and there should be
                    if (Configuration.SPELLS_LEARNABLE_FROM_ITEMS_ENABLED == false || spellTemplatesByEQID.ContainsKey(itemTemplate.EQScrollSpellID) == false)
                    {
                        itemTemplate.EQScrollSpellID = 0;
                        itemTemplate.DoesTeachSpell = false;
                        itemTemplate.Quality = ItemWOWQuality.Poor;
                        itemTemplate.Description = "The magic in this scroll has faded to time";
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.Description, itemTemplate.RequiredLevel, itemTemplate.AllowedClassTypes);
                    }
                    else
                    {
                        // If it is a valid spell scroll, there is one scroll per class that can learn it
                        SpellTemplate spellTemplate = spellTemplatesByEQID[itemTemplate.EQScrollSpellID];
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                        itemTemplate.WOWSpellID1 = spellTemplate.WOWSpellID;
                        itemTemplate.Description = string.Concat("Teaches the spell: ", spellTemplate.Name);
                        foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByClassType)
                        {
                            string scrollName = itemTemplate.Name;
                            if (spellTemplate.LearnScrollPropertiesByClassType.Count != 1)
                                scrollName = string.Concat(itemTemplate.Name, " (", scrollPropertiesByClassType.Key.ToString(), ")");
                            itemTemplateSQL.AddRow(itemTemplate, scrollPropertiesByClassType.Value.WOWItemTemplateID, scrollName,
                                itemTemplate.Description, scrollPropertiesByClassType.Value.LearnLevel, new List<ClassType>() { scrollPropertiesByClassType.Key });
                        }
                    }
                }
                else
                {
                    itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.Description, itemTemplate.RequiredLevel, itemTemplate.AllowedClassTypes);
                    for (int i = 0; i < itemTemplate.ContainedWOWItemTemplateIDs.Count; i++)
                    {
                        int curWOWItemTemplateID = itemTemplate.ContainedWOWItemTemplateIDs[i];
                        float curItemChance = itemTemplate.ContainedItemChances[i];
                        int curItemCount = 1;
                        if (itemTemplate.ContainedtemCounts.Count > i)
                            curItemCount = itemTemplate.ContainedtemCounts[i];
                        string comment = string.Concat(itemTemplate.Name, " - ", itemTemplatesByWOWID[curWOWItemTemplateID].Name);
                        if (curItemChance >= 100)
                            itemLootTemplateSQL.AddRow(itemTemplate.WOWEntryID, curWOWItemTemplateID, 2 + i, curItemChance, curItemCount, comment);
                        else
                            itemLootTemplateSQL.AddRow(itemTemplate.WOWEntryID, curWOWItemTemplateID, 1, curItemChance, curItemCount, comment);
                    }
                }
            }
            foreach (var itemLootTemplateByCreatureTemplateID in itemLootTemplatesByCreatureTemplateID.Values)
                foreach (ItemLootTemplate itemLootTemplate in itemLootTemplateByCreatureTemplateID)
                    creatureLootTableSQL.AddRow(itemLootTemplate);
        }

        private void PopulatePlayerStartData(List<Zone> zones, Dictionary<string, int> mapIDsByShortName)
        {
            // Restrict to loaded zones
            Dictionary<string, int> areaIDsByShortName = new Dictionary<string, int>();
            foreach (Zone zone in zones)
            {
                areaIDsByShortName.Add(zone.ShortName.ToLower().Trim(), Convert.ToInt32(zone.DefaultArea.DBCAreaTableID));
            }
            foreach (var classRaceProperties in PlayerClassRaceProperties.GetClassRacePropertiesByRaceAndClassID())
            {
                string startZoneShortName = classRaceProperties.Value.StartZoneShortName;
                if (mapIDsByShortName.ContainsKey(classRaceProperties.Value.StartZoneShortName) == false)
                {
                    Logger.WriteDebug(string.Concat("Could not map player location for zone short name '", classRaceProperties.Value.StartZoneShortName, "' since no zone was loaded with that shortname.  Using a fallback"));
                    startZoneShortName = mapIDsByShortName.First().Key;
                }

                playerCreateInfoSQL.AddRow(classRaceProperties.Key.Item1, classRaceProperties.Key.Item2, mapIDsByShortName[startZoneShortName],
                    areaIDsByShortName[startZoneShortName], classRaceProperties.Value.StartPositionX, classRaceProperties.Value.StartPositionY,
                    classRaceProperties.Value.StartPositionZ, classRaceProperties.Value.StartOrientation);
            }
        }

        private void PopulateQuestData(List<QuestTemplate> questTemplates, SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            Dictionary<int, int> creatureTextGroupIDsByCreatureTemplateID = new Dictionary<int, int>();
            foreach (QuestTemplate questTemplate in questTemplates)
            {
                // Skip quests where items needed to complete it are not available
                if (questTemplate.AreRequiredItemsPlayerObtainable(itemTemplatesByWOWEntryID) == false)
                    continue;

                string firstQuestName = questTemplate.Name;
                int firstQuestID = questTemplate.QuestIDWOW;
                string repeatQuestName = string.Concat(questTemplate.Name, " (repeat)");
                int repeatQuestID = firstQuestID + Configuration.SQL_QUEST_TEMPLATE_ID_REPEATABLE_SHIFT;

                questTemplateSQL.AddRow(questTemplate, firstQuestID, firstQuestName);
                questTemplateSQL.AddRow(questTemplate, repeatQuestID, repeatQuestName);
                questTemplateAddonSQL.AddRow(questTemplate, firstQuestID, 0, false);
                questTemplateAddonSQL.AddRow(questTemplate, repeatQuestID, firstQuestID, true);

                // Quest NPCs
                Dictionary<int, CreatureTemplate> creatureTemplateByWOWID = CreatureTemplate.GetCreatureTemplateListByWOWID();
                foreach (int creatureTemplateID in questTemplate.QuestgiverWOWCreatureTemplateIDs)
                {
                    creatureQuestStarterSQL.AddRow(firstQuestID, creatureTemplateID);
                    creatureQuestStarterSQL.AddRow(repeatQuestID, creatureTemplateID);
                    creatureQuestEnderSQL.AddRow(firstQuestID, creatureTemplateID);
                    creatureQuestEnderSQL.AddRow(repeatQuestID, creatureTemplateID);

                    // Reward say/yell/emote actions
                    foreach (QuestReaction reaction in questTemplate.Reactions)
                    {
                        if (reaction.type == QuestReactionType.Emote || reaction.type == QuestReactionType.Say || reaction.type == QuestReactionType.Yell)
                        {
                            // Broadcast Text
                            int broadcastID = BroadcastTextSQL.GenerateUniqueID();
                            broadcastTextSQL.AddRow(broadcastID, reaction.ReactionValue, reaction.ReactionValue);

                            // Creature Text
                            int creatureTextGroupID = 0;
                            if (creatureTextGroupIDsByCreatureTemplateID.ContainsKey(creatureTemplateID) == true)
                            {
                                creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID] += 2;
                                creatureTextGroupID = creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID];
                            }
                            else
                                creatureTextGroupIDsByCreatureTemplateID.Add(creatureTemplateID, 0);
                            string comment = string.Concat("EQ ", creatureTemplateByWOWID[creatureTemplateID].Name, " Quest ", reaction.type.ToString());
                            int messageType = 12; // Default to say
                            switch (reaction.type)
                            {
                                case QuestReactionType.Say: messageType = 12; break;
                                case QuestReactionType.Emote: messageType = 16; break;
                                case QuestReactionType.Yell: messageType = 14; break;
                                default: Logger.WriteError("Unhandled quest reaction type of " + reaction.type); break;
                            }
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID + 1, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);

                            // Smart Script
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID, firstQuestID, comment);
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID + 1, repeatQuestID, comment);
                        }
                    }
                }

                // Reputation rewards
                foreach (QuestCompletionFactionReward completionReputation in questTemplate.questCompletionFactionRewards)
                {
                    modEverquestQuestCompleteReputationSQL.AddRow(firstQuestID, completionReputation);
                    modEverquestQuestCompleteReputationSQL.AddRow(repeatQuestID, completionReputation);
                }
            }
        }

        private void PopulateSpellAndTradeskillData(List<SpellTemplate> spellTemplates, List<TradeskillRecipe> tradeskillRecipes,
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            // Spell split data
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                // Stack rules
                if (spellTemplate.SpellGroupStackingID > 0)
                    spellGroupSQL.AddRow(spellTemplate.SpellGroupStackingID, spellTemplate.WOWSpellID);
                
                // Additional spell data
                modEverquestSpellSQL.AddRow(spellTemplate, spellTemplate.WOWSpellID);

                // Grab effects in blocks of three
                List<SpellEffectBlock> groupedBaseSpellEffectBlocksForOutput = spellTemplate.GroupedBaseSpellEffectBlocksForOutput;

                // Teleports are only on the main 3-block
                for (int i = 0; i < groupedBaseSpellEffectBlocksForOutput[0].SpellEffects.Count; i++)
                {
                    if (groupedBaseSpellEffectBlocksForOutput[0].SpellEffects[i].EffectType == SpellWOWEffectType.TeleportUnits)
                    {
                        SpellEffectWOW curEffect = groupedBaseSpellEffectBlocksForOutput[0].SpellEffects[i];
                        spellTargetPositionSQL.AddRow(groupedBaseSpellEffectBlocksForOutput[0].WOWSpellID, i, curEffect.TeleMapID, curEffect.TelePosition, curEffect.TeleOrientation);
                    }
                }

                // Chains for spells with > 3 effects
                for (int i = 1; i < groupedBaseSpellEffectBlocksForOutput.Count; i++)
                {
                    SpellEffectBlock curEffectBlock = spellTemplate.GroupedBaseSpellEffectBlocksForOutput[i];

                    // TODO: Check if there are mixtures of aura and non-aura spells first, this may have a bug
                    if (spellTemplate.AuraDuration.MaxDurationInMS > 0)
                        spellLinkedSpellSQL.AddRowForAuraTrigger(spellTemplate.WOWSpellID, curEffectBlock.WOWSpellID, curEffectBlock.SpellName);
                    else
                        spellLinkedSpellSQL.AddRowForHitTrigger(spellTemplate.WOWSpellID, curEffectBlock.WOWSpellID, curEffectBlock.SpellName);
                    
                    // Additional spell data
                    modEverquestSpellSQL.AddRow(spellTemplate, curEffectBlock.WOWSpellID);

                    // Worn effects get their own copy too
                    if (spellTemplate.WOWSpellIDWorn > 0)
                    {
                        // Additional spell data
                        SpellEffectBlock curWornEffectBlock = spellTemplate.GroupedWornSpellEffectBlocksForOutput[i];
                        modEverquestSpellSQL.AddRow(spellTemplate, curWornEffectBlock.WOWSpellID);
                    }
                }

                // Additional changed spells (caused by chain effects directly)
                foreach(SpellTemplate chainedSpellTemplate in spellTemplate.ChainedSpellTemplates)
                {
                    List<SpellEffectBlock> chainedGroupedBaseSpellEffectBlocksForOutput = chainedSpellTemplate.GroupedBaseSpellEffectBlocksForOutput;
                    int chainedSpellID = chainedGroupedBaseSpellEffectBlocksForOutput[0].WOWSpellID;
                    string chainedSpellName = chainedGroupedBaseSpellEffectBlocksForOutput[0].SpellName;
                    if (spellTemplate.AuraDuration.MaxDurationInMS > 0)
                        spellLinkedSpellSQL.AddRowForAuraTrigger(spellTemplate.WOWSpellID, chainedSpellID, chainedSpellName);
                    else
                        spellLinkedSpellSQL.AddRowForHitTrigger(spellTemplate.WOWSpellID, chainedSpellID, chainedSpellName);
                }

                // Focus boostable spells use all blocks
                if (spellTemplate.IsFocusBoostableEffect)
                    for (int i = 0; i < groupedBaseSpellEffectBlocksForOutput.Count; i++)
                    {
                        if (groupedBaseSpellEffectBlocksForOutput[i].SpellEffects[0].IsAuraType())
                            spellScriptNamesSQL.AddRow(groupedBaseSpellEffectBlocksForOutput[i].WOWSpellID, "EverQuest_FocusBoostAuraScript");
                        else
                            spellScriptNamesSQL.AddRow(groupedBaseSpellEffectBlocksForOutput[i].WOWSpellID, "EverQuest_FocusBoostNonAuraScript");
                    }

                // Bard songs have a script
                if (spellTemplate.IsBardSongAura == true)
                    spellScriptNamesSQL.AddRow(spellTemplate.WOWSpellID, "EverQuest_BardSongAuraScript");

                // Save any pet details
                if (spellTemplate.SummonSpellPet != null)
                    modEverquestPetSQL.AddRow(spellTemplate.WOWSpellID, spellTemplate.SummonSpellPet.NamingType);
            }
            foreach (var spellGroupStackRuleByGroup in SpellTemplate.SpellGroupStackRuleByGroup)
                spellGroupStackRulesSQL.AddRow(spellGroupStackRuleByGroup.Key, spellGroupStackRuleByGroup.Value);

            // Custom gate and bind
            if (Configuration.PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START == true)
            {
                playerCreateInfoSpellCustomSQL.AddRow(Configuration.SPELLS_GATECUSTOM_SPELLDBC_ID, "Gate");
                playerCreateInfoSpellCustomSQL.AddRow(Configuration.SPELLS_BINDCUSTOM_SPELLDBC_ID, "Bind");
            }
        }

        private void PopulateTrainerAbilityListData()
        {
            // Trainer Abilities - Class
            foreach (ClassType classType in Enum.GetValues(typeof(ClassType)))
            {
                if (classType == ClassType.All || classType == ClassType.None)
                    continue;

                int lineID = SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWClassTrainer(classType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForClass(classType))
                    npcTrainerSQL.AddRowForTrainerAbility(lineID, trainerAbility);
            }

            // Trainer Abilities - Tradeskills
            foreach (TradeskillType tradeskillType in Enum.GetValues(typeof(TradeskillType)))
            {
                if (tradeskillType == TradeskillType.Unknown || tradeskillType == TradeskillType.None)
                    continue;

                int lineID = SpellTrainerAbility.GetTrainerSpellsReferenceLineIDForWOWTradeskillTrainer(tradeskillType);
                npcTrainerSQL.AddDevelopmentSkillsForTradeskill(lineID, tradeskillType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForTradeskill(tradeskillType))
                    npcTrainerSQL.AddRowForTrainerAbility(lineID, trainerAbility);
            }
        }

        private void PopulateTransportSQLData(List<Zone> zones, Dictionary<string, int> mapIDsByShortName)
        {
            foreach (TransportShip transportShip in TransportShip.GetAllTransportShips())
            {
                // Only add this transport ship if the full path is zones that are loaded
                bool zonesAreLoaded = true;
                foreach (string touchedZone in transportShip.GetTouchedZonesSplitOut())
                {
                    if (mapIDsByShortName.ContainsKey(touchedZone.ToLower().Trim()) == false)
                    {
                        zonesAreLoaded = false;
                        Logger.WriteDebug("Skipping transport ship since zone '" + touchedZone + "' isn't being converted");
                        break;
                    }
                }
                if (zonesAreLoaded == false)
                    continue;
                string name = "Ship EQ (" + transportShip.Name + ")";
                string longName = transportShip.TouchedZones + " (" + name + ")";
                transportsSQL.AddRow(transportShip.WOWGameObjectTemplateID, longName);
                gameObjectTemplateSQL.AddRowForTransportShip(transportShip.WOWGameObjectTemplateID, transportShip.GameObjectDisplayInfoID, name,
                    transportShip.TaxiPathID, Convert.ToInt32(transportShip.MapID));
                gameObjectTemplateAddonSQL.AddRowForTransport(transportShip.WOWGameObjectTemplateID);
            }
            foreach (TransportLift transportLift in TransportLift.GetAllTransportLifts())
            {
                // Only add this transport lift if the zone is loaded
                if (mapIDsByShortName.ContainsKey(transportLift.SpawnZoneShortName.ToLower().Trim()) == false)
                {
                    Logger.WriteDebug("Skipping transport lift since zone '" + transportLift.SpawnZoneShortName + "' isn't being converted");
                    continue;
                }
                int areaID = 0;
                foreach (Zone zone in zones)
                    if (zone.ShortName.ToLower().Trim() == transportLift.SpawnZoneShortName.ToLower().Trim())
                        areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                string name = "Lift EQ (" + transportLift.Name + ")";
                int mapID = mapIDsByShortName[transportLift.SpawnZoneShortName.ToLower().Trim()];
                gameObjectTemplateSQL.AddRowForTransportLift(transportLift.GameObjectTemplateID, transportLift.GameObjectDisplayInfoID, name, transportLift.EndTimestamp);
                gameObjectTemplateAddonSQL.AddRowForTransport(transportLift.GameObjectTemplateID);
                gameObjectSQL.AddRow(transportLift.GameObjectGUID, transportLift.GameObjectTemplateID, mapID, areaID, new Vector3(transportLift.SpawnX, transportLift.SpawnY, transportLift.SpawnZ), transportLift.Orientation);
            }
            foreach (TransportLiftTrigger transportLiftTrigger in TransportLiftTrigger.GetAllTransportLiftTriggers())
            {
                // Only add this transport lift if the zone is loaded
                if (mapIDsByShortName.ContainsKey(transportLiftTrigger.SpawnZoneShortName.ToLower().Trim()) == false)
                {
                    Logger.WriteDebug("Skipping transport lift trigger since zone '" + transportLiftTrigger.SpawnZoneShortName + "' isn't being converted");
                    continue;
                }
                int areaID = 0;
                foreach (Zone zone in zones)
                    if (zone.ShortName.ToLower().Trim() == transportLiftTrigger.SpawnZoneShortName.ToLower().Trim())
                        areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                string name = transportLiftTrigger.Name;
                int mapID = mapIDsByShortName[transportLiftTrigger.SpawnZoneShortName.ToLower().Trim()];
                gameObjectTemplateSQL.AddRowForTransportLiftTrigger(transportLiftTrigger.GameObjectTemplateID, transportLiftTrigger.GameObjectDisplayInfoID, name, transportLiftTrigger.ResetTimeInMS);
                gameObjectTemplateAddonSQL.AddRowNoDespawn(transportLiftTrigger.GameObjectTemplateID);
                gameObjectSQL.AddRow(transportLiftTrigger.GameObjectGUID, transportLiftTrigger.GameObjectTemplateID, mapID, areaID, new Vector3(transportLiftTrigger.SpawnX, transportLiftTrigger.SpawnY,
                    transportLiftTrigger.SpawnZ), transportLiftTrigger.Orientation);
            }
        }

        private void PopulateZoneData(List<Zone> zones, Dictionary<string, ZoneProperties> zonePropertiesByShortName, out Dictionary<string, int> mapIDsByShortName)
        {
            foreach (Zone zone in zones)
            {
                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID));

                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.DescriptiveNameOnlyLetters, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ShortName, zone.SafePosition.Y, zone.SafePosition.Y, zone.SafePosition.Z);

                // Weather data
                if (Configuration.ZONE_WEATHER_ENABLED == true)
                    gameWeatherSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DefaultZoneArea.DBCAreaTableID), zone.ZoneProperties.RainChanceWinter, zone.ZoneProperties.SnowChanceWinter,
                        zone.ZoneProperties.RainChanceSpring, zone.ZoneProperties.SnowChanceSpring, zone.ZoneProperties.RainChanceSummer, zone.ZoneProperties.SnowChanceSummer,
                        zone.ZoneProperties.RainChanceFall, zone.ZoneProperties.RainChanceWinter);

                // Zone lines
                foreach (ZonePropertiesZoneLineBox zoneLine in ZoneProperties.GetZonePropertiesForZone(zone.ShortName).ZoneLineBoxes)
                {
                    // Skip invalid zone lines
                    if (zonePropertiesByShortName.ContainsKey(zoneLine.TargetZoneShortName.ToLower()) == false)
                        continue;

                    // Area Trigger
                    areaTriggerSQL.AddRow(zoneLine.AreaTriggerID, zone.ZoneProperties.DBCMapID, zoneLine.BoxPosition.X, zoneLine.BoxPosition.Y,
                        zoneLine.BoxPosition.Z, zoneLine.BoxLength, zoneLine.BoxWidth, zoneLine.BoxHeight, zoneLine.BoxOrientation);

                    // Area Trigger Teleport
                    int areaTriggerID = zoneLine.AreaTriggerID;
                    string descriptiveName = "EQ " + zone.ShortName + " - " + zoneLine.TargetZoneShortName + " zone line";
                    int targetMapId = ZoneProperties.GetZonePropertiesForZone(zoneLine.TargetZoneShortName).DBCMapID;
                    float targetPositionX = zoneLine.TargetZonePosition.X;
                    float targetPositionY = zoneLine.TargetZonePosition.Y;
                    float targetPositionZ = zoneLine.TargetZonePosition.Z;
                    float targetOrientation = zoneLine.TargetZoneOrientation;
                    areaTriggerTeleportSQL.AddRow(areaTriggerID, descriptiveName, targetMapId, targetPositionX, targetPositionY, targetPositionZ, targetOrientation);
                }
            }
            mapIDsByShortName = new Dictionary<string, int>();
            foreach (Zone zone in zones)
                mapIDsByShortName.Add(zone.ShortName.ToLower().Trim(), zone.ZoneProperties.DBCMapID);

            // Graveyards
            foreach (ZonePropertiesGraveyard graveyard in ZonePropertiesGraveyard.GetAllGraveyards())
            {
                // Should be one for each ghost zone
                foreach (string zoneShortName in graveyard.GhostZoneShortNames)
                {
                    string zoneShortNameLower = zoneShortName.ToLower();
                    if (zonePropertiesByShortName.ContainsKey(zoneShortNameLower) == true)
                    {
                        int ghostZoneAreaID = Convert.ToInt32(ZoneProperties.GetZonePropertiesForZone(zoneShortNameLower).DefaultZoneArea.DBCAreaTableID);
                        graveyardZoneSQL.AddRow(graveyard, ghostZoneAreaID);
                    }
                }

                if (zonePropertiesByShortName.ContainsKey(graveyard.LocationShortName) == true)
                {
                    ZoneProperties curZoneProperties = ZoneProperties.GetZonePropertiesForZone(graveyard.LocationShortName);
                    int mapID = curZoneProperties.DBCMapID;
                    gameGraveyardSQL.AddRow(graveyard, mapID);

                    // And there should be one spirit healer per graveyard
                    int spiritHealerGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    int zoneAreaID = Convert.ToInt32(curZoneProperties.DefaultZoneArea.DBCAreaTableID);
                    creatureSQL.AddRow(spiritHealerGUID, Configuration.ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID, mapID, zoneAreaID, zoneAreaID,
                        graveyard.SpiritHealerX, graveyard.SpiritHealerY, graveyard.SpiritHealerZ, graveyard.SpiritHealerOrientation, CreatureMovementType.None, 0, true, true, string.Empty);
                }
            }

            // Game Objects
            if (Configuration.GENERATE_OBJECTS == true)
            {
                Dictionary<string, List<GameObject>> gameObjectsByZoneShortNames = GameObject.GetNonDoodadGameObjectsByZoneShortNames();
                foreach (var gameObjectByShortName in gameObjectsByZoneShortNames)
                {
                    // Skip invalid objects (zones not loaded)
                    if (zonePropertiesByShortName.ContainsKey(gameObjectByShortName.Key) == false)
                        continue;
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == gameObjectByShortName.Key)
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    foreach (GameObject gameObject in gameObjectByShortName.Value)
                    {
                        string comment = string.Concat("EQ ", gameObject.ObjectType.ToString(), " ", gameObject.ZoneShortName, " (", gameObject.ID, ")");
                        string name = gameObject.DisplayName;
                        if (name.Length == 0)
                            name = comment;
                        int mapID = mapIDsByShortName[gameObjectByShortName.Key];
                        gameObjectSQL.AddRow(gameObject.GameObjectGUID, gameObject.GameObjectTemplateEntryID, mapID, areaID, gameObject.Position, gameObject.Orientation, comment);
                        gameObjectTemplateSQL.AddRowForGameObject(name, gameObject);
                        gameObjectTemplateAddonSQL.AddRowNoDespawn(gameObject.GameObjectTemplateEntryID);

                        // Attach any smart scripts
                        if (gameObject.TriggerGameObjectGUID != 0)
                        {
                            string scriptComment = string.Concat("EQ GameObject GUID ", gameObject.GameObjectGUID, " Chain Activates GUID ", gameObject.TriggerGameObjectGUID);
                            smartScriptsSQL.AddRowForGameObjectStateTriggerEvent(gameObject.GameObjectTemplateEntryID, gameObject.TriggerGameObjectGUID, gameObject.TriggerGameObjectTemplateEntryID, scriptComment);
                        }
                    }
                }
            }
        }

        private void OutputSQLScriptsToDisk()
        {
            // Characters
            characterAuraSQL.SaveToDisk("character_aura", SQLFileType.Characters);
            modEverquestCharacterHomebindSQL.SaveToDisk("mod_everquest_character_homebind", SQLFileType.Characters);
            // World
            areaTriggerSQL.SaveToDisk("areatrigger", SQLFileType.World);
            areaTriggerTeleportSQL.SaveToDisk("areatrigger_teleport", SQLFileType.World);
            broadcastTextSQL.SaveToDisk("broadcast_text", SQLFileType.World);
            creatureSQL.SaveToDisk("creature", SQLFileType.World);
            creatureAddonSQL.SaveToDisk("creature_addon", SQLFileType.World);
            creatureLootTableSQL.SaveToDisk("creature_loot_template", SQLFileType.World);
            creatureModelInfoSQL.SaveToDisk("creature_model_info", SQLFileType.World);
            creatureTemplateSQL.SaveToDisk("creature_template", SQLFileType.World);
            creatureTemplateModelSQL.SaveToDisk("creature_template_model", SQLFileType.World);
            creatureTemplateSpellSQL.SaveToDisk("creature_template_spell", SQLFileType.World);
            creatureTextSQL.SaveToDisk("creature_text", SQLFileType.World);
            gameEventSQL.SaveToDisk("game_event", SQLFileType.World);
            gameGraveyardSQL.SaveToDisk("game_graveyard", SQLFileType.World);
            gameObjectSQL.SaveToDisk("gameobject", SQLFileType.World);
            gameObjectTemplateSQL.SaveToDisk("gameobject_template", SQLFileType.World);
            gameObjectTemplateAddonSQL.SaveToDisk("gameobject_template_addon", SQLFileType.World);
            gameTeleSQL.SaveToDisk("game_tele", SQLFileType.World);
            gameWeatherSQL.SaveToDisk("game_weather", SQLFileType.World);
            gossipMenuSQL.SaveToDisk("gossip_menu", SQLFileType.World);
            gossipMenuOptionSQL.SaveToDisk("gossip_menu_option", SQLFileType.World);
            graveyardZoneSQL.SaveToDisk("graveyard_zone", SQLFileType.World);
            instanceTemplateSQL.SaveToDisk("instance_template", SQLFileType.World);
            itemLootTemplateSQL.SaveToDisk("item_loot_template", SQLFileType.World);
            itemTemplateSQL.SaveToDisk("item_template", SQLFileType.World);
            modEverquestCreatureOnkillReputationSQL.SaveToDisk("mod_everquest_creature_onkill_reputation", SQLFileType.World);
            modEverquestPetSQL.SaveToDisk("mod_everquest_pet", SQLFileType.World);
            modEverquestSpellSQL.SaveToDisk("mod_everquest_spell", SQLFileType.World);
            modEverquestSystemConfigsSQL.SaveToDisk("mod_everquest_systemconfigs", SQLFileType.World);
            modEverquestQuestCompleteReputationSQL.SaveToDisk("mod_everquest_quest_complete_reputation", SQLFileType.World);
            npcTextSQL.SaveToDisk("npc_text", SQLFileType.World);
            npcTrainerSQL.SaveToDisk("npc_trainer", SQLFileType.World);
            npcVendorSQL.SaveToDisk("npc_vendor", SQLFileType.World);
            playerCreateInfoSQL.SaveToDisk("playercreateinfo", SQLFileType.World);
            playerCreateInfoSpellCustomSQL.SaveToDisk("playercreateinfo_spell_custom", SQLFileType.World);
            poolCreatureSQL.SaveToDisk("pool_creature", SQLFileType.World);
            poolPoolSQL.SaveToDisk("pool_pool", SQLFileType.World);
            poolTemplateSQL.SaveToDisk("pool_template", SQLFileType.World);
            smartScriptsSQL.SaveToDisk("smart_scripts", SQLFileType.World);
            spellGroupSQL.SaveToDisk("spell_group", SQLFileType.World);
            spellGroupStackRulesSQL.SaveToDisk("spell_group_stack_rules", SQLFileType.World);
            spellLinkedSpellSQL.SaveToDisk("spell_linked_spell", SQLFileType.World);
            spellScriptNamesSQL.SaveToDisk("spell_script_names", SQLFileType.World);
            spellTargetPositionSQL.SaveToDisk("spell_target_position", SQLFileType.World);
            transportsSQL.SaveToDisk("transports", SQLFileType.World);
            waypointDataSQL.SaveToDisk("waypoint_data", SQLFileType.World);
            if (Configuration.GENERATE_QUESTS == true)
            {
                creatureQuestEnderSQL.SaveToDisk("creature_questender", SQLFileType.World);
                creatureQuestStarterSQL.SaveToDisk("creature_queststarter", SQLFileType.World);
                questTemplateSQL.SaveToDisk("quest_template", SQLFileType.World);
                questTemplateAddonSQL.SaveToDisk("quest_template_addon", SQLFileType.World);
            }
        }

        public void DeployServerSQL()
        {
            Logger.WriteInfo("Deploying sql to server...");

            // Deploy all of the scripts
            try
            {
                // Character scripts
                string charactersSQLScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", SQLFileType.Characters.ToString());
                if (Directory.Exists(charactersSQLScriptFolder) == false)
                {
                    Logger.WriteError("Could not deploy SQL scripts to server. Path '" + charactersSQLScriptFolder + "' did not exist");
                    return;
                }
                using (MySqlConnection connection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_CHARACTERS))
                {
                    connection.Open();
                    string[] sqlFiles = Directory.GetFiles(charactersSQLScriptFolder);
                    foreach (string sqlFile in sqlFiles)
                    {
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        command.CommandTimeout = 288000;
                        command.CommandText = FileTool.ReadAllDataFromFile(sqlFile);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                // World scripts
                string worldSQLScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", SQLFileType.World.ToString());
                if (Directory.Exists(worldSQLScriptFolder) == false)
                {
                    Logger.WriteError("Could not deploy SQL scripts to server. Path '" + worldSQLScriptFolder + "' did not exist");
                    return;
                }
                using (MySqlConnection connection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_WORLD))
                {
                    connection.Open();
                    string[] sqlFiles = Directory.GetFiles(worldSQLScriptFolder);
                    foreach (string sqlFile in sqlFiles)
                    {
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = connection;
                        command.CommandTimeout = 288000;
                        command.CommandText = FileTool.ReadAllDataFromFile(sqlFile);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error occurred when executing a script: '" + ex.Message + "'");
                if (ex.StackTrace != null)
                    Logger.WriteDebug(ex.StackTrace.ToString());
                Logger.WriteError("Deploying sql to server failed.");
            }

            Logger.WriteDebug("Deploying sql to server complete");
        }
    }
}
