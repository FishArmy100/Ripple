using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    class NullOptionExeption : Exception { }

    public struct Option<T> where T : class
    {
        private readonly T m_Value;
        public Option(T value)
        {
            m_Value = value;
        }

        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }

        public T Value
        {
            get
            {
                if (m_Value == null)
                    return m_Value;
                else
                    throw new NullOptionExeption();
            }
        }

        public bool HasValue() => m_Value != null;

        public void Match(Action<T> okFunc, Action failFunc)
        {
            if (HasValue())
                okFunc(Value);
            else
                failFunc();
        }

        public void Match(Action<T> okFunc)
        {
            if (HasValue())
                okFunc(Value);
        }
    }
}
