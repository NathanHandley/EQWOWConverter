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

using System.Text;

namespace EQWOWConverter.EQFiles
{
    // Lots of good info on this XMI format here: https://moddingwiki.shikadi.net/wiki/XMI_Format
    internal class EQMusicXMI
    {
        public static void ExtractMIDIFromXMI(string workingFolder, string fileName)
        {
            Logger.WriteDebug("Extracting XMI from '", fileName, "' into directory '", workingFolder, "'...");

            // Grab the raw data
            string fullXMIFileName = Path.Combine(workingFolder, fileName);
            if (File.Exists(fullXMIFileName) == false)
            {
                Logger.WriteError("ExtractMIDIFromXMI failed, could not find file named '", fullXMIFileName, "'");
                return;
            }
            List<byte> dataBytes = FileTool.GetFileBytes(fullXMIFileName);
            int byteCursor = 0;

            // FORM + XDIR
            string formString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
            UInt32 formSize = ByteTool.ReadUInt32FromBytesBigEndian(dataBytes, ref byteCursor);
            string xdirString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
            if (formString != "FORM" || xdirString != "XDIR")
            {
                Logger.WriteError("Failed to extract midi from XMI file ", fileName, " as it was missing the FORM XDIR block at the start");
                return;
            }

            // INFO chunk, skip it
            string infoString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
            UInt32 infoSize = ByteTool.ReadUInt32FromBytesBigEndian(dataBytes, ref byteCursor);
            byteCursor += (int)infoSize;

            // CAT + XMID
            string catString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
            UInt32 catSize = ByteTool.ReadUInt32FromBytesBigEndian(dataBytes, ref byteCursor);
            string xmidString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
            if (catString != "CAT " || xmidString != "XMID")
            {
                Logger.WriteError("Failed to extract midi from XMI file ", fileName, " as it was missing the CAT XMID block");
                return;
            }

            // The rest of the data is the MIDI data
            while (byteCursor < dataBytes.Count - 8)
            {
                string subFormString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
                if (subFormString != "FORM")
                {
                    Logger.WriteError("Error, when extracting XMI there was a MIDI block that did not start with FORM for file, ", fileName);
                    break;
                }
                UInt32 songBlockSize = ByteTool.ReadUInt32FromBytesBigEndian(dataBytes, ref byteCursor);
                string subXMIDString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
                
                // Skip if it's not XMID
                if (subXMIDString != "XMID")
                {
                    byteCursor += (int)songBlockSize - 4;
                    continue;
                }

                // Grab EVNT data
                List<byte> evntBytes = new List<byte>();
                int formBytesCursorEnd = byteCursor + (int)songBlockSize - 4;
                while (byteCursor < formBytesCursorEnd)
                {
                    string chunkString = ByteTool.ReadStringFromBytes(dataBytes, ref byteCursor, 4);
                    UInt32 chunkSize = ByteTool.ReadUInt32FromBytesBigEndian(dataBytes, ref byteCursor);
                    if (chunkString == "EVNT")
                        evntBytes.AddRange(dataBytes.GetRange(byteCursor, (int)chunkSize));
                    byteCursor += (int)chunkSize;
                    if (chunkSize % 2 == 1)
                        byteCursor++; // This is for IFF padding
                }
                // If EVNT data was grabbed, this is what will turn into a MIDI
                if (evntBytes.Count > 0)
                {
                    // TODO
                }
            }
        }
    }
}
