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

namespace EQWOWConverter.Creatures.Teleporters
{
    internal class CreatureTeleportLocationNorrath
    {
        public string ZoneShortName = string.Empty;
        public bool AllowGood;
        public bool AllowNeutral;
        public bool AllowEvil;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;
        public string MenuItemText = string.Empty;

        private static List<CreatureTeleportLocationNorrath> TeleportLocations = new List<CreatureTeleportLocationNorrath>();
        private static HashSet<ClassType> GoodClasses = new HashSet<ClassType>();
        private static HashSet<ClassType> NeutralClasses = new HashSet<ClassType>();
        private static HashSet<ClassType> EvilClasses = new HashSet<ClassType>();
        private static HashSet<RaceType> GoodRaces = new HashSet<RaceType>();
        private static HashSet<RaceType> NeutralRaces = new HashSet<RaceType>();
        private static HashSet<RaceType> EvilRaces = new HashSet<RaceType>();

        public static readonly object AlignmentLock = new object();
        public static readonly object TeleporterLock = new object();

        public static List<CreatureTeleportLocationNorrath> GetAllTeleportLocations()
        {
            lock (TeleporterLock)
            {
                if (TeleportLocations.Count == 0)
                    LoadTeleportLocations();
                return TeleportLocations;
            }
        }

        public static List<ClassType> GetGoodClasses()
        {
            lock (AlignmentLock)
            {
                if (GoodClasses.Count == 0 && NeutralClasses.Count == 0 && EvilClasses.Count == 0)
                    LoadAlignmentInformation();
                return GoodClasses.ToList();
            }
        }

        public static List<ClassType> GetNeutralClasses()
        {
            lock (AlignmentLock)
            {
                if (GoodClasses.Count == 0 && NeutralClasses.Count == 0 && EvilClasses.Count == 0)
                    LoadAlignmentInformation();
                return NeutralClasses.ToList();
            }
        }

        public static List<ClassType> GetEvilClasses()
        {
            lock (AlignmentLock)
            {
                if (GoodClasses.Count == 0 && NeutralClasses.Count == 0 && EvilClasses.Count == 0)
                    LoadAlignmentInformation();
                return EvilClasses.ToList();
            }
        }

        public static List<RaceType> GetGoodRaces()
        {
            lock (AlignmentLock)
            {
                if (GoodRaces.Count == 0 && NeutralRaces.Count == 0 && EvilRaces.Count == 0)
                    LoadAlignmentInformation();
                return GoodRaces.ToList();
            }
        }

        public static List<RaceType> GetNeutralRaces()
        {
            lock (AlignmentLock)
            {
                if (GoodRaces.Count == 0 && NeutralRaces.Count == 0 && EvilRaces.Count == 0)
                    LoadAlignmentInformation();
                return NeutralRaces.ToList();
            }
        }

        public static List<RaceType> GetEvilRaces()
        {
            lock (AlignmentLock)
            {
                if (GoodRaces.Count == 0 && NeutralRaces.Count == 0 && EvilRaces.Count == 0)
                    LoadAlignmentInformation();
                return EvilRaces.ToList();
            }
        }

        //public static List<ClassType> GetClassTypes(bool includeGood, bool includeNeutral, bool includeEvil)
        //{
        //    lock (AlignmentLock)
        //    {
        //        if (GoodClasses.Count == 0 && NeutralClasses.Count == 0 && EvilClasses.Count == 0)
        //            LoadAlignmentInformation();
        //        HashSet<ClassType> returnClasses = new HashSet<ClassType>();
        //        if (includeGood == true)
        //            returnClasses.UnionWith(GoodClasses);
        //        if (includeNeutral == true)
        //            returnClasses.UnionWith(NeutralClasses);
        //        if (includeEvil == true)
        //            returnClasses.UnionWith(EvilClasses);
        //        return returnClasses.ToList();
        //    }
        //}

        //public static List<RaceType> GetRaceTypes(bool includeGood, bool includeNeutral, bool includeEvil)
        //{
        //    lock (AlignmentLock)
        //    {
        //        if (GoodRaces.Count == 0 && NeutralRaces.Count == 0 && EvilRaces.Count == 0)
        //            LoadAlignmentInformation();
        //        HashSet<RaceType> returnRaces = new HashSet<RaceType>();
        //        if (includeGood == true)
        //            returnRaces.UnionWith(GoodRaces);
        //        if (includeNeutral == true)
        //            returnRaces.UnionWith(NeutralRaces);
        //        if (includeEvil == true)
        //            returnRaces.UnionWith(EvilRaces);
        //        return returnRaces.ToList();
        //    }
        //}

        private static void LoadAlignmentInformation()
        {
            // Clear prior
            GoodClasses.Clear();
            NeutralClasses.Clear();
            EvilClasses.Clear();
            GoodRaces.Clear();
            NeutralRaces.Clear();
            EvilRaces.Clear();

            // Load in the class alignments
            string factionClassAlignmentFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionClassAlignment.csv");
            Logger.WriteDebug("Populating creature faction class alignments via file '" + factionClassAlignmentFile + "'");
            List<Dictionary<string, string>> classAlignmentRows = FileTool.ReadAllRowsFromFileWithHeader(factionClassAlignmentFile, "|");
            foreach (Dictionary<string, string> columns in classAlignmentRows)
            {
                ClassType classType = (ClassType)int.Parse(columns["ClassID"]);
                string alignmentString = columns["Alignment"].Trim().ToLower();
                switch (alignmentString)
                {
                    case "evil": EvilClasses.Add(classType); break;
                    case "good": GoodClasses.Add(classType); break;
                    case "neutral": NeutralClasses.Add(classType); break;
                    default: Logger.WriteError("In CreatureTeleportLocationNorrath, class alignment error as the alignment string '" + alignmentString + "' has no mapping"); break;
                }
            }

            // Load in the race alignments
            string factionRaceAlignmentFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "CreatureFactionRaceAlignment.csv");
            Logger.WriteDebug("Populating creature faction race alignments via file '" + factionRaceAlignmentFile + "'");
            List<Dictionary<string, string>> raceAlignmentRows = FileTool.ReadAllRowsFromFileWithHeader(factionRaceAlignmentFile, "|");
            foreach (Dictionary<string, string> columns in raceAlignmentRows)
            {
                RaceType raceType = (RaceType)int.Parse(columns["RaceID"]);
                string alignmentString = columns["Alignment"].Trim().ToLower();
                switch (alignmentString)
                {
                    case "evil": EvilRaces.Add(raceType); break;
                    case "good": GoodRaces.Add(raceType); break;
                    case "neutral": NeutralRaces.Add(raceType); break;
                    default: Logger.WriteError("In CreatureTeleportLocationNorrath, race alignment error, as the alignment string '" + alignmentString + "' has no mapping"); break;
                }
            }
        }

        private static void LoadTeleportLocations()
        {
            TeleportLocations.Clear();
            string teleportLocationFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TeleportLocationsToNorrath.csv");
            Logger.WriteDebug("Populating Norrath teleport locations list via file '" + teleportLocationFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(teleportLocationFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureTeleportLocationNorrath teleportLocation = new CreatureTeleportLocationNorrath();
                teleportLocation.ZoneShortName = columns["ZoneShortName"];
                teleportLocation.AllowGood = columns["AllowGood"].Trim() == "1" ? true : false;
                teleportLocation.AllowNeutral = columns["AllowNeutral"].Trim() == "1" ? true : false;
                teleportLocation.AllowEvil = columns["AllowEvil"].Trim() == "1" ? true : false;
                teleportLocation.XPosition = float.Parse(columns["X"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.YPosition = float.Parse(columns["Y"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.ZPosition = float.Parse(columns["Z"]) * Configuration.GENERATE_WORLD_SCALE;
                teleportLocation.Orientation = float.Parse(columns["O"]);
                teleportLocation.MenuItemText = columns["MenuItemText"];
                TeleportLocations.Add(teleportLocation);
            }
        }
    }
}

