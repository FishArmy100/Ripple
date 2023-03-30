using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils;
using System.Diagnostics;
using System.IO;
using Ripple.Utils.Extensions;

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
    }
}
