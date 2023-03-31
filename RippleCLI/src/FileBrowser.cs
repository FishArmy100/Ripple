using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharprompt;
using System.IO;

namespace RippleCLI
{
    static class FileBrowser
    {
        private const string BackSelection = "<- back";
        private const string SelectFolderOption = "- select -";

        public static string SelectFile(string startPath, params string[] extensions)
        {
            string currentPath = GetCurrentFolder(startPath);
            bool isCurrentFile = false;
            while (isCurrentFile == false)
            {
                List<string> options = new List<string>();
                List<string> folders = GetSubFolders(currentPath);
                List<string> files = GetFiles(currentPath, extensions);
                options.AddRange(folders);
                options.AddRange(files);
                options.Add(BackSelection);

                string selection = Prompt.Select("[Folder]: " + GetCurrentFolder(currentPath), options);
                if(folders.Contains(selection))
                {
                    currentPath += "\\" + selection;
                }
                else if(files.Contains(selection))
                {
                    currentPath += "\\" + selection;
                    isCurrentFile = true;
                }
                else if(selection == BackSelection)
                {
                    currentPath = Directory.GetParent(currentPath).FullName;
                }
            }

            return currentPath;
        }

        public static string SelectFolder(string startPath)
        {
            string currentPath = GetCurrentFolder(startPath);
            while (true)
            {
                List<string> options = new List<string>();
                List<string> folders = GetSubFolders(currentPath);
                options.AddRange(folders);
                options.Add(BackSelection);
                options.Add(SelectFolderOption);

                string selection = Prompt.Select("[Folder]: " + GetCurrentFolder(currentPath), options);
                if (folders.Contains(selection))
                {
                    currentPath += "\\" + selection;
                }
                else if(selection == SelectFolderOption)
                {
                    break;
                }
                else if (selection == BackSelection)
                {
                    currentPath = Directory.GetParent(currentPath).FullName;
                }
            }

            return currentPath;
        }

        private static string GetCurrentFolder(string path)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                return path;
            else
                return Directory.GetParent(path).FullName;
        }

        private static List<string> GetSubFolders(string path)
        {
            return Directory.GetDirectories(GetCurrentFolder(path))
                .ToList()
                .ConvertAll(path => new DirectoryInfo(path).Name);
        }

        private static List<string> GetFiles(string path, params string[] extensions)
        {
            return Directory.GetFiles(GetCurrentFolder(path))
                .ToList()
                .ConvertAll(path => new DirectoryInfo(path).Name)
                .Where(p => extensions.Contains(Path.GetExtension(p)))
                .ToList();
        }
    }
}
