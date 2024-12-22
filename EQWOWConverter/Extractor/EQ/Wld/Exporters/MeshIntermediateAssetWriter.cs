using System;
using System.Collections.Generic;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class MeshIntermediateAssetWriter : TextAssetWriter
    {
        private bool _useGroups;
        private bool _isCollisionMesh;
        private bool _isFirstMesh = true;
        private int _currentBaseIndex;

        public override void ClearExportData()
        {
            base.ClearExportData();
            _isFirstMesh = true;
            _currentBaseIndex = 0;
        }

        public MeshIntermediateAssetWriter(bool useGroups, bool isCollisionMesh)
        {
            _useGroups = useGroups;
            _isCollisionMesh = isCollisionMesh;
        }

        public override void AddFragmentData(WldFragment data)
        {
            Mesh mesh = data as Mesh;

            if (mesh == null)
            {
                return;
            }

            HashSet<int> usedVertices = new HashSet<int>();
            List<Polygon> newIndices = new List<Polygon>();

            int currentPolygon = 0;

            foreach (RenderGroup group in mesh.MaterialGroups)
            {
                for (int i = 0; i < group.PolygonCount; ++i)
                {
                    Polygon polygon = mesh.Indices[currentPolygon];

                    newIndices.Add(polygon.GetCopy());
                    currentPolygon++;

                    if (!polygon.IsSolid && _isCollisionMesh)
                    {
                        continue;
                    }

                    usedVertices.Add(polygon.Vertex1);
                    usedVertices.Add(polygon.Vertex2);
                    usedVertices.Add(polygon.Vertex3);
                }
            }

            // Get rid of this hack
            if (!_isCollisionMesh)
            {
                usedVertices.Clear();

                for (int i = 0; i < mesh.Vertices.Count; ++i)
                {
                    usedVertices.Add(i);
                }
            }

            int unusedVertices = 0;
            for (int i = mesh.Vertices.Count - 1; i >= 0; i--)
            {
                if (usedVertices.Contains(i))
                {
                    continue;
                }

                unusedVertices++;

                foreach (var polygon in newIndices)
                {
                    if (polygon.Vertex1 >= i && polygon.Vertex1 != 0)
                    {
                        polygon.Vertex1--;
                    }
                    if (polygon.Vertex2 >= i && polygon.Vertex2 != 0)
                    {
                        polygon.Vertex2--;
                    }
                    if (polygon.Vertex3 >= i && polygon.Vertex3 != 0)
                    {
                        polygon.Vertex3--;
                    }
                }
            }

            if (!_isCollisionMesh && (_isFirstMesh || _useGroups))
            {
                Export.Append("ml");
                Export.Append(",");
                Export.Append(FragmentNameCleaner.CleanName(mesh.MaterialList));
                Export.AppendLine();
                _isFirstMesh = false;
            }

            for (var i = 0; i < mesh.Vertices.Count; i++)
            {
                if (!usedVertices.Contains(i))
                {
                    continue;
                }

                var vertex = mesh.Vertices[i];
                Export.Append("v");
                Export.Append(",");
                Export.Append(vertex.x + mesh.Center.x);
                Export.Append(",");
                Export.Append(vertex.z + mesh.Center.z);
                Export.Append(",");
                Export.Append(vertex.y + mesh.Center.y);
                Export.AppendLine();
            }

            for (var i = 0; i < mesh.TextureUvCoordinates.Count; i++)
            {
                if (!usedVertices.Contains(i) || _isCollisionMesh)
                {
                    continue;
                }

                var textureUv = mesh.TextureUvCoordinates[i];
                Export.Append("uv");
                Export.Append(",");
                Export.Append(textureUv.x);
                Export.Append(",");
                Export.Append(textureUv.y);
                Export.AppendLine();
            }

            for (var i = 0; i < mesh.Normals.Count; i++)
            {
                if (!usedVertices.Contains(i) || _isCollisionMesh)
                {
                    continue;
                }

                var normal = mesh.Normals[i];
                Export.Append("n");
                Export.Append(",");
                Export.Append(normal.x);
                Export.Append(",");
                Export.Append(normal.y);
                Export.Append(",");
                Export.Append(normal.z);
                Export.AppendLine();
            }

            for (var i = 0; i < mesh.Colors.Count; i++)
            {
                if (!usedVertices.Contains(i) || _isCollisionMesh)
                {
                    continue;
                }

                var vertexColor = mesh.Colors[i];
                Export.Append("c");
                Export.Append(",");
                Export.Append(vertexColor.B);
                Export.Append(",");
                Export.Append(vertexColor.G);
                Export.Append(",");
                Export.Append(vertexColor.R);
                Export.Append(",");
                Export.Append(vertexColor.A);
                Export.AppendLine();
            }

            currentPolygon = 0;

            foreach (RenderGroup group in mesh.MaterialGroups)
            {
                /*if (!_isCollisionMesh)
                {
                    _export.Append("mg");
                    _export.Append(",");
                    _export.Append(group.MaterialIndex - mesh.StartTextureIndex);
                    _export.Append(",");
                    _export.Append(group.PolygonCount);
                    _export.AppendLine();
                }*/

                for (int i = 0; i < group.PolygonCount; ++i)
                {
                    Polygon polygon = newIndices[currentPolygon];

                    currentPolygon++;

                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(group.MaterialIndex);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex1);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex2);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex3);
                    Export.AppendLine();
                }
            }

            foreach (var bone in mesh.MobPieces)
            {
                Export.Append("b");
                Export.Append(",");
                Export.Append(bone.Key);
                Export.Append(",");
                Export.Append(bone.Value.Start);
                Export.Append(",");
                Export.Append(bone.Value.Count);
                Export.AppendLine();
            }

            var animatedVertices = mesh.AnimatedVerticesReference?.GetAnimatedVertices();
            if (animatedVertices != null && !_isCollisionMesh)
            {
                Export.Append("ad");
                Export.Append(",");
                Export.Append(animatedVertices.Delay);
                Export.AppendLine();

                for (var i = 0; i < animatedVertices.Frames.Count; i++)
                {
                    List<vec3> frame = animatedVertices.Frames[i];
                    foreach (vec3 position in frame)
                    {
                        Export.Append("av");
                        Export.Append(",");
                        Export.Append(i);
                        Export.Append(",");
                        Export.Append(position.x + mesh.Center.x);
                        Export.Append(",");
                        Export.Append(position.z + mesh.Center.z);
                        Export.Append(",");
                        Export.Append(position.y + mesh.Center.y);
                        Export.AppendLine();
                    }
                }
            }

            if (!_useGroups)
            {
                _currentBaseIndex += mesh.Vertices.Count - unusedVertices;
            }
        }

        public override void WriteAssetToFile(string fileName)
        {
            if (Export.Length == 0)
            {
                return;
            }

            Export.Insert(0, LanternStrings.ExportHeaderTitle + "Mesh Intermediate Format" + Environment.NewLine);

            base.WriteAssetToFile(fileName);
        }
    }
}
