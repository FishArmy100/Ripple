using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.App
{
    enum AppInputCommand
    {
        Run,
        Close,
        SelectFile,
        SelectFolder,
    }

    static class InputCommandHelper
    {
        private static Dictionary<string, AppInputCommand> s_Commands = new Dictionary<string, AppInputCommand>
        {
            { "run",    AppInputCommand.Run             },
            { "close",  AppInputCommand.Close           },
            { "file",   AppInputCommand.SelectFile      },
            { "folder",   AppInputCommand.SelectFolder  },
        };

        public static bool TryGetCommand(string input, out AppInputCommand command)
        {
            return s_Commands.TryGetValue(input, out command);
        }
    }
}
