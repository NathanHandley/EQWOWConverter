//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
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

namespace EQWOWConverter.Spells
{
    internal class SpellPet
    {
        private static readonly object SpellPetLock = new object();
        private static Dictionary<string, SpellPet> SpellPetsByTypeName = new Dictionary<string, SpellPet>();

        public string TypeName = string.Empty;
        public int EQCreatureTemplateID = 0;
        public bool IsTemp = false;
        public SpellPetControlType ControlType = SpellPetControlType.NoControl;
        public SpellPetNamingType NamingType = SpellPetNamingType.Pet;
        public bool MonsterFlag = false;

        public static SpellPet? GetSpellPetByTypeName(string typeName)
        {
            lock (SpellPetLock)
            {
                if (SpellPetsByTypeName.Count == 0)
                    LoadSpellPetData();
                if (SpellPetsByTypeName.ContainsKey(typeName) == true)
                    return SpellPetsByTypeName[typeName];
                else
                {
                    Logger.WriteError("Could not find a spell pet type with name '", typeName);
                    return null;
                }
            }
        }

        public static List<string> GetAllSpellTypeNames()
        {
            lock (SpellPetLock)
            {
                if (SpellPetsByTypeName.Count == 0)
                    LoadSpellPetData();
                return SpellPetsByTypeName.Keys.ToList();
            }
        }

        private static void LoadSpellPetData()
        {
            string spellPetFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "SpellPets.csv");
            Logger.WriteDebug(string.Concat("Loading spell pets via file '", spellPetFile, "'"));
            List<Dictionary<string, string>> spellPetRows = FileTool.ReadAllRowsFromFileWithHeader(spellPetFile, "|");
            foreach (Dictionary<string, string> columns in spellPetRows)
            {
                SpellPet spellPet = new SpellPet();
                spellPet.TypeName = columns["type"];
                spellPet.EQCreatureTemplateID = Convert.ToInt32(columns["npcID"]);
                spellPet.IsTemp = columns["temp"] == "1";
                spellPet.ControlType = (SpellPetControlType)Convert.ToInt32(columns["petcontrol"]);
                spellPet.NamingType = (SpellPetNamingType)Convert.ToInt32(columns["petnaming"]);
                SpellPetsByTypeName.Add(spellPet.TypeName, spellPet);
            }
            Logger.WriteDebug(string.Concat("Loading spell pets complete"));
        }
    }
}
