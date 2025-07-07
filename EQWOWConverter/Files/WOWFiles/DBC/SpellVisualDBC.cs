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
    internal class SpellVisualDBC : DBCFile
    {
        private static int CUR_ID = Configuration.DBCID_SPELLVISUAL_ID_START;
        private static readonly object SpellVisualIDLock = new object();

        public void AddRow(int id, string name, string relativeFileName)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(0); // PrecastKit (SpellVisualKit.ID for the casting)
            newRow.AddInt32(0); // CastKit" (SpellVisualKit.ID when the spell lets loose)
            newRow.AddInt32(0); // ImpactKit (SpellVisualKit.ID on the target)
            newRow.AddInt32(0); // StateKit (SpellVisualKit.ID shown when a buff/debuff is on the target)
            newRow.AddInt32(0); // StateDoneKit (SpellVisualKit.ID)
            newRow.AddInt32(0); // ChannelKit (SpellVisualKit.ID for channeling a spell)
            newRow.AddInt32(0); // HasMissile (1 = true, 0 = false)
            newRow.AddInt32(0); // MissileModel (SpellVisualEffectName.ID for the projectile)
            newRow.AddInt32(0); // MissilePathType (Mostly 0, rarely 1?)
            newRow.AddInt32(0); // MissileDestinationAttachment (Usually 1)
            newRow.AddInt32(0); // MissileSound (SoundEntries.ID for the missle)
            newRow.AddInt32(0); // AnimEventSoundID (SoundEntries.ID as well, but mostly 0)
            newRow.AddInt32(0); // Flags (Usually 0, but has some other values as well. Research)
            newRow.AddInt32(0); // CasterImpactKit (SpellVisualKit.ID, usually 0)
            newRow.AddInt32(0); // TargetImpactKit (SpellVisualKit.ID, usually 0)
            newRow.AddInt32(-1); // MissileAttachment (Unually -1, but values can go up to 5 it seems)
            newRow.AddInt32(0); // MissileFollowGroundHeight (Usuually 0, but see some 100s or larger)
            newRow.AddInt32(0); // MissileFollowGroundDropSpeed (Usually 0)
            newRow.AddInt32(0); // MissileFollowGroundApproach (Usually 0)
            newRow.AddInt32(0); // MissileFollowGroundFlags (Unknown, can go up to 15)
            newRow.AddInt32(0); // MissileMotion (Unknown, can be as high as 2853)
            newRow.AddInt32(0); // MissileTargetingKit (SpellVisualKit.ID, Typically 0, probably plays when a cast is in progress)
            newRow.AddInt32(0); // InstantAreaKit (SpellVisualKit.ID)
            newRow.AddInt32(0); // ImpactAreaKit (SpellVisualKit.ID)
            newRow.AddInt32(0); // PersistentAreaKit (SpellVisualKit.ID)
            newRow.AddFloat(0); // MissileCastOffsetX
            newRow.AddFloat(0); // MissileCastOffsetY
            newRow.AddFloat(0); // MissileCastOffsetZ
            newRow.AddFloat(0); // MissileImpactOffsetX
            newRow.AddFloat(0); // MissileImpactOffsetY
            newRow.AddFloat(0); // MissileImpactOffsetZ
            Rows.Add(newRow);
        }

        public static int GenerateID()
        {
            lock (SpellVisualIDLock)
            {
                int id = CUR_ID;
                CUR_ID++;
                return id;
            }
        }
    }
}
