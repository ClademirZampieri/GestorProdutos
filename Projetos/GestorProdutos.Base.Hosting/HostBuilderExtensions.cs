namespace GestorProdutos.Base.Hosting
{
    public static class HostBuilderExtensions
    {
        public static HostDeAplicacao UseStartup<TStartUp>(this HostDeAplicacao builder) where TStartUp: IStartup
        {
            builder.StartUp = typeof(TStartUp);
            return builder;
        }

        public static HostDeAplicacao UseJsonFile(this HostDeAplicacao builder, string jsonFile)
        {
            builder.JsonFiles.Add(jsonFile);
            return builder;
        }
    }
}
