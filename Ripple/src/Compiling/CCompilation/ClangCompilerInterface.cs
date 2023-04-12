using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using System.Diagnostics;
using System.IO;
using Raucse.Extensions;

namespace Ripple.Compiling.CCompilation
{
    static class ClangCompilerInterface
    {
        public static void CompileFiles(string workingDir, IEnumerable<string> files, string outputPath, string debugPath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C clang {files.Concat()} -o {outputPath} > {debugPath} 2>&1";
            startInfo.WorkingDirectory = workingDir;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static Option<string> GetC_AST(string dir)
        {
            List<string> files = GetFiles(dir).Where(d => Path.GetExtension(d) == ".h").ToList();
            if (files.Count == 0)
                return new Option<string>(string.Empty);

            return new Option<string>();
        }

        private static IEnumerable<string> GetFiles(string dir)
        {
            foreach (string path in Directory.GetFiles(dir))
                yield return path;

            foreach(string subdir in Directory.GetDirectories(dir))
            {
                foreach (string path in GetFiles(subdir))
                    yield return path;
            }
        }
    }
}
