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

namespace EQWOWConverter.Achievements
{
    internal class AchievementCategory
    {
        private static SortedDictionary<int, AchievementCategory> CategoriesByCategoryID = new SortedDictionary<int, AchievementCategory>();
        private static readonly object CategoryReadLock = new object();

        public int CategoryID;
        public int ParentCategoryID;
        public string Name = string.Empty;
        public int UIOrder;

        public static SortedDictionary<int, AchievementCategory> GetAchievementCategoriesByID()
        {
            lock (CategoryReadLock)
            {
                if (CategoriesByCategoryID.Count == 0)
                    PopulateCategoryList();
                return CategoriesByCategoryID;
            }
        }

        private static void PopulateCategoryList()
        {
            string categoriesFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "AchievementCategories.csv");
            Logger.WriteDebug("Populating achievement category list via file '" + categoriesFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(categoriesFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                AchievementCategory category = new AchievementCategory();
                category.CategoryID = int.Parse(columns["CategoryID"]);
                category.ParentCategoryID = int.Parse(columns["ParentCategoryID"]);
                category.Name = columns["Name"];
                category.UIOrder = int.Parse(columns["UIOrder"]);
                if (CategoriesByCategoryID.ContainsKey(category.CategoryID) == true)
                {
                    Logger.WriteError("AchievementCategories.csv has more than one row with CategoryID '" + category.CategoryID + "', so extra rows are skipped");
                    continue;
                }
                CategoriesByCategoryID.Add(category.CategoryID, category);
            }
        }
    }
}
