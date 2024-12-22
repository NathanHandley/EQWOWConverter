using LanternExtractor.EQ.Archive;
using LanternExtractor.EQ.Wld.Exporters;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.Infrastructure.Logger;

namespace LanternExtractor.EQ.Wld
{
    public class WldFileZoneObjects : WldFile
    {
        public WldFileZoneObjects(ArchiveFile wldFile, string zoneName, WldType type, ILogger logger, Settings settings, WldFile wldToInject = null) : base(
            wldFile, zoneName, type, logger, settings, wldToInject)
        {
        }

        public override void ExportData()
        {
            ExportObjectInstanceAndVertexColorList();
        }

        /// <summary>
        /// Exports the objects instance list (one file per zone) and vertex color lists (one file per object)
        /// </summary>
        private void ExportObjectInstanceAndVertexColorList()
        {
            var instanceList = GetFragmentsOfType<ObjectInstance>();

            if (instanceList.Count == 0)
            {
                Logger.LogWarning("Cannot export object instance list. No object instances found.");
                return;
            }

            ObjectInstanceWriter instanceWriter = new ObjectInstanceWriter();
            VertexColorsWriter colorWriter = new VertexColorsWriter();

            string colorsExportFolder = GetRootExportFolder() + "Objects/VertexColors/";

            foreach (var instance in instanceList)
            {
                instanceWriter.AddFragmentData(instance);

                if (instance.Colors == null)
                {
                    continue;
                }

                colorWriter.AddFragmentData(instance.Colors);
                colorWriter.WriteAssetToFile(colorsExportFolder + "vc_" + instance.Colors.Index + ".txt");
                colorWriter.ClearExportData();
            }

            if (WldToInject != null)
            {
                instanceList = WldToInject.GetFragmentsOfType<ObjectInstance>();

                foreach (var instance in instanceList)
                {
                    instanceWriter.AddFragmentData(instance);

                    if (instance.Colors == null)
                    {
                        continue;
                    }

                    colorWriter.AddFragmentData(instance.Colors);
                    colorWriter.WriteAssetToFile(colorsExportFolder + "vc_" + instance.Colors.Index + ".txt");
                    colorWriter.ClearExportData();
                }
            }

            instanceWriter.WriteAssetToFile(GetExportFolderForWldType() + "object_instances.txt");
        }
    }
}
