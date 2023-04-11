using Raucse.FileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse.Extensions;
using Ripple.Core;

namespace Ripple.Compiling
{
    public static class CompilerErrorFormatter
    {
        public static List<string> Format(IEnumerable<CompilerError> errors)
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
            SourceLocation location = error.Location;
            TextCoordinate coordinate = TextCoordinate.FromIndex(fileSource, location.Start);
            string text = $"{location.File}:{coordinate.Line}:{coordinate.Row}\n";
            text += $"error: {error.GetMessage()}\n";

            string slice = fileSource[location.Start..location.End];
            string[] lines = slice.Split('\n');
            int maxLineCharCount = (coordinate.Line + lines.Length - 1).ToString().Length;

            for(int i = 0; i < lines.Length; i++)
            {
                text += FormatErrorLine(lines[i], coordinate.Line + i, maxLineCharCount);
            }

            return text;
        }

        private static string FormatErrorLine(string line, int lineNumber, int lineCharCount)
        {
            int numberSpacerLength = lineCharCount - lineNumber.ToString().Length;
            string text = $" {new string(' ', numberSpacerLength)}{lineNumber} | {line}\n";

            string offset = new string(' ', lineCharCount);
            string squigles = new string('~', line.Length);
            text += $" {offset} | {squigles}\n";

            return text; 
        }
    }
}
