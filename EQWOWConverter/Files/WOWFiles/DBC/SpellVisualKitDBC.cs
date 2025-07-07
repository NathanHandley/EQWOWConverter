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
    internal class SpellVisualKitDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_SPELLVISUALKIT_ID_START;
        private static readonly object SpellVisualKitIDLock = new object();

        public void AddRow(int id)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(-1); // StartAnimID (AnimationData.ID, almost always -1)
            newRow.AddInt32(0); // AnimID (AnimationData.ID for the caster)
            newRow.AddInt32(0); // HeadEffect (SpellVisualEffectName.ID, over the head)
            newRow.AddInt32(0); // ChestEffect (SpellVisualEffectName.ID, at the chest)
            newRow.AddInt32(0); // BaseEffect (SpellVisualEffectName.ID, at the ground (base))
            newRow.AddInt32(0); // LeftHandEffect (SpellVisualEffectName.ID, left hand)
            newRow.AddInt32(0); // RightHandEffect (SpellVisualEffectName.ID, right hand)
            newRow.AddInt32(0); // BreathEffect (SpellVisualEffectName.ID, mouth? AoE?)
            newRow.AddInt32(0); // LeftWeaponEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // RightWeaponEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // SpecialEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // SpecialEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // SpecialEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // WorldEffect (SpellVisualEffectName.ID)
            newRow.AddInt32(0); // SoundID (SoundEntries.ID)
            newRow.AddInt32(0); // ShakeID (SpellEffectCameraShakes.ID)
            newRow.AddInt32(-1); // CharProc
            newRow.AddInt32(-1); // CharProc
            newRow.AddInt32(-1); // CharProc
            newRow.AddInt32(-1); // CharProc
            newRow.AddFloat(0); // CharParamZero (Decimal color mask, which was converted from HEX)
            newRow.AddFloat(0); // CharParamZero
            newRow.AddFloat(0); // CharParamZero
            newRow.AddFloat(0); // CharParamZero
            newRow.AddFloat(0); // CharParamOne
            newRow.AddFloat(0); // CharParamOne
            newRow.AddFloat(0); // CharParamOne
            newRow.AddFloat(0); // CharParamOne
            newRow.AddFloat(0); // CharParamTwo
            newRow.AddFloat(0); // CharParamTwo
            newRow.AddFloat(0); // CharParamTwo
            newRow.AddFloat(0); // CharParamTwo
            newRow.AddFloat(0); // CharParamThree
            newRow.AddFloat(0); // CharParamThree
            newRow.AddFloat(0); // CharParamThree
            newRow.AddFloat(0); // CharParamThree
            newRow.AddInt32(0); // Flags
            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            lock (SpellVisualKitIDLock)
            {
                int id = CUR_ID;
                CUR_ID++;
                return id;
            }
        }
    }
}
