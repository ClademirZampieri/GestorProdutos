using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GestorProduto.Sincronizacao.API.Base
{
    /// <summary>
    /// Controlador de i18n (internationalization)
    ///
    /// Essa classe é responsável por prover as resources de tradução apra o client
    ///
    /// </summary>
    [Route("api/[controller]")]
    public class I18nController : ApiControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;

        public I18nController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        /// <summary>
        /// Interface para obter as resources de uma linguagem específica
        /// </summary>
        /// <param name="lang">É a linguagem que deseja obter as resources. Provém diretamente da url pela configuração do Route</param>
        /// <returns>Retorna as resources de acordo com a linguagem</returns>
        [HttpGet]
        [Route("{lang}")]
        [AllowAnonymous]
        public IActionResult Get([FromRoute] string lang)
        {
            // Retorna a resource default
            var resourceDefaultPath = Path.GetFullPath(@"~\Resources\Strings." + lang + ".json").Replace("~\\", "");

            var resourcesDefault = JObject.Parse(System.IO.File.ReadAllText(resourceDefaultPath));

            var contentType = "application/json";

            return Content(resourcesDefault.ToString(), contentType);
        }
    }
}
