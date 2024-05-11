using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Objects
{
    internal class StaticObject
    {
        public string Name = string.Empty;
        public EQStaticData EQObjectData = new EQStaticData();

        public StaticObject(string name)
        {
            Name = name;
        }

        public void LoadEQObjectData(string inputObjectName, string inputObjectFolder)
        {
            // Clear any old data and reload
            EQObjectData = new EQStaticData();
            EQObjectData.LoadDataFromDisk(inputObjectName, inputObjectFolder);
        }
    }
}
