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
            if (!(data is Mesh mesh))
            {
                return;
            }

            if (_isCollisionMesh)
            {
                WriteCollisionMeshData(mesh);
            }
            else
            {
                WriteMeshData(mesh);
            }
        }

        private void WriteMeshData(Mesh mesh)
        {
            if (_isFirstMesh || _useGroups)
            {
                Export.Append("ml");
                Export.Append(",");
                Export.Append(FragmentNameCleaner.CleanName(mesh.MaterialList));
                Export.AppendLine();
                _isFirstMesh = false;
            }

            var centerX = mesh.Center.x;
            var centerY = mesh.Center.y;
            var centerZ = mesh.Center.z;
            foreach (var vertex in mesh.Vertices)
            {
                Export.Append("v");
                Export.Append(",");
                Export.Append(vertex.x + centerX);
                Export.Append(",");
                Export.Append(vertex.z + centerZ);
                Export.Append(",");
                Export.Append(vertex.y + centerY);
                Export.AppendLine();
            }

            foreach (var uv in mesh.Uvs)
            {
                Export.Append("uv");
                Export.Append(",");
                Export.Append(uv.x);
                Export.Append(",");
                Export.Append(uv.y);
                Export.AppendLine();
            }

            foreach (var normal in mesh.Normals)
            {
                Export.Append("n");
                Export.Append(",");
                Export.Append(normal.x);
                Export.Append(",");
                Export.Append(normal.y);
                Export.Append(",");
                Export.Append(normal.z);
                Export.AppendLine();
            }

            foreach (var vertexColor in mesh.Colors)
            {
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

            int currentPolygon = 0;
            foreach (MaterialGroup group in mesh.MaterialGroups)
            {
                for (int i = 0; i < group.TriangleCount; ++i)
                {
                    Triangle triangle = mesh.Triangles[currentPolygon];
                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(group.MaterialIndex);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + triangle.Index1);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + triangle.Index2);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + triangle.Index3);
                    Export.AppendLine();
                    currentPolygon++;
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
            if (animatedVertices != null)
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
                _currentBaseIndex += mesh.Vertices.Count;
            }
        }

        private void WriteCollisionMeshData(Mesh mesh)
        {
            HashSet<int> usedVertexIndices = new HashSet<int>();
            int currentPolygon = 0;

            foreach (MaterialGroup mg in mesh.MaterialGroups)
            {
                for (int i = 0; i < mg.TriangleCount; ++i)
                {
                    Triangle triangle = mesh.Triangles[currentPolygon];
                    currentPolygon++;

                    if (!triangle.IsSolid)
                    {
                        continue;
                    }

                    usedVertexIndices.Add(triangle.Index1);
                    usedVertexIndices.Add(triangle.Index2);
                    usedVertexIndices.Add(triangle.Index3);
                }
            }

            var oldToNewIndexMap = new Dictionary<int, int>();
            int newIndex = 0;
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (usedVertexIndices.Contains(i))
                {
                    oldToNewIndexMap[i] = newIndex++;
                }
            }

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (!usedVertexIndices.Contains(i))
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

            currentPolygon = 0;
            foreach (MaterialGroup group in mesh.MaterialGroups)
            {
                for (int i = 0; i < group.TriangleCount; ++i)
                {
                    Triangle triangle = mesh.Triangles[currentPolygon];
                    currentPolygon++;

                    if (!triangle.IsSolid)
                    {
                        continue;
                    }

                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(group.MaterialIndex);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + oldToNewIndexMap[triangle.Index1]);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + oldToNewIndexMap[triangle.Index2]);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + oldToNewIndexMap[triangle.Index3]);
                    Export.AppendLine();
                }
            }

            _currentBaseIndex += newIndex;
        }

        public override void WriteAssetToFile(string fileName)
        {
            if (Export.Length == 0 && !_isCollisionMesh)
            {
                return;
            }

            Export.Insert(0, LanternStrings.ExportHeaderTitle + "Mesh Intermediate Format" + Environment.NewLine);

            base.WriteAssetToFile(fileName);
        }
    }
}
