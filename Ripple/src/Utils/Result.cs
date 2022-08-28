using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    public abstract class Result<TSuccess, TError>
    {
        public class Ok : Result<TSuccess, TError>
        {
            public TSuccess Data { get; }
            public Ok(TSuccess data) => Data = data;
        }

        public class Fail : Result<TSuccess, TError>
        {
            public TError Error { get; }
            public Fail(TError error) => Error = error;
        }

        //Convenience methods
        public static Result<TSuccess, TError> Good(TSuccess data) => new Ok(data);
        public static Result<TSuccess, TError> Bad(TError error) => new Fail(error);
    }
}
