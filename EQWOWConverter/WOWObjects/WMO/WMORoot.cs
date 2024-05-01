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
using EQWOWConverter.WOWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal class WMORoot : WOWChunkedObject
    {
        public List<byte> RootBytes = new List<byte>();
        public Dictionary<string, UInt32> TextureNameOffsets = new Dictionary<string, UInt32>();
        public UInt32 GroupNameOffset = 0;
        public UInt32 GroupNameDescriptiveOffset = 0;

        public WMORoot(Zone zone)
        {
            // MVER (Version) ---------------------------------------------------------------------
            RootBytes.AddRange(GenerateMVERChunk(zone));

            // MOHD (Header) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOHDChunk(zone));

            // MOTX (Textures) --------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOTXChunk(zone));

            // MOMT (Materials) -------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOMTChunk(zone));

            // MOGN (Groups) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOGNChunk(zone));

            // MOGI (Group Information) -----------------------------------------------------------
            RootBytes.AddRange(GenerateMOGIChunk(zone));

            // MOSB (Skybox, optional) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOSBChunk(zone));

            // MOPV (Portal Verticies) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOPVChunk(zone));

            // MOPT (Portal Information) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOPTChunk(zone));

            // MOPR (Map Object Portal References) ------------------------------------------------
            RootBytes.AddRange(GenerateMOPRChunk(zone));

            // MOVV (Visible Block Verticies) -----------------------------------------------------
            RootBytes.AddRange(GenerateMOVVChunk(zone));

            // MOVB (Visible Block List) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOVBChunk(zone));

            // MOLT (Lighting Information) --------------------------------------------------------
            RootBytes.AddRange(GenerateMOLTChunk(zone));

            // MODS (Doodad Set Definitions) ------------------------------------------------------
            RootBytes.AddRange(GenerateMODSChunk(zone));

            // MODN (List of M2s) -----------------------------------------------------------------
            RootBytes.AddRange(GenerateMODNChunk(zone));

            // MODD (Doodad Instance Information) -------------------------------------------------
            RootBytes.AddRange(GenerateMODDChunk(zone));

            // MFOG (Fog Information) -------------------------------------------------------------
            RootBytes.AddRange(GenerateMFOGChunk(zone));
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk(Zone zone)
        {
            UInt32 version = 17;
            return WrapInChunk("MVER", BitConverter.GetBytes(version));
        }

        /// <summary>
        /// MOHD (Header)
        /// </summary>
        private List<byte> GenerateMOHDChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(BitConverter.GetBytes(zone.TextureCount));   // Number of Textures
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1))); // Number of Groups (always 1 for now)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Portals (Zero for now, but may cause problems?)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Lights (TBD)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Doodad Names
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Doodad Definitions
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1))); // Number of Doodad Sets (first is the global)
            chunkBytes.AddRange(zone.AmbientLight.ToBytes());                // Ambiant Light
            chunkBytes.AddRange(BitConverter.GetBytes(zone.WMOID));          // WMOID (inside WMOAreaTable.dbc)
            chunkBytes.AddRange(zone.BoundingBox.ToBytes());                 // Axis aligned bounding box for the zone mesh(es)

            // For now, get rid of these 
            //UInt32 rootFlags = GetPackedFlags(Convert.ToUInt32(WMORootFlags.DoNotAttenuateVerticesBasedOnDistanceToPortal),
            //                                  Convert.ToUInt32(WMORootFlags.UseUnifiedRenderingPath));
            //chunkBytes.AddRange(BitConverter.GetBytes(rootFlags)); // Flags
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Flags (temp, empty)
            return WrapInChunk("MOHD", chunkBytes.ToArray());

        }

        /// <summary>
        /// MOTX (Textures)
        /// </summary>
        private List<byte> GenerateMOTXChunk(Zone zone)
        {
            //  Store in "WORLD\EVERQUEST\ZONETEXTURES\<zone>\<texture>.BLP"
            //  Pad to make the lengths multiples of 4, with a buffer at the end
            List<byte> textureBuffer = new List<byte>();
            foreach (Material material in zone.Materials)
            {
                foreach (string textureName in material.AnimationTextures)
                {
                    TextureNameOffsets[textureName] = Convert.ToUInt32(textureBuffer.Count());
                    string curTextureFullPath = "WORLD\\EVERQUEST\\ZONETEXTURES\\" + zone.Name.ToUpper() + "\\" + textureName.ToUpper() + ".BLP\0\0\0\0\0";
                    textureBuffer.AddRange(Encoding.ASCII.GetBytes(curTextureFullPath));
                    while (textureBuffer.Count() % 4 != 0)
                        textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0"));
                }
            }
            // Add a buffer at the end
            textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));
            while (textureBuffer.Count() % 4 != 0)
                textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0"));

            return WrapInChunk("MOTX", textureBuffer.ToArray());
        }

        /// <summary>
        /// MOMT (Materials)
        /// </summary>
        private List<byte> GenerateMOMTChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (Material material in zone.Materials)
            {
                List<byte> curMaterialBytes = new List<byte>();

                // For now, don't put any flags. But see WMOMaterialFlags later
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // This is the shader, but just use Diffuse for now (0).  1 = Specular, 2 = Metal, 3 = Environment, etc see wowdev
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Blend Mode (zero for now)
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Texture reference (for diffuse above)
                if (material.AnimationTextures.Count == 0 || material.AnimationTextures[0] == String.Empty)
                {
                    // If there was a missing texture, use the first in the list
                    // TODO: Figure out why this can happen. arena has i_m0000 as a material
                    Logger.WriteLine("Missing texture in material binding, so using first");
                    curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                }
                else
                    curMaterialBytes.AddRange(BitConverter.GetBytes(TextureNameOffsets[material.AnimationTextures[0]]));

                // Emissive color (default to blank for now)
                ColorRGBA emissiveColor = new ColorRGBA(0, 0, 0, 255);
                curMaterialBytes.AddRange(emissiveColor.ToBytes());

                // Not sure what this is.  wowdev has this as sidnColor and 010 template shows flags_1.  Setting to zero for now.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Second texture.  Shouldn't need for EQ. 
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Diffuse color (seems to default to 149 in looking at Darnassus files... why?)  Mess with this later.
                ColorRGBA diffuseColor = new ColorRGBA(149, 149, 149, 255);
                curMaterialBytes.AddRange(diffuseColor.ToBytes());

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
            }            

            /*
            // Temp
            List<byte> curMaterialBytes = new List<byte>();

            // For now, don't put any flags. But see WMOMaterialFlags later
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // This is the shader, but just use Diffuse for now (0).  1 = Specular, 2 = Metal, 3 = Environment, etc see wowdev
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Blend Mode (zero for now)
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Texture reference (for diffuse above)
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Emissive color (default to blank for now)
            ColorRGBA emissiveColor = new ColorRGBA(0, 0, 0, 255);
            curMaterialBytes.AddRange(emissiveColor.ToBytes());

            // Not sure what this is.  wowdev has this as sidnColor and 010 template shows flags_1.  Setting to zero for now.
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Second texture.  Shouldn't need for EQ. 
            curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Diffuse color (seems to default to 149 in looking at Darnassus files... why?)  Mess with this later.
            ColorRGBA diffuseColor = new ColorRGBA(149, 149, 149, 255);
            curMaterialBytes.AddRange(diffuseColor.ToBytes());

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
            */
            return WrapInChunk("MOMT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGN (Groups)
        /// </summary>
        private List<byte> GenerateMOGNChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            /*
            // For reason unknown to me, put a blank spot in front
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            // Zone Name
            string zoneGroupName = zone.Name + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupName));

            // Descriptive Name
            GroupNameDescriptiveOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupNameDescriptive = zone.DescriptiveName + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupNameDescriptive));

            // Align the chunk with empty
            while (chunkBytes.Count() % 4 != 0)
                chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));
            */

            // Temp
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0oid\0\0\0"));
            return WrapInChunk("MOGN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGI (Group Information)
        /// </summary>
        private List<byte> GenerateMOGIChunk(Zone zone)
        {
            // TODO: Break up interior vs exterior?
            List<byte> chunkBytes = new List<byte>();

            /*
            // Group flags
            UInt32 groupInfoFlags = GetPackedFlags(Convert.ToUInt32(WMOGroupFlags.IsOutdoors));
            chunkBytes.AddRange(BitConverter.GetBytes(groupInfoFlags));

            // Since only one group, use the overall bounding box
            chunkBytes.AddRange(zone.BoundingBox.ToBytes());

            // Group name is the first offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(0)));
            List<byte> MOGIChunkByteBlock = WrapInChunk("MOGI", chunkBytes.ToArray());
            */

            // Temp
            // Group flags
            UInt32 groupInfoFlags = GetPackedFlags(Convert.ToUInt32(WMOGroupFlags.IsOutdoors), Convert.ToUInt32(WMOGroupFlags.UseExteriorLighting));
            chunkBytes.AddRange(BitConverter.GetBytes(groupInfoFlags));

            // Since only one group, use the overall bounding box
            AxisAlignedBox boundBox = new AxisAlignedBox();
            boundBox.TopCorner = new Vector3(10f, 10f, 10f);
            boundBox.BottomCorner = new Vector3(-10f, -10f, -10f);
            chunkBytes.AddRange(boundBox.ToBytes());

            // Group name is the first offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(-1)));
            List<byte> MOGIChunkByteBlock = WrapInChunk("MOGI", chunkBytes.ToArray());

            return WrapInChunk("MOGI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOSB (Skybox)
        /// </summary>
        private List<byte> GenerateMOSBChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // For now, just populate with blank (4 bytes)
            //chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            // Temp
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            // Align
            while (chunkBytes.Count() % 4 != 0)
                chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            return WrapInChunk("MOSB", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPV (Portal Verticies)
        /// </summary>
        private List<byte> GenerateMOPVChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOPV is intentially empty (no implementation)");

            return WrapInChunk("MOPV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPT (Portal Information)
        /// </summary>
        private List<byte> GenerateMOPTChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOPT is intentially empty (no implementation)");

            return WrapInChunk("MOPT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPR (Map Object Portal References)
        /// </summary>
        private List<byte> GenerateMOPRChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOPR is intentially empty (no implementation)");

            return WrapInChunk("MOPR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVV (Visible Block Verticies)
        /// </summary>
        private List<byte> GenerateMOVVChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOVV is intentially empty (no implementation)");

            return WrapInChunk("MOVV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVB (Visible Block List)
        /// </summary>
        private List<byte> GenerateMOVBChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOVB is intentially empty (no implementation)");

            return WrapInChunk("MOVB", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOLT (Lighting Information)
        /// </summary>
        private List<byte> GenerateMOLTChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();
            
            Logger.WriteLine("MOLT is intentially empty (no implementation)");
            
            return WrapInChunk("MOLT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODS (Doodad Set Definitions)
        /// </summary>
        private List<byte> GenerateMODSChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // Set Name, always 20 characters
            // There is always at least one set, the global set (Set_$DefaultGlobal)
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("Set_$DefaultGlobal\0\0"));

            // First doodad index
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Number of doodads
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

            // Padding
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

            return WrapInChunk("MODS", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODN (List of M2s)
        /// </summary>
        private List<byte> GenerateMODNChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MODN is intentially empty (no implementation)");

            return WrapInChunk("MODN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODD (Doodad Instance Information)
        /// </summary>
        private List<byte> GenerateMODDChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MODD is intentially empty (no implementation)");

            return WrapInChunk("MODD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MFOG (Fog Information)
        /// </summary>
        private List<byte> GenerateMFOGChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(zone.FogSettings.ToBytes());
            return WrapInChunk("MFOG", chunkBytes.ToArray());
        }
    }
}
