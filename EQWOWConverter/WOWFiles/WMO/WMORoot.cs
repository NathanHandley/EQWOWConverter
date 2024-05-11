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
using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
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
            RootBytes.AddRange(GenerateMVERChunk());

            // MOHD (Header) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOHDChunk(zone.WOWZoneData));

            // MOTX (Textures) --------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOTXChunk(zone));

            // MOMT (Materials) -------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOMTChunk(zone.WOWZoneData));

            // MOGN (Groups) ----------------------------------------------------------------------
            RootBytes.AddRange(GenerateMOGNChunk(zone));

            // MOGI (Group Information) -----------------------------------------------------------
            RootBytes.AddRange(GenerateMOGIChunk(zone.WOWZoneData));

            // MOSB (Skybox, optional) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOSBChunk());

            // MOPV (Portal Verticies) ------------------------------------------------------------
            RootBytes.AddRange(GenerateMOPVChunk());

            // MOPT (Portal Information) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOPTChunk());

            // MOPR (Map Object Portal References) ------------------------------------------------
            RootBytes.AddRange(GenerateMOPRChunk());

            // MOVV (Visible Block Verticies) -----------------------------------------------------
            RootBytes.AddRange(GenerateMOVVChunk());

            // MOVB (Visible Block List) ----------------------------------------------------------
            RootBytes.AddRange(GenerateMOVBChunk());

            // MOLT (Lighting Information) --------------------------------------------------------
            RootBytes.AddRange(GenerateMOLTChunk());

            // MODS (Doodad Set Definitions) ------------------------------------------------------
            RootBytes.AddRange(GenerateMODSChunk());

            // MODN (List of M2s) -----------------------------------------------------------------
            RootBytes.AddRange(GenerateMODNChunk());

            // MODD (Doodad Instance Information) -------------------------------------------------
            RootBytes.AddRange(GenerateMODDChunk());

            // MFOG (Fog Information) -------------------------------------------------------------
            RootBytes.AddRange(GenerateMFOGChunk(zone.WOWZoneData));
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
        private List<byte> GenerateMOHDChunk(WOWZoneData wowZoneData)
        {
            List<byte> chunkBytes = new List<byte>();

            // Number of Textures
            UInt32 numOfTextures = 0;
            foreach (Material material in wowZoneData.Materials)
                if (material.AnimationTextures.Count > 0)
                    numOfTextures++;
            chunkBytes.AddRange(BitConverter.GetBytes(numOfTextures));          

            // Number of Groups
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(wowZoneData.WorldObjects.Count())));             
            
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));    // Number of Portals (Zero for now, but may cause problems?)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));    // Number of Lights (TBD)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));    // Number of Doodad Names
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));    // Number of Doodad Definitions
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1)));    // Number of Doodad Sets (first is the global)
            chunkBytes.AddRange(wowZoneData.AmbientLight.ToBytes());            // Ambiant Light
            chunkBytes.AddRange(BitConverter.GetBytes(wowZoneData.WMOID));      // WMOID (inside WMOAreaTable.dbc)
            chunkBytes.AddRange(wowZoneData.BoundingBox.ToBytesHighRes());      // Axis aligned bounding box for the zone mesh(es)

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
            foreach (Material material in zone.WOWZoneData.Materials)
            {
                if (material.AnimationTextures.Count > 0)
                {
                    string textureName = material.AnimationTextures[0];
                    TextureNameOffsets[textureName] = Convert.ToUInt32(textureBuffer.Count());
                    string curTextureFullPath = "WORLD\\EVERQUEST\\ZONETEXTURES\\" + zone.ShortName.ToUpper() + "\\" + textureName.ToUpper() + ".BLP\0\0\0\0\0";
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
        private List<byte> GenerateMOMTChunk(WOWZoneData wowZoneData)
        {
            List<byte> chunkBytes = new List<byte>();
            foreach (Material material in wowZoneData.Materials)
            {
                List<byte> curMaterialBytes = new List<byte>();

                bool hasNoTexture = false;
                if (material.AnimationTextures.Count == 0 || material.AnimationTextures[0] == String.Empty)
                    hasNoTexture = true;

                // For now, don't put any flags. But see WMOMaterialFlags later
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
                //UInt32 materialFlags = GetPackedFlags(Convert.ToUInt32(WMOMaterialFlags.ClampTextureS), Convert.ToUInt32(WMOMaterialFlags.ClampTextureT));
                //chunkBytes.AddRange(BitConverter.GetBytes(materialFlags));

                // This is the shader, but just use Diffuse for now (0).  1 = Specular, 2 = Metal, 3 = Environment, etc see wowdev
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Blend Mode (2 = GxBlend_Alpha, 3 = GxBlend_Add and ghosty)
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(2)));

                // Texture reference (for diffuse above)
                if (hasNoTexture)
                {
                    // If there was a missing texture, use the first in the list
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

            return WrapInChunk("MOMT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGN (Groups)
        /// </summary>
        private List<byte> GenerateMOGNChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            // For reason unknown to me, put blank space in front (I've seen this in existing wmos)
            chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0\0"));

            // Zone Name
            GroupNameOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupName = zone.DescriptiveName + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupName));            

            // Descriptive Name
            GroupNameDescriptiveOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupNameDescriptive = zone.DescriptiveName + "\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupNameDescriptive));

            // Align the chunk with empty
            while (chunkBytes.Count() % 4 != 0)
                chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            return WrapInChunk("MOGN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGI (Group Information)
        /// </summary>
        private List<byte> GenerateMOGIChunk(WOWZoneData wowZoneData)
        {
            // TODO: Break up interior vs exterior?
            List<byte> chunkBytes = new List<byte>();

            foreach(WorldModelObject curWorldModelObject in wowZoneData.WorldObjects)
            {
                // Group flags
                UInt32 groupInfoFlags = GetPackedFlags(Convert.ToUInt32(WMOGroupFlags.IsOutdoors)); // Convert.ToUInt32(WMOGroupFlags.UseExteriorLighting));
                chunkBytes.AddRange(BitConverter.GetBytes(groupInfoFlags));

                // Since only one group, use the overall bounding box
                chunkBytes.AddRange(curWorldModelObject.BoundingBox.ToBytesHighRes());

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
            while (chunkBytes.Count() % 4 != 0)
                chunkBytes.AddRange(Encoding.ASCII.GetBytes("\0"));

            return WrapInChunk("MOSB", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPV (Portal Verticies)
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
        /// MOVV (Visible Block Verticies)
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
        private List<byte> GenerateMOLTChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MOLT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODS (Doodad Set Definitions)
        /// </summary>
        private List<byte> GenerateMODSChunk()
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
        private List<byte> GenerateMODNChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now
            // Important note: M2 files MUST be referenced as MDX in WMOs

            return WrapInChunk("MODN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODD (Doodad Instance Information)
        /// </summary>
        private List<byte> GenerateMODDChunk()
        {
            List<byte> chunkBytes = new List<byte>();

            // Intentionally blank for now

            return WrapInChunk("MODD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MFOG (Fog Information)
        /// </summary>
        private List<byte> GenerateMFOGChunk(WOWZoneData wowZoneData)
        {
            List<byte> chunkBytes = new List<byte>();
            chunkBytes.AddRange(wowZoneData.FogSettings.ToBytes());
            return WrapInChunk("MFOG", chunkBytes.ToArray());
        }
    }
}
