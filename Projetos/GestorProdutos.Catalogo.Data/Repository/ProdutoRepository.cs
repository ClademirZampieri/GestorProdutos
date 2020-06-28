﻿using Microsoft.EntityFrameworkCore;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorProdutos.Catalogo.Data.Extensions;
using GestorProdutos.Catalogo.Domain.Enums;

namespace GestorProdutos.Catalogo.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly CatalogoContext _context;

        public ProdutoRepository(CatalogoContext context)
        {
            _context = context;
        }
        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            var retorno = await _context.Produtos.AsNoTracking().ToListAsync();

            return retorno;
        }

        public async Task<IEnumerable<Produto>> ObterProdutosNaoSincronizados()
        {
            var query = _context.Produtos.Where(x => x.StatusSincronizacao != StatusSincronizacaoEnum.Sincronizacado);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Produto> ObterPorId(Guid id)
        {
            //return await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return await _context.Produtos.FindAsync(id);
        }

        public void Adicionar(Produto produto)
        {
            produto.StatusSincronizacao = StatusSincronizacaoEnum.PendenteDeCriacao;
            _context.Produtos.Add(produto);
        }

        public void Atualizar(Produto produto)
        {
            produto.StatusSincronizacao = StatusSincronizacaoEnum.PendenteDeAtualizacao;
            _context.DetachLocal(produto, produto.Id);
            _context.Produtos.Update(produto);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}