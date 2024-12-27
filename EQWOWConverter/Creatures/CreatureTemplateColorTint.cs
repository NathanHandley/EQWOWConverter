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

namespace EQWOWConverter.Creatures
{
    internal class CreatureTemplateColorTint
    {
        private static Dictionary<int, CreatureTemplateColorTint> CreatureTemplateColorTints = new Dictionary<int, CreatureTemplateColorTint>();

        public int ID = 0;
        public string Name = string.Empty;
        public int HelmRed = 0;
        public int HelmGreen = 0;
        public int HelmBlue = 0;
        public int ChestRed = 0;
        public int ChestGreen = 0;
        public int ChestBlue = 0;
        public int ArmsRed = 0;
        public int ArmsGreen = 0;
        public int ArmsBlue = 0;
        public int BracerRed = 0;
        public int BracerGreen = 0;
        public int BracerBlue = 0;
        public int HandsRed = 0;
        public int HandsGreen = 0;
        public int HandsBlue = 0;
        public int LegsRed = 0;
        public int LegsGreen = 0;
        public int LegsBlue = 0;
        public int FeetRed = 0;
        public int FeetGreen = 0;
        public int FeetBlue = 0;

        public static Dictionary<int, CreatureTemplateColorTint> GetCreatureTemplateColorTints()
        {
            if (CreatureTemplateColorTints.Count == 0)
                PopulateCreatureTemplateColorTints();
            return CreatureTemplateColorTints;
        }

        private static void PopulateCreatureTemplateColorTints()
        {
            CreatureTemplateColorTints.Clear();

            string creatureTemplateColorTintsFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplateColors.csv");
            Logger.WriteDetail("Populating Creature Template Color Tint list via file '" + creatureTemplateColorTintsFile + "'");
            string inputData = File.ReadAllText(creatureTemplateColorTintsFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureTemplatesColorTint list via file '" + creatureTemplateColorTintsFile + "' did not have enough rows");
                return;
            }

            // Load all of the data
            bool headerRow = true;
            foreach (string row in inputRows)
            {
                // Handle first row
                if (headerRow == true)
                {
                    headerRow = false;
                    continue;
                }

                // Skip blank rows
                if (row.Trim().Length == 0)
                    continue;

                // Load the row
                string[] rowBlocks = row.Split(",");
                CreatureTemplateColorTint colorTint = new CreatureTemplateColorTint();
                colorTint.ID = int.Parse(rowBlocks[0]);
                colorTint.Name = rowBlocks[1];
                colorTint.HelmRed = int.Parse(rowBlocks[2]);
                colorTint.HelmGreen = int.Parse(rowBlocks[3]);
                colorTint.HelmBlue = int.Parse(rowBlocks[4]);
                colorTint.ChestRed = int.Parse(rowBlocks[5]);
                colorTint.ChestGreen = int.Parse(rowBlocks[6]);
                colorTint.ChestBlue = int.Parse(rowBlocks[7]);
                colorTint.ArmsRed = int.Parse(rowBlocks[8]);
                colorTint.ArmsGreen = int.Parse(rowBlocks[9]);
                colorTint.ArmsBlue = int.Parse(rowBlocks[10]);
                colorTint.BracerRed = int.Parse(rowBlocks[11]);
                colorTint.BracerGreen = int.Parse(rowBlocks[12]);
                colorTint.BracerBlue = int.Parse(rowBlocks[13]);
                colorTint.HandsRed = int.Parse(rowBlocks[14]);
                colorTint.HandsGreen = int.Parse(rowBlocks[15]);
                colorTint.HandsBlue = int.Parse(rowBlocks[16]);
                colorTint.LegsRed = int.Parse(rowBlocks[17]);
                colorTint.LegsGreen = int.Parse(rowBlocks[18]);
                colorTint.LegsBlue = int.Parse(rowBlocks[19]);
                colorTint.FeetRed = int.Parse(rowBlocks[20]);
                colorTint.FeetGreen = int.Parse(rowBlocks[21]);
                colorTint.FeetBlue = int.Parse(rowBlocks[22]);
                if (CreatureTemplateColorTints.ContainsKey(colorTint.ID) == true)
                    Logger.WriteError("Unable to add creature template color tint with ID '" + colorTint.ID.ToString() + "' as that ID already exists in the collection");
                else
                    CreatureTemplateColorTints.Add(colorTint.ID, colorTint);
            }
        }
    }
}
