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
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using EQWOWConverter.Common;
using EQWOWConverter.ModelObjects;
using EQWOWConverter.Objects;
using EQWOWConverter.Zones;

namespace EQWOWConverter.WOWFiles
{
    internal class M2
    {
        private string TokenMagic = "MD20";
        private UInt32 Version = 264;
        private M2SerializableString Name;
        private M2Flags Flags = 0; // UInt32
        private M2GenericArray<M2Timestamps> GlobalLoopTimestamps = new M2GenericArray<M2Timestamps>();
        private M2GenericArray<ModelAnimation> AnimationSequences = new M2GenericArray<ModelAnimation>();
        private M2GenericArray<M2Int16> AnimationSequenceLookup = new M2GenericArray<M2Int16>();
        private M2BoneArray Bones = new M2BoneArray();
        private M2GenericArray<M2Int16> BoneKeyLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<ModelVertex> Vertices = new M2GenericArray<ModelVertex>();
        private UInt32 SkinProfileCount = 0;
        private M2GenericArray<M2Color> Colors = new M2GenericArray<M2Color>();
        private M2TextureArray Textures;
        private M2TrackSequencesArray<Fixed16> TextureTransparencyWeights = new M2TrackSequencesArray<Fixed16>();
        private M2GenericArray<ModelTextureTransform> TextureTransforms = new M2GenericArray<ModelTextureTransform>();
        private M2GenericArray<M2Int16> ReplaceableTextureLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<ModelMaterial> Materials = new M2GenericArray<ModelMaterial>();
        private M2GenericArray<M2Int16> BoneLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Int16> TextureLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Int16> TextureMappingLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Int16> TextureTransparencyLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Int16> TextureTransformsLookup = new M2GenericArray<M2Int16>();
        private BoundingBox BoundingBox = new BoundingBox();
        private float BoundingSphereRadius = 0f;
        private BoundingBox CollisionBox = new BoundingBox();
        private float CollisionSphereRadius = 0f;
        private M2GenericArray<TriangleFace> CollisionTriangleIndicies = new M2GenericArray<TriangleFace>();
        private M2GenericArray<Vector3> CollisionVerticies = new M2GenericArray<Vector3>();
        private M2GenericArray<Vector3> CollisionFaceNormals = new M2GenericArray<Vector3>();
        private M2GenericArray<M2Attachment> Attachments = new M2GenericArray<M2Attachment>();
        private M2GenericArray<M2Int16> AttachmentIndiciesLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Dummy> Events = new M2GenericArray<M2Dummy>();
        private M2GenericArray<M2Dummy> Lights = new M2GenericArray<M2Dummy>();
        private M2GenericArray<M2Dummy> Cameras = new M2GenericArray<M2Dummy>();
        private M2GenericArray<M2Int16> CamerasIndiciesLookup = new M2GenericArray<M2Int16>();
        private M2GenericArray<M2Dummy> RibbonEmitters = new M2GenericArray<M2Dummy>();
        private M2GenericArray<M2Dummy> ParticleEmitters = new M2GenericArray<M2Dummy>();
        private M2GenericArray<M2Dummy> SecondTextureMaterialOverrides = new M2GenericArray<M2Dummy>(); // Multitexturing will use second material from here for blending with first

        public M2Skin Skin;
        private List<byte> FileBytes = new List<byte>();

        public M2(ModelObject modelObject, string mpqObjectFolder)
        {
            // Populate the M2 Data objects
            PopulateElements(modelObject, mpqObjectFolder);
            Skin = new M2Skin(modelObject);
        }

        private void PopulateElements(ModelObject modelObject, string mpqObjectFolder)
        {
            WOWObjectModelData wowModelObject = modelObject.WOWModelObjectData;

            // Name
            Name = new M2SerializableString(wowModelObject.Name);

            // Global Loop Timestamps
            // None for now

            // Animation Sequences
            AnimationSequences.AddArray(wowModelObject.ModelAnimations);

            // Animation Sequence ID Lookup
            foreach (Int16 value in wowModelObject.AnimationSequenceIDLookups)
                AnimationSequenceLookup.Add(new M2Int16(value));

            // Bones
            Bones.AddModelBones(wowModelObject.ModelBones);

            // Key Bone ID Lookup
            foreach (Int16 value in wowModelObject.ModelBoneKeyLookups)
                BoneKeyLookup.Add(new M2Int16(value));

            // Verticies
            Vertices.AddArray(wowModelObject.ModelVerticies);

            // Number of Skin Profiles
            SkinProfileCount = 1;  // Fix to 1 for now

            // Color and Alpha Animation Definitions
            // none for now

            // Textures
            Textures = new M2TextureArray(mpqObjectFolder);
            Textures.AddModelTextures(wowModelObject.ModelTextures);

            // Texture Transparencies (Weights, just 1 for now)
            TextureTransparencyWeights.AddModelTrackSequences(wowModelObject.ModelTextureTransparencies);

            // Texture Transforms
            // none for now

            // Replaceable Texture ID Lookup
            foreach (Int16 value in wowModelObject.ModelReplaceableTextureLookups)
                ReplaceableTextureLookup.Add(new M2Int16(value));

            // Materials
            Materials.AddArray(wowModelObject.ModelMaterials);

            // Bone Lookup
            foreach (Int16 value in wowModelObject.ModelBoneLookups)
                BoneLookup.Add(new M2Int16(value));

            // Texture Lookup
            foreach (Int16 value in wowModelObject.ModelTextureLookups)
                TextureLookup.Add(new M2Int16(value));

            // Texture Mapping Lookup
            foreach (Int16 value in wowModelObject.ModelTextureMappingLookups)
                TextureMappingLookup.Add(new M2Int16(value));

            // Texture Transparency Lookup (Weights)
            foreach (Int16 value in wowModelObject.ModelTextureTransparencyWeightsLookups)
                TextureTransparencyLookup.Add(new M2Int16(value));

            // Texture Transformations Lookup
            foreach (Int16 value in wowModelObject.ModelTextureTransformationsLookup)
                TextureTransformsLookup.Add(new M2Int16(value));

            // Bounding Box
            BoundingBox = wowModelObject.BoundingBox;

            // Bounding Sphere Radius
            BoundingSphereRadius = wowModelObject.BoundingSphereRadius;

            // Collision Box
            CollisionBox = wowModelObject.CollisionBoundingBox;

            // Collision Sphere Raidus
            CollisionSphereRadius = wowModelObject.CollisionSphereRaidus;

            // Collision Triangle Incidies
            CollisionTriangleIndicies.AddArray(wowModelObject.CollisionTriangles);

            // Collision Verticies
            CollisionVerticies.AddArray(wowModelObject.CollisionPositions);

            // Collision Face Normals
            CollisionFaceNormals.AddArray(wowModelObject.CollisionFaceNormals);

            // Attachments
            // none for now

            // Attachment ID Lookup
            // none for now

            // Events
            // none for now

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
            headerSize += 8;  // CollisionTriangleIndicies
            headerSize += 8;  // CollisionVerticies
            headerSize += 8;  // CollisionFaceNormals
            headerSize += 8;  // Attachments
            headerSize += 8;  // AttachmentIndiciesLookup
            headerSize += 8;  // Events
            headerSize += 8;  // Lights
            headerSize += 8;  // Cameras
            headerSize += 8;  // CamerasIndiciesLookup
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
            File.WriteAllBytes(m2FileName, FileBytes.ToArray());

            // Create the skin
            Skin.WriteToDisk(outputFolderPath);
        }
    }
}
/*

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
    headerBytes.AddRange(TextureTransparencyWeights.GetHeaderBytes());
    headerBytes.AddRange(TextureTransforms.GetHeaderBytes());
    headerBytes.AddRange(ReplaceableTextureLookup.GetHeaderBytes());
    headerBytes.AddRange(Materials.GetHeaderBytes());
    headerBytes.AddRange(BoneLookup.GetHeaderBytes());
    headerBytes.AddRange(TextureLookup.GetHeaderBytes());
    headerBytes.AddRange(TextureMappingLookup.GetHeaderBytes());
    headerBytes.AddRange(TextureTransparencyLookup.GetHeaderBytes());
    headerBytes.AddRange(TextureTransformsLookup.GetHeaderBytes());
    headerBytes.AddRange(BoundingBox.ToBytesHighRes());
    headerBytes.AddRange(BitConverter.GetBytes(BoundingSphereRadius));
    headerBytes.AddRange(CollisionBox.ToBytesHighRes());
    headerBytes.AddRange(BitConverter.GetBytes(CollisionSphereRadius));
    headerBytes.AddRange(CollisionTriangleIndicies.GetHeaderBytes());
    headerBytes.AddRange(CollisionVerticies.GetHeaderBytes());
    headerBytes.AddRange(CollisionFaceNormals.GetHeaderBytes());
    headerBytes.AddRange(Attachments.GetHeaderBytes());
    headerBytes.AddRange(AttachmentIndiciesLookup.GetHeaderBytes());
    headerBytes.AddRange(Events.GetHeaderBytes());
    headerBytes.AddRange(Lights.GetHeaderBytes());
    headerBytes.AddRange(Cameras.GetHeaderBytes());
    headerBytes.AddRange(CamerasIndiciesLookup.GetHeaderBytes());
    headerBytes.AddRange(RibbonEmitters.GetHeaderBytes());
    headerBytes.AddRange(ParticleEmitters.GetHeaderBytes());
    if (Flags.HasFlag(M2Flags.BlendModeOverrides))
        headerBytes.AddRange(SecondTextureMaterialOverrides.GetHeaderBytes());
    return headerBytes;
}

public List<Byte> GetDataBytes(UInt32 dataStartOffset)
{
    List<byte> dataBytes = new List<byte>();
    Name.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    GlobalLoopTimestamps.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    AnimationSequences.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    AnimationSequenceLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Bones.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    BoneKeyLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Vertices.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Colors.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Textures.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureTransparencyWeights.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureTransforms.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    ReplaceableTextureLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Materials.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    BoneLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureMappingLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureTransparencyLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    TextureTransformsLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    CollisionTriangleIndicies.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    CollisionVerticies.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    CollisionFaceNormals.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Attachments.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    AttachmentIndiciesLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Events.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Lights.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    Cameras.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    CamerasIndiciesLookup.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    RibbonEmitters.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    ParticleEmitters.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    if (Flags.HasFlag(M2Flags.BlendModeOverrides))
        SecondTextureMaterialOverrides.AddDataToByteBufferAndUpdateHeader(ref dataStartOffset, ref dataBytes);
    return dataBytes;
}
*/

