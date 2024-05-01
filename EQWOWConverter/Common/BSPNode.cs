using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class BSPNode
    {
        public BSPNodeFlag Flags;
        public Int16 NegChild; // Right side, negative
        public Int16 PosChild; // Left side, positive
        public UInt16 NumFaces; // Number of faces (found in WMO MOBR)
        public UInt32 FaceStartIndex; // first triangle (found in WMO MOBR)
        public float PlaneDistance;

        public BSPNode() { }
        public BSPNode(BSPNodeFlag singleFlag, short negChild, short posChild, ushort numFaces, uint faceStartIndex, float planeDistance)
        {
            Flags = singleFlag;
            NegChild = negChild;
            PosChild = posChild;
            NumFaces = numFaces;
            FaceStartIndex = faceStartIndex;
            PlaneDistance = planeDistance;
        }

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Flags)));
            returnBytes.AddRange(BitConverter.GetBytes(NegChild));
            returnBytes.AddRange(BitConverter.GetBytes(PosChild));
            returnBytes.AddRange(BitConverter.GetBytes(NumFaces));
            returnBytes.AddRange(BitConverter.GetBytes(FaceStartIndex));
            returnBytes.AddRange(BitConverter.GetBytes(PlaneDistance));
            return returnBytes;
        }
    }
}
