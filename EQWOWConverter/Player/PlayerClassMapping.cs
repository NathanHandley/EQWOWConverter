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

namespace EQWOWConverter.Player
{
    internal class PlayerClassMapping
    {
        private static Dictionary<ClassEQType, ItemWOWArmorSubclassType> WOWMaxArmorClassTypeByEQClass = new Dictionary<ClassEQType, ItemWOWArmorSubclassType>();
        private static Dictionary<ClassEQType, List<ClassWOWType>> WOWClassesByEQClass = new Dictionary<ClassEQType, List<ClassWOWType>>();
        private static Dictionary<ClassWOWType, List<ClassEQType>> EQClassesByWOWClass = new Dictionary<ClassWOWType, List<ClassEQType>>();
        private static HashSet<ClassWOWType> WOWClassesWhichShouldHaveArchery = new HashSet<ClassWOWType>();
        private static readonly object ClassesLock = new object();

        public static HashSet<ClassWOWType> GetWhichWOWClassesHaveArchery()
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                return WOWClassesWhichShouldHaveArchery;
            }
        }

        public static List<ClassWOWType> GetWOWClassesForEQClass(ClassEQType eqClass)
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                if (WOWClassesByEQClass.ContainsKey(eqClass) == false)
                    return new List<ClassWOWType>();
                return WOWClassesByEQClass[eqClass];
            }
        }

        public static ItemWOWArmorSubclassType GetMaxArmorTypeForEQClass(ClassEQType eqClass)
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                if (WOWMaxArmorClassTypeByEQClass.ContainsKey(eqClass) == false)
                    return ItemWOWArmorSubclassType.Cloth;
                else
                    return WOWMaxArmorClassTypeByEQClass[eqClass];
            }
        }

        public static Dictionary<ClassEQType, List<ClassWOWType>> GetAllWOWClassesByEQClass()
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                return WOWClassesByEQClass;
            }
        }

        public static Dictionary<ClassWOWType, List<ClassEQType>> GetAllEQClassesByWOWClass()
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                return EQClassesByWOWClass;
            }
        }

        private static void PopulateClassMap()
        {
            EQClassesByWOWClass.Clear();
            WOWClassesWhichShouldHaveArchery.Clear();
            foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                EQClassesByWOWClass.Add(wowClassType, new List<ClassEQType>());
            WOWMaxArmorClassTypeByEQClass.Clear();
            WOWClassesByEQClass.Clear();
            foreach (ClassEQType eqClassType in Enum.GetValues(typeof(ClassEQType)))
            {
                WOWMaxArmorClassTypeByEQClass.Add(eqClassType, ItemWOWArmorSubclassType.Cloth);
                WOWClassesByEQClass.Add(eqClassType, new List<ClassWOWType>());
            }

            string classMapFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerClassMapping.csv");
            Logger.WriteDebug(string.Concat("Populating player class map via file '", classMapFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(classMapFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // EQ Class
                ClassEQType eqClass = ClassEQType.Warrior;
                switch(columns["EQ_Class"].ToLower().Trim())
                {
                    case "bard": eqClass = ClassEQType.Bard; break;
                    case "cleric": eqClass = ClassEQType.Cleric; break;
                    case "druid": eqClass = ClassEQType.Druid; break;
                    case "enchanter": eqClass = ClassEQType.Enchanter; break;
                    case "magician": eqClass = ClassEQType.Magician; break;
                    case "monk": eqClass = ClassEQType.Monk; break;
                    case "necromancer": eqClass = ClassEQType.Necromancer; break;
                    case "paladin": eqClass = ClassEQType.Paladin; break;
                    case "ranger": eqClass = ClassEQType.Ranger; break;
                    case "rogue": eqClass = ClassEQType.Rogue; break;
                    case "shadowknight": eqClass = ClassEQType.ShadowKnight; break;
                    case "shaman": eqClass = ClassEQType.Shaman; break;
                    case "warrior": eqClass = ClassEQType.Warrior; break;
                    case "wizard": eqClass = ClassEQType.Wizard; break;
                    default:
                        {
                            Logger.WriteError("Unable to convert EQ_Class from PlayerClassMapping from ", columns["EQ_Class"], "' to an EQ class");
                            continue;
                        }
                }

                // WOW Class
                ClassWOWType wowClass = ClassWOWType.Warrior;
                switch (columns["WOW_Class"].ToLower().Trim())
                {
                    case "deathknight": wowClass = ClassWOWType.DeathKnight; break;
                    case "druid": wowClass = ClassWOWType.Druid; break;
                    case "hunter": wowClass = ClassWOWType.Hunter; break;
                    case "mage": wowClass = ClassWOWType.Mage; break;
                    case "rogue": wowClass = ClassWOWType.Rogue; break;
                    case "paladin": wowClass = ClassWOWType.Paladin; break;
                    case "priest": wowClass = ClassWOWType.Priest; break;
                    case "shaman": wowClass = ClassWOWType.Shaman; break;
                    case "warlock": wowClass = ClassWOWType.Warlock; break;
                    case "warrior": wowClass = ClassWOWType.Warrior; break;
                    default:
                        {
                            Logger.WriteError("Unable to convert WOW_Class from PlayerClassMapping from ", columns["WOW_Class"], "' to a WOW class");
                            continue;
                        }
                }
                if (WOWClassesByEQClass[eqClass].Contains(wowClass) == false)
                    WOWClassesByEQClass[eqClass].Add(wowClass);
                if (EQClassesByWOWClass[wowClass].Contains(eqClass) == false)
                    EQClassesByWOWClass[wowClass].Add(eqClass);

                // Armor type
                ItemWOWArmorSubclassType itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Cloth;
                switch (columns["Max_Armor_Class"].ToLower().Trim())
                {
                    case "plate": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Plate; break;
                    case "mail": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Mail;  break;
                    case "leather": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Leather;  break;
                    case "cloth": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Cloth; break;
                    default:
                        {
                            Logger.WriteError("Unable to convert Max_Armor_Class from PlayerClassMapping from ", columns["WOW_Class"], "', so setting to Cloth");
                        } break;
                }
                WOWMaxArmorClassTypeByEQClass[eqClass] = itemWOWArmorSubclassType;

                // Archery / Bows
                if (columns["Has_Archery"].Trim() == "1")
                    WOWClassesWhichShouldHaveArchery.Add(wowClass);
            }
        }
    }
}
