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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class LiquidTypeDBC : DBCFile
    {
        public void AddRows()
        {
            // Water
            DBCRow newRow = new DBCRow();
            newRow.AddInt(200); // ID
            newRow.AddString("EverQuest Water"); // Name
            newRow.AddPackedFlags(15); // Flags
            newRow.AddInt(0); // Type
            newRow.AddInt(1111); // SoundID
            newRow.AddInt(0); // SpellID
            newRow.AddFloat(0); // Max Darken Depth
            newRow.AddFloat(0); // Fog Darken Intensity
            newRow.AddFloat(0); // Ambience Darken Intensity
            newRow.AddFloat(0); // Direction Darken Intensity
            newRow.AddInt(0); // Light ID
            newRow.AddFloat(1); // Particle Scale
            newRow.AddInt(0); // Particle Movement
            newRow.AddInt(0); // Particle Texture Slots
            newRow.AddInt(1); // Material ID
            newRow.AddString("XTextures\\everquest\\eqclear.%d.blp"); // Texture 1
            newRow.AddString(""); // Texture 2 
            newRow.AddString(""); // Texture 3
            newRow.AddString(""); // Texture 4
            newRow.AddString(""); // Texture 5
            newRow.AddString(""); // Texture 6
            newRow.AddInt(0); // Color 1 
            newRow.AddInt(0); // Color 2
            newRow.AddFloat(1); // Float 1
            newRow.AddFloat(0); // Float 2
            newRow.AddFloat(1); // Float 3 
            newRow.AddFloat(0); // Float 4
            newRow.AddFloat(0); // Float 5
            newRow.AddFloat(0); // Float 6
            newRow.AddFloat(0); // Float 7
            newRow.AddFloat(0); // Float 8
            newRow.AddFloat(0); // Float 9
            newRow.AddFloat(0); // Float 10
            newRow.AddFloat(0); // Float 11
            newRow.AddFloat(0); // Float 12 
            newRow.AddFloat(0); // Float 13
            newRow.AddFloat(0); // Float 14
            newRow.AddFloat(0); // Float 15
            newRow.AddFloat(0); // Float 16
            newRow.AddFloat(0); // Float 17
            newRow.AddFloat(0); // Float 18
            newRow.AddInt(1); // Int 1
            newRow.AddInt(1250); // Int 2
            newRow.AddInt(0); // Int 3
            newRow.AddInt(0); // Int 4
            Rows.Add(newRow);

            newRow = new DBCRow();
            newRow.AddInt(201); // ID
            newRow.AddString("EverQuest Green Water"); // Name
            newRow.AddPackedFlags(15); // Flags
            newRow.AddInt(0); // Type
            newRow.AddInt(1111); // SoundID
            newRow.AddInt(0); // SpellID
            newRow.AddFloat(0); // Max Darken Depth
            newRow.AddFloat(0); // Fog Darken Intensity
            newRow.AddFloat(0); // Ambience Darken Intensity
            newRow.AddFloat(0); // Direction Darken Intensity
            newRow.AddInt(0); // Light ID
            newRow.AddFloat(1); // Particle Scale
            newRow.AddInt(0); // Particle Movement
            newRow.AddInt(0); // Particle Texture Slots
            newRow.AddInt(1); // Material ID
            newRow.AddString("XTextures\\everquest\\eqclear.%d.blp"); // Texture 1
            newRow.AddString(""); // Texture 2 
            newRow.AddString(""); // Texture 3
            newRow.AddString(""); // Texture 4
            newRow.AddString(""); // Texture 5
            newRow.AddString(""); // Texture 6
            newRow.AddInt(0); // Color 1 
            newRow.AddInt(0); // Color 2
            newRow.AddFloat(1); // Float 1
            newRow.AddFloat(0); // Float 2
            newRow.AddFloat(1); // Float 3 
            newRow.AddFloat(0); // Float 4
            newRow.AddFloat(0); // Float 5
            newRow.AddFloat(0); // Float 6
            newRow.AddFloat(0); // Float 7
            newRow.AddFloat(0); // Float 8
            newRow.AddFloat(0); // Float 9
            newRow.AddFloat(0); // Float 10
            newRow.AddFloat(0); // Float 11
            newRow.AddFloat(0); // Float 12 
            newRow.AddFloat(0); // Float 13
            newRow.AddFloat(0); // Float 14
            newRow.AddFloat(0); // Float 15
            newRow.AddFloat(0); // Float 16
            newRow.AddFloat(0); // Float 17
            newRow.AddFloat(0); // Float 18
            newRow.AddInt(1); // Int 1
            newRow.AddInt(1250); // Int 2
            newRow.AddInt(0); // Int 3
            newRow.AddInt(0); // Int 4
            Rows.Add(newRow);

            newRow = new DBCRow();
            newRow.AddInt(202); // ID
            newRow.AddString("EverQuest Magma"); // Name
            newRow.AddPackedFlags(120); // Flags
            newRow.AddInt(2); // Type
            newRow.AddInt(3072); // SoundID
            newRow.AddInt(0); // SpellID
            newRow.AddFloat(0); // Max Darken Depth
            newRow.AddFloat(0); // Fog Darken Intensity
            newRow.AddFloat(0); // Ambience Darken Intensity
            newRow.AddFloat(0); // Direction Darken Intensity
            newRow.AddInt(7); // Light ID
            newRow.AddFloat(4); // Particle Scale
            newRow.AddInt(1); // Particle Movement
            newRow.AddInt(1); // Particle Texture Slots
            newRow.AddInt(1); // Material ID
            newRow.AddString("XTextures\\everquest\\eqclear.%d.blp"); // Texture 1
            newRow.AddString(""); // Texture 2 
            newRow.AddString(""); // Texture 3
            newRow.AddString(""); // Texture 4
            newRow.AddString(""); // Texture 5
            newRow.AddString(""); // Texture 6
            newRow.AddInt(0); // Color 1 
            newRow.AddInt(0); // Color 2
            newRow.AddFloat(0); // Float 1
            newRow.AddFloat(0.1f); // Float 2
            newRow.AddFloat(0); // Float 3 
            newRow.AddFloat(0); // Float 4
            newRow.AddFloat(0); // Float 5
            newRow.AddFloat(0); // Float 6
            newRow.AddFloat(0); // Float 7
            newRow.AddFloat(0); // Float 8
            newRow.AddFloat(0); // Float 9
            newRow.AddFloat(0); // Float 10
            newRow.AddFloat(0); // Float 11
            newRow.AddFloat(0); // Float 12 
            newRow.AddFloat(0); // Float 13
            newRow.AddFloat(0); // Float 14
            newRow.AddFloat(0); // Float 15
            newRow.AddFloat(0); // Float 16
            newRow.AddFloat(0); // Float 17
            newRow.AddFloat(0); // Float 18
            newRow.AddInt(0); // Int 1
            newRow.AddInt(0); // Int 2
            newRow.AddInt(0); // Int 3
            newRow.AddInt(0); // Int 4
            Rows.Add(newRow);

            newRow = new DBCRow();
            newRow.AddInt(203); // ID
            newRow.AddString("EverQuest Slime"); // Name
            newRow.AddPackedFlags(322); // Flags
            newRow.AddInt(3); // Type
            newRow.AddInt(3880); // SoundID
            newRow.AddInt(0); // SpellID
            newRow.AddFloat(0); // Max Darken Depth
            newRow.AddFloat(0); // Fog Darken Intensity
            newRow.AddFloat(0); // Ambience Darken Intensity
            newRow.AddFloat(0); // Direction Darken Intensity
            newRow.AddInt(6); // Light ID
            newRow.AddFloat(0); // Particle Scale
            newRow.AddInt(0); // Particle Movement
            newRow.AddInt(0); // Particle Texture Slots
            newRow.AddInt(1); // Material ID
            newRow.AddString("XTextures\\everquest\\eqclear.%d.blp"); // Texture 1
            newRow.AddString(""); // Texture 2 
            newRow.AddString(""); // Texture 3
            newRow.AddString(""); // Texture 4
            newRow.AddString(""); // Texture 5
            newRow.AddString(""); // Texture 6
            newRow.AddInt(0); // Color 1 
            newRow.AddInt(0); // Color 2
            newRow.AddFloat(0); // Float 1
            newRow.AddFloat(0.1f); // Float 2
            newRow.AddFloat(0); // Float 3 
            newRow.AddFloat(0); // Float 4
            newRow.AddFloat(0); // Float 5
            newRow.AddFloat(0); // Float 6
            newRow.AddFloat(0); // Float 7
            newRow.AddFloat(0); // Float 8
            newRow.AddFloat(0); // Float 9
            newRow.AddFloat(0); // Float 10
            newRow.AddFloat(0); // Float 11
            newRow.AddFloat(0); // Float 12 
            newRow.AddFloat(0); // Float 13
            newRow.AddFloat(0); // Float 14
            newRow.AddFloat(0); // Float 15
            newRow.AddFloat(0); // Float 16
            newRow.AddFloat(0); // Float 17
            newRow.AddFloat(0); // Float 18
            newRow.AddInt(0); // Int 1
            newRow.AddInt(0); // Int 2
            newRow.AddInt(0); // Int 3
            newRow.AddInt(0); // Int 4
            Rows.Add(newRow);

            newRow = new DBCRow();
            newRow.AddInt(204); // ID
            newRow.AddString("EverQuest Blood Water"); // Name
            newRow.AddPackedFlags(15); // Flags
            newRow.AddInt(0); // Type
            newRow.AddInt(1111); // SoundID
            newRow.AddInt(0); // SpellID
            newRow.AddFloat(0); // Max Darken Depth
            newRow.AddFloat(0); // Fog Darken Intensity
            newRow.AddFloat(0); // Ambience Darken Intensity
            newRow.AddFloat(0); // Direction Darken Intensity
            newRow.AddInt(0); // Light ID
            newRow.AddFloat(1); // Particle Scale
            newRow.AddInt(0); // Particle Movement
            newRow.AddInt(0); // Particle Texture Slots
            newRow.AddInt(1); // Material ID
            newRow.AddString("XTextures\\everquest\\eqclear.%d.blp"); // Texture 1
            newRow.AddString(""); // Texture 2 
            newRow.AddString(""); // Texture 3
            newRow.AddString(""); // Texture 4
            newRow.AddString(""); // Texture 5
            newRow.AddString(""); // Texture 6
            newRow.AddInt(0); // Color 1 
            newRow.AddInt(0); // Color 2
            newRow.AddFloat(1); // Float 1
            newRow.AddFloat(0); // Float 2
            newRow.AddFloat(1); // Float 3 
            newRow.AddFloat(0); // Float 4
            newRow.AddFloat(0); // Float 5
            newRow.AddFloat(0); // Float 6
            newRow.AddFloat(0); // Float 7
            newRow.AddFloat(0); // Float 8
            newRow.AddFloat(0); // Float 9
            newRow.AddFloat(0); // Float 10
            newRow.AddFloat(0); // Float 11
            newRow.AddFloat(0); // Float 12 
            newRow.AddFloat(0); // Float 13
            newRow.AddFloat(0); // Float 14
            newRow.AddFloat(0); // Float 15
            newRow.AddFloat(0); // Float 16
            newRow.AddFloat(0); // Float 17
            newRow.AddFloat(0); // Float 18
            newRow.AddInt(1); // Int 1
            newRow.AddInt(1250); // Int 2
            newRow.AddInt(0); // Int 3
            newRow.AddInt(0); // Int 4
            Rows.Add(newRow);
        }
    }
}
