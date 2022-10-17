using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    enum CompilerMode
    { 
        Lexing,
        Parsing,
        Validating,
        Transpiling,
    }

    class Application
    {
        private const string SelectFileCommand = "-f";
        private const string SetModeCommand = "-m";
        private const string CompileCodeCommand = "-c";
        private const string ExitCommand = "-exit";

        private static Dictionary<string, CompilerMode> CompilerModeCommands = new Dictionary<string, CompilerMode>
        {
            { "lex", CompilerMode.Lexing },
            { "par", CompilerMode.Parsing },
            { "val", CompilerMode.Validating },
            { "tsp", CompilerMode.Transpiling },
        };

        private CompilerMode m_CurrentMode;
        private string CurrentFilePath = null;
        private bool m_IsRunning = false;

        public void Run()
        {
            m_IsRunning = true;
            while(m_IsRunning)
            {

            }
        }

        private bool RunCommand(string command)
        {
            string[] data = command.Split();

            if (data.Length == 0)
                return false;
            string primaryCommand = data[0];
            if(primaryCommand == SelectFileCommand)
            {

            }
            else if(primaryCommand == SetModeCommand)
            {

            }
            else if(primaryCommand == CompileCodeCommand)
            {

            }
            else if(primaryCommand == ExitCommand)
            {

            }

            if(data.Length > 1 && CompilerModeCommands.TryGetValue(data[0], out CompilerMode mode))
            {
                Console.WriteLine("Set compiler mode to: " + mode.ToString());
                m_CurrentMode = mode;
            }

            return false;
        }

        private string SelectFolderDlg()
        {

            // Show the FolderBrowserDialog.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            folderBrowserDialog.Description = "Select a folder";
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }
            // Cancel button was pressed.
            else if (result == DialogResult.Cancel)
                return string.Empty;
        }
    }
}
