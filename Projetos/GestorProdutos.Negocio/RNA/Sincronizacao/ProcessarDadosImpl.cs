using AutoMapper;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Catalogo.Domain.Enums;
using NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe;
using NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe.NFSeEmissao;

namespace GestorProdutos.Negocio.RNA.Sincronizacao.ProcessarDados
{
    public class ProcessarDadosImpl : IProcessarDados
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IApiSincronizacaoClient _apiSincronizacaoClient;
        private readonly IMapper _mapeamento;

        public ProcessarDadosImpl(IProdutoRepository produtoRepository, IApiSincronizacaoClient apiSincronizacaoClient, IMapper mapeamento)
        {
            _produtoRepository = produtoRepository;
            _apiSincronizacaoClient = apiSincronizacaoClient;
            _mapeamento = mapeamento;
        }

        public void Processar()
        {
            var produtosNaoSincronizados = _produtoRepository.ObterProdutosNaoSincronizados().Result;

            foreach (var produto in produtosNaoSincronizados)
            {
                if (produto.StatusSincronizacao == StatusSincronizacaoEnum.PendenteDeCriacao)
                {
                    var command = _mapeamento.Map<Produto, CriarProdutoCommand>(produto);
                    var resposta = _apiSincronizacaoClient.CriarProdutoAsync(command);

                    if (resposta.Result.IsSuccess)
                        AtualizarStatusDoProdutoNoBanco(produto);
                }
                else
                {
                    var command = _mapeamento.Map<Produto, AtualizarProdutoCommand>(produto);
                    var resposta = _apiSincronizacaoClient.AtualizarProdutoAsync(command);

                    if (resposta.Result.IsSuccess)
                        AtualizarStatusDoProdutoNoBanco(produto);
                }
            }
        }

        private void AtualizarStatusDoProdutoNoBanco(Produto produto)
        {
            produto.StatusSincronizacao = StatusSincronizacaoEnum.Sincronizacado;
            _produtoRepository.Atualizar(produto);
        }
    }
}
