using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using System.Diagnostics;
using System.IO;
using Raucse.Extensions;
using Raucse.FileManagement;

namespace Ripple.Compiling.CCompilation
{
    static class ClangCompilerInterface
    {
        public static (bool, string) CompileFiles(string workingDir, IEnumerable<string> files, string outputPath, string debugPath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C clang {files.Concat()} -o {outputPath} 2>&1";
            startInfo.WorkingDirectory = workingDir;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string text = process.StandardOutput.ReadToEnd();

            return (process.ExitCode == 0, text);
        }
    }
}
