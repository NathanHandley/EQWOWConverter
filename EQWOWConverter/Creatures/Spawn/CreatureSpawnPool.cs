﻿//  Author: Nathan Handley (nathanhandley@protonmail.com)
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
    internal class CreatureSpawnPool
    {
        private static int CURRENT_POOL_TEMPLATE_ENTRYID = Configuration.CONFIG_SQL_POOL_TEMPLATE_ID_START;

        public List<CreatureSpawnInstance> CreatureSpawnInstances = new List<CreatureSpawnInstance>();
        public List<CreatureTemplate> CreatureTemplates = new List<CreatureTemplate>();
        public List<int> CreatureTemplateChances = new List<int>();
        public CreatureSpawnGroup CreatureSpawnGroup;

        public CreatureSpawnPool(CreatureSpawnGroup creatureSpawnGroup)
        {
            CreatureSpawnGroup = creatureSpawnGroup;
        }

        public void AddCreatureTemplate(CreatureTemplate creatureTemplate, int chance)
        {
            CreatureTemplates.Add(creatureTemplate);
            CreatureTemplateChances.Add(chance);
        }

        public int GetMaxSpawnCount()
        {
            if (CreatureSpawnGroup.SpawnLimit == 0)
                return CreatureSpawnInstances.Count;
            else
                return CreatureSpawnGroup.SpawnLimit;
        }

        public void AddSpawnInstance(CreatureSpawnInstance creatureSpawnInstance)
        {
            CreatureSpawnInstances.Add(creatureSpawnInstance);
        }

        public bool DoChancesAddTo100()
        {
            int totalChance = 0;
            foreach (int chance in CreatureTemplateChances)
                totalChance += chance;
            if (totalChance == 100)
                return true;
            else
                return false;
        }

        public static int GetPoolTemplateSQLID()
        {
            int returnTemplateID = CURRENT_POOL_TEMPLATE_ENTRYID;
            CURRENT_POOL_TEMPLATE_ENTRYID++;
            return returnTemplateID;
        }
    }
}
