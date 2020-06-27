using AutoMapper;
using GestorProdutos.Negocio.Services;
using GestorProdutos.Negocio.ViewModels;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Features.Catalogo.CriarProduto
{
    public class CriarProdutoHandler : IRequestHandler<CriarProdutoCommand, Result<Exception, bool>>
    {
        private readonly IProdutoAppService _produtoService;
        private readonly IMapper _mapeamento;

        public CriarProdutoHandler(IProdutoAppService produtoService, IMapper mapeamento)
        {
            _produtoService = produtoService;
            _mapeamento = mapeamento;
        }

        public async Task<Result<Exception, bool>> Handle(CriarProdutoCommand command, CancellationToken cancellationToken)
        {
            var produto = _mapeamento.Map<CriarProdutoCommand, ProdutoViewModel>(command);

            await _produtoService.AdicionarProduto(produto);

            return true;
        }
    }
}
