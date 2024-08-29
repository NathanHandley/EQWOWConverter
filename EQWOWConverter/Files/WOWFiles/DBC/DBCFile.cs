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

            public class DBCFieldInt : DBCField
            {
                public DBCFieldInt(Int32 value) { Value = value; }
                public Int32 Value;
            }

            public void AddInt(Int32 value)
            {
                AddedFields.Add(new DBCFieldInt(value));
            }

            public void AddPackedFlags(Int32 value)
            {
                AddedFields.Add(new DBCFieldInt(value));
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

            public List<DBCRow.DBCField> AddedFields = new List<DBCField>();
            public List<byte> SourceRawBytes = new List<byte>();
        }

        protected string FileName = string.Empty;
        protected DBCHeader Header = new DBCHeader();
        protected List<DBCRow> Rows = new List<DBCRow>();
        protected List<char> StringBlock = new List<char>();
        protected bool IsLoaded = false;

        public void LoadFromDisk(string fileFolder, string fileName)
        {
            Logger.WriteDetail("Loading dbc '" + fileName + "' started...");

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
            Logger.WriteDetail("Loading dbc '" + fileName + "' completed");
        }

        public void SaveToDisk(string fileFolder)
        {
            Logger.WriteDetail("Saving dbc at '" + FileName + "' started...");

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
                Logger.WriteDetail("File already exists, so deleting");
            }

            // Set the header data
            Header.RecordCount = Convert.ToUInt32(Rows.Count);
            Header.StringBlockSize = Convert.ToUInt32(StringBlock.Count);

            // Build the output file payload
            List<byte> outputBytes = new List<byte>();
            outputBytes.AddRange(Header.ToBytes());
            foreach (DBCRow row in Rows)
            { 
                // Use raw data if there are no added fields, otherwise use the added fields
                if (row.AddedFields.Count == 0)
                    outputBytes.AddRange(row.SourceRawBytes);
                else
                {
                    foreach(var addedField in row.AddedFields)
                    {
                        if (addedField.GetType() == typeof(DBCRow.DBCFieldInt))
                        {
                            DBCRow.DBCFieldInt rowField = (DBCRow.DBCFieldInt)addedField;
                            outputBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldFloat))
                        {
                            DBCRow.DBCFieldFloat rowField = (DBCRow.DBCFieldFloat)addedField;
                            outputBytes.AddRange(BitConverter.GetBytes(rowField.Value));
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldString))
                        {
                            // Strings store by offset
                            DBCRow.DBCFieldString rowField = (DBCRow.DBCFieldString)addedField;
                            string value = rowField.Value + "\0";
                            outputBytes.AddRange(BitConverter.GetBytes(StringBlock.Count)); 
                            StringBlock.AddRange(value);
                        }
                        else if (addedField.GetType() == typeof(DBCRow.DBCFieldStringLang))
                        {
                            // Language strings are 16 string columns and a flags field (17 total), with 0 (first) being english
                            DBCRow.DBCFieldStringLang rowField = (DBCRow.DBCFieldStringLang)addedField;
                            string value = rowField.Value + "\0";

                            // English string
                            outputBytes.AddRange(BitConverter.GetBytes(StringBlock.Count));
                            StringBlock.AddRange(value);

                            // 15 blank strings
                            for (int i = 0; i < 15; i++)
                                outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0)));

                            // Flags (language mask)
                            outputBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(16712190)));
                        }
                        else
                        {
                            Logger.WriteError("Error adding field to DBC file '" + FileName + "', as type of '" + addedField.GetType().ToString() + "' was not implemented. DBC will corrupt");
                        }
                    }
                }
            }
            outputBytes.AddRange(Encoding.ASCII.GetBytes(StringBlock.ToArray()));

            // Write it
            File.WriteAllBytes(fullFilePath, outputBytes.ToArray());

            Logger.WriteDetail("Saving dbc at '" + FileName + "' completed");
        }
    }
}
