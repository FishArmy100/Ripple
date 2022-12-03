using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    class NullOptionExeption : Exception { }

    public struct Option<T>
    {
        private readonly T m_Value;
        private readonly bool m_HasValue;

        public Option(T value)
        {
            if(value.GetType().IsValueType)
            {
                m_Value = value;
                m_HasValue = true;
            }
            else
            {
                m_Value = value;
                m_HasValue = value != null;
            }
        }

        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }

        public static explicit operator T(Option<T> option)
        {
            return option.Value;
        }

        public T Value
        {
            get
            {
                if (HasValue())
                    return m_Value;
                else
                    throw new NullOptionExeption();
            }
        }

        public bool HasValue() => m_HasValue;

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

        public TReturn Match<TReturn>(Func<T, TReturn> okFunc, Func<TReturn> failFunc)
        {
            if (HasValue())
                return okFunc(Value);
            else
                return failFunc();
        }
    }
}
