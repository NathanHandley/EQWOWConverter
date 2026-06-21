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
using EQWOWConverter.Player;
using EQWOWConverter.Spells;
using EQWOWConverter.Tradeskills;
using EQWOWConverter.Zones;

namespace EQWOWConverter.Creatures
{
    internal class CreatureTemplate
    {
        private static Dictionary<int, CreatureTemplate> CreatureTemplateListByEQID = new Dictionary<int, CreatureTemplate>();
        private static Dictionary<int, CreatureTemplate> CreatureTemplateListByWOWID = new Dictionary<int, CreatureTemplate>();
        private static SortedDictionary<int, Dictionary<string, float>> StatBaselinesByLevels = new SortedDictionary<int, Dictionary<string, float>>();
        private static Dictionary<(string, string), List<CreatureTemplate>> CreatureTemplatesBySpawnZonesAndName = new Dictionary<(string, string), List<CreatureTemplate>>();
        private static readonly object CreatureTemplateLock = new object();

        public bool IsWaypointDebugCreature = false;
        public int EQCreatureTemplateID = 0;
        public int WOWCreatureTemplateID = 0;
        public string Name = string.Empty; // Restrict to 100 characters
        private string NameNoFormat = string.Empty;
        public string SubName = string.Empty; // Restrict to 100 characters
        public int Level = 1;
        public CreatureRace Race = new CreatureRace();
        public int EQClass = 1;
        public int EQBodyType = 24; // This is common for the body type
        public int FaceID = 0;
        public int ColorTintID = 0;
        public float Size = 0f;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureID = 0;
        public int HelmTextureID = 0;
        public CreatureModelTemplate? ModelTemplate = null;
        public int MerchantID = 0;
        public int EQLootTableID = 0;
        public int WOWLootID = 0;
        public int MoneyMinInCopper = 0;
        public int MoneyMaxInCopper = 0;
        public bool HasMana = false;
        public float HPMod = 1f;
        public float DamageMod = 1f;
        public CreatureRankType Rank = CreatureRankType.Normal;
        public int EQFactionID = 0;
        public int EQNPCFactionID = 0;
        public int WOWFactionTemplateID = 0;
        public List<CreatureFactionKillReward> CreatureFactionKillRewards = new List<CreatureFactionKillReward>();
        public float DetectionRange = 0;
        public bool CanAssist = false;
        public bool IsBanker = false;
        public bool IsRidingTrainer = false;
        public bool IsNorrathPriestOfDiscord = false;
        public bool IsAzerothPriestOfDiscord = false;
        public ClassWOWType ClassTrainerType = ClassWOWType.None;
        public TradeskillType TradeskillTrainerType = TradeskillType.None;
        public int GossipMenuID = 0;
        public bool IsNonNPC = false;
        public string SpawnZones = string.Empty;
        public bool IsQuestGiver = false;
        public int SpawnLimit = 0;
        public bool HasSmartScript = false;
        public int DefaultEmoteID = 0;
        public int CreatureSpellListID = 0;
        public List<CreatureSpellEntry> CreatureSpellEntriesCombat = new List<CreatureSpellEntry>();
        public List<CreatureSpellEntry> CreatureSpellEntriesHeal = new List<CreatureSpellEntry>();
        public List<CreatureSpellEntry> CreatureSpellEntriesOutOfCombatBuff = new List<CreatureSpellEntry>();
        public List<CreatureSpellEntry> CreatureSpellEntriesOutOfCombatSummons = new List<CreatureSpellEntry>();
        public bool DoesSummonPets = false;
        public List<(int, int)> AttackEQSpellIDAndProcChance = new List<(int, int)>();
        public bool UsesBash = false;
        public bool UsesHarmTouch = false;
        public bool UsesLayOnHands = false;
        public long MechanicImmuneMask = 0;
        public int CreatureImmunitiesId = 0;
        public bool SeesInvisible = false;
        public bool SeesInvisibleUndead = false;
        public bool SeesStealth = false;
        public bool IsPet = false;
        public float ModelTemplateScale = 1.0f; // Used for form changes
        public bool IsStableMaster = false;
        public bool IsReagentVendor = false;

        private static readonly object CreatureIDsLock = new object();
        private static int CURRENT_SQL_CREATURE_GUID = -1;
        private static int CURRENT_SQL_CREATURE_TEMPLATE_GUID = -1;
        private static int CURRENT_CREATURE_EQID = 200000;

        public bool IsInteractive()
        {
            if (IsQuestGiver == true)
                return true;
            if (IsBanker == true)
                return true;
            if (MerchantID != 0)
                return true;
            if (IsStableMaster == true)
                return true;
            if (IsReagentVendor == true)
                return true;
            if (ClassTrainerType != ClassWOWType.None)
                return true;
            if (TradeskillTrainerType != TradeskillType.None && TradeskillTrainerType != TradeskillType.Unknown)
                return true;
            if (IsRidingTrainer == true)
                return true;
            return false;
        }

        public bool IsTameable()
        {
            if (Race.WOWCreatureType == 1) // beast
                return true;
            else
                return false;
        }

        public bool IsExoticTameable()
        {
            return Race.IsExoticTame;
        }

        public bool IsUndeadBodyTypeForInvisVsUndead()
        {
            return EQBodyType == 3 || EQBodyType == 8 || EQBodyType == 12;
        }

        public bool CanSeeThroughInvisVsUndead()
        {
            return IsUndeadBodyTypeForInvisVsUndead() == false || SeesInvisibleUndead == true;
        }

        public static Dictionary<int, CreatureTemplate> GetCreatureTemplateListByEQID()
        {
            if (CreatureTemplateListByEQID.Count == 0)
                PopulateCreatureTemplateList();
            return CreatureTemplateListByEQID;
        }

        public static Dictionary<int, CreatureTemplate> GetCreatureTemplateListByWOWID()
        {
            if (CreatureTemplateListByWOWID.Count == 0)
                PopulateCreatureTemplateList();
            return CreatureTemplateListByWOWID;
        }

        public static List<CreatureTemplate> GetCreatureTemplatesForSpawnZonesAndName(string spawnZones, string namePreFormat)
        {
            if (CreatureTemplatesBySpawnZonesAndName.Count == 0)
                PopulateCreatureTemplateList();

            // Fall back to no spawn zones if spawn zones lookup fails
            if (CreatureTemplatesBySpawnZonesAndName.ContainsKey((spawnZones, namePreFormat)) == true)
                return CreatureTemplatesBySpawnZonesAndName[(spawnZones, namePreFormat)];
            else if (CreatureTemplatesBySpawnZonesAndName.ContainsKey(("", namePreFormat)) == true)
                return CreatureTemplatesBySpawnZonesAndName[("", namePreFormat)];
            else
                return new List<CreatureTemplate>();
        }

        public static int GenerateCreatureSQLGUID()
        {
            lock (CreatureIDsLock)
            {
                if (CURRENT_SQL_CREATURE_GUID == -1)
                {
                    if (Configuration.CONFIGONLY_CREATURE_SPAWN_AND_WAYPOINT_DEBUG_MODE == true)
                        CURRENT_SQL_CREATURE_GUID = Configuration.CONFIGONLY_SQL_CREATURE_GUID_DEBUG_LOW;
                    else
                        CURRENT_SQL_CREATURE_GUID = Configuration.SQL_CREATURE_GUID_LOW;
                }
                int returnGUID = CURRENT_SQL_CREATURE_GUID;
                CURRENT_SQL_CREATURE_GUID++;
                return returnGUID;
            }
        }

        public static int GenerateCreatureTemplateID()
        {
            lock (CreatureIDsLock)
            {
                if (CURRENT_SQL_CREATURE_TEMPLATE_GUID == -1)
                    CURRENT_SQL_CREATURE_TEMPLATE_GUID = Configuration.SQL_CREATURETEMPLATE_GENERATED_START_ID;
                int returnID = CURRENT_SQL_CREATURE_TEMPLATE_GUID;
                CURRENT_SQL_CREATURE_TEMPLATE_GUID++;
                return returnID;
            }
        }

        public static CreatureTemplate GenerateCreatureTemplate(string name, CreatureRace race, CreatureGenderType genderType, int helmTextureID, int textureIndex, int faceIndex, int colorTintID,
            float modelTemplateScale, int wowFactionTemplateID)
        {
            lock (CreatureTemplateLock)
            {
                CreatureTemplate newCreatureTemplate = new CreatureTemplate();

                // Generate new IDs
                newCreatureTemplate.EQCreatureTemplateID = CURRENT_CREATURE_EQID;
                CURRENT_CREATURE_EQID++;
                newCreatureTemplate.WOWCreatureTemplateID = GenerateCreatureTemplateID();

                // Assign properties
                newCreatureTemplate.Name = name;
                newCreatureTemplate.Race = race;
                newCreatureTemplate.GenderType = genderType;
                newCreatureTemplate.HelmTextureID = helmTextureID;
                newCreatureTemplate.TextureID = textureIndex;
                newCreatureTemplate.FaceID = faceIndex;
                newCreatureTemplate.ColorTintID = colorTintID;
                newCreatureTemplate.Size = race.Height;
                newCreatureTemplate.ModelTemplateScale = modelTemplateScale;
                newCreatureTemplate.WOWFactionTemplateID = wowFactionTemplateID;

                // Store the creature template
                if (CreatureTemplateListByEQID.ContainsKey(newCreatureTemplate.EQCreatureTemplateID))
                {
                    Logger.WriteError("Creature Template with '" + newCreatureTemplate.EQCreatureTemplateID + "' was duplicate");
                    return newCreatureTemplate;
                }
                CreatureTemplateListByEQID.Add(newCreatureTemplate.EQCreatureTemplateID, newCreatureTemplate);
                CreatureTemplateListByWOWID.Add(newCreatureTemplate.WOWCreatureTemplateID, newCreatureTemplate);
                if (CreatureTemplatesBySpawnZonesAndName.ContainsKey((newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat)) == false)
                    CreatureTemplatesBySpawnZonesAndName.Add((newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat), new List<CreatureTemplate>());
                CreatureTemplatesBySpawnZonesAndName[(newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat)].Add(newCreatureTemplate);
                return newCreatureTemplate;
            }
        }

        private static void PopulateCreatureTemplateList()
        {
            lock (CreatureTemplateLock)
            {
                // Grab the baselines
                PopulateStatBaselinesByLevel();

                // Get the zone properties list for lookups
                Dictionary<string, ZoneProperties> zonePropertiesByShortName = ZoneProperties.GetZonePropertyListByShortName();

                // Get spell pets also for lookups
                List<string> allSpellPetNameTypes = SpellPet.GetAllSpellTypeNames();

                // Load all of the creature data
                string creatureTemplatesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplates.csv");
                Logger.WriteDebug("Populating Creature Template list via file '" + creatureTemplatesFile + "'");
                List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(creatureTemplatesFile, "|");
                foreach (Dictionary<string, string> columns in rows)
                {
                    // Skip invalid creatures
                    string namePreFormat = columns["name"];
                    string spawnZones = columns["spawnzones"].Trim();
                    if (allSpellPetNameTypes.Contains(namePreFormat) == false)
                    {
                        if (spawnZones.Length == 0)
                            continue;
                        bool zoneShortNameFound = false;
                        string[] spawnZoneShortNames = spawnZones.Split(',');
                        foreach (string spawnZoneShortName in spawnZoneShortNames)
                        {
                            if (zonePropertiesByShortName.ContainsKey(spawnZoneShortName))
                            {
                                zoneShortNameFound = true;
                                break;
                            }
                        }
                        if (zoneShortNameFound == false)
                            continue;
                    }
                    if (int.Parse(columns["enabled"]) == 0)
                        continue;
                    if (Configuration.GENERATE_ENABLE_GUILD_VAULTS == true && columns["hide_for_guild_bank"].Trim() == "1")
                        continue;

                    // Load the row
                    CreatureTemplate newCreatureTemplate = new CreatureTemplate();
                    newCreatureTemplate.EQCreatureTemplateID = int.Parse(columns["eq_id"]);
                    newCreatureTemplate.WOWCreatureTemplateID = int.Parse(columns["wow_id"]);
                    newCreatureTemplate.SpawnZones = spawnZones;
                    newCreatureTemplate.IsNonNPC = int.Parse(columns["non_npc"]) > 0;
                    if (newCreatureTemplate.WOWCreatureTemplateID < Configuration.SQL_CREATURETEMPLATE_ENTRY_LOW || newCreatureTemplate.WOWCreatureTemplateID > Configuration.SQL_CREATURETEMPLATE_ENTRY_HIGH)
                        Logger.WriteError("Creature template with EQ id of '' had a wow id of '', but that's outside the bounds of CREATURETEMPLATE_ENTRY_LOW and CREATURETEMPLATE_ENTRY_HIGH.  SQL deletes will not catch everything");
                    newCreatureTemplate.Rank = (CreatureRankType)int.Parse(columns["rank"]);
                    newCreatureTemplate.Name = columns["name"].Replace('_', ' ');
                    newCreatureTemplate.Name = newCreatureTemplate.Name.Replace("#", "");
                    newCreatureTemplate.SpawnLimit = int.Parse(columns["spawn_limit"]);
                    newCreatureTemplate.NameNoFormat = namePreFormat;
                    newCreatureTemplate.SubName = columns["lastname"].Replace('_', ' ');
                    if (Configuration.GENERATE_REBALANCE_CONTENT_TO_LEVEL_80 == true)
                        newCreatureTemplate.Level = int.Max(int.Parse(columns["level80"]), 1);
                    else
                        newCreatureTemplate.Level = int.Max(int.Parse(columns["level60"]), 1);
                    newCreatureTemplate.DefaultEmoteID = int.Max(int.Parse(columns["idle_emote_id"]), 0);
                    newCreatureTemplate.EQBodyType = int.Parse(columns["bodytype"]);
                    newCreatureTemplate.Size = float.Parse(columns["size"]);
                    if (newCreatureTemplate.Size <= 0)
                    {
                        Logger.WriteDebug("CreatureTemplate with size of zero or less detected with name '" + newCreatureTemplate.Name + "', so setting to 1");
                        newCreatureTemplate.Size = 1;
                    }
                    int genderID = int.Parse(columns["gender"]);
                    switch (genderID)
                    {
                        case 0: newCreatureTemplate.GenderType = CreatureGenderType.Male; break;
                        case 1: newCreatureTemplate.GenderType = CreatureGenderType.Female; break;
                        default: newCreatureTemplate.GenderType = CreatureGenderType.Neutral; break;
                    }
                    newCreatureTemplate.TextureID = int.Parse(columns["texture"]);
                    newCreatureTemplate.HelmTextureID = int.Parse(columns["helmtexture"]);
                    newCreatureTemplate.FaceID = int.Parse(columns["face"]);
                    if (newCreatureTemplate.FaceID > 9)
                    {
                        Logger.WriteDebug("CreatureTemplate with face ID greater than 9 detected, so setting to 0");
                        newCreatureTemplate.FaceID = 0;
                    }
                    newCreatureTemplate.EQLootTableID = int.Parse(columns["loottable_id"]);
                    int skillTrainerType = int.Parse(columns["skill_trainer"]);
                    ProcessProfessionTrainerType(ref newCreatureTemplate, skillTrainerType);
                    newCreatureTemplate.MerchantID = int.Parse(columns["merchant_id"]);
                    // Clear vendor lists for riding trainers if riding trainers are disabled
                    if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == false && skillTrainerType == 10)
                        newCreatureTemplate.MerchantID = 0;
                    newCreatureTemplate.ColorTintID = int.Parse(columns["armortint_id"]);
                    newCreatureTemplate.HasMana = (int.Parse(columns["mana"]) > 0);
                    newCreatureTemplate.HPMod = GetStatMod("hp", newCreatureTemplate.Level, newCreatureTemplate.Rank, float.Parse(columns["hp"]));
                    newCreatureTemplate.DamageMod = GetStatMod("avgdmg", newCreatureTemplate.Level, newCreatureTemplate.Rank, float.Parse(columns["avgdmg"]));
                    float detectionRange = float.Parse(columns["aggroradius"]);
                    newCreatureTemplate.DetectionRange = detectionRange * Configuration.GENERATE_WORLD_SCALE;
                    newCreatureTemplate.EQClass = int.Parse(columns["class"]);
                    ProcessEQClass(ref newCreatureTemplate, newCreatureTemplate.EQClass);
                    if (newCreatureTemplate.IsRidingTrainer == true && Configuration.CREATURE_RIDING_TRAINERS_ENABLED == false)
                        continue;
                    newCreatureTemplate.CreatureSpellListID = int.Parse(columns["creaturespelllistid"]);

                    // Determine if the creature should do any special abilities
                    string specialAbilitiesRaw = columns.ContainsKey("special_abilities") ? columns["special_abilities"] : string.Empty;
                    newCreatureTemplate.UsesBash = DetermineCreatureUsesBash(newCreatureTemplate.EQClass, newCreatureTemplate.Level, specialAbilitiesRaw);
                    newCreatureTemplate.UsesHarmTouch = DetermineCreatureUsesHarmTouch(newCreatureTemplate.EQClass, newCreatureTemplate.Level);
                    newCreatureTemplate.UsesLayOnHands = DetermineCreatureUsesLayOnHands(newCreatureTemplate.EQClass, newCreatureTemplate.Level);
                    if (newCreatureTemplate.UsesBash == true || newCreatureTemplate.UsesHarmTouch == true || newCreatureTemplate.UsesLayOnHands == true)
                        newCreatureTemplate.HasSmartScript = true;

                    // Crowd-control immunities from EQ special abilities
                    newCreatureTemplate.MechanicImmuneMask = DetermineCreatureMechanicImmuneMask(specialAbilitiesRaw);

                    // See invisibility
                    if (columns.ContainsKey("see_invis") && int.TryParse(columns["see_invis"], out int seeInvisValue) && seeInvisValue > 0)
                    {
                        newCreatureTemplate.SeesInvisible = true;
                        newCreatureTemplate.HasSmartScript = true;
                    }

                    // See invisibility versus undead
                    if (columns.ContainsKey("see_invis_undead") && int.TryParse(columns["see_invis_undead"], out int seeInvisUndeadValue) && seeInvisUndeadValue > 0)
                        newCreatureTemplate.SeesInvisibleUndead = true;

                    // See stealth
                    if ((columns.ContainsKey("see_sneak") && int.TryParse(columns["see_sneak"], out int seeSneakValue) && seeSneakValue > 0) ||
                        (columns.ContainsKey("see_improved_hide") && int.TryParse(columns["see_improved_hide"], out int seeImprovedHideValue) && seeImprovedHideValue > 0))
                    {
                        newCreatureTemplate.SeesStealth = true;
                        newCreatureTemplate.HasSmartScript = true;
                    }

                    // Special logic for a few variations of kobolds, which look wrong if not adjusted
                    int raceID = int.Parse(columns["race"]);
                    if (raceID == 0)
                    {
                        Logger.WriteDebug("Creature template had race of 0, so falling back to 1 (Human)");
                        raceID = 1;
                    }
                    if (raceID == 48)
                    {
                        if (newCreatureTemplate.TextureID == 2 || (newCreatureTemplate.TextureID == 1 && newCreatureTemplate.HelmTextureID == 0))
                        {
                            newCreatureTemplate.TextureID = 0;
                            newCreatureTemplate.HelmTextureID = 0;
                            newCreatureTemplate.FaceID = 0;
                        }
                    }

                    // Grab the race
                    List<CreatureRace> allRaces = CreatureRace.GetAllCreatureRaces();
                    CreatureRace? race = null;
                    foreach (CreatureRace curRace in allRaces)
                    {
                        if (curRace.ID == raceID && curRace.Gender == newCreatureTemplate.GenderType && curRace.VariantID == 0)
                        {
                            race = curRace;
                            break;
                        }
                    }

                    if (race == null)
                    {
                        Logger.WriteError("No valid race found that matches raceID '" + raceID + "' and gender '" + newCreatureTemplate.GenderType + "'");
                        continue;
                    }
                    else
                    {
                        // Make sure there's a skeleton
                        if (race.SkeletonName.Trim().Length == 0)
                        {
                            Logger.WriteDebug("Creature Template with name '" + newCreatureTemplate.Name + "' with race ID of '" + raceID + "' has no skeletons, so skipping");
                            continue;
                        }
                        newCreatureTemplate.Race = race;
                    }

                    // Add ID if debugging for it is true
                    //if (Configuration.CREATURE_ADD_DEBUG_VALUES_TO_NAME == true)
                    //    newCreatureTemplate.Name = newCreatureTemplate.Name + " " + newCreatureTemplate.EQCreatureTemplateID.ToString();
                    //newCreatureTemplate.Name = newCreatureTemplate.Name + " R" + newCreatureTemplate.Race.EQCreatureTemplateID + "-G" + Convert.ToInt32(newCreatureTemplate.GenderType).ToString() + "-V" + newCreatureTemplate.Race.VariantID;

                    // Reputation / Faction
                    newCreatureTemplate.EQFactionID = int.Parse(columns["faction_id"]);
                    newCreatureTemplate.EQNPCFactionID = int.Parse(columns["npc_faction_id"]);
                    newCreatureTemplate.WOWFactionTemplateID = CreatureFaction.GetWOWFactionTemplateIDForEQFactionID(newCreatureTemplate.EQFactionID);
                    newCreatureTemplate.CanAssist = CreatureFaction.CanFactionAssistPlayer(newCreatureTemplate.EQFactionID);
                    foreach (CreatureFactionKillReward factionKillReward in CreatureFaction.GetCreatureFactionKillRewards(newCreatureTemplate.EQNPCFactionID))
                        newCreatureTemplate.CreatureFactionKillRewards.Add(factionKillReward);

                    // Must be a unique record
                    if (CreatureTemplateListByEQID.ContainsKey(newCreatureTemplate.EQCreatureTemplateID))
                    {
                        Logger.WriteError("Creature Template list via file '" + creatureTemplatesFile + "' has an duplicate row with id '" + newCreatureTemplate.EQCreatureTemplateID + "'");
                        continue;
                    }
                    CreatureTemplateListByEQID.Add(newCreatureTemplate.EQCreatureTemplateID, newCreatureTemplate);
                    CreatureTemplateListByWOWID.Add(newCreatureTemplate.WOWCreatureTemplateID, newCreatureTemplate);

                    if (CreatureTemplatesBySpawnZonesAndName.ContainsKey((newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat)) == false)
                        CreatureTemplatesBySpawnZonesAndName.Add((newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat), new List<CreatureTemplate>());
                    CreatureTemplatesBySpawnZonesAndName[(newCreatureTemplate.SpawnZones, newCreatureTemplate.NameNoFormat)].Add(newCreatureTemplate);
                }
            }
        }

        private static bool DetermineCreatureUsesBash(int eqClass, int level, string specialAbilitiesRaw)
        {
            if (Configuration.COMBATSKILL_BASH_ENABLED == false)
                return false;
            if (level < Configuration.COMBATSKILL_BASH_CREATURE_MIN_LEVEL)
                return false;
            // EQ-like classes are warrior, cleric, paladin, shadowknight (and GM versions)
            switch (eqClass)
            {
                case 1:  // Warrior
                case 2:  // Cleric
                case 3:  // Paladin
                case 5:  // ShadowKnight
                case 20: // Warrior (GM)
                case 21: // Cleric (GM)
                case 22: // Paladin (GM)
                case 24: // ShadowKnight (GM)
                    return true;
            }

            // The "Use Warrior Skills" special ability (EQ id 44) grants warrior skills (including bash) to any class
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 44) == true)
                return true;

            return false;
        }

        private static bool DetermineCreatureUsesHarmTouch(int eqClass, int level)
        {
            if (Configuration.COMBATSKILL_HARMTOUCH_ENABLED == false)
                return false;
            if (level < Configuration.COMBATSKILL_HARMTOUCH_CREATURE_MIN_LEVEL)
                return false;
            // Harm Touch is a shadowknight ability (EQ class 5, GM 24)
            switch (eqClass)
            {
                case 5:  // ShadowKnight
                case 24: // ShadowKnight (GM)
                    return true;
            }
            return false;
        }

        private static bool DetermineCreatureUsesLayOnHands(int eqClass, int level)
        {
            if (Configuration.COMBATSKILL_LAYONHANDS_ENABLED == false)
                return false;
            if (level < Configuration.COMBATSKILL_LAYONHANDS_CREATURE_MIN_LEVEL)
                return false;
            // Lay on Hands is a paladin ability (EQ class 3, GM 22)
            switch (eqClass)
            {
                case 3:  // Paladin
                case 22: // Paladin (GM)
                    return true;
            }
            return false;
        }

        private static long DetermineCreatureMechanicImmuneMask(string specialAbilitiesRaw)
        {
            // creature_immunities.MechanicsMask uses (1 << SpellMechanicType) per immunity
            long mask = 0;
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 12) == true) // SlowImmunity
                mask |= 1L << (int)SpellMechanicType.Slowed;
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 13) == true) // MesmerizeImmunity
                mask |= 1L << (int)SpellMechanicType.Incapacitated;
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 14) == true) // CharmImmunity
                mask |= 1L << (int)SpellMechanicType.Charmed;
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 15) == true) // StunImmunity
                mask |= 1L << (int)SpellMechanicType.Stunned;
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 16) == true) // SnareImmunity (EQ blocks both snare and root)
                mask |= (1L << (int)SpellMechanicType.Snared) | (1L << (int)SpellMechanicType.Rooted);
            if (HasSpecialAbilityEnabled(specialAbilitiesRaw, 17) == true) // FearImmunity
                mask |= 1L << (int)SpellMechanicType.Fleeing;
            return mask;
        }

        private static bool HasSpecialAbilityEnabled(string specialAbilitiesRaw, int abilityID)
        {
            if (string.IsNullOrWhiteSpace(specialAbilitiesRaw))
                return false;

            // This field is separated by a carrot (^), note that I changed it because the source database had a pipe (|) but that messed up the .csv format
            string[] abilityGroups = specialAbilitiesRaw.Split('^');
            foreach (string abilityGroup in abilityGroups)
            {
                if (abilityGroup.Trim().Length == 0)
                    continue;
                string[] parts = abilityGroup.Split(',');
                if (parts.Length < 2)
                    continue;
                if (int.TryParse(parts[0].Trim(), out int curID) == false || curID != abilityID)
                    continue;
                if (int.TryParse(parts[1].Trim(), out int value) == true && value > 0)
                    return true;
            }
            return false;
        }

        private static void ProcessProfessionTrainerType(ref CreatureTemplate creatureTemplate, int tradeskillTrainerType)
        {
            switch (tradeskillTrainerType)
            {
                case 1: // Alchemy
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Alchemy;
                        creatureTemplate.SubName = "Alchemy Trainer";
                    } break;
                case 2: // Blacksmithing
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Blacksmithing;
                        creatureTemplate.SubName = "Blacksmithing Trainer";
                    } break;
                case 3: // Cooking
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Cooking;
                        creatureTemplate.SubName = "Cooking Trainer";
                    } break;
                case 4: // Engineering
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Engineering;
                        creatureTemplate.SubName = "Engineering Trainer";
                    } break;
                case 5: // Jewelcrafting
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Jewelcrafting;
                        creatureTemplate.SubName = "Jewelcrafting Trainer";
                    } break;
                case 6: // Inscription
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Inscription;
                        creatureTemplate.SubName = "Inscription Trainer";
                    } break;
                case 7: // Tailoring
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Tailoring;
                        creatureTemplate.SubName = "Tailoring Trainer";
                    } break;
                case 8: // Enchanting
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Enchanting;
                        creatureTemplate.SubName = "Enchanting Trainer";
                    } break;
                case 9: // Fishing
                    {
                        creatureTemplate.TradeskillTrainerType = TradeskillType.Fishing;
                        creatureTemplate.SubName = "Fishing Trainer";
                    }
                    break;
                case 10: // Riding
                    {
                        if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true)
                        {
                            creatureTemplate.IsRidingTrainer = true;
                            creatureTemplate.SubName = "Riding Trainer";
                        }
                    } break;
                default:  break;// Nothing
            }
        }

        private static void ProcessEQClass(ref CreatureTemplate creatureTemplate, int eqClass)
        {
            switch(eqClass)
            {
                case 20: // Warrior GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Warrior);
                        creatureTemplate.SubName = "Warrior Trainer";
                    } break;
                case 21: // Cleric GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Cleric);
                        creatureTemplate.SubName = "Cleric Trainer";
                    } break;
                case 22: // Paladin GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Paladin);
                        creatureTemplate.SubName = "Paladin Trainer";
                    } break;
                case 23: // RangerGM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Ranger);
                        creatureTemplate.SubName = "Ranger Trainer";
                    } break;
                case 24: // ShadowKnight GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.ShadowKnight);
                        creatureTemplate.SubName = "Shadow Knight Trainer";
                    } break;
                case 25: // Druid GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Druid);
                        creatureTemplate.SubName = "Druid Trainer";
                    } break;
                case 26: // Monk GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Monk);
                        creatureTemplate.SubName = "Monk Trainer";
                    } break;
                case 27: // Bard GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Bard);
                        creatureTemplate.SubName = "Bard Trainer";
                    } break;
                case 28: // Rogue GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Rogue);
                        creatureTemplate.SubName = "Rogue Trainer";
                    } break;
                case 29: // Shaman GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Shaman);
                        creatureTemplate.SubName = "Shaman Trainer";
                    } break;
                case 30: // Necromancer GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Necromancer);
                        creatureTemplate.SubName = "Necromancer Trainer";
                    } break;
                case 31: // Wizard GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Wizard);
                        creatureTemplate.SubName = "Wizard Trainer";
                    } break;
                case 32: // Magician GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Magician);
                        creatureTemplate.SubName = "Magician Trainer";
                    } break;
                case 33: // Enchanter GM
                    {
                        creatureTemplate.ClassTrainerType = PlayerClassMapping.GetWOWClassForBaseEQClass(ClassEQType.Enchanter);
                        creatureTemplate.SubName = "Enchanter Trainer";
                    } break;
                //case 34: // Beastlord GM
                case 40: // Banker
                    {
                        creatureTemplate.IsBanker = true;
                    } break;
                //case 41: // Lots of merchants have these, such what many Stablemasters had
                // All 100+ are not actually from EQ and were created new for these purposes
                case 100: // Priest of Discord (in Norrath)
                    {
                        creatureTemplate.IsNorrathPriestOfDiscord = true;
                        creatureTemplate.SubName = "Azeroth Gatemaster";
                    } break;
                case 101: // Priest of Discord (in Azeroth)
                    {
                        creatureTemplate.IsAzerothPriestOfDiscord = true;
                        creatureTemplate.SubName = "Norrath Gatemaster";
                    } break;
                case 102: // Stablemaster
                    {
                        creatureTemplate.IsStableMaster = true;
                        creatureTemplate.SubName = "Stable Master";
                    } break;
                case 103: // Reagent Vendor
                    {
                        creatureTemplate.IsReagentVendor = true;
                        creatureTemplate.SubName = "Reagent Merchant";
                    } break;
                default:
                    {   
                        // Do nothing
                    } break;
            }                
        }

        private static void PopulateStatBaselinesByLevel()
        {
            string creatureStatBaselineFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureStatBaselines.csv");
            Logger.WriteDebug("Populating Creature Stat Baselines list via file '" + creatureStatBaselineFile + "'");
            List<string> creatureStatBaselineRows = FileTool.ReadAllStringLinesFromFile(creatureStatBaselineFile, true, true);
            foreach (string row in creatureStatBaselineRows)
            {
                // Load the row
                string[] rowBlocks = row.Split("|");
                int level = int.Parse(rowBlocks[0]);
                int hp = int.Parse(rowBlocks[1]);
                float avgDMG = float.Parse(rowBlocks[2]);

                // Create the baseline record
                StatBaselinesByLevels.Add(level, new Dictionary<string, float>());
                StatBaselinesByLevels[level].Add("hp", hp);
                StatBaselinesByLevels[level].Add("avgdmg", avgDMG);
            }
        }

        private static float GetStatMod(string statName, int creatureLevel, CreatureRankType creatureRank, float creatureStatValue)
        {
            if (creatureLevel == 0)
                return 1;
            if (creatureStatValue < 1)
                return 1;

            // Determine the boundaries and adds
            float modAdd;
            float modMin;
            float modMax;
            float modSet;
            switch(statName)
            {
                case "hp":
                    {
                        modAdd = Configuration.CREATURE_STAT_MOD_HP_ADD;
                        modMin = Configuration.CREATURE_STAT_MOD_HP_MIN;
                        modMax = GetValueForRank(creatureRank, Configuration.CREATURE_STAT_MOD_HP_MAX_NORMAL,
                            -1, -1, -1, Configuration.CREATURE_STAT_MOD_HP_MAX_RARE);
                        modSet = GetValueForRank(creatureRank, -1, Configuration.CREATURE_STAT_MOD_HP_SET_ELITE,
                            Configuration.CREATURE_STAT_MOD_HP_SET_ELITERARE, Configuration.CREATURE_STAT_MOD_HP_SET_BOSS, -1);
                    } break;
                case "avgdmg":
                    {
                        modAdd = Configuration.CREATURE_STAT_MOD_AVGDMG_ADD;
                        modMin = Configuration.CREATURE_STAT_MOD_AVGDMG_MIN;
                        modMax = GetValueForRank(creatureRank, Configuration.CREATURE_STAT_MOD_AVGDMG_MAX_NORMAL,
                            -1, -1, -1, Configuration.CREATURE_STAT_MOD_AVGDMG_MAX_RARE);
                        modSet = GetValueForRank(creatureRank, -1, Configuration.CREATURE_STAT_MOD_AVGDMG_SET_ELITE,
                            Configuration.CREATURE_STAT_MOD_AVGDMG_SET_ELITERARE, Configuration.CREATURE_STAT_MOD_AVGDMG_SET_BOSS, -1);
                    } break;
                default:
                    {
                        Logger.WriteError("GetStatMod failed due to unhandled stat name of '" + statName + "'");
                        return 1;
                    }
            }

            // Calculate the specific baseline to use based on range
            int levelLow = -1;
            int levelHigh = -1;
            float statLow = -1;
            float statHigh = -1;
            foreach (var statBaselineForLevel in StatBaselinesByLevels)
            {
                // Grab the stat for this level row
                if (statBaselineForLevel.Value.ContainsKey(statName) == false)
                {
                    Logger.WriteError("Error in GetStatMod as stat name '" + statName + "' did not exist");
                    return 1;
                }
                int recordLevel = statBaselineForLevel.Key;
                float recordStat = statBaselineForLevel.Value[statName];

                // Store based on current bounds
                if (levelLow == -1)
                {
                    levelLow = recordLevel;
                    levelHigh = recordLevel;
                    statLow = recordStat;
                    statHigh = recordStat;
                    if (levelLow == creatureLevel)
                        break;
                    else
                        continue;
                }
                if (recordLevel <= creatureLevel)
                {
                    levelLow = recordLevel;
                    statLow = recordStat;
                }
                if (creatureLevel <= recordLevel)
                {
                    levelHigh = recordLevel;
                    statHigh = recordStat;
                    break;
                }
            }
            if (levelLow == -1 || levelHigh == -1 || statLow == -1 || statHigh == -1)
            {
                Logger.WriteError("GetStatMod failed as one of the range caps was -1");
                return 1;
            }

            // Generate a stat mod
            float statRelative;
            if (creatureLevel == levelLow)
                statRelative = statLow;
            else if (creatureLevel == levelHigh)
                statRelative = statHigh;
            else
            {
                float normalLevelRelative = (Convert.ToSingle(creatureLevel - levelLow) / Convert.ToSingle(levelHigh - levelLow));
                statRelative = normalLevelRelative * statHigh + ((1 - normalLevelRelative) * statLow);
            }
            float genStatValue = creatureStatValue / statRelative;

            // Apply adds and limits
            genStatValue += modAdd;
            if (modMin != -1)
                genStatValue = MathF.Max(genStatValue, modMin);
            if (modMax != -1)
                genStatValue = MathF.Min(genStatValue, modMax);
            if (modSet != -1)
                genStatValue = modSet;
            return genStatValue;
        }

        private static float GetValueForRank(CreatureRankType creatureRank, float normalValue, float eliteValue, float eliteRareValue,
            float bossValue, float rareValue)
        {
            switch (creatureRank)
            {
                case CreatureRankType.Normal: return normalValue;
                case CreatureRankType.Elite: return eliteValue;
                case CreatureRankType.Boss: return bossValue;
                case CreatureRankType.Rare: return rareValue;
                case CreatureRankType.EliteRare: return eliteRareValue;
                default:
                    {
                        Logger.WriteError("GeTValueForRank failed since rank '" + creatureRank + "' was not defined");
                        return 1;
                    }
            }
        }

        // Debug elements
        public float SpawnWaypointDebugXPosition;
        public float SpawnWaypointDebugYPosition;
        public float SpawnWaypointDebugZPosition;
        public int SpawnWaypointDebugMapID;
        public int SpawnWaypointDebugAreaID;
        private static int SPAWN_WAYPOINT_DEBUG_SQL_CREATURE_TEMPLATE_GUID = Configuration.CONFIGONLY_SQL_CREATURETEMPLATE_DEBUG_ENTRY_LOW;

        public static void SpawnWaypointDebugCreateCreatureTemplate(int spawnInstanceID, int gridID, int gridNumber, float xPosition, float yPosition, float zPosition,
            string zoneShortName, int mapID, int areaID)
        {
            CreatureTemplate newCreatureTemplate = new CreatureTemplate();
            newCreatureTemplate.WOWCreatureTemplateID = SPAWN_WAYPOINT_DEBUG_SQL_CREATURE_TEMPLATE_GUID;
            newCreatureTemplate.TextureID = 1;
            newCreatureTemplate.HelmTextureID = 0;
            newCreatureTemplate.Size = 6;
            newCreatureTemplate.Level = 1;
            newCreatureTemplate.Name = spawnInstanceID.ToString() + "," + gridID + "," + gridNumber;
            newCreatureTemplate.SubName = zoneShortName + "," + zPosition.ToString();
            newCreatureTemplate.SpawnWaypointDebugXPosition = xPosition;
            newCreatureTemplate.SpawnWaypointDebugYPosition = yPosition;
            newCreatureTemplate.SpawnWaypointDebugZPosition = zPosition;
            newCreatureTemplate.SpawnZones = zoneShortName;
            newCreatureTemplate.SpawnWaypointDebugMapID = mapID;
            newCreatureTemplate.SpawnWaypointDebugAreaID = areaID;
            newCreatureTemplate.IsWaypointDebugCreature = true;
            CreatureTemplateListByWOWID.Add(SPAWN_WAYPOINT_DEBUG_SQL_CREATURE_TEMPLATE_GUID, newCreatureTemplate);

            SPAWN_WAYPOINT_DEBUG_SQL_CREATURE_TEMPLATE_GUID++;
        }
    }
}
