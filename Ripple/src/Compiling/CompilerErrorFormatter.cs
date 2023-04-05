using Raucse.FileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;

namespace Ripple.Compiling
{
    public static class CompilerErrorFormatter
    {
        public static List<string> Format(List<CompilerError> errors)
        {
            return errors
                .GroupBy(e => e.Location.File)
                .Select(group =>
                {
                    string src = FileUtils.ReadFromFile(group.Key).Value;
                    return group.Select(e =>
                    {
                        return FormatError(e, src);
                    });
                })
                .SelectMany(e => e)
                .ToList();
        }

        private static string FormatError(CompilerError error, string fileSource)
        {
            
        }

        private static string FormatErrorLine(string line, int lineNumber)
        {
            string lineNumberText = lineNumber.ToString();
            string text = $"{lineNumber} | {line}\n";

            string offset = new string(' ', lineNumberText.Length);
            string squigles = new string('~', line.Length);
            text += $"{offset} | {squigles}\n";

            return text;
        }
    }
}
