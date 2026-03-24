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
    // And MID format here: https://moddingwiki.shikadi.net/wiki/MID_Format
    internal class EQMusicXMI
    {
        class MidiEvent
        {
            public UInt64 Tick;
            public byte Status;
            public byte Data1;
            public byte Data2;

            public MidiEvent(ulong tick, byte status, byte data1, byte data2)
            {
                Tick = tick;
                Status = status;
                Data1 = data1;
                Data2 = data2;
            }
        }

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
            int subSongID = 0;
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
                    // Derive a file name
                    string numFragment = string.Empty;
                    if (subSongID < 10)
                        numFragment = string.Concat("0", subSongID);
                    else
                        numFragment = subSongID.ToString();
                    string midiFileName = string.Concat( Path.GetFileNameWithoutExtension(fileName), "-", numFragment, ".mid");
                    subSongID++;

                    // Generate/Output the file
                    List<byte> midiBytes = GetMIDIBytesFromEVNTData(evntBytes);
                    string outputFileName = Path.Combine(workingFolder, midiFileName);
                    FileTool.WriteFileBytes(outputFileName, midiBytes);
                    Logger.WriteDebug("Wrote midi file named '", outputFileName, "'");
                }
            }
        }

        private static List<byte> GetMIDIBytesFromEVNTData(List<byte> evntBytes)
        {
            List<MidiEvent> events = new List<MidiEvent>();
            int byteCursor = 0;
            UInt64 tick = 0;

            while (byteCursor < evntBytes.Count)
            {
                ulong delta = ReadXmiDelta(evntBytes, ref byteCursor);
                tick += delta;

                if (byteCursor >= evntBytes.Count)
                    break;

                byte status = evntBytes[byteCursor++];
                byte cmd = (byte)(status & 0xF0);
                byte ch = (byte)(status & 0x0F);

                if (cmd == 0x90) // Note on + duration
                {
                    byte note = evntBytes[byteCursor++];
                    byte vel = evntBytes[byteCursor++];
                    UInt64 dur = ReadVLQ(evntBytes, ref byteCursor);

                    events.Add(new MidiEvent(tick, (byte)(0x90 | ch), note, vel));
                    if (vel > 0 && dur > 0)
                    {
                        events.Add(new MidiEvent(tick + dur, (byte)(0x80 | ch), note, 0));
                    }
                }
                else if (cmd == 0x80) // Note off
                {
                    byte note = evntBytes[byteCursor++];
                    byte vel = evntBytes[byteCursor++];
                    events.Add(new MidiEvent(tick, status, note, vel));
                }
                else if (cmd == 0xB0) // Controller
                {
                    byte ctrl = evntBytes[byteCursor++];
                    byte val = evntBytes[byteCursor++];
                    events.Add(new MidiEvent(tick, status, ctrl, val));
                }
                else if (cmd == 0xC0) // Instrument change
                {
                    byte prog = evntBytes[byteCursor++];
                    events.Add(new MidiEvent(tick, status, prog, 0));
                }
                else if (cmd == 0xE0) // Pitch bend
                {
                    byte lsb = evntBytes[byteCursor++];
                    byte msb = evntBytes[byteCursor++];
                    events.Add(new MidiEvent(tick, status, lsb, msb));
                }
                else if (cmd == 0xA0 || cmd == 0xD0) // Polyphonic key pressure & channel pressure
                {
                    byte data1 = evntBytes[byteCursor++];
                    events.Add(new MidiEvent(tick, status, data1, 0));
                }
                else if (status == 0xFF) // Meta (including tempo events, ignore for now for timing)
                {
                    byte metaType = evntBytes[byteCursor++];
                    UInt64 len = ReadVLQ(evntBytes, ref byteCursor);
                    byteCursor += (int)len;
                    if (metaType == 0x2F) break;
                }
                else if (status == 0xF0 || status == 0xF7) // System exclusive && Manufacturer-specific
                {
                    UInt64 len = ReadVLQ(evntBytes, ref byteCursor);
                    byteCursor += (int)len;
                }
                else
                {
                    int bytes = (cmd == 0xC0 || cmd == 0xD0) ? 1 : 2;
                    byteCursor += bytes;
                }
            }

            // Sort the events (not sure if required, but easier to debug)
            events = events.OrderBy(e => e.Tick)
                           .ThenBy(e => e.Status == 0x80 ? 0 : (e.Status == 0x90 ? 1 : 2))
                           .ToList();

            return BuildMidiFromEvents(events);
        }

        private static UInt64 ReadXmiDelta(List<byte> data, ref int byteCursor)
        {
            UInt64 delta = 0;
            while (byteCursor < data.Count)
            {
                byte b = data[byteCursor];
                if ((b & 0x80) != 0) // High bit means this is the last byte of data
                    break;
                delta += b;
                byteCursor++;
            }
            return delta;
        }

        private static List<byte> BuildMidiFromEvents(List<MidiEvent> events)
        {
            List<byte> midiData = new List<byte>();

            // Header chunk (MThd)
            midiData.AddRange(Encoding.ASCII.GetBytes("MThd"));

            // Header length is 6 bytes
            ByteTool.WriteUInt32BigEndianToBytes(midiData, 6);

            // Format 0, 1 track, 120 PPQN
            ByteTool.WriteUInt16BigEndianToBytes(midiData, 0);   // Format
            ByteTool.WriteUInt16BigEndianToBytes(midiData, 1);   // nTracks
            ByteTool.WriteUInt16BigEndianToBytes(midiData, 120); // Division (PPQN)

            // Track chunk (MTrk)
            midiData.AddRange(Encoding.ASCII.GetBytes("MTrk"));

            // Reserve 4 bytes for track length (gets filled later)
            int lengthPosition = midiData.Count;
            midiData.AddRange(new byte[4]);  // placeholder of "00 00 00 00"

            int dataStart = midiData.Count;

            // Delta = 0 before first event
            WriteVLQ(midiData, 0);

            // Set tempo meta (1,000,000 microseconds = 120 Hz / 120 ticks per second)
            midiData.Add(0xFF);
            midiData.Add(0x51);     // Tempo meta event
            midiData.Add(0x03);     // length = 3 bytes
            midiData.Add(0x0F);
            midiData.Add(0x42);
            midiData.Add(0x40);     // 0x0F4240 = 1,000,000 microseconds

            UInt64 prevTick = 0;
            foreach (MidiEvent ev in events)
            {
                UInt64 delta = ev.Tick - prevTick;
                prevTick = ev.Tick;

                WriteVLQ(midiData, delta);

                midiData.Add(ev.Status);
                midiData.Add(ev.Data1);

                if (IsSingleByteStatus(ev.Status) == false)
                    midiData.Add(ev.Data2);
            }

            // End of Track meta event
            WriteVLQ(midiData, 0);
            midiData.Add(0xFF);
            midiData.Add(0x2F);
            midiData.Add(0x00);

            // Patch the track length
            uint trackLength = (uint)(midiData.Count - dataStart);
            int currentPos = midiData.Count;

            // Go back and write the 4-byte Big Endian length
            midiData[lengthPosition + 0] = (byte)((trackLength >> 24) & 0xFF);
            midiData[lengthPosition + 1] = (byte)((trackLength >> 16) & 0xFF);
            midiData[lengthPosition + 2] = (byte)((trackLength >> 8) & 0xFF);
            midiData[lengthPosition + 3] = (byte)(trackLength & 0xFF);

            return midiData;
        }

        private static bool IsSingleByteStatus(byte status)
        {
            byte high = (byte)(status & 0xF0);
            return high == 0xC0 || high == 0xD0;
        }

        // VLQ = Variable-Length Quantity
        private static UInt64 ReadVLQ(List<byte> data, ref int byteCursor)
        {
            UInt64 value = 0;
            while (byteCursor < data.Count)
            {
                byte b = data[byteCursor++];
                value = (value << 7) | (UInt64)(b & 0x7F);
                if ((b & 0x80) == 0)
                    break;
            }
            return value;
        }

        private static void WriteVLQ(List<byte> data, UInt64 value)
        {
            if (value == 0)
            {
                data.Add(0);
                return;
            }

            byte[] temp = new byte[10];
            int i = 0;
            do
            {
                temp[i++] = (byte)(value & 0x7F);
                value >>= 7;
            } while (value > 0);

            for (int j = i - 1; j >= 0; j--)
            {
                byte b = temp[j];
                if (j > 0)
                    b |= 0x80;
                data.Add(b);
            }
        }
    }
}
