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

using EQWOWConverter;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "EverQuest to WoW Converter";
        Logger.ResetLog();
        Logger.WriteInfo("###### EQ WOW Converter ######");
        Logger.WriteInfo("");
        Logger.WriteInfo("Options:");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [1] [All] - Perform Everything (Do this if not sure)");
        Logger.WriteInfo(" ");
        Logger.WriteInfo(" [2] [Conditioner] - Extract EQ Data ONLY");
        Logger.WriteInfo(" [3] [Conditioner] - Condition Extracted EQ Data ONLY");
        Logger.WriteInfo(" [4] [Conditioner] - Convert PNG files into BLP ONLY");
        Logger.WriteInfo(" [5] [Conditioner] - Split raw world map images up and change to BLP");
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
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_GENERAL is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_GENERAL.ToString());
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_ZONES is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_ZONES.ToString());
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_TRANSPORTS is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_TRANSPORTS.ToString());
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_TRADESKILLS is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_TRADESKILLS.ToString());
            Logger.WriteInfo("- GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS is set to ", Configuration.GENERATE_EQ_EXPANSION_ID_EQUIPMENT_GRAPHICS.ToString());
            if (Configuration.GENERATE_PLAYER_ARMOR_GRAPHICS == false)
                Logger.WriteInfo("- GENERATE_PLAYER_ARMOR_GRAPHICS is false, so no player armor will be generated");
            if (Configuration.GENERATE_OBJECTS == false)
                Logger.WriteInfo("- GENERATE_OBJECTS is set to false");
            Logger.WriteInfo(string.Concat("- GENERATE_RIDING_TRAINERS_ENABLED is set to ", Configuration.CREATURE_RIDING_TRAINERS_ENABLED));
            if (Configuration.CREATURE_RIDING_TRAINERS_ENABLED == true)
                Logger.WriteInfo(string.Concat(" - CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY is set to ", Configuration.CREATURE_RIDING_TRAINERS_ALSO_TEACH_FLY));
            if (Configuration.PLAYER_USE_EQ_START_LOCATION == true)
                Logger.WriteInfo("- PLAYER_USE_EQ_START_LOCATION is true, so player start locations will be changed");
            if (Configuration.PLAYER_USE_EQ_START_ITEMS == true)
                Logger.WriteInfo("- PLAYER_USE_EQ_START_ITEMS is true, so player start items will be changed");
            Logger.WriteInfo(string.Concat("- PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED is set to ", Configuration.PLAYER_REDUCE_MODEL_COLLISION_HEIGHT_ENABLED));
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
                //try
                //{
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
                                    Logger.WriteInfo("Extracted EQ Data Conditioning Failed.");
                                else
                                    Logger.WriteInfo("Extracted EQ Data Conditioning Succeeded.");
                            }
                            break;
                        case "4":
                            {
                                AssetConditioner conditioner = new AssetConditioner();
                                conditioner.ConvertPNGFilesToBLP();
                            }
                            break;
                        case "5":
                        {
                            {
                                AssetConditioner conditioner = new AssetConditioner();
                                conditioner.ConditionWorldMapFilesOnly();
                            }
                        } break;
                        case "6":
                            {
                                AssetConverter converter = new AssetConverter();
                                bool conversionResult = converter.ConvertEQDataToWOW();
                                if (conversionResult == false)
                                    Logger.WriteInfo("EQ to WoW conversion Failed.");
                            }
                            break;
                        default: break;
                    }
                    Logger.WriteInfo("Exiting....");
                //}
                //catch (Exception ex)
                //{
                //    Logger.WriteError("Exception Occurred: " + ex.Message);
                //    if (ex.StackTrace != null)
                //        Logger.WriteDebug(ex.StackTrace);
                //}
            }
        }

        if (Configuration.CORE_CONSOLE_BEEP_ON_COMPLETE)
            Console.Beep();
        Console.WriteLine("");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}