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

using EQWOWConverter.Common;
using EQWOWConverter.Items;

namespace EQWOWConverter.Creatures
{
    // One chest armor appearance (whole-body texture set + tint) that exists among the EQ chest items
    internal class CreatureIllusionChestAppearance
    {
        public int BodySet; // 0 - 3 cloth/leather/chain/plate sets with 10 - 16 as robe sets
        public int TintID; // 0 = Untinted, otherwise an IllusionTintPalette tint ID

        public CreatureIllusionChestAppearance(int bodySet, int tintID)
        {
            BodySet = bodySet;
            TintID = tintID;
        }
    }

    internal static class CreatureIllusionTintPalette
    {
        // Generated CreatureTemplateColorTint records are this ID plus the tint ID
        public const int GENERATED_COLOR_TINT_ID_START = 20000;

        private static readonly object PaletteLock = new object();
        private static bool IsBuilt = false;
        private static Dictionary<int, ColorRGBA> PaletteColorsByTintID = new Dictionary<int, ColorRGBA>();
        private static Dictionary<int, int> TintIDsByItemColorRGB = new Dictionary<int, int>(); // Key is 0xRRGGBB
        private static List<CreatureIllusionChestAppearance> ChestAppearances = new List<CreatureIllusionChestAppearance>();

        // Returns the tint ID for a packed item color, or 0 if the item has no real tint
        public static int GetTintIDForColorPacked(Int64 colorPacked)
        {
            lock (PaletteLock)
            {
                BuildIfNeeded();
                if (colorPacked == 0)
                    return 0;
                int colorRGB = Convert.ToInt32(colorPacked & 0xFFFFFF);
                if (TintIDsByItemColorRGB.ContainsKey(colorRGB) == true)
                    return TintIDsByItemColorRGB[colorRGB];
                return CalculateNearestTintID(colorRGB);
            }
        }

        public static List<CreatureIllusionChestAppearance> GetChestAppearances()
        {
            lock (PaletteLock)
            {
                BuildIfNeeded();
                return new List<CreatureIllusionChestAppearance>(ChestAppearances);
            }
        }

        public static int GetColorTintIDForTintID(int tintID)
        {
            if (tintID == 0)
                return 0;
            return GENERATED_COLOR_TINT_ID_START + tintID;
        }

        public static int GetBodySetForEQArmorMaterialType(int eqArmorMaterialType)
        {
            // Materials 0 - 3 map to the whole-body texture sets 0 - 3 (cloth/base, leather, chain, plate), and robe materials 10 - 16 map to the matching robe body sets 10 - 16
            if (eqArmorMaterialType >= 0 && eqArmorMaterialType <= 3)
                return eqArmorMaterialType;
            if (eqArmorMaterialType >= 10 && eqArmorMaterialType <= 16)
                return eqArmorMaterialType;

            // Non-classic materials remap to their nearest classic body set
            switch (eqArmorMaterialType)
            {
                case 4: return 1;   // Monk => Leather
                case 7: return 2;   // Kunark Chain => Chain
                case 17: return 1;  // Velious Leather => Leather
                case 18: return 2;  // Velious Chain => Chain
                case 19: return 3;  // Velious Plate => Plate
                case 20: return 1;  // Velious Leather 2 => Leather
                case 21: return 2;  // Velious Chain 2 => Chain
                case 22: return 3;  // Velious Plate 2 => Plate
                case 23: return 1;  // Velious Monk => Leather
            }
            return 0;
        }

        private static void BuildIfNeeded()
        {
            if (IsBuilt == true)
                return;
            IsBuilt = true;
            Logger.WriteDebug("Building the illusion chest tint palette from the item templates...");

            // Collect colors and body sets from chest-wearable items.  The raw ItemTemplates.csv is read directly (instead of using ItemTemplate.GetItemTemplatesByEQDBIDs)
            // so that reruns of the converter doesn't change the IDs for those that are already known
            Dictionary<int, int> itemCountsByColorRGB = new Dictionary<int, int>();
            List<int[]> chestBodySetsAndColorRGBs = new List<int[]>(); // Values are [bodySet, colorRGB], colorRGB is -1 when untinted
            string itemsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "ItemTemplates.csv");
            List<Dictionary<string, string>> itemRows = FileTool.ReadAllRowsFromFileWithHeader(itemsFileName, "|");
            foreach (Dictionary<string, string> itemColumns in itemRows)
            {
                int slotMask = int.Parse(itemColumns["slots"]);
                if ((slotMask & Convert.ToInt32(ItemEQEquipSlotBitmaskType.Chest)) == 0)
                    continue;
                int bodySet = GetBodySetForEQArmorMaterialType(int.Parse(itemColumns["material"]));
                Int64 colorPacked = Int64.Parse(itemColumns["color"]);
                if (colorPacked == 0)
                {
                    chestBodySetsAndColorRGBs.Add(new int[] { bodySet, -1 });
                    continue;
                }
                int colorRGB = Convert.ToInt32(colorPacked & 0xFFFFFF);
                if (itemCountsByColorRGB.ContainsKey(colorRGB) == false)
                    itemCountsByColorRGB.Add(colorRGB, 0);
                itemCountsByColorRGB[colorRGB] = itemCountsByColorRGB[colorRGB] + 1;
                chestBodySetsAndColorRGBs.Add(new int[] { bodySet, colorRGB });
            }

            // Rank the distinct colors by how many items use them (most used first, ties break on the color value) and keep the top (tint ID 0 is the reserved untinted slot)
            List<KeyValuePair<int, int>> rankedColorCounts = new List<KeyValuePair<int, int>>();
            foreach (var itemCountByColorRGB in itemCountsByColorRGB)
                rankedColorCounts.Add(itemCountByColorRGB);
            rankedColorCounts.Sort(CompareColorCountsForRanking);
            int colorsToKeep = Configuration.CREATURE_ILLUSION_TINT_PALETTE_SIZE - 1;
            if (colorsToKeep < 0)
                colorsToKeep = 0;
            int curTintID = 1;
            foreach (KeyValuePair<int, int> rankedColorCount in rankedColorCounts)
            {
                if (curTintID > colorsToKeep)
                    break;
                int colorRGB = rankedColorCount.Key;
                byte red = Convert.ToByte((colorRGB >> 16) & 0xFF);
                byte green = Convert.ToByte((colorRGB >> 8) & 0xFF);
                byte blue = Convert.ToByte(colorRGB & 0xFF);
                ColorRGBA paletteColor = new ColorRGBA(red, green, blue, 255);
                PaletteColorsByTintID.Add(curTintID, paletteColor);
                CreatureTemplateColorTint.AddGeneratedColorTint(GetColorTintIDForTintID(curTintID),
                    string.Concat("Illusion Tint ", curTintID.ToString()), paletteColor);
                curTintID++;
            }

            // Map every distinct item color to the nearest kept palette color
            foreach (var itemCountByColorRGB in itemCountsByColorRGB)
                TintIDsByItemColorRGB.Add(itemCountByColorRGB.Key, CalculateNearestTintID(itemCountByColorRGB.Key));

            // Build the distinct chest appearances, always including the untinted whole-body sets 0 - 3
            HashSet<int> addedAppearanceKeys = new HashSet<int>();
            for (int bodySet = 0; bodySet <= 3; bodySet++)
            {
                addedAppearanceKeys.Add(bodySet * 10000);
                ChestAppearances.Add(new CreatureIllusionChestAppearance(bodySet, 0));
            }
            foreach (int[] chestBodySetAndColorRGB in chestBodySetsAndColorRGBs)
            {
                int bodySet = chestBodySetAndColorRGB[0];
                int tintID = 0;
                if (chestBodySetAndColorRGB[1] != -1)
                    tintID = TintIDsByItemColorRGB[chestBodySetAndColorRGB[1]];
                int appearanceKey = bodySet * 10000 + tintID;
                if (addedAppearanceKeys.Contains(appearanceKey) == true)
                    continue;
                addedAppearanceKeys.Add(appearanceKey);
                ChestAppearances.Add(new CreatureIllusionChestAppearance(bodySet, tintID));
            }
            ChestAppearances.Sort(CompareChestAppearancesForOrdering);

            Logger.WriteDebug(string.Concat("Illusion chest tint palette built with '", PaletteColorsByTintID.Count.ToString(),
                "' colors and '", ChestAppearances.Count.ToString(), "' chest appearances from '", itemCountsByColorRGB.Count.ToString(),
                "' distinct item colors"));
        }

        private static int CompareColorCountsForRanking(KeyValuePair<int, int> colorCountA, KeyValuePair<int, int> colorCountB)
        {
            // Higher counts rank first
            if (colorCountA.Value != colorCountB.Value)
                return colorCountB.Value.CompareTo(colorCountA.Value);
            return colorCountA.Key.CompareTo(colorCountB.Key);
        }

        private static int CompareChestAppearancesForOrdering(CreatureIllusionChestAppearance appearanceA, CreatureIllusionChestAppearance appearanceB)
        {
            if (appearanceA.BodySet != appearanceB.BodySet)
                return appearanceA.BodySet.CompareTo(appearanceB.BodySet);
            return appearanceA.TintID.CompareTo(appearanceB.TintID);
        }

        private static int CalculateNearestTintID(int colorRGB)
        {
            if (PaletteColorsByTintID.Count == 0)
                return 0;
            int red = (colorRGB >> 16) & 0xFF;
            int green = (colorRGB >> 8) & 0xFF;
            int blue = colorRGB & 0xFF;
            int nearestTintID = 0;
            int nearestDistanceSquared = int.MaxValue;
            foreach (var paletteColorByTintID in PaletteColorsByTintID)
            {
                int deltaRed = red - paletteColorByTintID.Value.R;
                int deltaGreen = green - paletteColorByTintID.Value.G;
                int deltaBlue = blue - paletteColorByTintID.Value.B;
                int distanceSquared = (deltaRed * deltaRed) + (deltaGreen * deltaGreen) + (deltaBlue * deltaBlue);
                if (distanceSquared < nearestDistanceSquared)
                {
                    nearestDistanceSquared = distanceSquared;
                    nearestTintID = paletteColorByTintID.Key;
                }
            }
            return nearestTintID;
        }
    }
}
