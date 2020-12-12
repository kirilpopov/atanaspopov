using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GenerateSummary
{

    class Program
    {
        private static string currentHeading = "";
        private static string currentPrefix = "#";
        private static List<string> currentLines = new List<string>();
        private static List<string> summary = new List<string>()
            {
                "# Sumary",
                "* [За книгата](README.md)"
            };
        private static int currentPart = 1;
        private static int currentChapter = 0;
        private static string outPath = "..\\..\\..\\..\\chapters\\";
        private static string dataPath = "..\\..\\..\\..\\..\\data\\book.md";
        private static string readmePath = "..\\..\\..\\..\\..\\data\\README.md";
        private static string mediaPath = "..\\..\\..\\..\\..\\data\\media";
        static void Main(string[] args)
        {
            
            string[] lines = File.ReadAllLines(dataPath);

            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
                Thread.Sleep(500);
            }
            Directory.CreateDirectory(outPath);

            foreach (var line in lines)
            {                
                if (line.StartsWith("##"))
                {
                    WriteCurrent("##");
                    currentHeading = line.Substring(2).Trim();
                    currentPrefix = "##";
                }
                else if (line.StartsWith("#"))
                {
                    WriteCurrent("#");
                    currentHeading = line.Substring(1).Trim();
                    currentPrefix = "#";
                }
                else
                {
                    currentLines.Add(line);
                }
            }

            WriteCurrent("##");
            File.WriteAllLines(outPath + "summary.md", summary);
            File.Copy(readmePath, outPath + "README.md");
            DirectoryCopy(mediaPath, outPath + "media", true);
        }

        private static void WriteCurrent(string newPrefix)
        {
            if (!String.IsNullOrEmpty(currentHeading))
            {
                // dump current lines into a file called
                var fileName = $"{currentPart}.{currentChapter}.md";
                File.WriteAllLines(outPath + "summary.md", currentLines);
                // add entry into summary 
                if (currentPrefix == "#")
                {
                    // * [Part I](part1/README.md)
                    summary.Add($"* [{currentHeading}]({fileName})");
                }
                else
                {
                    //      * [Writing is nice](part1 / writing.md)
                    summary.Add($"\t * [{currentHeading}]({fileName})");
                }

                if (newPrefix == "#")
                {
                    currentPart++;
                    currentChapter = 0;
                }
                else
                {
                    currentChapter++;
                }
                currentLines.Insert(0, $"# {currentHeading}");
                File.WriteAllLines(outPath + fileName, currentLines);
            }
            currentLines = new List<string>();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
