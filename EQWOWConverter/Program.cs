//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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

using EQWOWConverter;
using MySql.Data.MySqlClient;

internal class Program
{
    public static bool LoadAndVerifyConfig()
    {
        Configuration.LoadConfiguration();
        Configuration.SaveConfiguration();

        // Test WOW install directory
        string wowExecutableFileName = Path.Combine(Configuration.PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER, "Wow.exe");
        if (File.Exists(wowExecutableFileName) == false)
        {
            Logger.WriteError("Could not locate the World of Warcraft executable in the path defined by configuration variable PATH_WORLDOFWARCRAFT_CLIENT_INSTALL_FOLDER. ",
                "Edit the config and ensure this path resolves to the World of Warcraft install directory which has the Wow.exe in it.");
            return false;
        }

        // Test EverQuest Trilogy install directory
        string eqExecutableFileName = Path.Combine(Configuration.PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER, "eqgame.exe");
        if (File.Exists(eqExecutableFileName) == false)
        {
            Logger.WriteError("Could not locate the EverQuest executable in the path defined by configuration variable PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER. ",
                "Edit the config and ensure this path resolves to the EverQuest Trilogy install directory which has the eqgame.exe in it.");
            return false;
        }
        string eqThurgadinFileName = Path.Combine(Configuration.PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER, "thurgadina.s3d");
        if (File.Exists(eqThurgadinFileName) == false)
        {
            Logger.WriteError("Could not locate the a specific Velious file inside the EverQuest folder defined by configuration variable PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER. ",
                "While this may be an EverQuest folder, it is likely a version before EverQuest Trilogy. ",
                "Edit the config and ensure this path resolves to an unpatched EverQuest Trilogy install directory.");
            return false;
        }
        string eqShadowhavenFileName = Path.Combine(Configuration.PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER, "shadowhaven.s3d");
        if (File.Exists(eqShadowhavenFileName) == true)
        {
            Logger.WriteError("Found a post-Velious file inside the EverQuest folder defined by configuration variable PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER. ",
                "While this may be an EverQuest folder, it is likely a version after EverQuest Trilogy. ",
                "Edit the config and ensure this path resolves to an unpatched EverQuest Trilogy install directory. ",
                "Note that EverQuest Titanium is NOT compatible with this generator.");
            return false;
        }

        // Test Tools directory
        string toolsTestSubFolder = Path.Combine(Configuration.PATH_TOOLS_FOLDER, "blpconverter");
        if (Directory.Exists(toolsTestSubFolder) == false)
        {
            Logger.WriteError("Could not locate the tools folder PATH_TOOLS_FOLDER. Edit the config and ensure this path resolves to ",
                "the folder that contains the tool folders, such as the 'blpconverter' folder or the 'ffmpeg' folder. ",
                "By default, this is folder named 'Tools' that should have been located in the directory with the EQWOWConverter.exe");
            return false;
        }

        // Test Assets directory
        string assetsTestSubFolder = Path.Combine(Configuration.PATH_ASSETS_FOLDER, "AddOns");
        if (Directory.Exists(assetsTestSubFolder) == false)
        {
            Logger.WriteError("Could not locate the tools folder PATH_ASSETS_FOLDER. Edit the config and ensure this path resolves to ",
                "the folder that contains the asset folders, such as the 'AddOns' folder or the 'WorldData' folder. ",
                "By default, this is folder named 'Assets' that should have been located in the directory with the EQWOWConverter.exe");
            return false;
        }

        // Test DBC folder
        if (Configuration.DEPLOY_SERVER_FILES == true)
        {
            string serverDBCFolder = Configuration.DEPLOY_SERVER_DBC_FOLDER_LOCATION;
            if (Directory.Exists(serverDBCFolder) == false)
            {
                Logger.WriteError("Could not locate server's DBC folder defined by DEPLOY_SERVER_DBC_FOLDER_LOCATION, and deploying ",
                    "to server was set to true as defined in DEPLOY_SERVER_FILES. Edit the config and ensure this path resolves to ",
                    "the folder that is the DBC Folder of the server, or disable DEPLOY_SERVER_FILES");
                return false;
            }
            string spellDBCFile = Path.Combine(Configuration.DEPLOY_SERVER_DBC_FOLDER_LOCATION, "Spell.dbc");
            if (File.Exists(spellDBCFile) == false)
            {
                Logger.WriteError("Could not locate a specific .dbc file in the server's DBC folder defined by DEPLOY_SERVER_DBC_FOLDER_LOCATION, and deploying ",
                    "to server was set to true as defined in DEPLOY_SERVER_FILES. Edit the config and ensure this path resolves to ",
                    "the folder that is the DBC Folder of the server (typically '/data/dbc' in AzerothCore build folder), or disable DEPLOY_SERVER_FILES");
                return false;
            }
        }

        // Test connections
        if (Configuration.DEPLOY_SERVER_SQL == true)
        {
            // Character
            try
            {
                MySqlConnection worldConnection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_CHARACTERS);
                worldConnection.Open();
                worldConnection.Close();
            }
            catch (MySqlException e)
            {
                Logger.WriteError("Failed to connect to the character server as defined by the configuration connection string DEPLOY_SQL_CONNECTION_STRING_CHARACTERS, ",
                    "and DEPLOY_SERVER_SQL is true.  Here is the message from the exception: ", e.Message);
                return false;
            }
            catch (ArgumentException e)
            {
                Logger.WriteError("Failed to connect to the character server as defined by the configuration connection string DEPLOY_SQL_CONNECTION_STRING_CHARACTERS, ",
                    "and DEPLOY_SERVER_SQL is true.  It's likely the format of that connection string is improper.  Here is the message from the exception: ",
                    e.Message);
                return false;
            }

            // World
            try
            {
                MySqlConnection worldConnection = new MySqlConnection(Configuration.DEPLOY_SQL_CONNECTION_STRING_WORLD);
                worldConnection.Open();
                worldConnection.Close();
            }
            catch (MySqlException e)
            {
                Logger.WriteError("Failed to connect to the world server as defined by the configuration connection string DEPLOY_SQL_CONNECTION_STRING_WORLD, ",
                    "and DEPLOY_SERVER_SQL is true.  Here is the message from the exception: ", e.Message);
                return false;
            }
            catch (ArgumentException e)
            {
                Logger.WriteError("Failed to connect to the world server as defined by the configuration connection string DEPLOY_SQL_CONNECTION_STRING_WORLD, ",
                    "and DEPLOY_SERVER_SQL is true.  It's likely the format of that connection string is improper.  Here is the message from the exception: ",
                    e.Message);
                return false;
            }
        }

        return true;
    }

    private static void Main(string[] args)
    {
        Console.Title = "EverQuest to WoW Converter";
        Logger.ResetLog();

        bool configLoadResult = LoadAndVerifyConfig();
        if (configLoadResult == false)
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            return;
        }

        Logger.WriteInfo("###### EQ WOW Converter ######");
        Logger.WriteInfo("");
        Logger.WriteInfo("Options:");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [1] [All] - Perform Everything (Do this if not sure)");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [2] [Conditioner] - Extract EQ Data ONLY");
        Logger.WriteInfo(" [3] [Conditioner] - Condition Extracted EQ Data ONLY");
        //Logger.WriteInfo(" [4] [Conditioner] - Convert PNG files into BLP ONLY");
        //Logger.WriteInfo(" [5] [Conditioner] - Split raw world map images up and change to BLP");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [6] [Converter] - Convert EQ Data to WOW ONLY");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [X] - Exit");
        Logger.WriteInfo(" ");
        Logger.WriteInfo("Command (Default: X): ", false);
        string? enteredCommand = Console.ReadLine();
        if (enteredCommand == null || enteredCommand.Length == 0 || enteredCommand[0] == ' ' || enteredCommand.ToUpper()[0] == 'X')
        {
            if (Configuration.CORE_CONSOLE_BEEP_ON_COMPLETE)
                Console.Beep();
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            return;
        }
        if (enteredCommand == "6" || enteredCommand == "1")
        {
            //if (Configuration.GENERATE_CREATURES_AND_SPAWNS == true && Configuration.CREATURE_ADD_DEBUG_VALUES_TO_NAME == true)
            //    Logger.WriteInfo("- CREATURE_ADD_DEBUG_VALUES_TO_NAME is true, so creature names will be in debug mode");
            Logger.WriteInfo("- GENERATE_REBALANCE_CONTENT_TO_LEVEL_80 is set to ", Configuration.GENERATE_REBALANCE_CONTENT_TO_LEVEL_80.ToString());
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_GENERAL is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL.ToString());
            if (Configuration.CONFIGONLY_GENERATE_DELTA_ONLY_MAIN_PATCH == true)
                Logger.WriteInfo("- CONFIGONLY_GENERATE_DELTA_ONLY_MAIN_PATCH is true, so a delta-only patch can generate");
            if (Configuration.GENERATE_PLAYER_ARMOR_GRAPHICS == false)
                Logger.WriteInfo("- GENERATE_PLAYER_ARMOR_GRAPHICS is false, so no player armor will be generated");
            if (Configuration.GENERATE_OBJECTS == false)
                Logger.WriteInfo("- GENERATE_OBJECTS is set to false");
            Logger.WriteInfo("- GENERATE_RIDING_TRAINERS_ENABLED is set to ", Configuration.CREATURE_RIDING_TRAINERS_ENABLED.ToString());
            if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true)
                Logger.WriteInfo(" - CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY is set to ", Configuration.CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY.ToString());
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true)
                Logger.WriteInfo("- PLAYER_USE_EQ_START_LOCATION is true, so player start locations will be changed");
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
                Logger.WriteInfo("- PLAYER_USE_EQ_START_ITEMS is true, so player start items will be changed");
            if (Configuration.PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES == true)
            {
                Logger.WriteInfo("- PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES is true, so DKs will start at level 1");
                if (Configuration.PLAYER_USE_EQ_START_LOCATION == false)
                    Logger.WriteWarning("- Having PLAYER_DEATHKNIGHT_START_LIKE_OTHER_CLASSES = true and PLAYER_USE_EQ_START_LOCATION = false will cause DKs to get 'stuck' at creation");
            }
            Logger.WriteInfo("- DUNGEON_FINDER_ENABLED is set to ", Configuration.DUNGEON_FINDER_ENABLED.ToString());
            Logger.WriteInfo("- GENERATE_NON_PLAYER_OBTAINABLE_ITEMS is set to ", Configuration.GENERATE_NON_PLAYER_OBTAINABLE_ITEMS.ToString());
            Logger.WriteInfo("- PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED is set to ", Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED.ToString());
            Logger.WriteInfo("- SPELLS_LEARNABLE_FROM_ITEMS_ENABLED is set to ", Configuration.SPELLS_LEARNABLE_FROM_ITEMS_ENABLED.ToString());
        }
        Logger.WriteInfo("Are you sure Y/N? (Default: Y): ", false);
        string? enteredConfirm = Console.ReadLine();
        if (enteredConfirm != null && (enteredConfirm.Length == 0 || enteredConfirm.Trim().ToUpper()[0] == 'Y'))
        {
            Logger.WriteInfo(" ");
            if (enteredCommand == null)
                Logger.WriteInfo("Unknown command, exiting...");
            else
            {
                try
                {
                    switch (enteredCommand.ToUpper())
                    {
                        case "X":
                            {
                                Logger.WriteInfo("Exiting.");
                            }
                            break;
                        case "1":
                            {
                                Logger.WriteInfo("Performing all steps to convert EQ to WoW...");

                                // Extraction
                                Logger.WriteInfo("Extracting EQ files...");
                                if (LanternExtractor.LanternExtractor.ProcessRequest(Configuration.PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER, Configuration.PATH_EQEXPORTSRAW_FOLDER) == true)
                                    Logger.WriteInfo("Extraction completed successfully.");
                                else
                                {
                                    Logger.WriteError("Extraction failed!");
                                    break;
                                }

                                // Condition
                                AssetConditioner conditioner = new AssetConditioner();
                                bool condenseResult = conditioner.ConditionEQOutput();
                                if (condenseResult == false)
                                {
                                    Logger.WriteInfo("Extracted EQ Data Conditioning Failed.");
                                    break;
                                }

                                // Convert PNG to BLP
                                conditioner.ConvertPNGFilesToBLP();

                                // Convert to WoW
                                AssetConverter converter = new AssetConverter();
                                bool conversionResult = converter.ConvertEQDataToWOW();
                                if (conversionResult == false)
                                {
                                    Logger.WriteInfo("EQ to WoW conversion Failed.");
                                    break;
                                }
                                else
                                    Logger.WriteInfo("All steps completed, and the EQ data is converted to WoW");
                            }
                            break;
                        case "2":
                            {
                                Logger.WriteInfo("Extracting EQ files...");
                                if (LanternExtractor.LanternExtractor.ProcessRequest(Configuration.PATH_EVERQUEST_TRILOGY_CLIENT_INSTALL_FOLDER, Configuration.PATH_EQEXPORTSRAW_FOLDER) == true)
                                    Logger.WriteInfo("Extraction completed successfully.");
                                else
                                    Logger.WriteError("Extraction failed!");
                            }
                            break;
                        case "3":
                            {
                                AssetConditioner conditioner = new AssetConditioner();
                                bool condenseResult = conditioner.ConditionEQOutput();
                                if (condenseResult == false)
                                    Logger.WriteInfo("Extracted EQ Data Conditioning Failed (base conditioning)");
                                else
                                {
                                    condenseResult = conditioner.ConvertPNGFilesToBLP();
                                    if (condenseResult == false)
                                        Logger.WriteInfo("Extracted EQ Data Conditioning Failed (blp conversions)");
                                    else
                                    {
                                        condenseResult = conditioner.ConditionWorldMapFilesOnly();
                                        if (condenseResult == false)
                                        {
                                            Logger.WriteInfo("Extracted EQ Data Conditioning Failed (maps)");
                                            return;
                                        }
                                        Logger.WriteInfo("Extracted EQ Data Conditioning Succeeded.");
                                    }                                    
                                }
                            } break;
                        //case "4":
                        //    {
                        //        AssetConditioner conditioner = new AssetConditioner();
                        //        conditioner.ConvertPNGFilesToBLP();
                        //    }
                        //    break;
                        //case "5":
                        //{
                        //    {
                        //        AssetConditioner conditioner = new AssetConditioner();
                        //        conditioner.ConditionWorldMapFilesOnly();
                        //    }
                        //} break;
                        case "6":
                            {
                                AssetConverter converter = new AssetConverter();
                                bool conversionResult = converter.ConvertEQDataToWOW();
                                if (conversionResult == false)
                                    Logger.WriteInfo("EQ to WoW conversion Failed.");
                            }
                            break;
                        case "9":
                            {
                                //string outputMusicFolderRoot = Path.Combine(Configuration.PATH_EQEXPORTSCONDITIONED_FOLDER, "music");
                                AssetConditioner conditioner = new AssetConditioner();
                                //conditioner.ConditionMusicFiles(outputMusicFolderRoot);
                                conditioner.GenerateSpriteSheets();
                            } break;
                        default: break;
                    }
                    Logger.WriteInfo("Exiting....");
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Exception Occurred: " + ex.Message);
                    if (ex.StackTrace != null)
                        Logger.WriteDebug(ex.StackTrace);
                }
            }
        }

        if (Configuration.CORE_CONSOLE_BEEP_ON_COMPLETE)
            Console.Beep();
        Console.WriteLine("");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}