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
    internal class LightInstance
    {
        public Vector3 Position = new Vector3();
        public float Radius;
        public ColorRGBA Color = new ColorRGBA();

        public void SetColor(float R, float G, float B)
        {
            Color.R = Convert.ToByte(255f * R);
            Color.G = Convert.ToByte(255f * G);
            Color.B = Convert.ToByte(255f * B);
        }

        public List<byte> ToBytes()
        {
            // Calculate attentuation, factoring for world scale
            float attentuationEnd = Radius * Configuration.CONFIG_GENERATE_WORLD_SCALE;
            float attentuationStart = Configuration.CONFIG_LIGHT_INSTANCE_ATTENUATION_START_PROPORTION * attentuationEnd;

            // Write the bytes
            List<byte> returnBytes = new List<byte>();
            returnBytes.Add(0); // LightType (Omni[0], Spot[1], Directional[2], Ambient[3]).  All EQ lights are omni (point) lights
            returnBytes.Add(1); // If true, use attenuation, otherwise don't.
            returnBytes.Add(1); // Padding / Unknown Use
            returnBytes.Add(1); // Padding / Unknown Use
            returnBytes.AddRange(Color.ToBytesBGRA()); // Color of the light
            returnBytes.AddRange(Position.ToBytes()); // Light position
            returnBytes.AddRange(BitConverter.GetBytes(1.0f)); // Intensity.  Not 100% sure on the value range, but I see 0.6 a lot so assuming 0-1
            returnBytes.AddRange(BitConverter.GetBytes(0f)); // Unknown 1 (Start Radius 1?)
            returnBytes.AddRange(BitConverter.GetBytes(0f)); // Unknown 2 (End Radius 1?)
            returnBytes.AddRange(BitConverter.GetBytes(-1f)); // Unknown 3 (Start Radius 2?) - -1 is common in existing files
            returnBytes.AddRange(BitConverter.GetBytes(-0.5f)); // Unknown 4 (End Radius 2?) - -0.5 is common in existing files
            returnBytes.AddRange(BitConverter.GetBytes(attentuationStart)); // Attenuation Start. (3.305556f is a common value)
            returnBytes.AddRange(BitConverter.GetBytes(attentuationEnd)); // Attenuation End. (11.19444f is a common value)
            return returnBytes;
        }

    }
}
