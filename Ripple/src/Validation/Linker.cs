using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.Info;
using Ripple.Validation.Info.Types;
using Ripple.Utils;
using Raucse;
using Ripple.Validation.Info.Functions;

namespace Ripple.Validation
{
    class Linker
    {
        private readonly List<Pair<FunctionInfo, string>> m_ExternalFunctions;

        public Linker(List<Pair<FunctionInfo, string>> externalFunctions)
        {
            m_ExternalFunctions = externalFunctions;
        }

        public Option<string> TryGetHeader(FunctionInfo info)
        {
            foreach(var (other, file) in m_ExternalFunctions)
            {
                if (other.Equals(info))
                    return file;
            }

            return new Option<string>();
        }

        public static Option<Linker> FromExternals(List<Pair<List<FunctionInfo>, string>> externals)
        {
            var funcs = externals
                .Select(p => p.First.Select(f => new Pair<FunctionInfo, string>(f, p.Second)))
                .SelectMany(fs => fs);

            List<Pair<FunctionInfo, string>> dictionary = new List<Pair<FunctionInfo, string>>();
            foreach(var (info, file) in funcs)
            {
                dictionary.Add(new Pair<FunctionInfo, string>(info, file));
            }

            return new Linker(dictionary);
        }
    }
}
