using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace NDD.MicroServico.Base.Schedulador.Quartz
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
