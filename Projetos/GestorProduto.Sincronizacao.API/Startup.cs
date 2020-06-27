using FluentValidation.AspNetCore;
using GestorProdutos.Base.Enums;
using GestorProdutos.Catalogo.Domain.Configuracoes;
using GestorProduto.Sincronizacao.API.Extensions;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleInjector;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GestorProdutos.Core.DomainObjects;
using GestorProduto.Sincronizacao.API.Filters;
using NDD.GestorProdutos.Migracoes;

namespace GestorProduto.Sincronizacao.API
{
    public class Startup
    {
        private static readonly string SecaoRecursosHabilitaveis = "HabilitarRecurso";
        private static readonly string VariavelAmbienteAmbienteProducao = "GESTORPRODUTO_AMBIENTE_PRODUCAO";
        public static readonly Container Container = new Container();

        private readonly GestorProdutosConfiguracoes _configuracoes;

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        private readonly string _caminhoArquivoLog;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            _configuracoes = new GestorProdutosConfiguracoes();
            configuration.Bind(_configuracoes);

            //configuration.AdicionarConfiguracoesRepositorioLicenciamento();
            //configuration.PegarConfiguracoesParaProcessamentoDeLicenca();

            _caminhoArquivoLog = configuration["CaminhoArquivoLog"];

            Container.Options.AllowOverridingRegistrations = true;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            //services.AdicionarProvedorServicoMSDI();

            services.AddCors();
            services.AddSimpleInjector(Container);
            //services.AdicionarDependenciasNoJobRenovacaoLicencas();
            //services.AdicionarProcessamentoDeLicencas();
            //services.AdicionarDependenciasRepositorioslicenciamentoMSSql();

            InicializarRecursosHabilitaveis(services);
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

            services.AddScoped(sp =>
                sp.GetService<IOptionsSnapshot<GestorProdutosConfiguracoes>>().Value);

            //services.AdicionarMigracoes(_configuracoes.AppSettings.ConnectionString, DbProviderEnum.SqlServer);
        }


        private void InicializarRecursosHabilitaveis(IServiceCollection services)
        {
            //var secaoRecursosHabilitaveis = Configuration.GetSection(SecaoRecursosHabilitaveis);
            //var recursos = secaoRecursosHabilitaveis
            //    .AsEnumerable()
            //    .Select(item =>
            //        new KeyValuePair<string, bool>
            //        (
            //            item.Key,
            //            string.IsNullOrEmpty(item.Value) ? false : bool.Parse(item.Value)
            //        ));
            //services.AdicionarServicosDeHabilitarRecurso(recursos);
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

            // Captura o valor da variavel de ambiente(GESTORPRODUTO_AMBIENTE_PRODUCAO)
            string CentralDeSolucoesAmbienteDeProducao = Environment.GetEnvironmentVariable(VariavelAmbienteAmbienteProducao);

            // Verifica se o ambiente não é de Produção
            if (Convert.ToBoolean(CentralDeSolucoesAmbienteDeProducao) == false)
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
