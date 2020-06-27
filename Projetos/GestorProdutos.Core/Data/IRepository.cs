using System;
using GestorProdutos.Core.DomainObjects;

namespace GestorProdutos.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}