using AutoMapper;
using GestorProdutos.Base.Respostas;
using GestorProdutos.Negocio.Services;
using GestorProdutos.Negocio.ViewModels;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Features.Catalogo.CriarProduto
{
    public class CriarProdutoHandler : IRequestHandler<CriarProdutoCommand, Result<Exception, RespostaDeRequisicao>>
    {
        private readonly IProdutoAppService _produtoService;
        private readonly IMapper _mapeamento;

        public CriarProdutoHandler(IProdutoAppService produtoService, IMapper mapeamento)
        {
            _produtoService = produtoService;
            _mapeamento = mapeamento;
        }

        public async Task<Result<Exception, RespostaDeRequisicao>> Handle(CriarProdutoCommand command, CancellationToken cancellationToken)
        {
            var produto = _mapeamento.Map<CriarProdutoCommand, ProdutoViewModel>(command);

            return await _produtoService.AdicionarProduto(produto);
        }
    }
}
