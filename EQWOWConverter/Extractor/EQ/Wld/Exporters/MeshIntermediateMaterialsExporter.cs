using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class MeshIntermediateMaterialsWriter : TextAssetWriter
    {
        public override void AddFragmentData(WldFragment data)
        {
            if (!(data is MaterialList list))
            {
                return;
            }
            
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Material List Intermediate Format");
            Export.AppendLine(LanternStrings.ExportHeaderFormat + "Index, MaterialName, AnimationTextures, AnimationDelayMs, SkinTextures");

            for (int i = 0; i < list.Materials.Count; i++)
            {
                Material material = list.Materials[i];
                Export.Append(i);
                Export.Append(",");

                List<Material> allMaterials = new List<Material> {material};
                allMaterials.AddRange(list.GetMaterialVariants(material, null));
                
                for (int j = 0; j < allMaterials.Count; j++)
                {
                    var currentMaterial = allMaterials[j];

                    if (currentMaterial == null)
                    {
                        currentMaterial = allMaterials.First();
                    }

                    Export.Append(GetMaterialString(currentMaterial));

                    if (j < list.VariantCount)
                    {
                        Export.Append(";");
                    }
                }

                Export.Append(",");
                Export.Append(material.BitmapInfoReference?.BitmapInfo.AnimationDelayMs ?? 0);
                Export.AppendLine();
            }
        }
        
        private string GetMaterialString(Material material)
        {
            StringBuilder sb = new StringBuilder();
            var materialName = material.GetFullMaterialName();
            sb.Append(materialName);

            var bitmapNames = material.GetAllBitmapNames();

            foreach (var bitmap in bitmapNames)
            {
                sb.Append(":");
                sb.Append(bitmap);
            }

            return sb.ToString();
        }
    }
}