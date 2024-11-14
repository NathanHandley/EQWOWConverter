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

using EQWOWConverter.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.WOWFiles
{
    internal class M2CameraArrayByOffset : IOffsetByteSerializable
    {
        private UInt32 Count = 0;
        private UInt32 Offset = 0;
        private List<M2Camera> Cameras = new List<M2Camera>();

        public void AddCamera(M2Camera camera)
        {
            Cameras.Add(camera);
            Count++;
        }

        public List<Byte> GetHeaderBytes()
        {
            List<byte> returnBytes = new List<byte>();
            returnBytes.AddRange(BitConverter.GetBytes(Count));
            returnBytes.AddRange(BitConverter.GetBytes(Offset));
            return returnBytes;
        }

        public void AddDataBytes(ref List<byte> byteBuffer)
        {
            // Update header bytes
            if (Count == 0)
            {
                Offset = 0;
                return;
            }
            Offset = Convert.ToUInt32(byteBuffer.Count);

            // Determine space needed for all camera headers and reserve it
            UInt32 allCameraSubHeaderSize = 0;
            foreach (M2Camera camera in Cameras)
                allCameraSubHeaderSize += camera.GetHeaderSize();
            for (int i = 0; i < allCameraSubHeaderSize; i++)
                byteBuffer.Add(0);
            AddBytesToAlign(ref byteBuffer, 16);

            // Add all of the data
            foreach (M2Camera camera in Cameras)
                camera.AddDataBytes(ref byteBuffer);

            // Write the header data
            List<byte> headerBytes = new List<byte>();
            foreach (M2Camera camera in Cameras)
                headerBytes.AddRange(camera.GetHeaderBytes());
            for (int i = 0; i < allCameraSubHeaderSize; i++)
                byteBuffer[i + Convert.ToInt32(Offset)] = headerBytes[i];
        }

        private void AddBytesToAlign(ref List<byte> byteBuffer, int byteAlignMultiplier)
        {
            int bytesToAdd = byteAlignMultiplier - (byteBuffer.Count % byteAlignMultiplier);
            if (bytesToAdd == byteAlignMultiplier)
                return;
            for (int i = 0; i < bytesToAdd; ++i)
                byteBuffer.Add(0);
        }
    }
}
