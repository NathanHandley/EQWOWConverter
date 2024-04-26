using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Common
{
    internal class Fog
    {
        public bool HasInfiniteRadius = false;
        public Vector3 Position = new Vector3();
        public float NearRadius = 0;
        public float FarRadius = 0;
        public float NormalStartScalar = 0.25f;
        public float NormalEnd = 444.4445f;
        public ColorRGBA NormalColor = new ColorRGBA(255, 255, 255, 255);
        public float UnderwaterStartScalar = -0.5f;
        public float UnderwaterEnd = 222.2222f;
        public ColorRGBA UnderwaterColor = new ColorRGBA(255, 255, 255, 255);

        public List<byte> ToBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(HasInfiniteRadius ? 0x01 : 0x00)));
            returnBytes.AddRange(Position.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(NearRadius));
            returnBytes.AddRange(BitConverter.GetBytes(FarRadius));
            returnBytes.AddRange(BitConverter.GetBytes(NormalEnd));
            returnBytes.AddRange(BitConverter.GetBytes(NormalStartScalar));
            returnBytes.AddRange(NormalColor.ToBytes());
            returnBytes.AddRange(BitConverter.GetBytes(UnderwaterEnd));
            returnBytes.AddRange(BitConverter.GetBytes(UnderwaterStartScalar));
            returnBytes.AddRange(UnderwaterColor.ToBytes());
            return returnBytes;
        }
    }
}
