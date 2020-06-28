using AutoMapper;
using GestorProdutos.Base.Respostas;
using GestorProdutos.Negocio.Services;
using GestorProdutos.Negocio.ViewModels;
using MediatR;
using GestorProdutos.Base.Estruturas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Features.Catalogo.AtualizarProduto
{
    public class AtualizarProdutoHandler : IRequestHandler<AtualizarProdutoCommand, Result<Exception, RespostaDeRequisicao>>
    {
        private readonly IProdutoAppService _produtoService;
        private readonly IMapper _mapeamento;

        public AtualizarProdutoHandler(IProdutoAppService produtoService, IMapper mapeamento)
        {
            _produtoService = produtoService;
            _mapeamento = mapeamento;
        }

        public async Task<Result<Exception, RespostaDeRequisicao>> Handle(AtualizarProdutoCommand command, CancellationToken cancellationToken)
        {
            var produto = _mapeamento.Map<AtualizarProdutoCommand, ProdutoViewModel>(command);

            return await _produtoService.AtualizarProduto(produto);
        }
    }
}
