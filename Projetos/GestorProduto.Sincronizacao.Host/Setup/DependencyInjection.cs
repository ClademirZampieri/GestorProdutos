using GestorProdutos.Catalogo.Data;
using GestorProdutos.Catalogo.Data.Repository;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Negocio.Services;
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
            services.AddScoped<IApiSincronizacaoClient, ApiSincronizacaoClientImpl>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutoAppService, ProdutoAppService>();
            services.AddScoped<IEstoqueService, EstoqueService>();
            services.AddScoped<CatalogoContext>();
        }
    }
}