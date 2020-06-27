using GestorProdutos.Catalogo.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestorProdutos.Catalogo.Data.Extensions
{
    public static class EntityExtensions
    {
        public static void DetachLocal<T>(this DbContext context, T t, Guid entryId)
    where T : class, IIdentifier
        {
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
