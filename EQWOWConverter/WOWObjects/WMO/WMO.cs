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
        public string BaseFileName;

        public WMO(string baseFileName, Zone zone)
        {
            BaseFileName = baseFileName;

            // Create root object
            RootObject = new WMORoot(zone);

            // Create the groups (only one for now)
            GroupObjects.Add(new WMOGroup(zone, RootObject));
        }

        public void WriteToDisk(string baseFolderpath)
        {
            string wmoFolderName = Path.Combine(baseFolderpath, "WMO");
            FileTool.CreateBlankDirectory(wmoFolderName);
            string wmoRootFileName = Path.Combine(wmoFolderName, BaseFileName + ".wmo");
            File.WriteAllBytes(wmoRootFileName, RootObject.RootBytes.ToArray());
            UInt16 curGroupIndex = 0;
            foreach(WMOGroup group in GroupObjects)
            {
                string wmoGroupFileName;
                if (curGroupIndex < 10)
                    wmoGroupFileName = Path.Combine(wmoFolderName, BaseFileName + "_00" + curGroupIndex + ".wmo");
                else if (curGroupIndex < 100)
                    wmoGroupFileName = Path.Combine(wmoFolderName, BaseFileName + "_0" + curGroupIndex + ".wmo");
                else
                    wmoGroupFileName = Path.Combine(wmoFolderName, BaseFileName + "_" + curGroupIndex + ".wmo");
                File.WriteAllBytes(wmoGroupFileName, group.GroupBytes.ToArray());
                curGroupIndex++;
            }
        }
    }
}
