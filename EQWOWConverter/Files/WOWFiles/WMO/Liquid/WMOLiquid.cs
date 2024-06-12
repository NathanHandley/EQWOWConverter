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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal class WMOLiquid
    {
        // Header
        public Int32 XVertexCount = 0;
        public Int32 YVertexCount = 0;
        public Int32 XTileCount = 0;
        public Int32 YTileCount = 0;
        public Vector3 CornerPosition = new Vector3();
        public UInt16 MaterialID = 0;

        // Data
        public List<WMOWaterVert> WaterVerts = new List<WMOWaterVert>();
        public List<WMOMagmaVert> MagmaVerts = new List<WMOMagmaVert>();
        //public List<WMOLiquidTile> Tiles = new List<WMOLiquidTile>();
        public List<WMOLiquidFlags> TileFlags = new List<WMOLiquidFlags>();

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();

            // Header
            returnBytes.AddRange(BitConverter.GetBytes(XVertexCount));
            returnBytes.AddRange(BitConverter.GetBytes(YVertexCount));
            returnBytes.AddRange(BitConverter.GetBytes(XTileCount));
            returnBytes.AddRange(BitConverter.GetBytes(YTileCount));
            returnBytes.AddRange(CornerPosition.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(MaterialID));

            // Data - Note that it will only ever be one or the other, so no union is required
            foreach (WMOWaterVert vert in WaterVerts)
                returnBytes.AddRange(vert.ToBytes());
            foreach (WMOMagmaVert vert in MagmaVerts)
                returnBytes.AddRange(vert.ToBytes());
            foreach (WMOLiquidFlags tileFlags in TileFlags)
                returnBytes.Add(Convert.ToByte(tileFlags));
            return returnBytes;
        }
    }
}
