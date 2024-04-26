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
            Logger.WriteLine("Attempted to write WMO with BaseFileName '" + BaseFileName + "' to disk, but this is NYI");
        }
    }
}
