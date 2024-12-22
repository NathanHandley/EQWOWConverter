using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ParticleSystemWriter : TextAssetWriter
    {
        public ParticleSystemWriter()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "Particle System");
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            if (!(data is ParticleCloud))
            {
                return;
            }

            Export.AppendLine(FragmentNameCleaner.CleanName(data));
        }
    }
}