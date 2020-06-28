using System;

namespace GestorProdutos.Base.Estruturas
{
    public static partial class LinqExtensions
    {
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> @this,
            Func<TRight, TResult> func
        ) => @this.Map(func);
    }
}
