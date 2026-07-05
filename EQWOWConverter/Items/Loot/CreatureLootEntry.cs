//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2026 Nathan Handley
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

namespace EQWOWConverter.Items
{
    // "Flattened" loot row for a creature so that it can work more like EverQuest in the mod
    internal class CreatureLootEntry
    {
        public int CreatureTemplateEntryID = 0;
        public int LootGroupID = 0;
        public int GroupMultiplier = 1;
        public int GroupMultiplierMin = 0;
        public float GroupProbability = 100;
        public int DropLimit = 0;
        public int MinDrop = 0;
        public int ItemTemplateEntryID = 0;
        public float Chance = 0;
        public int ItemMultiplier = 1;
        public int ItemCharges = 1;
    }
}
