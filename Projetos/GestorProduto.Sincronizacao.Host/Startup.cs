using GestorProdutos.Application;
using GestorProdutos.Application.Schedules;
using GestorProdutos.Base.Enums;
using GestorProdutos.Base.Hosting;
using GestorProdutos.Base.Schedulador.Quartz;
using GestorProdutos.Infra.Configuracoes;
using GestorProdutos.Sincronizacao.Host.Extensions;
using GestorProdutos.Sincronizacao.Produtos.Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NDD.GestorProdutos.Migracoes;
using Quartz;
using System.Collections.Specialized;

namespace GestorProdutos.Sincronizacao.Host
{
    public class Startup : IStartup
    {
        public const string EnvironmentTest = "Test";
        private readonly GestorProdutosConfiguracoes _configuracoes;
        private IScheduladorProcessos<IScheduler> _scheduler;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

            _configuracoes = new GestorProdutosConfiguracoes();
            configuration.Bind(_configuracoes);
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public void Finalizacao(IHost host)
        {
            _scheduler?.Finalizar();
        }

        // Inicializacao dos caches
        public void Inicializacao(IHost host)
        {
            if (HostingEnvironment.EnvironmentName.Contains(EnvironmentTest) == false)
            {
                host.InicializarMigracoes();
            }

            NameValueCollection padroesDeExecucaoSchedulador = new NameValueCollection
            {
                {"quartz.serializer.type", "binary"},
                {"quartz.threadPool.threadCount", "1"}
            };

            _scheduler = host.Services.GetService<IScheduladorProcessos<IScheduler>>();
            ScheduleSincronizacaoProdutos scheduleSincronizacao = host.Services.GetService<ScheduleSincronizacaoProdutos>();
            _scheduler.Inicializar(scheduleSincronizacao.Inicializacao, padroesDeExecucaoSchedulador);
        }

        public void ConfigurarServicos(IServiceCollection services)
        {
            services.Configure<GestorProdutosConfiguracoes>(Configuration);

            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<GestorProdutosConfiguracoes>>().Value);
            services.AddDbContext<ProdutosSincronizacaoContext>(options => options.UseSqlServer(_configuracoes.AppSettings.ConnectionString), ServiceLifetime.Transient);
            services.AddMediatR(typeof(Startup));

            services.AddAutoMapper();
            services.AdicionarScheduladorQuartz();
            services.RegistrarServicos();
            services.AdicionarDependenciasNoJobDeSincronizacaoDeProdutos();

            services.AdicionarMigracoes(_configuracoes.AppSettings.ConnectionString, DbProviderEnum.SqlServer);

            services.BuildServiceProvider();
        }
    }
}