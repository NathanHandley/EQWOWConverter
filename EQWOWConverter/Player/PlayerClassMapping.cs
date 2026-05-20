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

        public static HashSet<ClassWOWType> GetWOWClassesEligibleForWeaponSubClass(ItemWOWWeaponSubclassType weaponSubClassType)
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();

                HashSet<ClassWOWType> eligibleClasses = new HashSet<ClassWOWType>();
                if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_MELEE_WEAPON_SKILLS_ON_ALL_CLASSES == false)
                {
                    switch (weaponSubClassType)
                    {
                        case ItemWOWWeaponSubclassType.AxeOneHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Rogue);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.AxeTwoHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.MaceOneHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Rogue);
                                eligibleClasses.Add(ClassWOWType.Priest);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.Druid);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.MaceTwoHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.Druid);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.Polearm:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Druid);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.SwordOneHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.Rogue);
                                eligibleClasses.Add(ClassWOWType.Mage);
                                eligibleClasses.Add(ClassWOWType.Warlock);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.SwordTwoHand:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Paladin);
                                eligibleClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWWeaponSubclassType.Staff:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Priest);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.Mage);
                                eligibleClasses.Add(ClassWOWType.Warlock);
                                eligibleClasses.Add(ClassWOWType.Druid);
                            } break;
                        case ItemWOWWeaponSubclassType.FistWeapon:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Rogue);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.Druid);
                            } break;
                        case ItemWOWWeaponSubclassType.Dagger:
                            {
                                eligibleClasses.Add(ClassWOWType.Warrior);
                                eligibleClasses.Add(ClassWOWType.Hunter);
                                eligibleClasses.Add(ClassWOWType.Rogue);
                                eligibleClasses.Add(ClassWOWType.Priest);
                                eligibleClasses.Add(ClassWOWType.Shaman);
                                eligibleClasses.Add(ClassWOWType.Mage);
                                eligibleClasses.Add(ClassWOWType.Warlock);
                                eligibleClasses.Add(ClassWOWType.Druid);
                            } break;
                        default: break;
                    }
                }
                else
                {
                    switch (weaponSubClassType)
                    {
                        case ItemWOWWeaponSubclassType.SwordOneHand: // 1H Slash
                        case ItemWOWWeaponSubclassType.AxeOneHand:
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Rogue]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Bard]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Druid]);
                            } break;
                        case ItemWOWWeaponSubclassType.SwordTwoHand: // 2H Slash
                        case ItemWOWWeaponSubclassType.AxeTwoHand:
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                            } break;
                        case ItemWOWWeaponSubclassType.FistWeapon: // 1H Blunt
                        case ItemWOWWeaponSubclassType.MaceOneHand:
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Monk]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Bard]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Shaman]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Cleric]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Druid]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Enchanter]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Magician]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Necromancer]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Wizard]);
                            } break;
                        case ItemWOWWeaponSubclassType.MaceTwoHand: // 2H Blunt
                        case ItemWOWWeaponSubclassType.Staff:
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Monk]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Shaman]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Cleric]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Druid]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Enchanter]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Magician]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Necromancer]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Wizard]);
                            } break;
                        case ItemWOWWeaponSubclassType.Dagger: // Pierce
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Bard]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Enchanter]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Magician]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Necromancer]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Rogue]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Shaman]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Wizard]);
                            } break;
                        case ItemWOWWeaponSubclassType.Polearm: // Pierce (2h, special)
                            {
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Paladin]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Warrior]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.ShadowKnight]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Shaman]);
                                eligibleClasses.UnionWith(WOWClassesByEQClass[ClassEQType.Ranger]);
                            } break;
                        default: break;
                    }
                }
                return eligibleClasses;
            }
        }

        public static ItemWOWArmorSubclassType GetArmorClassForItemWearableByEQClasses(List<ClassEQType> eqClasses)
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();

                // Determine the lowest armor class type, which can differ base on config
                int lowestItemWOWArmorSubClassType = (int)ItemWOWArmorSubclassType.Plate;
                if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
                {
                    foreach (ClassEQType eqClass in eqClasses)
                    {
                        if (WOWMaxArmorClassTypeByEQClass.ContainsKey(eqClass) == true)
                        {
                            if ((int)WOWMaxArmorClassTypeByEQClass[eqClass] < lowestItemWOWArmorSubClassType)
                                lowestItemWOWArmorSubClassType = (int)WOWMaxArmorClassTypeByEQClass[eqClass];
                        }
                    }
                }
                else
                {
                    // Get a list of all WOW classes
                    HashSet<ClassWOWType> wowClasses = new HashSet<ClassWOWType>();
                    foreach (ClassEQType eqClass in eqClasses)
                    {
                        if (WOWClassesByEQClass.ContainsKey(eqClass) == true)
                            wowClasses.UnionWith(WOWClassesByEQClass[eqClass]);
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

        public static HashSet<ClassWOWType> GetWOWClassesEligibleForArmorType(ItemWOWArmorSubclassType armorType)
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                HashSet<ClassWOWType> wowClasses = new HashSet<ClassWOWType>();
                
                // If armor is aligned, then use the EQ class list
                if (Configuration.PLAYER_SKILL_ENABLE_ALIGNED_ARMOR_TYPE_ON_ALL_CLASSES == true)
                {
                    switch (armorType)
                    {
                        case ItemWOWArmorSubclassType.Misc:
                        case ItemWOWArmorSubclassType.Cloth:
                            {
                                foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                                {
                                    if (wowClassType != ClassWOWType.All && wowClassType != ClassWOWType.None)
                                        wowClasses.Add(wowClassType);
                                }
                            } break;
                        default:
                            {
                                foreach (var wowMaxArmorClassTypeByEQClass in WOWMaxArmorClassTypeByEQClass)
                                {
                                    if ((int)wowMaxArmorClassTypeByEQClass.Value >= (int)armorType)
                                    {
                                        foreach (ClassWOWType wowClass in WOWClassesByEQClass[wowMaxArmorClassTypeByEQClass.Key])
                                            wowClasses.Add(wowClass);
                                    }
                                }
                            } break;
                    }
                }
                // Otherwise use the base WOW class alignments
                else
                {
                    switch (armorType)
                    {
                        case ItemWOWArmorSubclassType.Misc:
                        case ItemWOWArmorSubclassType.Cloth:
                            {
                                foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                                {
                                    if (wowClassType != ClassWOWType.All && wowClassType != ClassWOWType.None)
                                        wowClasses.Add(wowClassType);
                                }
                            } break;
                        case ItemWOWArmorSubclassType.Leather:
                            {
                                wowClasses.Add(ClassWOWType.Warrior);
                                wowClasses.Add(ClassWOWType.Paladin);
                                wowClasses.Add(ClassWOWType.Hunter);
                                wowClasses.Add(ClassWOWType.Rogue);
                                wowClasses.Add(ClassWOWType.DeathKnight);
                                wowClasses.Add(ClassWOWType.Shaman);
                                wowClasses.Add(ClassWOWType.Druid);
                            } break;
                        case ItemWOWArmorSubclassType.Mail:
                            {
                                wowClasses.Add(ClassWOWType.Warrior);
                                wowClasses.Add(ClassWOWType.Paladin);
                                wowClasses.Add(ClassWOWType.Hunter);
                                wowClasses.Add(ClassWOWType.DeathKnight);
                                wowClasses.Add(ClassWOWType.Shaman);
                            } break;
                        case ItemWOWArmorSubclassType.Plate:
                            {
                                wowClasses.Add(ClassWOWType.Warrior);
                                wowClasses.Add(ClassWOWType.Paladin);
                                wowClasses.Add(ClassWOWType.DeathKnight);
                            } break;
                        case ItemWOWArmorSubclassType.Shield:
                            {
                                wowClasses.Add(ClassWOWType.Warrior);
                                wowClasses.Add(ClassWOWType.Paladin);
                                wowClasses.Add(ClassWOWType.Shaman);
                                if (Configuration.PLAYER_SKILL_ENABLE_SHIELDS_ON_ALL_CLASSES == true)
                                {
                                    wowClasses.Clear();
                                    foreach (ClassWOWType wowClassType in Enum.GetValues(typeof(ClassWOWType)))
                                    {
                                        if (wowClassType != ClassWOWType.All && wowClassType != ClassWOWType.None)
                                            wowClasses.Add(wowClassType);
                                    }
                                }
                            } break;
                        default: break; // Nothing
                    }
                }

                return wowClasses;
            }
        }

        public static List<ClassWOWType> GetWOWClassesThatHaveArchery()
        {
            lock (ClassesLock)
            {
                if (WOWClassesByEQClass.Count == 0)
                    PopulateClassMap();
                return WOWClassesWhichShouldHaveArchery.ToList();
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
            {
                if (wowClassType != ClassWOWType.All && wowClassType != ClassWOWType.None)
                    EQClassesByWOWClass.Add(wowClassType, new List<ClassEQType>());
            }
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
