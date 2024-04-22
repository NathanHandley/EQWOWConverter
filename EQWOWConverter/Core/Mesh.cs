using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Core
{
    internal class Mesh
    {
        public List<Vector3> Verticies = new List<Vector3>();
        public List<TextureUv> TextureCoords = new List<TextureUv>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<ColorRGBA> VertexColors = new List<ColorRGBA>();
        public List<PolyIndex> Indicies = new List<PolyIndex>();
    }
}
