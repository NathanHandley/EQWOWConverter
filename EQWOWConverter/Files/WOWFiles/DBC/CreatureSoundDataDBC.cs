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

using EQWOWConverter.Creatures;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureSoundDataDBC : DBCFile
    {
        public void AddRow(int id, CreatureRace creatureRace, int creatureFootstepSoundID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(id); // ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundAttackName, creatureRace.SoundMaxDistance)); // Exertion SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundAttackName, creatureRace.SoundMaxDistance)); // Exertion Critical SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundHit1Name, creatureRace.SoundMaxDistance)); // Injury SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundHit3Name, creatureRace.SoundMaxDistance)); // Injury Critical SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundHit4Name, creatureRace.SoundMaxDistance)); // Injury Crushing Blow SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundDeathName, creatureRace.SoundMaxDistance)); // Death SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundHit2Name, creatureRace.SoundMaxDistance)); // Stun SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundLoopName, creatureRace.SoundMaxDistance)); // Stand SoundEntriesDBC.ID
            newRow.AddInt32(creatureFootstepSoundID); // Footstep sound in FootstepTerrainLookupDBC.ID
            newRow.AddInt32(0); // Agro SoundEntriesDBC.ID
            newRow.AddInt32(0); // Wing Flap SoundEntriesDBC.ID
            newRow.AddInt32(0); // Wing Glide SoundEntriesDBC.ID
            newRow.AddInt32(0); // Alert SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundIdle1Name, creatureRace.SoundMaxDistance)); // Fidget 1 SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundIdle2Name, creatureRace.SoundMaxDistance)); // Fidget 2 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Fidget 3 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Fidget 4 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Fidget 5 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Custom Attack 1 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Custom Attack 2 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Custom Attack 3 SoundEntriesDBC.ID
            newRow.AddInt32(0); // Custom Attack 4 SoundEntriesDBC.ID
            newRow.AddInt32(0); // NPC Sound SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundLoopName, creatureRace.SoundMaxDistance)); // Loop Sound (idle?) SoundEntriesDBC.ID
            newRow.AddInt32(0); // Creature impact type
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundJumpName, creatureRace.SoundMaxDistance)); // Jump Start ID SoundEntriesDBC.ID
            newRow.AddInt32(0); // Jump End ID SoundEntriesDBC.ID
            newRow.AddInt32(0); // Pet Attack ID SoundEntriesDBC.ID
            newRow.AddInt32(0); // Pet Order ID SoundEntriesDBC.ID
            newRow.AddInt32(0); // Pet Dismiss ID SoundEntriesDBC.ID
            newRow.AddFloat(0); // Fidget Delay Seconds Min (Time / Interval? 30 seconds?)
            newRow.AddFloat(0); // Fidget Delay Seconds Max (Time / Interval? 60 seconds?)
            newRow.AddInt32(0); // Birth SoundEntriesDBC.ID
            newRow.AddInt32(CreatureRace.GetSoundIDForSound(creatureRace.SoundSpellAttackName, creatureRace.SoundMaxDistance)); // Spell Cast Directed SoundEntriesDBC.ID
            newRow.AddInt32(0); // Submerge SoundEntriesDBC.ID
            newRow.AddInt32(0); // Submerged SoundEntriesDBC.ID
            newRow.AddInt32(0); // Creature Sound Data ID Pet (?) SoundEntriesDBC.ID
            Rows.Add(newRow);
        }
    }
}
