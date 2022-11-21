using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ripple.Validation.AstInfo;

namespace Ripple.Validation
{
    class LocalVariableStack
    {
        private readonly Stack<Dictionary<string, VariableData>> m_VariableStack = new Stack<Dictionary<string, VariableData>>();

        public bool TryAddVariable(VariableData variableData)
        {
            if (!ContainsVariable(variableData.GetName()))
            {
                m_VariableStack.Peek().Add(variableData.GetName(), variableData);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            m_VariableStack.Clear();
        }

        public void PushScope()
        {
            m_VariableStack.Push(new Dictionary<string, VariableData>());
        }

        public void PopScope()
        {
            m_VariableStack.Pop();
        }

        public bool TryGetVariable(string name, out VariableData data)
        {
            data = null;

            foreach(var frame in m_VariableStack)
            {
                if (frame.TryGetValue(name, out data))
                    return true;
            }

            return false;
        }

        public bool ContainsVariable(string name)
        {
            foreach(var frame in m_VariableStack)
            {
                if (frame.ContainsKey(name))
                    return true;
            }

            return false;
        }
    }
}
