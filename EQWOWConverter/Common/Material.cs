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
using System.Xml.Linq;

namespace EQWOWConverter.Common
{
    internal class Material
    {
        public uint Index = 0;
        public MaterialType MaterialType = MaterialType.Diffuse;
        public string Name = string.Empty;
        public readonly List<string> SourceTextureNameArray = new List<string>();
        public string TextureName = string.Empty;
        public uint AnimationDelayMs = 0;

        public Material(string nameAndOriginalTexturesNameBlock)
        {
            string[] parts = nameAndOriginalTexturesNameBlock.Split(':');

            // First block is always the name
            if (parts[0].Contains("_"))
            {
                string[] nameAttributeSplit = parts[0].Split('_');
                switch (nameAttributeSplit[0])
                {
                    case "d": MaterialType = MaterialType.Diffuse; break;
                    case "i": MaterialType = MaterialType.Invisible; break;
                    case "b": MaterialType = MaterialType.Boundary; break;
                    case "t25": MaterialType = MaterialType.Transparent25Percent; break;
                    case "t50": MaterialType = MaterialType.Transparent50Percent; break;
                    case "t75": MaterialType = MaterialType.Transparent75Percent; break;
                    case "ta": MaterialType = MaterialType.TransparentAdditive; break;
                    case "tau": MaterialType = MaterialType.TransparentAdditiveUnlit; break;
                    case "tm": MaterialType = MaterialType.TransparentMasked; break;
                    case "ds": MaterialType = MaterialType.DiffuseSkydome; break;
                    case "ts": MaterialType = MaterialType.TransparentSkydome; break;
                    case "taus": MaterialType = MaterialType.Boundary; break;
                    default:
                        {
                            Logger.WriteLine("Error, Material had a name of " + parts[0] + " which doesn't map to a type");
                            MaterialType = MaterialType.Diffuse;
                        }
                        break;
                }
            }
            Name = parts[0];

            // If there are more blocks, they are the textures
            if (parts.Length > 1)
                for (int i = 1; i < parts.Length; i++)
                    SourceTextureNameArray.Add(parts[i]);

            // Default the texture name to the first
            if (SourceTextureNameArray.Count > 0)
                TextureName = SourceTextureNameArray[0];
        }

        public bool IsAnimated()
        {
            if (AnimationDelayMs > 0)
                return true;
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
            if (SourceTextureNameArray.Count == 0)
                return false;
            return true;
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
    }
}
