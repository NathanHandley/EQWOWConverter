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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Zones
{
    internal class ZoneLiquidGroup
    {
        private List<ZoneLiquid> LiquidChunks = new List<ZoneLiquid>();
        public BoundingBox BoundingBox = new BoundingBox();
        public string ForcedAreaAssignmentName = string.Empty;

        public void AddLiquidChunk(ZoneLiquid liquid)
        {
            LiquidChunks.Add(liquid);
            RebuildBoundingBox();
        }

        private void RebuildBoundingBox()
        {
            List<BoundingBox> chunkBoxes = new List<BoundingBox>();
            foreach (ZoneLiquid chunk in LiquidChunks)
                chunkBoxes.Add(new BoundingBox(chunk.BoundingBox));
            BoundingBox = BoundingBox.GenerateBoxFromBoxes(chunkBoxes);
        }        

        public List<ZoneLiquid> GetLiquidChunks()
        {
            return LiquidChunks;
        }

        public void ClearLiquidChunks()
        {
            LiquidChunks.Clear();
        }

        public void DeleteLiquidChunkAtIndex(int index)
        {
            if (index >= LiquidChunks.Count)
                throw new Exception("Attempted to delete a liquid chunk at an invalid index");
            LiquidChunks.RemoveAt(index);
        }
    }
}
