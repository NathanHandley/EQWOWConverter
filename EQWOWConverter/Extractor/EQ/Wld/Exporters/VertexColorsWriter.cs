using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class VertexColorsWriter : TextAssetWriter
    {
        public VertexColorsWriter()
        {
            AddHeader();
        }

        private void AddHeader()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Vertex Colors");
            Export.AppendLine(LanternStrings.ExportHeaderFormat +
                               "Red, Green, Blue, Sunlight");
        }

        public override void AddFragmentData(WldFragment data)
        {
            VertexColors instance = data as VertexColors;

            if (instance == null)
            {
                return;
            }

            foreach (Color color in instance.Colors)
            {
                Export.Append(color.R);
                Export.Append(",");
                Export.Append(color.G);
                Export.Append(",");
                Export.Append(color.B);
                Export.Append(",");
                Export.Append(color.A);
                Export.AppendLine();
            }
        }

        public override void ClearExportData()
        {
            base.ClearExportData();
            AddHeader();
        }
    }
}