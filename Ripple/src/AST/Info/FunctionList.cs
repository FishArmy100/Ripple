using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;

namespace Ripple.AST.Info
{
    class FunctionList
    {
        private readonly Dictionary<string, List<FunctionInfo>> m_Functions = new Dictionary<string, List<FunctionInfo>>();

        public bool TryAddFunction(FunctionInfo function)
        {
            if (ContainsFunction(function))
                return false;

            string funcName = function.Name.Text;
            if (m_Functions.TryGetValue(funcName, out var functionOverloads))
            {
                functionOverloads.Add(function);
            }
            else
            {
                List<FunctionInfo> newOverloads = new List<FunctionInfo>() { function };
                m_Functions.Add(funcName, newOverloads);
            }

            return true;
        }

        public bool ContainsFunctionWithName(string name)
        {
            return m_Functions.ContainsKey(name);
        }

        public bool TryGetFunction(string name, List<string> parameterTypes, out FunctionInfo function)
        {
            if (m_Functions.TryGetValue(name, out var functionOverloads))
            {
                function = functionOverloads.FirstOrDefault(fn =>
                {
                    return fn.Parameters
                    .ConvertAll(p => p.Item1.Text)
                    .SequenceEqual(parameterTypes);
                });

                return function != null;
            }

            function = null;
            return false;
        }

        public bool ContainsFunction(string name, List<string> parameterTypes)
        {
            return TryGetFunction(name, parameterTypes, out _);
        }

        public bool ContainsFunction(FunctionInfo function)
        {
            string funcName = function.Name.Text;
            if (m_Functions.TryGetValue(funcName, out var functionOverloads))
            {
                return functionOverloads
                    .Any(fn => fn.Parameters.SequenceEquals(function.Parameters, (a, b) =>
                    {
                        return a.Item1.Text == b.Item1.Text;
                    }));
            }

            return false;
        }
    }
}
