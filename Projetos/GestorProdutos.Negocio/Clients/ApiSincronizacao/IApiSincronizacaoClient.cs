using GestorProdutos.Base.Estruturas;
using System;
using System.Threading.Tasks;

namespace NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe.NFSeEmissao
{
    public interface IApiSincronizacaoClient
    {
        Task<Result<Exception, Unit>> CriarProdutoAsync(CriarProdutoCommand command);

        Task<Result<Exception, Unit>> AtualizarProdutoAsync(AtualizarProdutoCommand command);
    }
}
