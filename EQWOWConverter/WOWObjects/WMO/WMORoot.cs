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
        private List<byte> rootBytes = new List<byte>();
        private Dictionary<string, UInt32> textureNameOffsets = new Dictionary<string, UInt32>();
        public UInt32 GroupNameOffset = 0;
        public UInt32 GroupNameDescriptiveOffset = 0;

        public WMORoot(Zone zone)
        {
            // MVER (Version) ---------------------------------------------------------------------
            rootBytes.AddRange(GenerateMVERChunk(zone));

            // MOHD (Header) ----------------------------------------------------------------------
            rootBytes.AddRange(GenerateMOHDChunk(zone));

            // MOTX (Textures) --------------------------------------------------------------------
            rootBytes.AddRange(GenerateMOTXChunk(zone));

            // MOMT (Materials) -------------------------------------------------------------------
            rootBytes.AddRange(GenerateMOMTChunk(zone));

            // MOGN (Groups) ----------------------------------------------------------------------
            rootBytes.AddRange(GenerateMOGNChunk(zone));

            // MOGI (Group Information) -----------------------------------------------------------
            rootBytes.AddRange(GenerateMOGIChunk(zone));

            // MOSB (Skybox, optional) ------------------------------------------------------------
            // Not implementing yet

            // MOPV (Portal Verticies) ------------------------------------------------------------
            rootBytes.AddRange(GenerateMOPVChunk(zone));

            // MOPT (Portal Information) ----------------------------------------------------------
            rootBytes.AddRange(GenerateMOPTChunk(zone));

            // MOPR (Map Object Portal References) ------------------------------------------------
            rootBytes.AddRange(GenerateMOPRChunk(zone));

            // MOLT (Lighting Information) --------------------------------------------------------
            rootBytes.AddRange(GenerateMOLTChunk(zone));

            // MODS (Doodad Set Definitions) ------------------------------------------------------
            rootBytes.AddRange(GenerateMODSChunk(zone));

            // MODN (List of M2s) -----------------------------------------------------------------
            rootBytes.AddRange(GenerateMODNChunk(zone));

            // MODD (Doodad Instance Information) -------------------------------------------------
            rootBytes.AddRange(GenerateMODDChunk(zone));

            // MFOG (Fog Information) -------------------------------------------------------------
            rootBytes.AddRange(GenerateMFOGChunk(zone));
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
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(1))); // Number of Groups (always 1)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Portals (Zero for now, but may cause problems?)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Lights (TBD)
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Doodad Names
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Doodad Definitions
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0))); // Number of Doodad Sets
            chunkBytes.AddRange(zone.AmbientLight.ToBytes());                // Ambiant Light
            chunkBytes.AddRange(BitConverter.GetBytes(zone.WMOID));          // WMOID (inside WMOAreaTable.dbc)
            chunkBytes.AddRange(zone.BoundingBox.ToBytes());                 // Axis aligned bounding box for the zone mesh(es)
            UInt32 rootFlags = GetPackedFlags(Convert.ToUInt32(WMORootFlags.DoNotAttenuateVerticesBasedOnDistanceToPortal),
                                              Convert.ToUInt32(WMORootFlags.UseUnifiedRenderingPath));
            chunkBytes.AddRange(BitConverter.GetBytes(rootFlags)); // Flags

            return WrapInChunk("MOHD", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOTX (Textures)
        /// </summary>
        private List<byte> GenerateMOTXChunk(Zone zone)
        {
            //  Store in "WORLD\EVERQUEST\ZONETEXTURES\<zone>\<texture>.BLP"
            //  Pad to make the lengths multiples of 4, with minimum total of 5 "\0"
            List<byte> textureBuffer = new List<byte>();
            foreach (Material material in zone.Materials)
            {
                foreach (string textureName in material.AnimationTextures)
                {
                    textureNameOffsets[textureName] = Convert.ToUInt32(textureBuffer.Count());
                    string curTextureFullPath = "WORLD\\EVERQUEST\\ZONETEXTURES\\" + zone.Name.ToUpper() + "\\" + textureName.ToUpper() + ".BLP\0\0\0\0\0";
                    textureBuffer.AddRange(Encoding.ASCII.GetBytes(curTextureFullPath));
                    while (textureBuffer.Count() % 4 != 0)
                        textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0"));
                }
            }
            // Add a final texture for 'blank' at the end
            textureNameOffsets[String.Empty] = Convert.ToUInt32(textureBuffer.Count());
            textureBuffer.AddRange(Encoding.ASCII.GetBytes("\0\0\0\0"));

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
                    curMaterialBytes.AddRange(BitConverter.GetBytes(textureNameOffsets[String.Empty]));
                else
                    curMaterialBytes.AddRange(BitConverter.GetBytes(textureNameOffsets[material.AnimationTextures[0]]));

                // Emissive color (default to blank for now)
                ColorRGBA emissiveColor = new ColorRGBA(0, 0, 0, 255);
                curMaterialBytes.AddRange(emissiveColor.ToBytes());

                // Not sure what this is.  wowdev has this as sidnColor and 010 template shows flags_1.  Setting to zero for now.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Second texture.  Shouldn't need for EQ. 
                curMaterialBytes.AddRange(BitConverter.GetBytes(textureNameOffsets[String.Empty]));

                // Diffuse color (seems to default to 149 in looking at Darnassus files... why?)  Mess with this later.
                ColorRGBA diffuseColor = new ColorRGBA(149, 149, 149, 255);
                curMaterialBytes.AddRange(diffuseColor.ToBytes());

                // TerrainType ID (from the DBC). TODO: Find a way to map this to be useful for EQ.  Setting to 6 for grass for now.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(6)));

                // 3rd texture offset (Specular?).  Not using it
                curMaterialBytes.AddRange(BitConverter.GetBytes(textureNameOffsets[String.Empty]));

                // Not 100% on this color.  Seems related to the 3rd texture.  Investigate if useful.
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // Can't find a definition for this other than it's a flag
                curMaterialBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                // 4 values that can be ignored, I think.  They seem runtime related.
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
            
            // For now, only have one group name since we'll have only one group for the whole zone (both a name and descriptive name)
            string zoneGroupName = zone.Name + "\0\0\0\0\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupName));
            GroupNameDescriptiveOffset = Convert.ToUInt32(chunkBytes.Count);
            string zoneGroupNameDescriptive = zone.DescriptiveName + "\0\0\0\0\0";
            chunkBytes.AddRange(Encoding.ASCII.GetBytes(zoneGroupNameDescriptive));
            return WrapInChunk("MOGN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOGI (Group Information)
        /// </summary>
        private List<byte> GenerateMOGIChunk(Zone zone)
        {
            // TODO: Break up interior vs exterior?
            List<byte> chunkBytes = new List<byte>();

            // Group flags
            UInt32 groupInfoFlags = GetPackedFlags(Convert.ToUInt32(WMOGroupFlags.IsOutdoors),
                                                   Convert.ToUInt32(WMOGroupFlags.HasLights),
                                                   // Convert.ToUInt32(WMOGroupFlags.DoShowSkybox) TODO: Uncomment when there's a skybox
                                                   Convert.ToUInt32(WMOGroupFlags.AlwaysDraw)); // Unsure if this should be set
            chunkBytes.AddRange(BitConverter.GetBytes(groupInfoFlags));

            // Since only one group, use the overall bounding  box
            chunkBytes.AddRange(zone.BoundingBox.ToBytes());

            // Group name is the first offset
            chunkBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));
            List<byte> MOGIChunkByteBlock = WrapInChunk("MOGI", chunkBytes.ToArray());

            return WrapInChunk("MOGI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOSB (Skybox, optional)
        /// </summary>
        private List<byte> GenerateMOSBChunk(Zone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOSB Generation unimplemented!");

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

            Logger.WriteLine("MODS is intentially empty (no implementation)");

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
