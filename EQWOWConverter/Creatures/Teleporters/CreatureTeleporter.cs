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
        public int MapID;
        public int AreaID;
        public float XPosition;
        public float YPosition;
        public float ZPosition;
        public float Orientation;

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
                newTeleporter.MapID = int.Parse(columns["MapID"]);
                newTeleporter.AreaID = int.Parse(columns["AreaID"]);
                newTeleporter.XPosition = float.Parse(columns["X"]);
                newTeleporter.YPosition = float.Parse(columns["Y"]);
                newTeleporter.ZPosition = float.Parse(columns["Z"]);
                newTeleporter.Orientation = float.Parse(columns["O"]);
                CreatureTeleporters.Add(newTeleporter);
            }
        }
    }
}
