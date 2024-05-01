using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    // Low resolution version of the axis aligned box
    internal class AxisAlignedBoxLR
    {
        private Int16 TopX;
        private Int16 TopY;
        private Int16 TopZ;
        private Int16 BottomX;
        private Int16 BottomY;
        private Int16 BottomZ;

        public AxisAlignedBoxLR()
        {

        }
        public AxisAlignedBoxLR(AxisAlignedBox axisAlignedBox)
        {
            TopX = Convert.ToInt16(Math.Round(axisAlignedBox.TopCorner.X, 0, MidpointRounding.AwayFromZero));
            TopY = Convert.ToInt16(Math.Round(axisAlignedBox.TopCorner.Y, 0, MidpointRounding.AwayFromZero));
            TopZ = Convert.ToInt16(Math.Round(axisAlignedBox.TopCorner.Z, 0, MidpointRounding.AwayFromZero));
            BottomX = Convert.ToInt16(Math.Round(axisAlignedBox.BottomCorner.X, 0, MidpointRounding.AwayFromZero));
            BottomY = Convert.ToInt16(Math.Round(axisAlignedBox.BottomCorner.Y, 0, MidpointRounding.AwayFromZero));
            BottomZ = Convert.ToInt16(Math.Round(axisAlignedBox.BottomCorner.Z, 0, MidpointRounding.AwayFromZero));
        }
        public AxisAlignedBoxLR(short topX, short topY, short topZ, short bottomX, short bottomY, short bottomZ)
        {
            TopX = topX;
            TopY = topY;
            TopZ = topZ;
            BottomX = bottomX;
            BottomY = bottomY;
            BottomZ = bottomZ;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(BottomX));
            returnBytes.AddRange(BitConverter.GetBytes(BottomY));
            returnBytes.AddRange(BitConverter.GetBytes(BottomZ));
            returnBytes.AddRange(BitConverter.GetBytes(TopX));
            returnBytes.AddRange(BitConverter.GetBytes(TopY));
            returnBytes.AddRange(BitConverter.GetBytes(TopZ));
            return returnBytes;
        }
    }
}
