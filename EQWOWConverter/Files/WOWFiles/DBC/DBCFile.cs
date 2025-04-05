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

using Google.Protobuf.WellKnownTypes;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter.WOWFiles
{
    internal class DBCFile
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
            public class DBCField
            {
            }

            public class DBCFieldInt32 : DBCField
            {
                public DBCFieldInt32(Int32 value) { Value = value; }
                public Int32 Value;
            }

            public void AddInt32(Int32 value)
            {
                AddedFields.Add(new DBCFieldInt32(value));
            }

            public void AddPackedFlags(Int32 value)
            {
                AddedFields.Add(new DBCFieldInt32(value));
            }

            public void AddIntFromSourceRawBytes(ref int offsetCursor)
            {
                if (offsetCursor >= SourceRawBytes.Count)
                {
                    Logger.WriteError("DBCRow AddIntFromSourceRawBytes failure, offsetCursor was beyond the raw data stream");
                    return;
                }
                if (offsetCursor + 4 > SourceRawBytes.Count + 1)
                {
                    Logger.WriteError("DBCRow AddIntFromSourceRawBytes failure, as offsetCursor is trying to pull data that will fall outside the stream");
                    return;
                }

                // Add it
                byte[] intBytes = SourceRawBytes.Skip(offsetCursor).Take(4).ToArray();
                AddedFields.Add(new DBCFieldInt32(BitConverter.ToInt32(intBytes, 0)));
                offsetCursor += 4;
            }

            public class DBCFieldUInt32 : DBCField
            {
                public DBCFieldUInt32(UInt32 value) { Value = value; }
                public UInt32 Value;
            }

            public void AddUInt32(UInt32 value)
            {
                AddedFields.Add(new DBCFieldUInt32(value));
            }

            public class DBCFieldInt64 : DBCField
            {
                public DBCFieldInt64(Int64 value) { Value = value; }
                public Int64 Value;
            }

            public void AddInt64(Int64 value)
            {
                AddedFields.Add(new DBCFieldInt64(value));
            }

            public class DBCFieldUInt64 : DBCField
            {
                public DBCFieldUInt64(UInt64 value) { Value = value; }
                public UInt64 Value;
            }

            public void AddUInt64(UInt64 value)
            {
                AddedFields.Add(new DBCFieldUInt64(value));
            }

            public class DBCFieldFloat : DBCField
            {
                public DBCFieldFloat(float value) { Value = value; }
                public float Value;
            }

            public void AddFloat(float value)
            {
                AddedFields.Add(new DBCFieldFloat(value));
            }

            public void AddFloatFromSourceRawBytes(ref int offsetCursor)
            {
                if (offsetCursor >= SourceRawBytes.Count)
                {
                    Logger.WriteError("DBCRow AddFloatFromSourceRawBytes failure, offsetCursor was beyond the raw data stream");
                    return;
                }
                if (offsetCursor + 4 > SourceRawBytes.Count + 1)
                {
                    Logger.WriteError("DBCRow AddFloatFromSourceRawBytes failure, as offsetCursor is trying to pull data that will fall outside the stream");
                    return;
                }

                // Add it
                byte[] floatBytes = SourceRawBytes.Skip(offsetCursor).Take(4).ToArray();
                AddedFields.Add(new DBCFieldFloat(BitConverter.ToSingle(floatBytes, 0)));
                offsetCursor += 4;
            }

            public class DBCFieldString : DBCField
            {
                public DBCFieldString(string value) { Value = value; }
                public string Value;
            }

            public void AddString(string value)
            {
                AddedFields.Add(new DBCFieldString(value));
            }

            public class DBCFieldStringLang : DBCField
            {
                public DBCFieldStringLang(string value) { Value = value; }
                public string Value;
            }

            public void AddStringLang(string value)
            {
                AddedFields.Add(new DBCFieldStringLang(value));
            }

            // Note: Only work with the first description (english).  Will probably break for other language types
            public void AddStringLangFromSourceRawBytes(ref int offsetCursor, List<char> stringBlock)
            {
                if (offsetCursor >= SourceRawBytes.Count)
                {
                    Logger.WriteError("DBCRow AddStringLangFromSourceRawBytes failure, offsetCursor was beyond the raw data stream");
                    return;
                }

                // Load in the 16 string offsets
                List<int> stringOffsets = new List<int>();
                for (int i = 0; i < 16; i++)
                {
                    // Current string offset
                    byte[] intBytes = SourceRawBytes.Skip(offsetCursor).Take(4).ToArray();
                    stringOffsets.Add(BitConverter.ToInt32(intBytes));
                    offsetCursor += 4;
                }

                // First string is the only one we care about
                string curLangStringValue = string.Empty;
                for (int i = stringOffsets[0]; i < stringBlock.Count; i++)
                {
                    if (stringBlock[i] == '\0')
                        break;
                    curLangStringValue += stringBlock[i];
                }
                AddedFields.Add(new DBCFieldStringLang(curLangStringValue));

                // Throw out the language mask flag
                offsetCursor += 4;
            }

            public List<DBCRow.DBCField> AddedFields = new List<DBCField>();
            public List<byte> SourceRawBytes = new List<byte>();
        }

        protected string FileName = string.Empty;
        protected DBCHeader Header = new DBCHeader();
        protected List<DBCRow> Rows = new List<DBCRow>();
        protected List<char> StringBlock = new List<char>();
        protected Dictionary<string, int> StringBlockStringOffsets = new Dictionary<string, int>();
        protected bool IsLoaded = false;

        public void LoadFromDisk(string fileFolder, string fileName)
        {
            Logger.WriteDebug("Loading dbc '" + fileName + "' started...");

            // Clear data if already loaded
            if (IsLoaded == true)
            {
                Logger.WriteDebug("DBC file was already loaded, so unloading first");
                DBCHeader header = new DBCHeader();
                Rows.Clear();
                StringBlock.Clear();
                IsLoaded = false;
            }

            // Don't try to load files that don't exist
            string fullFilePath = Path.Combine(fileFolder, fileName);
            if (File.Exists(fullFilePath) == false)
            {
                Logger.WriteError("Could not load dbc file at '" + fullFilePath + "' as it did not exist");
                return;
            }
            FileName = fileName;

            // Load all file bytes and the header
            List<byte> fileBytes = new List<byte>();
            fileBytes.AddRange(File.ReadAllBytes(fullFilePath));
            if (Header.PopulateFromBytes(fileBytes) == false)
            {
                Logger.WriteError("Could not load dbc file at '" + fullFilePath + "' as the header was invalid");
                return;
            }
            UInt32 curByteCursor = 20; // Header is 20 bytes in size

            // Read in all of the records
            for (UInt32 i = 0; i < Header.RecordCount; ++i)
            {
                DBCRow row = new DBCRow();
                row.SourceRawBytes.AddRange(fileBytes.GetRange(Convert.ToInt32(curByteCursor), Convert.ToInt32(Header.RecordSize)));
                Rows.Add(row);
                curByteCursor += Header.RecordSize;
            }

            // Read in the string block
            StringBlock.AddRange(Encoding.ASCII.GetChars(fileBytes.ToArray(), Convert.ToInt32(curByteCursor), Convert.ToInt32(Header.StringBlockSize)));

            // Done loading
            IsLoaded = true;
            Logger.WriteDebug("Loading dbc '" + fileName + "' completed");

            // Call post load event for hooks later
            OnPostLoadDataFromDisk();
        }

        public void SaveToDisk(string fileFolder)
        {
            Logger.WriteDebug("Saving dbc at '" + FileName + "' started...");

            // Don't try to save files not loaded
            if (IsLoaded == false)
            {
                Logger.WriteError("Could not save DBC to '" + fileFolder + "', it was not loaded");
                return;
            }

            // Delete the file if it already exists
            string fullFilePath = Path.Combine(fileFolder, FileName);
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
                Logger.WriteDebug("File already exists, so deleting");
            }

            // Build the output file payload
            List<byte> contentBytes = new List<byte>();
            foreach (DBCRow row in Rows)
            {
                // Use raw data if there are no added fields, otherwise use the added fields
                if (row.AddedFields.Count == 0)
                    contentBytes.AddRange(row.SourceRawBytes);
                else
                {
                    foreach (var addedField in row.AddedFields)
                    {
                        if (addedField.GetType() == typeof(DBCRow.DBCFieldInt32))
                        {
                            DBCRow.DBCFieldInt32 rowField = (DBCRow.DBCFieldInt32)addedField;
                            contentBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldUInt32))
                        {
                            DBCRow.DBCFieldUInt32 rowField = (DBCRow.DBCFieldUInt32)addedField;
                            contentBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldInt64))
                        {
                            DBCRow.DBCFieldInt64 rowField = (DBCRow.DBCFieldInt64)addedField;
                            contentBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldUInt64))
                        {
                            DBCRow.DBCFieldUInt64 rowField = (DBCRow.DBCFieldUInt64)addedField;
                            contentBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldFloat))
                        {
                            DBCRow.DBCFieldFloat rowField = (DBCRow.DBCFieldFloat)addedField;
                            contentBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldString))
                        {
                            // Strings store by offset
                            DBCRow.DBCFieldString rowField = (DBCRow.DBCFieldString)addedField;
                            string value = rowField.Value;
                            contentBytes.AddRange(BitConverter.GetBytes(PutStringInStringBlockAndGetOffset(value)));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldStringLang))
                        {
                            // Language strings are 16 string columns and a flags field (17 total), with 0 (first) being english
                            DBCRow.DBCFieldStringLang rowField = (DBCRow.DBCFieldStringLang)addedField;
                            string value = rowField.Value;

                            // English string
                            contentBytes.AddRange(BitConverter.GetBytes(PutStringInStringBlockAndGetOffset(value)));

                            // 15 blank strings
                            for (int i = 0; i < 15; i++)
                                contentBytes.AddRange(BitConverter.GetBytes(PutStringInStringBlockAndGetOffset(string.Empty)));

                            // Flags (language mask)
                            contentBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(16712190)));
                        }
                        else
                        {
                            Logger.WriteError("Error adding field to DBC file '" + FileName + "', as type of '" + addedField.GetType().ToString() + "' was not implemented. DBC will corrupt");
                        }
                    }
                }
            }
            contentBytes.AddRange(Encoding.ASCII.GetBytes(StringBlock.ToArray()));

            // Fill the header
            Header.RecordCount = Convert.ToUInt32(Rows.Count);
            Header.StringBlockSize = Convert.ToUInt32(StringBlock.Count);

            // Build the full payload
            List<byte> outputBytes = new List<byte>();
            outputBytes.AddRange(Header.ToBytes());
            outputBytes.AddRange(contentBytes);

            // Write it
            File.WriteAllBytes(fullFilePath, outputBytes.ToArray());

            Logger.WriteDebug("Saving dbc at '" + FileName + "' completed");
        }

        private int PutStringInStringBlockAndGetOffset(string stringToInsert)
        {
            // Append the null at the end
            stringToInsert = stringToInsert + "\0";

            // Re-use a known index if it exists
            if (StringBlockStringOffsets.ContainsKey(stringToInsert))
                return StringBlockStringOffsets[stringToInsert];

            // Add to the string block
            int newIndex = StringBlock.Count;
            StringBlock.AddRange(stringToInsert);

            // Create an index lookup for it
            StringBlockStringOffsets.Add(stringToInsert, newIndex);

            // Return the index
            return newIndex;
        }

        protected virtual void OnPostLoadDataFromDisk() { }
    }
}
