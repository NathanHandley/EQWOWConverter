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

using EQWOWConverter.ModelObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class M2TextureArray
    {
        private List<M2Texture> Textures = new List<M2Texture>();
        private string TextureFolder;

        public M2TextureArray(string textureFolder)
        {
            TextureFolder = textureFolder;
        }

        public void AddModelTextures(List<ModelTexture> modelTextures)
        {
            foreach(ModelTexture texture in modelTextures)
            {
                Textures.Add(new M2Texture(texture, TextureFolder));
            }
        }
    }
}
