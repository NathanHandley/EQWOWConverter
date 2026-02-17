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

namespace EQWOWConverter
{
    internal class FileTool
    {
        // Method taken from  https://stackoverflow.com/questions/7931304/comparing-two-files-in-c-sharp
        // Rights of this method belong to James Johnson (https://stackoverflow.com/users/879420/james-johnson)
        public static bool AreFilesTheSame(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static void CreateBlankDirectory(string targetDirectory, bool skipIfExists)
        {
            if (Directory.Exists(targetDirectory))
            {
                if (skipIfExists)
                    return;
                else
                    Directory.Delete(targetDirectory, true);
            }
            Directory.CreateDirectory(targetDirectory);
        }

        public static bool CopyDirectoryAndContents(string sourceDirectory, string targetDirectory, bool deleteTargetContents, bool recursive, string searchPattern = "*.*")
        {
            // Create the folder if it doesn't exist, or recreate if the contents should be deleted
            if (Directory.Exists(targetDirectory) == false)
                Directory.CreateDirectory(targetDirectory);
            else if (deleteTargetContents)
            {
                Directory.Delete(targetDirectory, true);
                Directory.CreateDirectory(targetDirectory);
            }

            // Get and cache the source directory details
            DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
            DirectoryInfo[] sourceDirectoryInfos = sourceDirectoryInfo.GetDirectories();

            // Copy files from source directory into destination directory
            foreach (FileInfo file in sourceDirectoryInfo.GetFiles(searchPattern))
            {
                string targetFilePath = Path.Combine(targetDirectory, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If set to recursive, also do the same for the directories
            if (recursive)
            {
                foreach (DirectoryInfo subDirectory in sourceDirectoryInfos)
                {
                    string newTargetDirectory = Path.Combine(targetDirectory, subDirectory.Name);
                    CopyDirectoryAndContents(subDirectory.FullName, newTargetDirectory, deleteTargetContents, recursive, searchPattern);
                }
            }

            return true;
        }

        public static bool DeleteFilesInDirectory(string directory, string searchPattern, bool recursive)
        {
            if (Directory.Exists(directory) == false)
            {
                return false;
            }

            string[] files = Directory.GetFiles(directory, searchPattern);
            foreach (string file in files)
            {
                File.Delete(file);
                Logger.WriteDebug("Deleted file '" + file + "' in folder '" + directory + "'");
            }

            if (recursive)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (DirectoryInfo curDirectory in directoryInfos)
                {
                    DeleteFilesInDirectory(curDirectory.FullName, searchPattern, recursive);
                }
            }

            return true;
        }

        public static string ReadAllDataFromFile(string fileName)
        {
            string returnString = string.Empty;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(fs, bufferSize: 102400)) // Set a 100 KB buffer
                        returnString = reader.ReadToEnd();
            }
            catch
            {
                Logger.WriteError("Unable to read all text from file '" + fileName + "'");
            }
            return returnString;
        }

        public static List<string> ReadAllStringLinesFromFile(string fileName, bool stripHeader, bool removeBlankRows)
        {
            // Load in item data
            Logger.WriteDebug("Reading all string lines from file '" + fileName + "'");
            string inputData = FileTool.ReadAllDataFromFile(fileName);
            List<string> inputRows = new List<string>(inputData.Split(Environment.NewLine));
            if (stripHeader == true)
            {
                if (inputRows.Count == 0)
                {
                    Logger.WriteError("Failed to read all string lines from file '" + fileName + "'. stripHeader is 'true' but there are no rows");
                    return new List<string>();
                }
                else
                {
                    inputRows.RemoveAt(0);
                    Logger.WriteDebug("stripHeaders was true, so the first row is deleted from file '" + fileName + "'");
                }
            }
            if (removeBlankRows == true)
            {
                for (int i = inputRows.Count - 1; i >= 0; i--)
                {
                    if (inputRows[i].Trim().Length == 0)
                        inputRows.RemoveAt(i);
                }
            }
            Logger.WriteDebug("All rows read from '" + fileName + "', which has '" + inputRows.Count + "' content rows");
            return inputRows;
        }

        public static List<Dictionary<string, string>> ReadAllRowsFromFileWithHeader(string fileName, string delimeter)
        {
            // Get the rows
            List<Dictionary<string, string>> returnRows = new List<Dictionary<string, string>>();
            List<string> rows = ReadAllStringLinesFromFile(fileName, false, true);

            // For each row, create a blocked return set
            bool isHeader = true;
            List<string> columnNames = new List<string>();
            foreach(string row in rows)
            {
                string[] rowBlocks = row.Split(delimeter);
                if (isHeader == true)
                {
                    foreach(string block in rowBlocks)
                        columnNames.Add(block);
                    isHeader = false;
                }
                else if (rowBlocks.Length == 1)
                {
                    Logger.WriteError("Could not read a proper line from '" + fileName + "'. Make sure the file has this as the delimiter: " + delimeter);
                    continue;
                }
                else
                {
                    Dictionary<string, string> rowValues = new Dictionary<string, string>();
                    for (int i = 0; i < columnNames.Count; i++)
                        rowValues.Add(columnNames[i], rowBlocks[i]);
                    returnRows.Add(rowValues);
                }
            }

            return returnRows;
        }

        public static bool IsFileLocked(string fileName)
        {
            try
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fs.Close();
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static void CopyFile(string sourcePath, string targetPath)
        {
            // Verify Inputs
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetPath))
            {
                Logger.WriteError("CopyFile failure, as source or target were empty");
                return;            
            }
            if (File.Exists(sourcePath) == false)
            {
                Logger.WriteError(string.Concat("CopyFile failure, as source file of '", sourcePath, "' did not exist"));
                return;
            }

            // Create directory if it doesn't exist (Skip for now for performance, but consider enabling)
            //string? targetDirectory = Path.GetDirectoryName(targetPath);
            //if (string.IsNullOrEmpty(targetDirectory) == false)
            //    Directory.CreateDirectory(targetDirectory);

            // Copy the file normally unless the file is locked, then do it manually by a file stream
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (IOException)
            {
                using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var destStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[81920]; // 80KB
                        int bytesRead;
                        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        public static void CreateCombinedTextFile(string sourceTextFilePath1, string sourceTextFilePath2, string targetTextFilePath)
        {
            // Read text from the two files
            string firstFileContent = File.ReadAllText(sourceTextFilePath1);
            string secondFileContent = File.ReadAllText(sourceTextFilePath2);

            // Combine, with the text on a new line
            string combinedContent = firstFileContent.TrimEnd() + Environment.NewLine + secondFileContent;

            // Write the file
            if (File.Exists(targetTextFilePath) == true)
                File.Delete(targetTextFilePath);
            File.WriteAllText(targetTextFilePath, combinedContent);
        }

        public static List<byte> GetFileBytes(string filePath)
        {
            return new List<byte>(File.ReadAllBytes(filePath));
        }

        public static void WriteFileBytes (string filePath, List<byte> bytes)
        {
            File.WriteAllBytes(filePath, bytes.ToArray());
        }
    }
}
