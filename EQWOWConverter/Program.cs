// Author: Nathan Handley (https://github.com/NathanHandley), 2024
// Released under GPLv3, unless otherwise noted in specific code files

using EQWOWConverter;
using System.Runtime.CompilerServices;

internal class Program
{
    // TODO: Move to config
    public static string CONFIG_PATH_EQEXPORTSRAW       = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExports-Int";
    public static string CONFIG_PATH_EQEXPORTSCONDITIONED = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExportsConditioned";

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
                    case "5":
                        {
                            Logger.WriteLine("Converting zones from EQ to WoW...");
                            AssetConverter converter = new AssetConverter();
                            bool conversionResult = AssetConverter.ConvertEQZonesToWOW(CONFIG_PATH_EQEXPORTSCONDITIONED);
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