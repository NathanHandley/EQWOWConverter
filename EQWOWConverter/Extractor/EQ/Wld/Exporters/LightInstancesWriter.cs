using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class LightInstancesWriter : TextAssetWriter
    {
        public LightInstancesWriter()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Light Instances");
            Export.AppendLine(LanternStrings.ExportHeaderFormat +
                                       "PosX, PosY, PosZ, Radius, ColorR, ColorG, ColorB");
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            LightInstance light = data as LightInstance;

            if (light == null)
            {
                return;
            }

            Export.Append(light.Position.x.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.Position.z.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.Position.y.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.Radius.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.LightReference.LightSource.Color.r.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.LightReference.LightSource.Color.g.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(light.LightReference.LightSource.Color.b.ToString(NumberFormat));
            Export.AppendLine();
        }
    }
}