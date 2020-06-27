using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestorProdutos.Infra.Extensions
{
    public static class QueryableExtensions
    {

        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>().DecompileAsync().ToListAsync();
        }

    }
}
