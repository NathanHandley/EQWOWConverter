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
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class WorldModelObject
    {
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureUv> TextureCoords = new List<TextureUv>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFacesSortedByMaterial = new List<TriangleFace>();
        public List<WorldModelRenderBatch> RenderBatches = new List<WorldModelRenderBatch>();
        public AxisAlignedBox BoundingBox = new AxisAlignedBox();

        public WorldModelObject(List<Vector3> verticies, List<TextureUv> textureCoords, List<Vector3> normals, List<ColorRGBA> vertexColors, 
            List<TriangleFace> triangleFacesSortedByMaterial, List<Material> materials)
        {
            Verticies = verticies;
            TextureCoords = textureCoords;
            Normals = normals;
            VertexColors = vertexColors;
            TriangleFacesSortedByMaterial = triangleFacesSortedByMaterial;
            CalculateBoundingBox();
            GenerateRenderBatches(materials);
        }

        private void GenerateRenderBatches(List<Material> materials)
        {
            RenderBatches = new List<WorldModelRenderBatch>();
            RenderBatches.Add(new WorldModelRenderBatch(0, 0, Convert.ToUInt16(TriangleFacesSortedByMaterial.Count * 3), 0, Convert.ToUInt16(Verticies.Count-1), Verticies));
            /*
            // Build a render batch per material, using the sorted triangle face array as the base
            int curMaterialIndex = -2;
            UInt32 curFirstTriangleFaceIndex = 0;
            UInt16 curNumOfTriangleFaces = 0;
            UInt16 curFirstVertexIndex = 0;
            UInt16 curLastVertexIndex = 0;
            List<Vector3> curVerticies = new List<Vector3>();
            for (int faceIndex = 0;  faceIndex < TriangleFacesSortedByMaterial.Count; faceIndex++)
            {
                // If it's a new material index, work on a new batch
                if (TriangleFacesSortedByMaterial[faceIndex].MaterialIndex != curMaterialIndex)
                {
                    // Only save the batch if it wasn't the first
                    if (curMaterialIndex != -2)
                    {
                        // Only make render batches for visable material types
                        if (materials[curMaterialIndex].MaterialType == MaterialType.Diffuse)
                        {
                            WorldModelRenderBatch newRenderBatch = new WorldModelRenderBatch(Convert.ToByte(curMaterialIndex), curFirstTriangleFaceIndex, curNumOfTriangleFaces,
                            curFirstVertexIndex, curLastVertexIndex, curVerticies);
                            RenderBatches.Add(newRenderBatch);
                        }
                        else
                        {
                            Logger.WriteLine("Skipped generating rendering batch since material " + curMaterialIndex.ToString() + " type was " + materials[curMaterialIndex].MaterialType.ToString());
                        }
                    }                    
                    curMaterialIndex = TriangleFacesSortedByMaterial[faceIndex].MaterialIndex;
                    curFirstTriangleFaceIndex = Convert.ToUInt32(faceIndex);
                    curNumOfTriangleFaces = 1;
                    curFirstVertexIndex = TriangleFacesSortedByMaterial[faceIndex].GetSmallestIndex();
                    curLastVertexIndex = TriangleFacesSortedByMaterial[faceIndex].GetLargestIndex();
                    curVerticies = new List<Vector3>
                    {
                        Verticies[TriangleFacesSortedByMaterial[faceIndex].V1],
                        Verticies[TriangleFacesSortedByMaterial[faceIndex].V2],
                        Verticies[TriangleFacesSortedByMaterial[faceIndex].V3]
                    };
                }
                // This is a continuation...
                else
                {
                    curNumOfTriangleFaces++;
                    if (TriangleFacesSortedByMaterial[faceIndex].GetSmallestIndex() < curFirstVertexIndex)
                        curFirstVertexIndex = TriangleFacesSortedByMaterial[faceIndex].GetSmallestIndex();
                    if (TriangleFacesSortedByMaterial[faceIndex].GetLargestIndex() > curLastVertexIndex)
                        curLastVertexIndex = TriangleFacesSortedByMaterial[faceIndex].GetLargestIndex();
                    curVerticies.Add(Verticies[TriangleFacesSortedByMaterial[faceIndex].V1]);
                    curVerticies.Add(Verticies[TriangleFacesSortedByMaterial[faceIndex].V2]);
                    curVerticies.Add(Verticies[TriangleFacesSortedByMaterial[faceIndex].V3]);
                }
            }

            // Save the last as long as it was more than zero faces
            if (curMaterialIndex != -2)
            {
                // Only make render batches for visable material types
                if (materials[curMaterialIndex].MaterialType == MaterialType.Diffuse)
                {
                    WorldModelRenderBatch newRenderBatch = new WorldModelRenderBatch(Convert.ToByte(curMaterialIndex), curFirstTriangleFaceIndex, Convert.ToUInt16(curNumOfTriangleFaces * 3),
                    curFirstVertexIndex, curLastVertexIndex, curVerticies);
                    RenderBatches.Add(newRenderBatch);
                }
                else
                {
                    Logger.WriteLine("Skipped generating rendering batch since material " + curMaterialIndex.ToString() + " type was " + materials[curMaterialIndex].MaterialType.ToString());
                }
            }
            */
        }

        private void CalculateBoundingBox()
        {
            BoundingBox = new AxisAlignedBox();
            foreach (Vector3 renderVert in Verticies)
            {
                if (renderVert.X < BoundingBox.BottomCorner.X)
                    BoundingBox.BottomCorner.X = renderVert.X;
                if (renderVert.Y < BoundingBox.BottomCorner.Y)
                    BoundingBox.BottomCorner.Y = renderVert.Y;
                if (renderVert.Z < BoundingBox.BottomCorner.Z)
                    BoundingBox.BottomCorner.Z = renderVert.Z;

                if (renderVert.X > BoundingBox.TopCorner.X)
                    BoundingBox.TopCorner.X = renderVert.X;
                if (renderVert.Y > BoundingBox.TopCorner.Y)
                    BoundingBox.TopCorner.Y = renderVert.Y;
                if (renderVert.Z > BoundingBox.TopCorner.Z)
                    BoundingBox.TopCorner.Z = renderVert.Z;
            }
        }
    }
}
