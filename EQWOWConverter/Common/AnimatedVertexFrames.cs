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

namespace EQWOWConverter.Common
{
    internal class AnimatedVertexFrames
    {
        public class Vector3StringLiteral
        {
            public Vector3StringLiteral(string x, string y, string z)
            {
                XString = x;
                YString = y;
                ZString = z;
            }
            public Vector3StringLiteral(Vector3StringLiteral other)
            {
                XString = other.XString;
                YString = other.YString;
                ZString = other.ZString;
            }

            public string XString = string.Empty;
            public string YString = string.Empty;
            public string ZString = string.Empty;
        }

        public List<Vector3> VertexOffsetFrames = new List<Vector3>();
        public List<Vector3StringLiteral> VertexOffsetFramesInStringLiteral = new List<Vector3StringLiteral>();

        public AnimatedVertexFrames() { }
    }
}
