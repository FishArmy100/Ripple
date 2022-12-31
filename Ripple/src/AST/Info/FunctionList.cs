using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Utils.Extensions;
using Ripple.Utils;

namespace Ripple.AST.Info
{
    class FunctionList
    {
        private readonly Dictionary<string, List<FunctionInfo>> m_Functions = new Dictionary<string, List<FunctionInfo>>();

        public bool TryAddFunction(FunctionInfo function)
        {
            if (ContainsFunction(function))
                return false;

            string funcName = function.NameToken.Text;
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

        public List<string> GetFunctionNames() => m_Functions.Keys.ToList();

        public Option<FunctionInfo> TryGetFunction(string name, List<TypeInfo> parameterTypes)
        {
            Option<FunctionInfo> function = new Option<FunctionInfo>();
            if (m_Functions.TryGetValue(name, out var functionOverloads))
            {
                function = functionOverloads.FirstOrDefault(fn =>
                {
                    List<TypeInfo> funcParams = fn.Parameters.ConvertAll(p => p.Type);

                    if (funcParams.Count != parameterTypes.Count)
                        return false;

                    for(int i = 0; i < funcParams.Count; i++)
                    {
                        if (!funcParams[i].EqualsWithoutLifetimes(parameterTypes[i]))
                            return false;
                    }

                    return true;
                });
            }

            return function;
        }

        public List<FunctionInfo> GetOverloadsWithName(string name)
        {
            return m_Functions.GetOrCreate(name);
        }

        public bool ContainsFunction(string name, List<TypeInfo> parameterTypes) => TryGetFunction(name, parameterTypes).HasValue();
        public bool ContainsFunction(FunctionInfo function) => ContainsFunction(function.Name, function.Parameters.ConvertAll(p => p.Type));
    }
}
