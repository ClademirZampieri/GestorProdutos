using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;

namespace GestorProdutos.Sincronizacao.API.Filters
{
    public class OcultarDocumentacaoSwagger : IDocumentFilter
    {
        private static readonly string AppSettingsJson = "appsettings.json";

        private static readonly string SecaoFiltrosSwagger = "FiltrosSwagger";

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            IConfigurationRoot arquivoDeConfiguracao = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), AppSettingsJson)).Build();

            OcultarDocumentacaoDasChamadas(swaggerDoc, arquivoDeConfiguracao);

            OcultarDocumentacaoDosControladores(swaggerDoc, arquivoDeConfiguracao);
        }

        private static void OcultarDocumentacaoDasChamadas(SwaggerDocument swaggerDoc, IConfigurationRoot arquivoDeConfiguracao)
        {
            foreach (KeyValuePair<string, PathItem> path in swaggerDoc.Paths)
            {
                string manterDocumentacaoDasChamadas = arquivoDeConfiguracao.GetSection(SecaoFiltrosSwagger).GetSection(path.Key).Value;

                bool manterDocumentacaoDasChamadasConvertida = manterDocumentacaoDasChamadas == "1";

                if (manterDocumentacaoDasChamadasConvertida == false)
                {
                    path.Value.Delete = null;
                    path.Value.Get = null;
                    path.Value.Head = null;
                    path.Value.Options = null;
                    path.Value.Patch = null;
                    path.Value.Post = null;
                    path.Value.Put = null;
                }
            }
        }

        private static void OcultarDocumentacaoDosControladores(SwaggerDocument swaggerDoc, IConfigurationRoot arquivoDeConfiguracao)
        {
            int contagemDeControladores = swaggerDoc.Tags.Count - 1;

            for (int i = contagemDeControladores; i >= 0; i--)
            {
                string manterDocumentacaoDosControladores = arquivoDeConfiguracao.GetSection(SecaoFiltrosSwagger).GetSection(swaggerDoc.Tags[i].Name).Value;

                bool manterDocumentacaoDosControladoresConvertido = manterDocumentacaoDosControladores == "1";

                if (manterDocumentacaoDosControladoresConvertido == false)
                {
                    swaggerDoc.Tags.RemoveAt(i);
                }
            }
        }
    }
}
