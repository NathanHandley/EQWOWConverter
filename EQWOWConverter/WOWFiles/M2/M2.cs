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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Objects;

namespace EQWOWConverter.WOWFiles
{
    internal class M2
    {
        private M2Skin Skin = new M2Skin();
        public List<byte> ModelBytes = new List<byte>();

        public M2(ModelObject modelObject)
        {

            // Header


            // Name

            // Flags

            // Global Loop Timestamps

            // Animation Sequences

            // Animation Sequence ID Lookup

            // Bones

            // Key Bone ID Lookup

            // Verticies

            // Number of Skin Profiles

            // Color and Alpha Animation Definitions

            // Textures

            // Texture Transparencies (Weights)

            // Texture Transforms

            // Replaceable Texture ID Lookup

            // Materials

            // Bone Lookup

            // Texture Lookup

            // Texture Mapping Lookup

            // Texture Transparency Lookup (Weights)

            // Texture Transformations Lookup

            // Bounding Box

            // Bounding Sphere Radius

            // Collision Box

            // Collision Sphere Radius

            // Collision Triangle Incidies

            // Collision Verticies

            // Collision Face Normals

            // Attachments

            // Attachment ID Lookup

            // Events

            // Lights

            // Cameras

            // Camera ID Lookup

            // Ribbon Emitters

            // Particle Emitters

            // Second Texture Material Override (Combos)
        }

        public void GenerateHeader()
        {
            

        }

        public void WriteToDisk(string outputFolderPath)
        {

        }
    }
}
