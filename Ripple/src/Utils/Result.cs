﻿using System;
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

        public void Match(Action<TSuccess> okFunc, Action<TError> failFunc)
        {
            if (this is Ok ok)
                okFunc(ok.Data);
            else if (this is Fail fail)
                failFunc(fail.Error);
        }
    }
}
