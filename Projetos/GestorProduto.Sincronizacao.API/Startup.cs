using FluentValidation.AspNetCore;
using GestorProdutos.Base.Enums;
using GestorProdutos.Core.DomainObjects;
using GestorProdutos.Front.Produtos.Data.Extensions;
using GestorProdutos.Infra.Configuracoes;
using GestorProdutos.Sincronizacao.API.Extensions;
using GestorProdutos.Sincronizacao.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NDD.GestorProdutos.Migracoes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleInjector;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GestorProdutos.Sincronizacao.API
{
    public class Startup
    {
        private static readonly string VariavelAmbienteAmbienteProducao = "GESTOR_AMBIENTE_PRODUCAO";
        public static readonly Container Container = new Container();

        private readonly GestorProdutosConfiguracoes _configuracoes;

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            _configuracoes = new GestorProdutosConfiguracoes();
            configuration.Bind(_configuracoes);

            Container.Options.AllowOverridingRegistrations = true;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddCors();
            services.AddSimpleInjector(Container);
            services.AddDependencies(Container, Configuration, HostingEnvironment);
            services.AddMediator(Container);
            services.AddValidators(Container);
            services.AddFilters();
            services.AddMvc()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<GestorProdutos.Application.AppModule>())
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API Sincronização Produtos",
                    Description = "Api de sincronização dos produtos",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Clademir",
                        Email = "mr.zampieri@hotmail.com",
                        Url = "http://intranet/paginas/Page.aspx"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });
                // Adiciona os comentários da API no Swagger JSON da UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
                // Adiciona os comentarios dos comandos no Swagger Json da Aplicacao
                xmlFile = $"{typeof(GestorProdutos.Application.AppModule).Assembly.GetName().Name}.xml";
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
                // Adiciona os comentarios dos comandos no Swagger Json da Dominio
                xmlFile = $"{typeof(Entity).Assembly.GetName().Name}.xml";
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
                c.DocumentFilter<OcultarDocumentacaoSwagger>();
            });

            services.Configure<GestorProdutosConfiguracoes>(Configuration);

            services.AddDbContext<ProdutosFrontContext>(options => options.UseSqlServer(_configuracoes.AppSettings.ConnectionString));

            services.AddMediatR(typeof(Startup));

            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<GestorProdutosConfiguracoes>>().Value);

            services.RegisterServices();

            services.AddTransient<GestorProdutosConfiguracoes>(c => new GestorProdutosConfiguracoes());
            services.AddTransient<GestorProdutosConfiguracoes>(c => _configuracoes);

            services.AdicionarMigracoes(_configuracoes.AppSettings.ConnectionString, DbProviderEnum.SqlServer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.InicializarMigracoes();

            // Habilitanto o uso de autenticação
            app.UseAuthentication();

            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            Container.RegisterMvcControllers(app);
            Container.AutoCrossWireAspNetComponents(app);

            // Captura o valor da variavel de ambiente(GestorProdutos_AMBIENTE_PRODUCAO)
            string prod = Environment.GetEnvironmentVariable(VariavelAmbienteAmbienteProducao);

            // Verifica se o ambiente não é de Produção
            if (Convert.ToBoolean(prod) == false)
            {
                // Habilita o Middleware do Swagger.
                app.UseSwagger();

                // Configura o Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Produtos V1");
                    c.RoutePrefix = string.Empty;
                });
            }
            app.UseMvc();
        }

        private IConfiguration CriarConfiguracaoBaseadoEmUmaSecao(string secao, params string[] chavesComuns)
        {
            Dictionary<string, string> chaves = new Dictionary<string, string>();

            if (chavesComuns != null && chavesComuns.Length > 0)
                Array.ForEach(chavesComuns, (item) => chaves.Add(item, Configuration[item]));

            IConfigurationSection secaoDesejada = Configuration.GetSection(secao);

            if (secaoDesejada == null)
                throw new ApplicationException($"Faltou informar a secao: {secao} no arquivo json de configuracao");

            string prefixo = secaoDesejada.Path + ":";

            foreach (KeyValuePair<string, string> item in secaoDesejada.AsEnumerable())
                chaves.Add(item.Key.Replace(prefixo, string.Empty), item.Value);

            return new ConfigurationBuilder().AddInMemoryCollection(chaves).Build();
        }
    }
}
