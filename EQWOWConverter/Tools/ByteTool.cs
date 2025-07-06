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
using System.Text;

namespace EQWOWConverter
{
    internal class ByteTool
    {
        public static short ReadInt16FromBytes(List<byte> sourceBytes, ref int byteCursor, bool doAdvanceCursor = true)
        {
            if (byteCursor + 2 >= sourceBytes.Count)
            {
                Logger.WriteError("ReadInt16FromBytes error, value would be out of bounds");
                return 0;
            }
            byte[] intBytes = new byte[2];
            for (int i = 0; i < 2; i++)
                intBytes[i] = sourceBytes[byteCursor + i];

            if (doAdvanceCursor == true)
                byteCursor += 2;

            return BitConverter.ToInt16(intBytes, 0);
        }

        public static int ReadInt32FromBytes(List<byte> sourceBytes, ref int byteCursor, bool doAdvanceCursor = true)
        {
            if (byteCursor + 4 >= sourceBytes.Count)
            {
                Logger.WriteError("ReadInt32FromBytes error, value would be out of bounds");
                return 0;
            }
            byte[] intBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                intBytes[i] = sourceBytes[byteCursor + i];
            
            if (doAdvanceCursor == true)
                byteCursor += 4;

            return BitConverter.ToInt32(intBytes, 0);
        }

        public static float ReadFloatFromBytes(List<byte> sourceBytes, ref int byteCursor, bool doAdvanceCursor = true)
        {
            if (byteCursor + 4 >= sourceBytes.Count)
            {
                Logger.WriteError("ReadFloatFromBytes error, value would be out of bounds");
                return 0;
            }
            byte[] floatBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                floatBytes[i] = sourceBytes[byteCursor + i];

            if (doAdvanceCursor == true)
                byteCursor += 4;

            return BitConverter.ToSingle(floatBytes, 0);
        }

        public static string ReadStringFromBytes(List<byte> sourceBytes, ref int byteCursor, int maxStringLength, bool doAdvanceCursor = true)
        {
            if (byteCursor + maxStringLength >= sourceBytes.Count)
            {
                Logger.WriteError("ReadStringFromBytes error, value would be out of bounds");
                return string.Empty;
            }
            int byteEndIndex = byteCursor + maxStringLength;
            int firstNullIndex = sourceBytes.IndexOf(0, byteCursor, Math.Min(maxStringLength, sourceBytes.Count - byteCursor));
            if (firstNullIndex == -1 || firstNullIndex >= byteEndIndex)
                firstNullIndex = byteEndIndex;
            string extractedString = Encoding.ASCII.GetString(sourceBytes.GetRange(byteCursor, firstNullIndex - byteCursor).ToArray());
            
            if (doAdvanceCursor == true)
                byteCursor += maxStringLength;

            return extractedString;
        }

        public static ColorRGBA ReadColorBGRAFromBytes(List<byte> sourceBytes, ref int byteCursor, bool doAdvanceCursor = true)
        {
            if (byteCursor + 4 >= sourceBytes.Count)
            {
                Logger.WriteError("ReadColorBGRAFromBytes error, value would be out of bounds");
                return new ColorRGBA();
            }
            byte[] intBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                intBytes[i] = sourceBytes[byteCursor + i];
            ColorRGBA colorRGBA = new ColorRGBA();
            colorRGBA.B = sourceBytes[0];
            colorRGBA.G = sourceBytes[1];
            colorRGBA.R = sourceBytes[2];
            colorRGBA.A = sourceBytes[3];

            if (doAdvanceCursor == true)
                byteCursor += 4;

            return colorRGBA;
        }
    }
}
