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

using EQWOWConverter.Common;
using EQWOWConverter.ModelObjects;
using EQWOWConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EQWOWConverter.Common
{
    internal class Material
    {
        public UInt32 Index = 0;
        public MaterialType MaterialType = MaterialType.Diffuse;
        public string Name = string.Empty;
        public string UniqueName = string.Empty;
        public List<string> TextureNames = new List<string>();
        public UInt32 AnimationDelayMs = 0;
        public int TextureWidth = 0;
        public int TextureHeight = 0;

        public Material() { }

        public Material(Material material)
        {
            Index = material.Index;
            MaterialType = material.MaterialType;
            Name = material.Name;
            UniqueName = material.UniqueName;
            foreach (string textureName in material.TextureNames)
                TextureNames.Add(textureName);
            AnimationDelayMs = material.AnimationDelayMs;
            TextureWidth = material.TextureWidth;
            TextureHeight = material.TextureHeight;
        }

        public Material(string name, string originalName, UInt32 index, MaterialType materialType, List<string> textureNames,
            UInt32 animationDelayMS, int sourceTextureWidth, int sourceTextureHeight)
        {
            UniqueName = name;
            Name = originalName;
            Index = index;
            MaterialType = materialType;
            TextureNames = textureNames;
            AnimationDelayMs = animationDelayMS;
            TextureWidth = sourceTextureWidth;
            TextureHeight = sourceTextureHeight;
        }

        public bool IsAnimated()
        {
            if (AnimationDelayMs > 0)
                return true;
            else
                return false;
        }

        public bool HasTransparency()
        {
            if (MaterialType == MaterialType.Invisible ||
                MaterialType == MaterialType.Boundary ||
                MaterialType == MaterialType.Transparent25Percent ||
                MaterialType == MaterialType.Transparent50Percent ||
                MaterialType == MaterialType.Transparent75Percent ||
                MaterialType == MaterialType.TransparentAdditive ||
                MaterialType == MaterialType.TransparentAdditiveUnlit ||
                MaterialType == MaterialType.TransparentMasked ||
                MaterialType == MaterialType.TransparentSkydome ||
                MaterialType == MaterialType.TransparentAdditiveUnlitSkydome)
            {
                return true;
            }
            else
                return false;
        }

        public bool IsRenderable()
        {
            if (MaterialType == MaterialType.Invisible)
                return false;
            if (MaterialType == MaterialType.Boundary)
                return false;
            if (MaterialType == MaterialType.DiffuseSkydome)
                return false;
            if (MaterialType == MaterialType.TransparentSkydome)
                return false;
            if (TextureNames.Count == 0)
                return false;
            return true;
        }

        public int NumOfAnimationFrames()
        {
            return TextureNames.Count();
        }

        public string GetTextureSuffix()
        {
            switch (MaterialType)
            {
                case MaterialType.Transparent25Percent: return "a25";
                case MaterialType.Transparent50Percent: return "a50";
                case MaterialType.Transparent75Percent: return "a75";
                default: return "";
            }
        }

        // This returns an animation-aware and half-pixel corrected (on edge) coordinate set
        public TextureCoordinates GetCorrectedBaseCoordinates(TextureCoordinates uncorrectedCoordinates)
        {
            TextureCoordinates correctedCoordinates = new TextureCoordinates();

            // u(x) == 0, half-pixel correct
            if (uncorrectedCoordinates.X <= float.Epsilon && uncorrectedCoordinates.X >= float.Epsilon)
            {
                correctedCoordinates.X = 0.5f / Convert.ToSingle(TextureWidth);
            }
            // u(x) == 1, half-pixel correct and factor animation
            else if (uncorrectedCoordinates.X <= (1.0f + float.Epsilon) && uncorrectedCoordinates.X >= (1.0f - float.Epsilon))
            {
                float workingXCoordinate = 1.0f - (0.5f / Convert.ToSingle(TextureWidth));
                correctedCoordinates.X = workingXCoordinate;
            }
            // u(x) == -1 half-pixel correct and factor animation
            else if (uncorrectedCoordinates.X >= (-1.0f - float.Epsilon) && uncorrectedCoordinates.X <= (-1.0f + float.Epsilon))
            {
                float workingXCoordinate = -1.0f + (0.5f / Convert.ToSingle(TextureWidth));
                correctedCoordinates.X = workingXCoordinate;
            }
            // Other cases are just animation factor (revisit if there are 'even' 2x and 3x+ coordinate cases
            else
            {
                correctedCoordinates.X = uncorrectedCoordinates.X;
            }

            // u(y) == 0, half-pixel correct
            if (uncorrectedCoordinates.Y <= float.Epsilon && uncorrectedCoordinates.Y >= float.Epsilon)
            {
                correctedCoordinates.Y = 0.5f / Convert.ToSingle(TextureHeight);
            }
            // u(y) == 1, half-pixel correct and factor animation
            else if (uncorrectedCoordinates.Y <= (1.0f + float.Epsilon) && uncorrectedCoordinates.Y >= (1.0f - float.Epsilon))
            {
                float workingYCoordinate = 1.0f - (0.5f / Convert.ToSingle(TextureHeight));
                correctedCoordinates.Y = workingYCoordinate;
            }
            // u(y) == -1 half-pixel correct and factor animation
            else if (uncorrectedCoordinates.Y >= (-1.0f - float.Epsilon) && uncorrectedCoordinates.Y <= (-1.0f + float.Epsilon))
            {
                float workingYCoordinate = -1.0f + (0.5f / Convert.ToSingle(TextureHeight));
                correctedCoordinates.Y = workingYCoordinate;
            }
            // Other cases are just animation factor (revisit if there are 'even' 2x and 3x+ coordinate cases
            else
            {
                correctedCoordinates.Y = uncorrectedCoordinates.Y;
            }

            return correctedCoordinates;
        }

        public short GetTransparencyValue()
        {
            // Calculated by 32767 / (percent)
            switch (MaterialType)
            {
                case MaterialType.Transparent25Percent: return 8192;
                case MaterialType.Transparent50Percent: return 16384;
                case MaterialType.Transparent75Percent: return 24575;
                default: return Int16.MaxValue;
            }
        }
    }
}