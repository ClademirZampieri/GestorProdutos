using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using GestorProdutos.Infra.Extensions;

namespace GestorProdutos.Sincronizacao.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.ConfigureProfiles(
                typeof(Application.AppModule)
                );
            services.AddSingleton<IMapper>(c => Mapper.Instance);
        }

    }
}
