using Microsoft.Extensions.Hosting;
using GestorProdutos.Base.Hosting;
using System;

namespace GestorProdutos.Sincronizacao.Host
{
    class Program
    {
        private static async System.Threading.Tasks.Task Main(string[] args)
        {
            string jsonFile = "appsettings.json";

            await new HostDeAplicacao()
                .UseStartup<Startup>()
                .UseJsonFile(jsonFile)
                .RunAsync(args, hostBuilder =>
                    hostBuilder
                        .UseConsoleLifetime()
                        .UseEnvironment(EnvironmentName.Development));
        }
    }
}
