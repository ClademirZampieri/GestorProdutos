using GestorProdutos.Catalogo.Data;
using GestorProdutos.Catalogo.Data.Repository;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Core.Communication.Mediator;
using GestorProdutos.Core.Messages.CommonMessages.Notifications;
using GestorProdutos.Negocio.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GestorProdutos.Sincronizacao.API
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Notifications
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Catalogo
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutoAppService, ProdutoAppService>();
            services.AddScoped<IEstoqueService, EstoqueService>();
            services.AddScoped<CatalogoContext>();
        }
    }
}