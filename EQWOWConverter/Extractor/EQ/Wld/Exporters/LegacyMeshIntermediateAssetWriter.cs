using System;
using GlmSharp;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    /// <summary>
    /// Exports an alternate mesh in the intermediate mesh format
    /// </summary>
    public class LegacyMeshIntermediateAssetWriter : TextAssetWriter
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

        public LegacyMeshIntermediateAssetWriter(bool useGroups, bool isCollisionMesh)
        {
            _useGroups = useGroups;
            _isCollisionMesh = isCollisionMesh;
        }

        public override void AddFragmentData(WldFragment data)
        {
            if (!(data is LegacyMesh lm))
            {
                return;
            }

            if (_isCollisionMesh && lm.PolyhedronReference != null)
            {
                var polyhedron = lm.PolyhedronReference.Polyhedron;

                // TODO: polyhedron scale factor
                foreach(var vertex in polyhedron.Vertices)
                {
                    Export.Append("v");
                    Export.Append(",");
                    Export.Append(vertex.x + lm.Center.x);
                    Export.Append(",");
                    Export.Append(vertex.z + lm.Center.z);
                    Export.Append(",");
                    Export.Append(vertex.y + lm.Center.y);
                    Export.AppendLine();
                }

                foreach(var polygon in polyhedron.Faces)
                {
                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(0);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex1);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex2);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex3);
                    Export.AppendLine();
                }

                return;
            }

            if (!_isCollisionMesh && (_isFirstMesh || _useGroups))
            {
                Export.Append("ml");
                Export.Append(",");
                Export.Append(FragmentNameCleaner.CleanName(lm.MaterialList));
                Export.AppendLine();
                _isFirstMesh = false;
            }

            foreach (var vertex in lm.Vertices)
            {
                Export.Append("v");
                Export.Append(",");
                Export.Append(vertex.x + lm.Center.x);
                Export.Append(",");
                Export.Append(vertex.z + lm.Center.z);
                Export.Append(",");
                Export.Append(vertex.y + lm.Center.y);
                Export.AppendLine();
            }

            foreach (var uv in lm.TexCoords)
            {
                Export.Append("uv");
                Export.Append(",");
                Export.Append(uv.x);
                Export.Append(",");
                Export.Append(uv.y);
                Export.AppendLine();
            }

            foreach (var normal in lm.Normals)
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

            int currentPolygon = 0;
            for (var i = 0; i < lm.RenderGroups.Count; i++)
            {
                var renderGroup = lm.RenderGroups[i];
                for (int j = 0; j < renderGroup.PolygonCount; ++j)
                {
                    Polygon polygon = lm.Polygons[currentPolygon];
                    currentPolygon++;

                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(renderGroup.MaterialIndex);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex1);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex2);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex3);
                    Export.AppendLine();
                }
            }

            if (lm.RenderGroups.Count == 0)
            {
                foreach (var polygon in lm.Polygons)
                {
                    Export.Append("i");
                    Export.Append(",");
                    Export.Append(polygon.MaterialIndex);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex1);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex2);
                    Export.Append(",");
                    Export.Append(_currentBaseIndex + polygon.Vertex3);
                    Export.AppendLine();
                }
            }

            foreach (var bone in lm.MobPieces)
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

            var animatedVertices = lm.AnimatedVerticesReference?.GetAnimatedVertices();
            if (animatedVertices != null && !_isCollisionMesh)
            {
                Export.Append("ad");
                Export.Append(",");
                Export.Append(animatedVertices.Delay);
                Export.AppendLine();

                for (var i = 0; i < animatedVertices.Frames.Count; i++)
                {
                    foreach (vec3 position in animatedVertices.Frames[i])
                    {
                        Export.Append("av");
                        Export.Append(",");
                        Export.Append(i);
                        Export.Append(",");
                        Export.Append(position.x + lm.Center.x);
                        Export.Append(",");
                        Export.Append(position.z + lm.Center.z);
                        Export.Append(",");
                        Export.Append(position.y + lm.Center.y);
                        Export.AppendLine();
                    }
                }
            }

            if (!_useGroups)
            {
                _currentBaseIndex += lm.Vertices.Count;
            }
        }

        public override void WriteAssetToFile(string fileName)
        {
            if (Export.Length == 0)
            {
                return;
            }

            Export.Insert(0, LanternStrings.ExportHeaderTitle + "Alternate Mesh Intermediate Format" + Environment.NewLine);

            base.WriteAssetToFile(fileName);
        }
    }
}
