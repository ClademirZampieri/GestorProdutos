using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Negocio.RNA.Sincronizacao.ProcessarDados;
using GestorProdutos.Sincronizacao.Produtos.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe.NFSeEmissao;
using NDD.Gerenciamento.Geral.Clients.Http;

namespace GestorProdutos.Sincronizacao.Host
{
    public static class DependencyInjection
    {
        public static void RegistrarServicos(this IServiceCollection services)
        {
            // Catalogo
            services.AddScoped<IHttpClient, StandardHttpClient>();
            services.AddScoped<IProcessarDados, ProcessarDadosImpl>();
            services.AddScoped<IApiSincronizacaoClient, ApiSincronizacaoClientImpl>();
            services.AddScoped<IProdutoSincronizacaoRepository, ProdutoSincronizacaoRepository>();
            services.AddScoped<ProdutoSincronizacaoRepository>();
        }
    }
}