using FluentMigrator;
using GestorProdutos.Infra.Configuracoes;
using NDD.CentralSolucoes.Base.Estruturas;
using System;

namespace NDD.GestorProdutos.Migracoes.V0100
{
    [Migration(2020060000001, TransactionBehavior.Default)]
    public class MigracaoCriacaoBase : MigrationGestorProdutosBase
    {
        public MigracaoCriacaoBase(GestorProdutosConfiguracoes conf) : base(conf) { }

        public override void Down()
        {
        }

        public override void Up()
        {
            Result<Exception, Migration> resposta = VerificarTabelasCriadas(this)
                .Bind(CriarSchemas)
                .Bind(CriarTabela_Produtos);
        }

        private Result<Exception, Migration> VerificarTabelasCriadas(Migration migration)
        {
            if (Schema.Schema(SchemaGestorProdutos).Table("Produtos").Exists())
            {
                return new ApplicationException("Base ja foi criada");
            }

            return migration;
        }

        #region Schemas

        private Result<Exception, Migration> CriarSchemas(Migration migration)
        {
            if (!Schema.Schema(SchemaGestorProdutos).Exists())
            {
                Create.Schema(SchemaGestorProdutos);
            }

            return migration;
        }

        #endregion

        #region Tabelas

        private Result<Exception, Migration> CriarTabela_Produtos(Migration migration)
        {
            Create.Table("Produtos")
                .InSchema(SchemaGestorProdutos)

                .WithColumn("Id")
                .AsGuid()
                .PrimaryKey("PK_Produtos")
                .Unique()
                .NotNullable()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("Nome")
                .AsAnsiString()
                .NotNullable()
                .WithColumnDescription("Identificador do parceiro da NDD")

                .WithColumn("QuantidadeEstoque")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription("Cnpj do Cliente de Cobran�a")

                .WithColumn("Valor")
                .AsDecimal(18,2)
                .NotNullable()
                .WithColumnDescription("Nome do Cliente de Cobran�a")

                .WithColumn("DataCadastro")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription("Nome do Cliente")

                .WithColumn("Imagem")
                .AsAnsiString(250)
                .NotNullable()
                .WithColumnDescription("Nome do Cliente")

                .WithColumn("Ativo")
                .AsBoolean()
                .NotNullable()
                .WithColumnDescription("Nome da licen�a")

                .WithColumn("StatusSincronizacao")
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(2)
                .WithColumnDescription("Status da sincroniza��o, podendo ser: 1 - sincronizado, 2 - cria��o pendente de sincroniza��o, 3 - atualiza��o pendente de sincroniza��o");

            return migration;
        }
        #endregion

    }
}

