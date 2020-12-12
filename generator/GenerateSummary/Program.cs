using System;
using System.Collections.Generic;
using System.IO;

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
        private static int currentPart = 0;
        private static int currentChapter = 0;
        private static string outPath = "..\\..\\..\\..\\chapters\\";
        private static string dataPath = "..\\..\\..\\..\\..\\data\\book.md";
        private static string readmePath = "..\\..\\..\\..\\..\\data\\README.md";
        static void Main(string[] args)
        {
            
            string[] lines = File.ReadAllLines(dataPath);

            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            Directory.CreateDirectory(outPath);

            foreach (var line in lines)
            {                
                if (line.StartsWith("##"))
                {
                    WriteCurrent();
                    currentHeading = line.Substring(2).Trim();
                    currentPrefix = "##";
                }
                else if (line.StartsWith("#"))
                {
                    currentPart++;
                    WriteCurrent();
                    currentHeading = line.Substring(1).Trim();
                    currentPrefix = "#";
                }
                else
                {
                    currentLines.Add(line);
                }
            }

            File.WriteAllLines(outPath + "summary.md", summary);
            File.Copy(readmePath, outPath + "README.md");
        }

        private static void WriteCurrent()
        {
            if (!String.IsNullOrEmpty(currentHeading))
            {
                // dump current lines into a file called
                var fileName = currentPrefix == "#" ? $"{currentPart}.md" : $"{currentPart}.{currentChapter}.md";
                File.WriteAllLines(outPath + "summary.md", currentLines);
                // add entry into summary 
                if (currentPrefix == "#")
                {
                    // * [Part I](part1/README.md)
                    summary.Add($"* [{currentHeading}]({fileName})");                                        
                    currentChapter = 0;
                }
                else
                {
                    //      * [Writing is nice](part1 / writing.md)
                    summary.Add($"\t * [{currentHeading}]({fileName})");
                    currentChapter++;
                }
                currentLines.Insert(0, $"# {currentHeading}");
                File.WriteAllLines(outPath + fileName, currentLines);
            }
            currentLines = new List<string>();
        }
    }
}
