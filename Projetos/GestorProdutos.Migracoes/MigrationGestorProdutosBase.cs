using FluentMigrator;
using GestorProdutos.Infra.Configuracoes;

namespace NDD.GestorProdutos.Migracoes
{
    public abstract class MigrationGestorProdutosBase : Migration
    {
        protected const string SchemaGestorProdutos = "dbo";
        protected const string CNSchemaGestorProdutos = "@SchemaGestorProdutos";

        protected readonly GestorProdutosConfiguracoes _config;

        public MigrationGestorProdutosBase(GestorProdutosConfiguracoes conf)
        {
            _config = conf;
        }
    }
}
