using System;
using System.Collections.Generic;
using Ripple.AST.Info;
using Ripple.Lexing;

namespace Ripple.AST.Utils
{
    class LocalVariableStack
    {
        private readonly Stack<Dictionary<string, VariableInfo>> m_VariableStack = new Stack<Dictionary<string, VariableInfo>>();
        public int ScopeCount => m_VariableStack.Count;

        public void PushScope()
        {
            m_VariableStack.Push(new Dictionary<string, VariableInfo>());
        }

        public void PopScope()
        {
            m_VariableStack.Pop();
        }

        public void AddVariables(List<Token> names, TypeInfo type, bool isUnsafe, Action<Token> onFailed)
        {
            foreach (Token name in names)
                AddVariable(name, type, isUnsafe, onFailed);
        }

        public void AddVariable(Token name, TypeInfo type, bool isUnsafe, Action<Token> onFailed)
        {
            VariableInfo info = new VariableInfo(name, type, isUnsafe, ScopeCount);
            if (m_VariableStack.Peek().ContainsKey(info.Name))
            {
                onFailed(name);
            }
            else
            {
                m_VariableStack.Peek().Add(info.Name, info);
            }
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
