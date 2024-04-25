using EQWOWConverter.EQObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWObjects
{
    internal class WMOGroup : WOWChunkedObject
    {
        private List<byte> groupBytes = new List<byte>();

        public WMOGroup(EQZone zone)
        {
            // ====================================================================================
            // WMO Group ==========================================================================
            // ====================================================================================

            // MVER (Version) ---------------------------------------------------------------------

            // MOGP (Container for all other chunks) ----------------------------------------------

                // MOPY (Material info for triangles) -------------------------------------------------

                // MOVI (MapObject Vertex Indicies) ---------------------------------------------------

                // MOVT (Verticies) -------------------------------------------------------------------

                // MONR (Normals) ---------------------------------------------------------------------

                // MOTV (Texture Coordinates) ---------------------------------------------------------

                // MOBA (Render Batches) --------------------------------------------------------------

                // MOLR (Light References) ------------------------------------------------------------
                // -- If has Lights

                // MODR (Doodad References) -----------------------------------------------------------
                // -- If has Doodads

                // MOBN (Nodes of the BSP tree, used also for collision?) -----------------------------
                // -- If HasBSPTree flag

                // MOBR (Face / Triangle Incidies) ----------------------------------------------------
                // -- If HasBSPTree flag

                // MOCV (Vertex Colors) ---------------------------------------------------------------
                // - If HasVertexColor Flag

                // MLIQ (Liquid/Water details) --------------------------------------------------------
                // - If HasWater flag

            // Note: There can be two MOTV and MOCV blocks depending on flags.  May need to factor for that
        }

        /// <summary>
        /// MVER (Version)
        /// </summary>
        private List<byte> GenerateMVERChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MVER Generation unimplemented!");

            return WrapInChunk("MVER", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOPY (Material info for triangles)
        /// </summary>
        private List<byte> GenerateMOPYChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOPY Generation unimplemented!");

            return WrapInChunk("MOPY", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVI (MapObject Vertex Indicies)
        /// </summary>
        private List<byte> GenerateMOVIChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOVI Generation unimplemented!");

            return WrapInChunk("MOVI", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOVT (Verticies)
        /// </summary>
        private List<byte> GenerateMOVTChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOVT Generation unimplemented!");

            return WrapInChunk("MOVT", chunkBytes.ToArray());
        }

        /// <summary>
        /// MONR (Normals)
        /// </summary>
        private List<byte> GenerateMONRChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MONR Generation unimplemented!");

            return WrapInChunk("MONR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOTV (Texture Coordinates)
        /// </summary>
        private List<byte> GenerateMOTVChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOTV Generation unimplemented!");

            return WrapInChunk("MOTV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBA (Render Batches)
        /// </summary>
        private List<byte> GenerateMOBAChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOBA Generation unimplemented!");

            return WrapInChunk("MOBA", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOLR (Light References)
        /// Optional.  Only if it has lights
        /// </summary>
        private List<byte> GenerateMOLRChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOLR Generation unimplemented!");

            return WrapInChunk("MOLR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MODR (Doodad References)
        /// Optional.  If has Doodads
        /// </summary>
        private List<byte> GenerateMODRChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MODR Generation unimplemented!");

            return WrapInChunk("MODR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBN (Nodes of the BSP tree, used also for collision?)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBNChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOBN Generation unimplemented!");

            return WrapInChunk("MOBN", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOBR (Face / Triangle Incidies)
        /// Optional.  If HasBSPTree flag.
        /// </summary>
        private List<byte> GenerateMOBRChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOBR Generation unimplemented!");

            return WrapInChunk("MOBR", chunkBytes.ToArray());
        }

        /// <summary>
        /// MOCV (Vertex Colors)
        /// Optional.  If HasVertexColor Flag
        /// </summary>
        private List<byte> GenerateMOCVChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MOCV Generation unimplemented!");

            return WrapInChunk("MOCV", chunkBytes.ToArray());
        }

        /// <summary>
        /// MLIQ (Liquid/Water details)
        /// Optional.  If HasWater flag
        /// </summary>
        private List<byte> GenerateMLIQChunk(EQZone zone)
        {
            List<byte> chunkBytes = new List<byte>();

            Logger.WriteLine("MLIQ Generation unimplemented!");

            return WrapInChunk("MLIQ", chunkBytes.ToArray());
        }
    }
}
