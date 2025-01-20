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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Creatures
{
    internal class CreatureTemplate
    {
        private static Dictionary<int, CreatureTemplate> CreatureTemplateListByEQID = new Dictionary<int, CreatureTemplate>();

        public int EQCreatureTemplateID = 0;
        public int WOWCreatureTemplateID = 0;
        public string Name = string.Empty; // Restrict to 100 characters
        public string SubName = string.Empty; // Restrict to 100 characters
        public int Level = 1;
        public CreatureRace Race = new CreatureRace();
        public int EQClass = 1;
        public int EQBodyType = 24; // This is common for the body type
        public int FaceID = 0;
        public int ColorTintID = 0;
        public float Size = 0f;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureID = 0;
        public int HelmTextureID = 0;
        public CreatureModelTemplate? ModelTemplate = null;
        public int MerchantID = 0;
        public int EQLootTableID = 0;
        public int WOWLootID = 0;
        public int MoneyMinInCopper = 0;
        public int MoneyMaxInCopper = 0;
        public bool HasMana = false;
        public float HPMod = 1f;

        private static int CURRENT_SQL_CREATURE_GUID = Configuration.CONFIG_SQL_CREATURE_GUID_LOW;
        private static int CURRENT_SQL_CREATURETEMPLATEID = Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW;
        
        public CreatureTemplate()
        {
            WOWCreatureTemplateID = CURRENT_SQL_CREATURETEMPLATEID;
            CURRENT_SQL_CREATURETEMPLATEID++;
        }

        public static Dictionary<int, CreatureTemplate> GetCreatureTemplateListByEQID()
        {
            if (CreatureTemplateListByEQID.Count == 0)
                PopulateCreatureTemplateList();
            return CreatureTemplateListByEQID;
        }

        public static int GenerateCreatureSQLGUID()
        {
            int returnGUID = CURRENT_SQL_CREATURE_GUID;
            CURRENT_SQL_CREATURE_GUID++;
            return returnGUID;
        }

        private static void PopulateCreatureTemplateList()
        {
            CreatureTemplateListByEQID.Clear();

            string creatureTemplatesFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplates.csv");
            Logger.WriteDetail("Populating Creature Template list via file '" + creatureTemplatesFile + "'");
            string inputData = FileTool.ReadAllDataFromFile(creatureTemplatesFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureTemplates list via file '" + creatureTemplatesFile + "' did not have enough rows");
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
                CreatureTemplate newCreatureTemplate = new CreatureTemplate();
                newCreatureTemplate.EQCreatureTemplateID = int.Parse(rowBlocks[0]);
                newCreatureTemplate.Name = rowBlocks[2];
                newCreatureTemplate.SubName = rowBlocks[3];
                newCreatureTemplate.Level = int.Max(int.Parse(rowBlocks[4]), 1);
                int raceID = int.Parse(rowBlocks[5]);
                if (raceID == 0)
                {
                    Logger.WriteDetail("Creature template had race of 0, so falling back to 1 (Human)");
                    raceID = 1;
                }
                newCreatureTemplate.EQClass = int.Parse(rowBlocks[6]);
                newCreatureTemplate.EQBodyType = int.Parse(rowBlocks[7]);
                newCreatureTemplate.Size = float.Parse(rowBlocks[8]);
                if (newCreatureTemplate.Size <= 0)
                {
                    Logger.WriteDetail("CreatureTemplate with size of zero or less detected with name '" + newCreatureTemplate.Name + "', so setting to 1");
                    newCreatureTemplate.Size = 1;
                }
                int genderID = int.Parse(rowBlocks[9]);
                switch (genderID)
                {
                    case 0: newCreatureTemplate.GenderType = CreatureGenderType.Male; break;
                    case 1: newCreatureTemplate.GenderType = CreatureGenderType.Female; break;
                    default: newCreatureTemplate.GenderType = CreatureGenderType.Neutral; break;
                }
                newCreatureTemplate.TextureID = int.Parse(rowBlocks[10]);
                newCreatureTemplate.HelmTextureID = int.Parse(rowBlocks[11]);
                newCreatureTemplate.FaceID = int.Parse(rowBlocks[12]);
                if (newCreatureTemplate.FaceID > 9)
                {
                    Logger.WriteDetail("CreatureTemplate with face ID greater than 9 detected, so setting to 0");
                    newCreatureTemplate.FaceID = 0;
                }
                newCreatureTemplate.EQLootTableID = int.Parse(rowBlocks[13]);
                newCreatureTemplate.MerchantID = int.Parse(rowBlocks[14]);
                newCreatureTemplate.ColorTintID = int.Parse(rowBlocks[15]);
                newCreatureTemplate.HasMana = (int.Parse(rowBlocks[16]) > 0);
                newCreatureTemplate.HPMod = float.Parse(rowBlocks[17]);

                // Strip underscores
                newCreatureTemplate.Name = newCreatureTemplate.Name.Replace('_', ' ');
                newCreatureTemplate.SubName = newCreatureTemplate.SubName.Replace('_', ' ');

                // Special logic for a few variations of kobolds, which look wrong if not adjusted
                if (raceID == 48)
                {
                    if  (newCreatureTemplate.TextureID == 2 || (newCreatureTemplate.TextureID == 1 && newCreatureTemplate.HelmTextureID == 0))
                    {
                        newCreatureTemplate.TextureID = 0;
                        newCreatureTemplate.HelmTextureID = 0;
                        newCreatureTemplate.FaceID = 0;
                    }
                }

                // Grab the race
                List<CreatureRace> allRaces = CreatureRace.GetAllCreatureRaces();
                CreatureRace? race = null;
                foreach (CreatureRace curRace in allRaces)
                {
                    if (curRace.ID == raceID && curRace.Gender == newCreatureTemplate.GenderType && curRace.VariantID == 0)
                    {
                        race = curRace;
                        break;
                    }
                }

                if (race == null)
                {
                    Logger.WriteError("No valid race found that matches raceID '" + raceID + "' and gender '" + newCreatureTemplate.GenderType + "'");
                    continue;
                }
                else
                {
                    // Make sure there's a skeleton
                    if (race.SkeletonName.Trim().Length == 0)
                    {
                        Logger.WriteDetail("Creature Template with name '" + newCreatureTemplate.Name + "' with race ID of '" + raceID + "' has no skeletons, so skipping");
                        continue;
                    }
                    newCreatureTemplate.Race = race;
                }

                // Add ID if debugging for it is true
                if (Configuration.CONFIG_CREATURE_ADD_ENTITY_ID_TO_NAME == true)
                    newCreatureTemplate.Name = newCreatureTemplate.Name + " " + newCreatureTemplate.EQCreatureTemplateID.ToString();
                //newCreatureTemplate.Name = newCreatureTemplate.Name + " R" + newCreatureTemplate.Race.EQCreatureTemplateID + "-G" + Convert.ToInt32(newCreatureTemplate.GenderType).ToString() + "-V" + newCreatureTemplate.Race.VariantID;

                // Must be a unique record
                if (CreatureTemplateListByEQID.ContainsKey(newCreatureTemplate.EQCreatureTemplateID))
                {
                    Logger.WriteError("Creature Template list via file '" + creatureTemplatesFile + "' has an duplicate row with id '" + newCreatureTemplate.EQCreatureTemplateID + "'");
                    continue;
                }

                CreatureTemplateListByEQID.Add(newCreatureTemplate.EQCreatureTemplateID, newCreatureTemplate);
            }
        }
    }
}
