using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using System.Diagnostics;
using System.IO;

namespace Ripple.Compiling.CCompilation
{
    static class ClangCompilerInterface
    {
        public static Option<List<string>> CompileFile(string path, string outputPath, string debugPath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C clang {Path.GetFileName(path)} -o {outputPath} > {debugPath} 2>&1";
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();

            return new Option<List<string>>();
        }
    }
}
