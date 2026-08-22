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
using EQWOWConverter.Creatures;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureModelDataDBC : DBCFile
    {
        // Third person camera constants taken out of the 3.3.5 client.
        // Note: The camera builds its pivot as (MouthBreath attachment Z + CAMERA_PIVOT_ANCHOR_ADD) * the unit's object scale (no ModelScale and no CreatureDisplayInfo scale), then clamps it to
        //   (effective collision height - CAMERA_PIVOT_CEILING_SUBTRACT) * CAMERA_PIVOT_CEILING_MULTIPLIER, where the collision height is always the player's OWN race and is never re-read from an illusion form
        public const float CAMERA_PIVOT_ANCHOR_ADD = 0.0972222f;
        public const float CAMERA_PIVOT_CEILING_SUBTRACT = 0.1666667f;
        public const float CAMERA_PIVOT_CEILING_MULTIPLIER = 1.2f;

        public static float GetCameraPivotCeiling(float effectiveCollisionHeight)
        {
            return (effectiveCollisionHeight - CAMERA_PIVOT_CEILING_SUBTRACT) * CAMERA_PIVOT_CEILING_MULTIPLIER;
        }


        public void AddRow(CreatureModelTemplate creatureModelTemplate, string modelName, int modelDataID, int creatureSoundDataID)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt32(modelDataID); // ID
            newRow.AddPackedFlags(0); // Flags,0x40: ?, 0x80: Can Form Mount, 0x10000: Has Wheels
            newRow.AddString(modelName); // Model Path ("Creature\....mdx), always ending in mdx
            newRow.AddInt32(1); // SizeClass (Big models are ~4, most 1)
            newRow.AddFloat(1); // ModelScale (always 1 since a model template's size lives in CreatureDisplayInfo (see CreatureModelTemplate.GetDBCDisplayScale))
            newRow.AddInt32(1); // BloodID (UnitBloodLevels.dbc)
            newRow.AddInt32(-1); // FootprintTextureID, -1 is none and references FootprintTextures.dbc
            newRow.AddFloat(18); // FootprintTextureLength, almost always 18
            newRow.AddFloat(12); // FootprintTextureWidth, most 12, but 0 - 20
            newRow.AddFloat(1); // FootprintParticleScale, most 1, but 0 - 5
            newRow.AddInt32(0); // FoleyMaterialID, always 0
            newRow.AddInt32(0); // FootstepShakeSize, references CameraShakes.dbc
            newRow.AddInt32(0); // DeathThudShakeSize, references CameraShakes.dbc
            newRow.AddInt32(creatureSoundDataID); // SoundID, references CreatureSoundData.dbc
            float collisionScaleCompensation = Configuration.GENERATE_EQUIPMENT_SCALE / Configuration.GENERATE_CREATURE_SCALE;
            newRow.AddFloat(0.6944f * collisionScaleCompensation); // CollisionWidth
            newRow.AddFloat(2.083f * collisionScaleCompensation); // CollisionHeight
            newRow.AddFloat(0); // MountHeight
            // Clickable boundary box
            BoundingBox clickBox = creatureModelTemplate.ModelClickBoundingBox;
            bool hasGeometryBox = clickBox.GetXDistance() > Configuration.GENERATE_FLOAT_EPSILON || clickBox.GetYDistance() > Configuration.GENERATE_FLOAT_EPSILON || clickBox.GetZDistance() > Configuration.GENERATE_FLOAT_EPSILON;
            float geoBoxMinX, geoBoxMinY, geoBoxMinZ, geoBoxMaxX, geoBoxMaxY, geoBoxMaxZ;
            if (hasGeometryBox == true)
            {
                geoBoxMinX = clickBox.BottomCorner.X;
                geoBoxMinY = clickBox.BottomCorner.Y;
                geoBoxMinZ = clickBox.BottomCorner.Z;
                geoBoxMaxX = clickBox.TopCorner.X;
                geoBoxMaxY = clickBox.TopCorner.Y;
                geoBoxMaxZ = clickBox.TopCorner.Z;
            }
            else
            {
                float inradius = creatureModelTemplate.Race.GeoboxInradius;
                geoBoxMinX = geoBoxMinY = geoBoxMinZ = -1 * inradius;
                geoBoxMaxX = geoBoxMaxY = geoBoxMaxZ = inradius;
            }
            newRow.AddFloat(geoBoxMinX); // GeoBoxMinX, Min vert X
            newRow.AddFloat(geoBoxMinY); // GeoBoxMinY, Min vert Y
            newRow.AddFloat(geoBoxMinZ); // GeoBoxMinZ, Min vert Z
            newRow.AddFloat(geoBoxMaxX); // GeoBoxMaxX, Max vert X
            newRow.AddFloat(geoBoxMaxY); // GeoBoxMaxY, Max vert Y
            newRow.AddFloat(geoBoxMaxZ); // GeoBoxMaxZ, Max vert Z
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
                Dictionary<int, float> playerModelDisplayScales = new Dictionary<int, float>();
                playerModelDisplayScales.Add(49, 1.0f);    // Human Male
                playerModelDisplayScales.Add(50, 1.0f);    // Human Female
                playerModelDisplayScales.Add(51, 1.0f);    // Orc Male
                playerModelDisplayScales.Add(52, 1.0f);    // Orc Female
                playerModelDisplayScales.Add(53, 1.0f);    // Dwarf Male
                playerModelDisplayScales.Add(54, 1.0f);    // Dwarf Female
                playerModelDisplayScales.Add(55, 1.0f);    // Night Elf Male
                playerModelDisplayScales.Add(56, 1.0f);    // Night Elf Female
                playerModelDisplayScales.Add(57, 1.0f);    // Undead Male
                playerModelDisplayScales.Add(58, 1.0f);    // Undead Female
                playerModelDisplayScales.Add(59, 1.35f);   // Tauren Male (display 59 renders at scale 1.35)
                playerModelDisplayScales.Add(60, 1.0f);    // Tauren Female
                playerModelDisplayScales.Add(182, 1.15f);  // Gnome Male (display 1563 renders at scale 1.15)
                playerModelDisplayScales.Add(183, 1.15f);  // Gnome Female (display 1564 renders at scale 1.15)
                playerModelDisplayScales.Add(185, 1.0f);   // Troll Male
                playerModelDisplayScales.Add(186, 1.0f);   // Troll Female
                playerModelDisplayScales.Add(2208, 1.0f);  // Blood Elf Male
                playerModelDisplayScales.Add(2209, 1.0f);  // Blood Elf Female
                playerModelDisplayScales.Add(2248, 1.0f);  // Draenei Male
                playerModelDisplayScales.Add(2250, 1.0f);  // Draenei Female

                // The client works in the effective height (CollisionHeight x ModelScale x CreatureDisplayInfo scale), and every player model ships ModelScale 1
                foreach (DBCRow row in Rows)
                {
                    DBCRow.DBCFieldInt32 idField = (DBCRow.DBCFieldInt32)row.AddedFields[0];
                    if (playerModelDisplayScales.TryGetValue(idField.Value, out float displayScale) == false)
                        continue;
                    DBCRow.DBCFieldFloat collisionHeight = (DBCRow.DBCFieldFloat)row.AddedFields[15];
                    float effectiveCollisionHeight = collisionHeight.Value * displayScale;
                    if (Configuration.PLAYER_RAISE_MODEL_COLLISION_HEIGHT_FOR_ILLUSION_CAMERA_ENABLED == true)
                        collisionHeight.Value = Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX / displayScale;
                    else if (effectiveCollisionHeight > Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX)
                        collisionHeight.Value = Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_MAX / displayScale;
                    Logger.WriteDebug(string.Concat("Player model ", idField.Value.ToString(), " collision height ", effectiveCollisionHeight.ToString(),  " -> ", (collisionHeight.Value * displayScale).ToString(), " effective, camera pivot ceiling ", GetCameraPivotCeiling(collisionHeight.Value * displayScale).ToString()));
                }
            }
        }
    }
}
