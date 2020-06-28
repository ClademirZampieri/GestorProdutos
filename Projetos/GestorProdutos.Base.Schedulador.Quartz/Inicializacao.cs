using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace GestorProdutos.Base.Schedulador.Quartz
{
    public static class Inicializacao
    {
        public static IServiceCollection AdicionarScheduladorQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IScheduladorProcessos<IScheduler>, ScheduladorProcessosImpl>();
            return services;
        }
    }
}
