using AutoMapper;
using GestorProdutos.Negocio.Services;
using GestorProdutos.Negocio.ViewModels;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Features.Catalogo.AtualizarProduto
{
    public class AtualizarProdutoHandler : IRequestHandler<AtualizarProdutoCommand, Result<Exception, bool>>
    {
        private readonly IProdutoAppService _produtoService;
        private readonly IMapper _mapeamento;

        public AtualizarProdutoHandler(IProdutoAppService produtoService, IMapper mapeamento)
        {
            _produtoService = produtoService;
            _mapeamento = mapeamento;
        }

        public async Task<Result<Exception, bool>> Handle(AtualizarProdutoCommand command, CancellationToken cancellationToken)
        {
            var produto = _mapeamento.Map<AtualizarProdutoCommand, ProdutoViewModel>(command);

            await _produtoService.AtualizarProduto(produto);

            return true;
        }
    }
}
