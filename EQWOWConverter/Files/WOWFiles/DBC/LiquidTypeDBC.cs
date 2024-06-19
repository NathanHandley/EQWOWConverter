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
            stringBuilder.AppendLine("\"200\",\"EverQuest Water\",\"15\",\"0\",\"1111\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"1\",\"0\",\"1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"1250\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"201\",\"EverQuest Ocean\",\"527\",\"1\",\"1114\",\"0\",\"30\",\"0.5\",\"0.5\",\"0.25\",\"0\",\"1\",\"0\",\"0\",\"1\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"0.25\",\"180\",\"1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"1\",\"1250\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"202\",\"EverQuest Magma\",\"120\",\"2\",\"3072\",\"0\",\"0\",\"0\",\"0\",\"0\",\"7\",\"4\",\"1\",\"1\",\"2\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"0\",\"0.1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\"");
            stringBuilder.AppendLine("\"203\",\"EverQuest Slime\",\"322\",\"3\",\"3880\",\"0\",\"0\",\"0\",\"0\",\"0\",\"6\",\"0\",\"0\",\"0\",\"2\",\"XTextures\\everquest\\eqclear.%d.blp\",\"\",\"\",\"\",\"\",\"\",\"0\",\"0\",\"0\",\"0.1\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\"");

            // Output it
            File.WriteAllText(fullFilePath, stringBuilder.ToString());
        }
    }
}
