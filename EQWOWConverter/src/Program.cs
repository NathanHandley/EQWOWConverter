// Author: Nathan Handley (https://github.com/NathanHandley), 2024
// Released under GPLv3, unless otherwise noted in specific code files

using EQWOWConverter;

internal class Program
{
    // TODO: Move to config
    public static string CONFIG_PATH_EQEXPORTSRAW       = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExports";
    public static string CONFIG_PATH_EQEXPORTSCONDITIONED = "E:\\Development\\EQWOW-Reference\\Working\\Assets\\EQExportsConditioned";

    private static void Main(string[] args)
    {
        Console.WriteLine("###### EQ WOW Converter ######");
        bool doLoopForCommands = true;
        while (doLoopForCommands == true)
        {
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine(" [1] - Condition Exported EQ Data");
            Console.WriteLine(" [X] - Exit");
            Console.Write("Command: ");
            string? enteredCommand = Console.ReadLine();
            if (enteredCommand == null)
            {
                Console.WriteLine("Enter a command");
            }
            else
            {
                switch (enteredCommand.ToUpper())
                {
                    case "X":
                        {
                            Console.WriteLine("Exiting.");
                            doLoopForCommands = false;
                        }
                        break;
                    case "1":
                        {
                            Console.WriteLine("Conditioning Exported EQ Data...");
                            EQAssetConditioner conditioner = new EQAssetConditioner();
                            bool condenseResult = conditioner.CondenseAll(CONFIG_PATH_EQEXPORTSRAW, CONFIG_PATH_EQEXPORTSCONDITIONED);
                            if (condenseResult == false)
                            {
                                Console.WriteLine("Exported EQ Data Conditioning Failed.");
                                break;
                            }                            
                            Console.WriteLine("Exported EQ Data Conditioning Succeeded.");
                        } break;
                    default:
                        {
                            Console.WriteLine("Unknown Command");
                        }
                        break;
                }
            }
        }
        Console.WriteLine("Press any key to quit...");
        Console.ReadKey();
    }
}