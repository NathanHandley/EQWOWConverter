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

using EQWOWConverter.Common;

namespace EQWOWConverter.Creatures
{
    internal class CreatureTemplateColorTint
    {
        private static Dictionary<int, CreatureTemplateColorTint> CreatureTemplateColorTints = new Dictionary<int, CreatureTemplateColorTint>();

        public int ID = 0;
        public string Name = string.Empty;
        public ColorRGBA? HelmColor = null;
        public ColorRGBA? ChestColor = null;
        public ColorRGBA? ArmsColor = null;
        public ColorRGBA? BracerColor = null;
        public ColorRGBA? HandsColor = null;
        public ColorRGBA? LegsColor = null;
        public ColorRGBA? FeetColor = null;

        public static Dictionary<int, CreatureTemplateColorTint> GetCreatureTemplateColorTints()
        {
            if (CreatureTemplateColorTints.Count == 0)
                PopulateCreatureTemplateColorTints();
            return CreatureTemplateColorTints;
        }

        private static void PopulateCreatureTemplateColorTints()
        {
            CreatureTemplateColorTints.Clear();

            string creatureTemplateColorTintsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplateColors.csv");
            Logger.WriteDebug("Populating Creature Template Color Tint list via file '" + creatureTemplateColorTintsFile + "'");
            string inputData = FileTool.ReadAllDataFromFile(creatureTemplateColorTintsFile);
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
                string[] rowBlocks = row.Split("|");
                CreatureTemplateColorTint colorTint = new CreatureTemplateColorTint();
                colorTint.ID = int.Parse(rowBlocks[0]);
                colorTint.Name = rowBlocks[1];
                int helmRed = int.Parse(rowBlocks[2]);
                int helmGreen = int.Parse(rowBlocks[3]);
                int helmBlue = int.Parse(rowBlocks[4]);
                if (helmRed != 0 || helmGreen != 0 || helmBlue != 0)
                    colorTint.HelmColor = new ColorRGBA(Convert.ToByte(helmRed), Convert.ToByte(helmGreen), Convert.ToByte(helmBlue));
                int chestRed = int.Parse(rowBlocks[5]);
                int chestGreen = int.Parse(rowBlocks[6]);
                int chestBlue = int.Parse(rowBlocks[7]);
                if (chestRed != 0 || chestGreen != 0 || chestBlue != 0)
                    colorTint.ChestColor = new ColorRGBA(Convert.ToByte(chestRed), Convert.ToByte(chestGreen), Convert.ToByte(chestBlue));
                int armsRed = int.Parse(rowBlocks[8]);
                int armsGreen = int.Parse(rowBlocks[9]);
                int armsBlue = int.Parse(rowBlocks[10]);
                if (armsRed != 0 || armsGreen != 0 || armsBlue != 0)
                    colorTint.ArmsColor = new ColorRGBA(Convert.ToByte(armsRed), Convert.ToByte(armsGreen), Convert.ToByte(armsBlue));
                int bracerRed = int.Parse(rowBlocks[11]);
                int bracerGreen = int.Parse(rowBlocks[12]);
                int bracerBlue = int.Parse(rowBlocks[13]);
                if (bracerRed != 0 || bracerGreen != 0 || bracerBlue != 0)
                    colorTint.BracerColor = new ColorRGBA(Convert.ToByte(bracerRed), Convert.ToByte(bracerGreen), Convert.ToByte(bracerBlue));
                int handsRed = int.Parse(rowBlocks[14]);
                int handsGreen = int.Parse(rowBlocks[15]);
                int handsBlue = int.Parse(rowBlocks[16]);
                if (handsRed != 0 || handsGreen != 0 || handsBlue != 0)
                    colorTint.HandsColor = new ColorRGBA(Convert.ToByte(handsRed), Convert.ToByte(handsGreen), Convert.ToByte(handsBlue));
                int legsRed = int.Parse(rowBlocks[17]);
                int legsGreen = int.Parse(rowBlocks[18]);
                int legsBlue = int.Parse(rowBlocks[19]);
                if (legsRed != 0 || legsGreen != 0 || legsBlue != 0)
                    colorTint.LegsColor = new ColorRGBA(Convert.ToByte(legsRed), Convert.ToByte(legsGreen), Convert.ToByte(legsBlue));
                int feetRed = int.Parse(rowBlocks[20]);
                int feetGreen = int.Parse(rowBlocks[21]);
                int feetBlue = int.Parse(rowBlocks[22]);
                if (feetRed != 0 || feetGreen != 0 || feetBlue != 0)
                    colorTint.FeetColor = new ColorRGBA(Convert.ToByte(feetRed), Convert.ToByte(feetGreen), Convert.ToByte(feetBlue));
                if (CreatureTemplateColorTints.ContainsKey(colorTint.ID) == true)
                    Logger.WriteError("Unable to add creature template color tint with ID '" + colorTint.ID.ToString() + "' as that ID already exists in the collection");
                else
                    CreatureTemplateColorTints.Add(colorTint.ID, colorTint);
            }
        }
    }
}
