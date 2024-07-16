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
    internal class PlaneAxisAlignedXY
    {
        public float HighZ;
        public float LowZ;
        public LiquidSlantType SlantType = LiquidSlantType.None;
        public Vector2 NWCornerXY = new Vector2();
        public Vector2 SECornerXY = new Vector2();

        public PlaneAxisAlignedXY() { }

        public PlaneAxisAlignedXY(PlaneAxisAlignedXY other)
        {
            HighZ = other.HighZ;
            LowZ = other.LowZ;
            SlantType = other.SlantType;
            NWCornerXY = new Vector2(other.NWCornerXY);
            SECornerXY = new Vector2(other.SECornerXY);
        }

        public PlaneAxisAlignedXY(float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float highZ, float lowZ, LiquidSlantType slantType)
        {
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            HighZ = highZ;
            LowZ = lowZ;
            SlantType = slantType;
        }

        public PlaneAxisAlignedXY(float nwCornerX, float nwCornerY, float seCornerX, float seCornerY,
            float fixedZ)
        {
            NWCornerXY.X = nwCornerX;
            NWCornerXY.Y = nwCornerY;
            SECornerXY.X = seCornerX;
            SECornerXY.Y = seCornerY;
            HighZ = fixedZ;
            LowZ = fixedZ;
        }

        public float GetXDistance()
        {
            if (NWCornerXY.X > SECornerXY.X)
                return MathF.Abs(NWCornerXY.X - SECornerXY.X);
            else
                return MathF.Abs(SECornerXY.X - NWCornerXY.X);
        }

        public float GetYDistance()
        {
            if (NWCornerXY.Y > SECornerXY.Y)
                return MathF.Abs(NWCornerXY.Y - SECornerXY.Y);
            else
                return MathF.Abs(SECornerXY.Y - NWCornerXY.Y);
        }
    }
}
