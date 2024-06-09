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
using System.Xml.Linq;

namespace EQWOWConverter.Objects
{
    internal class ModelObject
    {
        public string Name = string.Empty;
        public EQModelObjectData EQModelObjectData = new EQModelObjectData();
        public WOWObjectModelData WOWModelObjectData = new WOWObjectModelData();

        public ModelObject(string name)
        {
            Name = name;
        }

        public void LoadEQObjectData(string inputObjectName, string inputObjectFolder)
        {
            // Clear any old data and reload
            EQModelObjectData = new EQModelObjectData();
            EQModelObjectData.LoadDataFromDisk(inputObjectName, inputObjectFolder);
        }

        public void PopulateWOWModelObjectDataFromEQModelObjectData()
        {
            WOWModelObjectData = new WOWObjectModelData();

            if (EQModelObjectData.CollisionVertices.Count == 0)
            {
                List<TriangleFace> collisionFaces = new List<TriangleFace>();
                foreach (TriangleFace triangleFace in EQModelObjectData.TriangleFaces)
                    collisionFaces.Add(new TriangleFace(triangleFace));
                List<Vector3> collisionPositions = new List<Vector3>();
                foreach (Vector3 position in EQModelObjectData.Vertices)
                    collisionPositions.Add(new Vector3(position));
                WOWModelObjectData.Load(Name, EQModelObjectData.Materials, EQModelObjectData.TriangleFaces, EQModelObjectData.Vertices, EQModelObjectData.Normals,
                    new List<ColorRGBA>(), EQModelObjectData.TextureCoords, collisionPositions, collisionFaces, true);
            }
            else
                WOWModelObjectData.Load(Name, EQModelObjectData.Materials, EQModelObjectData.TriangleFaces, EQModelObjectData.Vertices, EQModelObjectData.Normals,
                    new List<ColorRGBA>(), EQModelObjectData.TextureCoords, EQModelObjectData.CollisionVertices, EQModelObjectData.CollisionTriangleFaces, true);
        }
    }
}
