﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

namespace EQWOWConverter.Zones
{
    internal class ZoneEQData
    {
        private bool IsLoaded = false;
        private string MaterialListFileName = string.Empty;
        public MeshData RenderMeshData = new MeshData();
        public MeshData CollisionMeshData = new MeshData();
        public List<Material> Materials = new List<Material>();

        public ColorRGBA AmbientLight = new ColorRGBA();
        public List<LightInstance> LightInstances = new List<LightInstance>();
        public List<ObjectInstance> ObjectInstances = new List<ObjectInstance>();
        public List<SoundInstance> Sound2DInstances = new List<SoundInstance>();
        public List<SoundInstance> Sound3DInstances = new List<SoundInstance>();

        public void LoadDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            if (IsLoaded == true)
                return;
            if (Directory.Exists(inputZoneFolderFullPath) == false)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: ERROR - Could not find path at '" + inputZoneFolderFullPath + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadMaterialDataFromDisk(inputZoneFolderName, inputZoneFolderFullPath);            
            LoadCollisionMeshData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadAmbientLightData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadLightInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadSound2DInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadSound3DInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            LoadObjectInstanceData(inputZoneFolderName, inputZoneFolderFullPath);
            IsLoaded = true;
        }

        public void LoadDataFromObjectOnDisk(string inputObjectFileName, string inputObjectFolderFullPath)
        {
            if (IsLoaded == true)
                return;
            if (Directory.Exists(inputObjectFolderFullPath) == false)
            {
                Logger.WriteError("- [" + inputObjectFolderFullPath + "]: ERROR - Could not find path at '" + inputObjectFolderFullPath + "'");
                return;
            }

            // Load the various blocks
            LoadRenderMeshData(inputObjectFileName, inputObjectFolderFullPath);
            LoadMaterialDataFromDisk(inputObjectFileName, inputObjectFolderFullPath);
            IsLoaded = true;
        }

        private void LoadRenderMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteDebug("- [" + inputZoneFolderName + "]: Reading render mesh data...");
            string renderMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + ".txt");
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(renderMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: ERROR - Could not find render mesh file that should be at '" + renderMeshFileName + "'");
                return;
            }
            RenderMeshData = meshData.Meshdata;
            MaterialListFileName = meshData.MaterialListFileName;
        }

        private void LoadCollisionMeshData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteDebug("- [" + inputZoneFolderName + "]: Reading collision mesh data...");
            string collisionMeshFileName = Path.Combine(inputZoneFolderFullPath, "Meshes", inputZoneFolderName + "_collision.txt");
            if (File.Exists(collisionMeshFileName) == false)
            {
                Logger.WriteDebug("- [" + inputZoneFolderName + "]: No collision mesh found, skipping for zone.");
                return;
            }
            EQMesh meshData = new EQMesh();
            if (meshData.LoadFromDisk(collisionMeshFileName) == false)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: Error loading collision mesh at '" + collisionMeshFileName + "'");
                return;
            }
            CollisionMeshData = meshData.Meshdata;
        }

        private void LoadMaterialDataFromDisk(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            Logger.WriteDebug("- [" + inputZoneFolderName + "]: Reading materials...");
            if (MaterialListFileName == string.Empty)
            {
                Logger.WriteError("- [" + inputZoneFolderName + "]: No material file name found");
                return;
            }
            string materialListFileName = Path.Combine(inputZoneFolderFullPath, "MaterialLists", MaterialListFileName + ".txt");
            EQMaterialList materialListData = new EQMaterialList();
            if (materialListData.LoadFromDisk(materialListFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolderName + "]: No material data found.");
            else
            {
                if (materialListData.MaterialsByTextureVariation.Count > 1)
                    Logger.WriteError("- [" + inputZoneFolderName + "]: Unhandled number of material texture variations");
                Materials = materialListData.MaterialsByTextureVariation[0];
            }
        }

        private void LoadAmbientLightData(string inputZoneFolderName, string inputZoneFolderFullPath)
        {
            // Get the ambient light
            Logger.WriteDebug("- [" + inputZoneFolderName + "]: Reading ambient light data...");
            string ambientLightFileName = Path.Combine(inputZoneFolderFullPath, "ambient_light.txt");
            if (File.Exists(ambientLightFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolderName + "]: No ambient light data found.");
            else
            {
                EQAmbientLight ambientLight = new EQAmbientLight();
                if (ambientLight.LoadFromDisk(ambientLightFileName) == false)
                {
                    Logger.WriteError("- [" + inputZoneFolderName + "]: Error loading ambient light at '" + ambientLightFileName + "'");
                    return;
                }
                AmbientLight = ambientLight.AmbientLight;
            }
        }

        private void LoadObjectInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the object instances
            Logger.WriteDebug("- [" + inputZoneFolder + "]: Reading object instances data...");
            string objectInstancesFileName = Path.Combine(inputZoneFolderFullPath, "object_instances.txt");
            if (File.Exists(objectInstancesFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolder + "]: No object instances data found.");
            else
            {
                EQObjectInstances objectInstances = new EQObjectInstances();
                if (objectInstances.LoadFromDisk(objectInstancesFileName) == false)
                {
                    Logger.WriteError("- [" + inputZoneFolder + "]: Error loading object instances at '" + objectInstancesFileName + "'");
                    return;
                }
                ObjectInstances = objectInstances.ObjectInstances;
            }
        }

        private void LoadLightInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the light instances
            Logger.WriteDebug("- [" + inputZoneFolder + "]: Reading light instances...");
            string lightInstancesFileName = Path.Combine(inputZoneFolderFullPath, "light_instances.txt");
            if (File.Exists(lightInstancesFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolder + "]: No light instance data found.");
            else
            {
                EQLightInstances lightInstances = new EQLightInstances();
                if (lightInstances.LoadFromDisk(lightInstancesFileName) == false)
                {
                    Logger.WriteError("- [" + inputZoneFolder + "]: Error loading light instances at '" + lightInstancesFileName + "'");
                    return;
                }
                LightInstances = lightInstances.LightInstances;
            }
        }

        private void LoadSound2DInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the 2D sound instances
            Logger.WriteDebug("- [" + inputZoneFolder + "]: Reading 2D sound instances...");
            string sound2DInstancesFileName = Path.Combine(inputZoneFolderFullPath, "sound2d_instances.txt");
            if (File.Exists(sound2DInstancesFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolder + "]: No 2D sound instance data found.");
            else
            {
                EQSound2DInstances sound2DInstances = new EQSound2DInstances();
                if (sound2DInstances.LoadFromDisk(sound2DInstancesFileName) == false)
                {
                    Logger.WriteError("- [" + inputZoneFolder + "]: Error loading 2D sound instances at '" + sound2DInstancesFileName + "'");
                    return;
                }
                Sound2DInstances = sound2DInstances.SoundInstances;
            }
        }

        private void LoadSound3DInstanceData(string inputZoneFolder, string inputZoneFolderFullPath)
        {
            // Get the 3D sound instances
            Logger.WriteDebug("- [" + inputZoneFolder + "]: Reading 3D sound instances...");
            string sound3DInstancesFileName = Path.Combine(inputZoneFolderFullPath, "sound3d_instances.txt");
            if (File.Exists(sound3DInstancesFileName) == false)
                Logger.WriteDebug("- [" + inputZoneFolder + "]: No 3D sound instance data found.");
            else
            {
                EQSound3DInstances sound3DInstances = new EQSound3DInstances();
                if (sound3DInstances.LoadFromDisk(sound3DInstancesFileName) == false)
                {
                    Logger.WriteError("- [" + inputZoneFolder + "]: Error loading 3D sound instances at '" + sound3DInstancesFileName + "'");
                    return;
                }
                Sound3DInstances = sound3DInstances.SoundInstances;
            }
        }
    }
}
