using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class FileTool
    {
        // Method taken from  https://stackoverflow.com/questions/7931304/comparing-two-files-in-c-sharp
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

        public static void CreateBlankDirectory(string targetDirectory)
        {
            if (Directory.Exists(targetDirectory))
                Directory.Delete(targetDirectory, true);
            Directory.CreateDirectory(targetDirectory);
        }

        public static bool CopyDirectoryAndContents(string sourceDirectory, string targetDirectory, bool deleteTargetContents, bool recursive)
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
            foreach (FileInfo file in sourceDirectoryInfo.GetFiles())
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
                    CopyDirectoryAndContents(subDirectory.FullName, newTargetDirectory, deleteTargetContents, recursive);
                }
            }

            return true;
        }
    }
}
