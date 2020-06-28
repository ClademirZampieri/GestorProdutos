using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestorProdutos.Sincronizacao.API.Behaviours;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using System;
using System.Reflection;
using GestorProdutos.Infra.Extensions;
using AutoMapper;
using GestorProdutos.Sincronizacao.API.Filters;

namespace GestorProdutos.Sincronizacao.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddSimpleInjector(this IServiceCollection services, Container container)
        {
            // Define que a instância vai existir dentro de um determinado escopo (implícito ou explícito).
            // Assume o fluxo de controle dos métodos assíncronos automaticamente
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));

            // Ao invocar o método de extensão UseSimpleInjectorAspNetRequestScoping, o tempo que uma 
            // requisição está ativa é o período que um escopo vai estar ativo. 
            services.UseSimpleInjectorAspNetRequestScoping(container);
            services.EnableSimpleInjectorCrossWiring(container);
        }

        public static void AddDependencies(this IServiceCollection services,
            Container container,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            //É necessário registrar para utilizar o i18n controller. Com o DI resolver padrão do asp.net core não é necessário fazer isso
            container.RegisterInstance<IHostingEnvironment>(hostingEnvironment);

            //NDD.Licenciamento.Infra.Data.Inicializacao.AdicionarDependencias(services, configuration, hostingEnvironment.EnvironmentName);
        }

        public static void AddMediator(this IServiceCollection services, Container container)
        {
            var assembly = AppDomain.CurrentDomain.Load("GestorProdutos.Application");

            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
            container.Register(typeof(IRequestHandler<,>), assembly);
            container.Register(typeof(IRequestHandler<>), assembly);

            var notificationHandlerTypes = container.GetTypesToRegister(typeof(INotificationHandler<>), assembly);
            container.Collection.Register(typeof(INotificationHandler<>), notificationHandlerTypes);

            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(ValidationPipeline<,>)
            });
        }

        public static void AddValidators(this IServiceCollection services, Container container)
        {
            container.Collection.Register(typeof(IValidator<>), typeof(GestorProdutos.Application.AppModule).GetTypeInfo().Assembly);
        }

        public static void AddFilters(this IServiceCollection services)
        {
            services.AddMvc(options => options.Filters.Add(new ExceptionHandlerAttribute()));
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.ConfigureProfiles(
                typeof(API.Startup),
                typeof(GestorProdutos.Application.AppModule)
                );
            services.AddSingleton<IMapper>(c => Mapper.Instance);
        }
    }
}
