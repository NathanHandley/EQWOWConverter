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
    private static void Main(string[] args)
    {
        Console.Title = "EverQuest to WoW Converter";
        Logger.ResetLog();
        Logger.WriteInfo("###### EQ WOW Converter ######");
        bool doLoopForCommands = true;
        while (doLoopForCommands == true)
        {   
            Logger.WriteInfo("");
            Logger.WriteInfo("Options:");
            Logger.WriteInfo(" [1] - Condition Exported EQ Data");
            Logger.WriteInfo(" [5] - Convert EQ Data to WOW");
            Logger.WriteInfo(" [X] - Exit");
            Logger.WriteInfo(" ");
            Logger.WriteInfo(" [9] - Condition only the Music Data");
            Logger.WriteInfo(" ");
            Logger.WriteInfo("Command (Default: X): ", true);
            string? enteredCommand = Console.ReadLine();
            if (enteredCommand == null)
            {
                Logger.WriteInfo("Exiting.");
                doLoopForCommands = false;
            }
            else
            {
                try
                {
                    switch (enteredCommand.ToUpper())
                    {
                        case "X":
                            {
                                Logger.WriteInfo("Exiting.");
                                doLoopForCommands = false;
                            }
                            break;
                        case "1":
                            {
                                Logger.WriteInfo("Conditioning Exported EQ Data...");
                                AssetConditioner conditioner = new AssetConditioner();
                                bool condenseResult = conditioner.ConditionEQOutput(Configuration.CONFIG_PATH_EQEXPORTSRAW_FOLDER, Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER);
                                if (condenseResult == false)
                                {
                                    Logger.WriteInfo("Exported EQ Data Conditioning Failed.");
                                    break;
                                }
                                Logger.WriteInfo("Exported EQ Data Conditioning Succeeded.");
                            }
                            break;
                        case "5":
                            {
                                AssetConverter converter = new AssetConverter();
                                bool conversionResult = converter.ConvertEQDataToWOW(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED_FOLDER, Configuration.CONFIG_PATH_EXPORT_FOLDER);
                                if (conversionResult == false)
                                {
                                    Logger.WriteInfo("EQ to WoW conversion Failed.");
                                    break;
                                }

                            }
                            break;
                        case "9":
                            {
                                AssetConditioner conditioner = new AssetConditioner();
                                conditioner.ConditionMusicFiles(Path.Combine(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED, "music"));
                            } break;
                        default:
                            {
                                Logger.WriteInfo("Exiting.");
                                doLoopForCommands = false;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Exception Occurred: " + ex.Message);
                    if (ex.StackTrace != null)
                        Logger.WriteDetail(ex.StackTrace);
                }
            }
        }
        Console.WriteLine("");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}