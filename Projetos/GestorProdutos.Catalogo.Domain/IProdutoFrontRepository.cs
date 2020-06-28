using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestorProdutos.Core.Data;

namespace GestorProdutos.Catalogo.Domain
{
    public interface IProdutoFrontRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> ObterTodos();
        Task<Produto> ObterPorId(Guid id);
        Task<IEnumerable<Produto>> ObterProdutosNaoSincronizados();


        void Adicionar(Produto produto);
        void Atualizar(Produto produto);
    }
}