﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Common;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.Zones;

namespace EQWOWConverter.WOWFiles
{
    internal class M2
    {
        private string TokenMagic = "MD20";
        private UInt32 Version = 264;
        private M2StringByOffset Name = new M2StringByOffset(string.Empty);
        private M2Flags Flags = 0; // UInt32
        private M2GenericArrayByOffset<M2Timestamp> GlobalLoopTimestamps = new M2GenericArrayByOffset<M2Timestamp>();
        private M2GenericArrayByOffset<ObjectModelAnimation> AnimationSequences = new M2GenericArrayByOffset<ObjectModelAnimation>();
        private M2GenericArrayByOffset<M2Int16> AnimationSequenceLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2BoneArrayByOffset Bones = new M2BoneArrayByOffset();
        private M2GenericArrayByOffset<M2Int16> BoneKeyLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<ObjectModelVertex> Vertices = new M2GenericArrayByOffset<ObjectModelVertex>();
        private UInt32 SkinProfileCount = 0;
        private M2GenericArrayByOffset<M2Color> Colors = new M2GenericArrayByOffset<M2Color>();
        private M2TextureArrayByOffset Textures;
        private M2TrackSequencesArrayByOffset<Fixed16> TextureTransparencySequences = new M2TrackSequencesArrayByOffset<Fixed16>();
        private M2TextureAnimationArrayByOffset TextureAnimations = new M2TextureAnimationArrayByOffset();
        private M2GenericArrayByOffset<M2Int16> ReplaceableTextureLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<ObjectModelMaterial> Materials = new M2GenericArrayByOffset<ObjectModelMaterial>();
        private M2GenericArrayByOffset<M2Int16> BoneLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<M2Int16> TextureLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<M2Int16> TextureMappingLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<M2Int16> TextureTransparencyLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<M2Int16> TextureAnimationsLookup = new M2GenericArrayByOffset<M2Int16>();
        private BoundingBox BoundingBox = new BoundingBox();
        private float BoundingSphereRadius = 0f;
        private BoundingBox CollisionBox = new BoundingBox();
        private float CollisionSphereRadius = 0f;
        private M2TriangleFacesArrayByOffset CollisionTriangleIndices = new M2TriangleFacesArrayByOffset();
        private M2GenericArrayByOffset<Vector3> CollisionVertices = new M2GenericArrayByOffset<Vector3>();
        private M2GenericArrayByOffset<Vector3> CollisionFaceNormals = new M2GenericArrayByOffset<Vector3>();
        private M2GenericArrayByOffset<M2Attachment> Attachments = new M2GenericArrayByOffset<M2Attachment>();
        private M2GenericArrayByOffset<M2Int16> AttachmentIndicesLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2EventArrayByOffset Events = new M2EventArrayByOffset();
        private M2GenericArrayByOffset<M2Dummy> Lights = new M2GenericArrayByOffset<M2Dummy>();
        private M2GenericArrayByOffset<M2Dummy> Cameras = new M2GenericArrayByOffset<M2Dummy>();
        private M2GenericArrayByOffset<M2Int16> CamerasIndicesLookup = new M2GenericArrayByOffset<M2Int16>();
        private M2GenericArrayByOffset<M2Dummy> RibbonEmitters = new M2GenericArrayByOffset<M2Dummy>();
        private M2GenericArrayByOffset<M2Dummy> ParticleEmitters = new M2GenericArrayByOffset<M2Dummy>();
        private M2GenericArrayByOffset<M2Dummy> SecondTextureMaterialOverrides = new M2GenericArrayByOffset<M2Dummy>(); // Multitexturing will use second material from here for blending with first

        public M2Skin Skin;

        public M2(ObjectModel wowObjectModel, string mpqObjectFolder)
        {
            // Populate the M2 Data objects
            Name = new M2StringByOffset(wowObjectModel.Name);
            Textures = new M2TextureArrayByOffset(mpqObjectFolder);
            PopulateElements(wowObjectModel);
            Skin = new M2Skin(wowObjectModel);
        }

        private void PopulateElements(ObjectModel wowObjectModel)
        {
            // Global Loop Timestamps
            foreach (UInt32 timestamp in wowObjectModel.GlobalLoopSequenceLimits)
                GlobalLoopTimestamps.Add(new M2Timestamp(timestamp));

            // Animation Sequences
            AnimationSequences.AddArray(wowObjectModel.ModelAnimations);

            // Animation Sequence ID Lookup
            foreach (Int16 value in wowObjectModel.AnimationSequenceIDLookups)
                AnimationSequenceLookup.Add(new M2Int16(value));

            // Bones
            Bones.AddModelBones(wowObjectModel.ModelBones);

            // Key Bone ID Lookup
            foreach (Int16 value in wowObjectModel.ModelBoneKeyLookups)
                BoneKeyLookup.Add(new M2Int16(value));

            // Vertices
            Vertices.AddArray(wowObjectModel.ModelVertices);

            // Number of Skin Profiles
            SkinProfileCount = 1;  // Fix to 1 for now

            // Color and Alpha Animation Definitions
            // none for now

            // Textures
            Textures.AddModelTextures(wowObjectModel.ModelTextures);

            // Texture Transparency Sequences
            foreach(var transparencySequenceSet in wowObjectModel.ModelTextureTransparencySequenceSetByMaterialIndex)
                TextureTransparencySequences.Add(transparencySequenceSet.Value);

            // Texture Transforms
            TextureAnimations.AddModelTextureAnimations(wowObjectModel.ModelTextureAnimations);

            // Replaceable Texture ID Lookup
            foreach (Int16 value in wowObjectModel.ModelReplaceableTextureLookups)
                ReplaceableTextureLookup.Add(new M2Int16(value));

            // Materials
            Materials.AddArray(wowObjectModel.ModelMaterials);

            // Bone Lookup
            foreach (Int16 value in wowObjectModel.ModelBoneLookups)
                BoneLookup.Add(new M2Int16(value));

            // Texture Lookup
            foreach (Int16 value in wowObjectModel.ModelTextureLookups)
                TextureLookup.Add(new M2Int16(value));

            // Texture Mapping Lookup
            foreach (Int16 value in wowObjectModel.ModelTextureMappingLookups)
                TextureMappingLookup.Add(new M2Int16(value));

            // Texture Transparency Lookup (Weights)
            foreach (var transparencyValue in wowObjectModel.ModelTextureTransparencyLookups)
                TextureTransparencyLookup.Add(new M2Int16(Convert.ToInt16(transparencyValue)));

            // Texture Transformations Lookup
            foreach (Int16 value in wowObjectModel.ModelTextureAnimationLookup)
                TextureAnimationsLookup.Add(new M2Int16(value));

            // Bounding Box
            BoundingBox = wowObjectModel.BoundingBox;

            // Bounding Sphere Radius
            BoundingSphereRadius = wowObjectModel.BoundingSphereRadius;

            // Collision Box
            CollisionBox = wowObjectModel.CollisionBoundingBox;

            // Collision Sphere Raidus
            CollisionSphereRadius = wowObjectModel.CollisionSphereRaidus;

            // Collision Triangle Incidies
            CollisionTriangleIndices.AddArray(wowObjectModel.CollisionTriangles);

            // Collision Vertices
            CollisionVertices.AddArray(wowObjectModel.CollisionPositions);

            // Collision Face Normals
            CollisionFaceNormals.AddArray(wowObjectModel.CollisionFaceNormals);

            // Attachments
            // none for now

            // Attachment ID Lookup
            // none for now

            // Events
            if (wowObjectModel.SoundIdleLoop != null)
                Events.AddIdleSoundEvent(wowObjectModel.SoundIdleLoop);

            // Lights
            // none for now

            // Cameras
            // none for now

            // Camera ID Lookup
            // none for now

            // Ribbon Emitters
            // none for now

            // Particle Emitters
            // none for now

            // Second Texture Material Override (Combos)
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
            {
                // Do nothing for now, so this flag can't be set
            }
        }

        private UInt32 GetM2HeaderSize()
        {
            UInt32 headerSize = 0;
            headerSize += 4;  // TokenMagic
            headerSize += 4;  // Version
            headerSize += 8;  // Name
            headerSize += 4;  // Flags
            headerSize += 8;  // GlobalLoopTimestamps
            headerSize += 8;  // AnimationSequences
            headerSize += 8;  // AnimationSequenceLookup
            headerSize += 8;  // Bones
            headerSize += 8;  // BoneKeyLookup
            headerSize += 8;  // Vertices
            headerSize += 4;  // SkinProfileCount
            headerSize += 8;  // Colors
            headerSize += 8;  // Textures
            headerSize += 8;  // TextureTransparencyWeights
            headerSize += 8;  // TextureTransforms
            headerSize += 8;  // ReplaceableTextureLookup
            headerSize += 8;  // Materials
            headerSize += 8;  // BoneLookup
            headerSize += 8;  // TextureLookup
            headerSize += 8;  // TextureMappingLookup
            headerSize += 8;  // TextureTransparencyLookup
            headerSize += 8;  // TextureTransformsLookup
            headerSize += 24; // BoundingBox
            headerSize += 4;  // BoundingSphereRadius
            headerSize += 24; // CollisionBox
            headerSize += 4;  // CollisionSphereRadius
            headerSize += 8;  // CollisionTriangleIndices
            headerSize += 8;  // CollisionVertices
            headerSize += 8;  // CollisionFaceNormals
            headerSize += 8;  // Attachments
            headerSize += 8;  // AttachmentIndicesLookup
            headerSize += 8;  // Events
            headerSize += 8;  // Lights
            headerSize += 8;  // Cameras
            headerSize += 8;  // CamerasIndicesLookup
            headerSize += 8;  // RibbonEmitters
            headerSize += 8;  // ParticleEmitters
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
                headerSize += 8;  // SecondTextureMaterialOverrides
            return headerSize;
        }

        public void WriteToDisk(string objectName, string outputFolderPath)
        {
            // Make the directory
            if (Directory.Exists(outputFolderPath) == false)
                FileTool.CreateBlankDirectory(outputFolderPath, true);

            // Create the M2
            string m2FileName = Path.Combine(outputFolderPath, objectName + ".m2");
            List<Byte> fileData = GetFileData();
            File.WriteAllBytes(m2FileName, fileData.ToArray());

            // Create the skin
            Skin.WriteToDisk(outputFolderPath);
        }

        private List<Byte> GetFileData()
        {
            List<Byte> fileBytes = new List<byte>();

            // Reserve header space
            UInt32 headerSize = GetM2HeaderSize();
            for (UInt32 i = 0; i < headerSize; ++i)
                fileBytes.Add(0);

            // Add the data bytes
            Name.AddDataBytes(ref fileBytes);
            GlobalLoopTimestamps.AddDataBytes(ref fileBytes);
            AnimationSequences.AddDataBytes(ref fileBytes);
            AnimationSequenceLookup.AddDataBytes(ref fileBytes);
            Bones.AddDataBytes(ref fileBytes);
            BoneKeyLookup.AddDataBytes(ref fileBytes);
            Vertices.AddDataBytes(ref fileBytes);
            Colors.AddDataBytes(ref fileBytes);
            Textures.AddDataBytes(ref fileBytes);
            TextureTransparencySequences.AddDataBytes(ref fileBytes);
            TextureAnimations.AddDataBytes(ref fileBytes);
            ReplaceableTextureLookup.AddDataBytes(ref fileBytes);
            Materials.AddDataBytes(ref fileBytes);
            BoneLookup.AddDataBytes(ref fileBytes);
            TextureLookup.AddDataBytes(ref fileBytes);
            TextureMappingLookup.AddDataBytes(ref fileBytes);
            TextureTransparencyLookup.AddDataBytes(ref fileBytes);
            TextureAnimationsLookup.AddDataBytes(ref fileBytes);
            CollisionTriangleIndices.AddDataBytes(ref fileBytes);
            CollisionVertices.AddDataBytes(ref fileBytes);
            CollisionFaceNormals.AddDataBytes(ref fileBytes);
            Attachments.AddDataBytes(ref fileBytes);
            AttachmentIndicesLookup.AddDataBytes(ref fileBytes);
            Events.AddDataBytes(ref fileBytes);
            Lights.AddDataBytes(ref fileBytes);
            Cameras.AddDataBytes(ref fileBytes);
            CamerasIndicesLookup.AddDataBytes(ref fileBytes);
            RibbonEmitters.AddDataBytes(ref fileBytes);
            ParticleEmitters.AddDataBytes(ref fileBytes);
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
                SecondTextureMaterialOverrides.AddDataBytes(ref fileBytes);
            // Put an empty byte at the end
            fileBytes.Add(0);

            // Populate the header section
            List<Byte> headerBytes = GetHeaderBytes();
            for (int i = 0; i < headerSize; ++i)
                fileBytes[i] = headerBytes[i];

            // Fill out the tail to a multiple of 16 (not actually required?)
            while (fileBytes.Count() % 16 != 0)
                fileBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            return fileBytes;
        }

        private List<Byte> GetHeaderBytes()
        {
            List<byte> headerBytes = new List<byte>();
            headerBytes.AddRange(Encoding.ASCII.GetBytes(TokenMagic));
            headerBytes.AddRange(BitConverter.GetBytes(Version));
            headerBytes.AddRange(Name.GetHeaderBytes());
            headerBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Flags)));
            headerBytes.AddRange(GlobalLoopTimestamps.GetHeaderBytes());
            headerBytes.AddRange(AnimationSequences.GetHeaderBytes());
            headerBytes.AddRange(AnimationSequenceLookup.GetHeaderBytes());
            headerBytes.AddRange(Bones.GetHeaderBytes());
            headerBytes.AddRange(BoneKeyLookup.GetHeaderBytes());
            headerBytes.AddRange(Vertices.GetHeaderBytes());
            headerBytes.AddRange(BitConverter.GetBytes(SkinProfileCount));
            headerBytes.AddRange(Colors.GetHeaderBytes());
            headerBytes.AddRange(Textures.GetHeaderBytes());
            headerBytes.AddRange(TextureTransparencySequences.GetHeaderBytes());
            headerBytes.AddRange(TextureAnimations.GetHeaderBytes());
            headerBytes.AddRange(ReplaceableTextureLookup.GetHeaderBytes());
            headerBytes.AddRange(Materials.GetHeaderBytes());
            headerBytes.AddRange(BoneLookup.GetHeaderBytes());
            headerBytes.AddRange(TextureLookup.GetHeaderBytes());
            headerBytes.AddRange(TextureMappingLookup.GetHeaderBytes());
            headerBytes.AddRange(TextureTransparencyLookup.GetHeaderBytes());
            headerBytes.AddRange(TextureAnimationsLookup.GetHeaderBytes());
            headerBytes.AddRange(BoundingBox.ToBytesHighRes());
            headerBytes.AddRange(BitConverter.GetBytes(BoundingSphereRadius));
            headerBytes.AddRange(CollisionBox.ToBytesHighRes());
            headerBytes.AddRange(BitConverter.GetBytes(CollisionSphereRadius));
            headerBytes.AddRange(CollisionTriangleIndices.GetHeaderBytes());
            headerBytes.AddRange(CollisionVertices.GetHeaderBytes());
            headerBytes.AddRange(CollisionFaceNormals.GetHeaderBytes());
            headerBytes.AddRange(Attachments.GetHeaderBytes());
            headerBytes.AddRange(AttachmentIndicesLookup.GetHeaderBytes());
            headerBytes.AddRange(Events.GetHeaderBytes());
            headerBytes.AddRange(Lights.GetHeaderBytes());
            headerBytes.AddRange(Cameras.GetHeaderBytes());
            headerBytes.AddRange(CamerasIndicesLookup.GetHeaderBytes());
            headerBytes.AddRange(RibbonEmitters.GetHeaderBytes());
            headerBytes.AddRange(ParticleEmitters.GetHeaderBytes());
            if (Flags.HasFlag(M2Flags.BlendModeOverrides))
                headerBytes.AddRange(SecondTextureMaterialOverrides.GetHeaderBytes());
            return headerBytes;
        }
    }
}

