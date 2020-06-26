using System;

namespace NDD.CentralSolucoes.Base.Estruturas
{
    /// <summary>
    ///  Extensão da classe Option para implementar métodos úteis na representação um objeto de retorno que pode ser nulo.
    /// </summary>
    public static class Option
    {
        /// <summary>
        ///  Método que retorna uma nova instancia de Option com base no paramêtro value
        /// </summary>
        /// <param name="value">É o resultado da operação</param>
        public static Option<T> Of<T>(T value) => new Option<T>(value, value != null);

        #region GetOrElse
        public static T GetOrElse<T>(this Option<T> @this, Func<T> fallback) =>
                @this.Match(
                    some: value => value,
                    none: fallback
                    );

        public static T GetOrElse<T>(this Option<T> @this, T @else) =>
                GetOrElse(@this, () => @else);
        #endregion

        #region OrElse
        public static Option<T> OrElse<T>(this Option<T> @this, Option<T> @else) =>
            @this.Match(
                some: _ => @this,
                none: () => @else
            );


        public static Option<T> OrElse<T>(this Option<T> @this, Func<Option<T>> fallback) =>
            @this.Match(
                some: _ => @this,
                none: fallback
            );
        #endregion

    }
}
