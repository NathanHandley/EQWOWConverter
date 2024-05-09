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
