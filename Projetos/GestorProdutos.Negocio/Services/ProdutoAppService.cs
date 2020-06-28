using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GestorProdutos.Base.Respostas;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Core.DomainObjects;
using GestorProdutos.Negocio.ViewModels;
using GestorProdutos.Base.Estruturas;

namespace GestorProdutos.Negocio.Services
{
    public class ProdutoAppService : IProdutoAppService
    {
        private readonly IProdutoFrontRepository _produtoRepository;
        private readonly IEstoqueService _estoqueService;
        private readonly IMapper _mapper;

        public ProdutoAppService(IProdutoFrontRepository produtoRepository,
                                 IMapper mapper,
                                 IEstoqueService estoqueService)
        {
            _produtoRepository = produtoRepository;
            _mapper = mapper;
            _estoqueService = estoqueService;
        }

        public async Task<ProdutoViewModel> ObterPorId(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }

        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterTodos());
        }

        public async Task<Result<Exception, RespostaDeRequisicao>> AdicionarProduto(ProdutoViewModel produtoViewModel)
        {
            try
            {
                var produto = _mapper.Map<Produto>(produtoViewModel);

                _produtoRepository.Adicionar(produto);

                await _produtoRepository.UnitOfWork.Commit();

                return CriarRespostaDeSucesso();
            }
            catch (Exception ex)
            {
                return CriarRespostaDeErro(ex);
            }
        }

        public async Task<Result<Exception, RespostaDeRequisicao>> AtualizarProduto(ProdutoViewModel produtoViewModel)
        {
            try
            {
                var produto = _mapper.Map<Produto>(produtoViewModel);
                _produtoRepository.Atualizar(produto);

                await _produtoRepository.UnitOfWork.Commit();

                return CriarRespostaDeSucesso();
            }
            catch (Exception ex)
            {
                return CriarRespostaDeErro(ex);
            }
        }

        public async Task<ProdutoViewModel> DebitarEstoque(Guid id, int quantidade)
        {
            if (!_estoqueService.DebitarEstoque(id, quantidade).Result)
            {
                throw new DomainException("Falha ao debitar estoque");
            }

            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }

        public async Task<ProdutoViewModel> ReporEstoque(Guid id, int quantidade)
        {
            if (!_estoqueService.ReporEstoque(id, quantidade).Result)
            {
                throw new DomainException("Falha ao repor estoque");
            }

            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }

        public void Dispose()
        {
            _produtoRepository?.Dispose();
            _estoqueService?.Dispose();
        }

        private RespostaDeRequisicao CriarRespostaDeErro(Exception ex)
        {
            return new RespostaDeRequisicao()
            {
                Codigo = 1,
                Mensagem = "Erro ao adicionar o produto",
                Descricao = $"{ex.Message} - {ex.StackTrace}",
                TipoResposta = TipoResposta.Informacao
            };
        }

        private RespostaDeRequisicao CriarRespostaDeSucesso()
        {
            return new RespostaDeRequisicao()
            {
                Codigo = 1,
                Mensagem = "Sucesso",
                Descricao = "Operação realizada com sucesso",
                TipoResposta = TipoResposta.Informacao
            };
        }
    }
}