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

namespace EQWOWConverter.Items
{
    internal class ItemPickpocketJunkbox
    {
        internal class JunkboxContentItem
        {
            public int EQItemID = 0;
            public string Name = string.Empty;
            public int Weight = 1;
        }

        // The item rows themselves live in ItemTemplates.csv, and are matched to these definitions by EQ item ID
        private static readonly List<(int EQItemID, int PickLockSkillRequired, int MinCreatureLevel, int MaxCreatureLevel)> JunkboxDefinitions =
            new List<(int, int, int, int)>()
        {
            (40038, 1, 1, 15),    // Battered Norrath Junkbox
            (40039, 50, 16, 20),  // Worn Norrath Junkbox
            (40044, 100, 21, 25), // Dented Norrath Junkbox
            (40045, 150, 26, 35), // Dusty Junkbox
            (40040, 200, 36, 45), // Sturdy Norrath Junkbox
            (40041, 250, 46, 60), // Heavy Norrath Junkbox
            (40042, 300, 61, 70), // Strong Norrath Junkbox
            (40043, 350, 71, 80)  // Reinforced Junkbox
        };

        private static List<ItemPickpocketJunkbox> Junkboxes = new List<ItemPickpocketJunkbox>();
        private static readonly object JunkboxLock = new object();

        public int EQItemID = 0;
        public int PickLockSkillRequired = 0;
        public int MinCreatureLevel = 0;
        public int MaxCreatureLevel = 0;
        public int LockDBCID = 0;
        public ItemTemplate? ItemTemplate = null;
        public List<JunkboxContentItem> ContentItems = new List<JunkboxContentItem>();

        public static List<ItemPickpocketJunkbox> GetJunkboxes()
        {
            lock (JunkboxLock)
            {
                if (Junkboxes.Count == 0)
                    PopulateJunkboxList();
                return Junkboxes;
            }
        }

        public static ItemPickpocketJunkbox? GetJunkboxForCreatureLevel(int creatureLevel)
        {
            List<ItemPickpocketJunkbox> junkboxes = GetJunkboxes();
            if (junkboxes.Count == 0)
                return null;
            foreach (ItemPickpocketJunkbox junkbox in junkboxes)
                if (creatureLevel >= junkbox.MinCreatureLevel && creatureLevel <= junkbox.MaxCreatureLevel)
                    return junkbox;

            // Anything under or over the defined bands falls into the first or last junkbox
            if (creatureLevel < junkboxes[0].MinCreatureLevel)
                return junkboxes[0];
            return junkboxes[junkboxes.Count - 1];
        }

        public int GetMinMoneyLootInCopper()
        {
            // Coin scales off the middle of the level band, since any creature in that band can carry this junkbox
            return GetMidCreatureLevel() * Configuration.ITEMS_PICKPOCKET_JUNKBOX_COIN_MIN_PER_LEVEL;
        }

        public int GetMaxMoneyLootInCopper()
        {
            return GetMidCreatureLevel() * Configuration.ITEMS_PICKPOCKET_JUNKBOX_COIN_MAX_PER_LEVEL;
        }

        public void AssignLockDBCID()
        {
            LockDBCID = IDGenerationTool.GenerateID("LockID", "junkbox", PickLockSkillRequired.ToString());
        }

        private int GetMidCreatureLevel()
        {
            return (MinCreatureLevel + MaxCreatureLevel) / 2;
        }

        private static void PopulateJunkboxList()
        {
            Junkboxes.Clear();

            Dictionary<int, ItemPickpocketJunkbox> junkboxesByEQItemID = new Dictionary<int, ItemPickpocketJunkbox>();
            foreach (var junkboxDefinition in JunkboxDefinitions)
            {
                ItemPickpocketJunkbox junkbox = new ItemPickpocketJunkbox();
                junkbox.EQItemID = junkboxDefinition.EQItemID;
                junkbox.PickLockSkillRequired = junkboxDefinition.PickLockSkillRequired;
                junkbox.MinCreatureLevel = junkboxDefinition.MinCreatureLevel;
                junkbox.MaxCreatureLevel = junkboxDefinition.MaxCreatureLevel;
                junkboxesByEQItemID.Add(junkbox.EQItemID, junkbox);
                Junkboxes.Add(junkbox);
            }

            string junkboxItemsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ItemPickpocketJunkboxItems.csv");
            Logger.WriteDebug("Populating Pick Pocket Junkbox Items List via file '" + junkboxItemsFile + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(junkboxItemsFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // Skip any invalid expansion rows
                int minExpansion = int.Parse(columns["min_expansion"]);
                int maxExpansion = int.Parse(columns["max_expansion"]);
                if (minExpansion != -1 && minExpansion > Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;
                if (maxExpansion != -1 && maxExpansion < Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL)
                    continue;

                int junkboxEQItemID = int.Parse(columns["junkbox_eq_itemid"]);
                if (junkboxesByEQItemID.ContainsKey(junkboxEQItemID) == false)
                {
                    Logger.WriteError("Pick Pocket Junkbox item row referenced junkbox with eq item id '", junkboxEQItemID.ToString(), "', but no junkbox with that ID is defined");
                    continue;
                }

                JunkboxContentItem contentItem = new JunkboxContentItem();
                contentItem.EQItemID = int.Parse(columns["eq_itemid"]);
                contentItem.Name = columns["item_name"];
                contentItem.Weight = int.Max(int.Parse(columns["weight"]), 1);
                junkboxesByEQItemID[junkboxEQItemID].ContentItems.Add(contentItem);
            }
        }
    }
}
