using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class MeshObjMtlWriter : TextAssetWriter
    {
        private Settings _settings;
        private string _modelName;
        private int _skinId;
        
        public MeshObjMtlWriter(Settings settings, string modelName)
        {
            _settings = settings;
            _modelName = modelName;
        }

        public void SetSkinId(int id)
        {
            _skinId = id;
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            MaterialList list = data as MaterialList;

            if (list == null)
            {
                return;
            }
            
            bool createdNullMaterial = false;

            foreach (Material material in list.Materials)
            {
                Material skinMaterial = material;

                if (_skinId != 0)
                {
                    int skinId = _skinId - 1;
                    var variants = list.GetMaterialVariants(skinMaterial, new EmptyLogger());

                    if (skinId >= 0 && skinId < variants.Count && variants[skinId] != null)
                    {
                        skinMaterial = variants[skinId];
                    }
                }

                string filenameWithoutExtension = skinMaterial.GetFirstBitmapNameWithoutExtension();
                
                if (string.IsNullOrEmpty(filenameWithoutExtension))
                {
                    if(!createdNullMaterial)
                    {
                        Export.AppendLine(LanternStrings.ObjNewMaterialPrefix + " " + "null");
                        Export.AppendLine("Ka 1.000 1.000 1.000");
                        Export.AppendLine("Kd 1.000 1.000 1.000");
                        Export.AppendLine("Ks 0.000 0.000 0.000");
                        Export.AppendLine("d 1.0 ");
                        Export.AppendLine("illum 2");

                        createdNullMaterial = true;
                    }

                    continue;
                }

                if (skinMaterial.ShaderType == ShaderType.Invisible && !_settings.ExportHiddenGeometry)
                {
                    continue;
                }

                Export.AppendLine(LanternStrings.ObjNewMaterialPrefix + " " + MaterialList.GetMaterialPrefix(material.ShaderType) + material.GetFirstBitmapNameWithoutExtension());
                Export.AppendLine("Ka 1.000 1.000 1.000");
                Export.AppendLine("Kd 1.000 1.000 1.000");
                Export.AppendLine("Ks 0.000 0.000 0.000");
                Export.AppendLine("d 1.0 ");
                Export.AppendLine("illum 2");
                Export.AppendLine("map_Kd " + "Textures/" + skinMaterial.GetFirstBitmapExportFilename());
            }
        }
    }
}