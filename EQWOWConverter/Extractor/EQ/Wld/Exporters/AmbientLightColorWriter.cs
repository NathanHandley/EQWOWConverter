using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class AmbientLightColorWriter : TextAssetWriter
    {
        public AmbientLightColorWriter()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Ambient Light Color");
            Export.AppendLine(LanternStrings.ExportHeaderFormat + "R, G, B");
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            GlobalAmbientLight globalAmbientLight = data as GlobalAmbientLight;

            if (globalAmbientLight == null)
            {
                return;
            }

            Export.Append(globalAmbientLight.Color.R.ToString());
            Export.Append(",");
            Export.Append(globalAmbientLight.Color.G.ToString());
            Export.Append(",");
            Export.Append(globalAmbientLight.Color.B.ToString());
        }
    }
}