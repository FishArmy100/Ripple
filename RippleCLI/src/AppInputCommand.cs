using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleCLI
{
    enum AppInputCommand
    {
        Run,
        Close,
        SelectFile,
        SelectFolder,
        SelectMode,
        PrintCompilerMode,
        PrintInputPath,
        HelpCommand,
    }

    static class InputCommandHelper
    {
        private static readonly Dictionary<string, AppInputCommand> s_Commands = new Dictionary<string, AppInputCommand>
        {
            { "run",        AppInputCommand.Run                 },
            { "close",      AppInputCommand.Close               },
            { "file",       AppInputCommand.SelectFile          },
            { "folder",     AppInputCommand.SelectFolder        },
            { "input",      AppInputCommand.PrintInputPath      },
            { "pmode",      AppInputCommand.PrintCompilerMode   },
            { "mode",       AppInputCommand.SelectMode          },
            { "help",       AppInputCommand.HelpCommand         },
        };

        public static bool TryGetCommand(string input, out AppInputCommand command)
        {
            return s_Commands.TryGetValue(input, out command);
        }

        public static List<string> GetCommands() => s_Commands.Keys.ToList();
    }
}
