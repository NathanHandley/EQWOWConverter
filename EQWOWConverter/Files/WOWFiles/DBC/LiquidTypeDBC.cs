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

namespace EQWOWConverter.WOWFiles.DBC
{
    internal class LiquidTypeDBC
    {
        public void WriteToDisk(string baseFolderPath)
        {
            FileTool.CreateBlankDirectory(baseFolderPath, true);
            string fullFilePath = Path.Combine(baseFolderPath, "LiquidTypeDBC.csv");

            // Data is hard coded and not calculated, so just write that
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"ID\",\"Name\",\"Flags\",\"Type\",\"SoundID\",\"SpellID\",\"MaxDarkenDepth\",\"FogDarkenintensity\",\"AmbDarkenintensity\",\"DirDarkenintensity\",\"LightID\",\"ParticleScale\",\"ParticleMovement\",\"ParticleTexSlots\",\"MaterialID\",\"Texture_1\",\"Texture_2\",\"Texture_3\",\"Texture_4\",\"Texture_5\",\"Texture_6\",\"Color_1\",\"Color_2\",\"Float_1\",\"Float_2\",\"Float_3\",\"Float_4\",\"Float_5\",\"Float_6\",\"Float_7\",\"Float_8\",\"Float_9\",\"Float_10\",\"Float_11\",\"Float_12\",\"Float_13\",\"Float_14\",\"Float_15\",\"Float_16\",\"Float_17\",\"Float_18\",\"Int_1\",\"Int_2\",\"Int_3\",\"Int_4\"");
            stringBuilder.AppendLine("\"200\",\"EverQuest Water\",\"15\",\"0\",\"1111\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"1\",\"0\",\"1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"1250\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"201\",\"EverQuest Green Water\",\"15\",\"0\",\"1111\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"1\",\"0\",\"1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"1250\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"202\",\"EverQuest Magma\",\"120\",\"2\",\"3072\",\"0\",\"0\",\"0\",\"0\",\"0\",\"7\",\"4\",\"1\",\"1\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"0\",\"0.1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"203\",\"EverQuest Slime\",\"322\",\"3\",\"3880\",\"0\",\"0\",\"0\",\"0\",\"0\",\"6\",\"0\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"0\",\"0.1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"204\",\"EverQuest Blood Water\",\"15\",\"0\",\"1111\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"1\",\"0\",\"1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"1250\",\"0\",\"0\"");

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
