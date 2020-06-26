using FluentMigrator.Runner.VersionTableInfo;

namespace NDD.Licenciamento.Migracoes
{
    public class VersionTableMetadata : IVersionTableMetaData
    {
        public object ApplicationContext { get; set; }

        public virtual string SchemaName => "Licenciamento";

        public virtual string TableName => "VersionInfo";

        public virtual string ColumnName => "Version";

        public virtual string UniqueIndexName => "UC_Version";

        public virtual string AppliedOnColumnName => "AppliedOn";

        public virtual string DescriptionColumnName => "Description";

        public virtual bool OwnsSchema => true;
    }
}
