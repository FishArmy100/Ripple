using System;
using System.Collections.Generic;
using Ripple.Validation.Info;
using Ripple.Lexing;
using Ripple.Validation.Info.Lifetimes;

namespace Ripple.Validation
{
    public class LocalVariableStack
    {
        private readonly Stack<Dictionary<string, VariableInfo>> m_VariableStack = new Stack<Dictionary<string, VariableInfo>>();
        public LifetimeInfo CurrentLifetime => new LifetimeInfo(m_VariableStack.Count);

        public void PushScope()
        {
            m_VariableStack.Push(new Dictionary<string, VariableInfo>());
        }

        public void PopScope()
        {
            m_VariableStack.Pop();
        }

        public bool ContainsVariable(string name) => TryGetVariable(name, out _);

        public bool TryAddVariable(VariableInfo info)
        {
            return m_VariableStack.Peek().TryAdd(info.Name, info);
        }

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
