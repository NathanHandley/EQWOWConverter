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
    internal class AnimatedVerticies
    {
        public List<List<Vector3>> Frames = new List<List<Vector3>>();
        public int FrameDelay = 0;

        public int GetFrameCount()
        {
            return Frames.Count;
        }

        public List<Vector3> GetVerticiesAtFrame(int frame)
        {
            if (frame >= Frames.Count)
                return new List<Vector3>();
            else
                return Frames[frame];
        }
    }
}
