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

namespace EQWOWConverter.WOWFiles
{
    internal class M2SkinBoneIndicies
    {
        public List<byte> BoneIndicies = new List<byte>(new byte[4]);   // Any more than 4 elements will be ignored

        public M2SkinBoneIndicies(List<byte> boneIndicies)
        {
            if (boneIndicies.Count == 4)
                for (int i = 0; i < boneIndicies.Count; i++)
                    BoneIndicies[i] = boneIndicies[i];
        }

        public List<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BoneIndicies.ToArray());
            return bytes;
        }
    }
}
