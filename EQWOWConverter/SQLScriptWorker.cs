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
using System.Text;
using EQWOWConverter.Zones;
using MySql.Data.MySqlClient;

namespace EQWOWConverter
{
    internal class SQLScriptWorker
    {
        // World
        private AchievementRewardSQL achievementRewardSQL = new AchievementRewardSQL();
        private AreaTriggerSQL areaTriggerSQL = new AreaTriggerSQL();
        private AreaTriggerTeleportSQL areaTriggerTeleportSQL = new AreaTriggerTeleportSQL();
        private BroadcastTextSQL broadcastTextSQL = new BroadcastTextSQL();
        private ConditionsSQL conditionsSQL = new ConditionsSQL();
        private CreatureSQL creatureSQL = new CreatureSQL();
        private CreatureAddonSQL creatureAddonSQL = new CreatureAddonSQL();
        private CreatureDefaultTrainerSQL creatureDefaultTrainerSQL = new CreatureDefaultTrainerSQL();
        private CreatureEquipTemplateSQL creatureEquipTemplateSQL = new CreatureEquipTemplateSQL();
        private CreatureImmunitiesSQL creatureImmunitiesSQL = new CreatureImmunitiesSQL();
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
        private GameTableDBCSQL gtChanceToSpellCritBaseDBCSQL = new GameTableDBCSQL("gtchancetospellcritbase_dbc");
        private GameTableDBCSQL gtChanceToSpellCritDBCSQL = new GameTableDBCSQL("gtchancetospellcrit_dbc");
        private InstanceTemplateSQL instanceTemplateSQL = new InstanceTemplateSQL();
        private ItemLootTemplateSQL itemLootTemplateSQL = new ItemLootTemplateSQL();
        private ItemTemplateSQL itemTemplateSQL = new ItemTemplateSQL();
        private ModEverquestClassMapSQL modEverquestClassMapSQL = new ModEverquestClassMapSQL();
        private ModEverquestCreatureSQL modEverquestCreatureSQL = new ModEverquestCreatureSQL();
        private ModEverquestCreatureInstanceSQL modEverquestCreatureInstanceSQL = new ModEverquestCreatureInstanceSQL();
        private ModEverquestCreatureLootSQL modEverquestCreatureLootSQL = new ModEverquestCreatureLootSQL();
        private ModEverquestCreatureOnkillReputationSQL modEverquestCreatureOnkillReputationSQL = new ModEverquestCreatureOnkillReputationSQL();
        private ModEverquestCreatureEmoteSQL modEverquestCreatureEmoteSQL = new ModEverquestCreatureEmoteSQL();
        private ModEverquestCreatureKillSpawnSQL modEverquestCreatureKillSpawnSQL = new ModEverquestCreatureKillSpawnSQL();
        private ModEverquestCreatureMovementSoundSQL modEverquestCreatureMovementSoundSQL = new ModEverquestCreatureMovementSoundSQL();
        private ModEverquestCreatureSpawnPointSQL modEverquestCreatureSpawnPointSQL = new ModEverquestCreatureSpawnPointSQL();
        private ModEverquestCreatureWaypointSQL modEverquestCreatureWaypointSQL = new ModEverquestCreatureWaypointSQL();
        private ModEverquestFactionSQL modEverquestFactionSQL = new ModEverquestFactionSQL();
        private ModEverquestForageZoneItemsSQL modEverquestForageZoneItemsSQL = new ModEverquestForageZoneItemsSQL();
        private ModEverquestIllusionDisplaySQL modEverquestIllusionDisplaySQL = new ModEverquestIllusionDisplaySQL();
        private ModEverquestIllusionFaceSQL modEverquestIllusionFaceSQL = new ModEverquestIllusionFaceSQL();
        private ModEverquestItemTemplateSQL modEverquestItemTemplateSQL = new ModEverquestItemTemplateSQL();
        private ModEverquestPetSQL modEverquestPetSQL = new ModEverquestPetSQL();
        private ModEverquestPlayerCreateInfoSQL modEverquestPlayerCreateInfoSQL = new ModEverquestPlayerCreateInfoSQL();
        private ModEverquestPlayerAutoLearnSkillsSQL modEverquestPlayerAutoLearnSkillsSQL = new ModEverquestPlayerAutoLearnSkillsSQL();
        private ModEverquestPlayerAutoLearnSpellsSQL modEverquestPlayerAutoLearnSpellsSQL = new ModEverquestPlayerAutoLearnSpellsSQL();
        private ModEverquestPlayerAutoAddItemsSQL modEverquestPlayerAutoAddItemsSQL = new ModEverquestPlayerAutoAddItemsSQL();
        private ModEverquestSpellSQL modEverquestSpellSQL = new ModEverquestSpellSQL();
        private ModEverquestSystemConfigsSQL modEverquestSystemConfigsSQL = new ModEverquestSystemConfigsSQL();
        private ModEverquestTransportTriggerSQL modEverquestTransportTriggerSQL = new ModEverquestTransportTriggerSQL();
        private ModEverquestZoneSafePointSQL modEverquestZoneSafePointSQL = new ModEverquestZoneSafePointSQL();
        private ModEverquestZoneSQL modEverquestZoneSQL = new ModEverquestZoneSQL();
        private ModEverquestQuestCompleteReputationSQL modEverquestQuestCompleteReputationSQL = new ModEverquestQuestCompleteReputationSQL();
        private ModEverquestQuestReactionSQL modEverquestQuestReactionSQL = new ModEverquestQuestReactionSQL();
        private ModEverquestGossipReactionSQL modEverquestGossipReactionSQL = new ModEverquestGossipReactionSQL();
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
            List<CreatureSpawnPool> creatureSpawnPools, Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID,
            Dictionary<int, List<CreatureLootEntry>> creatureLootEntriesByCreatureTemplateID, List<QuestTemplate> questTemplates,
            List<TradeskillRecipe> tradeskillRecipes, List<SpellTemplate> spellTemplates, List<GameEvent> gameEvents)
        {
            Logger.WriteInfo("Creating SQL Scripts...");

            // Clean the folder
            string sqlScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts");
            if (Directory.Exists(sqlScriptFolder))
                Directory.Delete(sqlScriptFolder, true);

            // System configs
            PopulateSystemConfigs();

            // Game tables (per-class stat scaling)
            PopulateGameTableData();

            // Creature factions
            PopulateCreatureFactionData();

            // Achievements
            PopulateAchievementData(creatureTemplates);

            // Zones
            Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();
            Dictionary<string, int> mapIDsByShortName;
            PopulateZoneData(zones, zonePropertiesByShortName, out mapIDsByShortName);

            // Trainer Abilities (Class and Profession)
            PopulateTrainerData(creatureTemplates);

            // Talk-triggered (gossip) reactions, before creature data so the gossip flags are set prior to creature_template rows generating
            PopulateCreatureGossipData();

            // Creature say/emote events, also before creature data since 'hailed' emotes set gossip flags on the creature templates
            PopulateCreatureEmoteData();

            // Creatures
            Dictionary<int, SpellTemplate> spellTemplatesByEQID = SpellTemplate.GetSpellTemplatesByEQID();
            PopulateCreatureData(creatureTemplates, creatureModelTemplates, creatureSpawnPools, spellTemplatesByEQID, mapIDsByShortName);

            // Kill-triggered spawns
            PopulateCreatureKillSpawnData(mapIDsByShortName);

            // Game Events
            foreach (GameEvent gameEvent in gameEvents)
                gameEventSQL.AddRow(gameEvent);

            // Items
            PopulateItemData(itemLootTemplatesByCreatureTemplateID, creatureLootEntriesByCreatureTemplateID, spellTemplatesByEQID);

            // Illusion appearance displays
            PopulateIllusionDisplayData(creatureModelTemplates);

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

        private void PopulateCreatureFactionData()
        {
            foreach (CreatureFaction creatureFaction in CreatureFaction.GetCreatureFactionsByFactionID().Values)
                modEverquestFactionSQL.AddRow(creatureFaction.FactionTemplateID, creatureFaction.WillDefendFriendlyPlayers, creatureFaction.DefendersWillAttackToDefendPlayer, creatureFaction.DefendCombatFactionTemplateID);
        }

        private void PopulateSystemConfigs()
        {
            modEverquestSystemConfigsSQL.AddRow("ModVersion", Configuration.CONFIGONLY_CORE_MOD_VERSION.ToString());
            modEverquestSystemConfigsSQL.AddRow("BardMaxConcurrentSongs", Configuration.SPELL_MAX_CONCURRENT_BARD_SONGS.ToString());
            modEverquestSystemConfigsSQL.AddRow("CreatureTemplateIDMin", Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW.ToString());
            modEverquestSystemConfigsSQL.AddRow("CreatureTemplateIDMax", Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH.ToString());
            modEverquestSystemConfigsSQL.AddRow("DazeEnabledInEQZones", Configuration.COMBAT_DAZE_IN_EQ_ZONES_ENABLED == true ? "1" : "0");
            modEverquestSystemConfigsSQL.AddRow("DeathKnightsStartLikeOtherClasses", Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true ? "1" : "0");
            modEverquestSystemConfigsSQL.AddRow("GameObjectTemplateIDMin", Configuration.SQL_GAMEOBJECTTEMPLATE_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("GameObjectTemplateIDMax", Configuration.SQL_GAMEOBJECTTEMPLATE_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("InvisVsUndeadDetectSpellID", Configuration.SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID.ToString());
            modEverquestSystemConfigsSQL.AddRow("ItemTemplateIDMin", Configuration.SQL_ITEM_TEMPLATE_ENTRY_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("ItemTemplateIDMax", Configuration.SQL_ITEM_TEMPLATE_ENTRY_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("LegacyAchievementID", Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_ENABLED == true ? Configuration.DBCID_ACHIEVEMENT_ID_START.ToString() : "0");
            modEverquestSystemConfigsSQL.AddRow("LegacyAchievementAccountCreatedBefore", Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_ENABLED == true ? Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_CREATED_BEFORE_DATE : "");
            modEverquestSystemConfigsSQL.AddRow("AdventurerAchievementID", Configuration.ACHIEVEMENT_EQ_ADVENTURER_ENABLED == true ? (Configuration.DBCID_ACHIEVEMENT_ID_START + 1).ToString() : "0");
            modEverquestSystemConfigsSQL.AddRow("AdventurerAuraSpellID", Configuration.ACHIEVEMENT_EQ_ADVENTURER_ENABLED == true ? Configuration.SPELL_EQ_ADVENTURER_AURA_SPELL_ID.ToString() : "0");
            modEverquestSystemConfigsSQL.AddRow("AdventurerAchievementLevel", Configuration.ACHIEVEMENT_EQ_ADVENTURER_LEVEL.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMin", Configuration.DBCID_MAP_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("MapDBCIDMax", Configuration.DBCID_MAP_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("ShipEntryTemplateIDMin", Configuration.SQL_GAMEOBJECTTEMPLATE_SHIP_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("ShipEntryTemplateIDMax", Configuration.SQL_GAMEOBJECTTEMPLATE_SHIP_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMin", Configuration.DBCID_SPELL_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("SpellDBCIDMax", Configuration.DBCID_SPELL_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMin", Configuration.SQL_QUEST_TEMPLATE_ID_START.ToString());
            modEverquestSystemConfigsSQL.AddRow("QuestSQLIDMax", Configuration.SQL_QUEST_TEMPLATE_ID_END.ToString());
            modEverquestSystemConfigsSQL.AddRow("WorldScale", Configuration.GENERATE_WORLD_SCALE.ToString());
            modEverquestSystemConfigsSQL.AddRow("RangedAttackSpellID", Configuration.COMBATSKILL_RANGED_ENABLED == true ? Configuration.COMBATSKILL_RANGED_SPELL_ID.ToString() : "0");
            modEverquestSystemConfigsSQL.AddRow("ResistAdjustmentSpellID", Configuration.SPELL_RESIST_ADJUSTMENT_SPELL_ID.ToString());
        }

        private void PopulateGameTableData()
        {
            // The stock spell crit game tables have all-zero rows for Warrior (1), Rogue (4) and DeathKnight (6), making their spell crit a hard 0% in combat
            // So pull the data from a donor table.  This has to be saved in the database for AzerothCore for the server to use it
            string dbcInputFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "ExportedDBCFiles");
            int donorClassID = Configuration.PLAYER_STAT_GAMETABLE_FILL_DONOR_CLASS_ID;
            List<int> zeroedClassIDs = new List<int>() { 1, 4, 6 }; // Warrior, Rogue, DeathKnight

            GameTableDBC spellCritBaseDBC = new GameTableDBC();
            spellCritBaseDBC.LoadFromDisk(dbcInputFolder, "gtChanceToSpellCritBase.dbc");
            GameTableDBC spellCritDBC = new GameTableDBC();
            spellCritDBC.LoadFromDisk(dbcInputFolder, "gtChanceToSpellCrit.dbc");
            foreach (int zeroedClassID in zeroedClassIDs)
            {
                gtChanceToSpellCritBaseDBCSQL.AddRow(zeroedClassID - 1, spellCritBaseDBC.GetSingleFloatValue(donorClassID - 1));
                for (int levelRow = 0; levelRow < 100; levelRow++)
                    gtChanceToSpellCritDBCSQL.AddRow((zeroedClassID - 1) * 100 + levelRow, spellCritDBC.GetSingleFloatValue((donorClassID - 1) * 100 + levelRow));
            }
        }

        private void PopulateAchievementData(List<CreatureTemplate> creatureTemplates)
        {
            if (Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_ENABLED == false && Configuration.ACHIEVEMENT_EQ_ADVENTURER_ENABLED == false)
                return;

            // The reward mail 'from' name shown in the client is the name of the sender creature template
            int senderCreatureTemplateID = 0;
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                if (creatureTemplate.Name == Configuration.ACHIEVEMENT_MAIL_SENDER_CREATURE_NAME)
                {
                    senderCreatureTemplateID = creatureTemplate.WOWCreatureTemplateID;
                    break;
                }
            }
            if (senderCreatureTemplateID == 0)
            {
                Logger.WriteError("An achievement was enabled but no creature template named '" + Configuration.ACHIEVEMENT_MAIL_SENDER_CREATURE_NAME + "' could be found in CreatureTemplates.csv, so no reward mail will be sent");
                return;
            }

            if (Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_ENABLED == true)
                achievementRewardSQL.AddRow(Configuration.DBCID_ACHIEVEMENT_ID_START, Configuration.ACHIEVEMENT_TUTORIAL_PORT_STONE_WOW_ITEM_ID, senderCreatureTemplateID,
                    Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_NAME, Configuration.ACHIEVEMENT_LEGACY_ACCOUNT_MAIL_BODY_TEXT);
            if (Configuration.ACHIEVEMENT_EQ_ADVENTURER_ENABLED == true)
                achievementRewardSQL.AddRow(Configuration.DBCID_ACHIEVEMENT_ID_START + 1, Configuration.ACHIEVEMENT_TUTORIAL_PORT_STONE_WOW_ITEM_ID, senderCreatureTemplateID,
                    Configuration.ACHIEVEMENT_EQ_ADVENTURER_NAME, Configuration.ACHIEVEMENT_EQ_ADVENTURER_MAIL_BODY_TEXT);
        }

        private void PopulateCreatureGossipData()
        {
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();

            // Group the reactions by the creature they attach to
            Dictionary<(string, string), List<QuestGossipReaction>> gossipReactionsByZoneAndCreatureName = new Dictionary<(string, string), List<QuestGossipReaction>>();
            foreach (QuestGossipReaction gossipReaction in QuestGossipReaction.GetGossipReactions())
            {
                (string, string) key = (gossipReaction.ZoneShortName, gossipReaction.CreatureName);
                if (gossipReactionsByZoneAndCreatureName.ContainsKey(key) == false)
                    gossipReactionsByZoneAndCreatureName.Add(key, new List<QuestGossipReaction>());
                gossipReactionsByZoneAndCreatureName[key].Add(gossipReaction);
            }

            foreach (var gossipReactionsForCreature in gossipReactionsByZoneAndCreatureName)
            {
                string zoneShortName = gossipReactionsForCreature.Key.Item1;
                string creatureName = gossipReactionsForCreature.Key.Item2;
                List<QuestGossipReaction> gossipReactions = gossipReactionsForCreature.Value;
                List<CreatureTemplate> gossipCreatureTemplates = CreatureTemplate.GetCreatureTemplatesForSpawnZonesAndName(zoneShortName, creatureName);
                if (gossipCreatureTemplates.Count == 0)
                {
                    Logger.WriteDebug(string.Concat("Skipping gossip reactions with zone '", zoneShortName, "' and name '", creatureName, "' as no creature template could be found"));
                    continue;
                }

                // Greeting text shown when the gossip window opens
                string menuText = gossipReactions[0].MenuText;
                if (menuText.Length == 0)
                    menuText = "Greetings, $N.";
                int menuBroadcastTextID = IDGenerationTool.GenerateID("BroadcastTextID", "gossipgreeting", zoneShortName, creatureName);
                broadcastTextSQL.AddRow(menuBroadcastTextID, menuText, menuText);
                int menuNPCTextID = IDGenerationTool.GenerateID("NPCTextID", "gossipgreeting", zoneShortName, creatureName);
                npcTextSQL.AddRow(menuNPCTextID, menuText, menuBroadcastTextID);

                foreach (CreatureTemplate gossipCreatureTemplate in gossipCreatureTemplates)
                {
                    gossipCreatureTemplate.HasGossipReactions = true;
                    foreach (QuestGossipReaction gossipReaction in gossipReactions)
                    {
                        int targetCreatureTemplateID = 0;
                        if (gossipReaction.CreatureEQID > 0)
                        {
                            if (creatureTemplatesByEQID.ContainsKey(gossipReaction.CreatureEQID) == false)
                            {
                                Logger.WriteDebug(string.Concat("Skipping gossip reaction for creature '", creatureName, "' as the target creature EQID of ", gossipReaction.CreatureEQID.ToString(), " could not be found"));
                                continue;
                            }
                            targetCreatureTemplateID = creatureTemplatesByEQID[gossipReaction.CreatureEQID].WOWCreatureTemplateID;
                        }
                        if (gossipReaction.CreatureIsSelf == true)
                            targetCreatureTemplateID = gossipCreatureTemplate.WOWCreatureTemplateID;
                        modEverquestGossipReactionSQL.AddRow(gossipCreatureTemplate.WOWCreatureTemplateID, menuNPCTextID, gossipReaction, targetCreatureTemplateID);
                    }
                }
            }
        }

        private void PopulateCreatureEmoteData()
        {
            // Event emotes
            Dictionary<int, List<CreatureEmote>> emotesByEQEmoteSetID = CreatureEmote.GetEmotesByEQEmoteSetID();
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            foreach (CreatureTemplate creatureTemplate in creatureTemplatesByEQID.Values)
            {
                if (creatureTemplate.EQEmoteSetID == 0)
                    continue;
                if (emotesByEQEmoteSetID.ContainsKey(creatureTemplate.EQEmoteSetID) == false)
                {
                    Logger.WriteDebug("Skipping creature emotes for creature template '", creatureTemplate.EQCreatureTemplateID.ToString(), "' since emote set '", creatureTemplate.EQEmoteSetID.ToString(), "' has no rows");
                    continue;
                }
                foreach (CreatureEmote emote in emotesByEQEmoteSetID[creatureTemplate.EQEmoteSetID])
                {
                    if (emote.EventType == CreatureEmoteEventType.Hailed)
                        creatureTemplate.HasHailedEmote = true;
                    modEverquestCreatureEmoteSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, emote);
                }
            }

            // Ambient emotes
            foreach (var ambientEmotesForCreature in CreatureEmote.GetAmbientEmotesByEQCreatureTemplateID())
            {
                if (creatureTemplatesByEQID.ContainsKey(ambientEmotesForCreature.Key) == false)
                {
                    Logger.WriteDebug("Skipping ambient emotes for creature template '" + ambientEmotesForCreature.Key + "' since no creature template could be found");
                    continue;
                }
                CreatureTemplate creatureTemplate = creatureTemplatesByEQID[ambientEmotesForCreature.Key];
                foreach (CreatureEmote emote in ambientEmotesForCreature.Value)
                    modEverquestCreatureEmoteSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, emote);
            }
        }

        private void PopulateCreatureKillSpawnData(Dictionary<string, int> mapIDsByShortName)
        {
            Dictionary<int, CreatureTemplate> creatureTemplatesByEQID = CreatureTemplate.GetCreatureTemplateListByEQID();
            foreach (CreatureKillSpawn killSpawn in CreatureKillSpawn.GetKillSpawnList())
            {
                if (mapIDsByShortName.ContainsKey(killSpawn.ZoneShortName) == false)
                {
                    Logger.WriteDebug("Skipping creature kill spawn '" + killSpawn.ID + "' since zone '" + killSpawn.ZoneShortName + "' has no map");
                    continue;
                }
                if (creatureTemplatesByEQID.ContainsKey(killSpawn.TriggerEQCreatureTemplateID) == false)
                {
                    Logger.WriteDebug("Skipping creature kill spawn '" + killSpawn.ID + "' since trigger creature '" + killSpawn.TriggerEQCreatureTemplateID + "' has no template");
                    continue;
                }
                int targetWOWID = 0;
                if (killSpawn.TargetEQCreatureTemplateID > 0)
                {
                    if (creatureTemplatesByEQID.ContainsKey(killSpawn.TargetEQCreatureTemplateID) == false)
                    {
                        Logger.WriteDebug("Skipping creature kill spawn '" + killSpawn.ID + "' since target creature '" + killSpawn.TargetEQCreatureTemplateID + "' has no template");
                        continue;
                    }
                    targetWOWID = creatureTemplatesByEQID[killSpawn.TargetEQCreatureTemplateID].WOWCreatureTemplateID;
                }
                int onlyIfNotAliveWOWID = 0;
                if (killSpawn.OnlyIfNotAliveEQCreatureTemplateID > 0 && creatureTemplatesByEQID.ContainsKey(killSpawn.OnlyIfNotAliveEQCreatureTemplateID))
                    onlyIfNotAliveWOWID = creatureTemplatesByEQID[killSpawn.OnlyIfNotAliveEQCreatureTemplateID].WOWCreatureTemplateID;
                List<string> requireDeadWOWIDs = new List<string>();
                foreach (int eqID in killSpawn.RequireDeadEQCreatureTemplateIDs)
                    if (creatureTemplatesByEQID.ContainsKey(eqID))
                        requireDeadWOWIDs.Add(creatureTemplatesByEQID[eqID].WOWCreatureTemplateID.ToString());
                List<string> requireAliveWOWIDs = new List<string>();
                foreach (int eqID in killSpawn.RequireAliveEQCreatureTemplateIDs)
                    if (creatureTemplatesByEQID.ContainsKey(eqID))
                        requireAliveWOWIDs.Add(creatureTemplatesByEQID[eqID].WOWCreatureTemplateID.ToString());
                modEverquestCreatureKillSpawnSQL.AddRow(killSpawn.ID, creatureTemplatesByEQID[killSpawn.TriggerEQCreatureTemplateID].WOWCreatureTemplateID,
                    Convert.ToInt32(killSpawn.TriggerType), mapIDsByShortName[killSpawn.ZoneShortName], Convert.ToInt32(killSpawn.ActionType), targetWOWID, killSpawn.Chance,
                    killSpawn.AltGroup, killSpawn.AltID, killSpawn.AltWeight, killSpawn.SpawnAtCorpse, killSpawn.XPosition, killSpawn.YPosition,
                    killSpawn.ZPosition, killSpawn.Orientation, killSpawn.DelayMinMS, killSpawn.DelayMaxMS, onlyIfNotAliveWOWID,
                    string.Join(",", requireDeadWOWIDs), string.Join(",", requireAliveWOWIDs), killSpawn.AddToHateList,
                    killSpawn.TriggerMinLevel, killSpawn.TriggerMaxLevel, killSpawn.RespawnTimeInSec, killSpawn.Comment);
            }
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
                    int creatureSQLGUID = IDGenerationTool.GenerateID("CreatureGUID", "spawn", spawnInstance.ID.ToString(), creatureTemplate.EQCreatureTemplateID.ToString());
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

                    // Cycle groups (capped, with near-instant EQ respawn times, like the Trakanon's Teeth forager/hunter cycles) get every point-and-candidate combination as a plain spawn so any candidate can appear at any point.
                    bool isCycleSpawnGroup = hasRealSpawnCap;
                    if (isCycleSpawnGroup == true)
                        foreach (CreatureSpawnInstance spawnInstance in spawnPool.CreatureSpawnInstances)
                            if (spawnInstance.RespawnTimeInSeconds <= 0 || spawnInstance.RespawnTimeInSeconds > Configuration.CREATURE_SPAWN_CYCLE_MAX_EQ_RESPAWN_TIME_IN_SEC)
                                isCycleSpawnGroup = false;
                    if (isCycleSpawnGroup == true)
                    {
                        foreach (CreatureSpawnInstance spawnInstance in spawnPool.CreatureSpawnInstances)
                        {
                            for (int i = 0; i < spawnPool.CreatureTemplates.Count; i++)
                            {
                                CreatureTemplate template = spawnPool.CreatureTemplates[i];
                                int chance = spawnPool.CreatureTemplateChances[i];
                                int guid = IDGenerationTool.GenerateID("CreatureGUID", "spawn", spawnInstance.ID.ToString(), template.EQCreatureTemplateID.ToString(), i.ToString());
                                modEverquestCreatureSpawnPointSQL.AddRow(guid, spawnInstance.MapID, spawnInstance.ID, spawnPool.SpawnGroup.ID, spawnPool.SpawnLimit,
                                    spawnInstance.RespawnTimeInSeconds, chance);
                                string comment = string.Concat(template.Name, " - EQ Group: ", spawnPool.SpawnGroup.ID, ", EQ NPC ID: ", template.EQCreatureTemplateID, ", EQ Instance ID: ", spawnInstance.ID);
                                CreateCreatureAndRelatedSQLEntries(guid, template, spawnInstance, spawnPool.SpawnGroup, comment, Configuration.CREATURE_SPAWN_CYCLE_MEMBER_RESPAWN_TIME_IN_SEC);
                                if (spawnPool.LinkedSpawnGameEvent != null)
                                    gameEventCreatureSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, guid, true);
                                if (spawnPool.LinkedDespawnGameEvent != null)
                                    gameEventCreatureSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, guid, false);
                            }
                        }
                    }
                    else if (hasRealSpawnCap == true)
                    {
                        int poolID = IDGenerationTool.GenerateID("PoolTemplateID", "cappedpool", spawnPool.SpawnGroup.ID.ToString());
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
                            int creatureGUID = IDGenerationTool.GenerateID("CreatureGUID", "spawn", spawnInstance.ID.ToString(), creatureTemplate.EQCreatureTemplateID.ToString());
                            poolCreatureSQL.AddRow(creatureGUID, poolID, 0, creatureTemplate.Name);
                            modEverquestCreatureSpawnPointSQL.AddRow(creatureGUID, spawnInstance.MapID, spawnInstance.ID, spawnPool.SpawnGroup.ID, spawnPool.SpawnLimit, 0, 0);
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
                            int creatureSQLGUID = IDGenerationTool.GenerateID("CreatureGUID", "spawn", spawnInstance.ID.ToString(), template.EQCreatureTemplateID.ToString());
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
                            int poolID = IDGenerationTool.GenerateID("PoolTemplateID", "weightedpool", spawnPool.SpawnGroup.ID.ToString(), spawnInstance.ID.ToString());
                            poolTemplateSQL.AddRow(poolID, poolDescription, 1);
                            if (spawnPool.LinkedSpawnGameEvent != null)
                                gameEventPoolSQL.AddRow(spawnPool.LinkedSpawnGameEvent.GameEventsSQLID, poolID, true);
                            if (spawnPool.LinkedDespawnGameEvent != null)
                                gameEventPoolSQL.AddRow(spawnPool.LinkedDespawnGameEvent.GameEventsSQLID, poolID, false);
                            for (int i = 0; i < spawnPool.CreatureTemplates.Count; i++)
                            {
                                CreatureTemplate template = spawnPool.CreatureTemplates[i];
                                int chance = spawnPool.CreatureTemplateChances[i];
                                int guid = IDGenerationTool.GenerateID("CreatureGUID", "spawn", spawnInstance.ID.ToString(), template.EQCreatureTemplateID.ToString(), i.ToString());
                                poolCreatureSQL.AddRow(guid, poolID, chance, template.Name);
                                modEverquestCreatureSpawnPointSQL.AddRow(guid, spawnInstance.MapID, spawnInstance.ID, spawnPool.SpawnGroup.ID, 0, 0, 0);
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
                int menuBroadcastTextID = IDGenerationTool.GenerateID("BroadcastTextID", "podcooldown");
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT);
                int menuNPCTextID = IDGenerationTool.GenerateID("NPCTextID", "podcooldown");
                priestOfDiscordCooldownMenuNPCTextID = menuNPCTextID;
                npcTextSQL.AddRow(menuNPCTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_CANT_PORT_GOSSIP_TEXT, menuBroadcastTextID);

                // Teleporting to Azeroth
                azerothTeleportLocations = CreatureTeleportLocationAzeroth.GetAllTeleportLocations();
                norrathPriestOfDiscordGossipMenuID = IDGenerationTool.GenerateID("GossipMenuID", "podnorrath");
                menuBroadcastTextID = IDGenerationTool.GenerateID("BroadcastTextID", "podnorrathmenu");
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_NORRATH_GOSSIP_TEXT);
                menuNPCTextID = IDGenerationTool.GenerateID("NPCTextID", "podnorrathmenu");
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
                    int menuBroadcastID = IDGenerationTool.GenerateID("BroadcastTextID", "podnorrathoption", curMenuOptionID.ToString(), teleportLocation.MenuItemText);
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
                azerothPriestOfDiscordGossipMenuID = IDGenerationTool.GenerateID("GossipMenuID", "podazeroth");
                menuBroadcastTextID = IDGenerationTool.GenerateID("BroadcastTextID", "podazerothmenu");
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT, Configuration.CREATURE_PRIEST_OF_DISCORD_TELEPORTER_AZEROTH_GOSSIP_TEXT);
                menuNPCTextID = IDGenerationTool.GenerateID("NPCTextID", "podazerothmenu");
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
                    int menuBroadcastID = IDGenerationTool.GenerateID("BroadcastTextID", "podazerothoption", curMenuOptionID.ToString(), teleportLocation.MenuItemText);
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

            // Planes teleportation
            List<CreatureTeleportLocationPlanes> planesTeleportLocations;
            SortedDictionary<int, CreatureTeleportLocationPlanes> planesTeleportLocationByMenuOptionID = new SortedDictionary<int, CreatureTeleportLocationPlanes>();
            int planesTeleporterGossipMenuID = -1;
            if (Configuration.GENERANE_ENABLE_PLANES_TELEPORTATION == true)
            {
                // Base menu
                int menuBroadcastTextID = IDGenerationTool.GenerateID("BroadcastTextID", "planesmenu");
                broadcastTextSQL.AddRow(menuBroadcastTextID, Configuration.CREATURE_PLANES_TELEPORTER_GOSSIP_TEXT, Configuration.CREATURE_PLANES_TELEPORTER_GOSSIP_TEXT);
                int menuNPCTextID = IDGenerationTool.GenerateID("NPCTextID", "planesmenu");
                planesTeleportLocations = CreatureTeleportLocationPlanes.GetAllTeleportLocations();
                planesTeleporterGossipMenuID = IDGenerationTool.GenerateID("GossipMenuID", "planes");
                npcTextSQL.AddRow(menuNPCTextID, Configuration.CREATURE_PLANES_TELEPORTER_GOSSIP_TEXT, menuBroadcastTextID);
                gossipMenuSQL.AddRow(planesTeleporterGossipMenuID, menuNPCTextID);

                // Teleports
                int curMenuOptionID = 0;
                foreach (CreatureTeleportLocationPlanes teleportLocation in planesTeleportLocations)
                {
                    // Broadcast
                    int menuBroadcastID = IDGenerationTool.GenerateID("BroadcastTextID", "planesoption", curMenuOptionID.ToString(), teleportLocation.MenuItemText);
                    broadcastTextSQL.AddRow(menuBroadcastID, teleportLocation.MenuItemText, teleportLocation.MenuItemText);

                    // Menu Option
                    gossipMenuOptionSQL.AddRow(planesTeleporterGossipMenuID, curMenuOptionID, 0, teleportLocation.MenuItemText, menuBroadcastID, 1, 1, 0);

                    // Save
                    planesTeleportLocationByMenuOptionID.Add(curMenuOptionID, teleportLocation);
                    curMenuOptionID++;
                }
            }

            // Creature Templates
            Dictionary<int, List<CreatureVendorItem>> vendorItems = CreatureVendorItem.GetCreatureVendorItemsByMerchantIDs();
            List<CreatureVendorItem> reagentVendorItems = CreatureVendorItem.GetCreatureReagentItems();
            // Distinct crowd-control immunity masks get a shared creature_immunities row so the table stays tiny
            Dictionary<long, int> creatureImmunitiesIdByMask = new Dictionary<long, int>();
            foreach (CreatureTemplate creatureTemplate in creatureTemplates)
            {
                // Skip invalid creatures
                if (creatureTemplate.ModelTemplate == null)
                {
                    Logger.WriteError("Error generating azeroth core scripts since model template was null for creature template '" + creatureTemplate.Name + "'");
                    continue;
                }

                // Simplified logic for debug mode
                if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true && creatureTemplate.IsWaypointDebugCreature == true)
                {
                    creatureTemplateSQL.AddRow(creatureTemplate);
                    creatureTemplateModelSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureTemplate.ModelTemplate.DBCCreatureDisplayID,
                        Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);
                    int creatureGUID = IDGenerationTool.GenerateID("CreatureGUID", "debugwaypoint", creatureTemplate.WOWCreatureTemplateID.ToString());
                    string comment = string.Concat(creatureTemplate.Name, " - EQ Debug Creature");
                    creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, creatureTemplate.SpawnWaypointDebugMapID, creatureTemplate.SpawnWaypointDebugAreaID,
                        creatureTemplate.SpawnWaypointDebugAreaID, creatureTemplate.SpawnWaypointDebugXPosition, creatureTemplate.SpawnWaypointDebugYPosition,
                        creatureTemplate.SpawnWaypointDebugZPosition, 0, CreatureMovementType.None, GetCreatureRespawnTimeInSeconds(creatureTemplate), comment, false);
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

                // If there are any planes teleports
                if (Configuration.GENERANE_ENABLE_PLANES_TELEPORTATION == true && creatureTemplate.IsPlaneTeleporter == true)
                {
                    creatureTemplate.HasSmartScript = true;
                    creatureTemplate.GossipMenuID = planesTeleporterGossipMenuID;
                    foreach (var planesTeleportLocationByGossipMenuOptionID in planesTeleportLocationByMenuOptionID)
                    {
                        CreatureTeleportLocationPlanes curTeleportLocation = planesTeleportLocationByGossipMenuOptionID.Value;
                        if (mapIDsByShortName.ContainsKey(curTeleportLocation.ZoneShortName) == false)
                            continue;
                        int mapID = mapIDsByShortName[curTeleportLocation.ZoneShortName];
                        string comment = string.Concat("EQ teleport player to Planes ('", curTeleportLocation.MenuItemText, "')");
                        smartScriptsSQL.AddRowForMenuOptionTriggeredTeleport(creatureTemplate.WOWCreatureTemplateID, planesTeleporterGossipMenuID, planesTeleportLocationByGossipMenuOptionID.Key,
                            mapID, curTeleportLocation.XPosition, curTeleportLocation.YPosition, curTeleportLocation.ZPosition, curTeleportLocation.Orientation, comment);
                    }
                }

                // Ranged attack data, if any
                int rangedMinRangeWOW = creatureTemplate.RangedAttackMinRangeEQ > 0 ? Convert.ToInt32(Math.Round(creatureTemplate.RangedAttackMinRangeEQ * Configuration.GENERATE_WORLD_SCALE)) : 0;
                int rangedMaxRangeWOW = creatureTemplate.RangedAttackMaxRangeEQ > 0 ? Convert.ToInt32(Math.Round(creatureTemplate.RangedAttackMaxRangeEQ * Configuration.GENERATE_WORLD_SCALE)) : 0;

                // Rampage range is an EQ distance, so scale it into WOW units (0 = use default)
                int rampageRangeWOW = creatureTemplate.RampageRangeEQ > 0 ? Convert.ToInt32(Math.Round(creatureTemplate.RampageRangeEQ * Configuration.GENERATE_WORLD_SCALE)) : 0;

                // Mod data
                modEverquestCreatureSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, creatureTemplate.Race.CanHoldVisualItems, creatureTemplate.Race.CanHoldVisualShields,
                    creatureTemplate.SpawnLimit, creatureTemplate.HasRangedAttackAbility, rangedMinRangeWOW, rangedMaxRangeWOW, creatureTemplate.RangedAttackDamageModPercent,
                    creatureTemplate.AgroSocialDistanceMod, creatureTemplate.HasEnrageAbility, creatureTemplate.EnrageHPPercent, creatureTemplate.EnrageDurationInMS,
                    creatureTemplate.EnrageCooldownInMS, creatureTemplate.HasFlurryAbility, creatureTemplate.FlurryChancePercent, creatureTemplate.HasRampageAbility,
                    creatureTemplate.RampageChancePercent, rampageRangeWOW, creatureTemplate.RampageDamagePercent, creatureTemplate.HasWildRampageAbility,
                    creatureTemplate.WildRampageChancePercent, creatureTemplate.WildRampageMaxTargets, creatureTemplate.WildRampageDamagePercent,
                    creatureTemplate.EQAttackRoundTimeInMS);

                // Determine the display id
                int displayID = creatureTemplate.ModelTemplate.DBCCreatureDisplayID;
                if (creatureTemplate.IsNonNPC == true)
                    displayID = 11686; // Dranei totem

                // Crowd control immunities
                if (creatureTemplate.MechanicImmuneMask != 0)
                {
                    if (creatureImmunitiesIdByMask.TryGetValue(creatureTemplate.MechanicImmuneMask, out int existingImmunitiesId) == true)
                        creatureTemplate.CreatureImmunitiesId = existingImmunitiesId;
                    else
                    {
                        int creatureImmunitiesId = IDGenerationTool.GenerateID("CreatureImmunitiesID", creatureTemplate.MechanicImmuneMask.ToString());
                        creatureTemplate.CreatureImmunitiesId = creatureImmunitiesId;
                        creatureImmunitiesIdByMask.Add(creatureTemplate.MechanicImmuneMask, creatureImmunitiesId);
                        creatureImmunitiesSQL.AddRow(creatureImmunitiesId, creatureTemplate.MechanicImmuneMask);
                    }
                }

                // See invis/stealth
                if (creatureTemplate.SeesInvisible == true || creatureTemplate.SeesStealth == true)
                {
                    string seeComment = string.Concat("EQ See Invisibility/Stealth ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ")");
                    smartScriptsSQL.AddRowForCreatureTemplateApplyAuraOnSpawn(creatureTemplate.WOWCreatureTemplateID,
                        Configuration.SPELL_CREATURE_SEE_INVIS_AND_STEALTH_SPELL_ID, seeComment);
                }

                // Create the records
                // This scale ensures that creature held equipment is the right size
                float scale = creatureTemplate.Size * creatureTemplate.Race.SpawnSizeMod * (Configuration.GENERATE_CREATURE_SCALE / Configuration.GENERATE_EQUIPMENT_SCALE);

                // Companion pets normalize to a uniform world height
                if (creatureTemplate.IsCompanionPet == true && creatureTemplate.ModelTemplate.ModelStandingHeight > Configuration.GENERATE_FLOAT_EPSILON)
                    scale = (Configuration.CREATURE_COMPANION_PETS_MODEL_HEIGHT / creatureTemplate.ModelTemplate.ModelStandingHeight) * creatureTemplate.CompanionPetSizeMod;
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
                                    foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByEQClassType)
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
                    // Reduce mana regeneration on spell-casting creatures, if configured
                    if (Configuration.CREATURE_MANA_REGEN_PERCENT < 100)
                    {
                        string manaRegenComment = string.Concat("EQ Reduce Mana Regen ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateApplyAuraOnSpawn(creatureTemplate.WOWCreatureTemplateID,
                            Configuration.SPELL_CREATURE_REDUCED_MANA_REGEN_SPELL_ID, manaRegenComment);
                    }

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

                        // Gates detrimental casts behind per-type rolls and honor priority order (lower number = preferred)
                        int eventChance = CreatureSpellEntry.GetCombatSpellEventChance(creatureSpellEntry.TypeFlags, creatureSpellEntry.Priority);

                        string comment = string.Concat("EQ In Combat ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateInCombatSpellCast(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, curSpellTemplate.WOWSpellID, comment, eventChance, curSpellTemplate.IsSelfCenteredAreaBreath);
                    }

                    // Add spell events for every in-combat self buff entry (cast on self, not the victim)
                    foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesInCombatBuff)
                    {
                        SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                        string comment = string.Concat("EQ In Combat Self Buff ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                        smartScriptsSQL.AddRowForCreatureTemplateInCombatSelfBuffCast(creatureTemplate.WOWCreatureTemplateID,
                            creatureSpellEntry.CalculatedMinimumDelayInMS, Configuration.CREATURE_SPELL_INCOMBAT_BUFF_CAST_CHANCE, curSpellTemplate.WOWSpellID, comment);
                    }

                    // Add escape spells
                    if (creatureTemplate.IsPet == false)
                    {
                        foreach (CreatureSpellEntry creatureSpellEntry in creatureTemplate.CreatureSpellEntriesEscape)
                        {
                            SpellTemplate curSpellTemplate = spellTemplatesByEQID[creatureSpellEntry.EQSpellID];
                            string comment = string.Concat("EQ Escape ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast ", curSpellTemplate.Name, " (", curSpellTemplate.WOWSpellID, ")");
                            smartScriptsSQL.AddRowForCreatureTemplateEscapeSelfCast(creatureTemplate.WOWCreatureTemplateID, Configuration.CREATURE_SPELL_ESCAPE_HEALTH_TRIGGER_PCT, Configuration.CREATURE_SPELL_ESCAPE_CAST_CHANCE,
                                Configuration.CREATURE_SPELL_ESCAPE_RECAST_DELAY_IN_MS, curSpellTemplate.WOWSpellID, comment);
                        }
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
                            curSpellTemplate.WOWSpellID, Configuration.CREATURE_SPELL_ATTACK_PROC_COOLDOWN_IN_MS, comment);
                    }

                    // Summons need to add an aura to the caster
                    if (creatureTemplate.DoesSummonPets == true)
                    {
                        smartScriptsSQL.AddRowForCreatureTemplateCastOnSummoned(creatureTemplate.WOWCreatureTemplateID, Configuration.SPELL_SUMMON_CASTER_AURA_SPELL_ID,
                            string.Concat("EQ Summon Pet Add Summoner Aura to Caster ", creatureTemplate.Name, "(", creatureTemplate.WOWCreatureTemplateID, ")"));
                    }
                }

                // Assign bash to creatures
                if (creatureTemplate.UsesBash == true && creatureTemplate.IsPet == false)
                {
                    string comment = string.Concat("EQ Bash ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast Bash (", Configuration.COMBATSKILL_BASH_SPELL_ID, ")");
                    smartScriptsSQL.AddRowForCreatureTemplateInCombatSpellCast(creatureTemplate.WOWCreatureTemplateID, Configuration.COMBATSKILL_BASH_COOLDOWN_IN_MS, Configuration.COMBATSKILL_BASH_SPELL_ID, comment);
                }

                // Assign Harm Touch to creatures
                if (creatureTemplate.UsesHarmTouch == true && creatureTemplate.IsPet == false)
                {
                    string comment = string.Concat("EQ Harm Touch ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast Harm Touch (", Configuration.COMBATSKILL_HARMTOUCH_SPELL_ID, ")");
                    smartScriptsSQL.AddRowForCreatureTemplateAttackSpellCastOnVictimNoResetOnLeaveCombat(creatureTemplate.WOWCreatureTemplateID,
                        Configuration.COMBATSKILL_HARMTOUCH_CREATURE_INITIAL_DELAY_IN_MS, Configuration.COMBATSKILL_HARMTOUCH_COOLDOWN_IN_MS, Configuration.COMBATSKILL_HARMTOUCH_SPELL_ID, comment);
                }

                // Assign Lay on Hands to creatures
                if (creatureTemplate.UsesLayOnHands == true && creatureTemplate.IsPet == false)
                {
                    string comment = string.Concat("EQ Lay on Hands ", creatureTemplate.Name, " (", creatureTemplate.WOWCreatureTemplateID, ") cast Lay on Hands (", Configuration.COMBATSKILL_LAYONHANDS_SPELL_ID, ")");
                    smartScriptsSQL.AddRowForCreatureTemplateLowHealthSelfCastNoResetOnLeaveCombat(creatureTemplate.WOWCreatureTemplateID,
                        Configuration.COMBATSKILL_LAYONHANDS_HEALTH_TRIGGER_PCT, Configuration.COMBATSKILL_LAYONHANDS_COOLDOWN_IN_MS, Configuration.COMBATSKILL_LAYONHANDS_SPELL_ID, comment);
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

                    // Assign bash to pets
                    if (creatureTemplate.UsesBash == true)
                    {
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, Configuration.COMBATSKILL_BASH_SPELL_ID);
                        curIndex++;
                    }

                    // Assign Harm Touch to pets (are there any?)
                    if (creatureTemplate.UsesHarmTouch == true)
                    {
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, Configuration.COMBATSKILL_HARMTOUCH_SPELL_ID);
                        curIndex++;
                    }

                    // Assign Lay on Hands to pets (are there any?)
                    if (creatureTemplate.UsesLayOnHands == true)
                    {
                        creatureTemplateSpellSQL.AddRow(creatureTemplate.WOWCreatureTemplateID, curIndex, Configuration.COMBATSKILL_LAYONHANDS_SPELL_ID);
                        curIndex++;
                    }
                }
            }

            // Creature models
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                creatureModelInfoSQL.AddRow(creatureModelTemplate.DBCCreatureDisplayID, Convert.ToInt32(creatureModelTemplate.GenderType));

                // When playing walk/run sounds through the mod (and not an illusion form or a silent companion pet), need to send the data to the server
                if (Configuration.AUDIO_CREATURE_MOVEMENT_SOUNDS_FROM_MOD_ENABLED == true && creatureModelTemplate.FaceIndex != CreatureModelTemplate.ILLUSION_REPLACEABLE_FACE_INDEX
                    && creatureModelTemplate.IsCompanionPetVersion == false)
                {
                    AddCreatureMovementSoundRowIfNeeded(creatureModelTemplate, creatureModelTemplate.DBCCreatureDisplayID);
                }
            }
            
            // Azeroth creatures for teleports
            if (Configuration.GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION == true)
            {
                // Creatures themselves
                List<CreatureTeleporterInAzeroth> creatureTeleporters = CreatureTeleporterInAzeroth.GetAllCreatureTeleporters();
                Dictionary<int, CreatureTemplate> creatureTemplatesByWOWID = CreatureTemplate.GetCreatureTemplateListByWOWID();
                if (creatureTemplatesByWOWID.ContainsKey(Configuration.GENERATE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID) == false)
                {
                    Logger.WriteError("GENERATE_ENABLE_PRIEST_OF_DISCORD_WORLD_TRANSPORTATION was true but no creature template with ID as defined by GENERATE_ENABLE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID could be found.");
                }
                else
                {
                    foreach (CreatureTeleporterInAzeroth creatureTeleporter in creatureTeleporters)
                    {
                        string creatureComment = string.Concat("EQ Azeroth Priest of Discord");
                        int creatureGUID = IDGenerationTool.GenerateID("CreatureGUID", "azerothteleporter", creatureTeleporter.MapID.ToString(), creatureTeleporter.AreaID.ToString(), creatureTeleporter.XPosition.ToString("0"));
                        creatureSQL.AddRow(creatureGUID, Configuration.GENERATE_PRIST_OF_DISCORD_WORLD_TRANSPORTATION_CREATURE_TEMPLATE_ID, creatureTeleporter.MapID,
                            creatureTeleporter.AreaID, creatureTeleporter.AreaID, creatureTeleporter.XPosition, creatureTeleporter.YPosition,
                            creatureTeleporter.ZPosition, creatureTeleporter.Orientation, CreatureMovementType.None, 300, creatureComment, false);
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

        private static int GetCreatureRespawnTimeInSeconds(CreatureTemplate creatureTemplate, CreatureSpawnInstance? spawnInstance = null)
        {
            switch (creatureTemplate.DifficultyType)
            {
                case CreatureDifficultyType.RaidBoss:
                case CreatureDifficultyType.RaidTrash:
                    {
                        int maxRespawnTimeInSec;
                        if (creatureTemplate.DifficultyType == CreatureDifficultyType.RaidBoss)
                            maxRespawnTimeInSec = Configuration.CREATURE_RAID_BOSS_RESPAWN_MAX_TIME_IN_SEC;
                        else
                            maxRespawnTimeInSec = Configuration.CREATURE_RAID_TRASH_RESPAWN_MAX_TIME_IN_SEC;
                        if (spawnInstance == null || spawnInstance.RespawnTimeInSeconds <= 0)
                            return maxRespawnTimeInSec;

                        // TAKP respawns in "respawntime +/- (variance / 2)", so the minimum (fastest) respawn is "respawntime - (variance / 2)"
                        int minRespawnTimeInSec = Math.Max(1, spawnInstance.RespawnTimeInSeconds - (spawnInstance.Variance / 2));
                        return Math.Min(minRespawnTimeInSec, maxRespawnTimeInSec);
                    }
                default: return 300;
            }
        }

        private static Dictionary<int, HashSet<int>> alreadySavedCustomWaypointGridIDsByMapID = new Dictionary<int, HashSet<int>>(); // Ensure only 1 of each waypoint set is saved
        private void CreateCreatureAndRelatedSQLEntries(int creatureGUID, CreatureTemplate creatureTemplate, CreatureSpawnInstance spawnInstance, CreatureSpawnGroup spawnGroup, string comment, int respawnTimeOverrideInSec = 0)
        {
            int respawnTimeInSec;
            if (respawnTimeOverrideInSec > 0)
                respawnTimeInSec = respawnTimeOverrideInSec;
            else
                respawnTimeInSec = GetCreatureRespawnTimeInSeconds(creatureTemplate, spawnInstance);
            List<CreaturePathGridEntry> pathEntries = spawnInstance.GetPathGridEntries();
            CreatureMovementType movementType = CreatureMovementType.None;
            CreaturePathGridWanderType wanderType = spawnInstance.GetPathGrid().WanderType;

            // Grant the "invis vs undead" detect aura on spawn to anything that should see through it (non-undead + see_invis_undead)
            string creatureAddonAuras = creatureTemplate.CanSeeThroughInvisVsUndead() == true ? Configuration.SPELL_CREATURE_INVIS_VS_UNDEAD_DETECT_SPELL_ID.ToString() : "";
            if (spawnGroup.DoesRoam() == true)
            {
                if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                    creatureTemplate.SubName = spawnInstance.ID + " Roams";
                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID, spawnInstance.SpawnStandState, creatureAddonAuras);
                modEverquestCreatureInstanceSQL.AddRow(creatureGUID, wanderType, spawnInstance.GetPathGrid().PauseType, spawnInstance.MapID, spawnInstance.GetPathGrid().GridID,
                    true, spawnGroup.RoamMinX, spawnGroup.RoamMaxX, spawnGroup.RoamMinY, spawnGroup.RoamMaxY, spawnGroup.RoamMinZ, spawnGroup.RoamMaxZ, spawnGroup.RoamMinDelayInMS, 
                    spawnGroup.RoamMaxDelayInMS);
                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                    spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, respawnTimeInSec, comment, true);
            }
            else if (pathEntries.Count > 0 && wanderType != CreaturePathGridWanderType.None)
            {
                // Only about half of waypoint types are handled directly by the AzerothCore game engine
                if (wanderType == CreaturePathGridWanderType.GridRandom10 || wanderType == CreaturePathGridWanderType.GridRandom
                    || wanderType == CreaturePathGridWanderType.GridRand5LoS || wanderType == CreaturePathGridWanderType.GridRandomCenterPoint
                    || wanderType == CreaturePathGridWanderType.GridRandomPath)
                {
                    creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID, spawnInstance.SpawnStandState, creatureAddonAuras);
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
                        spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, respawnTimeInSec, comment, true);
                }
                else
                {
                    int waypointGUID = creatureGUID * 1000;
                    if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                        waypointGUID = creatureGUID * 10;
                    creatureAddonSQL.AddRow(creatureGUID, waypointGUID, creatureTemplate.DefaultEmoteID, spawnInstance.SpawnStandState, creatureAddonAuras);
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

                    if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true && pathEntries.Count > 0)
                        creatureTemplate.SubName = spawnInstance.ID + " WP " + pathEntries[0].GridID;

                    movementType = CreatureMovementType.Path;
                    creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                        spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, respawnTimeInSec, comment, useModScript);
                }
            }
            else
            {
                if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                    creatureTemplate.SubName = spawnInstance.ID + " Immobile";
                creatureAddonSQL.AddRow(creatureGUID, 0, creatureTemplate.DefaultEmoteID, spawnInstance.SpawnStandState, creatureAddonAuras);
                creatureSQL.AddRow(creatureGUID, creatureTemplate.WOWCreatureTemplateID, spawnInstance.MapID, spawnInstance.AreaID, spawnInstance.AreaID, spawnInstance.SpawnXPosition,
                    spawnInstance.SpawnYPosition, spawnInstance.SpawnZPosition, spawnInstance.Orientation, movementType, respawnTimeInSec, comment, false);
            }
        }

        private void PopulateItemData(Dictionary<int, List<ItemLootTemplate>> itemLootTemplatesByCreatureTemplateID,
            Dictionary<int, List<CreatureLootEntry>> creatureLootEntriesByCreatureTemplateID, Dictionary<int, SpellTemplate> spellTemplatesByEQID)
        {
            SortedDictionary<int, ItemTemplate> itemTemplatesByWOWID = ItemTemplate.GetItemTemplatesByWOWEntryID();
            HashSet<int> addedLearnScrollItemIDs = new HashSet<int>();
            foreach (ItemTemplate itemTemplate in ItemTemplate.GetItemTemplatesByEQDBIDs().Values)
            {
                if (itemTemplate.IsExistingItemAlready == true)
                    continue;

                // Save any additional metadata
                int creatureWornEffectSpellID = itemTemplate.GetCreatureGrantableWornEffectSpellID(spellTemplatesByEQID);
                int illusionBodySet = CreatureIllusionTintPalette.GetBodySetForEQArmorMaterialType(itemTemplate.EQArmorMaterialType);
                if (illusionBodySet < 0)
                    illusionBodySet = 0;
                int illusionTintID = CreatureIllusionTintPalette.GetTintIDForColorPacked(itemTemplate.ColorPacked);
                modEverquestItemTemplateSQL.AddRow(itemTemplate.WOWEntryID, itemTemplate.WOWEntryID, creatureWornEffectSpellID, itemTemplate.AllowedClassTypesEQ,
                    illusionBodySet, illusionTintID);

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
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.GetDescriptionStringWithAddedAllowedClasses(itemTemplate.AllowedClassTypesEQ), itemTemplate.RequiredLevel,
                            itemTemplate.ItemDisplayInfo);
                    }
                    else
                    {
                        // If it is a valid spell scroll, there is one scroll per class that can learn it
                        SpellTemplate spellTemplate = spellTemplatesByEQID[itemTemplate.EQScrollSpellID];
                        itemTemplate.ClassID = 9;
                        itemTemplate.SubClassID = 0;
                        itemTemplate.WOWSpellID1 = spellTemplate.WOWSpellID;
                        itemTemplate.Description = string.Concat("Teaches the spell: ", spellTemplate.Name);
                        foreach (var scrollPropertiesByClassType in spellTemplate.LearnScrollPropertiesByEQClassType)
                        {
                            string scrollName = itemTemplate.Name;
                            if (spellTemplate.LearnScrollPropertiesByEQClassType.Count != 1)
                                scrollName = string.Concat(itemTemplate.Name, " (", scrollPropertiesByClassType.Key.ToString(), ")");
                            itemTemplateSQL.AddRow(itemTemplate, scrollPropertiesByClassType.Value.WOWItemTemplateID, scrollName,
                                itemTemplate.GetDescriptionStringWithAddedAllowedClasses(new List<ClassEQType>() { scrollPropertiesByClassType.Key }), scrollPropertiesByClassType.Value.LearnLevel, itemTemplate.ItemDisplayInfo);

                            // This also needs a metadata item
                            if (addedLearnScrollItemIDs.Contains(scrollPropertiesByClassType.Value.WOWItemTemplateID) == false)
                            {
                                modEverquestItemTemplateSQL.AddRow(scrollPropertiesByClassType.Value.WOWItemTemplateID, scrollPropertiesByClassType.Value.WOWItemTemplateID,
                                    creatureWornEffectSpellID, new List<ClassEQType>() { scrollPropertiesByClassType.Key }, illusionBodySet, illusionTintID);
                                addedLearnScrollItemIDs.Add(scrollPropertiesByClassType.Value.WOWItemTemplateID);
                            }
                        }
                    }
                }
                else
                {
                    // Factor for starter versions
                    itemTemplateSQL.AddRow(itemTemplate, itemTemplate.WOWEntryID, itemTemplate.Name, itemTemplate.GetDescriptionStringWithAddedAllowedClasses(itemTemplate.AllowedClassTypesEQ), itemTemplate.RequiredLevel, itemTemplate.ItemDisplayInfo);
                    if (itemTemplate.StarterVersionItemTemplateID > 0)
                    {
                        string startVersionName = itemTemplate.Name;
                        if (startVersionName.Contains("*") == false)
                            startVersionName = String.Concat(startVersionName, "*");
                        itemTemplateSQL.AddRow(itemTemplate, itemTemplate.StarterVersionItemTemplateID, startVersionName, itemTemplate.GetDescriptionStringWithAddedAllowedClasses(itemTemplate.AllowedClassTypesEQ), itemTemplate.RequiredLevel, itemTemplate.ItemDisplayInfo);
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

            foreach (var creatureLootEntriesForCreature in creatureLootEntriesByCreatureTemplateID.Values)
                foreach (CreatureLootEntry creatureLootEntry in creatureLootEntriesForCreature)
                    modEverquestCreatureLootSQL.AddRow(creatureLootEntry);

            // Page text (for books)
            foreach (ItemTemplate.BookText bookText in ItemTemplate.GetAllBookTexts())
                pageTextSQL.AddRow(bookText.PageTextID, bookText.Text);
        }

        private HashSet<int> creatureMovementSoundAddedDisplayIDs = new HashSet<int>();

        private void AddCreatureMovementSoundRowIfNeeded(CreatureModelTemplate creatureModelTemplate, int displayID)
        {
            if (creatureMovementSoundAddedDisplayIDs.Contains(displayID) == true)
                return;
            CreatureRace race = creatureModelTemplate.Race;
            string walkSoundEntryIDs;
            string walkSoundDurationsMS;
            GetCreatureMovementSoundPieceLists(race.SoundWalkingName, race.SoundMaxDistance, out walkSoundEntryIDs, out walkSoundDurationsMS);
            string runSoundEntryIDs;
            string runSoundDurationsMS;
            GetCreatureMovementSoundPieceLists(race.SoundRunningName, race.SoundMaxDistance, out runSoundEntryIDs, out runSoundDurationsMS);
            if (walkSoundEntryIDs.Length == 0 && runSoundEntryIDs.Length == 0)
                return;
            creatureMovementSoundAddedDisplayIDs.Add(displayID);
            modEverquestCreatureMovementSoundSQL.AddRow(displayID, walkSoundEntryIDs, walkSoundDurationsMS, runSoundEntryIDs, runSoundDurationsMS, race.SoundMaxDistance);
        }

        private void GetCreatureMovementSoundPieceLists(string soundFileName, int soundMaxDistance, out string pieceSoundEntryIDs, out string pieceSoundDurationsMS)
        {
            pieceSoundEntryIDs = string.Empty;
            pieceSoundDurationsMS = string.Empty;
            CreatureMovementSoundSet? movementSoundSet = CreatureRace.GetMovementSoundSet(soundFileName, soundMaxDistance);
            if (movementSoundSet == null)
                return;
            StringBuilder soundEntryIDsSB = new StringBuilder();
            StringBuilder soundDurationsSB = new StringBuilder();
            for (int i = 0; i < movementSoundSet.PieceSounds.Count; i++)
            {
                if (i > 0)
                {
                    soundEntryIDsSB.Append(";");
                    soundDurationsSB.Append(";");
                }
                soundEntryIDsSB.Append(movementSoundSet.PieceSounds[i].DBCID);
                soundDurationsSB.Append(movementSoundSet.PieceDurationsMS[i]);
            }
            pieceSoundEntryIDs = soundEntryIDsSB.ToString();
            pieceSoundDurationsMS = soundDurationsSB.ToString();
        }

        private void PopulateIllusionDisplayData(List<CreatureModelTemplate> creatureModelTemplates)
        {
            // These rows are generated during creature model file generation, so that must run before this
            List<CreatureIllusionDisplayRow> displayRows = CreatureIllusionVersionRegistry.GetDisplayRows();

            // Keep only unique primary key
            HashSet<string> addedRowKeys = new HashSet<string>();
            foreach (CreatureIllusionDisplayRow displayRow in displayRows)
            {
                string rowKey = string.Concat(displayRow.FormSpellID, "|", displayRow.BodySet, "|", displayRow.TintID, "|", displayRow.HelmOn);
                if (addedRowKeys.Contains(rowKey) == true)
                    continue;
                addedRowKeys.Add(rowKey);
                modEverquestIllusionDisplaySQL.AddRow(displayRow.FormSpellID, displayRow.BodySet, displayRow.TintID, displayRow.HelmOn, displayRow.DisplayID);
            }

            // Selectable faces per illusion. These face displays are always in CreatureDisplayInfo.dbc since DBCFileWorker generates the DBC rows from the
            // same template data, and each face display also gets a creature_model_info row
            foreach (CreatureModelTemplate creatureModelTemplate in creatureModelTemplates)
            {
                foreach (var faceDisplayIDByFaceIndex in creatureModelTemplate.IllusionFaceDisplayIDsByFaceIndex)
                {
                    modEverquestIllusionFaceSQL.AddRow(creatureModelTemplate.DBCCreatureDisplayID, faceDisplayIDByFaceIndex.Key, faceDisplayIDByFaceIndex.Value);
                    creatureModelInfoSQL.AddRow(faceDisplayIDByFaceIndex.Value, Convert.ToInt32(creatureModelTemplate.GenderType));
                }
            }
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
            int junkReferenceID = IDGenerationTool.GenerateID("ReferenceLootTemplateID", "junkfishing");
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

            // Class map
            foreach (PlayerClassMapping classMapping in PlayerClassMapping.GetClassMappingsByWOWClass().Values)
                modEverquestClassMapSQL.AddRow(classMapping.WOWClass, classMapping.BaseEQClass, classMapping.DefaultSecondEQClass, classMapping.AllowedSecondEQClasses);

            // Item Adds, Spell and Skill Learns - By EQ Class
            Dictionary<ClassEQType, PlayerEQClassProperties> eqClassPropertiesByEQClass = PlayerEQClassProperties.GetAllEQClassPropertiesByEQClass();
            foreach (PlayerEQClassProperties eqClassProperties in eqClassPropertiesByEQClass.Values)
            {
                foreach (RaceType raceType in Enum.GetValues(typeof(RaceType)))
                {
                    if (raceType == RaceType.All)
                        continue;

                    // Bind/Gate
                    if (Configuration.PLAYER_ADD_CUSTOM_BIND_AND_GATE_ON_START == true)
                    {
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.SPELLS_BINDCUSTOM_SPELLDBC_ID, 1);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.SPELLS_GATECUSTOM_SPELLDBC_ID, 1);
                    }

                    // Forage
                    if (eqClassProperties.HasForage == true)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.FORAGE_SPELL_TEMPLATE_ID, 1);

                    // Bash
                    if (Configuration.COMBATSKILL_BASH_ENABLED == true && Configuration.COMBATSKILL_BASH_PLAYER_LEARNABLE == true && eqClassProperties.HasBash == true)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.COMBATSKILL_BASH_SPELL_ID, 1);

                    // Feign Death
                    if (Configuration.COMBATSKILL_FEIGNDEATH_ENABLED == true && Configuration.COMBATSKILL_FEIGNDEATH_PLAYER_LEARNABLE == true && eqClassProperties.EQClass == ClassEQType.Monk)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.COMBATSKILL_FEIGNDEATH_SPELL_ID, 1);

                    // Harm Touch
                    if (Configuration.COMBATSKILL_HARMTOUCH_ENABLED == true && Configuration.COMBATSKILL_HARMTOUCH_PLAYER_LEARNABLE == true && eqClassProperties.EQClass == ClassEQType.ShadowKnight)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, Configuration.COMBATSKILL_HARMTOUCH_SPELL_ID, 1);

                    // Stealth (Existing WoW version)
                    if (eqClassProperties.EQClass == ClassEQType.Rogue)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 1784, 1);  // Stealth

                    // Auto Shot (Existing WoW version)
                    if (eqClassProperties.EQClass == ClassEQType.Ranger)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 75, 1);  // Auto Shot

                    // Dual Wield (Existing WoW version) - Learned at the EQ-appropriate level.
                    int dualWieldLearnLevel = 0;
                    switch (eqClassProperties.EQClass)
                    {
                        case ClassEQType.Monk: dualWieldLearnLevel = 1; break;
                        case ClassEQType.Rogue: dualWieldLearnLevel = 13; break;
                        case ClassEQType.Warrior: dualWieldLearnLevel = 13; break;
                        case ClassEQType.Ranger: dualWieldLearnLevel = 17; break;
                        case ClassEQType.Bard: dualWieldLearnLevel = 17; break;
                        default: break;
                    }
                    if (dualWieldLearnLevel > 0)
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 674, dualWieldLearnLevel);  // Dual Wield

                    // Auto-added items (not race dependent, so add only once per EQ class)
                    if (raceType == RaceType.Human)
                    {
                        // Shaman gets a Totem of the Earthen Ring (so they can use shaman totems)
                        if (eqClassProperties.EQClass == ClassEQType.Shaman)
                            modEverquestPlayerAutoAddItemsSQL.AddRow(eqClassProperties.EQClass, 46978);
                    }

                    // Shield
                    if (Configuration.PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES == true)
                    {
                        if (raceType == RaceType.Human)
                            modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 433);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 9116, 1);
                    }

                    // Bow
                    if (Configuration.PLAYER_SKILL_ENABLE_BOWS_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES == true && eqClassProperties.HasBow == true)
                    {
                        if (raceType == RaceType.Human)
                            modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 45);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 264, 1);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 3018, 1); // Shoot
                    }

                    // Thrown
                    if (Configuration.PLAYER_SKILL_ENABLE_THROWN_ON_ALL_APPROPRIATE_EQ_ALIGNED_CLASSES == true && eqClassProperties.HasThrown == true)
                    {
                        if (raceType == RaceType.Human)
                            modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 176);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 2567, 1);
                        modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 2764, 1); // Throw
                    }

                    // Melee weapon skills
                    if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES == true)
                    {
                        if (eqClassProperties.Has1HAxe == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 44);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 196, 1);
                        }
                        if (eqClassProperties.Has2HAxe == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 172);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 197, 1);
                        }
                        if (eqClassProperties.Has1HMace == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 54);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 198, 1);
                        }
                        if (eqClassProperties.Has2HMace == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 160);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 199, 1);
                        }
                        if (eqClassProperties.HasPolearm == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 229);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 200, 1);
                        }
                        if (eqClassProperties.Has1HSword == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 43);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 201, 1);
                        }
                        if (eqClassProperties.Has2HSword == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 55);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 202, 1);
                        }
                        if (eqClassProperties.HasStaff == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 136);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 227, 1);
                        }
                        if (eqClassProperties.HasFistWeapon == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 473);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 15590, 1);
                        }
                        if (eqClassProperties.HasDagger == true)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 173);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 1180, 1);
                        }
                    }

                    // Armor skills
                    if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
                    {
                        ItemWOWArmorSubclassType armorSubClassByClass = PlayerEQClassProperties.GetArmorClassForItemWearableByEQClasses(new List<ClassEQType>() { eqClassProperties.EQClass });
                        if (armorSubClassByClass >= ItemWOWArmorSubclassType.Leather)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 414);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 9077, 1);
                        }
                        if (armorSubClassByClass >= ItemWOWArmorSubclassType.Mail)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 413);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 8737, 1);
                        }
                        if (armorSubClassByClass >= ItemWOWArmorSubclassType.Plate)
                        {
                            if (raceType == RaceType.Human)
                                modEverquestPlayerAutoLearnSkillsSQL.AddRow(eqClassProperties.EQClass, 293);
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassProperties.EQClass, raceType, 750, 1);
                        }

                    }
                }
            }
            // Spell and Skill Learns - By WOW Race
            Dictionary<RaceType, PlayerWOWRaceProperties> wowRacePropertiesByRaceType = PlayerWOWRaceProperties.GetAllWOWRacePropertiesByRaceType();
            foreach (PlayerWOWRaceProperties wowRaceProperties in wowRacePropertiesByRaceType.Values)
            {
                foreach (ClassEQType eqClassType in Enum.GetValues(typeof(ClassEQType)))
                {
                    if (eqClassType == ClassEQType.All || eqClassType == ClassEQType.None)
                        continue;

                    if (Configuration.COMBATSKILL_SLAM_ENABLED == true && Configuration.COMBATSKILL_SLAM_PLAYER_LEARNABLE == true)
                    {
                        // Slam
                        if (wowRaceProperties.HasSlam == true)
                            modEverquestPlayerAutoLearnSpellsSQL.AddRow(eqClassType, wowRaceProperties.WOWRaceType, Configuration.COMBATSKILL_SLAM_SPELL_ID, 1);
                    }
                }
            }

            // DK pre-55 stats
            if (Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true)
            {
                foreach (PlayerClassLevelStats dkLevelStats in PlayerClassLevelStats.GetPre55DKLevelStats())
                    playerClassStatsSQL.AddRow(ClassWOWType.DeathKnight, dkLevelStats.Level, dkLevelStats.BaseHP,
                        dkLevelStats.BaseMana, dkLevelStats.Strength, dkLevelStats.Agility, dkLevelStats.Stamina,
                        dkLevelStats.Intellect, dkLevelStats.Spirit);
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
                            // Creature Text
                            int creatureTextGroupID = 0;
                            if (creatureTextGroupIDsByCreatureTemplateID.ContainsKey(creatureTemplateID) == true)
                            {
                                creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID] += 2;
                                creatureTextGroupID = creatureTextGroupIDsByCreatureTemplateID[creatureTemplateID];
                            }
                            else
                                creatureTextGroupIDsByCreatureTemplateID.Add(creatureTemplateID, 0);

                            // Bake in the names
                            string reactionText = reaction.ReactionValue;
                            if (reaction.ReactionType == QuestReactionType.Emote)
                                reactionText = string.Concat(creatureTemplateByWOWID[creatureTemplateID].Name, " ", reactionText);

                            // Broadcast Text
                            int broadcastID = IDGenerationTool.GenerateID("BroadcastTextID", "questreaction", creatureTemplateID.ToString(), creatureTextGroupID.ToString());
                            broadcastTextSQL.AddRow(broadcastID, reactionText, reactionText);
                            string comment = string.Concat("EQ ", creatureTemplateByWOWID[creatureTemplateID].Name, " Quest ", reaction.ReactionType.ToString());
                            int messageType = 12; // Default to say
                            switch (reaction.ReactionType)
                            {
                                case QuestReactionType.Say: messageType = 12; break;
                                case QuestReactionType.Emote: messageType = 16; break;
                                case QuestReactionType.Yell: messageType = 14; break;
                                default: Logger.WriteError("Unhandled quest reaction type of " + reaction.ReactionType); break;
                            }
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID, messageType, reactionText, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);
                            creatureTextSQL.AddRow(creatureTemplateID, creatureTextGroupID + 1, messageType, reactionText, broadcastID, Configuration.QUESTS_TEXT_DURATION_IN_MS, comment);

                            // Smart Script
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID, firstQuestID, comment);
                            smartScriptsSQL.AddRowForQuestCompleteTalkEvent(creatureTemplateID, creatureTextGroupID + 1, repeatQuestID, comment);
                        }

                        // Attack/Spawn/Despawn/KillSpawn actions
                        if (reaction.ReactionType == QuestReactionType.AttackPlayer || reaction.ReactionType == QuestReactionType.Despawn || reaction.ReactionType == QuestReactionType.Spawn || reaction.ReactionType == QuestReactionType.SpawnUnique || reaction.ReactionType == QuestReactionType.KillSpawn)
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

                            modEverquestQuestReactionSQL.AddRow(firstQuestID, creatureTemplateID, reaction);
                            modEverquestQuestReactionSQL.AddRow(repeatQuestID, creatureTemplateID, reaction);
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

        private void AddSpellChain(SpellTemplate baseSpellTemplate, SpellEffectBlock triggerBlock, int chainedSpellID, string chainedSpellName, bool forceHitTrigger = false)
        {
            bool triggerBlockHasAura = false;
            foreach (SpellEffectWOW triggerEffect in triggerBlock.SpellEffects)
            {
                if (triggerEffect.IsAuraType() == true)
                {
                    triggerBlockHasAura = true;
                    break;
                }
            }

            if (forceHitTrigger == false && baseSpellTemplate.AuraDuration.MaxDurationInMS > 0 && triggerBlockHasAura == true)
                spellLinkedSpellSQL.AddRowForAuraTrigger(triggerBlock.WOWSpellID, chainedSpellID, chainedSpellName);
            else
                spellLinkedSpellSQL.AddRowForHitTrigger(triggerBlock.WOWSpellID, chainedSpellID, chainedSpellName);
        }

        // Recasting a spell should refresh it instead of stack-competing with itself, so every cast group member also gets a single-member
        // subgroup (id = spell id) referenced negatively in the parent group which makes the core skip the group stack rule when comparing
        // a spell against itself.  Might be a better way to do this, but this works.        
        HashSet<int> SelfExceptionSpellGroupSpellIDsAdded = new HashSet<int>();
        private void AddCastSpellGroupMember(int spellGroupStackingID, int wowSpellID)
        {
            spellGroupSQL.AddRow(spellGroupStackingID, wowSpellID);
            if (SelfExceptionSpellGroupSpellIDsAdded.Add(wowSpellID) == true)
                spellGroupSQL.AddRow(wowSpellID, wowSpellID);
            spellGroupSQL.AddRow(spellGroupStackingID, -wowSpellID);
        }

        HashSet<int> PetSpellIDsAdded = new HashSet<int>();
        private void AddSpellDataBlock(SpellTemplate spellTemplate, List<SpellEffectBlock> spellEffectBlocks, string commentFragment, int clickyFixedLevel = 0)
        {
            if (spellEffectBlocks.Count == 0 ||  spellEffectBlocks[0].WOWSpellID <= 0)
                return;

            for (int i = 0; i < spellEffectBlocks.Count; i++)
            {
                SpellEffectBlock curEffectBlock = spellEffectBlocks[i];

                // Mod data
                modEverquestSpellSQL.AddRow(spellTemplate, curEffectBlock.WOWSpellID, commentFragment == " (Worn)", clickyFixedLevel);

                // Spell power
                if (spellTemplate.InfluencedBySpellPower == true && commentFragment != " (Worn)")
                {
                    float directBonus;
                    float dotBonus;
                    spellTemplate.CalculateSpellPowerCoefficientsForBlock(curEffectBlock, out directBonus, out dotBonus);
                    spellBonusDataSQL.AddRow(curEffectBlock.WOWSpellID, directBonus, dotBonus, string.Concat("EQ Spell ", spellTemplate.Name, commentFragment, " Block ", i));
                }

                // Additional effects beyond the first
                if (i > 0)
                    AddSpellChain(spellTemplate, spellEffectBlocks[0], curEffectBlock.WOWSpellID, curEffectBlock.SpellName);
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
            if (spellTemplate.IsCharmSpell == true)
                spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_CharmAuraScript");
            if (spellTemplate.IsllusionSpellParent == true)
                spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_IllusionSpellScript");
            if (spellTemplate.ResistDiff != 0 && commentFragment != " (Worn)")
                spellScriptNamesSQL.AddRow(spellEffectBlocks[0].WOWSpellID, "EverQuest_ResistDiffSpellScript");

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
                foreach (List<SpellEffectBlock> wornSpellEffectBlocks in spellTemplate.ItemWornSpellEffectBlockSets)
                    AddSpellDataBlock(spellTemplate, wornSpellEffectBlocks, " (Worn)");
                AddSpellDataBlock(spellTemplate, spellTemplate.GroupedGoodProcSpellEffectBlocksForOutput, " (Proc)");
                for (int i = 0; i < spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters.Count; i++)
                    AddSpellDataBlock(spellTemplate, spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters[i], " (Clicky)", spellTemplate.ClickySpellParatemers[i].FixedLevel);

                // Stack rules
                foreach (int spellGroupStackingID in spellTemplate.SpellGroupStackingIDs)
                {
                    AddCastSpellGroupMember(spellGroupStackingID, spellTemplate.WOWSpellID);
                    if (spellTemplate.WOWSpellIDProcAndGoodEffect != -1)
                        AddCastSpellGroupMember(spellGroupStackingID, spellTemplate.WOWSpellIDProcAndGoodEffect);
                    for (int clickyIndex = 0; clickyIndex < spellTemplate.ClickySpellParatemers.Count; clickyIndex++)
                        AddCastSpellGroupMember(spellGroupStackingID, spellTemplate.ClickySpellParatemers[clickyIndex].WOWSpellID);
                }

                // Worn auras are item bonuses in EQ that never conflict with cast buffs, so they only join worn-specific groups
                foreach (int wornSpellGroupStackingID in spellTemplate.WornSpellGroupStackingIDs)
                    foreach (List<SpellEffectBlock> wornSpellEffectBlocks in spellTemplate.ItemWornSpellEffectBlockSets)
                        spellGroupSQL.AddRow(wornSpellGroupStackingID, wornSpellEffectBlocks[0].WOWSpellID);

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
                    bool forceHitTrigger = chainedSpellTemplate.ChainAppliesViaHitTrigger;
                    AddSpellChain(spellTemplate, spellTemplate.GroupedBaseSpellEffectBlocksForOutput[0], chainedSpellID, chainedSpellName, forceHitTrigger);
                    foreach (List<SpellEffectBlock> wornSpellEffectBlocks in spellTemplate.ItemWornSpellEffectBlockSets)
                        AddSpellChain(spellTemplate, wornSpellEffectBlocks[0], chainedSpellID, chainedSpellName, forceHitTrigger);
                    if (spellTemplate.WOWSpellIDProcAndGoodEffect > 0)
                        AddSpellChain(spellTemplate, spellTemplate.GroupedGoodProcSpellEffectBlocksForOutput[0], chainedSpellID, chainedSpellName, forceHitTrigger);
                    for (int clickyIndex = 0; clickyIndex < spellTemplate.ClickySpellParatemers.Count; clickyIndex++)
                        AddSpellChain(spellTemplate, spellTemplate.GroupedClickySpellEffectBlocksForOutputBySpellParameters[clickyIndex][0], chainedSpellID, chainedSpellName, forceHitTrigger);
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
                trainerIDsByClass.Add(classType, IDGenerationTool.GenerateID("TrainerID", "class", classType.ToString()));
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
                
                trainerIDsByTradeskill.Add(tradeskillType, IDGenerationTool.GenerateID("TrainerID", "tradeskill", tradeskillType.ToString()));
                trainerSQL.AddRow(trainerIDsByTradeskill[tradeskillType], 2, 0, "What would you like to learn?");
                trainerSpellSQL.AddDevelopmentSkillsForTradeskill(trainerIDsByTradeskill[tradeskillType], tradeskillType);
                foreach (SpellTrainerAbility trainerAbility in SpellTrainerAbility.GetTrainerSpellsForTradeskill(tradeskillType))
                    trainerSpellSQL.AddRow(trainerIDsByTradeskill[tradeskillType], trainerAbility);
            }

            // Trainer Abilities - Riding Trainer
            int trainerIDForRidingTrainer = IDGenerationTool.GenerateID("TrainerID", "riding");
            trainerSQL.AddRow(trainerIDForRidingTrainer, 1, 0, "What would you like to learn?");
            trainerSpellSQL.AddRiderSkills(trainerIDForRidingTrainer);

            // Pre-generate class trainer menus
            Dictionary<ClassWOWType, int> classTrainerMenuIDs = new Dictionary<ClassWOWType, int>();
            foreach (ClassWOWType classType in Enum.GetValues(typeof(ClassWOWType)))
            {
                if (classType == ClassWOWType.All || classType == ClassWOWType.None)
                    continue;

                // Base menu
                int gossipMenuID = IDGenerationTool.GenerateID("GossipMenuID", "classtrainer", classType.ToString());
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
            int nonClassTrainerGossipMenuIDNoShop = IDGenerationTool.GenerateID("GossipMenuID", "nonclasstrainernoshop");
            gossipMenuSQL.AddRow(nonClassTrainerGossipMenuIDNoShop, Configuration.CREATURE_GOSSIP_NPC_TEXT_ID);
            gossipMenuOptionSQL.AddRow(nonClassTrainerGossipMenuIDNoShop, 0, 3, "I would like to train.",
                Configuration.CREATURE_GOSSIP_TRAIN_BROADCAST_TEXT_ID, 5, 16, 0);
            int nonClassTrainerGossipMenuIDWithShop = IDGenerationTool.GenerateID("GossipMenuID", "nonclasstrainerwithshop");
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

                // Zone safe point (used by the mod for in-zone succor teleports)
                modEverquestZoneSafePointSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ZoneProperties.SafePosition);

                // Zone rules (used by the mod for zone-level behavior like bind restrictions)
                modEverquestZoneSQL.AddRow(Convert.ToInt32(zone.ZoneProperties.DBCMapID), zone.ZoneProperties.AllowBind);

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
                    int spiritHealerGUID = IDGenerationTool.GenerateID("CreatureGUID", "spirithealer", graveyard.WorldSafeLocsDBCID.ToString());
                    int zoneAreaID = Convert.ToInt32(curZoneProperties.DefaultZoneArea.DBCAreaTableID);
                    creatureSQL.AddRow(spiritHealerGUID, Configuration.ZONE_GRAVEYARD_SPIRIT_HEALER_CREATURETEMPLATE_ID, mapID, zoneAreaID, zoneAreaID,
                        graveyard.SpiritHealerX, graveyard.SpiritHealerY, graveyard.SpiritHealerZ, graveyard.SpiritHealerOrientation, CreatureMovementType.None, 300, string.Empty, false);
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
                        gameObjectTemplateAddonSQL.AddRowNoDespawn(gameObject.GameObjectTemplateEntryID, gameObject.LockDBCID != 0);
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
                            if (gameObject.LockDBCID != 0)
                                smartScriptsSQL.AddRowForGameObjectTriggeredTeleportOnActivate(gameObject.GameObjectTemplateEntryID, gameObject.DestinationMapID, gameObject.DestinationPosition.X,
                                    gameObject.DestinationPosition.Y, gameObject.DestinationPosition.Z, gameObject.DestinationOrientation, scriptComment);
                            else
                                smartScriptsSQL.AddRowForGameObjectTriggeredTeleport(gameObject.GameObjectTemplateEntryID, gameObject.DestinationMapID, gameObject.DestinationPosition.X,
                                    gameObject.DestinationPosition.Y, gameObject.DestinationPosition.Z, gameObject.DestinationOrientation, scriptComment);
                        }
                    }
                }
            }
        }

        private void OutputSQLScriptsToDisk()
        {
            // World
            achievementRewardSQL.SaveToDisk("achievement_reward", SQLFileType.World);
            areaTriggerSQL.SaveToDisk("areatrigger", SQLFileType.World);
            areaTriggerTeleportSQL.SaveToDisk("areatrigger_teleport", SQLFileType.World);
            broadcastTextSQL.SaveToDisk("broadcast_text", SQLFileType.World);
            conditionsSQL.SaveToDisk("conditions", SQLFileType.World);
            creatureSQL.SaveToDisk("creature", SQLFileType.World);
            creatureAddonSQL.SaveToDisk("creature_addon", SQLFileType.World);
            creatureDefaultTrainerSQL.SaveToDisk("creature_default_trainer", SQLFileType.World);
            creatureEquipTemplateSQL.SaveToDisk("creature_equip_template", SQLFileType.World);
            creatureImmunitiesSQL.SaveToDisk("creature_immunities", SQLFileType.World);
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
            gtChanceToSpellCritBaseDBCSQL.SaveToDisk("gtchancetospellcritbase_dbc", SQLFileType.World);
            gtChanceToSpellCritDBCSQL.SaveToDisk("gtchancetospellcrit_dbc", SQLFileType.World);
            instanceTemplateSQL.SaveToDisk("instance_template", SQLFileType.World);
            itemLootTemplateSQL.SaveToDisk("item_loot_template", SQLFileType.World);
            itemTemplateSQL.SaveToDisk("item_template", SQLFileType.World);
            modEverquestClassMapSQL.SaveToDisk("mod_everquest_classmap", SQLFileType.World);
            modEverquestCreatureSQL.SaveToDisk("mod_everquest_creature", SQLFileType.World);
            modEverquestCreatureInstanceSQL.SaveToDisk("mod_everquest_creature_instance", SQLFileType.World);
            modEverquestCreatureLootSQL.SaveToDisk("mod_everquest_creature_loot", SQLFileType.World);
            modEverquestCreatureOnkillReputationSQL.SaveToDisk("mod_everquest_creature_onkill_reputation", SQLFileType.World);
            modEverquestCreatureEmoteSQL.SaveToDisk("mod_everquest_creature_emote", SQLFileType.World);
            modEverquestCreatureKillSpawnSQL.SaveToDisk("mod_everquest_creature_kill_spawn", SQLFileType.World);
            modEverquestCreatureMovementSoundSQL.SaveToDisk("mod_everquest_creature_movement_sound", SQLFileType.World);
            modEverquestCreatureSpawnPointSQL.SaveToDisk("mod_everquest_creature_spawn_point", SQLFileType.World);
            modEverquestCreatureWaypointSQL.SaveToDisk("mod_everquest_creature_waypoint", SQLFileType.World);
            modEverquestFactionSQL.SaveToDisk("mod_everquest_faction", SQLFileType.World);
            modEverquestForageZoneItemsSQL.SaveToDisk("mod_everquest_forage_zone_items", SQLFileType.World);
            modEverquestIllusionDisplaySQL.SaveToDisk("mod_everquest_illusion_display", SQLFileType.World);
            modEverquestIllusionFaceSQL.SaveToDisk("mod_everquest_illusion_face", SQLFileType.World);
            modEverquestItemTemplateSQL.SaveToDisk("mod_everquest_item_template", SQLFileType.World);
            modEverquestPetSQL.SaveToDisk("mod_everquest_pet", SQLFileType.World);
            modEverquestPlayerCreateInfoSQL.SaveToDisk("mod_everquest_playercreateinfo", SQLFileType.World);
            modEverquestPlayerAutoLearnSkillsSQL.SaveToDisk("mod_everquest_playerautolearnskills", SQLFileType.World);
            modEverquestPlayerAutoLearnSpellsSQL.SaveToDisk("mod_everquest_playerautolearnspells", SQLFileType.World);
            modEverquestPlayerAutoAddItemsSQL.SaveToDisk("mod_everquest_playerautoadditems", SQLFileType.World);
            modEverquestSpellSQL.SaveToDisk("mod_everquest_spell", SQLFileType.World);
            modEverquestSystemConfigsSQL.SaveToDisk("mod_everquest_systemconfigs", SQLFileType.World);
            modEverquestQuestCompleteReputationSQL.SaveToDisk("mod_everquest_quest_complete_reputation", SQLFileType.World);
            modEverquestTransportTriggerSQL.SaveToDisk("mod_everquest_transport_trigger", SQLFileType.World);
            modEverquestZoneSafePointSQL.SaveToDisk("mod_everquest_zone_safe_point", SQLFileType.World);
            modEverquestZoneSQL.SaveToDisk("mod_everquest_zone", SQLFileType.World);
            modEverquestQuestReactionSQL.SaveToDisk("mod_everquest_quest_reaction", SQLFileType.World);
            modEverquestGossipReactionSQL.SaveToDisk("mod_everquest_gossip_reaction", SQLFileType.World);
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
                //string charactersSQLScriptFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "SQLScripts", SQLFileType.Characters.ToString());
                //if (Directory.Exists(charactersSQLScriptFolder) == false)
                //{
                //    Logger.WriteError("Could not deploy SQL scripts to server. Path '" + charactersSQLScriptFolder + "' did not exist");
                //    return;
                //}
                //using (MySqlConnection connection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_CHARACTERS))
                //{
                //    connection.Open();
                //    string[] sqlFiles = Directory.GetFiles(charactersSQLScriptFolder);
                //    foreach (string sqlFile in sqlFiles)
                //    {
                //        currentScriptFileName = sqlFile;
                //        MySqlCommand command = new MySqlCommand();
                //        command.Connection = connection;
                //        command.CommandTimeout = 288000;
                //        command.CommandText = FileTool.ReadAllDataFromFile(sqlFile);
                //        command.ExecuteNonQuery();
                //    }
                //    connection.Close();
                //}

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

            Logger.WriteInfo("Deploying sql to server complete");
        }
    }
}
