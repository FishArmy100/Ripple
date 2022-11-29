using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Info;

namespace Ripple.Validation
{
    class LocalVariableStack
    {
        private readonly Stack<Dictionary<string, VariableInfo>> m_VariableStack = new Stack<Dictionary<string, VariableInfo>>();

        public void PushScope()
        {
            m_VariableStack.Push(new Dictionary<string, VariableInfo>());
        }

        public void PopScope()
        {
            m_VariableStack.Pop();
        }

        public bool AddVariable(VariableInfo info)
        {
            return m_VariableStack.Peek().TryAdd(info.Name, info);
        }

        public bool ContainsVariable(string name) => TryGetVariable(name, out _);

        public bool TryGetVariable(string name, out VariableInfo info)
        {
            foreach(var dict in m_VariableStack)
            {
                if (dict.TryGetValue(name, out info))
                    return true;
            }

            info = null;
            return false;
        }
    }
}
