using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NDD.MicroServico.Base.Hosting
{
    public sealed class HostDeAplicacao: IDisposable
    {
        private volatile bool _pararExecucao;
        private IHost _host;

        private Type _startUp;
        private IStartup _startUpObject;
        private CancellationTokenSource _cancellationToken;
        private Func<IHost, CancellationTokenSource> _funcaoCriacaoCancelamento;
        private Func<IHost, bool> _funcaoDeEfetuarParada;
        private IPlataformaHostLifetime _plataformaHostLifetime;
        private readonly List<string> _jsonFiles = new List<string>();

        public HostDeAplicacao CriarTokenDeCancelamento(Func<IHost, CancellationTokenSource> funcaoCriacaoCancelamento)
        {
            _funcaoCriacaoCancelamento = funcaoCriacaoCancelamento;
            return this;
        }
        public HostDeAplicacao PararExecucaoDoHost(Func<IHost, bool> funcaoDeEfetuarParada)
        {
            _funcaoDeEfetuarParada = funcaoDeEfetuarParada;
            return this;
        }
        public async Task RunAsync(string[] args, Action<IHostBuilder> customBuilder = null)
        {
            while (true)
            {
                _pararExecucao = true;
                IHostBuilder hostBuilder = new HostBuilder()
                     .ConfigureAppConfiguration((hostingContext, config) => CarregarConfiguracoes(args, config))
                     .ConfigureServices((hostingContext, services) => ConfigurarServicos(hostingContext, services));
                customBuilder?.Invoke(hostBuilder);
                using (_host = hostBuilder.Build())
                {
                    _plataformaHostLifetime = _host.Services.GetService<IPlataformaHostLifetime>();
                    var appLife = _host.Services.GetService<IApplicationLifetime>();
                    appLife.ApplicationStarted.Register(() => _startUpObject.Inicializacao(_host));
                    appLife.ApplicationStopping.Register(() => _startUpObject.Finalizacao(_host));
                    appLife.ApplicationStarted.Register(() => _plataformaHostLifetime?.NaInicializacao(this));
                    appLife.ApplicationStopping.Register(() => _plataformaHostLifetime?.NaFinalizacao(this));
                    _cancellationToken = _funcaoCriacaoCancelamento?.Invoke(_host);
                    _plataformaHostLifetime?.AntesDeExecutar(this);
                    if (_cancellationToken == null)
                        await _host.RunAsync();
                    else
                        await _host.RunAsync(_cancellationToken.Token);
                    _plataformaHostLifetime?.DepoisDeExecutar(this);
                }
                _host = null;
                if (_pararExecucao)
                    break;
            }
        }
        public HostDeAplicacao Start(string[] args, Action<IHostBuilder> customBuilder = null)
        {
            IHostBuilder hostBuilder = new HostBuilder()
                 .ConfigureAppConfiguration((hostingContext, config) => CarregarConfiguracoes(args, config))
                 .ConfigureServices((hostingContext, services) => ConfigurarServicos(hostingContext, services));
            customBuilder?.Invoke(hostBuilder);
            _host = hostBuilder.Build();
            _startUpObject.Inicializacao(_host);
            return this;
        }
        public HostDeAplicacao Stop()
        {
            _startUpObject?.Finalizacao(_host);
            return this;
        }

        public Type StartUp { get => _startUp; set => _startUp = value; }
        public List<string> JsonFiles { get => _jsonFiles; }
        public IHost Host { get => _host; }
        public bool PararExecucao { get => _pararExecucao; set => _pararExecucao = value; }
        public IStartup InstanciaDaClasseStartUp { get => _startUpObject; }
        private void ConfigurarServicos(HostBuilderContext hostingContext, IServiceCollection services)
        {
            IChangeToken reloadToken = hostingContext.Configuration.GetReloadToken();
            reloadToken.RegisterChangeCallback(callback =>
            {
                if (_funcaoDeEfetuarParada == null)
                    _pararExecucao = false;
                else
                    _pararExecucao = _funcaoDeEfetuarParada(_host);
                _cancellationToken?.Cancel();
            }, null);

            _startUpObject = (IStartup)Activator.CreateInstance(_startUp,
                new object[] { hostingContext.Configuration, hostingContext.HostingEnvironment });
            services.AddOptions();
            _startUpObject.ConfigurarServicos(services);
        }
        private void CarregarConfiguracoes(string[] args, IConfigurationBuilder config)
        {
            // config.Sources.Clear();
            _jsonFiles.ForEach(item => config.AddJsonFile(item, optional: false, reloadOnChange: true));
            config.AddEnvironmentVariables();
            if (args != null && args.Length > 0)
                config.AddCommandLine(args);
        }
        public void Dispose()
        {
            Stop();
            _host?.Dispose();
        }
    }
}

