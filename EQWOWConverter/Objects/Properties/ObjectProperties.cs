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

using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Objects.Properties
{
    internal class ObjectProperties
    {
        static private Dictionary<string, ObjectProperties> ObjectPropertiesByByName = new Dictionary<string, ObjectProperties>();

        public string Name = string.Empty;
        public List<ObjectPropertiesClimbingFrame> ClimbingFrames = new List<ObjectPropertiesClimbingFrame>();

        public ObjectProperties() { }
        protected ObjectProperties(string name)
        {
            Name = name;
        }

        protected void AddClimbingFrame(float extendDistance, float stepDistance)
        {
            ObjectPropertiesClimbingFrame climbingFrame = new ObjectPropertiesClimbingFrame(extendDistance, stepDistance);
            ClimbingFrames.Add(climbingFrame);
        }

        public static ObjectProperties GetObjectPropertiesForObject(string objectName)
        {
            if (ObjectPropertiesByByName.Count == 0)
                PopulateObjectPropertiesList();
            if (ObjectPropertiesByByName.ContainsKey(objectName) == false)
                return new ObjectProperties(objectName);
            else
                return ObjectPropertiesByByName[objectName];
        }

        private static void PopulateObjectPropertiesList()
        {
            ObjectPropertiesByByName.Clear();
            ObjectPropertiesByByName.Add("Ladder14", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("Ladder28", new Ladder14ObjectProperties());
            ObjectPropertiesByByName.Add("Ladder42", new Ladder14ObjectProperties());
        }
    }
}
