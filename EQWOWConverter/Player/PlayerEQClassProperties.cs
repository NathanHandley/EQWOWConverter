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
    internal class PlayerEQClassProperties
    {
        private static Dictionary<ClassEQType, PlayerEQClassProperties> EQClassPropertiesByEQClass = new Dictionary<ClassEQType, PlayerEQClassProperties>();
        private static readonly object READ_LOCK = new object();

        public ClassEQType EQClass = ClassEQType.Warrior;
        public ItemWOWArmorSubclassType MaxArmorType = ItemWOWArmorSubclassType.Cloth;
        public bool HasForage = false;
        public bool HasBash = false;
        public bool Has1HAxe = false;
        public bool Has2HAxe = false;
        public bool Has1HMace = false;
        public bool Has2HMace = false;
        public bool HasPolearm = false;
        public bool Has1HSword = false;
        public bool Has2HSword = false;
        public bool HasStaff = false;
        public bool HasFistWeapon = false;
        public bool HasDagger = false;
        public bool HasBow = false;

        public static Dictionary<ClassEQType, PlayerEQClassProperties> GetAllEQClassPropertiesByEQClass()
        {
            lock (READ_LOCK)
            {
                if (EQClassPropertiesByEQClass.Count == 0)
                    PopulateClassProperties();
                return EQClassPropertiesByEQClass;
            }
        }

        private static void PopulateClassProperties()
        {
            EQClassPropertiesByEQClass.Clear();

            string classPropertiesFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerEQClassProperties.csv");
            Logger.WriteDebug(string.Concat("Populating EQ Class Properties via file '", classPropertiesFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(classPropertiesFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                // EQ Class
                ClassEQType eqClass = ClassEQType.Warrior;
                switch (columns["EQ_Class"].ToLower().Trim())
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
                ItemWOWArmorSubclassType itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Cloth;
                switch (columns["Max_Armor_Class"].ToLower().Trim())
                {
                    case "plate": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Plate; break;
                    case "mail": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Mail; break;
                    case "leather": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Leather; break;
                    case "cloth": itemWOWArmorSubclassType = ItemWOWArmorSubclassType.Cloth; break;
                    default:
                        {
                            Logger.WriteError("Unable to convert Max_Armor_Class from PlayerClassMapping from ", columns["WOW_Class"], "', so setting to Cloth");
                        } break;
                }

                PlayerEQClassProperties classProperties = new PlayerEQClassProperties();
                classProperties.EQClass = eqClass;
                classProperties.MaxArmorType = itemWOWArmorSubclassType;
                classProperties.HasForage = columns["Forage"].Trim() == "1";
                classProperties.HasBash = columns["Bash"].Trim() == "1";
                classProperties.Has1HAxe = columns["1H_Axe"].Trim() == "1";
                classProperties.Has2HAxe = columns["2H_Axe"].Trim() == "1";
                classProperties.Has1HMace = columns["1H_Mace"].Trim() == "1";
                classProperties.Has2HMace = columns["2H_Mace"].Trim() == "1";
                classProperties.HasPolearm = columns["Polearm"].Trim() == "1";
                classProperties.Has1HSword = columns["1H_Sword"].Trim() == "1";
                classProperties.Has2HSword = columns["2H_Sword"].Trim() == "1";
                classProperties.HasStaff = columns["Staff"].Trim() == "1";
                classProperties.HasFistWeapon = columns["Fist"].Trim() == "1";
                classProperties.HasDagger = columns["Dagger"].Trim() == "1";
                classProperties.HasBow = columns["Bow"].Trim() == "1";

                if (EQClassPropertiesByEQClass.ContainsKey(eqClass) == true)
                    Logger.WriteError("In PlayerEQClassProperties attempted to add more than one ", eqClass.ToString());
                else
                    EQClassPropertiesByEQClass.Add(eqClass, classProperties);
            }
        }

        public static ItemWOWArmorSubclassType GetArmorClassForItemWearableByEQClasses(List<ClassEQType> eqClasses)
        {
            lock (READ_LOCK)
            {
                if (EQClassPropertiesByEQClass.Count == 0)
                    PopulateClassProperties();

                // Determine the lowest armor class type, which can differ base on config
                int lowestItemWOWArmorSubClassType = (int)ItemWOWArmorSubclassType.Plate;
                if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
                {
                    foreach (ClassEQType eqClass in eqClasses)
                    {
                        if (EQClassPropertiesByEQClass.ContainsKey(eqClass) == true)
                        {
                            PlayerEQClassProperties classProperties = EQClassPropertiesByEQClass[eqClass];
                            if ((int)classProperties.MaxArmorType < lowestItemWOWArmorSubClassType)
                                lowestItemWOWArmorSubClassType = (int)classProperties.MaxArmorType;
                        }
                    }
                }
                else
                {
                    // Not changing player skills for eq skills, so use wow classes
                    // Get a list of all WOW classes
                    Dictionary<ClassWOWType, PlayerClassMapping> classMappingsByWOWClass = PlayerClassMapping.GetClassMappingsByWOWClass();
                    HashSet<ClassWOWType> wowClasses = new HashSet<ClassWOWType>();
                    foreach (ClassEQType eqClass in eqClasses)
                    {
                        foreach (PlayerClassMapping classMapping in classMappingsByWOWClass.Values)
                        {
                            if (classMapping.BaseEQClass == eqClass)
                                wowClasses.Add(classMapping.WOWClass);
                        }
                    }

                    // Find the lowest
                    foreach (ClassWOWType wowClass in wowClasses)
                    {
                        int curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Cloth;
                        switch (wowClass)
                        {
                            case ClassWOWType.DeathKnight: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Plate; break;
                            case ClassWOWType.Druid: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Leather; break;
                            case ClassWOWType.Hunter: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Mail; break;
                            case ClassWOWType.Mage: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Cloth; break;
                            case ClassWOWType.Paladin: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Plate; break;
                            case ClassWOWType.Priest: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Cloth; break;
                            case ClassWOWType.Rogue: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Leather; break;
                            case ClassWOWType.Shaman: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Mail; break;
                            case ClassWOWType.Warlock: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Cloth; break;
                            case ClassWOWType.Warrior: curClassMaxArmorSubClassType = (int)ItemWOWArmorSubclassType.Plate; break;
                            default: Logger.WriteError("Unhandled class type '", wowClass.ToString(), "' in GetArmorClassForItemWearableByEQClasses"); break;
                        }
                        lowestItemWOWArmorSubClassType = Math.Min(curClassMaxArmorSubClassType, lowestItemWOWArmorSubClassType);
                    }
                }
                return (ItemWOWArmorSubclassType)lowestItemWOWArmorSubClassType;
            }
        }
    }
}
