using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Utils
{
    public class Result<TSuccess, TError>
    {
        private readonly Either<TSuccess, TError> m_Value;

        public bool IsError() => m_Value.IsOptionB;
        public bool IsOk() => m_Value.IsOptionA;

        public TSuccess Value => m_Value.AValue;
        public TError Error => m_Value.BValue;

        public Result(TSuccess success)
        {
            m_Value = success;
        }

        public Result(TError error)
        {
            m_Value = error;
        }

        public static implicit operator Result<TSuccess, TError>(TSuccess success)
        {
            return new Result<TSuccess, TError>(success);
        }

        public static implicit operator Result<TSuccess, TError>(TError error)
        {
            return new Result<TSuccess, TError>(error);
        }

        public void Match(Action<TSuccess> okFunc, Action<TError> failFunc)
        {
            m_Value.Match(okFunc, failFunc);
        }

        public TReturn Match<TReturn>(Func<TSuccess, TReturn> okFunc, Func<TError, TReturn> failFunc)
        {
            return m_Value.Match(okFunc, failFunc);
        }

        public Option<TSuccess> ToOption()
        {
            return Match(ok => new Option<TSuccess>(ok), fail => new Option<TSuccess>());
        }

        public Option<TError> GetErrorOption()
        {
            return Match(ok => new Option<TError>(), fail => new Option<TError>(fail));
        }
    }

    public static class ResultExtensions
	{

        public static Result<List<TSuccess>, List<TError>> AggregateResults<TSuccess, TError>(this IEnumerable<Result<TSuccess, List<TError>>> self)
		{
            List<TSuccess> successes = new List<TSuccess>();
            List<TError> errors = new List<TError>();
            foreach (var result in self)
            {
                result.Match(
                    ok => successes.Add(ok),
                    fail => errors.AddRange(fail));
            }

            if (errors.Any())
                return errors;

            return successes;
        }
    }
}
