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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    class SplitBox
    {
        public BoundingBox BoxA = new BoundingBox();
        public BoundingBox BoxB = new BoundingBox();
        public float PlaneDistance;

        public static SplitBox GenerateXYSplitBoxFromBoundingBox(BoundingBox box)
        {
            SplitBox splitBox = new SplitBox();
            splitBox.BoxA = new BoundingBox(box);
            splitBox.BoxB = new BoundingBox(box);

            if (box.GetXDistance() > box.GetYDistance())
            {
                float planeSplitDistance = (box.TopCorner.X + box.BottomCorner.X) * 0.5f;
                splitBox.PlaneDistance = planeSplitDistance;
                splitBox.BoxA.TopCorner.X = planeSplitDistance;
                splitBox.BoxB.BottomCorner.X = planeSplitDistance;
            }
            else
            {
                float planeSplitDistance = (box.TopCorner.Y + box.BottomCorner.Y) * 0.5f;
                splitBox.PlaneDistance = planeSplitDistance;
                splitBox.BoxA.TopCorner.Y = planeSplitDistance;
                splitBox.BoxB.BottomCorner.Y = planeSplitDistance;
            }

            return splitBox;
        }
    }
}
