using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ripple.Utils
{
    static class FileUtils
    {
        public static void WriteToFile(string path, string text)
        {
            if (!File.Exists(path))
            {
                if(!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                using var fileStream = File.Create(path);
            }

            File.WriteAllLines(path, text.Split('\n'));
        }

        public static bool ReadFolder(string path, out FolderData folderData)
        {
            folderData = new FolderData();
            if(Directory.Exists(path))
            {
                List<(string, string)> files = Directory.GetFiles(path)
                    .ToList()
                    .ConvertAll(p => (p, File.ReadAllText(p)));

                List<string> folderPaths = Directory.GetDirectories(path).ToList();
                List<FolderData> folders = new List<FolderData>();
                foreach(string folderPath in folderPaths)
                {
                    if(ReadFolder(path, out FolderData data))
                    {
                        folders.Add(data);
                    }
                }

                folderData = new FolderData(files, folders);
                return true;
            }

            return false;
        }

        public static bool ReadFile(string path, out string text)
        {
            text = null;
            if (File.Exists(path))
            {
                text = File.ReadAllText(path);
                return true;
            }
            return false;
        }
    }
}
