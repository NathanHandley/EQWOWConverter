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
using EQWOWConverter.ModelObjects;
using EQWOWConverter.Objects;

namespace EQWOWConverter.WOWFiles
{
    internal class M2
    {
        private M2Skin Skin = new M2Skin();
        public List<byte> ModelBytes = new List<byte>();
        private M2Header Header = new M2Header();
        private string Name = string.Empty;

        public M2(ModelObject modelObject)
        {
            ModelBytes.Clear();
            List<byte> nonHeaderBytes = new List<byte>();
            int curOffset = Header.GetSize();

            // Name
            Name = modelObject.WOWModelObjectData.Name;
            List<byte> nameBytes = GenerateNameBlock(modelObject.WOWModelObjectData);
            Header.Name.Offset = Convert.ToUInt32(curOffset);
            Header.Name.Size = Convert.ToUInt32(nameBytes.Count);
            curOffset += nameBytes.Count;
            nonHeaderBytes.AddRange(nameBytes);

            // Global Loop Timestamps
            // None for now

            // Animation Sequences
            List<byte> animationSequencesBytes = GenerateAnimationSequencesBlock(modelObject.WOWModelObjectData);
            Header.AnimationSequences.Offset = Convert.ToUInt32(curOffset);
            Header.GlobalLoopTimestamps.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelAnimations.Count);
            curOffset += animationSequencesBytes.Count;
            nonHeaderBytes.AddRange(animationSequencesBytes);

            // Animation Sequence ID Lookup
            // none for now

            // Bones
            List<byte> bonesBytes = GenerateBonesBlock(modelObject.WOWModelObjectData);
            Header.Bones.Offset = Convert.ToUInt32(curOffset);
            Header.Bones.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelBones.Count);
            curOffset += bonesBytes.Count;
            nonHeaderBytes.AddRange(bonesBytes);

            // Key Bone ID Lookup
            List<byte> boneKeyLookupBytes = GenerateBoneKeyLookupBlock(modelObject.WOWModelObjectData);
            Header.BoneKeyLookup.Offset = Convert.ToUInt32(curOffset);
            Header.BoneKeyLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelBoneKeyLookups.Count);
            curOffset += boneKeyLookupBytes.Count;
            nonHeaderBytes.AddRange(boneKeyLookupBytes);

            // Verticies
            List<byte> verticiesBytes = GenerateVerticiesBlock(modelObject.WOWModelObjectData);
            Header.Vertices.Offset = Convert.ToUInt32(curOffset);
            Header.Vertices.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelAnimationVerticies.Count);
            curOffset += verticiesBytes.Count;
            nonHeaderBytes.AddRange(verticiesBytes);

            // Number of Skin Profiles
            Header.SkinProfileCount = 1; // Fix it to 1 for now

            // Color and Alpha Animation Definitions
            // none for now

            // Textures
            List<byte> textureBytes = GenerateTexturesBlock(modelObject.WOWModelObjectData);
            Header.Textures.Offset = Convert.ToUInt32(curOffset);
            Header.Textures.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextures.Count);
            curOffset += textureBytes.Count;
            nonHeaderBytes.AddRange(textureBytes);

            // Texture Transparencies (Weights)
            List<byte> textureTransparenciesBytes = GenerateTextureTransparencyWeightsBlock(modelObject.WOWModelObjectData);
            Header.TextureTransparencyWeights.Offset = Convert.ToUInt32(curOffset);
            Header.TextureTransparencyWeights.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextureTransparencies.Count);
            curOffset += textureTransparenciesBytes.Count;
            nonHeaderBytes.AddRange(textureTransparenciesBytes);

            // Texture Transforms
            // none for now

            // Replaceable Texture ID Lookup
            List<byte> replaceableTextureLookupBytes = GenerateReplaceableTextureLookupBlock(modelObject.WOWModelObjectData);
            Header.ReplaceableTextureLookup.Offset = Convert.ToUInt32(curOffset);
            Header.ReplaceableTextureLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelReplaceableTextureLookups.Count);
            curOffset += replaceableTextureLookupBytes.Count;
            nonHeaderBytes.AddRange(replaceableTextureLookupBytes);

            // Materials
            List<byte> materialBytes = GenerateMaterialsBlock(modelObject.WOWModelObjectData);
            Header.Materials.Offset = Convert.ToUInt32(curOffset);
            Header.Materials.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelMaterials.Count);
            curOffset += materialBytes.Count;
            nonHeaderBytes.AddRange(materialBytes);

            // Bone Lookup
            List<byte> boneLookupBytes = GenerateBoneLookupBlock(modelObject.WOWModelObjectData);
            Header.BoneLookup.Offset = Convert.ToUInt32(curOffset);
            Header.BoneLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelBoneLookups.Count);
            curOffset += boneLookupBytes.Count;
            nonHeaderBytes.AddRange(boneLookupBytes);

            // Texture Lookup
            List<byte> textureLookupBytes = GenerateTextureLookupBlock(modelObject.WOWModelObjectData);
            Header.TextureLookup.Offset = Convert.ToUInt32(curOffset);
            Header.TextureLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextureLookups.Count);
            curOffset += textureLookupBytes.Count;
            nonHeaderBytes.AddRange(textureLookupBytes);

            // Texture Mapping Lookup
            List<byte> textureMappingLookupBytes = GenerateTextureMappingLookupBlock(modelObject.WOWModelObjectData);
            Header.TextureMappingLookup.Offset = Convert.ToUInt32(curOffset);
            Header.TextureMappingLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextureMappingLookups.Count);
            curOffset += textureMappingLookupBytes.Count;
            nonHeaderBytes.AddRange(textureMappingLookupBytes);

            // Texture Transparency Lookup (Weights)
            List<byte> textureTransparencyLookupBytes = GenerateTextureTransparencyLookupBlock(modelObject.WOWModelObjectData);
            Header.TextureTransparencyLookup.Offset = Convert.ToUInt32(curOffset);
            Header.TextureTransparencyLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextureTransparencyWeightsLookups.Count);
            curOffset += textureTransparencyLookupBytes.Count;
            nonHeaderBytes.AddRange(textureTransparencyLookupBytes);

            // Texture Transformations Lookup
            List<byte> textureTransformationLookupBytes = GenerateTextureTransformsLookupBlock(modelObject.WOWModelObjectData);
            Header.TextureTransformsLookup.Offset = Convert.ToUInt32(curOffset);
            Header.TextureTransformsLookup.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelTextureTransformationsLookup.Count);
            curOffset += textureTransformationLookupBytes.Count;
            nonHeaderBytes.AddRange(textureTransformationLookupBytes);

            // Collision Triangle Incidies
            // none for now

            // Collision Verticies
            // none for now

            // Collision Face Normals
            // none for now

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
            List<byte> secondTextureMaterialOverrideBytes = GenerateSecondTextureMaterialOverridesBlock(modelObject.WOWModelObjectData);
            Header.SecondTextureMaterialOverrides.Offset = Convert.ToUInt32(curOffset);
            Header.SecondTextureMaterialOverrides.Size = Convert.ToUInt32(modelObject.WOWModelObjectData.ModelSecondTextureMaterialOverrides.Count);
            curOffset += textureTransformationLookupBytes.Count;
            nonHeaderBytes.AddRange(textureTransformationLookupBytes);

            ///////////////////////////////////////////////////////////////
            // Set header values
            // Flags (leave blank for now)
            // Bounding and Collision Properties
            Header.BoundingBox = modelObject.WOWModelObjectData.BoundingBox;
            Header.BoundingSphereRadius = modelObject.WOWModelObjectData.BoundingSphereRadius;
            Header.CollisionBox = modelObject.WOWModelObjectData.CollisionBoundingBox;
            Header.CollisionSphereRadius = modelObject.WOWModelObjectData.CollisionSphereRaidus;

            // Skin Profile Count (just one for now)
            Header.SkinProfileCount = 1;

            // Assemble the byte stream together, header first
            ModelBytes.AddRange(Header.ToBytes());
            ModelBytes.AddRange(nonHeaderBytes);
        }

        /// <summary>
        /// Name
        /// </summary>
        private List<byte> GenerateNameBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            blockBytes.AddRange(Encoding.ASCII.GetBytes(modelObject.Name + "\0"));
            return blockBytes;
        }

        /// <summary>
        /// Global Loop Timestamps
        /// </summary>
        private List<byte> GenerateGlobalLoopTimestampsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Animation Sequences
        /// </summary>
        private List<byte> GenerateAnimationSequencesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (ModelAnimation animation in modelObject.ModelAnimations)
                blockBytes.AddRange(animation.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Animation Sequences Lookup
        /// </summary>
        private List<byte> GenerateAnimationSequencesLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Bones
        /// </summary>
        private List<byte> GenerateBonesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ModelBone bone in modelObject.ModelBones)
                blockBytes.AddRange(bone.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Bone Key Lookup
        /// </summary>
        private List<byte> GenerateBoneKeyLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (UInt16 boneKeyLookup in modelObject.ModelBoneKeyLookups)
                blockBytes.AddRange(BitConverter.GetBytes(boneKeyLookup));
            return blockBytes;
        }

        /// <summary>
        /// Verticies
        /// </summary>
        private List<byte> GenerateVerticiesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ModelAnimationVertex vertex in modelObject.ModelAnimationVerticies)
                blockBytes.AddRange(vertex.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Colors
        /// </summary>
        private List<byte> GenerateColorsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Textures
        /// </summary>
        private List<byte> GenerateTexturesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ModelTexture texture in modelObject.ModelTextures)
                blockBytes.AddRange(texture.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Texture Transparency Weights
        /// </summary>
        private List<byte> GenerateTextureTransparencyWeightsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(ModelTextureTransparency textureTransparency in modelObject.ModelTextureTransparencies)
                blockBytes.AddRange(textureTransparency.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Texture Transforms
        /// </summary>
        private List<byte> GenerateTextureTransformsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Replaceable Texture Lookup
        /// </summary>
        private List<byte> GenerateReplaceableTextureLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(Int16 replaceableTextureLookup in modelObject.ModelReplaceableTextureLookups)
                blockBytes.AddRange(BitConverter.GetBytes(replaceableTextureLookup));
            return blockBytes;
        }

        /// <summary>
        /// Materials
        /// </summary>
        private List<byte> GenerateMaterialsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (ModelMaterial material in modelObject.ModelMaterials)
                blockBytes.AddRange(material.ToBytes());
            return blockBytes;
        }

        /// <summary>
        /// Bone Lookup
        /// </summary>
        private List<byte> GenerateBoneLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(Int16 boneLookup in modelObject.ModelBoneLookups)
                blockBytes.AddRange(BitConverter.GetBytes(boneLookup));
            return blockBytes;
        }

        /// <summary>
        /// Texture Lookup
        /// </summary>
        private List<byte> GenerateTextureLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (Int16 textureLookup in modelObject.ModelTextureLookups)
                blockBytes.AddRange(BitConverter.GetBytes(textureLookup));
            return blockBytes;
        }

        /// <summary>
        /// Texture Mapping Lookup
        /// </summary>
        private List<byte> GenerateTextureMappingLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (Int16 textureMappingLookup in modelObject.ModelTextureMappingLookups)
                blockBytes.AddRange(BitConverter.GetBytes(textureMappingLookup));
            return blockBytes;
        }

        /// <summary>
        /// Texture Transparency Lookup (Weights)
        /// </summary>
        private List<byte> GenerateTextureTransparencyLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (Int16 textureTransLookup in modelObject.ModelTextureTransparencyWeightsLookups)
                blockBytes.AddRange(BitConverter.GetBytes(textureTransLookup));
            return blockBytes;
        }

        /// <summary>
        /// Texture Transforms Lookup
        /// </summary>
        private List<byte> GenerateTextureTransformsLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach (Int16 textureTransformationLookup in modelObject.ModelTextureTransformationsLookup)
                blockBytes.AddRange(BitConverter.GetBytes(textureTransformationLookup));
            return blockBytes;
        }

        /// <summary>
        /// Collision Triangle Indicies
        /// </summary>
        private List<byte> GenerateCollisionTriangleIncidiesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Collision Verticies
        /// </summary>
        private List<byte> GenerateCollisionVerticiesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Collision Face Normals
        /// </summary>
        private List<byte> GenerateCollisionFaceNormalsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Attachments
        /// </summary>
        private List<byte> GenerateAttachmentsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Attachment Incidies Lookup
        /// </summary>
        private List<byte> GenerateAttachmentIncidiesLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Events
        /// </summary>
        private List<byte> GenerateEventsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Lights
        /// </summary>
        private List<byte> GenerateLightsBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Cameras
        /// </summary>
        private List<byte> GenerateCamerasBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Cameras Indicies Lookup
        /// </summary>
        private List<byte> GenerateCamerasIndiciesLookupBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Ribbon Emitters
        /// </summary>
        private List<byte> GenerateRibbonEmittersBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// Particle Emitters
        /// </summary>
        private List<byte> GenerateParticleEmittersBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();

            return blockBytes;
        }

        /// <summary>
        /// SecondTextureMaterialOverrides
        /// </summary>
        private List<byte> GenerateSecondTextureMaterialOverridesBlock(WOWObjectModelData modelObject)
        {
            List<byte> blockBytes = new List<byte>();
            foreach(UInt16 overideSecondTexture in modelObject.ModelSecondTextureMaterialOverrides)
                blockBytes.AddRange(BitConverter.GetBytes(overideSecondTexture));
            return blockBytes;
        }

        public void WriteToDisk(string outputFolderPath)
        {
            // Make the directory
            if (Directory.Exists(outputFolderPath))
                Directory.Delete(outputFolderPath, true);
            FileTool.CreateBlankDirectory(outputFolderPath, true);

            // Create the M2
            string m2FileName = Path.Combine(outputFolderPath, Name + ".m2");
            File.WriteAllBytes(m2FileName, ModelBytes.ToArray());
        }
    }
}
