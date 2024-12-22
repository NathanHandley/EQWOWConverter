using System.Text;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ActorWriterNew : TextAssetWriter
    {
        public override void AddFragmentData(WldFragment data)
        {
            Actor actor = data as Actor;

            if (actor == null)
            {
                return;
            }

            Export.Append(actor.ActorType.ToString());
            Export.Append(",");
            Export.Append(actor.ReferenceName);
            
            Export.Append(FragmentNameCleaner.CleanName(actor));
            Export.AppendLine();
        }

        public override void WriteAssetToFile(string fileName)
        {
            if (Export.Length == 0)
            {
                return;
            }
            
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.AppendLine(LanternStrings.ExportHeaderTitle + "Actor");
            Export.Insert(0, headerBuilder.ToString());
            base.WriteAssetToFile(fileName);
        }
    }
}