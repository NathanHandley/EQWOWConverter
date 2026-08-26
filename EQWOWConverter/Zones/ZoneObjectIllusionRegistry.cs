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
using EQWOWConverter.ObjectModels;
using System.Globalization;

namespace EQWOWConverter.Zones
{
    internal class ZoneObjectIllusionDisplay
    {
        public string ObjectModelName = string.Empty;
        public float Scale = 1f;
        public int DBCCreatureDisplayID = 0;
        public int DBCCreatureModelDataID = 0;
        public bool IsTree = false;

        public ZoneObjectIllusionDisplay(string objectModelName, float scale, int dbcCreatureDisplayID, int dbcCreatureModelDataID, bool isTree)
        {
            ObjectModelName = objectModelName;
            Scale = scale;
            DBCCreatureDisplayID = dbcCreatureDisplayID;
            DBCCreatureModelDataID = dbcCreatureModelDataID;
            IsTree = isTree;
        }
    }

    internal class ZoneObjectIllusionPlacement
    {
        public int MapID = 0;
        public Vector3 Position = new Vector3();
        public ZoneObjectIllusionDisplay Display;

        public ZoneObjectIllusionPlacement(int mapID, Vector3 position, ZoneObjectIllusionDisplay display)
        {
            MapID = mapID;
            Position = position;
            Display = display;
        }
    }

    internal static class ZoneObjectIllusionRegistry
    {
        private static readonly object RegistryLock = new object();
        private static SortedDictionary<string, ZoneObjectIllusionDisplay> DisplaysByObjectModelNameAndScale = new SortedDictionary<string, ZoneObjectIllusionDisplay>();
        private static SortedDictionary<string, int> ModelDataIDsByObjectModelName = new SortedDictionary<string, int>();
        private static List<ZoneObjectIllusionPlacement> Placements = new List<ZoneObjectIllusionPlacement>();
        private static HashSet<string> TreeObjectModelNames = new HashSet<string>();
        private static bool TreeObjectModelNamesLoaded = false;

        public static void PopulatePlacementsFromZones(List<Zone> zones)
        {
            lock (RegistryLock)
            {
                Placements.Clear();

                List<Zone> sortedZones = new List<Zone>(zones);
                sortedZones.Sort(CompareZonesByMapID);
                foreach (Zone zone in sortedZones)
                {
                    int mapID = Convert.ToInt32(zone.ZoneProperties.DBCMapID);
                    foreach (ZoneDoodadInstance doodadInstance in zone.DoodadInstances)
                    {
                        // Only real placed objects qualify
                        if (doodadInstance.DoodadType != ZoneDoodadInstanceType.StaticObject)
                            continue;
                        // Model names key the exported doodad folders, so they are used exactly as the doodad has them
                        string modelName = doodadInstance.ObjectName;
                        if (ObjectModel.StaticObjectModelsByName.ContainsKey(modelName) == false)
                            continue;

                        // Anything the player would not actually see is no good
                        ObjectModel objectModel = ObjectModel.StaticObjectModelsByName[modelName];
                        if (objectModel.Properties.RenderingEnabled == false || objectModel.ModelVertices.Count == 0)
                            continue;

                        // Doodad positions are in zone model space, and the zone WMO is placed into the map unrotated at the map center, so the world position is the X and Y flip of it
                        Vector3 worldPosition = new Vector3(-1 * doodadInstance.Position.X, -1 * doodadInstance.Position.Y, doodadInstance.Position.Z);

                        // The placed size carries over so the player matches what is standing there
                        ZoneObjectIllusionDisplay display = GetOrCreateDisplayNoLock(modelName, GetQuantizedPlacementScale(doodadInstance.Scale));
                        Placements.Add(new ZoneObjectIllusionPlacement(mapID, worldPosition, display));
                    }
                }
                Logger.WriteInfo(string.Concat("Zone object illusion registry captured ", Placements.Count.ToString(), " object placements, generating ",
                    DisplaysByObjectModelNameAndScale.Count.ToString(), " displays across ", ModelDataIDsByObjectModelName.Count.ToString(), " object models"));
            }
        }

        public static List<ZoneObjectIllusionDisplay> GetDisplays()
        {
            lock (RegistryLock)
            {
                return new List<ZoneObjectIllusionDisplay>(DisplaysByObjectModelNameAndScale.Values);
            }
        }

        public static List<ZoneObjectIllusionPlacement> GetPlacements()
        {
            lock (RegistryLock)
            {
                return new List<ZoneObjectIllusionPlacement>(Placements);
            }
        }

        private static int CompareZonesByMapID(Zone zoneA, Zone zoneB)
        {
            return zoneA.ZoneProperties.DBCMapID.CompareTo(zoneB.ZoneProperties.DBCMapID);
        }

        // Placed sizes are continuous in the EQ data (0.1 through 10), so they snap to a step to keep the number of generated display rows down
        private static float GetQuantizedPlacementScale(float placementScale)
        {
            float quantum = Configuration.SPELL_ILLUSION_OBJECT_SCALE_STEP_SIZE;
            if (quantum <= Configuration.GENERATE_FLOAT_EPSILON)
                return 1f;
            float quantizedScale = MathF.Round(placementScale / quantum) * quantum;
            if (quantizedScale < quantum)
                quantizedScale = quantum;
            return quantizedScale;
        }

        // One model data row per object model, and one display row per size that model is actually placed at
        private static ZoneObjectIllusionDisplay GetOrCreateDisplayNoLock(string modelName, float scale)
        {
            string scaleString = scale.ToString("0.00", CultureInfo.InvariantCulture);
            string displayKey = string.Concat(modelName, "|", scaleString);
            if (DisplaysByObjectModelNameAndScale.ContainsKey(displayKey) == true)
                return DisplaysByObjectModelNameAndScale[displayKey];
            LoadTreeObjectModelNamesIfNeeded();
            if (ModelDataIDsByObjectModelName.ContainsKey(modelName) == false)
                ModelDataIDsByObjectModelName.Add(modelName, IDGenerationTool.GenerateID("CreatureModelDataID", "illusionobject", modelName));
            int displayID = IDGenerationTool.GenerateID("CreatureDisplayInfoID", "illusionobject", modelName, scaleString);
            ZoneObjectIllusionDisplay newDisplay = new ZoneObjectIllusionDisplay(modelName, scale, displayID, ModelDataIDsByObjectModelName[modelName],
                TreeObjectModelNames.Contains(modelName.ToLower()));
            DisplaysByObjectModelNameAndScale.Add(displayKey, newDisplay);
            return newDisplay;
        }

        private static void LoadTreeObjectModelNamesIfNeeded()
        {
            if (TreeObjectModelNamesLoaded == true)
                return;
            TreeObjectModelNamesLoaded = true;
            string treeObjectsFileName = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "IllusionTreeObjects.csv");
            Logger.WriteDebug("Populating illusion tree object list via file " + treeObjectsFileName);
            foreach (string treeObjectName in FileTool.ReadAllStringLinesFromFile(treeObjectsFileName, true, true))
                TreeObjectModelNames.Add(treeObjectName.Trim().ToLower());
        }
    }
}
