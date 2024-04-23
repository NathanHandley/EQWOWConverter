using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class AxisAlignedBox
    {
        public Vector3 TopCorner = new Vector3();
        public Vector3 BottomCorner = new Vector3();

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.X));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(BottomCorner.Z));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.X));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Y));
            returnBytes.AddRange(BitConverter.GetBytes(TopCorner.Z));
            return returnBytes;
        }
    }
}
