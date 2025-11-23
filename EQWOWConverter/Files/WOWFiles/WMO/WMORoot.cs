//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
using EQWOWConverter.Zones;
using System.Text;

namespace EQWOWConverter.WOWFiles
{
    internal class WMORoot : WOWChunkedObject
    {
        public List<byte> RootBytes = new List<byte>();
        public Dictionary<string, UInt32> TextureNameOffsets = new Dictionary<string, UInt32>();
        public Dictionary<string, UInt32> DoodadPathOffsetsByName = new Dictionary<string, UInt32>();
        public Dictionary<int, int> BatchMaterialIDsByMaterialIndex = new Dictionary<int, int>();
        public List<string> DoodadPathStrings = new List<string>();
        public UInt32 GroupNameOffset = 0;
        public UInt32 GroupNameDescriptiveOffset = 0;

        public WMORoot(Zone zone, string textureRelativeOutputFolder, string relativeStaticDoodadsFolder, string relativeZoneObjectsFolder,
            bool addConvexVolumePlanes)
        {
            List<ZoneDoodadInstance> doodadInstances;
            if (Configuration.ZONE_MINIMAP_GENERATION_MODE_ENABLED == true)
                doodadInstances = new List<ZoneDoodadInstance>();
            else
                doodadInstances = zone.DoodadInstances;
            
            PopulateDoodadPathStringOffsets(doodadInstances, relativeStaticDoodadsFolder, relativeZoneObjectsFolder);

            // MVER (Version) ---------------------------------------------------------------------
            RootBytes.AddRange(GenerateMVERChunk());

            // MOHD (Header) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOHDChunk(zone.Materials, zone.ZoneObjectModels.Count(), zone.LightInstances.Count(),
                DoodadPathOffsetsByName.Count(), zone.ZoneProperties.DBCWMOID, zone.AllGeometryBoundingBox));

            // MOTX (Textures) --------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOTXChunk(textureRelativeOutputFolder, zone.ShortName, zone.Materials));

            // MOMT (Materials) -------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOMTChunk(zone.ShortName, zone.Materials));

            // MOGN (Groups) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOGNChunk(zone.DescriptiveName));

            // MOGI (Group Information) -----------------------------------------------------------
            RootBytes.AddRange(GenerateMOGIChunk(zone.ZoneObjectModels));

            // MOSB (Skybox, optional) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOSBChunk());

            // MOPV (Portal Vertices) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOPVChunk());

            // MOPT (Portal Information) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOPTChunk());

            // MOPR (Map Object Portal References) ------------------------------------------------
            RootBytes.AddRange(GenerateMOPRChunk());

            // MOVV (Visible Block Vertices) -----------------------------------------------------
            RootBytes.AddRange(GenerateMOVVChunk());

            // MOVB (Visible Block List) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOVBChunk());

            // MOLT (Lighting Information) --------------------------------------------------------
            RootBytes.AddRange(GenerateMOLTChunk(zone.LightInstances));

            // MODS (Doodad Set Definitions) ------------------------------------------------------
            RootBytes.AddRange(GenerateMODSChunk(doodadInstances));

            // MODN (List of M2s) -----------------------------------------------------------------
            RootBytes.AddRange(GenerateMODNChunk());

            // MODD (Doodad Instance Information) -------------------------------------------------
            RootBytes.AddRange(GenerateMODDChunk(doodadInstances));

            // MFOG (Fog Information) -------------------------------------------------------------
            RootBytes.AddRange(GenerateMFOGChunk());

            // MCVP (Convex Volume Planes (optional)) ---------------------------------------------
            if (addConvexVolumePlanes == true)
                RootBytes.AddRange(GenerateMCVPChunk());
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk()
        {
            UInt32 version = 17;
            return WrapInChunk("MVER", BitConverter.GetBytes(version));
        }

        /// <summary>
        /// MOHD (Header)
        /// </summary>
        private List<byte> GenerateMOHDChunk(List<Material> materials, int numOfZoneObjectModels, int numOfLightInstances,
            int numOfDoodadInstances, uint dbcWMOID, BoundingBox boundingBox)
        {
            List<byte> chunkBytes = new List<byte>();

            // Number of Textures
            UInt32 numOfTextures = 0;
            foreach (Material material in materials)
            {
                if (material.IsAnimated() || material.HasTransparency())
                    continue;
                if (material.IsRenderable() == false)
                    continue;
                if (material.TextureNames.Count > 0)
                    numOfTextures++;
            }
            chunkBytes.AddRange(BitConverter.GetBytes(numOfTextures));          

            // Number of Groups
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(numOfZoneObjectModels)));

            // Number of Portals
            chunkBytes.AddRange(BitConverter.GetBytes(0));    

            // Number of Lights
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(numOfLightInstances)));

            // Number of Doodad Names
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(numOfDoodadInstances)));

            // Number of Doodad Definitions
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(numOfDoodadInstances)));

            // Number of Doodad Sets (first is the global)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1)));

            // Ambiant Light (indoor areas only)
            chunkBytes.AddRange(new ColorRGBA().ToBytesBGRA());

            // WMOID (inside WMOAreaTable.dbc)
            chunkBytes.AddRange(BitConverter.GetBytes(dbcWMOID));

            // Axis aligned bounding box for the zone mesh(es)
            chunkBytes.AddRange(boundingBox.ToBytesHighRes());      

            // Set any flags
            WMORootFlags rootFlags = WMORootFlags.UseLiquidTypeDBCID;
            rootFlags |= WMORootFlags.DoNotFixVertexColorAlpha;
            rootFlags |= WMORootFlags.UseUnifiedRenderingPath;
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(rootFlags)));
            return WrapInChunk("MOHD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOTX (Textures)
        /// </summary>
        private List<byte> GenerateMOTXChunk(string relativeTextureFolder, string name, List<Material> materials)
        {
            //  Pad to make the lengths multiples of 4, with a buffer at the end
            List<byte> textureBuffer = new List<byte>();
            foreach (Material material in materials)
            {
                // Don't add the texture if the material won't be used anyway
                if (material.IsAnimated() || material.HasTransparency())
                    continue;
                if (material.IsRenderable() == false)
                    continue;

                if (material.TextureNames.Count > 0)
                {
                    // TODO: Account for more texture names when texture animation is fully implemented
                    string textureName = material.TextureNames[0];
                    TextureNameOffsets[textureName] = Convert.ToUInt32(textureBuffer.Count());
                    string curTextureFullPath = relativeTextureFolder + "\\" + name.ToUpper() + "\\" + textureName.ToUpper() + ".BLP\0\0\0\0\0";
                    textureBuffer.AddRange(Encoding.ASCII.GetBytes(curTextureFullPath));
                    while (textureBuffer.Count() % 4 != 0)
                        textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0"));
                }
            }
            // Add a buffer at the end
            textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));
            AlignBytesTo4Bytes(ref textureBuffer);

            return WrapInChunk("MOTX", textureBuffer.ToArray());
        }

        /// <summary>
        /// MOMT (Materials)
        /// </summary>
        private List<byte> GenerateMOMTChunk(string name, List<Material> materials)
        {
            List<byte> chunkBytes = new List<byte>();
            int curBatchMaterialID = 0;
            for (int i = 0; i < materials.Count; ++i)
            {
                Material material = materials[i];

                // Skip any non-rendered materials
                if (material.IsAnimated() || material.HasTransparency())
                    continue;
                if (material.IsRenderable() == false)
                    continue;

                // For kedge, put in the front since there are no non-animated textures in the zone
                if (name == "kedge")
                    BatchMaterialIDsByMaterialIndex.Add(0, curBatchMaterialID);
                else
                    BatchMaterialIDsByMaterialIndex.Add(i, curBatchMaterialID);

                List<byte> curMaterialBytes = new List<byte>();

                bool hasNoTexture = false;
                if (material.TextureNames.Count == 0)
                    hasNoTexture = true;

                if (material.AlwaysBrightOverride == true)
                    curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(WMOMaterialFlags.LightAtNight)));
                else
                    curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // This is the shader, but just use Diffuse for now (0).  1 = Specular, 2 = Metal, 3 = Environment, etc see wowdev
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Blend Mode (2 = GxBlend_Alpha, 3 = GxBlend_Add and ghosty)
                switch (material.MaterialType)
                {
                    case MaterialType.TransparentAdditive:
                    case MaterialType.TransparentAdditiveUnlit:
                    case MaterialType.TransparentAdditiveUnlitSkydome:
                        {
                            // GxBlend_Add
                            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(3)));
                        }
                        break;
                    case MaterialType.Transparent25Percent:
                    case MaterialType.Transparent75Percent:
                    case MaterialType.Transparent50Percent:
                    case MaterialType.TransparentMasked:
                        {
                            // GxBlend_Alpha
                            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(2)));
                        }
                        break;
                    default:
                        {
                            // GxBlend_Opaque
                            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                        }
                        break;
                }                

                // Texture reference (for diffuse above)
                // TODO: Account for more texture names once texture animation is fully implemented
                if (hasNoTexture)
                {
                    // If there was a missing texture, use the first in the list
                    curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                }
                else
                    curMaterialBytes.AddRange(BitConverter.GetBytes(TextureNameOffsets[material.TextureNames[0]]));

                // Emissive color (default to blank for now)
                ColorRGBA emissiveColor = new ColorRGBA(0, 0, 0, 255);
                curMaterialBytes.AddRange(emissiveColor.ToBytesRGBA());

                // Not sure what this is.  wowdev has this as sidnColor and 010 template shows flags_1.  Setting to zero for now.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Second texture.  Shouldn't need for EQ. 
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Diffuse color (seems to default to 149 in looking at Darnassus files... why?)  Mess with this later.
                ColorRGBA diffuseColor = new ColorRGBA(149, 149, 149, 255);
                curMaterialBytes.AddRange(diffuseColor.ToBytesRGBA());

                // TerrainType ID (from the DBC). TODO: Find a way to map this to be useful for EQ.  Setting to 6 for grass for now.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(6)));

                // 3rd texture offset (Specular?).  Not using it
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Not 100% on this color.  Seems related to the 3rd texture.  Investigate if useful.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Can't find a definition for this other than it's a flag
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // 4 values that can be ignored, I think.  They seem runtime related.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Add to the bigger container
                chunkBytes.AddRange(curMaterialBytes.ToArray());
                
                curBatchMaterialID++;
            }            

            return WrapInChunk("MOMT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGN (Groups)
        /// </summary>
        private List<byte> GenerateMOGNChunk(string descriptiveName)
        {
            List<byte> chunkBytes = new List<byte>();

            // For reason unknown to me, put blank space in front (I've seen this in existing wmos)
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0"));

            // Zone Name
            GroupNameOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupName = descriptiveName + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupName));            

            // Descriptive Name
            GroupNameDescriptiveOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupNameDescriptive = descriptiveName + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupNameDescriptive));

            // Align the chunk with empty
            AlignBytesTo4Bytes(ref chunkBytes);

            return WrapInChunk("MOGN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGI (Group Information)
        /// </summary>
        private List<byte> GenerateMOGIChunk(List<ZoneModelObject> zoneModelObjects)
        {
            // TODO: Break up interior vs exterior?
            List<byte> chunkBytes = new List<byte>();

            foreach(ZoneModelObject curWorldObjectModel in zoneModelObjects)
            {
                // Header flags
                chunkBytes.AddRange(BitConverter.GetBytes(curWorldObjectModel.GenerateWMOHeaderFlags()));

                // Use the bounding box
                chunkBytes.AddRange(curWorldObjectModel.BoundingBox.ToBytesHighRes());

                // Group name is the first offset
                chunkBytes.AddRange(BitConverter.GetBytes(GroupNameOffset));
            }

            return WrapInChunk("MOGI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOSB (Skybox)
        /// </summary>
        private List<byte> GenerateMOSBChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Blank for now
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            // Align
            AlignBytesTo4Bytes(ref chunkBytes);

            return WrapInChunk("MOSB", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPV (Portal Vertices)
        /// </summary>
        private List<byte> GenerateMOPVChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOPV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPT (Portal Information)
        /// </summary>
        private List<byte> GenerateMOPTChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOPT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPR (Map Object Portal References)
        /// </summary>
        private List<byte> GenerateMOPRChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOPR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVV (Visible Block Vertices)
        /// </summary>
        private List<byte> GenerateMOVVChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOVV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVB (Visible Block List)
        /// </summary>
        private List<byte> GenerateMOVBChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOVB", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOLT (Lighting Information)
        /// </summary>
        private List<byte> GenerateMOLTChunk(List<LightInstance> lightInstances)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (LightInstance lightInstance in lightInstances)
                chunkBytes.AddRange(lightInstance.ToBytes());
            return WrapInChunk("MOLT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODS (Doodad Set Definitions)
        /// </summary>
        private List<byte> GenerateMODSChunk(List<ZoneDoodadInstance> doodadInstances)
        {
            List<byte> chunkBytes = new List<byte>();

            // Set Name, always 20 characters
            // There is always at least one set, the global set (Set_$DefaultGlobal)
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("Set_$DefaultGlobal\0\0"));

            // First doodad index
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Number of doodads
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(doodadInstances.Count)));

            // Padding
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            return WrapInChunk("MODS", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODN (List of M2s)
        ///  NOTE: M2 files must be reference as MDX in the WMO, regardless of real name
        /// </summary>
        private List<byte> GenerateMODNChunk()
        {
            List<byte> doodadNameBuffer = new List<byte>();
            foreach (string doodadFullPath in DoodadPathStrings)
                doodadNameBuffer.AddRange(Encoding.ASCII.GetBytes(doodadFullPath));
            if (doodadNameBuffer.Count != 0)
                AlignBytesTo4Bytes(ref doodadNameBuffer);
            return WrapInChunk("MODN", doodadNameBuffer.ToArray());
        }

        /// <summary>
        /// MODD (Doodad Instance Information)
        /// </summary>
        private List<byte> GenerateMODDChunk(List<ZoneDoodadInstance> doodadInstances)
        {
            List<byte> chunkBytes = new List<byte>();

            foreach (ZoneDoodadInstance doodadInstance in doodadInstances)
            {
                doodadInstance.WMOObjectNameOffset = DoodadPathOffsetsByName[doodadInstance.ObjectName];
                chunkBytes.AddRange(doodadInstance.ToBytesForWMO());
            }

            // Align
            AlignBytesTo4Bytes(ref chunkBytes);

            return WrapInChunk("MODD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MFOG (Fog Information)
        /// </summary>
        private List<byte> GenerateMFOGChunk()
        {
            // Hard coded defaults since we won't be using this block
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0x00))); // HasInfiniteRadius boolean
            chunkBytes.AddRange(new Vector3().ToBytes()); // Position
            chunkBytes.AddRange(BitConverter.GetBytes(0)); // Near Radius
            chunkBytes.AddRange(BitConverter.GetBytes(0)); // Far Radius
            chunkBytes.AddRange(BitConverter.GetBytes(444.4445f)); // Fog End Distance
            chunkBytes.AddRange(BitConverter.GetBytes(0.25f)); // Fog Start Scalar
            chunkBytes.AddRange(new ColorRGBA(255, 255, 255, 255).ToBytesBGRA()); // Fog color
            chunkBytes.AddRange(BitConverter.GetBytes(222.2222f)); // Underwater Fog End Distance
            chunkBytes.AddRange(BitConverter.GetBytes(-0.5f)); // Underwater Fog Start Scalar
            chunkBytes.AddRange(new ColorRGBA(255, 0, 0, 255).ToBytesBGRA()); // Underwater Fog Color
            return WrapInChunk("MFOG", chunkBytes.ToArray());
        }

        /// <summary>
        /// MCVP (Convex Volume Planes), optional
        /// </summary>
        private List<byte> GenerateMCVPChunk()
        {
            // This could be done better, but functions as desired
            List<Plane> convexVolumePlanes = new List<Plane>();
            convexVolumePlanes.Add(new Plane(0.000000f, 0.000000f, 1.000000f, -1f));
            convexVolumePlanes.Add(new Plane(0.000000f, 0.000000f, -1.000000f, -1f));
            convexVolumePlanes.Add(new Plane(0.000000f, 1.000000f, 0.000000f, -1f));
            convexVolumePlanes.Add(new Plane(0.000000f, -1.000000f, 0.000000f, -1f));
            convexVolumePlanes.Add(new Plane(1.000000f, 0.000000f, 0.000000f, -1f));
            convexVolumePlanes.Add(new Plane(-1.000000f, 0.000000f, 0.000000f, -1f));

            List<byte> chunkBytes = new List<byte>();
            foreach (Plane plane in convexVolumePlanes)
                chunkBytes.AddRange(plane.ToBytes());
            return WrapInChunk("MCVP", chunkBytes.ToArray());
        }

        private void PopulateDoodadPathStringOffsets(List<ZoneDoodadInstance> objectInstances, string relativeStaticDoodadsFolder, string relativeZoneObjectsFolder)
        {
            int curPathOffset = 0;
            foreach (ZoneDoodadInstance objectInstance in objectInstances)
            {
                string objectName = objectInstance.ObjectName;
                string objectFullPath = string.Empty;
                if (objectInstance.DoodadType == ZoneDoodadInstanceType.StaticObject)
                    objectFullPath = Path.Combine(relativeStaticDoodadsFolder, objectName, objectName + ".MDX" + "\0").ToUpper();
                else if (objectInstance.DoodadType == ZoneDoodadInstanceType.ZoneMaterial || objectInstance.DoodadType == ZoneDoodadInstanceType.SoundInstance)
                    objectFullPath = Path.Combine(relativeZoneObjectsFolder, objectName, objectName + ".MDX" + "\0").ToUpper();
                else
                    Logger.WriteError("Unhandled type of doodad instance '" + objectInstance.DoodadType.ToString() + "' for doodad name '" + objectInstance.ObjectName + "'");

                if (DoodadPathOffsetsByName.ContainsKey(objectName) == false)
                {
                    DoodadPathOffsetsByName.Add(objectName, Convert.ToUInt32(curPathOffset));
                    DoodadPathStrings.Add(objectFullPath);
                    curPathOffset += objectFullPath.Length;
                }
            }
        }
    }
}
