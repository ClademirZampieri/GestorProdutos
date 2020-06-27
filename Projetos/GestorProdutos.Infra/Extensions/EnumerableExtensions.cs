using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestorProdutos.Infra.Extensions
{
    public static class EnumerableExtensions
    {

        public static Result<Exception, IQueryable<TSuccess>> AsResult<TSuccess>(this IEnumerable<TSuccess> source)
        {
            return Result.Run(() => source.AsQueryable());
        }

    }
}
