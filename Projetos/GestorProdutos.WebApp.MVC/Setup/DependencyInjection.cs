using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Core.Communication.Mediator;
using GestorProdutos.Core.Messages.CommonMessages.Notifications;
using GestorProdutos.Front.Produtos.Data.Extensions;
using GestorProdutos.Front.Produtos.Data.Repository;
using GestorProdutos.Negocio.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GestorProdutos.WebApp.MVC.Setup
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
            services.AddScoped<IProdutoFrontRepository, ProdutoFrontRepository>();
            services.AddScoped<IProdutoAppService, ProdutoAppService>();
            services.AddScoped<IEstoqueService, EstoqueService>();
            services.AddScoped<ProdutosFrontContext>();
        }
    }
}