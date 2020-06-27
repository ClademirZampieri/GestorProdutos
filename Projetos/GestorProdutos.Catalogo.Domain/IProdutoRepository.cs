using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestorProdutos.Core.Data;

namespace GestorProdutos.Catalogo.Domain
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> ObterTodos();
        Task<Produto> ObterPorId(Guid id);

        void Adicionar(Produto produto);
        void Atualizar(Produto produto);
    }
}