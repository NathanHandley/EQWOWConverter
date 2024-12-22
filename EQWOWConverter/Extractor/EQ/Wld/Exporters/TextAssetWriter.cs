using System.Globalization;
using System.IO;
using System.Text;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public abstract class TextAssetWriter
    {
        protected StringBuilder Export = new StringBuilder();
        protected NumberFormatInfo NumberFormat = new NumberFormatInfo {NumberDecimalSeparator = "."};

        public abstract void AddFragmentData(WldFragment data);

        public virtual void WriteAssetToFile(string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            if (Export.Length == 0)
            {
                return;
            }
            
            Directory.CreateDirectory(directory);
            
            File.WriteAllText(fileName, Export.ToString());
        }

        public virtual void ClearExportData()
        {
            Export.Clear();
        }

        public int GetExportByteCount()
        {
            return Export.ToString().Length;
        }
    }
}