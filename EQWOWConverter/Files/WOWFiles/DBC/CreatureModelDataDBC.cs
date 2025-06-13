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
    internal class CreatureModelDataDBC : DBCFile
    {
        public void AddRow(CreatureModelTemplate creatureModelTemplate, string modelName)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(creatureModelTemplate.DBCCreatureModelDataID); // ID
            newRow.AddPackedFlags(0); // Flags,0x40: ?, 0x80: Can Form Mount, 0x10000: Has Wheels
            newRow.AddString(modelName); // Model Path ("Creature\....mdx), always ending in mdx
            newRow.AddInt32(1); // SizeClass (Big models are ~4, most 1)
            newRow.AddFloat(1); // ModelScale
            newRow.AddInt32(1); // BloodID (UnitBloodLevels.dbc)
            newRow.AddInt32(-1); // FootprintTextureID, -1 is none and references FootprintTextures.dbc
            newRow.AddFloat(18); // FootprintTextureLength, almost always 18
            newRow.AddFloat(12); // FootprintTextureWidth, most 12, but 0 - 20
            newRow.AddFloat(1); // FootprintParticleScale, most 1, but 0 - 5
            newRow.AddInt32(0); // FoleyMaterialID, always 0
            newRow.AddInt32(0); // FootstepShakeSize, references CameraShakes.dbc
            newRow.AddInt32(0); // DeathThudShakeSize, references CameraShakes.dbc
            newRow.AddInt32(creatureModelTemplate.DBCCreatureSoundDataID); // SoundID, references CreatureSoundData.dbc
            newRow.AddFloat(0.6944f); // CollisionWidth, must be > 0.41670012920929
            newRow.AddFloat(2.083f); // CollisionHeight
            newRow.AddFloat(0); // MountHeight
            newRow.AddFloat(-1 * creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMinX, Min vert X
            newRow.AddFloat(-1 * creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMinY, Min vert Y
            newRow.AddFloat(-1 * creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMinZ, Min vert Z
            newRow.AddFloat(creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMaxX, Max vert X
            newRow.AddFloat(creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMaxY, Max vert Y
            newRow.AddFloat(creatureModelTemplate.Race.GeoboxInradius); // GeoBoxMaxZ, Max vert Z
            newRow.AddFloat(1); // WorldEffectScale, typical is 1 but can be 0.03 - 0.9
            newRow.AddFloat(1); // AttachedEffectScale, typical is 1, but can be 0.5 - 2.9
            newRow.AddFloat(0); // MissileCollisionRadius
            newRow.AddFloat(0); // MissileCollisionPush
            newRow.AddFloat(0); // MissileCollisionRaise
            Rows.Add(newRow);

            // Sort by ID
            newRow.SortValue1 = creatureModelTemplate.DBCCreatureModelDataID;
        }

        protected override void OnPostLoadDataFromDisk()
        {
            // Convert any raw data rows to actual data rows (which should be all of them)
            foreach (DBCRow row in Rows)
            {
                // This shouldn't be possible, but control for it just in case
                if (row.SourceRawBytes.Count == 0)
                {
                    Logger.WriteError("FactionDBC had no source raw bytes when converting a row in OnPostLoadDataFromDisk");
                    continue;
                }

                // Fill every field
                int byteCursor = 0;
                row.AddIntFromSourceRawBytes(ref byteCursor); // ID
                row.AddPackedFlagsFromSourceRawBytes(ref byteCursor); // Flags
                row.AddStringFromSourceRawBytes(ref byteCursor, StringBlock); // Model Path
                row.AddIntFromSourceRawBytes(ref byteCursor); // SizeClass
                row.AddFloatFromSourceRawBytes(ref byteCursor); // ModelScale
                row.AddIntFromSourceRawBytes(ref byteCursor); // BloodID
                row.AddIntFromSourceRawBytes(ref byteCursor); // FootprintTextureID
                row.AddFloatFromSourceRawBytes(ref byteCursor); // FootprintTextureLength
                row.AddFloatFromSourceRawBytes(ref byteCursor); // FootprintTextureWidth
                row.AddFloatFromSourceRawBytes(ref byteCursor); // FootprintParticleScale
                row.AddIntFromSourceRawBytes(ref byteCursor); // FoleyMaterialID
                row.AddIntFromSourceRawBytes(ref byteCursor); // FootstepShakeSize
                row.AddIntFromSourceRawBytes(ref byteCursor); // DeathThudShakeSize
                row.AddIntFromSourceRawBytes(ref byteCursor); // SoundID
                row.AddFloatFromSourceRawBytes(ref byteCursor); // CollisionWidth
                row.AddFloatFromSourceRawBytes(ref byteCursor); // CollisionHeight
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MountHeight
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMinX
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMinY
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMinZ
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMaxX
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMaxY
                row.AddFloatFromSourceRawBytes(ref byteCursor); // GeoBoxMaxZ
                row.AddFloatFromSourceRawBytes(ref byteCursor); // WorldEffectScale
                row.AddFloatFromSourceRawBytes(ref byteCursor); // AttachedEffectScale
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MissileCollisionRadius
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MissileCollisionPush
                row.AddFloatFromSourceRawBytes(ref byteCursor); // MissileCollisionRase

                // Purge raw data
                row.SourceRawBytes.Clear();

                // Sort on ID
                row.SortValue1 = ((DBCRow.DBCFieldInt32)row.AddedFields[0]).Value; // ID
            }

            // Update collision heights if needed
            if (Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED == true)
            {
                // IDs to update
                HashSet<int> playerModelIDs = new HashSet<int>();
                playerModelIDs.Add(49); // Human Male
                playerModelIDs.Add(50); // Human Female
                playerModelIDs.Add(51); // Orc Male
                playerModelIDs.Add(52); // Orc Female
                playerModelIDs.Add(53); // Dwarf Male
                playerModelIDs.Add(54); // Dwarf Female
                playerModelIDs.Add(55); // Night Elf Male
                playerModelIDs.Add(56); // Night Elf Female
                playerModelIDs.Add(57); // Undead Male
                playerModelIDs.Add(58); // Undead Female
                playerModelIDs.Add(59); // Tauren Male
                playerModelIDs.Add(60); // Tauren Female
                playerModelIDs.Add(182); // Gnome Male
                playerModelIDs.Add(183); // Gnome Female
                playerModelIDs.Add(185); // Troll Male
                playerModelIDs.Add(186); // Troll Female
                playerModelIDs.Add(2208); // Blood Elf Male
                playerModelIDs.Add(2209); // Blood Elf Female
                playerModelIDs.Add(2248); // Draenei Male
                playerModelIDs.Add(2250); // Draenei Female

                // Look for these IDs and update them if they are larger than the max
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 idField = (DBCRow.DBCFieldInt32)row.AddedFields[0];
                    if (playerModelIDs.Contains(idField.Value))
                    {
                        DBCRow.DBCFieldFloat collisionHeight = (DBCRow.DBCFieldFloat)row.AddedFields[15];
                        if (collisionHeight.Value > Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX)
                            collisionHeight.Value = Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX;
                    }
                }
            }
        }
    }
}
