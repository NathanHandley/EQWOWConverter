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
using EQWOWConverter.WOWFiles;
using EQWOWConverter.Zones;

namespace EQWOWConverter.GameObjects
{
    internal class GameObject
    {
        protected static Dictionary<string, List<GameObject>> NonDoodadGameObjectsByZoneShortname = new Dictionary<string, List<GameObject>>();
        protected static Dictionary<string, List<GameObject>> DoodadGameObjectsByZoneShortname = new Dictionary<string, List<GameObject>>();
        protected static readonly object GameObjectsLock = new object();
        protected static Dictionary<(string, GameObjectOpenType), ObjectModel> NonDoodadObjectModelsByNameAndOpenType = new Dictionary<(string, GameObjectOpenType), ObjectModel>();
        protected static Dictionary<(string, GameObjectOpenType), int> GameObjectDisplayInfoIDsByModelNameAndOpenType = new Dictionary<(string, GameObjectOpenType), int>();
        protected static Dictionary<string, List<string>> SourceStaticModelNamesByZoneShortName = new Dictionary<string, List<string>>();
        protected static Dictionary<string, List<string>> SourceSkeletalModelNamesByZoneShortName = new Dictionary<string, List<string>>();
        public static Dictionary<(string, GameObjectOpenType), Sound> OpenSoundsByModelNameAndOpenType = new Dictionary<(string, GameObjectOpenType), Sound>();
        public static Dictionary<(string, GameObjectOpenType), Sound> CloseSoundsByModelNameAndOpenType = new Dictionary<(string, GameObjectOpenType), Sound>();
        public static Dictionary<string, Sound> AllSoundsBySoundName = new Dictionary<string, Sound>();
        protected static bool DataIsLoaded = false;

        public int ID;
        public int DoorID;
        public int TriggerDoorID;
        public GameObjectType ObjectType = GameObjectType.Unknown;
        public GameObjectOpenType OpenType = GameObjectOpenType.Unknown;
        public GameObjectTradeskillFocusType TradeskillFocusType = GameObjectTradeskillFocusType.None;
        public string ZoneShortName = string.Empty;
        public string OriginalModelName = string.Empty;
        public string ModelName = string.Empty;
        public bool ModelIsSkeletal = false;
        public bool ModelIsInEquipmentFolder = false;
        public bool HasColission = false;
        public bool RenderingEnabled = true;
        public string DisplayName = string.Empty;
        public float Scale = 1.0f;
        public Vector3 Position = new Vector3();
        public float Orientation;
        public float EQHeading;
        public float EQIncline;
        public ObjectModel? ObjectModel = null;
        public int GameObjectGUID;
        public int GameObjectTemplateEntryID;
        public int TriggerGameObjectGUID = 0;
        public int TriggerGameObjectTemplateEntryID = 0;
        public int GameObjectDisplayInfoID = -1;
        public Sound? OpenSound = null;
        public Sound? CloseSound = null;
        public bool LoadAsZoneDoodad = false;
        public int CloseTimeInMS;

        public static Dictionary<string, List<GameObject>> GetNonDoodadGameObjectsByZoneShortNames()
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                return NonDoodadGameObjectsByZoneShortname;
            }
        }

        public static Dictionary<string, List<GameObject>> GetDoodadGameObjectsByZoneShortNames()
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                return DoodadGameObjectsByZoneShortname;
            }
        }

        public static List<GameObject> GetDoodadGameObjectsForZoneShortname(string zoneShortName)
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                if (DoodadGameObjectsByZoneShortname.ContainsKey(zoneShortName) == false)
                    return new List<GameObject>();
                else
                    return DoodadGameObjectsByZoneShortname[zoneShortName];
            }
        }

        public static Dictionary<string, List<string>> GetSourceStaticModelNamesByZoneShortName()
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                return SourceStaticModelNamesByZoneShortName;
            }
        }

        public static Dictionary<string, List<string>> GetSourceSkeletalModelNamesByZoneShortName()
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                return SourceSkeletalModelNamesByZoneShortName;
            }
        }

        public static List<GameObject> GetAllDoodadGameObjects()
        {
            lock (GameObjectsLock)
            {
                LoadGameObjects();
                List<GameObject> returnGameObjects = new List<GameObject>();
                foreach (var objectsByZone in DoodadGameObjectsByZoneShortname)
                    returnGameObjects.AddRange(objectsByZone.Value);
                return returnGameObjects;
            }
        }

        public static Dictionary<(string, GameObjectOpenType), int> GetGameObjectDisplayInfoIDsByModelNameAndOpenType()
        {
            lock (GameObjectsLock)
            {
                if (GameObjectDisplayInfoIDsByModelNameAndOpenType.Count == 0)
                    Logger.WriteError("GameObjectDisplayInfoIDsByModelNameAndOpenType called before models were loaded");
                return GameObjectDisplayInfoIDsByModelNameAndOpenType;
            }
        }

        public static Dictionary<(string, GameObjectOpenType), ObjectModel> GetNonDoodadObjectModelsByNameAndOpenType()
        {
            lock (GameObjectsLock)
            {
                if (NonDoodadObjectModelsByNameAndOpenType.Count == 0)
                    Logger.WriteError("GetNonDoodadObjectModelsByNameAndOpenType called before models were loaded");
                return NonDoodadObjectModelsByNameAndOpenType;
            }
        }

        public static void LoadModelObjectsForNonDoodadGameObjects()
        {
            Logger.WriteInfo("Loading model objects for non doodad game objects...");
            string eqExportsConditionedPath = Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER;
            string exportMPQRootFolder = Path.Combine(Configuration.PATH_EXPORT_FOLDER, "MPQReady");
            string objectsFolderRoot = Path.Combine(eqExportsConditionedPath, "objects");
            string equipmentFolderRoot = Path.Combine(eqExportsConditionedPath, "equipment");

            // Clear the folder first
            string gameObjectOutputFolderRoot = Path.Combine(exportMPQRootFolder, "World", "Everquest", "GameObjects");
            if (Directory.Exists(gameObjectOutputFolderRoot))
                Directory.Delete(gameObjectOutputFolderRoot, true);
            Directory.CreateDirectory(gameObjectOutputFolderRoot);

            // Process the objects
            Dictionary<string, List<GameObject>> allNonDoodadGameObjectsByZoneShortName = GetNonDoodadGameObjectsByZoneShortNames();
            foreach (var gameObjectByShortName in allNonDoodadGameObjectsByZoneShortName)
            {
                foreach (GameObject gameObject in gameObjectByShortName.Value)
                {
                    if (gameObject.ObjectModel != null)
                    {
                        Logger.WriteError("Attempted to LoadModelObjectsForNonDoodadGameObjects for GameObject, but one already had a model");
                        return;
                    }

                    // Skip non-interactive
                    if (gameObject.ObjectType == GameObjectType.NonInteract)
                        continue;

                    // Reuse an assigned, otherwise load
                    if (NonDoodadObjectModelsByNameAndOpenType.ContainsKey((gameObject.OriginalModelName, gameObject.OpenType)) == true)
                    {
                        gameObject.ObjectModel = NonDoodadObjectModelsByNameAndOpenType[(gameObject.OriginalModelName, gameObject.OpenType)];
                        gameObject.GameObjectDisplayInfoID = GameObjectDisplayInfoIDsByModelNameAndOpenType[(gameObject.OriginalModelName, gameObject.OpenType)];
                    }
                    else
                    {
                        // Determine folder to load from
                        string modelDataRootFolder = objectsFolderRoot;
                        if (gameObject.ModelIsInEquipmentFolder == true)
                            modelDataRootFolder = equipmentFolderRoot;

                        // Tradeskill items have an atypically small visibility range
                        float objectVisibilityBoundingBoxMinSize = Configuration.GENERATE_OBJECT_MODEL_MIN_BOUNDARY_BOX_SIZE;
                        if (gameObject.TradeskillFocusType != GameObjectTradeskillFocusType.None)
                            objectVisibilityBoundingBoxMinSize = Configuration.OBJECT_GAMEOBJECT_TRADESKILLFOCUS_EFFECT_AREA_MIN_SIZE;
                        else if (gameObject.ObjectType == GameObjectType.Door || gameObject.ObjectType == GameObjectType.Bridge)
                            objectVisibilityBoundingBoxMinSize = Configuration.OBJECT_GAMEOBJECT_DOOR_INTERACT_BOUNDARY_MIN_SIZE;

                        // Load it
                        string modelFileName = string.Concat(gameObject.OriginalModelName, "_", gameObject.OpenType.ToString());
                        ObjectModel curObjectModel;
                        Logger.WriteDebug("- [" + gameObject.OriginalModelName + "]: Importing EQ game object '" + gameObject.OriginalModelName + "'");
                        switch(gameObject.ObjectType)
                        {
                            case GameObjectType.Door:
                                {
                                    switch (gameObject.OpenType)
                                    {
                                        case GameObjectOpenType.TYPE0:
                                        case GameObjectOpenType.TYPE1:
                                        case GameObjectOpenType.TYPE2:                                        
                                            {
                                                ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateRotateAroundZClockwiseQuarter, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                                curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                                curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                            } break;
                                        case GameObjectOpenType.TYPE5:
                                        case GameObjectOpenType.TYPE6:
                                        case GameObjectOpenType.TYPE7:
                                            {
                                                ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateRotateAroundZCounterclockwiseQuarter, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                                curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                                curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                            } break;
                                        case GameObjectOpenType.TYPE12:
                                        case GameObjectOpenType.TYPE15:
                                        case GameObjectOpenType.TYPE17:
                                        case GameObjectOpenType.TYPE26:                                        
                                            {
                                                ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateSlideLeft, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                                curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                                curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                            } break;
                                        case GameObjectOpenType.TYPE60: 
                                        case GameObjectOpenType.TYPE61: 
                                        case GameObjectOpenType.TYPE65:
                                        case GameObjectOpenType.TYPE66: 
                                        case GameObjectOpenType.TYPE70:
                                        case GameObjectOpenType.TYPE75:
                                        case GameObjectOpenType.TYPE76:
                                            {
                                                ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateSlideUp, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                                curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                                curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                            } break;
                                        case GameObjectOpenType.TYPE16:
                                            {
                                                ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateRotateUpOpen, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                                curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                                curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                            } break;
                                        case GameObjectOpenType.TYPE10: // TODO: Figure this out, Thurgadin Door (probably slide).  Velious.
                                        case GameObjectOpenType.TYPE21: // TODO: two in CityMist. CMGATE101
                                        case GameObjectOpenType.TYPE22: // TODO: Only in charasis.  SBDOOR102
                                        case GameObjectOpenType.TYPE25: // TODO: Only in mischiefplane (KNBOOKC101 and BRISPORT202)
                                        case GameObjectOpenType.TYPE30: // TODO: Only in mischiefplane (POMDOOR206)
                                        case GameObjectOpenType.TYPE35: // TODO: Only in sleeper (SLTDOOR200)
                                        case GameObjectOpenType.TYPE71: // TODO: Only in Sebilis (SBDOOR103)
                                        case GameObjectOpenType.TYPE72: // TODO: Only in CityMist (CMDOOR2) and Sebilis (SBDOOR101)
                                        case GameObjectOpenType.TYPE74: // TODO: Only in Skyshrine (MARBDOOR200)
                                        case GameObjectOpenType.TYPE77: // TODO: Only in sleeper (SHRINEGATE200)
                                        case GameObjectOpenType.TYPE145: // TODO: Only in Timorous (FAYBRAZIER)
                                        default:
                                            {
                                                Logger.WriteError("Unhandled door open type of " + gameObject.OpenType);
                                                continue;
                                            }
                                    }
                                } break;
                            case GameObjectType.Bridge:
                                {
                                    ObjectModelProperties objectProperties = new ObjectModelProperties(ActiveDoodadAnimType.OnActivateRotateDownClosedBackwards, 0, Configuration.OBJECT_GAMEOBJECT_OPENCLOSE_ANIMATIONTIME_INMS, gameObject.HasColission, gameObject.RenderingEnabled);
                                    curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                    curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.OriginalModelName);
                                } break;
                            case GameObjectType.TradeskillFocus:
                                {
                                    ObjectModelProperties objectProperties = new ObjectModelProperties();
                                    objectProperties.DoGenerateCollisionFromMeshData = gameObject.HasColission;
                                    objectProperties.RenderingEnabled = gameObject.RenderingEnabled;
                                    curObjectModel = new ObjectModel(modelFileName, objectProperties, ObjectModelType.StaticDoodad, objectVisibilityBoundingBoxMinSize);
                                    curObjectModel.LoadEQObjectFromFile(modelDataRootFolder, gameObject.ModelName);
                                } break;
                            default:
                                {
                                    Logger.WriteError("When trying to create the object model for a gameobject, this object type is not implemented: " + gameObject.ObjectType);
                                    continue;
                                }
                        }
                        Logger.WriteDebug("- [" + gameObject.OriginalModelName + "]: Importing EQ transport lift trigger object '" + gameObject.OriginalModelName + "' complete");

                        // Attach sounds
                        if (gameObject.OpenSound != null)
                            curObjectModel.SoundsByAnimationType.Add(AnimationType.Open, gameObject.OpenSound);
                        if (gameObject.CloseSound != null)
                            curObjectModel.SoundsByAnimationType.Add(AnimationType.Close, gameObject.CloseSound);

                        // Create the M2 and Skin
                        string relativeMPQPath = Path.Combine("World", "Everquest", "GameObjects", modelFileName);
                        M2 objectM2 = new M2(curObjectModel, relativeMPQPath);
                        string curGameObjectOutputFolder = Path.Combine(gameObjectOutputFolderRoot, modelFileName);
                        objectM2.WriteToDisk(modelFileName, curGameObjectOutputFolder);

                        // Place the related textures
                        string objectTextureFolder = Path.Combine(modelDataRootFolder, "textures");
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
                        gameObject.ObjectModel = curObjectModel;
                        NonDoodadObjectModelsByNameAndOpenType.Add((gameObject.OriginalModelName, gameObject.OpenType), curObjectModel);
                        int gameObjectDisplayInfoID = GameObjectDisplayInfoDBC.GenerateID();
                        gameObject.GameObjectDisplayInfoID = gameObjectDisplayInfoID;
                        GameObjectDisplayInfoIDsByModelNameAndOpenType.Add((gameObject.OriginalModelName, gameObject.OpenType), gameObjectDisplayInfoID);
                    }
                }
            }
        }

        private static void LoadGameObjects()
        {
            if (DataIsLoaded == true)
                return;

            // Load the file first
            string gameObjectsFile = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "WorldData", "GameObjects.csv");
            Logger.WriteDebug(string.Concat("Populating Game Object list via file '", gameObjectsFile, "'"));
            List<Dictionary<string, string>> gameObjectsRows = FileTool.ReadAllRowsFromFileWithHeader(gameObjectsFile, "|");

            // Track chain reaction lookups
            List<GameObject> interactiveGameObjects = new List<GameObject>();
            Dictionary<(string, int), GameObject> interactiveGameObjectsByZoneShortNameAndDoorID = new Dictionary<(string, int), GameObject>();

            // Process the rows
            List<string> validZoneShortNames = ZoneProperties.GetZonePropertyListByShortName().Keys.ToList();
            foreach (Dictionary<string, string> gameObjectsRow in gameObjectsRows)
            {
                // Skip disabled
                if (int.Parse(gameObjectsRow["enabled"]) != 1)
                    continue;

                // Store the model name in the lookup
                string zoneShortName = gameObjectsRow["zone"];
                string modelName = gameObjectsRow["model_name"].ToLower(); // Make lower so it works with the asset conditioner
                bool isSkeletal = gameObjectsRow["model_is_skeletal"] == "0" ? false : true;
                if (isSkeletal == true)
                {
                    if (SourceSkeletalModelNamesByZoneShortName.ContainsKey(zoneShortName) == false)
                        SourceSkeletalModelNamesByZoneShortName.Add(zoneShortName, new List<string>());
                    if (SourceSkeletalModelNamesByZoneShortName[zoneShortName].Contains(modelName) == false)
                        SourceSkeletalModelNamesByZoneShortName[zoneShortName].Add(modelName);
                }
                else
                {
                    if (SourceStaticModelNamesByZoneShortName.ContainsKey(zoneShortName) == false)
                        SourceStaticModelNamesByZoneShortName.Add(zoneShortName, new List<string>());
                    if (SourceStaticModelNamesByZoneShortName[zoneShortName].Contains(modelName) == false)
                        SourceStaticModelNamesByZoneShortName[zoneShortName].Add(modelName);
                }

                // Skip invalid object types
                GameObjectType gameObjectType = GetType(gameObjectsRow["type"]);
                if (gameObjectType != GameObjectType.Door && gameObjectType != GameObjectType.NonInteract && gameObjectType != GameObjectType.TradeskillFocus && gameObjectType != GameObjectType.Bridge)
                    continue;

                // Skip zones not being loaded
                if (validZoneShortNames.Contains(zoneShortName) == false)
                    continue;

                GameObject newGameObject = new GameObject();
                newGameObject.ID = int.Parse(gameObjectsRow["id"]);
                newGameObject.GameObjectTemplateEntryID = int.Parse(gameObjectsRow["gotemplate_id"]);
                newGameObject.DoorID = int.Parse(gameObjectsRow["doorid"]);
                newGameObject.TriggerDoorID = int.Parse(gameObjectsRow["triggerdoor"]);
                newGameObject.ObjectType = gameObjectType;
                newGameObject.OpenType = GetOpenType(int.Parse(gameObjectsRow["opentype"]));
                newGameObject.ZoneShortName = gameObjectsRow["zone"];
                newGameObject.OriginalModelName = modelName;
                newGameObject.ModelName = modelName;
                newGameObject.ModelIsSkeletal = isSkeletal;
                newGameObject.HasColission = int.Parse(gameObjectsRow["has_collision"]) == 1 ? true : false;
                newGameObject.RenderingEnabled = int.Parse(gameObjectsRow["render_enabled"]) == 1 ? true : false;
                newGameObject.DisplayName = gameObjectsRow["display_name"];
                newGameObject.CloseTimeInMS = int.Parse(gameObjectsRow["close_time"]) * 1000;
                float xPosition = float.Parse(gameObjectsRow["pos_x"]);
                float yPosition = float.Parse(gameObjectsRow["pos_y"]);
                float zPosition = float.Parse(gameObjectsRow["pos_z"]);
                if (gameObjectType != GameObjectType.NonInteract)
                {
                    xPosition *= Configuration.GENERATE_WORLD_SCALE;
                    yPosition *= Configuration.GENERATE_WORLD_SCALE;
                    zPosition *= Configuration.GENERATE_WORLD_SCALE;
                    newGameObject.Position = new Vector3(xPosition, yPosition, zPosition);
                }
                else
                {
                    // Non-interact should be in EQ coordinate properties since they are loaded as doodads
                    // Also make sure to flip Z and Y for non-interact since doodads get changed later
                    newGameObject.Position = new Vector3(xPosition, zPosition, yPosition);
                }
                newGameObject.Scale = float.Parse(gameObjectsRow["size"]) / 100f;
                newGameObject.GameObjectGUID = GameObjectSQL.GenerateGUID();
                newGameObject.ModelIsInEquipmentFolder = gameObjectsRow["model_in_equipment"].Trim() == "1" ? true : false;
                string tradeskillFocusTypeString = gameObjectsRow["tradeskill_focus"].Trim().ToLower();
                switch(tradeskillFocusTypeString)
                {
                    case "cookingfire": newGameObject.TradeskillFocusType = GameObjectTradeskillFocusType.CookingFire; break;
                    case "forge": newGameObject.TradeskillFocusType = GameObjectTradeskillFocusType.Forge; break;
                    default: break; // Nothing
                }

                // "Heading" in EQ was 0-512 instead of 0-360, and the result needs to rotate 180 degrees due to y axis difference
                newGameObject.EQHeading = float.Parse(gameObjectsRow["heading"]);
                if (newGameObject.EQHeading == 0)
                    newGameObject.Orientation = MathF.PI;
                if (newGameObject.EQHeading != 0)
                {
                    float orientationInDegrees = (newGameObject.EQHeading / 512) * 360;
                    float orientationInRadians = orientationInDegrees * MathF.PI / 180.0f;
                    newGameObject.Orientation = orientationInRadians + MathF.PI;
                }
                newGameObject.EQIncline = float.Parse(gameObjectsRow["incline"]);

                // Different logic based on type
                if (gameObjectType == GameObjectType.NonInteract)
                {
                    // Add it as a doodad item
                    newGameObject.LoadAsZoneDoodad = true;
                    if (DoodadGameObjectsByZoneShortname.ContainsKey(newGameObject.ZoneShortName) == false)
                        DoodadGameObjectsByZoneShortname.Add(newGameObject.ZoneShortName, new List<GameObject>());
                    DoodadGameObjectsByZoneShortname[newGameObject.ZoneShortName].Add(newGameObject);
                }
                else
                {
                    if (gameObjectType == GameObjectType.Door || gameObjectType == GameObjectType.Bridge)
                    {
                        // Save this up in the trigger chain lookup
                        interactiveGameObjectsByZoneShortNameAndDoorID.Add((newGameObject.ZoneShortName, newGameObject.DoorID), newGameObject);

                        GetSoundsForOpenType(newGameObject.OpenType, out newGameObject.OpenSound, out newGameObject.CloseSound);
                        if (newGameObject.OpenSound != null)
                            if (OpenSoundsByModelNameAndOpenType.ContainsKey((newGameObject.OriginalModelName, newGameObject.OpenType)) == false)
                                OpenSoundsByModelNameAndOpenType.Add((newGameObject.OriginalModelName, newGameObject.OpenType), newGameObject.OpenSound);
                        if (newGameObject.CloseSound != null)
                            if (CloseSoundsByModelNameAndOpenType.ContainsKey((newGameObject.OriginalModelName, newGameObject.OpenType)) == false)
                                CloseSoundsByModelNameAndOpenType.Add((newGameObject.OriginalModelName, newGameObject.OpenType), newGameObject.CloseSound);
                    }

                    // Add it
                    if (NonDoodadGameObjectsByZoneShortname.ContainsKey(newGameObject.ZoneShortName) == false)
                        NonDoodadGameObjectsByZoneShortname.Add(newGameObject.ZoneShortName, new List<GameObject>());
                    NonDoodadGameObjectsByZoneShortname[newGameObject.ZoneShortName].Add(newGameObject);
                    interactiveGameObjects.Add(newGameObject);
                }
            }

            // Store the chain reactions from the lookup
            foreach (GameObject gameObject in interactiveGameObjects)
            {
                if (gameObject.TriggerDoorID == 0)
                    continue;
                if (interactiveGameObjectsByZoneShortNameAndDoorID.ContainsKey((gameObject.ZoneShortName, gameObject.TriggerDoorID)))
                {
                    gameObject.TriggerGameObjectGUID = interactiveGameObjectsByZoneShortNameAndDoorID[(gameObject.ZoneShortName, gameObject.TriggerDoorID)].GameObjectGUID;
                    gameObject.TriggerGameObjectTemplateEntryID = interactiveGameObjectsByZoneShortNameAndDoorID[(gameObject.ZoneShortName, gameObject.TriggerDoorID)].GameObjectTemplateEntryID;
                }   
            }
            DataIsLoaded = true;
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
                case "tradeskillfocus": return GameObjectType.TradeskillFocus;
                default:
                    {
                        Logger.WriteError("Can't determine GameObjectType due to an unmapped open type name value of " + typeNameValue);
                        return GameObjectType.Unknown;
                    }

            }
        }

        // The sound data references here was shared from "kicnlag" from the Project Latern project, 3/13/2025
        private static void GetSoundsForOpenType(GameObjectOpenType openType, out Sound openSound, out Sound closeSound)
        {
            switch (openType)
            {
                case GameObjectOpenType.TYPE0: // STANDARD_WOOD
                case GameObjectOpenType.TYPE5: // STANDARD_WOOD_CLOCKWISE
                    {
                        openSound = GetSound("doorwd_o.wav");
                        closeSound = GetSound("doorwd_c.wav");
                    } break;
                case GameObjectOpenType.TYPE1: // STANDARD_METAL
                case GameObjectOpenType.TYPE6: // STANDARD_METAL_CLOCKWISE
                    {
                        openSound = GetSound("doormt_o.wav");
                        closeSound = GetSound("doormt_c.wav");
                    } break;
                case GameObjectOpenType.TYPE2: // STANDARD_STONE
                case GameObjectOpenType.TYPE7: // STANDARD_STONE_CLOCKWISE
                case GameObjectOpenType.TYPE74: // Marble door in Skyshrine
                case GameObjectOpenType.TYPE140: // BLOCK_ON_CHAIN
                case GameObjectOpenType.TYPE145:
                    {
                        openSound = GetSound("doorst_o.wav");
                        closeSound = GetSound("doorst_c.wav");
                    } break;
                case GameObjectOpenType.TYPE10: // SMALL_SLIDING
                case GameObjectOpenType.TYPE12: // SMALL_SLIDING_STONE
                case GameObjectOpenType.TYPE15: // MEDIUM_SLIDING
                case GameObjectOpenType.TYPE16: // MEDIUM_SLIDING_METAL
                case GameObjectOpenType.TYPE17: // MEDIUM_SLIDING_STONE
                case GameObjectOpenType.TYPE21: // LARGE_SLIDING_METAL
                case GameObjectOpenType.TYPE22: // LARGE_SLIDING_STONE
                case GameObjectOpenType.TYPE25:
                case GameObjectOpenType.TYPE26: // GIANT_SLIDING_METAL
                    {
                        openSound = GetSound("sldorsto.wav");
                        closeSound = GetSound("sldorstc.wav");
                    } break;
                case GameObjectOpenType.TYPE30: // DRAW_BRIDGE
                    {
                        openSound = GetSound("dbrdg_lp.wav");
                        closeSound = GetSound("dbrdgstp.wav");
                    } break;
                case GameObjectOpenType.TYPE35:
                    {
                        openSound = GetSound("trapdoor.wav");
                        closeSound = GetSound("trapdoor.wav");
                    } break;
                case GameObjectOpenType.TYPE40: // LEVER
                case GameObjectOpenType.TYPE45: // TOGGLE
                    {
                        openSound = GetSound("lever.wav");
                        closeSound = GetSound("lever.wav");
                    } break;
                case GameObjectOpenType.TYPE59: // ELEVATOR
                    {
                        openSound = GetSound("elevloop.wav");
                        closeSound = GetSound("elevloop.wav");
                    } break;
                case GameObjectOpenType.TYPE60: // SMALL_SLIDING_UPWARDS
                case GameObjectOpenType.TYPE61: // SMALL_SLIDING_UPWARDS_METAL
                case GameObjectOpenType.TYPE65: // MEDIUM_SLIDING_UPWARDS
                case GameObjectOpenType.TYPE66: // MEDIUM_SLIDING_UPWARDS_METAL
                case GameObjectOpenType.TYPE70: // LARGE_SLIDING_UPWARDS
                case GameObjectOpenType.TYPE71: // LARGE_SLIDING_UPWARDS_METAL
                case GameObjectOpenType.TYPE72: // LARGE_SLIDING_UPWARDS_STONE
                case GameObjectOpenType.TYPE75: // GIANT_SLIDING_UPWARDS
                case GameObjectOpenType.TYPE76: // GIANT_SLIDING_UPWARDS_METAL
                case GameObjectOpenType.TYPE77:
                    {
                        openSound = GetSound("portc_lp.wav");
                        closeSound = GetSound("portcstp.wav");
                    } break;                
                case GameObjectOpenType.TYPE120: // SPEAR_DOWN
                    {
                        openSound = GetSound("speardn.wav");
                        closeSound = GetSound("speardn.wav");
                    } break;
                case GameObjectOpenType.TYPE125: // SPEAR_UP
                    {
                        openSound = GetSound("spearup.wav");
                        closeSound = GetSound("spearup.wav");
                    } break;
                case GameObjectOpenType.TYPE130: // PENDULUM
                    {
                        openSound = GetSound("null1.wav");
                        closeSound = GetSound("null1.wav");
                    } break;
                default:
                    {
                        openSound = GetSound("null1.wav");
                        closeSound = GetSound("null1.wav");
                        Logger.WriteError("Unhandled GameObject open type of " + openType);
                    } break;
            }
        }

        private static Sound GetSound(string soundName)
        {
            if (AllSoundsBySoundName.ContainsKey(soundName.Trim()) == true)
                return AllSoundsBySoundName[soundName.Trim()];
            else
            {
                string name = "EQ GameObject " + Path.GetFileNameWithoutExtension(soundName);
                Sound returnSound = new Sound(name, Path.GetFileNameWithoutExtension(soundName), SoundType.GameObject, 8, 20, false);
                AllSoundsBySoundName.Add(soundName.Trim(), returnSound);
                return returnSound;
            }
        }

        public string GenerateModelFileNameNoExt()
        {
            return string.Concat("go_", ModelName, "_", OpenType.ToString()).ToLower();
        }
    }
}
