using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWObjects
{
    // TODO: Delete?
    internal class WMO
    {
        public WMORoot RootObject = new WMORoot();
        public List<WMOGroup> GroupObjects = new List<WMOGroup>();
        public string BaseFileName;

        public WMO(string baseFileName)
        {
            BaseFileName = baseFileName;
        }

        public void WriteToDisk(string baseFolderpath)
        {
            Logger.WriteLine("Attempted to write WMO with BaseFileName '" + BaseFileName + "' to disk, but this is NYI");
        }
    }
}
