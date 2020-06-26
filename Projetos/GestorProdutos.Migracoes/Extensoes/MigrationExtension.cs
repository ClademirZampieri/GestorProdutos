using FluentMigrator;
using NDD.CentralSolucoes.Base.Estruturas;
using System;

namespace NDD.Licenciamento.Migracoes.Extensoes
{
    public static class MigrationExtension
    {
        private const string ScriptCriacaoFila = @"
            CREATE TABLE [@schema].[@fila](
	            [Id] [uniqueidentifier] NOT NULL,
	            [CorrelationId] [varchar](255) NULL,
	            [ReplyToAddress] [varchar](255) NULL,
	            [Recoverable] [bit] NOT NULL,
	            [Expires] [datetime] NULL,
	            [Headers] [varchar](max) NOT NULL,
	            [Body] [varbinary](max) NULL,
	            [RowVersion] [bigint] IDENTITY(1,1) NOT NULL
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

            CREATE TABLE [@schema].[@fila.Timeouts](
	            [Id] [uniqueidentifier] NOT NULL,
	            [CorrelationId] [varchar](255) NULL,
	            [ReplyToAddress] [varchar](255) NULL,
	            [Recoverable] [bit] NOT NULL,
	            [Expires] [datetime] NULL,
	            [Headers] [varchar](max) NOT NULL,
	            [Body] [varbinary](max) NULL,
	            [RowVersion] [bigint] IDENTITY(1,1) NOT NULL
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

            CREATE TABLE [@schema].[@fila.TimeoutsDispatcher](
	            [Id] [uniqueidentifier] NOT NULL,
	            [CorrelationId] [varchar](255) NULL,
	            [ReplyToAddress] [varchar](255) NULL,
	            [Recoverable] [bit] NOT NULL,
	            [Expires] [datetime] NULL,
	            [Headers] [varchar](max) NOT NULL,
	            [Body] [varbinary](max) NULL,
	            [RowVersion] [bigint] IDENTITY(1,1) NOT NULL
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ";

        public static Result<Exception, Migration> CriarTabela_FilasBase(this Migration migration, string schema, string fila)
        {
            string script = ScriptCriacaoFila
                .Replace("@schema", schema)
                .Replace("@fila", fila);

            migration.Execute.Sql(script);

            return migration;
        }
    }
}
