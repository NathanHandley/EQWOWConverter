using System.Linq;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class SkeletonHierarchyWriter : TextAssetWriter
    {
        private bool _stripModelBase;

        public SkeletonHierarchyWriter(bool stripModelBase)
        {
            _stripModelBase = stripModelBase;
        }

        public override void AddFragmentData(WldFragment data)
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Skeleton Hierarchy");
            Export.AppendLine(LanternStrings.ExportHeaderFormat + "BoneName, Children, Mesh, AlternateMesh, ParticleCloud");

            SkeletonHierarchy skeleton = data as SkeletonHierarchy;

            if (skeleton == null)
            {
                return;
            }

            if (skeleton.Meshes != null && skeleton.Meshes.Count != 0)
            {
                Export.Append("meshes");
                foreach (var mesh in skeleton.Meshes)
                {
                    Export.Append(",");
                    Export.Append(FragmentNameCleaner.CleanName(mesh));
                }
                Export.AppendLine();

                Export.Append("secondary_meshes");
                foreach (var mesh in skeleton.SecondaryMeshes)
                {
                    Export.Append(",");
                    Export.Append(FragmentNameCleaner.CleanName(mesh));
                }

                Export.AppendLine();
            }

            if (skeleton.AlternateMeshes != null && skeleton.AlternateMeshes.Count != 0)
            {
                Export.Append("meshes");
                foreach (var mesh in skeleton.AlternateMeshes)
                {
                    Export.Append(",");
                    Export.Append(FragmentNameCleaner.CleanName(mesh));
                }
                Export.AppendLine();

                Export.Append("secondary_meshes");
                foreach (var mesh in skeleton.SecondaryAlternateMeshes)
                {
                    Export.Append(",");
                    Export.Append(FragmentNameCleaner.CleanName(mesh));
                }

                Export.AppendLine();
            }

            foreach (var node in skeleton.Skeleton)
            {
                string childrenList = string.Empty;

                foreach (var children in node.Children)
                {
                    childrenList += children;

                    if (children != node.Children.Last())
                    {
                        childrenList += ";";
                    }
                }

                var boneName = node.CleanedName;

                if (_stripModelBase)
                {
                    boneName = StripModelBase(boneName, skeleton.ModelBase);
                }

                Export.Append(CleanSkeletonNodeName(boneName));
                Export.Append(",");
                Export.Append(childrenList);

                Export.Append(",");

                if (node.MeshReference?.Mesh != null)
                {
                    Export.Append(FragmentNameCleaner.CleanName(node.MeshReference.Mesh));
                }

                Export.Append(",");

                if (node.MeshReference?.LegacyMesh != null)
                {
                    Export.Append(FragmentNameCleaner.CleanName(node.MeshReference.LegacyMesh));
                }

                Export.Append(",");

                if (node.ParticleCloud != null)
                {
                    Export.Append(FragmentNameCleaner.CleanName(node.ParticleCloud));
                }

                Export.AppendLine();
            }
        }

        private string CleanSkeletonNodeName(string name)
        {
            return name.Replace("_DAG", "").ToLower();
        }

        private string StripModelBase(string boneName, string modelBase)
        {
            if (boneName.StartsWith(modelBase))
            {
                boneName = boneName.Substring(modelBase.Length);
            }

            if (string.IsNullOrEmpty(boneName))
            {
                boneName = "root";
            }

            return boneName;
        }
    }
}
