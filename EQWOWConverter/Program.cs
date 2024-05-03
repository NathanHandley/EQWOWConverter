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
using System.Runtime.CompilerServices;

internal class Program
{
    // TODO: Move to config
    public static string CONFIG_PATH_EQEXPORTSRAW           = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExports-Int";
    public static string CONFIG_PATH_EQEXPORTSCONDITIONED   = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExportsConditioned";
    public static string CONFIG_PATH_EXPORT_FOLDER          = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\WOWExports";

    private static void Main(string[] args)
    {
        Logger.ResetLog();
        Logger.WriteLine("###### EQ WOW Converter ######");
        bool doLoopForCommands = true;
        while (doLoopForCommands == true)
        {   
            Logger.WriteLine("");
            Logger.WriteLine("Options:");
            Logger.WriteLine(" [1] - Condition Exported EQ Model Data");
            Logger.WriteLine(" [2] - Update image references - NOTE: Do after stop 1 AND converting .png -> .blp");
            Logger.WriteLine(" [3] - Generate EQ Zone Index and Face Index Association Maps (Takes a long time)");
            Logger.WriteLine(" [5] - Convert Zones to WMO");
            Logger.WriteLine(" [X] - Exit");
            Console.Write("Command: ");
            string? enteredCommand = Console.ReadLine();
            if (enteredCommand == null)
            {
                Logger.WriteLine("Enter a command");
            }
            else
            {
                switch (enteredCommand.ToUpper())
                {
                    case "X":
                        {
                            Logger.WriteLine("Exiting.");
                            doLoopForCommands = false;
                        }
                        break;
                    case "1":
                        {
                            Logger.WriteLine("Conditioning Exported EQ Data...");
                            AssetConditioner conditioner = new AssetConditioner();
                            bool condenseResult = conditioner.ConditionAllModels(CONFIG_PATH_EQEXPORTSRAW, CONFIG_PATH_EQEXPORTSCONDITIONED);
                            if (condenseResult == false)
                            {
                                Logger.WriteLine("Exported EQ Data Conditioning Failed.");
                                break;
                            }
                            Logger.WriteLine("Exported EQ Data Conditioning Succeeded.");
                        } break;
                    case "2":
                        {
                            Logger.WriteLine("Updating image references...");
                            AssetConditioner conditioner = new AssetConditioner();
                            bool condenseResult = conditioner.UpdateImageReferences(CONFIG_PATH_EQEXPORTSCONDITIONED);
                            if (condenseResult == false)
                            {
                                Logger.WriteLine("Updating image references failed.");
                                break;
                            }
                            Logger.WriteLine("Image reference updates complete.");
                        } break;
                    case "3":
                        {
                            Logger.WriteLine("Generating Association Maps (takes a very long time)...");
                            AssetConditioner conditioner = new AssetConditioner();
                            bool condenseResult = conditioner.GenerateAssociationMaps(CONFIG_PATH_EQEXPORTSCONDITIONED);
                            if (condenseResult == false)
                            {
                                Logger.WriteLine("Association Map Generation Failure.");
                                break;
                            }
                            Logger.WriteLine("Assocation Maps Generated.");
                        } break;
                    case "5":
                        {
                            Logger.WriteLine("Converting zones from EQ to WoW...");
                            AssetConverter converter = new AssetConverter();
                            bool conversionResult = AssetConverter.ConvertEQZonesToWOW(CONFIG_PATH_EQEXPORTSCONDITIONED, CONFIG_PATH_EXPORT_FOLDER);
                            if (conversionResult == false)
                            {
                                Logger.WriteLine("EQ to WoW zone conversion Failed.");
                                break;
                            }
                            Logger.WriteLine("Conversion of zones complete");
                        } break;
                    default:
                        {
                            Logger.WriteLine("Unknown Command");
                        }
                        break;
                }
            }
        }
        Logger.WriteLine("Press any key to quit...");
        Console.ReadKey();
    }
}