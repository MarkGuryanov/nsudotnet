using System;
using System.IO;

namespace Guryanov.Nsudotnet.LinesCounter
{
    class Program
    {
        private static ulong CountLines(string filename)
        {
            ulong linesCount = 0;
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    bool inMultiLineComment = false;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        line = line.Trim();

                        bool lineContainsChars = !inMultiLineComment && line.Length == 1;
                        for (int i = 0; i < line.Length - 1; i++)
                        {
                            if (!inMultiLineComment)
                            {
                                if (line[i] == '/')
                                {
                                    if (line[i + 1] == '*')
                                    {
                                        inMultiLineComment = true;
                                        ++i;
                                    }
                                    else if (line[i + 1] == '/') break;
                                    else lineContainsChars = true;
                                }
                                else if (!char.IsWhiteSpace(line, i))
                                    lineContainsChars = true;
                            }
                            else
                            {
                                if (line[i] == '*' && line[i + 1] == '/')
                                {
                                    inMultiLineComment = false;
                                    ++i;
                                }
                            }
                        }
                        if (lineContainsChars) ++linesCount;
                    }
                }
            }

            return linesCount;
        }
        
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Guryanov.Nsudotnet.LinesCounter.exe <source files extension>");
                return;
            }

            string path = Directory.GetCurrentDirectory();
            string searchPattern = "*." + args[0];
            string[] filenames = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);

            ulong totalLines = 0;
            foreach (var filename in filenames)
            {
                totalLines += CountLines(filename);
            }
            Console.WriteLine("Total number of code lines: {0}", totalLines);
        }
    }
}
