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

using EQWOWConverter.WOWFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.Files.WOWFiles
{
    internal class DBC
    {
        public class DBCHeader
        {
            public UInt32 RecordCount;
            public UInt32 FieldCount;
            public UInt32 RecordSize;
            public UInt32 StringBlockSize;

            public bool PopulateFromBytes(List<byte> bytes)
            {
                string magic = new string(Encoding.ASCII.GetChars(bytes.ToArray(), 0, 4));
                if (magic != "WDBC")
                {
                    Logger.WriteError("Attempted to read a dbc that didn't have WDBC in the header");
                    return false;
                }
                RecordCount = BitConverter.ToUInt32(bytes.ToArray(), 4);
                FieldCount = BitConverter.ToUInt32(bytes.ToArray(), 8);
                RecordSize = BitConverter.ToUInt32(bytes.ToArray(), 12);
                StringBlockSize = BitConverter.ToUInt32(bytes.ToArray(), 16);
                return true;
            }

            public List<byte> ToBytes()
            {
                List<byte> byteArray = new List<byte>();
                byteArray.AddRange(Encoding.ASCII.GetBytes("WDBC"));
                byteArray.AddRange(BitConverter.GetBytes(RecordCount));
                byteArray.AddRange(BitConverter.GetBytes(FieldCount));
                byteArray.AddRange(BitConverter.GetBytes(RecordSize));
                byteArray.AddRange(BitConverter.GetBytes(StringBlockSize));
                return byteArray;
            }
        }

        public class DBCRow
        {
            public List<byte> RawBytes = new List<byte>();
        }

        protected DBCHeader Header = new DBCHeader();
        protected List<DBCRow> Rows = new List<DBCRow>();
        protected List<char> StringBlock = new List<char>();
        protected bool IsLoaded = false;

        public void LoadFromDisk(string fileName)
        {
            Logger.WriteDetail("Loading dbc at '" + fileName + "' started...");

            // Clear data if already loaded
            if (IsLoaded == true)
            {
                Logger.WriteDetail("DBC file was already loaded, so unloading first");
                DBCHeader header = new DBCHeader();
                Rows.Clear();
                StringBlock.Clear();
                IsLoaded = false;
            }

            // Don't try to load files that don't exist
            if (File.Exists(fileName) == false)
            {
                Logger.WriteError("Could not load dbc file at '" + fileName + "' as it did not exist");
                return;
            }

            // Load all file bytes and the header
            List<byte> fileBytes = new List<byte>();
            fileBytes.AddRange(File.ReadAllBytes(fileName));
            if (Header.PopulateFromBytes(fileBytes) == false)
            {
                Logger.WriteError("Could not load dbc file at '" + fileName + "' as the header was invalid");
                return;
            }
            UInt32 curByteCursor = 20; // Header is 20 bytes in size

            // Read in all of the records
            for (UInt32 i = 0; i < Header.RecordCount; ++i)
            {
                DBCRow row = new DBCRow();
                row.RawBytes.AddRange(fileBytes.GetRange(Convert.ToInt32(curByteCursor), Convert.ToInt32(Header.RecordSize)));
                Rows.Add(row);
                curByteCursor += Header.RecordSize;
            }

            // Read in the string block
            StringBlock.AddRange(Encoding.ASCII.GetChars(fileBytes.ToArray(), Convert.ToInt32(curByteCursor), Convert.ToInt32(Header.StringBlockSize)));

            // Done loading
            IsLoaded = true;
            Logger.WriteDetail("Loading dbc at '" + fileName + "' completed");
        }

        public void SaveToDisk(string fileName)
        {
            Logger.WriteDetail("Saving dbc at '" + fileName + "' started...");
            
            // Don't try to save files not loaded
            if (IsLoaded == false)
            {
                Logger.WriteError("Could not save DBC to '" + fileName + "', it was not loaded");
                return;
            }

            // Delete the file if it already exists
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                Logger.WriteDetail("File already exists, so deleting");
            }

            // Set the header data
            Header.RecordCount = Convert.ToUInt32(Rows.Count);
            Header.StringBlockSize = Convert.ToUInt32(StringBlock.Count);

            // Build the output file payload
            List<byte> outputBytes = new List<byte>();
            outputBytes.AddRange(Header.ToBytes());
            foreach (DBCRow row in Rows)
                outputBytes.AddRange(row.RawBytes);
            outputBytes.AddRange(Encoding.ASCII.GetBytes(StringBlock.ToArray()));

            // Write it
            File.WriteAllBytes(fileName, outputBytes.ToArray());

            Logger.WriteDetail("Saving dbc at '" + fileName + "' completed");
        }
    }
}
