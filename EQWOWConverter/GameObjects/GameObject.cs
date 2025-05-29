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

using EQWOWConverter.Common;
using EQWOWConverter.ObjectModels;
using EQWOWConverter.ObjectModels.Properties;
using EQWOWConverter.Transports;
using EQWOWConverter.WOWFiles;

namespace EQWOWConverter.GameObjects
{
    internal class GameObject
    {
        protected static Dictionary<string, List<GameObject>> GameObjectsByZoneShortname = new Dictionary<string, List<GameObject>>();
        protected static readonly object GameObjectsLock = new object();
        protected static Dictionary<string, ObjectModel> ObjectModelByName = new Dictionary<string, ObjectModel>();

        public int ID;
        public int DoorID;
        public GameObjectType ObjectType = GameObjectType.Unknown;
        public GameObjectOpenType OpenType = GameObjectOpenType.Unknown;
        public string ZoneShortName = string.Empty;
        public string ModelName = string.Empty;
        public float Scale = 1.0f;
        public Vector3 Position = new Vector3();
        public float Orientation;
        public ObjectModel? ObjectModel = null;

        public static Dictionary<string, List<GameObject>> GetAllGameObjectsByZoneShortNames()
        {
            lock (GameObjectsLock)
            {
                if (GameObjectsByZoneShortname.Count == 0)
                    LoadGameObjects();
                return GameObjectsByZoneShortname;
            }
        }

        public static void LoadModelObjectsForGameObjects()
        {
            Logger.WriteInfo("Loading model objects for game objects...");
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            string objectsFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");

            Dictionary<string, List<GameObject>> allGameObjectsByZoneShortName = GetAllGameObjectsByZoneShortNames();
            foreach (var gameObjectByShortName in allGameObjectsByZoneShortName)
            {
                foreach (GameObject gameObject in gameObjectByShortName.Value)
                {
                    if (gameObject.ObjectModel != null)
                    {
                        Logger.WriteError("Attempted to LoadModelObjects for GameObject, but one already had a model");
                        return;
                    }

                    // Reuse an assigned, otherwise load
                    if (ObjectModelByName.ContainsKey(gameObject.ModelName) == true)
                        gameObject.ObjectModel = ObjectModelByName[gameObject.ModelName];
                    else
                    {
                        // Load it
                        ObjectModel curObjectModel = new ObjectModel(gameObject.ModelName, new ObjectModelProperties(), ObjectModelType.StaticDoodad);
                        Logger.WriteDebug("- [" + gameObject.ModelName + "]: Importing EQ transport lift trigger object '" + gameObject.ModelName + "'");
                        curObjectModel.LoadEQObjectFromFile(objectsFolderRoot, gameObject.ModelName); // TODO: Animation
                        Logger.WriteDebug("- [" + gameObject.ModelName + "]: Importing EQ transport lift trigger object '" + gameObject.ModelName + "' complete");
                        // TODO: Sound

                        // Create the M2 and Skin
                        string relativeMPQPath = Path.Combine("World", "Everquest", "GameObjects", gameObject.ModelName);
                        M2 objectM2 = new M2(curObjectModel, relativeMPQPath);
                        string curGameObjectOutputFolder = Path.Combine(exportMPQRootFolder, "World", "Everquest", "GameObjects", gameObject.ModelName);
                        objectM2.WriteToDisk(curObjectModel.Name, curGameObjectOutputFolder);

                        // Place the related textures
                        string objectTextureFolder = Path.Combine(objectsFolderRoot, "textures");
                        foreach (ObjectModelTexture texture in curObjectModel.ModelTextures)
                        {
                            string inputTextureName = Path.Combine(objectTextureFolder, texture.TextureName + ".blp");
                            string outputTextureName = Path.Combine(curGameObjectOutputFolder, texture.TextureName + ".blp");
                            if (Path.Exists(inputTextureName) == false)
                            {
                                Logger.WriteError("- [" + curObjectModel.Name + "]: Error Texture named '" + texture.TextureName + ".blp' not found.  Did you run blpconverter?");
                                return;
                            }
                            FileTool.CopyFile(inputTextureName, outputTextureName);
                            Logger.WriteDebug("- [" + curObjectModel.Name + "]: Texture named '" + texture.TextureName + ".blp' copied");
                        }

                        // Store it
                        //int gameObjectDisplayInfoID = GameObjectDisplayInfoDBC.GenerateID();
                        gameObject.ObjectModel = curObjectModel;
                        ObjectModelByName.Add(gameObject.ModelName, curObjectModel);
                    }
                }
            }
        }

        private static void LoadGameObjects()
        {
            // Load the file first
            string gameObjectsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "GameObjects.csv");
            Logger.WriteDebug(string.Concat("Populating Game Object list via file '", gameObjectsFile, "'"));
            List<Dictionary<string, string>> gameObjectsRows = FileTool.ReadAllRowsFromFileWithHeader(gameObjectsFile, "|");

            // Process the rows
            foreach(Dictionary<string, string> gameObjectsRow in gameObjectsRows)
            {
                GameObject newGameObject = new GameObject();
                newGameObject.ID = int.Parse(gameObjectsRow["id"]);
                newGameObject.DoorID = int.Parse(gameObjectsRow["doorid"]);
                newGameObject.ObjectType = GetType(gameObjectsRow["type"]);
                newGameObject.OpenType = GetOpenType(int.Parse(gameObjectsRow["opentype"]));
                newGameObject.ZoneShortName = gameObjectsRow["zone"];
                newGameObject.ModelName = gameObjectsRow["name"];
                float xPosition = float.Parse(gameObjectsRow["pos_x"]) * Configuration.GENERATE_WORLD_SCALE;
                float yPosition = float.Parse(gameObjectsRow["pos_y"]) * Configuration.GENERATE_WORLD_SCALE;
                float zPosition = float.Parse(gameObjectsRow["pos_z"]) * Configuration.GENERATE_WORLD_SCALE;
                newGameObject.Position = new Vector3(xPosition, yPosition, zPosition);
                float eqHeading = float.Parse(gameObjectsRow["heading"]);
                float wowHeading = 0;
                if (eqHeading != 0)
                    wowHeading = eqHeading / (256f / 360f);
                newGameObject.Orientation = wowHeading;
                newGameObject.Scale = float.Parse(gameObjectsRow["size"]) / 100f;

                // Add it
                if (GameObjectsByZoneShortname.ContainsKey(newGameObject.ZoneShortName) == false)
                    GameObjectsByZoneShortname.Add(newGameObject.ZoneShortName, new List<GameObject>());
                GameObjectsByZoneShortname[newGameObject.ZoneShortName].Add(newGameObject);
            }
        }

        private static GameObjectOpenType GetOpenType(int openTypeID)
        {
            switch (openTypeID)
            {
                case 0: return GameObjectOpenType.TYPE0;
                case 1: return GameObjectOpenType.TYPE1;
                case 2: return GameObjectOpenType.TYPE2;
                case 5: return GameObjectOpenType.TYPE5;
                case 6: return GameObjectOpenType.TYPE6;
                case 7: return GameObjectOpenType.TYPE7;
                case 10: return GameObjectOpenType.TYPE10;
                case 12: return GameObjectOpenType.TYPE12;
                case 15: return GameObjectOpenType.TYPE15;
                case 16: return GameObjectOpenType.TYPE16;
                case 17: return GameObjectOpenType.TYPE17;
                case 21: return GameObjectOpenType.TYPE21;
                case 22: return GameObjectOpenType.TYPE22;
                case 25: return GameObjectOpenType.TYPE25;
                case 26: return GameObjectOpenType.TYPE26;
                case 30: return GameObjectOpenType.TYPE30;
                case 35: return GameObjectOpenType.TYPE35;
                case 40: return GameObjectOpenType.TYPE40;
                case 45: return GameObjectOpenType.TYPE45;
                case 53: return GameObjectOpenType.TYPE53;
                case 54: return GameObjectOpenType.TYPE54;
                case 55: return GameObjectOpenType.TYPE55;
                case 56: return GameObjectOpenType.TYPE56;
                case 57: return GameObjectOpenType.TYPE57;
                case 58: return GameObjectOpenType.TYPE58;
                case 59: return GameObjectOpenType.TYPE59;
                case 60: return GameObjectOpenType.TYPE60;
                case 61: return GameObjectOpenType.TYPE61;
                case 65: return GameObjectOpenType.TYPE65;
                case 66: return GameObjectOpenType.TYPE66;
                case 70: return GameObjectOpenType.TYPE70;
                case 71: return GameObjectOpenType.TYPE71;
                case 72: return GameObjectOpenType.TYPE72;
                case 74: return GameObjectOpenType.TYPE74;
                case 75: return GameObjectOpenType.TYPE75;
                case 76: return GameObjectOpenType.TYPE76;
                case 77: return GameObjectOpenType.TYPE77;
                case 100: return GameObjectOpenType.TYPE100;
                case 101: return GameObjectOpenType.TYPE101;
                case 105: return GameObjectOpenType.TYPE105;
                case 106: return GameObjectOpenType.TYPE106;
                case 120: return GameObjectOpenType.TYPE120;
                case 125: return GameObjectOpenType.TYPE125;
                case 130: return GameObjectOpenType.TYPE130;
                case 140: return GameObjectOpenType.TYPE140;
                case 145: return GameObjectOpenType.TYPE145;
                case 156: return GameObjectOpenType.TYPE156;
                default:
                    {
                        Logger.WriteError("Can't determine GameObjectOpenType due to an unmapped open type id of " + openTypeID.ToString());
                        return GameObjectOpenType.Unknown;
                    }
            }
        }

        private static GameObjectType GetType(string typeNameValue)
        {
            switch (typeNameValue.Trim().ToLower())
            {
                case "noninteract": return GameObjectType.NonInteract;
                case "door": return GameObjectType.Door;
                case "boat": return GameObjectType.Boat;
                case "bridge": return GameObjectType.Bridge;
                case "lever": return GameObjectType.Lever;
                case "teleport": return GameObjectType.Teleport;
                case "trap": return GameObjectType.Trap;
                default:
                    {
                        Logger.WriteError("Can't determine GameObjectType due to an unmapped open type name value of " + typeNameValue);
                        return GameObjectType.Unknown;
                    }

            }
        }
    }
}
