using AutoMapper;
using GestorProdutos.Base.Exceptions;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Catalogo.Domain.Enums;
using Microsoft.Extensions.Logging;
using NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe;
using NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe.NFSeEmissao;
using System;

namespace GestorProdutos.Negocio.RNA.Sincronizacao.ProcessarDados
{
    public class ProcessarDadosImpl : IProcessarDados
    {
        private readonly IProdutoSincronizacaoRepository _produtoRepository;
        private readonly IApiSincronizacaoClient _apiSincronizacaoClient;
        private readonly IMapper _mapeamento;
        private readonly ILogger<ProcessarDadosImpl> _logger;

        public ProcessarDadosImpl(IProdutoSincronizacaoRepository produtoRepository, IApiSincronizacaoClient apiSincronizacaoClient, IMapper mapeamento, ILogger<ProcessarDadosImpl> logger)
        {
            _produtoRepository = produtoRepository;
            _apiSincronizacaoClient = apiSincronizacaoClient;
            _mapeamento = mapeamento;
            _logger = logger;
        }

        public void Processar()
        {
            _logger.LogInformation($"Iniciando o processo de sincronização dos produtos");

            var produtosNaoSincronizados = _produtoRepository.ObterProdutosNaoSincronizados().Result;

            foreach (var produto in produtosNaoSincronizados)
            {
                _logger.LogInformation($"Sincronizando produto {produto.Id}");

                if (produto.StatusSincronizacao == StatusSincronizacaoEnum.PendenteDeCriacao)
                {
                    try
                    {
                        var command = _mapeamento.Map<Produto, CriarProdutoCommand>(produto);
                        var resposta = _apiSincronizacaoClient.CriarProdutoAsync(command);

                        if (resposta.Result.IsSuccess)
                        {
                            AtualizarStatusDoProdutoNoBanco(produto);
                        }
                        else throw new BusinessException(ErrorCodes.Unhandled, $"Retorno da API {resposta.Result.Failure?.Message}");
                    }
                    catch(Exception ex)
                    {
                        _logger.LogInformation($"Erro na chamada do método de criação de produto da API: {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        var command = _mapeamento.Map<Produto, AtualizarProdutoCommand>(produto);
                        var resposta = _apiSincronizacaoClient.AtualizarProdutoAsync(command);

                        if (resposta.Result.IsSuccess)
                            AtualizarStatusDoProdutoNoBanco(produto);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Erro na chamada do método de atualização de produto da API: {ex.Message}");
                    }
                }

                _logger.LogInformation($"Sincronização produto {produto.Id} finalizada com sucesso");
            }

            _logger.LogInformation($"Finalizando o processo de sincronização dos produtos");
        }

        private void AtualizarStatusDoProdutoNoBanco(Produto produto)
        {
            produto.StatusSincronizacao = StatusSincronizacaoEnum.Sincronizado;
            
            _produtoRepository.Atualizar(produto);

            _produtoRepository.UnitOfWork.Commit();
        }
    }
}
