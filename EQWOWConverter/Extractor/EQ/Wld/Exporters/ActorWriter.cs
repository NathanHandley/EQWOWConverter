using System.Text;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ActorWriter : TextAssetWriter
    {
        private ActorType _actorType;
        private int _actorCount;
        public ActorWriter(ActorType actorType)
        {
            _actorType = actorType;
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            Actor actor = data as Actor;

            if (actor == null)
            {
                return;
            }
            
            if (actor.ActorType != _actorType)
            {
                return;
            }

            if (_actorType == ActorType.Skeletal)
            {
                Export.Append(FragmentNameCleaner.CleanName(actor));
                Export.Append(",");
                Export.Append(FragmentNameCleaner.CleanName(actor.SkeletonReference.SkeletonHierarchy));
                Export.AppendLine();
            }
            else if (_actorType == ActorType.Static)
            {
                Export.Append(FragmentNameCleaner.CleanName(actor));
                Export.Append(",");

                if (actor.MeshReference.Mesh != null)
                {
                    Export.Append(FragmentNameCleaner.CleanName(actor.MeshReference.Mesh));
                }
                else if (actor.MeshReference.LegacyMesh != null)
                {
                    Export.Append(FragmentNameCleaner.CleanName(actor.MeshReference.LegacyMesh));
                }
                
                Export.AppendLine();
            }
            else
            {
                Export.AppendLine(FragmentNameCleaner.CleanName(actor));
            }
            
            _actorCount++;
        }

        public override void WriteAssetToFile(string fileName)
        {
            if (Export.Length == 0)
            {
                return;
            }
            
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.AppendLine(LanternStrings.ExportHeaderTitle +
                                     "Actor List");
            headerBuilder.AppendLine("# Total models: " + _actorCount);
            Export.Insert(0, headerBuilder.ToString());
            
            base.WriteAssetToFile(fileName);
        }
    }
}