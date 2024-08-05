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
    internal class ZoneShadowBox
    {
        public Material SelectedMaterial;
        public MeshData MeshData = new MeshData();

        public ZoneShadowBox(BoundingBox boundingBox, List<Material> materials, string zoneShortName)
        {
            // Find a material that can be used by looking for something opaque
            Material? selectedMaterial = null;
            foreach (Material material in materials)
            {
                if (material.HasTransparency() == false && material.IsRenderable() == true && material.IsAnimated() == false)
                {
                    selectedMaterial = material;
                    break;
                }
            }
            if (selectedMaterial == null)
            {
                Logger.WriteError("Error, no suitable material found for ShadowBox for zone shortname '" + zoneShortName + "', shadowbox may not render properly");
                selectedMaterial = new Material();
            }
            SelectedMaterial = selectedMaterial;
            int materialIndex = Convert.ToInt32(SelectedMaterial.Index);

            // Generate an outward facing box, pushed away from the start
            MeshData = new MeshData();
            float highX = boundingBox.TopCorner.X + 50f;
            float lowX = boundingBox.BottomCorner.X - 50f;
            float highY = boundingBox.TopCorner.Y + 50f;
            float lowY = boundingBox.BottomCorner.Y - 50f;
            float highZ = boundingBox.TopCorner.Z + 50f;
            float lowZ = boundingBox.BottomCorner.Z;

            // Side 1
            int quadFaceStartVert = MeshData.Vertices.Count;
            MeshData.Vertices.Add(new Vector3(highX, lowY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, lowY, lowZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, lowY, lowZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, lowY, highZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));

            // Side 2
            quadFaceStartVert = MeshData.Vertices.Count;
            MeshData.Vertices.Add(new Vector3(highX, highY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, highY, highZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, highY, lowZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, highY, lowZ));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));

            // Side 3
            quadFaceStartVert = MeshData.Vertices.Count;
            MeshData.Vertices.Add(new Vector3(highX, highY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, highY, lowZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, lowY, lowZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, lowY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));

            // Side 4
            quadFaceStartVert = MeshData.Vertices.Count;
            MeshData.Vertices.Add(new Vector3(lowX, highY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, lowY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, lowY, lowZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, highY, lowZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));

            // Top
            quadFaceStartVert = MeshData.Vertices.Count;
            MeshData.Vertices.Add(new Vector3(highX, highY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(highX, lowY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, lowY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.Vertices.Add(new Vector3(lowX, highY, highZ));
            MeshData.Normals.Add(new Vector3(0, 0, 0));
            MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));

            // Bottom
            //quadFaceStartVert = MeshData.Vertices.Count;
            //MeshData.Vertices.Add(new Vector3(highX, highY, lowZ));
            //MeshData.Normals.Add(new Vector3(0, 0, 0));
            //MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 1));
            //MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            //MeshData.Vertices.Add(new Vector3(lowX, highY, lowZ));
            //MeshData.Normals.Add(new Vector3(0, 0, 0));
            //MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 1));
            //MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            //MeshData.Vertices.Add(new Vector3(lowX, lowY, lowZ));
            //MeshData.Normals.Add(new Vector3(0, 0, 0));
            //MeshData.TextureCoordinates.Add(new TextureCoordinates(0, 0));
            //MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            //MeshData.Vertices.Add(new Vector3(highX, lowY, lowZ));
            //MeshData.Normals.Add(new Vector3(0, 0, 0));
            //MeshData.TextureCoordinates.Add(new TextureCoordinates(1, 0));
            //MeshData.VertexColors.Add(new ColorRGBA(0, 0, 0));
            //MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert, quadFaceStartVert + 3));
            //MeshData.TriangleFaces.Add(new TriangleFace(materialIndex, quadFaceStartVert + 1, quadFaceStartVert + 3, quadFaceStartVert + 2));
        }
    }
}
