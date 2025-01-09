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
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class CreatureModelDataDBC : DBCFile
    {
        public void AddRow(CreatureModelTemplate creatureModelTemplate, string modelName)
        {
            DBCRow newRow = new DBCRow();
            newRow.AddInt(creatureModelTemplate.DBCCreatureModelDataID); // ID
            newRow.AddPackedFlags(0); // Flags,0x40: ?, 0x80: Can Form Mount, 0x10000: Has Wheels
            newRow.AddString(modelName); // Model Path ("Creature\....mdx), always ending in mdx
            newRow.AddInt(1); // SizeClass (Big models are ~4, most 1)
            newRow.AddFloat(1); // ModelScale
            newRow.AddInt(1); // BloodID (UnitBloodLevels.dbc)
            newRow.AddInt(-1); // FootprintTextureID, -1 is none and references FootprintTextures.dbc
            newRow.AddFloat(18); // FootprintTextureLength, almost always 18
            newRow.AddFloat(12); // FootprintTextureWidth, most 12, but 0 - 20
            newRow.AddFloat(1); // FootprintParticleScale, most 1, but 0 - 5
            newRow.AddInt(0); // FoleyMaterialID, always 0
            newRow.AddInt(0); // FootstepShakeSize, references CameraShakes.dbc
            newRow.AddInt(0); // DeathThudShakeSize, references CameraShakes.dbc
            newRow.AddInt(creatureModelTemplate.DBCCreatureSoundDataID); // SoundID, references CreatureSoundData.dbc
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
        }
    }
}
