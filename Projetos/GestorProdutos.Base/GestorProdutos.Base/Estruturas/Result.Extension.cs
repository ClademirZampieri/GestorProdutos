using System;
using System.Collections.Generic;
using System.Linq;

namespace GestorProdutos.Base.Estruturas
{
    using static Helpers;

    public static class Result
    {

        /*public static Result<Exception, TSuccess> Run<TSuccess>(this Func<TSuccess> func)
        {
            Result
            {
                return func();
            }
            catch (Exception e)
            {
                return e;
            }
        }

         public static Result<Exception, Unit> Run(this Action action) => Run(ToFunc(action));

         public static Result<Exception, TSuccess> Run<TSuccess>(this Exception ex) => ex;

         public static Result<TFailure, TSuccess> Bind<TFailure, TSuccess>(this Result<TFailure, TSuccess> input,
             Func<TSuccess, Result<TFailure, TSuccess>> switchFunction)
         {
             if (input.IsSuccess)
                 return switchFunction(input.Success);
             return input;
         }*/


        #region Of
        public static Result<IEnumerable<TFailure>, Func<TA, Result<IEnumerable<TFailure>, TSuccess>>> Of
            <TA, TFailure, TSuccess>(
                Func<TA, Result<IEnumerable<TFailure>, TSuccess>> func
            ) => func;

        public static Result<IEnumerable<TFailure>, Func<TA, TB, Result<IEnumerable<TFailure>, TSuccess>>> Of
            <TA, TB, TFailure, TSuccess>(
                Func<TA, TB, Result<IEnumerable<TFailure>, TSuccess>> func
            ) => func;

        public static Result<IEnumerable<TFailure>, Func<TA, TB, TC, Result<IEnumerable<TFailure>, TSuccess>>> Of
            <TA, TB, TC, TFailure, TSuccess>(
                Func<TA, TB, TC, Result<IEnumerable<TFailure>, TSuccess>> func
            ) => func;
        #endregion

        #region Lift

        public static Result<TFailure, TSuccess> Lift<TFailure, TSuccess>(
            this Result<TFailure, Result<TFailure, TSuccess>> @Result
        ) => @Result.Match(
            failure: f => f,
            success: s => s
        );
        #endregion

        #region Apply
        public static Result<TFailure, Func<TB, TResult>> Apply<TFailure, TA, TB, TResult>(
            this Result<TFailure, Func<TA, TB, TResult>> func, Result<TFailure, TA> arg
        )
        {
            return arg.Match(
                failure: e => e,
                success: a => func.Match(
                    failure: e2 => e2,
                    success: f => Result<TFailure, Func<TB, TResult>>.Of(b => f(a, b))
                )
            );
        }

        public static Result<TFailure, TResult> Apply<TFailure, TA, TResult>(
            this Result<TFailure, Func<TA, TResult>> func, Result<TFailure, TA> arg
        ) => arg.Match(
            failure: e => e,
            success: a => func.Match(
                failure: e2 => e2,
                success: f => Result<TFailure, TResult>.Of(f(a))
            )
        );

        public static Result<IEnumerable<TFailure>, Func<TB, TResult>> Apply<TFailure, TA, TB, TResult>(
            this Result<IEnumerable<TFailure>, Func<TA, TB, TResult>> func, Result<TFailure, TA> arg
        ) => arg.Match(
            failure: e => Result<IEnumerable<TFailure>, Func<TB, TResult>>.Of(
                func.OptionalFailure.GetOrElse(Enumerable.Empty<TFailure>).Concat(new[] { e })
            ),
            success: a => func.Match(
                failure: Result<IEnumerable<TFailure>, Func<TB, TResult>>.Of,
                success: f => Result<IEnumerable<TFailure>, Func<TB, TResult>>.Of(b => f(a, b))
            )
        );


        public static Result<IEnumerable<TFailure>, TResult> Apply<TFailure, TA, TResult>(
            this Result<IEnumerable<TFailure>, Func<TA, TResult>> func, Result<TFailure, TA> arg
        ) => arg.Match(
                failure: e => Result<IEnumerable<TFailure>, TResult>.Of(
                    func.OptionalFailure.GetOrElse(Enumerable.Empty<TFailure>).Concat(new[] { e })
                ),
                success: a => func.Match(
                    failure: Result<IEnumerable<TFailure>, TResult>.Of,
                    success: f => Result<IEnumerable<TFailure>, TResult>.Of(f(a))
                )
            );
        #endregion

        #region Map
        public static Result<TFailure, NewTSuccess> Map<TFailure, TSuccess, NewTSuccess>(
                this Result<TFailure, TSuccess> @Result,
                Func<TSuccess, NewTSuccess> func
            )
            => @Result.IsSuccess
                ? func(@Result.Success)
                : Result<TFailure, NewTSuccess>.Of(@Result.Failure);

        public static Result<TFailure, Func<TB, NewTSuccess>> Map<TFailure, TSuccess, TB, NewTSuccess>(
            this Result<TFailure, TSuccess> @this,
            Func<TSuccess, TB, NewTSuccess> func
        ) => @this.Map(func.Curry());
        #endregion

        #region Bind
        public static Result<TFailure, NewTSuccess> Bind<TFailure, TSuccess, NewTSuccess>(
            this Result<TFailure, TSuccess> @Result,
            Func<TSuccess, Result<TFailure, NewTSuccess>> func
        )
            => @Result.IsSuccess
                ? func(@Result.Success)
                : Result<TFailure, NewTSuccess>.Of(@Result.Failure);

        public static Either<TFailure, TSuccess> ToEither<TFailure, TSuccess>(
            this Result<TFailure, TSuccess> @Result
        ) => @Result.Match<Either<TFailure, TSuccess>>(
            failure: f => f,
            success: s => s
        );
        #endregion

        #region Run
        public static Result<Exception, TSuccess> Run<TSuccess>(this Func<TSuccess> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Result<Exception, Unit> Run(this Action action) => Run(ToFunc(action));

        #endregion

        public static Result<TFailure, TSuccess> Flatten<TFailure, TSuccess>(
            this Result<TFailure, Option<TSuccess>> @Result,
            Func<TFailure> ifNone
        )
        {
            if (@Result.IsFailure)
            {
                return @Result.Failure;
            }
            return @Result.Success.IsSome
                ? @Result.Success.Value
                : (Result<TFailure, TSuccess>)ifNone();
        }

        public static Result<TFailure, TSuccess> IfNone<TFailure, TSuccess>(
            this Result<TFailure, Option<TSuccess>> @Result,
            Func<TFailure> ifNone
        ) => @Result.Flatten(ifNone);



    }
}
