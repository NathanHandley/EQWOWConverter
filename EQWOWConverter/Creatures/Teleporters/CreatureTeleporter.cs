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

namespace EQWOWConverter.Creatures.Teleporters
{
    internal class CreatureTeleporter
    {
        public int WOWCreatureTemplateID = -1;
        public int WOWCreatureGUID = -1;
        public int CreatureModelDataID;
        public int MapID;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;
        public int MainHandWOWItemTemplateID;
        public int ChestWOWItemTemplateID;
        public int NPCSoundID;
        public int DisplayRaceID;
        public int DisplaySexID;
        public int SkinID;
        public int FaceID;
        public int HairStyleID;
        public int HairColorID;
        public int FacialHairID;

        private static List<CreatureTeleporter> CreatureTeleporters = new List<CreatureTeleporter>();

        public static readonly object CreatureTeleporterLock = new object();

        public static List<CreatureTeleporter> GetAllCreatureTeleporters()
        {
            lock (CreatureTeleporterLock)
            {
                if (CreatureTeleporters.Count == 0)
                    LoadCreatureTeleporters();
                return CreatureTeleporters;
            }
        }

        private static void LoadCreatureTeleporters()
        {
            CreatureTeleporters.Clear();
            string creatureTeleporterFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "TeleportCreaturesInAzeroth.csv");
            Logger.WriteDebug("Populating Creature Teleportes (in Azeroth) list via file '" + creatureTeleporterFileName + "'");
            List<Dictionary<string, string>> rows = FileTool.ReadAllRowsFromFileWithHeader(creatureTeleporterFileName, "|");
            foreach (Dictionary<string, string> columns in rows)
            {
                CreatureTeleporter newTeleporter = new CreatureTeleporter();
                newTeleporter.CreatureModelDataID = int.Parse(columns["ModelDataID"]);
                newTeleporter.MapID = int.Parse(columns["MapID"]);
                newTeleporter.XPosition = float.Parse(columns["X"]);
                newTeleporter.YPosition = float.Parse(columns["Y"]);
                newTeleporter.ZPosition = float.Parse(columns["Z"]);
                newTeleporter.Orientation = float.Parse(columns["O"]);
                newTeleporter.NPCSoundID = int.Parse(columns["NPCSoundID"]);
                newTeleporter.DisplayRaceID = int.Parse(columns["DisplayRaceID"]);
                newTeleporter.DisplaySexID = int.Parse(columns["DisplaySexID"]);
                newTeleporter.SkinID = int.Parse(columns["SkinID"]);
                newTeleporter.FaceID = int.Parse(columns["FaceID"]);
                newTeleporter.HairStyleID = int.Parse(columns["HairStyleID"]);
                newTeleporter.HairColorID = int.Parse(columns["HairColorID"]);
                newTeleporter.FacialHairID = int.Parse(columns["FacialHairID"]);
                newTeleporter.MainHandWOWItemTemplateID = int.Parse(columns["MainHandWOWItemTemplateID"]);
                newTeleporter.ChestWOWItemTemplateID = int.Parse(columns["ChestWOWItemTemplateID"]);
                CreatureTeleporters.Add(newTeleporter);
            }
        }
    }
}
