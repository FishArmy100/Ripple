using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    struct FolderData
    {
        public readonly List<(string, string)> Files;
        public readonly List<FolderData> Folders;

        public FolderData(List<(string, string)> files, List<FolderData> folders)
        {
            Files = files;
            Folders = folders;
        }
    }
}
