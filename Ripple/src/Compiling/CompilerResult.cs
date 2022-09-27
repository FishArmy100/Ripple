using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Compiling
{
    abstract class CompilerResult<TSuccess>
    {
        public class Ok : CompilerResult<TSuccess>
        {
            public readonly TSuccess Data;

            public Ok(TSuccess data)
            {
                Data = data;
            }
        }

        public class Fail : CompilerResult<TSuccess>
        {
            public readonly List<CompilerError> Errors;

            public Fail(List<CompilerError> errors)
            {
                Errors = errors;
            }
        }

        public CompilerResult<TOut> Match<TOut>(Func<TSuccess, CompilerResult<TOut>> okFunc)
        {
            return this switch
            {
                Ok ok => okFunc(ok.Data),
                Fail fail => new CompilerResult<TOut>.Fail(fail.Errors),
                _ => throw new ArgumentException("Result has a different result than ok, or fail")
            };
        }

        public void Match(Action<TSuccess> okFunc, Action<List<CompilerError>> failFunc)
        {
            switch(this)
            {
                case Ok ok:
                    okFunc(ok.Data);
                    break;
                case Fail fail:
                    failFunc(fail.Errors);
                    break;
                default:
                    throw new ArgumentException("Result has a different result than ok, or fail");
            };
        }
    }
}
