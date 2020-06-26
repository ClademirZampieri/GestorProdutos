using FluentMigrator;
using NDD.Licenciamento.Domain.Configuracoes;

namespace NDD.Licenciamento.Migracoes
{
    public abstract class MigrationLicenciamentoBase : Migration
    {
        protected const string SchemaLicenciamento = "Licenciamento";
        protected const string SchemaMiddleware = "middleware";
        protected const string CNSchemaLicenciamento = "@SchemaLicenciamento";

        protected readonly LicenciamentoConfiguracoes _config;

        public MigrationLicenciamentoBase(LicenciamentoConfiguracoes conf)
        {
            _config = conf;
        }
    }
}
