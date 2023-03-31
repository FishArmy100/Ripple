using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Raucse;

namespace Ripple.Compiling
{
    public class SourceData
    {
        public readonly string StartPath;
        public readonly List<SourceFile> Files;
        public readonly string SourceName;

        private SourceData(string path, bool isDirectory)
        {
            if(isDirectory)
            {
                StartPath = path;
                SourceName = Path.GetFileName(path);
                Files = GetAllFiles(path);
            }
            else
            {
                StartPath = Path.GetDirectoryName(path);
                SourceName = Path.GetFileNameWithoutExtension(path);
                Files = new List<SourceFile> { new SourceFile(StartPath, Path.GetFileName(path)) };
            }
        }

        public static Option<SourceData> FromPath(string path)
        {
            if(File.Exists(path))
            {
                return new SourceData(path, false);
            }
            if(Directory.Exists(path))
            {
                return new SourceData(path, true);
            }
            else
            {
                return new Option<SourceData>();
            }
        }

        private List<SourceFile> GetAllFiles(string path)
        {
            List<string> files = Directory.GetFiles(path)
                .Where(p => Path.GetExtension(p) == Core.FileExtensions.RippleFileExtension)
                .Select(p => p.Remove(0, StartPath.Length + 1))
                .ToList();
            
            var subFiles = Directory.GetDirectories(path)
                .Select(p => GetAllFiles(path))
                .SelectMany(p => p);

            return files.Select(f => new SourceFile(StartPath, f)).ToList();
        }
    }
}
