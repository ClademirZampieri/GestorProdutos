using GestorProdutos.Sincronizacao.API.Exceptions;
using GestorProdutos.Sincronizacao.API.Features.Share;
using GestorProdutos.Application.Features.Catalogo.AtualizarProduto;
using GestorProdutos.Application.Features.Catalogo.CriarProduto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestorProdutos.Sincronizacao.API.Features
{
    [Route("api/[controller]")]
    public class GestorProdutosController : ApiBaseExecucaoControlador
    {
        public GestorProdutosController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Cria um novo produto na base de dados
        /// </summary>
        [ProducesResponseType(typeof(CriarProdutoRespostaViewModel), 200)]
        [ProducesResponseType(typeof(ExceptionPayload), 400)]
        [HttpPost("CriarProduto")]
        public async Task<IActionResult> CriarProduto([FromBody] CriarProdutoCommand criarProdutoCommand)
            => await Executar<CriarProdutoCommand, bool, CriarProdutoRespostaViewModel>(
                criarProdutoCommand,
                () => "Os dados do produto não podem ser nulos.",
                () => _mediator.Send(criarProdutoCommand));

        /// <summary>
        /// Atualiza os dados de um determinado produto na base de dados
        /// </summary>
        [ProducesResponseType(typeof(AtualizarProdutoRespostaViewModel), 200)]
        [ProducesResponseType(typeof(ExceptionPayload), 400)]
        [HttpPost("AtualizarProduto")]
        public async Task<IActionResult> AtualizarProduto([FromBody] AtualizarProdutoCommand atualizarProdutoCommand)
            => await Executar<AtualizarProdutoCommand, bool, AtualizarProdutoRespostaViewModel>(
                atualizarProdutoCommand,
                () => "Os dados do produto não podem ser nulos.",
                () => _mediator.Send(atualizarProdutoCommand));
    }
}
