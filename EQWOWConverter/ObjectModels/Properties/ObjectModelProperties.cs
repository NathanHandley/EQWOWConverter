//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using EQWOWConverter.Common;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ObjectModels.Properties
{
    internal class ObjectModelProperties
    {
        static private Dictionary<string, ObjectModelProperties> ObjectPropertiesByByName = new Dictionary<string, ObjectModelProperties>();

        public string Name = string.Empty;
        public ObjectModelCustomCollisionType CustomCollisionType = ObjectModelCustomCollisionType.None;
        public bool DisableCollision = false;

        public ObjectModelProperties() { }
        protected ObjectModelProperties(string name)
        {
            Name = name;
        }

        protected void SetCustomCollisionType(ObjectModelCustomCollisionType customCollisionType)
        {
            CustomCollisionType = customCollisionType;
        }

        protected void SetCollisionDisabled(bool disableCollision)
        {
            DisableCollision = disableCollision;
        }

        public static ObjectModelProperties GetObjectPropertiesForObject(string objectName)
        {
            if (ObjectPropertiesByByName.Count == 0)
                PopulateObjectPropertiesList();
            if (ObjectPropertiesByByName.ContainsKey(objectName) == false)
                return new ObjectModelProperties(objectName);
            else
                return ObjectPropertiesByByName[objectName];
        }

        private static void PopulateObjectPropertiesList()
        {
            ObjectPropertiesByByName.Clear();
            ObjectPropertiesByByName.Add("anvil1", new Anvil1ObjectProperties());
            ObjectPropertiesByByName.Add("anvil2", new Anvil2ObjectProperties());
            ObjectPropertiesByByName.Add("ladder14", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder20", new Ladder20ObjectProperties());
            ObjectPropertiesByByName.Add("ladder28", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder42", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("ladder60", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("torch2", new Torch2ObjectProperties());
            ObjectPropertiesByByName.Add("torch2alt1", new Torch2Alt1ObjectProperties());            
        }
    }
}
