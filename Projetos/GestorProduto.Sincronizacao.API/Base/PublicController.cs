using Microsoft.AspNetCore.Mvc;

namespace GestorProdutos.Sincronizacao.API.Base
{
    [Route("api/[controller]")]
    public class PublicController : ApiControllerBase
    {
        /// <summary>
        /// Informa para o client que está ativa
        /// Útil para validar tokens e para descobrir o estado da API
        /// </summary>
        [HttpGet]
        [Route("is-alive")]
        public IActionResult IsAlive()
        {
            return Ok(true);
        }
    }
}
