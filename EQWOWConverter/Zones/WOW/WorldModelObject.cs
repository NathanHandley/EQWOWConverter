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

using EQWOWConverter.Common;
using EQWOWConverter.Zones;
using EQWOWConverter.Zones.WOW;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WorldModelObject
    {
        private static UInt32 CURRENT_WMOGROUPID = Configuration.CONFIG_DBCID_WMOGROUPID_START;

        public UInt32 WMOGroupID;
        public bool IsLoaded = false;
        public WorldModelObjectType WMOType = WorldModelObjectType.Rendered;
        public List<Material> Materials = new List<Material>();
        public MeshData MeshData = new MeshData();
        public List<WorldModelRenderBatch> RenderBatches = new List<WorldModelRenderBatch>();
        public Dictionary<int, WorldModelObjectDoodadInstance> DoodadInstances = new Dictionary<int, WorldModelObjectDoodadInstance>();
        public BoundingBox BoundingBox = new BoundingBox();
        public BSPTree BSPTree = new BSPTree(new BoundingBox(), new List<UInt32>());

        public bool IsCompletelyInLiquid = false;
        public LiquidType LiquidType = LiquidType.None;
        public Material LiquidMaterial = new Material();
        public PlaneAxisAlignedXY LiquidPlane = new PlaneAxisAlignedXY();

        public WorldModelObject()
        {
            WMOGroupID = CURRENT_WMOGROUPID;
            CURRENT_WMOGROUPID++;
        }

        public void LoadAsLiquidVolume(LiquidType liquidType, BoundingBox boundingBox)
        {
            WMOType = WorldModelObjectType.LiquidVolume;
            BoundingBox = boundingBox;
            LiquidType = liquidType;
            BSPTree = new BSPTree(boundingBox, new List<UInt32>());
            IsLoaded = true;
        }

        public void LoadAsLiquidPlane(LiquidType liquidType, PlaneAxisAlignedXY liquidPlane, Material liquidMaterial, BoundingBox boundingBox)
        {
            WMOType = WorldModelObjectType.LiquidPlane;
            BoundingBox = boundingBox;
            LiquidType = liquidType;
            LiquidMaterial = liquidMaterial;
            LiquidPlane = liquidPlane;
            BSPTree = new BSPTree(boundingBox, new List<UInt32>());
            IsLoaded = true;
        }

        public void LoadAsRendered(MeshData meshData, List<Material> materials, List<WorldModelObjectDoodadInstance> zoneWideDoodadInstances,
            ZoneProperties zoneProperties)
        {
            WMOType = WorldModelObjectType.Rendered;
            MeshData = meshData;
            Materials = materials;
            BoundingBox = BoundingBox.GenerateBoxFromVectors(meshData.Vertices, Configuration.CONFIG_EQTOWOW_ADDED_BOUNDARY_AMOUNT);
            List<UInt32> collisionTriangleIndices;
            GenerateRenderBatches(materials, zoneProperties, out collisionTriangleIndices);
            BSPTree = new BSPTree(BoundingBox, collisionTriangleIndices);
            CreateZoneWideDoodadAssociations(zoneWideDoodadInstances);
            if (zoneProperties.IsCompletelyInLiquid)
            {
                IsCompletelyInLiquid = zoneProperties.IsCompletelyInLiquid;
                LiquidType = zoneProperties.CompletelyInLiquidType;
            }
            IsLoaded = true;
        }

        private void GenerateRenderBatches(List<Material> materials, ZoneProperties zoneProperties, out List<UInt32> collisionTriangleIncidies)
        {
            // Don't make a render batch if static rendering is disabled
            if (Configuration.CONFIG_EQTOWOW_ZONE_GENERATE_STATIC_GEOMETRY == false)
            {
                collisionTriangleIncidies = new List<UInt32>();
                return;
            }

            // Reorder the faces and related objects
            MeshData.SortDataByMaterial();

            // Build a render batch per material
            Dictionary<int, WorldModelRenderBatch> renderBatchesByMaterialID = new Dictionary<int, WorldModelRenderBatch>();
            for (int i = 0; i < MeshData.TriangleFaces.Count; i++)
            {
                // Work off material index
                int curMaterialIndex = MeshData.TriangleFaces[i].MaterialIndex;

                // Skip materials that shouldn't be rendered
                if (materials[curMaterialIndex].IsRenderable() == false)
                    continue;
                if (materials[curMaterialIndex].IsAnimated() == true)
                    continue;
                if (materials[curMaterialIndex].HasTransparency() == true)
                    continue;

                // Create a new one if this is the first instance of the material
                if (renderBatchesByMaterialID.ContainsKey(curMaterialIndex) == false)
                {
                    renderBatchesByMaterialID.Add(curMaterialIndex, new WorldModelRenderBatch());
                    renderBatchesByMaterialID[curMaterialIndex].MaterialIndex = Convert.ToByte(curMaterialIndex);
                    renderBatchesByMaterialID[curMaterialIndex].FirstTriangleFaceIndex = Convert.ToUInt32(i * 3);
                    renderBatchesByMaterialID[curMaterialIndex].NumOfFaceIndices = 3;
                    renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex = MeshData.TriangleFaces[i].GetSmallestIndex();
                    renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex = MeshData.TriangleFaces[i].GetLargestIndex();
                }
                // Otherwise add to an existing
                else
                {
                    renderBatchesByMaterialID[curMaterialIndex].NumOfFaceIndices += 3;
                    int curFaceMinIndex = MeshData.TriangleFaces[i].GetSmallestIndex();
                    if (curFaceMinIndex < renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex)
                        renderBatchesByMaterialID[curMaterialIndex].FirstVertexIndex = Convert.ToUInt16(curFaceMinIndex);
                    int curFaceMaxIndex = MeshData.TriangleFaces[i].GetLargestIndex();
                    if (curFaceMaxIndex > renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex)
                        renderBatchesByMaterialID[curMaterialIndex].LastVertexIndex = Convert.ToUInt16(curFaceMaxIndex);
                }
            }

            // Construct the collision triangle indices
            collisionTriangleIncidies = new List<UInt32>();
            for (int i = 0; i < MeshData.TriangleFaces.Count; ++i)
            {
                Material curMaterial = materials[MeshData.TriangleFaces[i].MaterialIndex];
                if (zoneProperties.NonCollisionMaterialNames.Contains(curMaterial.UniqueName) == false)
                {
                    collisionTriangleIncidies.Add(Convert.ToUInt32(i));
                }
            }

            // Store the new render batches
            RenderBatches.Clear();
            foreach (var renderBatch in renderBatchesByMaterialID)
            {
                renderBatch.Value.BoundingBox = new BoundingBox(BoundingBox);
                RenderBatches.Add(renderBatch.Value);
            }
        }

        private void CreateZoneWideDoodadAssociations(List<WorldModelObjectDoodadInstance> zoneWidedoodadInstances)
        {
            // All zonewide doodads should be associated with all WMOs
            for (int i = 0; i < zoneWidedoodadInstances.Count; i++)
            {
                WorldModelObjectDoodadInstance doodadInstance = zoneWidedoodadInstances[i];
                DoodadInstances.Add(i, doodadInstance);
            }
        }
    }
}
