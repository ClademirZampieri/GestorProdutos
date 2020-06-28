using GestorProdutos.Base.Respostas;
using GestorProdutos.Negocio.ViewModels;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorProdutos.Negocio.Services
{
    public interface IProdutoAppService : IDisposable
    {
        Task<ProdutoViewModel> ObterPorId(Guid id);
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();

        Task<Result<Exception, RespostaDeRequisicao>> AdicionarProduto(ProdutoViewModel produtoViewModel);
        Task<Result<Exception, RespostaDeRequisicao>> AtualizarProduto(ProdutoViewModel produtoViewModel);

        Task<ProdutoViewModel> DebitarEstoque(Guid id, int quantidade);
        Task<ProdutoViewModel> ReporEstoque(Guid id, int quantidade);
    }
}