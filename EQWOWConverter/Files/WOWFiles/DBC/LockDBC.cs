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

namespace EQWOWConverter.WOWFiles
{
    internal class LockDBC : DBCFile
    {
        private const int LOCK_SLOT_COUNT = 8;
        private const int LOCK_TYPE_ITEM = 1;
        private const int LOCK_TYPE_SKILL = 2;
        private const int LOCK_INDEX_PICKLOCK = 1;
        private const int LOCK_ACTION_OPEN = 1;

        public void AddRowForItemKeysAndPickLock(int lockID, int keyItemWOWID, int altKeyItemWOWID, int pickLockSkillRequired)
        {
            List<int> slotTypes = new List<int>();
            List<int> slotIndexes = new List<int>();
            List<int> slotSkills = new List<int>();
            List<int> slotActions = new List<int>();
            if (keyItemWOWID > 0)
            {
                slotTypes.Add(LOCK_TYPE_ITEM);
                slotIndexes.Add(keyItemWOWID);
                slotSkills.Add(0);
                slotActions.Add(LOCK_ACTION_OPEN);
            }
            if (altKeyItemWOWID > 0)
            {
                slotTypes.Add(LOCK_TYPE_ITEM);
                slotIndexes.Add(altKeyItemWOWID);
                slotSkills.Add(0);
                slotActions.Add(LOCK_ACTION_OPEN);
            }
            if (pickLockSkillRequired > 0)
            {
                slotTypes.Add(LOCK_TYPE_SKILL);
                slotIndexes.Add(LOCK_INDEX_PICKLOCK);
                slotSkills.Add(pickLockSkillRequired);
                slotActions.Add(LOCK_ACTION_OPEN);
            }
            if (slotTypes.Count == 0)
            {
                Logger.WriteError("LockDBC was given lock ID '", lockID.ToString(), "' with no key item and no pick lock skill, so it was skipped");
                return;
            }
            if (slotTypes.Count > LOCK_SLOT_COUNT)
            {
                Logger.WriteError("LockDBC was given lock ID '", lockID.ToString(), "' with more conditions than the ", LOCK_SLOT_COUNT.ToString(), " available slots");
                return;
            }
            while (slotTypes.Count < LOCK_SLOT_COUNT)
            {
                slotTypes.Add(0);
                slotIndexes.Add(0);
                slotSkills.Add(0);
                slotActions.Add(0);
            }

            DBCRow newRow = new DBCRow();
            newRow.AddInt32(lockID); // ID
            foreach (int slotType in slotTypes)
                newRow.AddInt32(slotType); // Type_1 through Type_8
            foreach (int slotIndex in slotIndexes)
                newRow.AddInt32(slotIndex); // Index_1 through Index_8
            foreach (int slotSkill in slotSkills)
                newRow.AddInt32(slotSkill); // Skill_1 through Skill_8
            foreach (int slotAction in slotActions)
                newRow.AddInt32(slotAction); // Action_1 through Action_8
            Rows.Add(newRow);
        }
    }
}
