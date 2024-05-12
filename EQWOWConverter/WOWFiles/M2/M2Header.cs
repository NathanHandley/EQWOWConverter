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
using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    public class M2HeaderElement
    {
        public UInt32 Count = 0;
        public UInt32 Offset = 0;
        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Count));
            bytes.AddRange(BitConverter.GetBytes(Offset));
            return bytes;
        }
    }

    internal class M2Header
    {
        private string TokenMagic = "MD20";
        private UInt32 Version = 264;
        public M2HeaderElement Name = new M2HeaderElement();
        public M2Flags Flags = 0; // UInt32
        public M2HeaderElement GlobalLoopTimestamps = new M2HeaderElement();
        public M2HeaderElement AnimationSequences = new M2HeaderElement();
        public M2HeaderElement AnimationSequenceLookup = new M2HeaderElement();
        public M2HeaderElement Bones = new M2HeaderElement();
        public M2HeaderElement BoneKeyLookup = new M2HeaderElement();
        public M2HeaderElement Vertices = new M2HeaderElement();
        public UInt32 SkinProfileCount = 0;
        public M2HeaderElement Colors = new M2HeaderElement();
        public M2HeaderElement Textures = new M2HeaderElement();
        public M2HeaderElement TextureTransparencyWeights = new M2HeaderElement();
        public M2HeaderElement TextureTransforms = new M2HeaderElement();
        public M2HeaderElement ReplaceableTextureLookup = new M2HeaderElement();
        public M2HeaderElement Materials = new M2HeaderElement();
        public M2HeaderElement BoneLookup = new M2HeaderElement();
        public M2HeaderElement TextureLookup = new M2HeaderElement();
        public M2HeaderElement TextureMappingLookup = new M2HeaderElement();
        public M2HeaderElement TextureTransparencyLookup = new M2HeaderElement(); // Weights
        public M2HeaderElement TextureTransformsLookup = new M2HeaderElement();
        public BoundingBox BoundingBox = new BoundingBox();
        public float BoundingSphereRadius = 0f;
        public BoundingBox CollisionBox = new BoundingBox();
        public float CollisionSphereRadius = 0f;
        public M2HeaderElement CollisionTriangleIndicies = new M2HeaderElement();
        public M2HeaderElement CollisionVerticies = new M2HeaderElement();
        public M2HeaderElement CollisionFaceNormals = new M2HeaderElement();
        public M2HeaderElement Attachments = new M2HeaderElement();
        public M2HeaderElement AttachmentIndiciesLookup = new M2HeaderElement();
        public M2HeaderElement Events = new M2HeaderElement();
        public M2HeaderElement Lights = new M2HeaderElement();
        public M2HeaderElement Cameras = new M2HeaderElement();
        public M2HeaderElement CamerasIndiciesLookup = new M2HeaderElement();
        public M2HeaderElement RibbonEmitters = new M2HeaderElement();
        public M2HeaderElement ParticleEmitters = new M2HeaderElement();
        public M2HeaderElement SecondTextureMaterialOverrides = new M2HeaderElement(); // Multitexturing will use second material from here for blending with first
        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(TokenMagic));
            bytes.AddRange(BitConverter.GetBytes(Version));
            bytes.AddRange(Name.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            bytes.AddRange(GlobalLoopTimestamps.ToBytes());
            bytes.AddRange(AnimationSequences.ToBytes());
            bytes.AddRange(AnimationSequenceLookup.ToBytes());
            bytes.AddRange(Bones.ToBytes());
            bytes.AddRange(BoneKeyLookup.ToBytes());
            bytes.AddRange(Vertices.ToBytes());
            bytes.AddRange(BitConverter.GetBytes(SkinProfileCount));
            bytes.AddRange(Colors.ToBytes());
            bytes.AddRange(Textures.ToBytes());
            bytes.AddRange(TextureTransparencyWeights.ToBytes());
            bytes.AddRange(TextureTransforms.ToBytes());
            bytes.AddRange(ReplaceableTextureLookup.ToBytes());
            bytes.AddRange(Materials.ToBytes());
            bytes.AddRange(BoneLookup.ToBytes());
            bytes.AddRange(TextureLookup.ToBytes());
            bytes.AddRange(TextureMappingLookup.ToBytes());
            bytes.AddRange(TextureTransparencyLookup.ToBytes());
            bytes.AddRange(TextureTransformsLookup.ToBytes());
            bytes.AddRange(BoundingBox.ToBytesHighRes());
            bytes.AddRange(BitConverter.GetBytes(BoundingSphereRadius));
            bytes.AddRange(CollisionBox.ToBytesHighRes());
            bytes.AddRange(BitConverter.GetBytes(CollisionSphereRadius));
            bytes.AddRange(CollisionTriangleIndicies.ToBytes());
            bytes.AddRange(CollisionVerticies.ToBytes());
            bytes.AddRange(CollisionFaceNormals.ToBytes());
            bytes.AddRange(Attachments.ToBytes());
            bytes.AddRange(AttachmentIndiciesLookup.ToBytes());
            bytes.AddRange(Events.ToBytes());
            bytes.AddRange(Lights.ToBytes());
            bytes.AddRange(Cameras.ToBytes());
            bytes.AddRange(CamerasIndiciesLookup.ToBytes());
            bytes.AddRange(RibbonEmitters.ToBytes());
            bytes.AddRange(ParticleEmitters.ToBytes());
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
                bytes.AddRange(SecondTextureMaterialOverrides.ToBytes());
            return bytes;
        }
        public int GetSize()
        {
            int size = 0;
            size += 4;  // TokenMagic
            size += 4;  // Version
            size += 8;  // Name
            size += 4;  // Flags
            size += 8;  // GlobalLoopTimestamps
            size += 8;  // AnimationSequences
            size += 8;  // AnimationSequenceLookup
            size += 8;  // Bones
            size += 8;  // BoneKeyLookup
            size += 8;  // Vertices
            size += 4;  // SkinProfileCount
            size += 8;  // Colors
            size += 8;  // Textures
            size += 8;  // TextureTransparencyWeights
            size += 8;  // TextureTransforms
            size += 8;  // ReplaceableTextureLookup
            size += 8;  // Materials
            size += 8;  // BoneLookup
            size += 8;  // TextureLookup
            size += 8;  // TextureMappingLookup
            size += 8;  // TextureTransparencyLookup
            size += 8;  // TextureTransformsLookup
            size += 24; // BoundingBox
            size += 4;  // BoundingSphereRadius
            size += 24; // CollisionBox
            size += 4;  // CollisionSphereRadius
            size += 8;  // CollisionTriangleIndicies
            size += 8;  // CollisionVerticies
            size += 8;  // CollisionFaceNormals
            size += 8;  // Attachments
            size += 8;  // AttachmentIndiciesLookup
            size += 8;  // Events
            size += 8;  // Lights
            size += 8;  // Cameras
            size += 8;  // CamerasIndiciesLookup
            size += 8;  // RibbonEmitters
            size += 8;  // ParticleEmitters
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
                size += 8;  // SecondTextureMaterialOverrides
            return size;
        }
    }
}
