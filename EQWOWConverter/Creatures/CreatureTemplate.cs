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
    internal class CreatureTemplate
    {
        private static Dictionary<int, CreatureTemplate> CreatureTemplateList = new Dictionary<int, CreatureTemplate>();

        public int ID = 0;
        public string Name = string.Empty; // Restrict to 100 characters
        public string SubName = string.Empty; // Restrict to 100 characters
        public int Level = 1;
        public int RaceID = 0;
        public int Class = 1;
        public int BodyType = 24; // This is common for the body type
        public int HP = 1;
        public float Size = 0f;
        public CreatureGenderType GenderType = CreatureGenderType.Neutral;
        public int TextureID = 0;
        public int HelmTextureID = 0;
        public CreatureModelTemplate? ModelTemplate = null;

        private static int CURRENT_SQL_CREATURE_GUID = Configuration.CONFIG_SQL_CREATURE_GUID_LOW;
        private static int CURRENT_SQL_CREATURETEMPLATEID = Configuration.CONFIG_SQL_CREATURETEMPLATE_ENTRY_LOW;
        public int SQLCreatureTemplateID;

        public CreatureTemplate()
        {
            SQLCreatureTemplateID = CURRENT_SQL_CREATURETEMPLATEID;
            CURRENT_SQL_CREATURETEMPLATEID++;
        }

        public static Dictionary<int, CreatureTemplate> GetCreatureTemplateList()
        {
            if (CreatureTemplateList.Count == 0)
                PopulateCreatureTemplateList();
            return CreatureTemplateList;
        }

        public static int GenerateCreatureSQLGUID()
        {
            int returnGUID = CURRENT_SQL_CREATURE_GUID;
            CURRENT_SQL_CREATURE_GUID++;
            return returnGUID;
        }

        private static void PopulateCreatureTemplateList()
        {
            CreatureTemplateList.Clear();

            string creatureTemplatesFile = Path.Combine(Configuration.CONFIG_PATH_ASSETS_FOLDER, "WorldData", "CreatureTemplates.csv");
            Logger.WriteDetail("Populating Creature Template list via file '" + creatureTemplatesFile + "'");
            string inputData = File.ReadAllText(creatureTemplatesFile);
            string[] inputRows = inputData.Split(Environment.NewLine);
            if (inputRows.Length < 2)
            {
                Logger.WriteError("CreatureTemplates list via file '" + creatureTemplatesFile + "' did not have enough rows");
                return;
            }

            // Grab races for fallback
            Dictionary<int, CreatureRace> allRacesById = CreatureRace.GetAllCreatureRacesByID();

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
                CreatureTemplate newCreatureTemplate = new CreatureTemplate();
                newCreatureTemplate.ID = int.Parse(rowBlocks[0]);
                newCreatureTemplate.Name = rowBlocks[1];
                newCreatureTemplate.SubName = rowBlocks[2];
                newCreatureTemplate.Level = int.Parse(rowBlocks[3]);
                newCreatureTemplate.RaceID = int.Parse(rowBlocks[4]);
                newCreatureTemplate.Class = int.Parse(rowBlocks[5]);
                newCreatureTemplate.BodyType = int.Parse(rowBlocks[6]);
                newCreatureTemplate.HP = int.Parse(rowBlocks[7]);
                newCreatureTemplate.Size = float.Parse(rowBlocks[12]);
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

                // Strip underscores
                newCreatureTemplate.Name = newCreatureTemplate.Name.Replace('_', ' ');
                newCreatureTemplate.SubName = newCreatureTemplate.SubName.Replace('_', ' ');

                // Add ID if debugging for it is true
                if (Configuration.CONFIG_CREATURE_ADD_ENTITY_ID_TO_NAME == true)
                    newCreatureTemplate.Name = newCreatureTemplate.Name + " " + newCreatureTemplate.ID.ToString();

                // Fallback on race ID
                if (allRacesById.ContainsKey(newCreatureTemplate.RaceID) == false)
                {
                    Logger.WriteDetail("Creature Template with name '" + newCreatureTemplate.Name + "' has an invalid race ID of '" + newCreatureTemplate.RaceID + "', so falling back to 1 (human)");
                    newCreatureTemplate.RaceID = 1;
                }

                if (CreatureTemplateList.ContainsKey(newCreatureTemplate.ID))
                {
                    Logger.WriteError("Creature Template list via file '" + creatureTemplatesFile + "' has an duplicate row with id '" + newCreatureTemplate.ID + "'");
                    continue;
                }
                CreatureTemplateList.Add(newCreatureTemplate.ID, newCreatureTemplate);
            }
        }
    }
}
