using GestorProduto.Sincronizacao.API.Exceptions;
using GestorProduto.Sincronizacao.API.Features.Share;
using GestorProdutos.Application.Features.Catalogo.AtualizarProduto;
using GestorProdutos.Application.Features.Catalogo.CriarProduto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestorProduto.Sincronizacao.API.Features
{
    [Route("api/[controller]")]
    public class GestorProdutosController : ApiBaseExecucaoControlador
    {
        public GestorProdutosController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Salva a licença e captura o cnpj a ser persisitido com base no parâmetro
        /// </summary>
        [ProducesResponseType(typeof(CriarProdutoRespostaViewModel), 200)]
        [ProducesResponseType(typeof(ExceptionPayload), 400)]
        [HttpPost("SalvarLicenca")]
        public async Task<IActionResult> SalvarLicenca([FromBody] CriarProdutoCommand salvarProdutoCommand)
            => await Executar<CriarProdutoCommand, bool, CriarProdutoRespostaViewModel>(
                salvarProdutoCommand,
                () => "Os dados do produto não podem ser nulos.",
                () => _mediator.Send(salvarProdutoCommand));

        /// <summary>
        /// Salva a licença e captura o cnpj a ser persisitido com base no parâmetro
        /// </summary>
        [ProducesResponseType(typeof(AtualizarProdutoRespostaViewModel), 200)]
        [ProducesResponseType(typeof(ExceptionPayload), 400)]
        [HttpPost("AtualizarLicenca")]
        public async Task<IActionResult> AtualizarLicenca([FromBody] AtualizarProdutoCommand atualizarProdutoCommand)
            => await Executar<AtualizarProdutoCommand, bool, AtualizarProdutoRespostaViewModel>(
                atualizarProdutoCommand,
                () => "Os dados do produto não podem ser nulos.",
                () => _mediator.Send(atualizarProdutoCommand));
    }
}
