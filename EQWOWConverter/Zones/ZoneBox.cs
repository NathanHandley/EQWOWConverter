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
    internal class ZoneBox
    {
        public Material SelectedMaterial;
        public MeshData MeshData = new MeshData();

        public ZoneBox(BoundingBox boundingBox, List<Material> materials, string zoneShortName, float addedSize, MeshBoxRenderType renderType)
        {
            // Find a material that can be used by looking for something opaque
            Material? selectedMaterial = null;
            foreach (Material material in materials)
            {
                if (material.HasTransparency() == false && material.IsRenderable() == true && material.IsAnimated() == false)
                {
                    selectedMaterial = material;
                    break;
                }
            }
            if (selectedMaterial == null)
            {
                Logger.WriteError("Error, no suitable material found for box for zone shortname '" + zoneShortName + "', box may not render properly");
                selectedMaterial = new Material();
            }
            SelectedMaterial = selectedMaterial;
            int materialIndex = Convert.ToInt32(SelectedMaterial.Index);

            // Expand the bounding box by any added size
            boundingBox.TopCorner.X += addedSize;
            boundingBox.TopCorner.Y += addedSize;
            boundingBox.TopCorner.Z += addedSize;
            boundingBox.BottomCorner.X -= addedSize;
            boundingBox.BottomCorner.Y -= addedSize;
            boundingBox.BottomCorner.Z -= addedSize;

            // Generate a box
            MeshData.GenerateAsBox(boundingBox, materialIndex, renderType);
        }
    }
}
