using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    struct Either<TA, TB>
    {
        private readonly object Value;

        public bool IsOptionA => Value is TA;
        public bool IsOptionB => Value is TB;

        public TA AValue
        {
            get
            {
                if (IsOptionA)
                    return (TA)Value;
                throw new InvalidOperationException();
            }
        }

        public TB BValue
        {
            get
            {
                if (IsOptionB)
                    return (TB)Value;
                throw new InvalidOperationException();
            }
        }

        public Either(TA value)
        {
            Value = value;
        }

        public Either(TB value)
        {
            Value = value;
        }

        public static implicit operator Either<TA, TB>(TA a)
        {
            return new Either<TA, TB>(a);
        }

        public static implicit operator Either<TA, TB>(TB b)
        {
            return new Either<TA, TB>(b);
        }

        public void Match(Action<TA> aFunc, Action<TB> bFunc)
        {
            if (Value is TA a)
                aFunc(a);
            else if (Value is TB b)
                bFunc(b);
            else
                throw new ArgumentException("This should never be called, and if it is, you've done messed up.");
        }

        public TReturn Match<TReturn>(Func<TA, TReturn> aFunc, Func<TB, TReturn> bFunc)
        {
            if (Value is TA a)
                return aFunc(a);
            else if (Value is TB b)
                return bFunc(b);
            else
                throw new ArgumentException("This should never be called, and if it is, you've done messed up.");
        }
	}
}
