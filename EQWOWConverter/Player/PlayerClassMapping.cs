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

namespace EQWOWConverter.Player
{
    internal class PlayerClassMapping
    {
        private static Dictionary<ClassWOWType, PlayerClassMapping> ClassMappingByWOWClass = new Dictionary<ClassWOWType, PlayerClassMapping>();
        private static readonly object READ_LOCK = new object();

        public ClassWOWType WOWClass = ClassWOWType.None;
        public ClassEQType BaseEQClass = ClassEQType.Warrior;
        public ClassEQType DefaultSecondEQClass = ClassEQType.Warrior;
        public List<ClassEQType> AllowedSecondEQClasses = new List<ClassEQType>();

        public static Dictionary<ClassWOWType, PlayerClassMapping> GetClassMappingsByWOWClass()
        {
            lock (READ_LOCK)
            {
                if (ClassMappingByWOWClass.Count == 0)
                    PopulateClassMapping();
                return ClassMappingByWOWClass;
            }
        }

        private static void PopulateClassMapping()
        {
            ClassMappingByWOWClass.Clear();
            string classMappingFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PlayerEQClassProperties.csv");
            Logger.WriteDebug(string.Concat("Populating EQ Class Mapping via file '", classMappingFile, "'"));
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(classMappingFile, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                PlayerClassMapping classMapping = new PlayerClassMapping();
                classMapping.WOWClass = GetWOWClassFromString(columns["WOW_Class"]);
                classMapping.BaseEQClass = GetEQClassFromString(columns["EQ_Class_Base"]);
                classMapping.DefaultSecondEQClass = GetEQClassFromString(columns["EQ_Class_Default_Second"]);
                if (columns["Bard"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Bard);
                if (columns["Cleric"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Cleric);
                if (columns["Druid"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Druid);
                if (columns["Enchanter"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Enchanter);
                if (columns["Magician"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Magician);
                if (columns["Monk"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Monk);
                if (columns["Necromancer"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Necromancer);
                if (columns["Paladin"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Paladin);
                if (columns["Ranger"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Ranger);
                if (columns["Rogue"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Rogue);
                if (columns["ShadowKnight"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.ShadowKnight);
                if (columns["Shaman"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Shaman);
                if (columns["Warrior"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Warrior);
                if (columns["Wizard"].Trim() == "1")
                    classMapping.AllowedSecondEQClasses.Add(ClassEQType.Wizard);

                // Remove base class from the allowed classes to simplify things
                if (classMapping.AllowedSecondEQClasses.Contains(classMapping.BaseEQClass))
                    classMapping.AllowedSecondEQClasses.Remove(classMapping.BaseEQClass);

                // Unique entries only
                if (ClassMappingByWOWClass.ContainsKey(classMapping.WOWClass) == true)
                    Logger.WriteError("PlayerClassMapping attempted to add more than one wowclass '", classMapping.WOWClass.ToString(), "'");
                else
                    ClassMappingByWOWClass.Add(classMapping.WOWClass, classMapping);
            }
        }

        private static ClassWOWType GetWOWClassFromString(string inputString)
        {
            switch (inputString.ToLower().Trim())
            {
                case "deathknight": return ClassWOWType.DeathKnight;
                case "druid": return ClassWOWType.Druid;
                case "hunter": return ClassWOWType.Hunter;
                case "mage": return ClassWOWType.Mage;
                case "rogue": return ClassWOWType.Rogue;
                case "paladin": return ClassWOWType.Paladin;
                case "priest": return ClassWOWType.Priest;
                case "shaman": return ClassWOWType.Shaman;
                case "warlock": return ClassWOWType.Warlock;
                case "warrior": return ClassWOWType.Warrior;
                default:
                    {
                        Logger.WriteError("Unable to convert WOW Class from '", inputString, "'");
                        return ClassWOWType.None;
                    }
            }
        }

        private static ClassEQType GetEQClassFromString(string inputString)
        {
            switch (inputString.ToLower().Trim())
            {
                case "bard": return ClassEQType.Bard;
                case "cleric": return ClassEQType.Cleric;
                case "druid": return ClassEQType.Druid;
                case "enchanter": return ClassEQType.Enchanter;
                case "magician": return ClassEQType.Magician;
                case "monk": return ClassEQType.Monk;
                case "necromancer": return ClassEQType.Necromancer;
                case "paladin": return ClassEQType.Paladin;
                case "ranger": return ClassEQType.Ranger;
                case "rogue": return ClassEQType.Rogue;
                case "shadowknight": return ClassEQType.ShadowKnight;
                case "shaman": return ClassEQType.Shaman;
                case "warrior": return ClassEQType.Warrior;
                case "wizard": return ClassEQType.Wizard;
                default:
                    {
                        Logger.WriteError("Unable to convert EQ Class from '", inputString, "'");
                        return ClassEQType.Warrior;
                    }
            }
        }
    }
}
