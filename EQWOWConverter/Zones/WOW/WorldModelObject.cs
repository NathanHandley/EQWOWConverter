using EQWOWConverter.Common;
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
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();
        public AxisAlignedBox BoundingBox = new AxisAlignedBox();
        public AxisAlignedBoxLR BoundingBoxLowRes = new AxisAlignedBoxLR();

        public WorldModelObject(List<Vector3> verticies, List<TextureUv> textureCoords, List<Vector3> normals, List<ColorRGBA> vertexColors, List<TriangleFace> triangleFaces)
        {
            Verticies = verticies;
            TextureCoords = textureCoords;
            Normals = normals;
            VertexColors = vertexColors;
            TriangleFaces = triangleFaces;
            CalculateBoundingBoxes();
        }

        private void CalculateBoundingBoxes()
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
            BoundingBoxLowRes = new AxisAlignedBoxLR(BoundingBox);
        }
    }
}
