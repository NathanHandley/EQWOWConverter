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
    internal class Material
    {
        public uint Index = 0;
        public MaterialType MaterialType = MaterialType.Diffuse;
        public string Name = string.Empty;
        public List<string> AnimationTextures = new List<string>();
        public uint AnimationDelayMs = 0;

        public Material(string name)
        {
            if (name.Contains("_"))
            {
                string[] parts = name.Split('_');
                switch (parts[0])
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
                            Logger.WriteLine("Error, Material had a name of " + name + " which doesn't map to a type");
                            MaterialType = MaterialType.Diffuse;
                        } break;
                }
                string[] subParts = parts[1].Split(":");
                Name = subParts[0];
            }
            else
            {
                string[] subParts = name.Split(":");
                Name = subParts[0];
            }
        }

        public bool IsAnimated()
        {
            if (AnimationDelayMs > 0 && AnimationTextures.Count > 1)
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
            if (AnimationTextures.Count == 0)
                return false;
            return true;
        }
    }
}
