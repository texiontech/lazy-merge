using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeLeak
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = @"D:\merge\deleak\logging";

            var files = Directory.GetFiles(source, "*.log", SearchOption.AllDirectories);
            var fileGroup = new Dictionary<string, string>();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                var directory = new FileInfo(file).Directory;

                var section = directory.FullName.Split('\\');

                fileGroup.Add(file, section[section.Length - 2] == "service" ? "service" : directory.Name);

            }

            System.Console.WriteLine("Preparation destination directory");

            var group = from element in fileGroup
                        group element by element.Value
                  into groups
                        select groups.Key;

            string destination = Path.Combine(source, "merged");

            TryCreateDirectory(destination);

            foreach (var item in group)
            {
                System.Console.WriteLine("GROUP::::=>" + item);

                var fileList = fileGroup.Where(c => c.Value == item).Select(x => x.Key).ToList();

                var mergedFile = Path.Combine(destination, string.Concat(item, ".csv"));
                MergeFile(fileList, mergedFile);
            }

        }

        private static void MergeFile(List<string> files, string destination)
        {
            System.Console.WriteLine(String.Concat("File count:", files.Count));
            System.Console.WriteLine("Create file :" + destination);
            using (var streamOutput = File.Create(destination))
            {
                foreach (var file in files)
                {
                    using (var streamInput = File.OpenRead(file))
                    {
                        streamInput.CopyTo(streamOutput);
                    }
                }
            }
        }

        private static void TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                System.Console.WriteLine("Create Directory:" + path);
            }
            else
                System.Console.WriteLine("Skip create directory, Existing Directory:" + path);
        }
    }
}
