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

using EQWOWConverter.WOWFiles;
using EQWOWConverter.Common;

namespace EQWOWConverter.Zones
{
    internal class ZoneModelObject
    {
        private static UInt32 CURRENT_WMOGROUPID = Configuration.DBCID_WMOAREATABLE_WMOGROUPID_START;
        private static readonly object WMOGroupIDLock = new object();

        public UInt32 WMOGroupID;
        public string DisplayName = string.Empty;
        public UInt32 AreaTableID = 0;
        public UInt16 GroupIndex;
        public bool IsLoaded = false;
        public ZoneObjectModelType WMOType = ZoneObjectModelType.Rendered;
        public List<Material> Materials = new List<Material>();
        public MeshData MeshData = new MeshData();
        public List<ZoneRenderBatch> RenderBatches = new List<ZoneRenderBatch>();
        public Dictionary<int, ZoneDoodadInstance> DoodadInstances = new Dictionary<int, ZoneDoodadInstance>();
        public BoundingBox BoundingBox = new BoundingBox();
        public BSPTree BSPTree = new BSPTree();
        public bool IsCompletelyInLiquid = false;
        public bool IsExterior = true;
        public ZoneLiquidType LiquidType = ZoneLiquidType.None;
        public ZoneLiquid? Liquid = null;
        public List<UInt16> LightInstanceIDs = new List<UInt16>();
        public ZoneAreaMusic? ZoneMusic = null;

        public ZoneModelObject(UInt16 groupIndex, UInt32 areaTableID)
        {
            lock (WMOGroupIDLock)
            {
                WMOGroupID = CURRENT_WMOGROUPID;
                CURRENT_WMOGROUPID++;
            }
            GroupIndex = groupIndex;
            AreaTableID = areaTableID;
        }

        public void LoadAsRendered(MeshData meshData, List<Material> materials)
        {
            WMOType = ZoneObjectModelType.Rendered;
            MeshData = meshData;
            Materials = materials;
            BoundingBox = BoundingBox.GenerateBoxFromVectors(meshData.Vertices, Configuration.GENERATE_ADDED_BOUNDARY_AMOUNT);
            GenerateRenderBatches(materials);
            IsLoaded = true;
        }

        public void LoadAsCollidableArea(MeshData collisionMeshData, BoundingBox boundingBox, string displayName, ZoneAreaMusic? zoneMusic, ZoneLiquid? liquid)
        {
            WMOType = ZoneObjectModelType.Collidable;
            DisplayName = displayName;
            MeshData = collisionMeshData;
            BoundingBox = boundingBox;
            List<UInt32> collisionTriangleIncidies = new List<UInt32>();
            for (UInt32 i = 0; i < MeshData.TriangleFaces.Count; ++i)
                collisionTriangleIncidies.Add(i);
            BSPTree = new BSPTree(collisionMeshData);
            ZoneMusic = zoneMusic;
            if (liquid != null)
            {
                Liquid = liquid;
                LiquidType = liquid.LiquidType;
                BoundingBox = liquid.BoundingBox;
            }
            IsLoaded = true;
        }

        public void LoadAsShadowBox(List<Material> materials, BoundingBox boundingBox, ZoneProperties zoneProperties)
        {
            WMOType = ZoneObjectModelType.ShadowBox;
            BoundingBox = boundingBox;
            Materials = materials;
            ZoneBox shadowBox = new ZoneBox(boundingBox, materials, zoneProperties.ShortName, Configuration.ZONE_SHADOW_BOX_ADDED_SIZE, MeshBoxRenderType.Outward);
            MeshData = shadowBox.MeshData;
            GenerateRenderBatches(materials);
            IsLoaded = true;
        }

        private void GenerateRenderBatches(List<Material> materials)
        {
            // Don't make a render batch if static rendering is disabled
            if (Configuration.ZONE_SHOW_STATIC_GEOMETRY == false)
                return;

            // Reorder the faces and related objects
            MeshData.SortDataByMaterialBonesAndGenerateRenderGroups();

            // Build a render batch per material
            Dictionary<int, ZoneRenderBatch> renderBatchesByMaterialID = new Dictionary<int, ZoneRenderBatch>();
            for (int i = 0; i < MeshData.TriangleFaces.Count; i++)
            {
                // Work off material index
                int curMaterialIndex = MeshData.TriangleFaces[i].MaterialIndex;

                // Skip materials that shouldn't be rendered
                if (materials[curMaterialIndex].IsAnimated() == true)
                    continue;
                if (materials[curMaterialIndex].HasTransparency() == true)
                    continue;
                if (materials[curMaterialIndex].IsRenderable() == false)
                    continue;

                // Create a new one if this is the first instance of the material
                if (renderBatchesByMaterialID.ContainsKey(curMaterialIndex) == false)
                {
                    renderBatchesByMaterialID.Add(curMaterialIndex, new ZoneRenderBatch());
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

            // Store the new render batches
            RenderBatches.Clear();
            foreach (var renderBatch in renderBatchesByMaterialID)
            {
                renderBatch.Value.BoundingBox = new BoundingBox(BoundingBox);
                RenderBatches.Add(renderBatch.Value);
            }
        }

        public UInt32 GenerateWMOHeaderFlags()
        {
            UInt32 headerFlags = Convert.ToUInt32(WMOGroupFlags.HasBSPTree);
            if (IsExterior == false)
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.IsIndoors);
            else
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.IsOutdoors);
            if (DoodadInstances.Count > 0 && Configuration.ZONE_MINIMAP_GENERATION_MODE_ENABLED == false)
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.HasDoodads);
            if (IsCompletelyInLiquid == false)
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.HasWater);
            if (LightInstanceIDs.Count > 0)
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.HasLights);
            if (WMOType == ZoneObjectModelType.Rendered && MeshData.VertexColors.Count > 0)
                headerFlags |= Convert.ToUInt32(WMOGroupFlags.HasVertexColors);
            return headerFlags;
        }
    }
}
