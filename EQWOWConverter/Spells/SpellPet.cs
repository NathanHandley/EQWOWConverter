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
        public int MainhandItemIDWOW = 0;
        public int OffhandItemIDWOW = 0;
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
                spellPet.MainhandItemIDWOW = int.Parse(columns["itemIDWOW_main"]);
                spellPet.OffhandItemIDWOW = int.Parse(columns["itemIDWOW_off"]);
                SpellPetsByTypeName.Add(spellPet.TypeName, spellPet);
            }
            Logger.WriteDebug(string.Concat("Loading spell pets complete"));
        }

        public static List<string> GetRandomPetNamePrefixes()
        {
            List<string> prefixes = new List<string>();
            prefixes.Add("G");
            prefixes.Add("J");
            prefixes.Add("K");
            prefixes.Add("L");
            prefixes.Add("V");
            prefixes.Add("X");
            prefixes.Add("Z");
            return prefixes;
        }

        public static List<string> GetRandomPetNameSuffixes()
        {
            List<string> suffixes = new List<string>();
            suffixes.Add("abab");
            suffixes.Add("abanab");
            suffixes.Add("abaner");
            suffixes.Add("abann");
            suffixes.Add("abantik");
            suffixes.Add("abarab");
            suffixes.Add("abarer");
            suffixes.Add("abarn");
            suffixes.Add("abartik");
            suffixes.Add("abekab");
            suffixes.Add("abeker");
            suffixes.Add("abekn");
            suffixes.Add("aber");
            suffixes.Add("abn");
            suffixes.Add("abobab");
            suffixes.Add("abober");
            suffixes.Add("abobn");
            suffixes.Add("abobtik");
            suffixes.Add("abtik");
            suffixes.Add("anab");
            suffixes.Add("aner");
            suffixes.Add("ann");
            suffixes.Add("antik");
            suffixes.Add("arab");
            suffixes.Add("aranab");
            suffixes.Add("araner");
            suffixes.Add("arann");
            suffixes.Add("arantik");
            suffixes.Add("ararab");
            suffixes.Add("ararer");
            suffixes.Add("ararn");
            suffixes.Add("arartik");
            suffixes.Add("arekab");
            suffixes.Add("areker");
            suffixes.Add("arekn");
            suffixes.Add("arer");
            suffixes.Add("arn");
            suffixes.Add("arobab");
            suffixes.Add("arober");
            suffixes.Add("arobn");
            suffixes.Add("arobtik");
            suffixes.Add("artik");
            suffixes.Add("asab");
            suffixes.Add("asanab");
            suffixes.Add("asaner");
            suffixes.Add("asann");
            suffixes.Add("asantik");
            suffixes.Add("asarab");
            suffixes.Add("asarer");
            suffixes.Add("asarn");
            suffixes.Add("asartik");
            suffixes.Add("asekab");
            suffixes.Add("aseker");
            suffixes.Add("asekn");
            suffixes.Add("aser");
            suffixes.Add("asn");
            suffixes.Add("asobab");
            suffixes.Add("asober");
            suffixes.Add("asobn");
            suffixes.Add("asobtik");
            suffixes.Add("astik");
            suffixes.Add("ebab");
            suffixes.Add("ebanab");
            suffixes.Add("ebaner");
            suffixes.Add("ebann");
            suffixes.Add("ebantik");
            suffixes.Add("ebarab");
            suffixes.Add("ebarer");
            suffixes.Add("ebarn");
            suffixes.Add("ebartik");
            suffixes.Add("ebekab");
            suffixes.Add("ebeker");
            suffixes.Add("ebekn");
            suffixes.Add("eber");
            suffixes.Add("ebn");
            suffixes.Add("ebobab");
            suffixes.Add("ebober");
            suffixes.Add("ebobn");
            suffixes.Add("ebobtik");
            suffixes.Add("ebtik");
            suffixes.Add("ekab");
            suffixes.Add("eker");
            suffixes.Add("ekn");
            suffixes.Add("enab");
            suffixes.Add("enanab");
            suffixes.Add("enaner");
            suffixes.Add("enann");
            suffixes.Add("enantik");
            suffixes.Add("enarab");
            suffixes.Add("enarer");
            suffixes.Add("enarn");
            suffixes.Add("enartik");
            suffixes.Add("enekab");
            suffixes.Add("eneker");
            suffixes.Add("enekn");
            suffixes.Add("ener");
            suffixes.Add("enn");
            suffixes.Add("enobab");
            suffixes.Add("enober");
            suffixes.Add("enobn");
            suffixes.Add("enobtik");
            suffixes.Add("entik");
            suffixes.Add("ibab");
            suffixes.Add("ibanab");
            suffixes.Add("ibaner");
            suffixes.Add("ibann");
            suffixes.Add("ibantik");
            suffixes.Add("ibarab");
            suffixes.Add("ibarer");
            suffixes.Add("ibarn");
            suffixes.Add("ibartik");
            suffixes.Add("ibekab");
            suffixes.Add("ibeker");
            suffixes.Add("ibekn");
            suffixes.Add("iber");
            suffixes.Add("ibn");
            suffixes.Add("ibobab");
            suffixes.Add("ibober");
            suffixes.Add("ibobn");
            suffixes.Add("ibobtik");
            suffixes.Add("ibtik");
            suffixes.Add("obab");
            suffixes.Add("obanab");
            suffixes.Add("obaner");
            suffixes.Add("obann");
            suffixes.Add("obantik");
            suffixes.Add("obarab");
            suffixes.Add("obarer");
            suffixes.Add("obarn");
            suffixes.Add("obartik");
            suffixes.Add("obekab");
            suffixes.Add("obeker");
            suffixes.Add("obekn");
            suffixes.Add("ober");
            suffixes.Add("obn");
            suffixes.Add("obobab");
            suffixes.Add("obober");
            suffixes.Add("obobn");
            suffixes.Add("obobtik");
            suffixes.Add("obtik");
            suffixes.Add("onab");
            suffixes.Add("onanab");
            suffixes.Add("onaner");
            suffixes.Add("onann");
            suffixes.Add("onantik");
            suffixes.Add("onarab");
            suffixes.Add("onarer");
            suffixes.Add("onarn");
            suffixes.Add("onartik");
            suffixes.Add("onekab");
            suffixes.Add("oneker");
            suffixes.Add("onekn");
            suffixes.Add("oner");
            suffixes.Add("onn");
            suffixes.Add("onobab");
            suffixes.Add("onober");
            suffixes.Add("onobn");
            suffixes.Add("onobtik");
            suffixes.Add("ontik");
            suffixes.Add("tik");
            return suffixes;
        }
    }
}
