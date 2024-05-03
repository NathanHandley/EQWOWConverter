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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WOWZoneData
    {
        public List<WorldModelObject> WorldObjects = new List<WorldModelObject>();
        public List<Material> Materials = new List<Material>();
        public ColorRGBA AmbientLight = new ColorRGBA();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public AxisAlignedBox BoundingBox = new AxisAlignedBox();
        public Fog FogSettings = new Fog();
        public UInt32 TextureCount = 0;

        public UInt32 WMOID { get; }

        public WOWZoneData(UInt32 wmoid)
        {
            // Save and gen WMOID
            WMOID = wmoid;
        }

        public void LoadFromEQZone(EQZoneData eqZoneData)
        {
            Materials = eqZoneData.Materials;
            AmbientLight = eqZoneData.AmbientLight;
            LightInstances = eqZoneData.LightInstances;
            
            // For now, only load into a single world object
            WorldObjects = new List<WorldModelObject>();
            
            // Store a new copy of the materials
            List<TriangleFace> newFaces = new List<TriangleFace>();
            foreach(TriangleFace eqFace in eqZoneData.TriangleFaces)
            {
                TriangleFace newFace = new TriangleFace();
                newFace.MaterialIndex = eqFace.MaterialIndex;

                // Rotate the verticies for culling differences
                newFace.V1 = eqFace.V3;
                newFace.V2 = eqFace.V2;
                newFace.V3 = eqFace.V1;               

                // Add it
                newFaces.Add(newFace);
            }
            // Sort by material ID for render batching reasons
            newFaces.Sort();

            // Texture count is calculated from the material list
            foreach (Material material in Materials)
            {
                if (material.AnimationTextures.Count > 0)
                    TextureCount++;
            }

            WorldModelObject curWorldModelObject = new WorldModelObject(eqZoneData.Verticies, eqZoneData.TextureCoords, eqZoneData.Normals,
                eqZoneData.VertexColors, newFaces, Materials);
            WorldObjects.Add(curWorldModelObject);

            // Rebuild the bounding box
            CalculateBoundingBox();
        }

        public void CalculateBoundingBox()
        {
            // Calculate it by using the bounding box of all WorldModelObjects
            BoundingBox = new AxisAlignedBox();
            foreach(WorldModelObject worldModelObject in WorldObjects)
            {
                if (worldModelObject.BoundingBox.TopCorner.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = worldModelObject.BoundingBox.TopCorner.X;
                if (worldModelObject.BoundingBox.TopCorner.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = worldModelObject.BoundingBox.TopCorner.Y;
                if (worldModelObject.BoundingBox.TopCorner.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = worldModelObject.BoundingBox.TopCorner.Z;

                if (worldModelObject.BoundingBox.BottomCorner.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = worldModelObject.BoundingBox.BottomCorner.X;
                if (worldModelObject.BoundingBox.BottomCorner.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = worldModelObject.BoundingBox.BottomCorner.Y;
                if (worldModelObject.BoundingBox.BottomCorner.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = worldModelObject.BoundingBox.BottomCorner.Z;
            }
        }

        private void GenerateTextureAlignedSubMeshes(string parentName)
        {
            //if (Materials.Count == 0)
            //{
            //    Logger.WriteLine("- [" + parentName + "]: Could not generate sub meshes since there are no materials");
            //    return;
            //}

            //// Perform unique copy of faces into sub objects
            //List<TriangleFace> facesToConsume = new List<TriangleFace>(TriangleFaces);
            //while (facesToConsume.Count > 0)
            //{
            //    List<int> facesToDelete = new List<int>();
            //    List<TriangleFace> newMeshTriangles = new List<TriangleFace>();

            //    // Iterate through and collect like faces
            //    int curMaterialIndex = -2;
            //    for (int i = 0; i < facesToConsume.Count; i++)
            //    {
            //        // If there is no assigned working material index, grab it
            //        if (curMaterialIndex == -2)
            //        {
            //            // If it's an invalid index, just delete it
            //            if (facesToConsume[i].MaterialIndex < 0)
            //            {
            //                facesToDelete.Add(i);
            //                break;
            //            }
            //            curMaterialIndex = facesToConsume[i].MaterialIndex;
            //        }

            //        // Capture like faces
            //        if (facesToConsume[i].MaterialIndex == curMaterialIndex)
            //        {
            //            // TODO: Add data to mesh
            //            newMeshTriangles.Add(facesToConsume[i]);
            //            curMaterialIndex = facesToConsume[i].MaterialIndex;
            //            facesToDelete.Add(i);
            //        }
            //    }

            //    // Remove the faces
            //    for (int j = facesToDelete.Count - 1; j >= 0; j--)
            //        facesToConsume.RemoveAt(j);

            //    // Save the mesh
            //    if (newMeshTriangles.Count > 0)
            //    {
            //        // When creating the new mesh, the indicies of the faces to include need to be remapped because
            //        // the related arrays will be subsets
            //        EQZoneData newZoneMesh = new EQZoneData();
            //        newZoneMesh.Materials = new List<Material>(Materials);
            //        Dictionary<int, int> copiedIndiciesAndNewValues = new Dictionary<int, int>();
            //        foreach(TriangleFace face in newMeshTriangles)
            //        {
            //            TriangleFace realignedFace = new TriangleFace();
            //            if (copiedIndiciesAndNewValues.ContainsKey(face.V1) == true)
            //            {
            //                realignedFace.V1 = copiedIndiciesAndNewValues[face.V1];
            //            }
            //            else
            //            {
            //                realignedFace.V1 = newZoneMesh.Verticies.Count;
            //                copiedIndiciesAndNewValues.Add(face.V1, realignedFace.V1);
            //                newZoneMesh.Verticies.Add(Verticies[face.V1]);
            //                newZoneMesh.Normals.Add(Normals[face.V1]);
            //                newZoneMesh.VertexColors.Add(VertexColors[face.V1]);
            //                newZoneMesh.TextureCoords.Add(TextureCoords[face.V1]);
            //            }

            //            if (copiedIndiciesAndNewValues.ContainsKey(face.V2) == true)
            //            {
            //                realignedFace.V2 = copiedIndiciesAndNewValues[face.V2];
            //            }
            //            else
            //            {
            //                realignedFace.V2 = newZoneMesh.Verticies.Count;
            //                copiedIndiciesAndNewValues.Add(face.V2, realignedFace.V2);
            //                newZoneMesh.Verticies.Add(Verticies[face.V2]);
            //                newZoneMesh.Normals.Add(Normals[face.V2]);
            //                newZoneMesh.VertexColors.Add(VertexColors[face.V2]);
            //                newZoneMesh.TextureCoords.Add(TextureCoords[face.V2]);
            //            }

            //            if (copiedIndiciesAndNewValues.ContainsKey(face.V3) == true)
            //            {
            //                realignedFace.V3 = copiedIndiciesAndNewValues[face.V3];
            //            }
            //            else
            //            {
            //                realignedFace.V3 = newZoneMesh.Verticies.Count;
            //                copiedIndiciesAndNewValues.Add(face.V3, realignedFace.V3);
            //                newZoneMesh.Verticies.Add(Verticies[face.V3]);
            //                newZoneMesh.Normals.Add(Normals[face.V3]);
            //                newZoneMesh.VertexColors.Add(VertexColors[face.V3]);
            //                newZoneMesh.TextureCoords.Add(TextureCoords[face.V3]);
            //            }

            //            realignedFace.MaterialIndex = face.MaterialIndex;
            //            newZoneMesh.TriangleFaces.Add(realignedFace);
            //        }
            //        newZoneMesh.CalculateBoundingBox();
            //        TextureAlignedSubMeshes.Add(newZoneMesh);
            //    }
            //    else
            //        Logger.WriteLine("-[" + parentName + "]: Error: In the loop to generate TextureAlignedSubMeshes, there were no verticies added but a mesh was added.");
            //}

            //if (TextureAlignedSubMeshes.Count >= 1000)
            //{
            //    Logger.WriteLine("-[" + parentName + "]: Error: More than 1000 sub meshes was generated, so WMO generation will fail...");
            //}
        }
    }
}
