using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NDD.MicroServico.Base.Hosting
{
    public interface IStartup
    {
        void ConfigurarServicos(IServiceCollection services);
        void Inicializacao(IHost host);
        void Finalizacao(IHost host);
    }
}
