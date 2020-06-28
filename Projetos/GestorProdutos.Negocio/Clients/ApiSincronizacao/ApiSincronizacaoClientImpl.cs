using GestorProdutos.Infra.Configuracoes;
using Microsoft.Extensions.Options;
using NDD.CentralSolucoes.Base.Estruturas;
using NDD.Gerenciamento.Geral.Clients.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe.NFSeEmissao
{
    public class ApiSincronizacaoClientImpl : IApiSincronizacaoClient
    {
        private readonly IHttpClient _httpClient;

        private readonly string _urlApiSincronizacao;

        public ApiSincronizacaoClientImpl(
            IHttpClient httpClient,
            IOptions<GestorProdutosConfiguracoes> settings)
        {
            _httpClient = httpClient;
            _urlApiSincronizacao = $"{settings.Value.ParametrosDeSincronizacao.UrlWebAPI}/api/GestorProdutos";
        }

        public async Task<Result<Exception, Unit>> AtualizarProdutoAsync(AtualizarProdutoCommand command)
        {
            var url = $"{_urlApiSincronizacao}/AtualizarProduto";
            var response = await _httpClient.SendAsync(HttpMethod.Put, url, command);
            return response.ProcessResultadoApi();
        }

        public async Task<Result<Exception, Unit>> CriarProdutoAsync(CriarProdutoCommand command)
        {
            var url = $"{_urlApiSincronizacao}/CriarProduto";
            var response = await _httpClient.SendAsync(HttpMethod.Put, url, command);
            return response.ProcessResultadoApi();
        }
    }
}
