using FluentMigrator;
using NDD.CentralSolucoes.Base.Estruturas;
using NDD.Licenciamento.Domain.Configuracoes;
using System;

namespace NDD.Licenciamento.Migracoes.V0100
{
    [Migration(2020040000001, TransactionBehavior.Default)]
    public class MigracaoCriacaoBase : MigrationLicenciamentoBase
    {
        public MigracaoCriacaoBase(LicenciamentoConfiguracoes conf) : base(conf) { }

        public override void Down()
        {
        }

        public override void Up()
        {
            Result<Exception, Migration> resposta = VerificarTabelasCriadas(this)
                .Bind(CriarSchemas)
                .Bind(CriarFuncao_fnMontarListaBigInt)
                .Bind(CriarTabela_Licenca)
                .Bind(CriarTabela_LicencaEmpresa)
                .Bind(CriarTabela_LicencaProduto)
                .Bind(CriarTabela_LicencaGerada)
                .Bind(CriarTabela_LicencaOcorrencia)
                .Bind(CriarTabela_HistoricoLicencaEmpresa)
                .Bind(CriarTabela_ParceiroNdd);
        }

        private Result<Exception, Migration> VerificarTabelasCriadas(Migration migration)
        {
            if (Schema.Schema(SchemaLicenciamento).Table("Licenca").Exists())
            {
                return new ApplicationException("Base ja foi criada");
            }

            return migration;
        }

        #region Schemas

        private Result<Exception, Migration> CriarSchemas(Migration migration)
        {
            if (!Schema.Schema(SchemaMiddleware).Exists())
            {
                Create.Schema(SchemaMiddleware);
            }

            if (!Schema.Schema(SchemaLicenciamento).Exists())
            {
                Create.Schema(SchemaLicenciamento);
            }

            return migration;
        }

        #endregion

        #region Tabelas

        private Result<Exception, Migration> CriarTabela_Licenca(Migration migration)
        {
            Create.Table("Licenca")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_Licenca")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("ParceiroNdd")
                .AsGuid()
                .NotNullable()
                .WithColumnDescription("Identificador do parceiro da NDD")

                .WithColumn("CnpjCliente")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Cnpj do Cliente de Cobran�a")

                .WithColumn("NomeCobrador")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription("Nome do Cliente de Cobran�a")

                .WithColumn("NomeCliente")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription("Nome do Cliente")

                .WithColumn("NomeLicenca")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription("Nome da licen�a")

                .WithColumn("Ambiente")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription("Tipo do ambiente sendo 1 = Produ��o, 2 = Homologa��o")

                .WithColumn("TipoLicenca")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription("Tipo da licen�a sendo 1 = Trial, 2 = Emergencial, 3 = Produ��o")

                .WithColumn("DataInicial")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription("Data do in�cio da vig�ncia do per�odo da licen�a")

                .WithColumn("DataExpiracao")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription("Data limite da vig�ncia da licen�a")

                .WithColumn("DiasDeCarencia")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription("Quantidade de dias que a licen�a pode operar em modo de car�ncia")

                .WithColumn("Volumetria")
                .AsInt64()
                .Nullable()
                .WithColumnDescription("Volumetria estimada do cliente")

                .WithColumn("RenovacaoAutomatica")
                .AsBoolean()
                .NotNullable()
                .WithColumnDescription("Informa se a licenca deve ser renovada automaticamente ou n�o");

            Create
                .Index("IX_Licenca_LicencaId")
                .OnTable("Licenca")
                .InSchema(SchemaLicenciamento)
                .OnColumn("CnpjCliente").Ascending()
                .OnColumn("Ambiente").Ascending();


            return migration;
        }

        private Result<Exception, Migration> CriarTabela_LicencaEmpresa(Migration migration)
        {
            Create.Table("LicencaEmpresa")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_LicencaEmpresa")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("LicencaId")
                .AsInt64()
                .Nullable()
                .WithColumnDescription("Identificador da licen�a a qual est� vinculada")

                .WithColumn("Cnpj")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Cnpj da empresa anexa")

                .WithColumn("Nome")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription("Nome da empresa anexa")

                .WithColumn("Status")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription("Status da empresa sendo 0 = Inativa, 1 = Ativa,")

                .WithColumn("DataDesvinculo")
                .AsDateTime()
                .Nullable()
                .WithColumnDescription("Data em que a empresa foi desvinculada da licen�a pela �ltima vez")

                .WithColumn("UtilizouTrial")
                .AsBoolean()
                .NotNullable()
                .WithColumnDescription("Indica se a empresa j� utilizou uma licen�a trial");

            Create
                .Index("IX_LicencaEmpresa_LicencaId")
                .OnTable("LicencaEmpresa")
                .InSchema(SchemaLicenciamento)
                .OnColumn("LicencaId").Ascending();

            Create
                .Index("IX_LicencaEmpresa_Cnpj")
                .OnTable("LicencaEmpresa")
                .InSchema(SchemaLicenciamento)
                .OnColumn("Cnpj").Ascending();

            return migration;
        }

        private Result<Exception, Migration> CriarTabela_LicencaProduto(Migration migration)
        {
            Create.Table("LicencaProduto")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_LicencaProduto")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("LicencaId")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Identificador da licen�a a qual est� vinculada")

                .WithColumn("Produto")
                .AsInt32()
                .NotNullable()
                .WithColumnDescription(@"Tipo do produto vinculado sendo NFeEmissao = 2, NFeRecepcao = 3, CTeEmissao = 4, CTeRecepcao = 5, MDFeEmissao = 6, MDFeRecepcao = 7, NFSeEmissao = 8, NFCeEmissao = 9, NFSeRecepcao = 10");


            Create
                .Index("IX_LicencaProduto_LicencaId")
                .OnTable("LicencaProduto")
                .InSchema(SchemaLicenciamento)
                .OnColumn("LicencaId").Ascending();

            return migration;
        }

        private Result<Exception, Migration> CriarTabela_LicencaGerada(Migration migration)
        {
            Create.Table("LicencaGerada")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_LicencaGerada")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("LicencaId")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Identificador da licen�a a qual est� vinculada")

                .WithColumn("Xml")
                .AsXml()
                .NotNullable()
                .WithColumnDescription(@"Xml compilado da licen�a")

                .WithColumn("Versao")
                .AsAnsiString(10)
                .NotNullable()
                .WithColumnDescription(@"Vers�o do Xml compilado da licen�a")

                .WithColumn("Data")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription(@"Data da gera��o do Xml compilado da licen�a")

                .WithColumn("Usuario")
                .AsAnsiString(30)
                .NotNullable()
                .WithColumnDescription(@"Nome do usu�rio que solicitou o Xml compilado da licen�a");


            Create
                .Index("IX_LicencaGerada_LicencaId")
                .OnTable("LicencaGerada")
                .InSchema(SchemaLicenciamento)
                .OnColumn("LicencaId").Ascending();

            return migration;
        }

        private Result<Exception, Migration> CriarTabela_LicencaOcorrencia(Migration migration)
        {
            Create.Table("LicencaOcorrencia")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_LicencaOcorrencia")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("LicencaId")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Identificador da licen�a a qual est� vinculada")

                .WithColumn("Descricao")
                .AsAnsiString(int.MaxValue)
                .NotNullable()
                .WithColumnDescription(@"Descri��o da ocorr�ncia")

                .WithColumn("DataOcorrencia")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription(@"Data da ocorr�ncia")

                .WithColumn("Usuario")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription(@"Nome do usu�rio que gerou a ocorr�ncia");


            Create
                .Index("IX_LicencaOcorrencia_LicencaId")
                .OnTable("LicencaOcorrencia")
                .InSchema(SchemaLicenciamento)
                .OnColumn("LicencaId").Ascending();

            return migration;
        }

        private Result<Exception, Migration> CriarTabela_ParceiroNdd(Migration migration)
        {
            Create.Table("ParceiroNdd")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsGuid()
                .PrimaryKey("PK_ParceiroNdd")
                .Unique()
                .NotNullable()
                .WithDefaultValue("NEWID()")
                .WithColumnDescription("Identificador �nico")

                .WithColumn("Nome")
                .AsAnsiString(100)
                .NotNullable()
                .WithColumnDescription(@"Nome do parceiro Ndd");

            return migration;
        }

        private Result<Exception, Migration> CriarTabela_HistoricoLicencaEmpresa(Migration migration)
        {
            Create.Table("HistoricoLicencaEmpresa")
                .InSchema(SchemaLicenciamento)

                .WithColumn("Id")
                .AsInt64()
                .PrimaryKey("PK_HistoricoLicencaEmpresa")
                .Unique()
                .NotNullable()
                .Identity()
                .WithColumnDescription("Identificador �nico")

                .WithColumn("LicencaId")
                .AsInt64()
                .Nullable()
                .WithColumnDescription("Identificador da licen�a a qual est� vinculada")

                .WithColumn("Cnpj")
                .AsInt64()
                .NotNullable()
                .WithColumnDescription("Cnpj da empresa anexa")

                .WithColumn("DataAtivacao")
                .AsDateTime()
                .NotNullable()
                .WithColumnDescription("Data em que a empresa foi desvinculada da licen�a pela �ltima vez")

                .WithColumn("DataDesativacao")
                .AsDateTime()
                .Nullable()
                .WithColumnDescription("Data em que a empresa foi desvinculada da licen�a pela �ltima vez");

            Create
                .Index("IX_HistoricoLicencaEmpresa_LicencaId")
                .OnTable("HistoricoLicencaEmpresa")
                .InSchema(SchemaLicenciamento)
                .OnColumn("LicencaId").Ascending()
                .OnColumn("DataAtivacao").Ascending()
                .OnColumn("DataDesativacao").Ascending()
                .OnColumn("Cnpj").Ascending();

            return migration;
        }

        #endregion

        #region Funcoes

        private const string CN_fnMontarListaBigInt = @"
            CREATE FUNCTION [@SchemaLicenciamento].[fnMontarListaBigInt](
                @sInputList VARCHAR(MAX) -- List of delimited items
              , @sDelimiter VARCHAR(1) = '|' -- delimiter that separates items
            ) RETURNS @List TABLE (id bigint)

            BEGIN
            DECLARE @sItem VARCHAR(8000)
            WHILE CHARINDEX(@sDelimiter,@sInputList,0) <> 0
            BEGIN
	            SELECT
		            @sItem=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@sDelimiter,@sInputList,0)-1))),
		            @sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@sDelimiter,@sInputList,0)+LEN(@sDelimiter),LEN(@sInputList))))
	            IF LEN(@sItem) > 0
		            INSERT INTO @List SELECT CONVERT(bigint,@sItem)
            END
            IF LEN(@sInputList) > 0
	            INSERT INTO @List SELECT CONVERT(bigint,@sInputList) -- Put the last item in
            RETURN
            END        
        ";

        private Result<Exception, Migration> CriarFuncao_fnMontarListaBigInt(Migration migration)
        {
            Execute.Sql(CN_fnMontarListaBigInt.Replace(CNSchemaLicenciamento, SchemaLicenciamento));
            return migration;
        }

        #endregion
    }
}

