using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ObjectInstanceWriter : TextAssetWriter
    {
        public ObjectInstanceWriter()
        {
            
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Object Instances");
            Export.AppendLine(LanternStrings.ExportHeaderFormat +
                                        "ModelName, PosX, PosY, PosZ, RotX, RotY, RotZ, ScaleX, ScaleY, ScaleZ, ColorIndex");
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            ObjectInstance instance = data as ObjectInstance;

            if (instance == null)
            {
                return;
            }
            
            Export.Append(instance.ObjectName);
            Export.Append(",");
            Export.Append(instance.Position.x.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Position.z.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Position.y.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Rotation.x.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Rotation.z.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Rotation.y.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Scale.x.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Scale.y.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Scale.z.ToString(NumberFormat));
            Export.Append(",");
            Export.Append(instance.Colors == null ? -1 :instance.Colors.Index);

            Export.AppendLine();
        }        
    }
}