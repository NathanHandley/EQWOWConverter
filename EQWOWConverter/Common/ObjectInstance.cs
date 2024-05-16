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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class ObjectInstance
    {
        public string ModelName = string.Empty;
        public Vector3 Position = new Vector3();
        public Vector3 Rotation = new Vector3();
        public Vector3 Scale = new Vector3();
        public Int32 ColorIndex = 0;

        public ObjectInstance()
        {
        }

        public ObjectInstance(ObjectInstance otherInstance)
        {
            ModelName = new string(otherInstance.ModelName);
            Position = new Vector3(otherInstance.Position);
            Rotation = new Vector3(otherInstance.Rotation);
            Scale = new Vector3(otherInstance.Scale);
            ColorIndex = otherInstance.ColorIndex;
        }
    }
}
