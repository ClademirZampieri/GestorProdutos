using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NDD.MicroServico.Base.Repositorios;
using System;
using System.Reflection;

namespace NDD.Licenciamento.Migracoes
{
    public static class Inicializacao
    {
        public static IServiceCollection AdicionarMigracoes(this IServiceCollection services, string connectionString, DbProviderEnum dbProvider, Func<IMigrationRunnerBuilder, bool> customizacao = null)
        {
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    if (customizacao != null && customizacao(rb))
                        return;
                    rb.ScanIn(Assembly.GetExecutingAssembly()).For.Migrations();
                    rb.ScanIn(Assembly.GetExecutingAssembly()).For.EmbeddedResources();

                    rb.WithGlobalConnectionString(connectionString);
                    switch (dbProvider)
                    {
                        case DbProviderEnum.SqlServer:
                            rb.AddSqlServer2016();
                            break;

                        case DbProviderEnum.SqlLite:
                            rb.AddSQLite();
                            break;
                    }
                })
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .AddScoped(typeof(IVersionTableMetaData), typeof(VersionTableMetadata));

            return services;
        }

        public static IHost InicializarMigracoes(this IHost host)
        {
            IMigrationRunner runner = host.Services.GetRequiredService<IMigrationRunner>();
            ExecutorDoMigrador(runner);
            return host;
        }

        private static void ExecutorDoMigrador(IMigrationRunner runner)
        {
            using (IMigrationScope runnerScope = runner.BeginScope())
            {
                try
                {
                    runner.MigrateUp();
                    runnerScope.Complete();
                }
                catch (Exception)
                {
                    runnerScope.Cancel();
                    throw;
                }
            }
        }

        public static IApplicationBuilder InicializarMigracoes(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                ExecutorDoMigrador(runner);
                return app;
            }
        }
    }
}
