using GestorProdutos.Application.Jobs;
using GestorProdutos.Application.Schedules;
using GestorProdutos.Negocio.RNA.Sincronizacao;
using GestorProdutos.Negocio.RNA.Sincronizacao.ProcessarDados;
using Microsoft.Extensions.DependencyInjection;

namespace GestorProdutos.Application
{
    public static class Inicializacao
    {
        public static IServiceCollection AdicionarDependenciasNoJobDeSincronizacaoDeProdutos(this IServiceCollection services)
        {
            services.AddTransient<ISincronizacaoProdutosJob, SincronizacaoProdutosJobImpl>();
            services.AddTransient<ScheduleSincronizacaoProdutos>();
            services.AddTransient<IProcessarDados, ProcessarDadosImpl>();

            return services;
        }
    }
}
