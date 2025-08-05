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

        public void AddRowForQuestCompleteTalkEvent(int creatureTemplateID, int groupID, int questID, string comment)
        {
            AddRow(creatureTemplateID,
                   0,          // SMART_SCRIPT_TYPE_CREATURE = 0
                   20,         // SMART_EVENT_REWARD_QUEST = 20
                   questID,
                   0,
                   0,
                   0,
                   1,          // SMART_ACTION_TALK = 1
                   groupID,
                   0,
                   7,          // SMART_TARGET_SELF = 1
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
                2,  // State = Active (1 = Ready, 2 = Active Alternative)
                0,
                0,
                0,
                9,  // SMART_ACTION_ACTIVATE_GOBJECT,
                0,
                0,
                14, // SMART_TARGET_GAMEOBJECT_GUID
                targetGameObjectGUID,
                targetGameObjectEntryID,
                comment);
        }

        public void AddRowForCreatureTemplateInCombatSpellCast(int creatureTemplateID, int recastDelayInMS, int wowSpellID,
            string comment)
        {
            AddRow(creatureTemplateID,
               0,          
               0, // SMART_EVENT_UPDATE_IC
               0, // Initial delay in MS (minimum)
               0, // Initial delay in MS (maximum)
               recastDelayInMS, // Recast delay in MS (minimum)
               recastDelayInMS, // Recast delay in MS (maximum)
               11, // SMART_ACTION_CAST
               wowSpellID,
               64, // SMARTCAST_COMBAT_MOVE (prevents creature moving during casting)
               2, // SMART_TARGET_VICTIM
               0,
               0,
               comment
            );
        }

        public void AddRow(int entryOrGUIDID, int sourceType, int eventType, int eventParam1, int eventParam2, int eventParam3, int eventParam4,
            int actionType, int actionParam1, int actionParam2, int targetType, int targetParam1, int targetParam2, string comment)
        {
            SQLRow newRow = new SQLRow();
            newRow.AddInt("entryorguid", entryOrGUIDID);
            newRow.AddInt("source_type", sourceType); // 0 = Creature, 1 = GameObject, 2 = AreaTrigger, 9 = TimedActionList
            newRow.AddInt("id", GetUniqueID(entryOrGUIDID, sourceType));
            newRow.AddInt("link", 0);
            newRow.AddInt("event_type", eventType); 
            newRow.AddInt("event_phase_mask", 0);
            newRow.AddInt("event_chance", 100);
            newRow.AddInt("event_flags", 0);
            newRow.AddInt("event_param1", eventParam1);
            newRow.AddInt("event_param2", eventParam2);
            newRow.AddInt("event_param3", eventParam3);
            newRow.AddInt("event_param4", eventParam4);
            newRow.AddInt("event_param5", 0);
            newRow.AddInt("event_param6", 0);
            newRow.AddInt("action_type", actionType);
            newRow.AddInt("action_param1", actionParam1);
            newRow.AddInt("action_param2", actionParam2);
            newRow.AddInt("action_param3", 0);
            newRow.AddInt("action_param4", 0);
            newRow.AddInt("action_param5", 0);
            newRow.AddInt("action_param6", 0);
            newRow.AddInt("target_type", targetType);
            newRow.AddInt("target_param1", targetParam1);
            newRow.AddInt("target_param2", targetParam2);
            newRow.AddInt("target_param3", 0);
            newRow.AddInt("target_param4", 0);
            newRow.AddFloat("target_x", 0);
            newRow.AddFloat("target_y", 0);
            newRow.AddFloat("target_z", 0);
            newRow.AddFloat("target_o", 0);
            newRow.AddString("comment", comment);
            Rows.Add(newRow);
        }
    }
}
