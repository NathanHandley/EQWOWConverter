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
using EQWOWConverter.Creatures.Teleporters;
using EQWOWConverter.Events;
using EQWOWConverter.Fishing;
using EQWOWConverter.Forage;
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
        // World
        private AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
        private AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();
        private BroadcastTextSQL broadcastTextSQL = new BroadcastTextSQL();
        private ConditionsSQL conditionsSQL = new ConditionsSQL();
        private CreatureSQL creatureSQL = new CreatureSQL();
        private CreatureAddonSQL creatureAddonSQL = new CreatureAddonSQL();
        private CreatureDefaultTrainerSQL creatureDefaultTrainerSQL = new CreatureDefaultTrainerSQL();
        private CreatureEquipTemplateSQL creatureEquipTemplateSQL = new CreatureEquipTemplateSQL();
        private CreatureLootTemplateSQL creatureLootTableSQL = new CreatureLootTemplateSQL();
        private CreatureModelInfoSQL creatureModelInfoSQL = new CreatureModelInfoSQL();
        private CreatureQuestEnderSQL creatureQuestEnderSQL = new CreatureQuestEnderSQL();
        private CreatureQuestStarterSQL creatureQuestStarterSQL = new CreatureQuestStarterSQL();
        private CreatureTemplateSQL creatureTemplateSQL = new CreatureTemplateSQL();
        private CreatureTemplateModelSQL creatureTemplateModelSQL = new CreatureTemplateModelSQL();
        private CreatureTemplateSpellSQL creatureTemplateSpellSQL = new CreatureTemplateSpellSQL();
        private CreatureTextSQL creatureTextSQL = new CreatureTextSQL();
        private FishingLootTemplateSQL fishingLootTemplateSQL = new FishingLootTemplateSQL();
        private GameEventSQL gameEventSQL = new GameEventSQL();
        private GameEventCreatureSQL gameEventCreatureSQL = new GameEventCreatureSQL();
        private GameEventPoolSQL gameEventPoolSQL = new GameEventPoolSQL();
        private GameGraveyardSQL gameGraveyardSQL = new GameGraveyardSQL();
        private GameTeleSQL gameTeleSQL = new GameTeleSQL();
        private GameWeatherSQL gameWeatherSQL = new GameWeatherSQL();
        private GossipMenuSQL gossipMenuSQL = new GossipMenuSQL();
        private GossipMenuOptionSQL gossipMenuOptionSQL = new GossipMenuOptionSQL();
        private GraveyardZoneSQL graveyardZoneSQL = new GraveyardZoneSQL();
        private GameObjectSQL gameObjectSQL = new GameObjectSQL();
        private GameObjectAddonSQL gameObjectAddonSQL = new GameObjectAddonSQL();
        private GameObjectLootTemplateSQL gameObjectLootTemplateSQL = new GameObjectLootTemplateSQL();
        private GameObjectTemplateSQL gameObjectTemplateSQL = new GameObjectTemplateSQL();
        private GameObjectTemplateAddonSQL gameObjectTemplateAddonSQL = new GameObjectTemplateAddonSQL();
        private InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
        private ItemLootTemplateSQL itemLootTemplateSQL = new ItemLootTemplateSQL();
        private ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
        private ModEverquestCreatureSQL modEverquestCreatureSQL = new ModEverquestCreatureSQL();
        private ModEverquestCreatureInstanceSQL modEverquestCreatureInstanceSQL = new ModEverquestCreatureInstanceSQL();
        private ModEverquestCreatureOnkillReputationSQL modEverquestCreatureOnkillReputationSQL = new ModEverquestCreatureOnkillReputationSQL();
        private ModEverquestCreatureWaypointSQL modEverquestCreatureWaypointSQL = new ModEverquestCreatureWaypointSQL();
        private ModEverquestForageZoneItemsSQL modEverquestForageZoneItemsSQL = new ModEverquestForageZoneItemsSQL();
        private ModEverquestItemTemplateSQL modEverquestItemTemplateSQL = new ModEverquestItemTemplateSQL();
        private ModEverquestPetSQL modEverquestPetSQL = new ModEverquestPetSQL();
        private ModEverquestPlayerCreateInfoSQL modEverquestPlayerCreateInfoSQL = new ModEverquestPlayerCreateInfoSQL();
        private ModEverquestPlayerAutoLearnSkillsSQL modEverquestPlayerAutoLearnSkillsSQL = new ModEverquestPlayerAutoLearnSkillsSQL();
        private ModEverquestPlayerAutoLearnSpellsSQL modEverquestPlayerAutoLearnSpellsSQL = new ModEverquestPlayerAutoLearnSpellsSQL();
        private ModEverquestSpellSQL modEverquestSpellSQL = new ModEverquestSpellSQL();
        private ModEverquestSystemConfigsSQL modEverquestSystemConfigsSQL = new ModEverquestSystemConfigsSQL();
        private ModEverquestTransportTriggerSQL modEverquestTransportTriggerSQL = new ModEverquestTransportTriggerSQL();
        private ModEverquestQuestCompleteReputationSQL modEverquestQuestCompleteReputationSQL = new ModEverquestQuestCompleteReputationSQL();
        private ModEverquestQuestReactionSQL modEverquestQuestReactionSQL = new ModEverquestQuestReactionSQL();
        private NPCTextSQL npcTextSQL = new NPCTextSQL();
        private NPCVendorSQL npcVendorSQL = new NPCVendorSQL();
        private PageTextSQL pageTextSQL = new PageTextSQL();
        private PetNameGenerationSQL petNameGenerationSQL = new PetNameGenerationSQL();
        private PlayerClassStatsSQL playerClassStatsSQL = new PlayerClassStatsSQL();
        private PlayerCreateInfoSpellCustomSQL playerCreateInfoSpellCustomSQL = new PlayerCreateInfoSpellCustomSQL();
        private PoolCreatureSQL poolCreatureSQL = new PoolCreatureSQL();
        private PoolPoolSQL poolPoolSQL = new PoolPoolSQL(); // Consider delete
        private PoolTemplateSQL poolTemplateSQL = new PoolTemplateSQL();
        private QuestTemplateSQL questTemplateSQL = new QuestTemplateSQL();
        private QuestTemplateAddonSQL questTemplateAddonSQL = new QuestTemplateAddonSQL();
        private ReferenceLootTemplateSQL referenceLootTemplateSQL = new ReferenceLootTemplateSQL();
        private SkillFishingBaseLevelSQL skillFishingBaseLevelSQL = new SkillFishingBaseLevelSQL();
        private SmartScriptsSQL smartScriptsSQL = new SmartScriptsSQL();
        private SpellBonusDataSQL spellBonusDataSQL = new SpellBonusDataSQL();
        private SpellGroupSQL spellGroupSQL = new SpellGroupSQL();
        private SpellGroupStackRulesSQL spellGroupStackRulesSQL = new SpellGroupStackRulesSQL();
        private SpellLinkedSpellSQL spellLinkedSpellSQL = new SpellLinkedSpellSQL();
        private SpellScriptNamesSQL spellScriptNamesSQL = new SpellScriptNamesSQL();
        private SpellTargetPositionSQL spellTargetPositionSQL = new SpellTargetPositionSQL();
        private TrainerSQL trainerSQL = new TrainerSQL();
        private TrainerSpellSQL trainerSpellSQL = new TrainerSpellSQL();
        private TransportsSQL transportsSQL = new TransportsSQL();
        private WaypointDataSQL waypointDataSQL = new WaypointDataSQL();

        public void CreateSQLScripts(List<Zone> zones, List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID, List<QuestTemplate> questTemplates,
            List<TradeskillRecipe> tradeskillRecipes, List<SpellTemplate> spellTemplates, List<GameEvent> gameEvents)
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

            // Trainer Abilities (Class and Profession)
            PopulateTrainerData(creatureTemplates);

            // Creatures
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
            PopulateCreatureData(creatureTemplates, creatureModelTemplates, creatureSpawnPools, spellTemplatesByEQID, mapIDsByShortName);

            // Game Events
            foreach (GameEvent gameEvent in gameEvents)
                gameEventSQL.AddRow(gameEvent);

            // Items
            PopulateItemData(itemLootTemplatesByCreatureTemplateID, spellTemplatesByEQID);

            // Forage
            PopulateForageData();

            // Fishing
            PopulateFishingData(zonePropertiesByShortName);

            // Player properties
            PopulatePlayerData(zones, mapIDsByShortName);

            // Quests
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            PopulateQuestData(questTemplates, itemTemplatesByWOWEntryID);

            // Spells
            PopulateSpellAndTradeskillData(spellTemplates, tradeskillRecipes, itemTemplatesByWOWEntryID);

            // Transports
            if (Configuration.GENERATE_TRANSPORTS == true)
                PopulateTransportSQLData(zones, mapIDsByShortName);

            // Output them
            OutputSQLScriptsToDisk();
        }

        private void PopulateSystemConfigs()
        {
            modEverquestSystemConfigsSQL.AddRow("ModVersion", Configuration.CORE_MOD_VERSION.ToString());
            modEverquestSystemConfigsSQL.AddRow("BardMaxConcurrentSongs", Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS.ToString());
            modEverquestSystemConfigsSQL.AddRow("CreatureTemplateIDMin", Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString());
            modEverquestSystemConfigsSQL.AddRow("CreatureTemplateIDMax", Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH.ToString());
            modEverquestSystemConfigsSQL.AddRow("DeathKnightsStartLikeOtherClasses", Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true ? "1" : "0");
            modEverquestSystemConfigsSQL.AddRow("GameObjectTemplateIDMin", Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("GameObjectTemplateIDMax", Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMin", Configuration.DBCID_MAP_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMax", Configuration.DBCID_MAP_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("ShipEntryTemplateIDMin", Configuration.SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("ShipEntryTemplateIDMax", Configuration.SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMin", Configuration.DBCID_SPELL_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMax", Configuration.DBCID_SPELL_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMin", Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMax", Configuration.SQL_QUEST_TEMPLATE_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("WorldScale", Configuration.GENERATE_WORLD_SCALE.ToString());
        }

        private void PopulateCreatureData(List<CreatureTemplate> creatureTemplates, List<CreatureModelTemplate> creatureModelTemplates,
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, SpellTemplate> spellTemplatesByEQID, Dictionary<string, int> mapIDsByShortName)
        {
            // Creature and Creature Spawn Pools
            foreach (CreatureSpawnPool spawnPool in creatureSpawnPools)
            {
                bool isSingleInstance = spawnPool.CreatureSpawnInstances.Count == 1;
                bool isSingleCreatureTemplate = spawnPool.CreatureTemplates.Count == 1;

                // No pool needed, single instance
                if (isSingleInstance == true && isSingleCreatureTemplate == true)
                {
                    CreatureTemplate creatureTemplate = spawnPool.CreatureTemplates[0];
                    CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[0];
                    int creatureSQLGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.SpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                    CreateCreatureAndRelatedSQLEntries(creatureSQLGUID, creatureTemplate, spawnInstance, spawnPool.SpawnGroup, comment);
                    if (spawnPool.LinkedSpawnGameEvent != null)
                        gameEventCreatureSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, creatureSQLGUID, true);
                    if (spawnPool.LinkedDespawnGameEvent != null)
                        gameEventCreatureSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, creatureSQLGUID, false);
                }

                // Pool required (more than one spawn point and/or more than one candidate creature)
                else
                {
                    List<string> poolNames = new List<string>();
                    foreach (CreatureTemplate template in spawnPool.CreatureTemplates)
                        if (poolNames.Contains(template.Name) == false)
                            poolNames.Add(template.Name);
                    string poolDescription = "(" + spawnPool.SpawnGroup.ID + ")";
                    foreach (string name in poolNames)
                        poolDescription += ", " + name;

                    // Identify which spawn pools need a real cap
                    bool hasRealSpawnCap = spawnPool.SpawnLimit > 0 && spawnPool.SpawnLimit < spawnPool.CreatureSpawnInstances.Count;

                    if (hasRealSpawnCap == true)
                    {
                        int poolID = CreatureSpawnPool.GetPoolTemplateSQLID();
                        poolTemplateSQL.AddRow(poolID, poolDescription, spawnPool.SpawnLimit);
                        if (spawnPool.LinkedSpawnGameEvent != null)
                            gameEventPoolSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, poolID, true);
                        if (spawnPool.LinkedDespawnGameEvent != null)
                            gameEventPoolSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, poolID, false);

                        // Trick the chance system to enforce a cap
                        List<CreatureTemplate> pointTemplates = DistributeCandidatesAcrossSpawnPoints(
                            spawnPool.CreatureTemplates, spawnPool.CreatureTemplateChances, spawnPool.CreatureSpawnInstances.Count);
                        for (int spawnInstanceIndex = 0; spawnInstanceIndex < spawnPool.CreatureSpawnInstances.Count; spawnInstanceIndex++)
                        {
                            CreatureSpawnInstance spawnInstance = spawnPool.CreatureSpawnInstances[spawnInstanceIndex];
                            CreatureTemplate creatureTemplate = pointTemplates[spawnInstanceIndex];
                            int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                            poolCreatureSQL.AddRow(creatureGUID, poolID, 0, creatureTemplate.Name);
                            string comment = string.Concat(creatureTemplate.Name, " - EQ Group: ", spawnPool.SpawnGroup.ID, ", EQ NPC ID: ", creatureTemplate.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                            CreateCreatureAndRelatedSQLEntries(creatureGUID, creatureTemplate, spawnInstance, spawnPool.SpawnGroup, comment);
                        }
                    }
                    // If there is no cap, have one weighted pool per spawn point
                    else if (isSingleCreatureTemplate == true)
                    {
                        CreatureTemplate template = spawnPool.CreatureTemplates[0];
                        foreach (CreatureSpawnInstance spawnInstance in spawnPool.CreatureSpawnInstances)
                        {
                            int creatureSQLGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                            string comment = string.Concat(template.Name, " - EQ Group: ", spawnPool.SpawnGroup.ID, ", EQ NPC ID: ", template.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                            CreateCreatureAndRelatedSQLEntries(creatureSQLGUID, template, spawnInstance, spawnPool.SpawnGroup, comment);
                            if (spawnPool.LinkedSpawnGameEvent != null)
                                gameEventCreatureSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, creatureSQLGUID, true);
                            if (spawnPool.LinkedDespawnGameEvent != null)
                                gameEventCreatureSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, creatureSQLGUID, false);
                        }
                    }
                    // Multiple candidate creatures per spawn point, so use one pool per spawn with full weighted list
                    else
                    {
                        foreach (CreatureSpawnInstance spawnInstance in spawnPool.CreatureSpawnInstances)
                        {
                            int poolID = CreatureSpawnPool.GetPoolTemplateSQLID();
                            poolTemplateSQL.AddRow(poolID, poolDescription, 1);
                            if (spawnPool.LinkedSpawnGameEvent != null)
                                gameEventPoolSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, poolID, true);
                            if (spawnPool.LinkedDespawnGameEvent != null)
                                gameEventPoolSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, poolID, false);
                            for (int i = 0; i < spawnPool.CreatureTemplates.Count; i++)
                            {
                                CreatureTemplate template = spawnPool.CreatureTemplates[i];
                                int chance = spawnPool.CreatureTemplateChances[i];
                                int guid = CreatureTemplate.GenerateCreatureSQLGUID();
                                poolCreatureSQL.AddRow(guid, poolID, chance, template.Name);
                                string comment = string.Concat(template.Name, " - EQ Group: ", spawnPool.SpawnGroup.ID, ", EQ NPC ID: ", template.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                                CreateCreatureAndRelatedSQLEntries(guid, template, spawnInstance, spawnPool.SpawnGroup, comment);
                            }
                        }
                    }
                }
            }

            // Priest of Discord teleportation
            List<CreatureTeleportLocationAzeroth> azerothTeleportLocations;
            List<CreatureTeleportLocationNorrath> norrathTeleportLocations;
            int norrathPriestOfDiscordGossipMenuID = -1;
            int azerothPriestOfDiscordGossipMenuID = -1;
            int priestOfDiscordCooldownMenuNPCTextID = -1;
            SortedDictionary<int, CreatureTeleportLocationAzeroth> azerothTeleportLocationsByGossipMenuOptionID = new SortedDictionary<int, CreatureTeleportLocationAzeroth>();
            SortedDictionary<int, CreatureTeleportLocationNorrath> norrathTeleportLocationsByGossipMenuOptionID = new SortedDictionary<int, CreatureTeleportLocationNorrath>();
            if (Configuration.GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION == true)
            {
                // Cooldown menu text
                int menuBroadcastTextID = BroadcastTextSQL.GenerateUniqueID();
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT);
                int menuNPCTextID = NPCTextSQL.GenerateUniqueID();
                priestOfDiscordCooldownMenuNPCTextID = menuNPCTextID;
                npcTextSQL.AddRow(menuNPCTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT, menuBroadcastTextID);

                // Teleporting to Azeroth
                azerothTeleportLocations = CreatureTeleportLocationAzeroth.GetAllTeleportLocations();
                norrathPriestOfDiscordGossipMenuID = GossipMenuSQL.GenerateUniqueMenuID();
                menuBroadcastTextID = BroadcastTextSQL.GenerateUniqueID();
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT);
                menuNPCTextID = NPCTextSQL.GenerateUniqueID();
                npcTextSQL.AddRow(menuNPCTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT, menuBroadcastTextID);
                gossipMenuSQL.AddRow(norrathPriestOfDiscordGossipMenuID, menuNPCTextID);
                gossipMenuSQL.AddRow(norrathPriestOfDiscordGossipMenuID, priestOfDiscordCooldownMenuNPCTextID);
                if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                {
                    string menuConditionComment = string.Concat("Show Priest of Discord menu if player does not have spell aura ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                    conditionsSQL.AddRowForMenuRestrictionIfAura(norrathPriestOfDiscordGossipMenuID, menuNPCTextID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, menuConditionComment, true);
                    menuConditionComment = string.Concat("Show Priest of Discord cooldown menu if player does have spell aura ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                    conditionsSQL.AddRowForMenuRestrictionIfAura(norrathPriestOfDiscordGossipMenuID, priestOfDiscordCooldownMenuNPCTextID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, menuConditionComment, false);
                }

                int curMenuOptionID = 0;
                foreach (CreatureTeleportLocationAzeroth teleportLocation in azerothTeleportLocations)
                {
                    // Broadcast
                    int menuBroadcastID = BroadcastTextSQL.GenerateUniqueID();
                    broadcastTextSQL.AddRow(menuBroadcastID, teleportLocation.MenuItemText, teleportLocation.MenuItemText);

                    // Menu Option
                    gossipMenuOptionSQL.AddRow(norrathPriestOfDiscordGossipMenuID, curMenuOptionID, 0, teleportLocation.MenuItemText, menuBroadcastID, 1, 1, 0);

                    // Condition
                    string conditionsComment = string.Concat("Restrict menu option for race ", teleportLocation.Race.ToString());
                    conditionsSQL.AddRowForMenuOptionRaceRestriction(norrathPriestOfDiscordGossipMenuID, curMenuOptionID, new List<RaceType>() { teleportLocation.Race }, conditionsComment);
                    if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                    {
                        conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                        conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(norrathPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment);
                    }

                    azerothTeleportLocationsByGossipMenuOptionID.Add(curMenuOptionID, teleportLocation);
                    curMenuOptionID++;
                }

                // Teleporting to Norrath
                norrathTeleportLocations = CreatureTeleportLocationNorrath.GetAllTeleportLocations();
                azerothPriestOfDiscordGossipMenuID = GossipMenuSQL.GenerateUniqueMenuID();
                menuBroadcastTextID = BroadcastTextSQL.GenerateUniqueID();
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT);
                menuNPCTextID = NPCTextSQL.GenerateUniqueID();
                npcTextSQL.AddRow(menuNPCTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT, menuBroadcastTextID);
                gossipMenuSQL.AddRow(azerothPriestOfDiscordGossipMenuID, menuNPCTextID);
                gossipMenuSQL.AddRow(azerothPriestOfDiscordGossipMenuID, priestOfDiscordCooldownMenuNPCTextID);
                if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                {
                    string menuConditionComment = string.Concat("Show Priest of Discord menu if player does not have spell aura ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                    conditionsSQL.AddRowForMenuRestrictionIfAura(azerothPriestOfDiscordGossipMenuID, menuNPCTextID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, menuConditionComment, true);
                    menuConditionComment = string.Concat("Show Priest of Discord cooldown menu if player does have spell aura ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                    conditionsSQL.AddRowForMenuRestrictionIfAura(azerothPriestOfDiscordGossipMenuID, priestOfDiscordCooldownMenuNPCTextID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, menuConditionComment, false);
                }

                curMenuOptionID = 0;
                foreach (CreatureTeleportLocationNorrath teleportLocation in norrathTeleportLocations)
                {
                    // Broadcast
                    int menuBroadcastID = BroadcastTextSQL.GenerateUniqueID();
                    broadcastTextSQL.AddRow(menuBroadcastID, teleportLocation.MenuItemText, teleportLocation.MenuItemText);

                    // Menu Option
                    gossipMenuOptionSQL.AddRow(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, 0, teleportLocation.MenuItemText, menuBroadcastID, 1, 1, 0);

                    // Option conditionals, which is based on alignment                   
                    int curElseGroup = 0;

                    // Good
                    if (teleportLocation.AllowGood)
                    {
                        // Class is Good
                        conditionsSQL.AddRowForMenuOptionClassRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetGoodClasses(),
                            "Good alignment (Class Good)", curElseGroup);
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                            conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment, curElseGroup);
                        }
                        curElseGroup++;

                        // Class NOT Evil AND Race is Good
                        conditionsSQL.AddRowForMenuOptionClassRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetEvilClasses(),
                            "Good alignment (Class NOT Evil)", curElseGroup, true);
                        conditionsSQL.AddRowForMenuOptionRaceRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetGoodRaces(),
                            "Good alignment (Race Good)", curElseGroup);
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                            conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment, curElseGroup);
                        }
                        curElseGroup++;
                    }

                    // Evil
                    if (teleportLocation.AllowEvil)
                    {
                        // Class is Evil
                        conditionsSQL.AddRowForMenuOptionClassRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetEvilClasses(),
                            "Evil alignment (Class Evil)", curElseGroup);
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                            conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment, curElseGroup);
                        }
                        curElseGroup++;

                        // Class NOT Good AND Race is Evil
                        conditionsSQL.AddRowForMenuOptionClassRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetGoodClasses(),
                            "Evil alignment (Class NOT Good)", curElseGroup, true);
                        conditionsSQL.AddRowForMenuOptionRaceRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetEvilRaces(),
                            "Evil alignment (Race Evil)", curElseGroup);
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                            conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment, curElseGroup);
                        }
                        curElseGroup++;
                    }

                    // Neutral
                    if (teleportLocation.AllowNeutral)
                    {
                        // Class Neutral AND Race Neutral
                        conditionsSQL.AddRowForMenuOptionClassRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, CreatureTeleportLocationNorrath.GetNeutralClasses(),
                            "Neutral alignment (Class Neutral)", curElseGroup);
                        conditionsSQL.AddRowForMenuOptionRaceRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID,
                            CreatureTeleportLocationNorrath.GetNeutralRaces(), "Neutral alignment (Race Neutral)", curElseGroup);
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string conditionsComment = string.Concat("Restrict menu option for spell ", Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID.ToString());
                            conditionsSQL.AddRowForMenuOptionAuraExistsRestriction(azerothPriestOfDiscordGossipMenuID, curMenuOptionID, Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, conditionsComment, curElseGroup);
                        }
                    }

                    norrathTeleportLocationsByGossipMenuOptionID.Add(curMenuOptionID, teleportLocation);
                    curMenuOptionID++;
                }
            }

            // Creature Templates
            Dictionary<int, List<CreatureVendorItem>> vendorItems = CreatureVendorItem.GetCreatureVendorItemsByMerchantIDs();
            List<CreatureVendorItem> reagentVendorItems = CreatureVendorItem.GetCreatureReagentItems();
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                // Skip invalid creatures
                if (creatureTemplate.ModelTemplate == null)
                {
                    Logger.WriteError("Error generating azeroth core scripts since model template was null for creature template '" + creatureTemplate.Name + "'");
                    continue;
                }

                // Simplified logic for debug mode
                if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true && creatureTemplate.IsWaypointDebugCreature == true)
                {
                    creatureTemplateSQL.AddRow(creatureTemplate);
                    creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureTemplate.ModelTemplate.DBCCreatureDisplayID, 1f);
                    int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                    string comment = string.Concat(creatureTemplate.Name, " - EQ Debug Creature");
                    creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, creatureTemplate.SpawnWaypointDebugMapID, creatureTemplate.SpawnWaypointDebugAreaID,
                        creatureTemplate.SpawnWaypointDebugAreaID, creatureTemplate.SpawnWaypointDebugXPosition, creatureTemplate.SpawnWaypointDebugYPosition,
                        creatureTemplate.SpawnWaypointDebugZPosition, 0, CreatureMovementType.None, comment, false);
                    continue;
                }

                // If there are any Azeroth teleports
                if (Configuration.GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION == true && creatureTemplate.IsNorrathPriestOfDiscord == true)
                {
                    creatureTemplate.HasSmartScript = true;
                    creatureTemplate.GossipMenuID = norrathPriestOfDiscordGossipMenuID;
                    creatureTemplate.WOWFactionTemplateID = Configuration.CREATURE_FACTION_TEMPLATE_NEUTRAL_INTERACTIVE;
                    foreach (var azerothTeleportLocationByGossipMenuOptionID in azerothTeleportLocationsByGossipMenuOptionID)
                    {
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string spellComment = string.Concat("EQ Apply Azeroth-Norrath Teleport Cooldown Aura");
                            smartScriptsSQL.AddRowForMenuOptionTriggeredAura(creatureTemplate.WOWCreatureTemplateID, norrathPriestOfDiscordGossipMenuID, azerothTeleportLocationByGossipMenuOptionID.Key,
                                Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, spellComment);
                        }
                        CreatureTeleportLocationAzeroth curTeleportLocation = azerothTeleportLocationByGossipMenuOptionID.Value;
                        string comment = string.Concat("EQ teleport player to Azeroth ('", curTeleportLocation.MenuItemText, "')");
                        smartScriptsSQL.AddRowForMenuOptionTriggeredTeleport(creatureTemplate.WOWCreatureTemplateID, norrathPriestOfDiscordGossipMenuID, azerothTeleportLocationByGossipMenuOptionID.Key,
                            curTeleportLocation.MapID, curTeleportLocation.XPosition, curTeleportLocation.YPosition, curTeleportLocation.ZPosition,
                            curTeleportLocation.Orientation, comment);
                    }
                }

                // If there are any Norrath teleports
                if (Configuration.GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION == true && creatureTemplate.IsAzerothPriestOfDiscord == true)
                {
                    creatureTemplate.HasSmartScript = true;
                    creatureTemplate.GossipMenuID = azerothPriestOfDiscordGossipMenuID;
                    foreach (var norrathTeleportLocationByGossipMenuOptionID in norrathTeleportLocationsByGossipMenuOptionID)
                    {
                        if (Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_DURATION_IN_MIN > 0)
                        {
                            string spellComment = string.Concat("EQ Apply Azeroth-Norrath Teleport Cooldown Aura");
                            smartScriptsSQL.AddRowForMenuOptionTriggeredAura(creatureTemplate.WOWCreatureTemplateID, azerothPriestOfDiscordGossipMenuID, norrathTeleportLocationByGossipMenuOptionID.Key,
                                Configuration.SPELL_PRIEST_OF_DISCORD_PORTAL_COOLDOWN_SPELL_ID, spellComment);
                        }
                        CreatureTeleportLocationNorrath curTeleportLocation = norrathTeleportLocationByGossipMenuOptionID.Value;
                        if (mapIDsByShortName.ContainsKey(curTeleportLocation.ZoneShortName) == false)
                            continue;
                        int mapID = mapIDsByShortName[curTeleportLocation.ZoneShortName];
                        string comment = string.Concat("EQ teleport player to Norrath ('", curTeleportLocation.MenuItemText, "')");
                        smartScriptsSQL.AddRowForMenuOptionTriggeredTeleport(creatureTemplate.WOWCreatureTemplateID, azerothPriestOfDiscordGossipMenuID, norrathTeleportLocationByGossipMenuOptionID.Key,
                            mapID, curTeleportLocation.XPosition, curTeleportLocation.YPosition, curTeleportLocation.ZPosition, curTeleportLocation.Orientation, comment);
                    }
                }

                // All creature data
                modEverquestCreatureSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureTemplate.Race.CanHoldVisualItems, creatureTemplate.Race.CanHoldVisualShields);

                // Determine the display id
                int displayID = creatureTemplate.ModelTemplate.DBCCreatureDisplayID;
                if (creatureTemplate.IsNonNPC == true)
                    displayID = 11686; // Dranei totem

                // Create the records
                float scale = creatureTemplate.Size * creatureTemplate.Race.SpawnSizeMod;
                creatureTemplateSQL.AddRow(creatureTemplate);
                creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, displayID, scale);

                // If it's a vendor, add the vendor records too
                if ((creatureTemplate.MerchantID != 0 && vendorItems.ContainsKey(creatureTemplate.MerchantID)) || creatureTemplate.IsReagentVendor == true)
                {
                    int curSlotNum = 0;
                    // General vendors
                    if (creatureTemplate.MerchantID != 0 && vendorItems.ContainsKey(creatureTemplate.MerchantID))
                    {
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
                    // Reagent vendors
                    if (creatureTemplate.EQClass == 103)
                    {
                        foreach (CreatureVendorItem vendorItem in reagentVendorItems)
                        {
                            npcVendorSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, vendorItem.WOWItemID, curSlotNum);
                            curSlotNum++;
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

                    // Add spell events for every summon
                    if (creatureTemplate.CreatureSpellEntriesOutOfCombatSummons.Count > 0)
                    {
                        // Group spells and put them in a timed list
                        List<int> spellTemplateIDs = new List<int>();
                        foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesOutOfCombatSummons)
                        {
                            SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                            spellTemplateIDs.Add(curSpellTemplate.WOWSpellID);
                        }
                        string timedActionListComment = string.Concat("EQ Out of Combat Summon ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") casting one of ", spellTemplateIDs.Count, " summon spells");
                        smartScriptsSQL.AddRowsForCreatureTimedActionListOfOutOfCombatSpells(creatureTemplate.WOWCreatureTemplateID, spellTemplateIDs, timedActionListComment);
                        conditionsSQL.AddSmartScriptRestrictionIfAura(creatureTemplate.WOWCreatureTemplateID, smartScriptsSQL.GetLastUniqueID(creatureTemplate.WOWCreatureTemplateID, 0) + 1,
                            0, Configuration.SPELL_SUMMON_CASTER_AURA_SPELL_ID, string.Concat("Restrict Summon of spells if a summon is active for ", creatureTemplate.Name), true);                       
                    }

                    // Add spell events for every buff entry
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesOutOfCombatBuff)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        string comment = string.Concat("EQ Out of Combat Buffs ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateOutOfCombatBuffCastSelf(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, curSpellTemplate.WOWSpellID, comment);
                        if (curSpellTemplate.RemoveAuraWhenCasterCreatureInitsAgro == true)
                        {
                            string removeAuraComment = string.Concat("EQ Out of Combat Buffs ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") remove aura ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ") when agro on the player");
                            smartScriptsSQL.AddRowForCreatureTemplateRemoveSpellAuraOnAgro(creatureTemplate.WOWCreatureTemplateID, curSpellTemplate.WOWSpellID, removeAuraComment);
                        }
                    }

                    // Spell on Attack
                    foreach (var eqSpellIDAndProcChance in creatureTemplate.AttackEQSpellIDAndProcChance)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[eqSpellIDAndProcChance.Item1];
                        string comment = string.Concat("EQ Attack Proc ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateApplySpellOnDamageDone(creatureTemplate.WOWCreatureTemplateID, eqSpellIDAndProcChance.Item2,
                            curSpellTemplate.WOWSpellID, comment);
                    }

                    // Summons need to add an aura to the caster
                    if (creatureTemplate.DoesSummonPets == true)
                    {
                        smartScriptsSQL.AddRowForCreatureTemplateCastOnSummoned(creatureTemplate.WOWCreatureTemplateID, Configuration.SPELL_SUMMON_CASTER_AURA_SPELL_ID,
                            string.Concat("EQ Summon Pet Add Summoner Aura to Caster ", creatureTemplate.Name, "(", creatureTemplate.WOWCreatureTemplateID, ")"));
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
            
            // Azeroth creatures for teleports
            if (Configuration.GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION == true)
            {
                // Creatures themselves
                List<CreatureTeleporter> creatureTeleporters = CreatureTeleporter.GetAllCreatureTeleporters();
                Dictionary<int, CreatureTemplate> creatureTemplatesByWOWID = CreatureTemplate.GetCreatureTemplateListByWOWID();
                if (creatureTemplatesByWOWID.ContainsKey(Configuration.GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID) == false)
                {
                    Logger.WriteError("GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION was true but no creature template with ID as defined by GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID could be found.");
                }
                else
                {
                    foreach (CreatureTeleporter creatureTeleporter in creatureTeleporters)
                    {
                        string creatureComment = string.Concat("EQ Azeroth Priest of Discord");
                        int creatureGUID = CreatureTemplate.GenerateCreatureSQLGUID();
                        creatureSQL.AddRow(creatureGUID, Configuration.GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID, creatureTeleporter.MapID,
                            creatureTeleporter.AreaID, creatureTeleporter.AreaID, creatureTeleporter.XPosition, creatureTeleporter.YPosition,
                            creatureTeleporter.ZPosition, creatureTeleporter.Orientation, CreatureMovementType.None, creatureComment, false);
                    }
                }
            }
        }

        // Aligns one creature candidate to each spawn pont, and spreading them out proportionate to their spawn chance in a way that comes out to exactly one point count
        private static List<CreatureTemplate> DistributeCandidatesAcrossSpawnPoints(List<CreatureTemplate> templates, List<int> chances, int pointCount)
        {
            List<CreatureTemplate> assigned = new List<CreatureTemplate>();
            if (templates.Count <= 1)
            {
                for (int p = 0; p < pointCount; p++)
                    assigned.Add(templates[0]);
                return assigned;
            }

            int totalChance = 0;
            foreach (int chance in chances)
                totalChance += chance;
            if (totalChance <= 0)
                totalChance = templates.Count;

            double[] exact = new double[templates.Count];
            int[] counts = new int[templates.Count];
            int placed = 0;
            for (int i = 0; i < templates.Count; i++)
            {
                exact[i] = (double)chances[i] / totalChance * pointCount;
                counts[i] = (int)Math.Floor(exact[i]);
                placed += counts[i];
            }

            // Total up remainder
            int remaining = pointCount - placed;
            while (remaining > 0)
            {
                int bestIndex = 0;
                double bestRemainder = -1.0;
                for (int i = 0; i < templates.Count; i++)
                {
                    double remainder = exact[i] - Math.Floor(exact[i]);
                    if (remainder > bestRemainder)
                    {
                        bestRemainder = remainder;
                        bestIndex = i;
                    }
                }
                counts[bestIndex]++;
                exact[bestIndex] = Math.Floor(exact[bestIndex]);
                remaining--;
            }

            for (int i = 0; i < templates.Count; i++)
                for (int k = 0; k < counts[i]; k++)
                    assigned.Add(templates[i]);
            return assigned;
        }

        private static Dictionary<int, HashSet<int>> alreadySavedCustomWaypointGridIDsByMapID = new Dictionary<int, HashSet<int>>(); // Ensure only 1 of each waypoint set is saved
        private void CreateCreatureAndRelatedSQLEntries(int creatureGUID, CreatureTemplate creatureTemplate, CreatureSpawnInstance spawnInstance, CreatureSpawnGroup spawnGroup, string comment)
        {
            List<CreaturePathGridEntry> pathEntries = spawnInstance.GetPathGridEntries();
            CreatureMovementType movementType = CreatureMovementType.None;
            CreaturePathGridWanderType wanderType = spawnInstance.GetPathGrid().WanderType;

            if (spawnGroup.DoesRoam() == true)
            {
                if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                    creatureTemplate.SubName = spawnInstance.ID + " Roams";
                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                modEverquestCreatureInstanceSQL.AddRow(creatureGUID, wanderType, spawnInstance.GetPathGrid().PauseType, spawnInstance.MapID, spawnInstance.GetPathGrid().GridID,
                    true, spawnGroup.RoamMinX, spawnGroup.RoamMaxX, spawnGroup.RoamMinY, spawnGroup.RoamMaxY, spawnGroup.RoamMinZ, spawnGroup.RoamMaxZ, spawnGroup.RoamMinDelayInMS, 
                    spawnGroup.RoamMaxDelayInMS);
                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                    spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, comment, true);
            }
            else if (pathEntries.Count > 0 && wanderType != CreaturePathGridWanderType.None)
            {
                // Only about half of waypoint types are handled directly by the AzerothCore game engine
                if (wanderType == CreaturePathGridWanderType.GridRandom10 || wanderType == CreaturePathGridWanderType.GridRandom
                    || wanderType == CreaturePathGridWanderType.GridRand5LoS || wanderType == CreaturePathGridWanderType.GridRandomCenterPoint
                    || wanderType == CreaturePathGridWanderType.GridRandomPath)
                {
                    creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                    if (alreadySavedCustomWaypointGridIDsByMapID.ContainsKey(spawnInstance.MapID) == false)
                        alreadySavedCustomWaypointGridIDsByMapID.Add(spawnInstance.MapID, new HashSet<int>());
                    if (alreadySavedCustomWaypointGridIDsByMapID[spawnInstance.MapID].Contains(pathEntries[0].GridID) == false)
                    {
                        for (int i = 0; i < pathEntries.Count; i++)
                        {
                            CreaturePathGridEntry entry = pathEntries[i];
                            modEverquestCreatureWaypointSQL.AddRow(spawnInstance.MapID, entry.GridID, entry.Number, entry.NodeX, entry.NodeY, entry.NodeZ, entry.PauseInSec);
                        }
                        alreadySavedCustomWaypointGridIDsByMapID[spawnInstance.MapID].Add(pathEntries[0].GridID);
                    }
                    modEverquestCreatureInstanceSQL.AddRow(creatureGUID, wanderType, spawnInstance.GetPathGrid().PauseType, spawnInstance.MapID, spawnInstance.GetPathGrid().GridID,
                        false, 0, 0, 0, 0, 0, 0, 0, 0, -1, spawnInstance.GetPathGrid().DisableGroundContour);
                    creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                        spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, comment, true);
                }
                else
                {
                    int waypointGUID = creatureGUID * 1000;
                    if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                        waypointGUID = creatureGUID * 10;
                    creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID);
                    bool useModScript = false;
                    switch (wanderType)
                    {
                        case CreaturePathGridWanderType.GridCircular:
                        case CreaturePathGridWanderType.GridCenterPoint:
                            {
                                for (int i = 0; i < pathEntries.Count; i++)
                                {
                                    CreaturePathGridEntry entry = pathEntries[i];
                                    waypointDataSQL.AddRow(waypointGUID, i + 1, entry.NodeX, entry.NodeY, entry.NodeZ, entry.PauseInSec * 1000);
                                }
                            } break;
                        case CreaturePathGridWanderType.GridPatrol:
                            {
                                int pointID = 1;
                                for (int i = 0; i < pathEntries.Count; i++)
                                {
                                    CreaturePathGridEntry entry = pathEntries[i];
                                    int pauseInMS = entry.PauseInSec * 1000;
                                    waypointDataSQL.AddRow(waypointGUID, pointID, entry.NodeX, entry.NodeY, entry.NodeZ, pauseInMS);
                                    pointID++;
                                }
                                for (int i = pathEntries.Count - 2; i >= 0; i--)
                                {
                                    CreaturePathGridEntry entry = pathEntries[i];
                                    int pauseInMS = entry.PauseInSec * 1000;
                                    if (i == 0)
                                        pauseInMS = 0;
                                    waypointDataSQL.AddRow(waypointGUID, pointID, entry.NodeX, entry.NodeY, entry.NodeZ, pauseInMS);
                                    pointID++;
                                }
                            } break;
                        case CreaturePathGridWanderType.GridOneWayRepop:
                        case CreaturePathGridWanderType.GridOneWayDepop:
                            {
                                int pointID = 0;
                                for (int i = 0; i < pathEntries.Count; i++)
                                {
                                    CreaturePathGridEntry entry = pathEntries[i];
                                    int pauseInMS = entry.PauseInSec * 1000;
                                    pointID++;
                                    waypointDataSQL.AddRow(waypointGUID, pointID, entry.NodeX, entry.NodeY, entry.NodeZ, pauseInMS);
                                }
                                // Duplicate last node for a despawn event
                                CreaturePathGridEntry lastEntryCopy = pathEntries[pathEntries.Count - 1];
                                waypointDataSQL.AddRow(waypointGUID, pointID + 1, lastEntryCopy.NodeX, lastEntryCopy.NodeY, lastEntryCopy.NodeZ, 0);
                                modEverquestCreatureInstanceSQL.AddRow(creatureGUID, wanderType, spawnInstance.GetPathGrid().PauseType, spawnInstance.MapID, spawnInstance.GetPathGrid().GridID,
                                    false, 0, 0, 0, 0, 0, 0, 0, 0, pointID + 1, spawnInstance.GetPathGrid().DisableGroundContour);
                                useModScript = true;
                            } break;
                        default:
                            {
                                Logger.WriteError("CreateCreatureAndRelatedSQLEntries error, unhandled wanderType of '", wanderType.ToString(), "' for spawn group '", spawnGroup.ID.ToString(), "'");
                            } break;
                    }

                    if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true && pathEntries.Count > 0)
                        creatureTemplate.SubName = spawnInstance.ID + " WP " + pathEntries[0].GridID;

                    movementType = CreatureMovementType.Path;
                    creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                        spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, comment, useModScript);
                }
            }
            else
            {
                if (Configuration.CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                    creatureTemplate.SubName = spawnInstance.ID + " Immobile";
                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID);
                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                    spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, comment, false);
            }
        }

        private void PopulateItemData(Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID, Dictionary<int, SpellTemplate> spellTemplatesByEQID)
        {
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;

                // Save any additional metadata
                if (itemTemplate.WOWEntryIDForCreatureEquip != 0)
                    modEverquestItemTemplateSQL.AddRow(itemTemplate.WOWEntryID, itemTemplate.WOWEntryIDForCreatureEquip);
                else
                    modEverquestItemTemplateSQL.AddRow(itemTemplate.WOWEntryID, itemTemplate.WOWEntryID);

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
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.Description, itemTemplate.RequiredLevel,
                            itemTemplate.AllowedClassTypes, itemTemplate.ItemDisplayInfo);
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
                                itemTemplate.Description, scrollPropertiesByClassType.Value.LearnLevel, new List<ClassWOWType>() { scrollPropertiesByClassType.Key }, itemTemplate.ItemDisplayInfo);
                        }
                    }
                }
                else
                {
                    // Factor for creature-wearable versions and starter versions
                    itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.Description, itemTemplate.RequiredLevel, itemTemplate.AllowedClassTypes, itemTemplate.ItemDisplayInfo);
                    if (itemTemplate.ItemDisplayInfoForCreatureEquip != null)
                    {
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryIDForCreatureEquip, string.Concat(itemTemplate.Name, " (npc)"), itemTemplate.Description,
                            itemTemplate.RequiredLevel, itemTemplate.AllowedClassTypes, itemTemplate.ItemDisplayInfoForCreatureEquip);
                    }
                    if (itemTemplate.StarterVersionItemTemplateID > 0)
                    {
                        string startVersionName = itemTemplate.Name;
                        if (startVersionName.Contains("*") == false)
                            startVersionName = String.Concat(startVersionName, "*");
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.StarterVersionItemTemplateID, startVersionName, itemTemplate.Description, itemTemplate.RequiredLevel, itemTemplate.AllowedClassTypes, itemTemplate.ItemDisplayInfo);
                    }
                    for (int i = 0; i < itemTemplate.ContainedItems.Count; i++)
                    {
                        int curWOWItemTemplateID = itemTemplate.ContainedItems[i].itemTemplateIDWOW;
                        float curItemChance = itemTemplate.ContainedItems[i].chance;
                        int curItemCount = itemTemplate.ContainedItems[i].count;
                        int curItemGroup = itemTemplate.ContainedItems[i].group;
                        string comment;
                        if (itemTemplate.ContainedItems[i].parentItemTemplateIDWOW == 0)
                            comment = string.Concat(itemTemplate.Name, " - ", itemTemplatesByWOWID[curWOWItemTemplateID].Name);
                        else
                            comment = string.Concat(itemTemplate.Name, " - ", itemTemplatesByWOWID[itemTemplate.ContainedItems[i].parentItemTemplateIDWOW].Name);
                        itemLootTemplateSQL.AddRow(itemTemplate.WOWEntryID, curWOWItemTemplateID, curItemGroup, curItemChance, curItemCount, comment);
                    }
                }
            }
            foreach (var itemLootTemplateByCreatureTemplateID in itemLootTemplatesByCreatureTemplateID.Values)
                foreach (ItemLootTemplate itemLootTemplate in itemLootTemplateByCreatureTemplateID)
                    creatureLootTableSQL.AddRow(itemLootTemplate);

            // Page text (for books)
            foreach (ItemTemplate.BookText bookText in ItemTemplate.GetAllBookTexts())
                pageTextSQL.AddRow(bookText.PageTextID, bookText.Text);
        }

        private void PopulateForageData()
        {
            foreach (ForageZoneItem forageZoneItem in ForageZoneItem.GetAllZoneItems())
            {
                if (forageZoneItem.WOWMapID == -1 || forageZoneItem.WOWItemTemplateID == -1)
                    continue;
                modEverquestForageZoneItemsSQL.AddRow(forageZoneItem.WOWMapID, forageZoneItem.WOWItemTemplateID, forageZoneItem.Chance,
                    forageZoneItem.ForageType);
            }
        }

        private void PopulateFishingData(Dictionary<string, ZoneProperties> zonePropertiesByShortName)
        {
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWID = ItemTemplate.GetItemTemplatesByWOWEntryID();

            // Junk fishing reference
            int junkReferenceID = ReferenceLootTemplateSQL.GenerateID();
            foreach (FishingZoneItem junkFishingZoneItem in FishingZoneItem.GetJunkFishingItems())
            {
                ItemTemplate curItemTemplate = itemTemplatesByWOWID[junkFishingZoneItem.WOWItemTemplateID];
                string comment = string.Concat("EQ Junk ", curItemTemplate.Name);
                referenceLootTemplateSQL.AddRow(junkReferenceID, junkFishingZoneItem.WOWItemTemplateID, junkFishingZoneItem.ChanceAbsolute, true, comment);
            }

            Dictionary<string, List<FishingZoneItem>> fishingZoneItemsByZoneShortName = FishingZoneItem.GetFishingZoneItemsByZoneShortName();
            Dictionary<string, int> wowFishingLevelByZoneShortName = FishingZoneItem.GetWOWFishingLevelByZoneShortName();
            foreach (var fishingZoneItemsInZoneByShortName in fishingZoneItemsByZoneShortName)
            {
                if (zonePropertiesByShortName.ContainsKey(fishingZoneItemsInZoneByShortName.Key) == false)
                {
                    Logger.WriteError("PopulateFishingData skipped a row because zone with short name '", fishingZoneItemsInZoneByShortName.Key, "' was not found");
                    continue;
                }
                ZoneProperties zoneProperties = zonePropertiesByShortName[fishingZoneItemsInZoneByShortName.Key];
                HashSet<int> areaTableIDs = new HashSet<int>();
                areaTableIDs.Add((int)zoneProperties.DefaultZoneArea.DBCAreaTableID);
                foreach (ZoneArea subArea in zoneProperties.SubZoneAreas)
                    areaTableIDs.Add((int)subArea.DBCAreaTableID);
                foreach (int areaTableID in areaTableIDs)
                {
                    // Zone fishing properties
                    skillFishingBaseLevelSQL.AddRow(areaTableID, wowFishingLevelByZoneShortName[fishingZoneItemsInZoneByShortName.Key]);

                    // Fishing Loot Rows
                    foreach (FishingZoneItem fishingZoneItem in fishingZoneItemsInZoneByShortName.Value)
                    {
                        if (itemTemplatesByWOWID.ContainsKey(fishingZoneItem.WOWItemTemplateID) == false)
                        {
                            Logger.WriteError("PopulateFishingData skipped a row because no item template could be found with wowid '", fishingZoneItem.WOWItemTemplateID.ToString(), "'");
                            continue;
                        }
                        ItemTemplate curItemTemplate = itemTemplatesByWOWID[fishingZoneItem.WOWItemTemplateID];
                        string comment = string.Concat("EQ ", curItemTemplate.Name, " (", zoneProperties.ShortName, ")");
                        fishingLootTemplateSQL.AddRow(areaTableID, 0, fishingZoneItem.WOWItemTemplateID, fishingZoneItem.ChanceAbsolute, false, 1, comment);
                    }

                    // Also add the junk items
                    fishingLootTemplateSQL.AddRow(areaTableID, junkReferenceID, junkReferenceID, 100, true, 0, "(ReferenceTable)");
                }
            }
        }

        private void PopulatePlayerData(List<Zone> zones, Dictionary<string, int> mapIDsByShortName)
        {
            // Player Start Data
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true && zones.Count > 0)
            {
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

                    modEverquestPlayerCreateInfoSQL.AddRow(classRaceProperties.Key.Item1, classRaceProperties.Key.Item2, mapIDsByShortName[startZoneShortName],
                        areaIDsByShortName[startZoneShortName], classRaceProperties.Value.StartPositionX, classRaceProperties.Value.StartPositionY,
                        classRaceProperties.Value.StartPositionZ, classRaceProperties.Value.StartOrientation);
                }
            }

            // Autolearn Skills / Spells
            foreach (ClassWOWType wowClassType in PlayerClassMapping.GetWOWClassesThatShouldHaveForage())
                modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, Configuration.FORAGE_SPELL_TEMPLATE_ID, true);
            if (Configuration.PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START == true)
            {
                foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                {
                    if (wowClassType == ClassWOWType.All || wowClassType == ClassWOWType.None)
                        continue;

                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, Configuration.SPELLS_BINDCUSTOM_SPELLDBC_ID, true);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, Configuration.SPELLS_GATECUSTOM_SPELLDBC_ID, true);                    
                }
            }
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> leatherClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Leather).ToList();
                foreach (ClassWOWType wowClassType in leatherClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 414);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 9077);
                }
                List<ClassWOWType> mailClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Mail).ToList();
                foreach (ClassWOWType wowClassType in mailClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 413);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 8737);
                }
                List<ClassWOWType> plateClasses = PlayerClassMapping.GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType.Plate).ToList();
                foreach (ClassWOWType wowClassType in plateClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 293);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 750);
                }

            }
            if (Configuration.PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES == true)
            {
                foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                {
                    if (wowClassType != ClassWOWType.All && wowClassType != ClassWOWType.None)
                    {
                        modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 433);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 9116);
                    }
                }
            }
            if (Configuration.PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES == true)
            {
                List<ClassWOWType> bowClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Bow).ToList();
                foreach (ClassWOWType wowClassType in bowClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 45);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 264);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 3018, true); // Shoot
                }
            }
            if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES == true)
            {
                List<ClassWOWType> axeOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeOneHand).ToList();
                foreach (ClassWOWType wowClassType in axeOneHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 44);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 196);
                }
                List<ClassWOWType> axeTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.AxeTwoHand).ToList();
                foreach (ClassWOWType wowClassType in axeTwoHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 172);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 197);
                }
                List<ClassWOWType> maceOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceOneHand).ToList();
                foreach (ClassWOWType wowClassType in maceOneHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 54);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 198);
                }
                List<ClassWOWType> maceTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.MaceTwoHand).ToList();
                foreach (ClassWOWType wowClassType in maceTwoHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 160);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 199);
                }
                List<ClassWOWType> polearmClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Polearm).ToList();
                foreach (ClassWOWType wowClassType in polearmClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 229);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 200);
                }
                List<ClassWOWType> swordOneHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordOneHand).ToList();
                foreach (ClassWOWType wowClassType in swordOneHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 43);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 201);
                }
                List<ClassWOWType> swordTwoHandClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.SwordTwoHand).ToList();
                foreach (ClassWOWType wowClassType in swordTwoHandClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 55);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 202);
                }
                List<ClassWOWType> staffClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Staff).ToList();
                foreach (ClassWOWType wowClassType in staffClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 136);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 227);
                }
                List<ClassWOWType> fistWeaponClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.FistWeapon).ToList();
                foreach (ClassWOWType wowClassType in fistWeaponClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 473);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 15590);
                }
                List<ClassWOWType> daggerClasses = PlayerClassMapping.GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType.Dagger).ToList();
                foreach (ClassWOWType wowClassType in daggerClasses)
                {
                    modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)wowClassType, 173);
                    modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)wowClassType, 1180);
                }
            }

            // DK pre-55 stuff
            if (Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true)
            {
                // Stats
                foreach (PlayerClassLevelStats dkLevelStats in PlayerClassLevelStats.GetPre55DKLevelStats())
                    playerClassStatsSQL.AddRow(ClassWOWType.DeathKnight, dkLevelStats.Level, dkLevelStats.BaseHP,
                        dkLevelStats.BaseMana, dkLevelStats.Strength, dkLevelStats.Agility, dkLevelStats.Stamina,
                        dkLevelStats.Intellect, dkLevelStats.Spirit);

                // Runeforging
                modEverquestPlayerAutoLearnSkillsSQL.AddRow((int)ClassWOWType.DeathKnight, 776);
                modEverquestPlayerAutoLearnSpellsSQL.AddRow((int)ClassWOWType.DeathKnight, 53428);
            }
        }

        private void PopulateQuestData(List<QuestTemplate> questTemplates, SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            Dictionary<int, int> creatureTextGroupIDsByCreatureTemplateID = new Dictionary<int, int>();
            foreach (QuestTemplate questTemplate in questTemplates)
            {
                // Skip quests where items needed to complete it are not available
                if (questTemplate.AreRequiredItemsPlayerObtainable(itemTemplatesByWOWEntryID) == false)
                    continue;
                if (questTemplate.IsValidQuest == false)
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

                    foreach (QuestReaction reaction in questTemplate.Reactions)
                    {
                        // Reward say/yell/emote actions
                        if (reaction.ReactionType == QuestReactionType.Emote || reaction.ReactionType == QuestReactionType.Say || reaction.ReactionType == QuestReactionType.Yell)
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
                            string comment = string.Concat("EQ ", creatureTemplateByWOWID[creatureTemplateID].Name, " Quest ", reaction.ReactionType.ToString());
                            int messageType = 12; // Default to say
                            switch (reaction.ReactionType)
                            {
                                case QuestReactionType.Say: messageType = 12; break;
                                case QuestReactionType.Emote: messageType = 16; break;
                                case QuestReactionType.Yell: messageType = 14; break;
                                default: Logger.WriteError("Unhandled quest reaction type of " + reaction.ReactionType); break;
                            }
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID + 1, messageType, reaction.ReactionValue, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);

                            // Smart Script
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID, firstQuestID, comment);
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID + 1, repeatQuestID, comment);
                        }

                        // Attack/Spawn/Despawn actions
                        if (reaction.ReactionType == QuestReactionType.AttackPlayer || reaction.ReactionType == QuestReactionType.Despawn || reaction.ReactionType == QuestReactionType.Spawn || reaction.ReactionType == QuestReactionType.SpawnUnique)
                        {
                            if (reaction.CreatureEQID > 0)
                            {
                                if (creatureTemplatesByEQID.ContainsKey(reaction.CreatureEQID) == false)
                                {
                                    Logger.WriteDebug("Skipping quest reaction for quest ", questTemplate.QuestIDWOW.ToString(), " as the target creature EQID of ", reaction.CreatureEQID.ToString() ,"could not be found");
                                    continue;
                                }
                                reaction.CreatureWOWID = creatureTemplatesByEQID[reaction.CreatureEQID].WOWCreatureTemplateID;
                            }
                            if (reaction.CreatureIsSelf == true)
                                reaction.CreatureWOWID = creatureTemplateID;

                            modEverquestQuestReactionSQL.AddRow(firstQuestID, reaction);
                            modEverquestQuestReactionSQL.AddRow(repeatQuestID, reaction);
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

        private void AddSpellChain(SpellTemplate baseSpellTemplate, int parentSpellTemplateID, int chainedSpellID, string chainedSpellName)
        {
            if (baseSpellTemplate.AuraDuration.MaxDurationInMS > 0)
                spellLinkedSpellSQL.AddRowForAuraTrigger(parentSpellTemplateID, chainedSpellID, chainedSpellName);
            else
                spellLinkedSpellSQL.AddRowForHitTrigger(parentSpellTemplateID, chainedSpellID, chainedSpellName);
        }

        HashSet<int> PetSpellIDsAdded = new HashSet<int>();
        private void AddSpellDataBlock(SpellTemplate spellTemplate, List<SpellEffectBlock> spellEffectBlocks, string commentFragment)
        {
            if (spellEffectBlocks.Count == 0 ||  spellEffectBlocks[0].WOWSpellID <= 0)
                return;

            for (int i = 0; i < spellEffectBlocks.Count; i++)
            {
                SpellEffectBlock curEffectBlock = spellEffectBlocks[i];

                // Mod data
                modEverquestSpellSQL.AddRow(spellTemplate, curEffectBlock.WOWSpellID);

                // Spell bonus (TODO: do something more tailored)
                spellBonusDataSQL.AddRow(curEffectBlock.WOWSpellID, string.Concat("EQ Spell ", spellTemplate.Name, commentFragment, " Block ", i));

                // Additional effects beyond the first
                if (i > 0)
                    AddSpellChain(spellTemplate, spellEffectBlocks[0].WOWSpellID, curEffectBlock.WOWSpellID, curEffectBlock.SpellName);
            }

            // Scripts
            if (spellTemplate.IsFocusBoostableEffect == true)
            {
                if (spellEffectBlocks[0].SpellEffects[0].IsAuraType() == true)
                    spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_FocusBoostAuraScript");
                else
                    spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_FocusBoostNonAuraScript");
            }
            if (spellTemplate.IsBardSongAura == true)
                spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_BardSongAuraScript");
            if (spellTemplate.IsllusionSpellParent == true)
                spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_IllusionSpellScript");

            // Pet (but avoid duplicates)
            if (spellTemplate.SummonSpellPet != null && PetSpellIDsAdded.Contains(spellEffectBlocks[0].WOWSpellID) == false)
            {
                modEverquestPetSQL.AddRow(spellEffectBlocks[0].WOWSpellID, spellTemplate.SummonSpellPet.NamingType, spellTemplate.SummonCreatureTemplateID,
                    spellTemplate.SummonPropertiesDBCID, spellTemplate.SummonSpellPet.MainhandItemIDWOW, spellTemplate.SummonSpellPet.OffhandItemIDWOW);

                if (spellTemplate.SummonSpellPet.NamingType == SpellPetNamingType.Random)
                {
                    foreach (string prefix in SpellPet.GetRandomPetNamePrefixes())
                        petNameGenerationSQL.AddRow(spellTemplate.SummonCreatureTemplateID, prefix, true);
                    foreach (string suffix in SpellPet.GetRandomPetNameSuffixes())
                        petNameGenerationSQL.AddRow(spellTemplate.SummonCreatureTemplateID, suffix, false);
                }
                PetSpellIDsAdded.Add(spellEffectBlocks[0].WOWSpellID);
            }
        }

        private void PopulateSpellAndTradeskillData(List<SpellTemplate> spellTemplates, List<TradeskillRecipe> tradeskillRecipes,
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWEntryID)
        {
            // Spell split data
            foreach (SpellTemplate spellTemplate in spellTemplates)
            {
                // Core Spell Data
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedBaseSpellEffectBlocksForOutput, "");
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedWornSpellEffectBlocksForOutput, " (Worn)");
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedGoodProcSpellEffectBlocksForOutput, " (Proc)");
                for (int i = 0; i < spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters.Count; i++)
                    AddSpellDataBlock(spellTemplate, spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters[i], " (Clicky)");

                // Stack rules
                foreach (int spellGroupStackingID in spellTemplate.SpellGroupStackingIDs)
                {
                    spellGroupSQL.AddRow(spellGroupStackingID, spellTemplate.WOWSpellID);
                    if (spellTemplate.WOWSpellIDWorn > 0)
                        spellGroupSQL.AddRow(spellGroupStackingID, spellTemplate.WOWSpellIDWorn);
                    if (spellTemplate.WOWSpellIDProcAndGoodEffect != -1)
                        spellGroupSQL.AddRow(spellGroupStackingID, spellTemplate.WOWSpellIDProcAndGoodEffect);
                    for (int clickyIndex = 0; clickyIndex < spellTemplate.ClickySpellParatemers.Count; clickyIndex++)
                        spellGroupSQL.AddRow(spellGroupStackingID, spellTemplate.ClickySpellParatemers[clickyIndex].WOWSpellID);
                }

                // Teleports
                for (int i = 0; i < spellTemplate.GroupedBaseSpellEffectBlocksForOutput[0].SpellEffects.Count; i++)
                {
                    if (spellTemplate.GroupedBaseSpellEffectBlocksForOutput[0].SpellEffects[i].EffectType == SpellWOWEffectType.TeleportUnits)
                    {
                        List<SpellEffectBlock> groupedBaseSpellEffectBlocksForOutput = spellTemplate.GroupedBaseSpellEffectBlocksForOutput;
                        SpellEffectWOW curEffect = groupedBaseSpellEffectBlocksForOutput[0].SpellEffects[i];
                        spellTargetPositionSQL.AddRow(groupedBaseSpellEffectBlocksForOutput[0].WOWSpellID, i, curEffect.TeleMapID, curEffect.TelePosition, curEffect.TeleOrientation);
                        // Skip worm
                        // Skip good proc
                        for (int clickyIndex = 0; clickyIndex < spellTemplate.ClickySpellParatemers.Count; clickyIndex++)
                            spellTargetPositionSQL.AddRow(spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters[clickyIndex][0].WOWSpellID, i, curEffect.TeleMapID, curEffect.TelePosition, curEffect.TeleOrientation);
                    }
                }

                // Chains
                foreach (SpellTemplate chainedSpellTemplate in spellTemplate.ChainedSpellTemplates)
                {
                    List<SpellEffectBlock> chainedGroupedBaseSpellEffectBlocksForOutput = chainedSpellTemplate.GroupedBaseSpellEffectBlocksForOutput;
                    int chainedSpellID = chainedGroupedBaseSpellEffectBlocksForOutput[0].WOWSpellID;
                    string chainedSpellName = chainedGroupedBaseSpellEffectBlocksForOutput[0].SpellName;
                    AddSpellChain(spellTemplate, spellTemplate.WOWSpellID, chainedSpellID, chainedSpellName);
                    if (spellTemplate.WOWSpellIDWorn > 0)
                        AddSpellChain(spellTemplate, spellTemplate.WOWSpellIDWorn, chainedSpellID, chainedSpellName);
                    if (spellTemplate.WOWSpellIDProcAndGoodEffect != -1)
                        AddSpellChain(spellTemplate, spellTemplate.WOWSpellIDProcAndGoodEffect, chainedSpellID, chainedSpellName);
                    for (int clickyIndex = 0; clickyIndex < spellTemplate.ClickySpellParatemers.Count; clickyIndex++)
                        AddSpellChain(spellTemplate, spellTemplate.ClickySpellParatemers[clickyIndex].WOWSpellID, chainedSpellID, chainedSpellName);
                }
            }
            foreach (var spellGroupStackRuleByGroup in SpellTemplate.SpellGroupStackRuleByGroup)
                spellGroupStackRulesSQL.AddRow(spellGroupStackRuleByGroup.Key, spellGroupStackRuleByGroup.Value);
        }

        private void PopulateTrainerData(List<CreatureTemplate> creatureTemplates)
        {
            // Trainer Abilities - Class
            Dictionary<ClassWOWType, int> trainerIDsByClass = new Dictionary<ClassWOWType, int>();
            foreach (ClassWOWType classType in Enum.GetValues(typeof(ClassWOWType)))
            {
                if (classType == ClassWOWType.All || classType == ClassWOWType.None)
                    continue;
                trainerIDsByClass.Add(classType, TrainerSQL.GenerateUniqueTrainerID());
                trainerSQL.AddRow(trainerIDsByClass[classType], 0, (int)classType, "What would you like to learn?");
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForClass(classType))
                    trainerSpellSQL.AddRow(trainerIDsByClass[classType], trainerAbility);
            }

            // Trainer Abilities - Tradeskills
            Dictionary<TradeskillType, int> trainerIDsByTradeskill = new Dictionary<TradeskillType, int>();
            foreach (TradeskillType tradeskillType in Enum.GetValues(typeof(TradeskillType)))
            {
                if (tradeskillType == TradeskillType.Unknown || tradeskillType == TradeskillType.None)
                    continue;
                
                trainerIDsByTradeskill.Add(tradeskillType, TrainerSQL.GenerateUniqueTrainerID());
                trainerSQL.AddRow(trainerIDsByTradeskill[tradeskillType], 2, 0, "What would you like to learn?");
                trainerSpellSQL.AddDevelopmentSkillsForTradeskill(trainerIDsByTradeskill[tradeskillType], tradeskillType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForTradeskill(tradeskillType))
                    trainerSpellSQL.AddRow(trainerIDsByTradeskill[tradeskillType], trainerAbility);
            }

            // Trainer Abilities - Riding Trainer
            int trainerIDForRidingTrainer = TrainerSQL.GenerateUniqueTrainerID();
            trainerSQL.AddRow(trainerIDForRidingTrainer, 1, 0, "What would you like to learn?");
            trainerSpellSQL.AddRiderSkills(trainerIDForRidingTrainer);

            // Pre-generate class trainer menus
            Dictionary<ClassWOWType, int> classTrainerMenuIDs = new Dictionary<ClassWOWType, int>();
            foreach (ClassWOWType classType in Enum.GetValues(typeof(ClassWOWType)))
            {
                if (classType == ClassWOWType.All || classType == ClassWOWType.None)
                    continue;

                // Base menu
                int gossipMenuID = GossipMenuSQL.GenerateUniqueMenuID();
                classTrainerMenuIDs.Add(classType, gossipMenuID);
                gossipMenuSQL.AddRow(gossipMenuID, Configuration.CREATURE_GOSSIP_NPC_TEXT_ID);

                // Menu options
                gossipMenuOptionSQL.AddRow(gossipMenuID, 0, 3, "I would like to train.",
                    Configuration.CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
                gossipMenuOptionSQL.AddRow(gossipMenuID, 1, 0, "I wish to unlearn my talents",
                    Configuration.CREATURE_GOSSIP_UNLEARN_BROADCAST_TEXT_ID, 16, 16, Configuration.CREATURE_CLASS_TRAINER_UNLEARN_MENU_ID);
                gossipMenuOptionSQL.AddRow(gossipMenuID, 2, 0, "I wish to know about Dual Talent Specialization.",
                    Configuration.CREATURE_GOSSIP_DUALTALENT_BROADCAST_TEXT_ID, 20, 1, Configuration.CREATURE_CLASS_TRAINER_DUALTALENT_MENU_ID);

                // Restrictions to menu options
                string conditionsComment = string.Concat("Restrict menu option for class ", classType.ToString());
                conditionsSQL.AddRowForMenuOptionClassRestriction(gossipMenuID, 0, new List<ClassWOWType>() { classType }, conditionsComment);
                conditionsSQL.AddRowForMenuOptionClassRestriction(gossipMenuID, 1, new List<ClassWOWType>() { classType }, conditionsComment);
                conditionsSQL.AddRowForMenuOptionClassRestriction(gossipMenuID, 2, new List<ClassWOWType>() { classType }, conditionsComment);
            }

            // Pre-generate profession/rider trainer menus
            int nonClassTrainerGossipMenuIDNoShop = GossipMenuSQL.GenerateUniqueMenuID();
            gossipMenuSQL.AddRow(nonClassTrainerGossipMenuIDNoShop, Configuration.CREATURE_GOSSIP_NPC_TEXT_ID);
            gossipMenuOptionSQL.AddRow(nonClassTrainerGossipMenuIDNoShop, 0, 3, "I would like to train.",
                Configuration.CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
            int nonClassTrainerGossipMenuIDWithShop = GossipMenuSQL.GenerateUniqueMenuID();
            gossipMenuSQL.AddRow(nonClassTrainerGossipMenuIDWithShop, Configuration.CREATURE_GOSSIP_NPC_TEXT_ID);
            gossipMenuOptionSQL.AddRow(nonClassTrainerGossipMenuIDWithShop, 0, 3, "I would like to train.",
                Configuration.CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
            gossipMenuOptionSQL.AddRow(nonClassTrainerGossipMenuIDWithShop, 1, 1, "I want to browse your goods.",
                Configuration.CREATURE_GOSSIP_PURCHASE_BROADCAST_TEXT_ID, 3, 128, 0);

            // Associate creature templates to trainer lists
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                if (creatureTemplate.ClassTrainerType != ClassWOWType.All && creatureTemplate.ClassTrainerType != ClassWOWType.None)
                {
                    creatureDefaultTrainerSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, trainerIDsByClass[creatureTemplate.ClassTrainerType]);
                    creatureTemplate.GossipMenuID = classTrainerMenuIDs[creatureTemplate.ClassTrainerType];
                }
                else if (creatureTemplate.TradeskillTrainerType != TradeskillType.None && creatureTemplate.TradeskillTrainerType != TradeskillType.Unknown)
                {
                    creatureDefaultTrainerSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, trainerIDsByTradeskill[creatureTemplate.TradeskillTrainerType]);
                    if (creatureTemplate.MerchantID != 0)
                        creatureTemplate.GossipMenuID = nonClassTrainerGossipMenuIDWithShop;
                    else
                        creatureTemplate.GossipMenuID = nonClassTrainerGossipMenuIDNoShop;
                }
                if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true && creatureTemplate.IsRidingTrainer == true)
                {
                    creatureDefaultTrainerSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, trainerIDForRidingTrainer);
                    if (creatureTemplate.MerchantID != 0)
                        creatureTemplate.GossipMenuID = nonClassTrainerGossipMenuIDWithShop;
                    else
                        creatureTemplate.GossipMenuID = nonClassTrainerGossipMenuIDNoShop;
                }
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
                int moveSpeed = Configuration.TRANSPORT_MOVE_SPEED;
                if (transportShip.FixedSpeed > 0 && Configuration.TRANSPORT_ALLOW_FIXED_SPEEDS == true)
                    moveSpeed = transportShip.FixedSpeed;
                transportsSQL.AddRow(transportShip.WOWGameObjectTemplateID, longName);
                gameObjectTemplateSQL.AddRowForTransportShip(transportShip.WOWGameObjectTemplateID, transportShip.GameObjectDisplayInfoID, name,
                    transportShip.TaxiPathID, Convert.ToInt32(transportShip.MapID), moveSpeed);
                gameObjectTemplateAddonSQL.AddRowForTransport(transportShip.WOWGameObjectTemplateID);
                if (transportShip.TriggeredByGameObjectTemplateID > 0)
                {
                    modEverquestTransportTriggerSQL.AddRow(transportShip.TriggeredByGameObjectTemplateID, transportShip.WOWGameObjectTemplateID,
                        transportShip.TriggeredByStepNum, transportShip.TriggeredActivateStepNum);
                }
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
                gameObjectSQL.AddRow(transportLift.GameObjectGUID, transportLift.GameObjectTemplateID, mapID, areaID, new Vector3(transportLift.SpawnX, transportLift.SpawnY, transportLift.SpawnZ), 
                    transportLift.Orientation, new Quaternion(0, 0, 0, 1), 0, name);
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
                    transportLiftTrigger.SpawnZ), transportLiftTrigger.Orientation, new Quaternion(0, 0, 0, 1), 0,name);
            }
        }

        private void PopulateZoneData(List<Zone> zones, Dictionary<string, ZoneProperties> zonePropertiesByShortName, out Dictionary<string, int> mapIDsByShortName)
        {
            foreach (Zone zone in zones)
            {
                // Instance list
                instanceTemplateSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID));

                // Teleport scripts to safe positions (add a record for both descriptive and short name if they are different)
                gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.DescriptiveNameOnlyLetters, zone.ZoneProperties.TelePosition.X, zone.ZoneProperties.TelePosition.Y, zone.ZoneProperties.TelePosition.Z, zone.ZoneProperties.TeleOrientation);
                if (zone.DescriptiveNameOnlyLetters.ToLower() != zone.ShortName.ToLower())
                    gameTeleSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ShortName, zone.ZoneProperties.TelePosition.X, zone.ZoneProperties.TelePosition.Y, zone.ZoneProperties.TelePosition.Z, zone.ZoneProperties.TeleOrientation);

                // Weather data
                if (Configuration.ZONE_WEATHER_ENABLED == true)
                    gameWeatherSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DefaultZoneArea.DBCAreaTableID), zone.ZoneProperties.RainChanceWinter, zone.ZoneProperties.SnowChanceWinter,
                        zone.ZoneProperties.RainChanceSpring, zone.ZoneProperties.SnowChanceSpring, zone.ZoneProperties.RainChanceSummer, zone.ZoneProperties.SnowChanceSummer,
                        zone.ZoneProperties.RainChanceFall, zone.ZoneProperties.SnowChanceFall);

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
                        graveyard.SpiritHealerX, graveyard.SpiritHealerY, graveyard.SpiritHealerZ, graveyard.SpiritHealerOrientation, CreatureMovementType.None, string.Empty, false);
                }
            }

            // Game Objects
            if (Configuration.GENERATE_OBJECTS == true)
            {
                Dictionary<string, List<GameObject>> gameObjectsByZoneShortNames = GameObject.GetNonDoodadGameObjectsByZoneShortNames();
                foreach (var gameObjectByShortName in gameObjectsByZoneShortNames)
                {
                    // Skip invalid zones
                    if (zonePropertiesByShortName.ContainsKey(gameObjectByShortName.Key) == false)
                        continue;
                    int areaID = 0;
                    foreach (Zone zone in zones)
                        if (zone.ShortName.ToLower().Trim() == gameObjectByShortName.Key)
                            areaID = Convert.ToInt32(zone.DefaultArea.DBCAreaTableID);

                    foreach (GameObject gameObject in gameObjectByShortName.Value)
                    {
                        // Skip invalid objects
                        if (gameObject.GameObjectDisplayInfoID < 0)
                        {
                            Logger.WriteError("Skipping gameobject named '", gameObject.DisplayName, "' since it has no displayinfoID");
                            continue;
                        }
                        string name = gameObject.DisplayName;
                        string comment = string.Concat("EQ ", gameObject.ObjectType.ToString(), " ", gameObject.ZoneShortName, " (", gameObject.ID, ")");

                        // Chest items
                        if (gameObject.ObjectType == GameObjects.GameObjectType.Chest)
                        {
                            if (gameObject.ContainedItemTemplate == null)
                                Logger.WriteError("Skipping loot for gameobject named '", gameObject.DisplayName, "' due to null containedItemTemplate");
                            else
                            {
                                string chestComment = string.Concat("EQ ", gameObject.ZoneShortName, " (", gameObject.ID, ")", " contains ", gameObject.ContainedItemTemplate.Name);
                                gameObjectLootTemplateSQL.AddRow(gameObject.GameObjectTemplateEntryID, gameObject.ContainedItemTemplate.WOWEntryID, chestComment);
                                name = gameObject.ContainedItemTemplate.Name;
                            }
                        }

                        if (name.Length == 0)
                            name = comment;
                        int mapID = mapIDsByShortName[gameObjectByShortName.Key];
                        int spawnTimeInSec = gameObject.RespawnTimeInMS / 1000;
                        if (Configuration.OBJECT_GAMEOBJECT_CHEST_USE_FIXED_RESPAWN_TIMER == true)
                            spawnTimeInSec = Configuration.OBJECT_GAMEOBJECT_CHEST_FIXED_RESPAWN_TIME_IN_SEC;
                        gameObjectSQL.AddRow(gameObject.GameObjectGUID, gameObject.GameObjectTemplateEntryID, mapID, areaID, gameObject.Position, gameObject.Orientation, gameObject.InteractiveRotation, spawnTimeInSec, comment);
                        gameObjectTemplateSQL.AddRowForGameObject(name, gameObject);
                        gameObjectTemplateAddonSQL.AddRowNoDespawn(gameObject.GameObjectTemplateEntryID);
                        if (gameObject.EQIncline != 0)
                            gameObjectAddonSQL.AddRow(gameObject.GameObjectGUID);

                        // Attach any smart scripts
                        if (gameObject.TriggerGameObjectGUID != 0)
                        {
                            string scriptComment = string.Concat("EQ GameObject GUID ", gameObject.GameObjectGUID, " Chain Activates GUID ", gameObject.TriggerGameObjectGUID);
                            smartScriptsSQL.AddRowForGameObjectStateTriggerEvent(gameObject.GameObjectTemplateEntryID, gameObject.TriggerGameObjectGUID, gameObject.TriggerGameObjectTemplateEntryID, scriptComment);
                        }
                        else if (gameObject.ObjectType == GameObjects.GameObjectType.Teleport)
                        {
                            string scriptComment = string.Concat("EQ GameObject GUID ", gameObject.GameObjectGUID, " Teleports to ", gameObject.DestinationZoneShortName);
                            smartScriptsSQL.AddRowForGameObjectTriggeredTeleport(gameObject.GameObjectTemplateEntryID, gameObject.DestinationMapID, gameObject.DestinationPosition.X,
                                gameObject.DestinationPosition.Y, gameObject.DestinationPosition.Z, gameObject.DestinationOrientation, scriptComment);
                        }
                    }
                }
            }
        }

        private void OutputSQLScriptsToDisk()
        {
            // Characters
            characterAuraSQL.SaveToDisk("character_aura", SQLFileType.Characters);
            // World
            areaTriggerSQL.SaveToDisk("areatrigger", SQLFileType.World);
            areaTriggerTeleportSQL.SaveToDisk("areatrigger_teleport", SQLFileType.World);
            broadcastTextSQL.SaveToDisk("broadcast_text", SQLFileType.World);
            conditionsSQL.SaveToDisk("conditions", SQLFileType.World);
            creatureSQL.SaveToDisk("creature", SQLFileType.World);
            creatureAddonSQL.SaveToDisk("creature_addon", SQLFileType.World);
            creatureDefaultTrainerSQL.SaveToDisk("creature_default_trainer", SQLFileType.World);
            creatureEquipTemplateSQL.SaveToDisk("creature_equip_template", SQLFileType.World);
            creatureLootTableSQL.SaveToDisk("creature_loot_template", SQLFileType.World);
            creatureModelInfoSQL.SaveToDisk("creature_model_info", SQLFileType.World);
            creatureQuestEnderSQL.SaveToDisk("creature_questender", SQLFileType.World);
            creatureQuestStarterSQL.SaveToDisk("creature_queststarter", SQLFileType.World);
            creatureTemplateSQL.SaveToDisk("creature_template", SQLFileType.World);
            creatureTemplateModelSQL.SaveToDisk("creature_template_model", SQLFileType.World);
            creatureTemplateSpellSQL.SaveToDisk("creature_template_spell", SQLFileType.World);
            creatureTextSQL.SaveToDisk("creature_text", SQLFileType.World);
            fishingLootTemplateSQL.SaveToDisk("fishing_loot_template", SQLFileType.World);
            gameEventSQL.SaveToDisk("game_event", SQLFileType.World);
            gameEventCreatureSQL.SaveToDisk("game_event_creature", SQLFileType.World);
            gameEventPoolSQL.SaveToDisk("game_event_pool", SQLFileType.World);
            gameGraveyardSQL.SaveToDisk("game_graveyard", SQLFileType.World);
            gameObjectSQL.SaveToDisk("gameobject", SQLFileType.World);
            gameObjectAddonSQL.SaveToDisk("gameobject_addon", SQLFileType.World);
            gameObjectLootTemplateSQL.SaveToDisk("gameobject_loot_template", SQLFileType.World);
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
            modEverquestCreatureSQL.SaveToDisk("mod_everquest_creature", SQLFileType.World);
            modEverquestCreatureInstanceSQL.SaveToDisk("mod_everquest_creature_instance", SQLFileType.World);
            modEverquestCreatureOnkillReputationSQL.SaveToDisk("mod_everquest_creature_onkill_reputation", SQLFileType.World);
            modEverquestCreatureWaypointSQL.SaveToDisk("mod_everquest_creature_waypoint", SQLFileType.World);
            modEverquestForageZoneItemsSQL.SaveToDisk("mod_everquest_forage_zone_items", SQLFileType.World);
            modEverquestItemTemplateSQL.SaveToDisk("mod_everquest_item_template", SQLFileType.World);
            modEverquestPetSQL.SaveToDisk("mod_everquest_pet", SQLFileType.World);
            modEverquestPlayerCreateInfoSQL.SaveToDisk("mod_everquest_playercreateinfo", SQLFileType.World);
            modEverquestPlayerAutoLearnSkillsSQL.SaveToDisk("mod_everquest_playerautolearnskills", SQLFileType.World);
            modEverquestPlayerAutoLearnSpellsSQL.SaveToDisk("mod_everquest_playerautolearnspells", SQLFileType.World);
            modEverquestSpellSQL.SaveToDisk("mod_everquest_spell", SQLFileType.World);
            modEverquestSystemConfigsSQL.SaveToDisk("mod_everquest_systemconfigs", SQLFileType.World);
            modEverquestQuestCompleteReputationSQL.SaveToDisk("mod_everquest_quest_complete_reputation", SQLFileType.World);
            modEverquestTransportTriggerSQL.SaveToDisk("mod_everquest_transport_trigger", SQLFileType.World);
            modEverquestQuestReactionSQL.SaveToDisk("mod_everquest_quest_reaction", SQLFileType.World);
            npcTextSQL.SaveToDisk("npc_text", SQLFileType.World);
            npcVendorSQL.SaveToDisk("npc_vendor", SQLFileType.World);
            pageTextSQL.SaveToDisk("page_text", SQLFileType.World);
            petNameGenerationSQL.SaveToDisk("pet_name_generation", SQLFileType.World);
            playerClassStatsSQL.SaveToDisk("player_class_stats", SQLFileType.World);
            playerCreateInfoSpellCustomSQL.SaveToDisk("playercreateinfo_spell_custom", SQLFileType.World);
            poolCreatureSQL.SaveToDisk("pool_creature", SQLFileType.World);
            poolPoolSQL.SaveToDisk("pool_pool", SQLFileType.World);
            poolTemplateSQL.SaveToDisk("pool_template", SQLFileType.World);
            questTemplateSQL.SaveToDisk("quest_template", SQLFileType.World);
            questTemplateAddonSQL.SaveToDisk("quest_template_addon", SQLFileType.World);
            referenceLootTemplateSQL.SaveToDisk("reference_loot_template", SQLFileType.World);
            skillFishingBaseLevelSQL.SaveToDisk("skill_fishing_base_level", SQLFileType.World);
            smartScriptsSQL.SaveToDisk("smart_scripts", SQLFileType.World);
            spellBonusDataSQL.SaveToDisk("spell_bonus_data", SQLFileType.World);
            spellGroupSQL.SaveToDisk("spell_group", SQLFileType.World);
            spellGroupStackRulesSQL.SaveToDisk("spell_group_stack_rules", SQLFileType.World);
            spellLinkedSpellSQL.SaveToDisk("spell_linked_spell", SQLFileType.World);
            spellScriptNamesSQL.SaveToDisk("spell_script_names", SQLFileType.World);
            spellTargetPositionSQL.SaveToDisk("spell_target_position", SQLFileType.World);
            trainerSQL.SaveToDisk("trainer", SQLFileType.World);
            trainerSpellSQL.SaveToDisk("trainer_spell", SQLFileType.World);
            transportsSQL.SaveToDisk("transports", SQLFileType.World);
            waypointDataSQL.SaveToDisk("waypoint_data", SQLFileType.World);
        }

        public void DeployServerSQL()
        {
            Logger.WriteInfo("Deploying sql to server...");

            // Deploy all of the scripts
            string currentScriptFileName = string.Empty;
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
                        currentScriptFileName = sqlFile;
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
                        currentScriptFileName = sqlFile;
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
                Logger.WriteError("Error occurred when executing a SQL script: '", currentScriptFileName, "' with message: " + ex.Message + "'");
                if (ex.StackTrace != null)
                    Logger.WriteDebug(ex.StackTrace.ToString());
                Logger.WriteError("Deploying sql to server failed.");
            }

            Logger.WriteDebug("Deploying sql to server complete");
        }
    }
}
