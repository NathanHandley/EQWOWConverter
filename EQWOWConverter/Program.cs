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
        Logger.ResetLog();
        Logger.WriteLine("###### EQ WOW Converter ######");
        bool doLoopForCommands = true;
        while (doLoopForCommands == true)
        {   
            Logger.WriteLine("");
            Logger.WriteLine("Options:");
            Logger.WriteLine(" [1] - Condition Exported EQ Data");
            Logger.WriteLine(" [5] - Convert EQ Data to WOW");
            Logger.WriteLine(" [X] - Exit");
            Console.Write("Command (Default: X): ");
            string? enteredCommand = Console.ReadLine();
            if (enteredCommand == null)
            {
                Logger.WriteLine("Exiting.");
                doLoopForCommands = false;
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
                            bool condenseResult = conditioner.ConditionEQOutput(Configuration.CONFIG_PATH_EQEXPORTSRAW, Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED);
                            if (condenseResult == false)
                            {
                                Logger.WriteLine("Exported EQ Data Conditioning Failed.");
                                break;
                            }
                            Logger.WriteLine("Exported EQ Data Conditioning Succeeded.");
                        }
                        break;
                    case "5":
                        {
                            Logger.WriteLine("Converting from EQ to WoW...");
                            AssetConverter converter = new AssetConverter();
                            bool conversionResult = AssetConverter.ConvertEQDataToWOW(Configuration.CONFIG_PATH_EQEXPORTSCONDITIONED, Configuration.CONFIG_PATH_EXPORT_FOLDER);
                            if (conversionResult == false)
                            {
                                Logger.WriteLine("EQ to WoW conversion Failed.");
                                break;
                            }
                            Logger.WriteLine("Conversion of data complete");
                        } break;
                    default:
                        {
                            Logger.WriteLine("Exiting.");
                            doLoopForCommands = false;
                        }
                        break;
                }
            }
        }
    }
}