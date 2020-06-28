using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NDD.CentralSolucoes.Base.NSB;
using NDD.Licenciamento.Application;
using NDD.Licenciamento.Application.Schedules;
using NDD.Licenciamento.Domain.Configuracoes;
using NDD.Licenciamento.Host.Extensions;
using NDD.Licenciamento.Migracoes;
using NDD.Licenciamento.Negocio;
using NDD.Licenciamento.Repositorio.MSSql;
using NDD.MicroServico.Base;
using NDD.MicroServico.Base.Cache;
using NDD.MicroServico.Base.Filas;
using NDD.MicroServico.Base.Hosting;
using NDD.MicroServico.Base.IO;
using NDD.MicroServico.Base.Log4Net;
using NDD.MicroServico.Base.ProvedorServico.MSDI;
using NDD.MicroServico.Base.Repositorios;
using NDD.MicroServico.Base.Schedulador;
using NDD.MicroServico.Base.Schedulador.Quartz;
using NServiceBus;
using NServiceBus.Logging;
using Quartz;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using GestorProdutos.Infra.Configuracoes;
using GestorProdutos.Base.Enums;

namespace GestorProdutos.Sincronizacao.Host
{
    public class Startup : IStartup
    {
        public const string EnvironmentTest = "Test";
        private static readonly string SecaoRecursosHabilitaveis = "HabilitarRecurso";
        private static readonly string SecaoCertificadoDigital = "CertificadoDigital";
        private static readonly object lockObject = new object();

        private readonly LicenciamentoConfiguracoes _configuracoes;
        private IMiddleware _middleware;
        private IMiddlewareFileWatcher _middlewaWatcher;
        private IServiceCollection _services;

        private IScheduladorProcessos<IScheduler> _scheduler;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            _configuracoes = new LicenciamentoConfiguracoes();
            configuration.Bind(_configuracoes);

            configuration.PegarConfiguracoesParaProcessamentoDeLicenca();
            configuration.AdicionarConfiguracoesRepositorioLicenciamento();
            configuration.AdicionarConfiguracoesNoJobRenovacaoLicencas();
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void Finalizacao(IHost host)
        {
            _middleware?.Finalizar();
            _scheduler?.Finalizar();
            _middlewaWatcher?.Finalizar();
        }

        // Inicializacao dos caches
        public void Inicializacao(IHost host)
        {
            if (HostingEnvironment.EnvironmentName.Contains(EnvironmentTest) == false)
            {
                host.InicializarMigracoes();
            }

            InicializarFilas(host);

            NameValueCollection padroesDeExecucaoSchedulador = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"},
                {"quartz.threadPool.threadCount", "1"}
            };

            _scheduler = host.Services.GetService<IScheduladorProcessos<IScheduler>>();
            ScheduleRenovacaoAutomaticaLicencas scheduleRenovacaoLicencas = host.Services.GetService<ScheduleRenovacaoAutomaticaLicencas>();
            _scheduler.Inicializar(scheduleRenovacaoLicencas.Inicializacao, padroesDeExecucaoSchedulador);
        }

        private void InicializarFilas(IHost host)
        {
            if (HostingEnvironment.EnvironmentName.Contains(EnvironmentTest) == false)
            {
                string xml = File.ReadAllText(_configuracoes.CaminhoMiddlewareXml);

                host.InicializarMiddleware(xml, middCfg =>
                {
                    lock (lockObject)
                    {
                        LogManager.Use<Log4NetFactory>();

                        ContainerBuilder containerBuilder = new ContainerBuilder();
                        containerBuilder.Populate(_services);
                        containerBuilder.RegisterInstance(host.Services.GetService<IMemoryCache>()).SingleInstance();
                        IContainer container = containerBuilder.Build();

                        middCfg.UseContainer<AutofacBuilder>
                            (customizations => { customizations.ExistingLifetimeScope(container); });
                        middCfg.RegistrarBehaviors();
                    }
                });

                _middlewaWatcher = host.Services.GetService<IMiddlewareFileWatcher>();

                _middlewaWatcher.Inicializar(_configuracoes.CaminhoMiddlewareXml,
                    (middleware, novoXml) => middleware.AplicarMudancasDeConfiguracao(novoXml));

                _middleware = host.Services.GetService<IMiddleware>();
            }
        }

        public void ConfigurarServicos(IServiceCollection services)
        {
            _services = services;
            services.Configure<GestorProdutosConfiguracoes>(Configuration);

            services.AddScoped(sp =>
                sp.GetService<IOptionsSnapshot<GestorProdutosConfiguracoes>>().Value);

            services.AddAutoMapper();
            services.AdicionarProvedorServicoMSDI();
            services.AdicionarServicosBaseLog();
            services.InicializarInputOutput();
            services.AdicionarServicoCacheEmMemoriaLocal("Licenciamento");
            services.AdicionarProcessamentoDeLicencas();
            services.AdicionarDependenciasRepositorioslicenciamentoMSSql();
            services.AdicionarDependenciasNoJobRenovacaoLicencas();
            services.AdicionarLog4Net(_configuracoes.CaminhoArquivoLog);
            services.AdicionarMiddleware();
            services.AdicionarScheduladorQuartz();

            InicializarRecursosHabilitaveis(services);
            InicializarClienteCertificadoDigital(services);

            services.AdicionarMigracoes(_configuracoes.AppSettings.ConnectionString, DbProviderEnum.SqlServer);

            services.BuildServiceProvider();
        }

        private void InicializarRecursosHabilitaveis(IServiceCollection services)
        {
            IConfigurationSection secaoRecursosHabilitaveis = Configuration.GetSection(SecaoRecursosHabilitaveis);

            IEnumerable<KeyValuePair<string, bool>> recursos = secaoRecursosHabilitaveis
                .AsEnumerable()
                .Select(item =>
                    new KeyValuePair<string, bool>
                    (
                        item.Key,
                        string.IsNullOrEmpty(item.Value)
                            ? false
                            : bool.Parse(item.Value)
                    ));

            services.AdicionarServicosDeHabilitarRecurso(recursos);
        }

        private void InicializarClienteCertificadoDigital(IServiceCollection serviceCollection)
        {
            CriarConfiguracaoBaseadoEmUmaSecao(SecaoCertificadoDigital)
                .AdicionarConfiguracaoCertificadoAcessoApi()
                .AdicionarConfiguracaoCertificadoCliente();

            serviceCollection.AdicionarServicosCertificadoCliente();
            serviceCollection.AdicionarServicosCertificadoAcessoApi();
            serviceCollection.AdicionarServicosCertificadoStore();
            serviceCollection.AddMemoryCache();
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