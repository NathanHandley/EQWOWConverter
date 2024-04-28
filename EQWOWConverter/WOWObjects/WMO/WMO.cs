using EQWOWConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWObjects
{
    internal class WMO
    {
        public WMORoot RootObject;
        public List<WMOGroup> GroupObjects = new List<WMOGroup>();
        private string BaseFileName;
        private string FullWMOFolderPath;
        public string RootFileRelativePathWithFileName;

        public WMO(Zone zone, string baseFolderPath)
        {
            BaseFileName = zone.Name;

            // Create root object
            RootObject = new WMORoot(zone);

            // Create the groups (only one for now)
            GroupObjects.Add(new WMOGroup(zone, RootObject));

            // Generate the root file name
            FullWMOFolderPath = Path.Combine(baseFolderPath,"World", "wmo", "Everquest");
            RootFileRelativePathWithFileName = Path.Combine("World", "wmo", "Everquest", BaseFileName + ".wmo");
        }

        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(FullWMOFolderPath);
            string RootFileFullPathAndFileName = Path.Combine(FullWMOFolderPath, BaseFileName + ".wmo");
            File.WriteAllBytes(RootFileFullPathAndFileName, RootObject.RootBytes.ToArray());
            
            UInt16 curGroupIndex = 0;
            foreach (WMOGroup group in GroupObjects)
            {
                string wmoGroupFileName;
                if (curGroupIndex < 10)
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_00" + curGroupIndex + ".wmo");
                else if (curGroupIndex < 100)
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_0" + curGroupIndex + ".wmo");
                else
                    wmoGroupFileName = Path.Combine(FullWMOFolderPath, BaseFileName + "_" + curGroupIndex + ".wmo");
                File.WriteAllBytes(wmoGroupFileName, group.GroupBytes.ToArray());
                curGroupIndex++;
            }
        }
    }
}
