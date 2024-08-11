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
using EQWOWConverter.EQFiles;
using EQWOWConverter.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.ObjectModels
{
    internal class ObjectModelEQData
    {
        public MeshData MeshData = new MeshData();
        public AnimatedVertices AnimatedVertices = new AnimatedVertices();
        public List<Material> Materials = new List<Material>();
        public List<Vector3> CollisionVertices = new List<Vector3>();
        public List<TriangleFace> CollisionTriangleFaces = new List<TriangleFace>();
        private string MaterialListFileName = string.Empty;

        public void LoadDataFromDisk(string inputObjectName, string inputObjectFolder)
        {
            if (Directory.Exists(inputObjectFolder) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error - Could not find path at '" + inputObjectFolder + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputObjectName, inputObjectFolder);
            LoadMaterialDataFromDisk(inputObjectName, inputObjectFolder);
            LoadCollisionMeshData(inputObjectName, inputObjectFolder);
        }

        private void LoadRenderMeshData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + ".txt");
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(renderMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }
            MeshData = meshData.Meshdata;
            MaterialListFileName = meshData.MaterialListFileName;
        }

        private void LoadMaterialDataFromDisk(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading materials...");
            string materialListFileName = Path.Combine(inputObjectFolder, "MaterialLists", MaterialListFileName + ".txt");
            EQMaterialList materialListData = new EQMaterialList();
            if (materialListData.LoadFromDisk(materialListFileName) == false)
                Logger.WriteDetail("- [" + inputObjectName + "]: No material data found.");
            else
            {
                Materials = materialListData.Materials;
            }
        }
        private void LoadCollisionMeshData(string inputObjectName, string inputObjectFolder)
        {
            Logger.WriteDetail("- [" + inputObjectName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputObjectFolder, "Meshes", inputObjectName + "_collision.txt");
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteDetail("- [" + inputObjectName + "]: No collision mesh found, skipping for zone.");
                return;
            }
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(collisionMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputObjectName + "]: Error loading collision mesh at '" + collisionMeshFileName + "'");
                return;
            }
            CollisionTriangleFaces = meshData.Meshdata.TriangleFaces;
            CollisionVertices = meshData.Meshdata.Vertices;
        }
    }
}
