using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.AST.Info;
using Ripple.AST;
using Ripple.Lexing;

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

        public void AddVariable(VarDecl varDecl, Action<VariableInfo> onFailed)
        {
            TypeInfo type = TypeInfo.FromASTType(varDecl.Type);
            foreach (Token name in varDecl.VarNames)
            {
                VariableInfo info = new VariableInfo(name, type, varDecl.UnsafeToken.HasValue);
                if (!AddVariable(info))
                    onFailed(info);
            }
        }

        public void AddVariable(VarDecl varDecl)
        {
            TypeInfo type = TypeInfo.FromASTType(varDecl.Type);
            foreach (Token name in varDecl.VarNames)
            {
                VariableInfo info = new VariableInfo(name, type, varDecl.UnsafeToken.HasValue);
                AddVariable(info);
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
