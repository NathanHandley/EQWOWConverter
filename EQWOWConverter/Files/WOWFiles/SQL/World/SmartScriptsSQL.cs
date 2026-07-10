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

namespace EQWOWConverter.WOWFiles
{
    internal class SmartScriptsSQL : SQLFile
    {
        // This ensures no ID collision issues, without giving that responsiblity to the caller
        private static Dictionary<int, Dictionary<int, int>> IDByEntryOrGUIDBySourceType = new Dictionary<int, Dictionary<int, int>>();
        private static readonly object IDLock = new object();
        private Random RandomGenerator = new Random();

        public override string DeleteRowSQL()
        {
            return "DELETE FROM smart_scripts WHERE COMMENT LIKE 'EQ%';";
        }

        private int GetUniqueID(int entryOrGUID, int sourceType)
        {
            lock (IDLock)
            {
                if (IDByEntryOrGUIDBySourceType.ContainsKey(sourceType) == false)
                    IDByEntryOrGUIDBySourceType.Add(sourceType, new Dictionary<int, int>());
                if (IDByEntryOrGUIDBySourceType[sourceType].ContainsKey(entryOrGUID) == false)
                {
                    IDByEntryOrGUIDBySourceType[sourceType].Add(entryOrGUID, 0);
                    return 0;
                }
                else
                {
                    IDByEntryOrGUIDBySourceType[sourceType][entryOrGUID]++;
                    return IDByEntryOrGUIDBySourceType[sourceType][entryOrGUID];
                }
            }
        }

        public int GetLastUniqueID(int entryOrGUID, int sourceType)
        {
            lock (IDLock)
            {
                if (IDByEntryOrGUIDBySourceType.ContainsKey(sourceType) == false)
                    IDByEntryOrGUIDBySourceType.Add(sourceType, new Dictionary<int, int>());
                if (IDByEntryOrGUIDBySourceType[sourceType].ContainsKey(entryOrGUID) == false)
                    return -1;
                else
                    return IDByEntryOrGUIDBySourceType[sourceType][entryOrGUID];
            }
        }

        public void AddRowForQuestCompleteTalkEvent(int creatureTemplateID, int groupID, int questID, string comment)
        {
            AddRow(creatureTemplateID,
                0,          // SMART_SCRIPT_TYPE_CREATURE = 0
                20,         // SMART_EVENT_REWARD_QUEST = 20
                100,
                questID,
                0,
                0,
                0,
                0,
                0,
                1,          // SMART_ACTION_TALK = 1
                groupID,
                0,
                0,
                0,
                0,
                0,
                7,          // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForGameObjectStateTriggerEvent(int sourceGameObjectTemplateID, int targetGameObjectGUID, int targetGameObjectEntryID, string comment)
        {
            AddRow(sourceGameObjectTemplateID, // Negative for GUID, Positive for Entry
                1,  // SMART_SCRIPT_TYPE_GAMEOBJECT
                70, // SMART_EVENT_GO_STATE_CHANGED
                100,
                2,  // State = Active (1 = Ready, 2 = Active Alternative)
                0,
                0,
                0,
                0,
                0,
                9,  // SMART_ACTION_ACTIVATE_GOBJECT,
                0,
                0,
                0,
                0,
                0,
                0,
                14, // SMART_TARGET_GAMEOBJECT_GUID
                targetGameObjectGUID,
                targetGameObjectEntryID,
                0,
                0,
                0,
                0,
                comment);
        }

        public void AddRowForCreatureTemplateInCombatSpellCast(int creatureTemplateID, int recastDelayInMS, int wowSpellID,
            string comment, int eventChance = 100, bool castOnSelf = false)
        {
            int recastDelayInMSMax = recastDelayInMS + Convert.ToInt32(Convert.ToSingle(recastDelayInMS) * Configuration.CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD);
            int targetType = castOnSelf ? 1 : 2; // SMART_TARGET_SELF : SMART_TARGET_VICTIM
            AddRow(creatureTemplateID,
                0,
                0, // SMART_EVENT_UPDATE_IC
                eventChance,
                1, // Initial delay in MS (minimum) - Set to 1 so it defaults after heals
                1, // Initial delay in MS (maximum) - Set to 1 so it defaults after heals
                recastDelayInMS, // Recast delay in MS (minimum)
                recastDelayInMSMax, // Recast delay in MS (maximum)
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                96, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting) + SMARTCAST_AURA_NOT_PRESENT (32)
                0,
                0,
                0,
                0,
                targetType,
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateAttackSpellCastOnVictimNoResetOnLeaveCombat(int creatureTemplateID, int initialDelayInMS, int recastDelayInMS, int wowSpellID, string comment)
        {
            AddRow(creatureTemplateID,
                0,
                0, // SMART_EVENT_UPDATE_IC
                100,
                initialDelayInMS, // Initial delay in MS (minimum)
                initialDelayInMS, // Initial delay in MS (maximum)
                recastDelayInMS,  // Recast delay in MS (minimum)
                recastDelayInMS,  // Recast delay in MS (maximum)
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                64, // SMARTCAST_COMBAT_MOVE (move into range if needed)
                0,
                0,
                0,
                0,
                2, // SMART_TARGET_VICTIM
                0,
                0,
                0,
                0,
                0,
                0,
                comment,
                idOverride: -1,
                eventFlags: 256 // SMART_EVENT_FLAG_DONT_RESET
            );
        }

        public void AddRowForCreatureTemplateEscapeSelfCast(int creatureTemplateID, int healthTriggerPct, int eventChance, int recastDelayInMS, int wowSpellID, string comment)
        {
            AddRow(creatureTemplateID,
                0,
                2, // SMART_EVENT_HEALTH_PCT
                eventChance,
                0,                // HP min %
                healthTriggerPct, // HP max %
                recastDelayInMS,  // Repeat min in MS
                recastDelayInMS,  // Repeat max in MS
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                0,  // Cast flags (self cast, no combat move needed)
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateInCombatSelfBuffCast(int creatureTemplateID, int recastDelayInMS, int eventChance, int wowSpellID, string comment)
        {
            int recastDelayInMSMax = recastDelayInMS + Convert.ToInt32(Convert.ToSingle(recastDelayInMS) * Configuration.CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD);
            AddRow(creatureTemplateID,
                0,
                0, // SMART_EVENT_UPDATE_IC
                eventChance,
                1, // Initial delay in MS (minimum)
                1, // Initial delay in MS (maximum)
                recastDelayInMS, // Recast delay in MS (minimum)
                recastDelayInMSMax, // Recast delay in MS (maximum)
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                96, // SMARTCAST_COMBAT_MOVE (64) + SMARTCAST_AURA_NOT_PRESENT (32)
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateLowHealthSelfCastNoResetOnLeaveCombat(int creatureTemplateID, int healthTriggerPct, int recastDelayInMS, int wowSpellID, string comment)
        {
            AddRow(creatureTemplateID,
                0,
                2, // SMART_EVENT_HEALTH_PCT
                100,
                0,                // HP min %
                healthTriggerPct, // HP max %
                recastDelayInMS,  // Repeat min in MS
                recastDelayInMS,  // Repeat max in MS
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                0,  // Cast flags (self cast, no combat move needed)
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment,
                idOverride: -1,
                eventFlags: 256 // SMART_EVENT_FLAG_DONT_RESET
            );
        }

        public void AddRowForCreatureTemplateInCombatHealCast(int creatureTemplateID, int recastDelayInMS, int wowSpellID,
            int range, string comment)
        {
            int recastDelayInMSMax = recastDelayInMS + Convert.ToInt32(Convert.ToSingle(recastDelayInMS) * Configuration.CREATURE_SPELL_COMBAT_RECAST_DELAY_MAX_ADD_MOD);
            AddRow(creatureTemplateID,
                0,
                74, // SMART_EVENT_FRIENDLY_HEALTH_PCT
                100,
                0, // Initial delay in MS (minimum)
                0, // Initial delay in MS (maximum)
                recastDelayInMS,
                recastDelayInMSMax,
                Configuration.CREATURE_SPELL_COMBAT_HEAL_MIN_LIFE_PERCENT, // HP Min %
                range,
                11, // SMART_ACTION_CAST
                wowSpellID,
                96, // SMARTCAST_COMBAT_MOVE + SMARTCAST_AURA_NOT_PRESENT
                0,
                0,
                0,
                0,
                7, // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateOutOfCombatBuffCastSelf(int creatureTemplateID, int recastDelayInMS, int wowSpellID, string comment)
        {
            int addedDelayRandomValue = RandomGenerator.Next(0, Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS);

            AddRow(creatureTemplateID,
                0,
                1, // SMART_EVENT_UPDATE_OOC
                100,
                Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS + addedDelayRandomValue, // Initial delay in MS (minimum)
                Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS + addedDelayRandomValue, // Initial delay in MS (maximum)
                recastDelayInMS, // Recast delay in MS (minimum)
                recastDelayInMS, // Recast delay in MS (maximum)
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                96, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting) + SMARTCAST_AURA_NOT_PRESENT (32)
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateCastOnAgro(int creatureTemplateID, int chance, int wowSpellID, string comment)
        {
            AddRow(creatureTemplateID,
                0,
                4, // SMART_EVENT_AGGRO
                chance,
                0,
                0,
                0,
                0,
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                64, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting)
                0,
                0,
                0,
                0,
                2, // SMART_TARGET_VICTIM
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateRemoveSpellAuraOnAgro(int creatureTemplateID, int wowSpellID, string comment)
        {
            // Damage event
            AddRow(creatureTemplateID,
                0,
                4, // SMART_EVENT_AGGRO
                100,
                0,
                0,
                0,
                0,
                0,
                0,
                28, // SMART_ACTION_REMOVEAURASFROMSPELL
                wowSpellID,
                0,
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateApplyAuraOnSpawn(int creatureTemplateID, int wowSpellID, string comment)
        {
            AddRow(creatureTemplateID,
                0,
                63, // SMART_EVENT_JUST_CREATED
                100,
                0,
                0,
                0,
                0,
                0,
                0,
                75, // SMART_ACTION_ADD_AURA
                wowSpellID,
                0,
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowForCreatureTemplateCastOnSummoned(int creatureID, int wowSpellID, string comment)
        {
            // Damage event
            AddRow(creatureID,
                0,
                17, // SMART_EVENT_SUMMONED_UNIT
                100,
                0,
                0,
                0,
                0,
                0,
                0,
                11, // SMART_ACTION_ADD_AURA
                wowSpellID,
                0,
                0,
                0,
                0,
                0,
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );
        }

        public void AddRowsForCreatureTimedActionListOfOutOfCombatSpells(int creatureTemplateID, List<int> spellTemplateIDs, string comment)
        {
            if (spellTemplateIDs.Count > 6)
            {
                Logger.WriteWarning("For AddRowsForCreatureTimedActionListOfOutOfCombatSpells added for creature with ID ", creatureTemplateID.ToString(), " there were > 6 spellIDs, so from 7 on will be skipped");
            }

            // Create rows for the spells
            List<int> generatedEntryOrGUIDIDs = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                // Generate the IDs
                if (i >= spellTemplateIDs.Count)
                {
                    generatedEntryOrGUIDIDs.Add(0);
                    continue;
                }
                int curGeneratedID = (creatureTemplateID * 100) + i;
                generatedEntryOrGUIDIDs.Add(curGeneratedID);

                // Add the OOC row
                AddRow(curGeneratedID,
                    9,
                    1, // SMART_EVENT_UPDATE_OOC
                    100,
                    0, // Initial delay in MS (minimum)
                    0, // Initial delay in MS (maximum)
                    0, // Recast delay in MS (minimum)
                    0, // Recast delay in MS (maximum)
                    0, 0,
                    11, // SMART_ACTION_CAST
                    spellTemplateIDs[i],
                    96, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting) + SMARTCAST_AURA_NOT_PRESENT (32)
                    0,
                    0,
                    0,
                    0,
                    1, // SMART_TARGET_SELF
                    0, 0, 0, 0, 0, 0,
                    string.Concat(comment, " Spell Random ", i),
                    0
                );
            }

            // Add the trigger
            int addedDelayRandomValue = RandomGenerator.Next(0, Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_RANDOM_RANGE_ADD_IN_MS);
            AddRow(creatureTemplateID,
                0,
                1, // SMART_EVENT_UPDATE_OOC
                100,
                Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MIN_IN_MS + addedDelayRandomValue, // Initial delay in MS (minimum)
                Configuration.CREATURE_SPELL_OCC_BUFF_INITIAL_DELAY_MAX_IN_MS + addedDelayRandomValue, // Initial delay in MS (maximum)
                0, // Recast delay in MS (minimum)
                0, // Recast delay in MS (maximum)
                0,
                0,
                87, // SMART_ACTION_CALL_RANDOM_TIMED_ACTIONLIST
                generatedEntryOrGUIDIDs[0],
                generatedEntryOrGUIDIDs[1],
                generatedEntryOrGUIDIDs[2],
                generatedEntryOrGUIDIDs[3],
                generatedEntryOrGUIDIDs[4],
                generatedEntryOrGUIDIDs[5],
                1, // SMART_TARGET_SELF
                0,
                0,
                0,
                0,
                0,
                0,
                string.Concat(comment, " Trigger Controller")
            );
        }

        public void AddRowForCreatureTemplateApplySpellOnDamageDone(int creatureTemplateID, int chance, int wowSpellID, int cooldownInMS, /* int spellVisualImpactKitID, */string comment)
        {
            AddRow(creatureTemplateID,
                0,
                33, // SMART_EVENT_DAMAGED_TARGET
                chance,
                0, // Min Damage
                1000000, // Max Damage (ensures always happens
                cooldownInMS, // Cooldown min (ms) between procs
                cooldownInMS, // Cooldown max (ms) between procs
                0,
                0,
                11, // SMART_ACTION_CAST
                wowSpellID,
                64, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting)
                0,
                0,
                0,
                0,
                2, // SMART_TARGET_VICTIM
                0,
                0,
                0,
                0,
                0,
                0,
                comment
            );

            //// Spell Visual
            //AddRow(creatureTemplateID,
            //   0,
            //   33, // SMART_EVENT_DAMAGED_TARGET
            //   chance,
            //   0, // Min Damage
            //   1000000, // Max Damage (ensures always happens
            //   0,
            //   0,
            //   0,
            //   0,
            //   11, // SMART_ACTION_CAST
            //   wowSpellID,
            //   64, // SMARTCAST_COMBAT_MOVE (64) (prevents creature moving during casting)
            //   2, // SMART_TARGET_VICTIM
            //   0,
            //   0,
            //   comment
            //);
        }

        public void AddRowForGameObjectTriggeredTeleportOnActivate(int gameObjectTemplateID, int targetMapID, float targetX, float targetY, float targetZ,
            float targetOrientation, string comment)
        {
            // Locked (keyed) teleports activate through the key's unlock "spell", which skips the gossip so listen here for the state (both unlock and clicking will cause this)
            AddRow(gameObjectTemplateID, // Negative for GUID, Positive for Entry
                1,  // SMART_SCRIPT_TYPE_GAMEOBJECT
                70, // SMART_EVENT_GO_STATE_CHANGED
                100,
                2,  // State = Active (1 = Ready, 2 = Active Alternative)
                0,
                0,
                0,
                0,
                0,
                62,  // SMART_ACTION_TELEPORT,
                targetMapID,
                0,
                0,
                0,
                0,
                0,
                7, // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                targetX,
                targetY,
                targetZ,
                targetOrientation,
                comment);
        }

        public void AddRowForGameObjectTriggeredTeleport(int gameObjectTemplateID, int targetMapID, float targetX, float targetY, float targetZ,
            float targetOrientation, string comment)
        {
            AddRow(gameObjectTemplateID, // Negative for GUID, Positive for Entry
                1,  // SMART_SCRIPT_TYPE_GAMEOBJECT
                64, // SMART_EVENT_GOSSIP_HELLO - When you click on it
                100,
                0,
                0,
                0,
                0,
                0,
                0,
                62,  // SMART_ACTION_TELEPORT,
                targetMapID,
                0,
                0,
                0,
                0,
                0,
                7, // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                targetX,
                targetY,
                targetZ,
                targetOrientation,
                comment);
        }

        public void AddRowForMenuOptionTriggeredTeleport(int creatureEntry, int menuID, int menuOptionID, int targetMapID, float targetX, float targetY, float targetZ,
            float targetOrientation, string comment)
        {
            AddRow(creatureEntry, // Negative for GUID, Positive for Entry
                0,  // SMART_SCRIPT_TYPE_CREATURE
                62, // SMART_EVENT_GOSSIP_SELECT
                100,
                menuID,
                menuOptionID,
                0,
                0,
                0,
                0,
                62,  // SMART_ACTION_TELEPORT
                targetMapID,
                0,
                0,
                0,
                0,
                0,
                7, // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                targetX,
                targetY,
                targetZ,
                targetOrientation,
                comment);
        }

        public void AddRowForMenuOptionTriggeredAura(int creatureEntry, int menuID, int menuOptionID, int spellTemplateID, string comment)
        {
            AddRow(creatureEntry, // Negative for GUID, Positive for Entry
                0,  // SMART_SCRIPT_TYPE_CREATURE
                62, // SMART_EVENT_GOSSIP_SELECT
                100,
                menuID,
                menuOptionID,
                0,
                0,
                0,
                0,
                11,  // SMART_ACTION_CAST
                spellTemplateID,
                0,
                0,
                0,
                0,
                0,
                7, // SMART_TARGET_ACTION_INVOKER
                0,
                0,
                0,
                0,
                0,
                0,
                comment);
        }

        public void AddRow(int entryOrGUIDID, int sourceType, int eventType, int eventChance, int eventParam1, int eventParam2, int eventParam3, int eventParam4,
            int eventParam5, int eventParam6, int actionType, int actionParam1, int actionParam2, int actionParam3, int actionParam4, int actionParam5, int actionParam6, 
            int targetType, int targetParam1, int targetParam2, float targetX, float targetY, float targetZ, float targetOrientation, string comment, int idOverride = -1,
            int eventFlags = 0)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entryorguid", entryOrGUIDID);
            newRow.AddInt("source_type", sourceType); // 0 = Creature, 1 = GameObject, 2 = AreaTrigger, 9 = TimedActionList
            if (idOverride == -1)
                newRow.AddInt("id", GetUniqueID(entryOrGUIDID, sourceType));
            else
                newRow.AddInt("id", idOverride);
            newRow.AddInt("link", 0);
            newRow.AddInt("event_type", eventType); 
            newRow.AddInt("event_phase_mask", 0);
            newRow.AddInt("event_chance", eventChance);
            newRow.AddInt("event_flags", eventFlags);
            newRow.AddInt("event_param1", eventParam1);
            newRow.AddInt("event_param2", eventParam2);
            newRow.AddInt("event_param3", eventParam3);
            newRow.AddInt("event_param4", eventParam4);
            newRow.AddInt("event_param5", eventParam5);
            newRow.AddInt("event_param6", eventParam6);
            newRow.AddInt("action_type", actionType);
            newRow.AddInt("action_param1", actionParam1);
            newRow.AddInt("action_param2", actionParam2);
            newRow.AddInt("action_param3", actionParam3);
            newRow.AddInt("action_param4", actionParam4);
            newRow.AddInt("action_param5", actionParam5);
            newRow.AddInt("action_param6", actionParam6);
            newRow.AddInt("target_type", targetType);
            newRow.AddInt("target_param1", targetParam1);
            newRow.AddInt("target_param2", targetParam2);
            newRow.AddInt("target_param3", 0);
            newRow.AddInt("target_param4", 0);
            newRow.AddFloat("target_x", targetX);
            newRow.AddFloat("target_y", targetY);
            newRow.AddFloat("target_z", targetZ);
            newRow.AddFloat("target_o", targetOrientation);
            newRow.AddString("comment", comment);
            Rows.Add(newRow);
        }
    }
}
