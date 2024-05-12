using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class MapChunk
    {
        public int ID;
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureCoordinates> TextureCoords = new List<TextureCoordinates>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<TriangleFace> TriangleFaces = new List<TriangleFace>();

        public MapChunk(int iD)
        {
            ID = iD;
        }
    }
}
