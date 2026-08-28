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

namespace EQWOWConverter.Spells
{
    internal class SpellPetType
    {
        private static readonly object SpellPetTypeLock = new object();
        private static Dictionary<string, SpellPetType> SpellPetTypesByTypeName = new Dictionary<string, SpellPetType>();

        public string TypeName = string.Empty;
        public bool HasSingleTaunt = false;
        public bool HasMultiTaunt = false;

        public static SpellPetType? GetSpellPetTypeByTypeName(string typeName)
        {
            lock (SpellPetTypeLock)
            {
                if (SpellPetTypesByTypeName.Count == 0)
                    LoadSpellPetTypeData();
                if (SpellPetTypesByTypeName.ContainsKey(typeName) == true)
                    return SpellPetTypesByTypeName[typeName];
                Logger.WriteError("Could not find a pet type with name '", typeName, "'");
                return null;
            }
        }

        public static List<SpellPetType> GetAllSpellPetTypes()
        {
            lock (SpellPetTypeLock)
            {
                if (SpellPetTypesByTypeName.Count == 0)
                    LoadSpellPetTypeData();
                return SpellPetTypesByTypeName.Values.ToList();
            }
        }

        private static void LoadSpellPetTypeData()
        {
            string petTypeFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "PetTypes.csv");
            Logger.WriteDebug(string.Concat("Loading pet types via file '", petTypeFile, "'"));
            List<Dictionary<string, string>> petTypeRows = FileTool.ReadAllRowsFromFileWithHeader(petTypeFile, "|");
            foreach (Dictionary<string, string> columns in petTypeRows)
            {
                SpellPetType petType = new SpellPetType();
                petType.TypeName = columns["type"].Trim();
                petType.HasSingleTaunt = columns["HasSingleTaunt"].Trim() == "1";
                petType.HasMultiTaunt = columns["HasMultiTaunt"].Trim() == "1";
                if (SpellPetTypesByTypeName.ContainsKey(petType.TypeName) == true)
                {
                    Logger.WriteError("Pet type '", petType.TypeName, "' has more than one row");
                    continue;
                }
                SpellPetTypesByTypeName.Add(petType.TypeName, petType);
            }
            Logger.WriteDebug("Loading pet types complete");
        }
    }
}
