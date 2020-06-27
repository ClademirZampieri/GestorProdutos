using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;

namespace GestorProdutos.Application.Features.Catalogo.CriarProduto
{
    public class CriarProdutoCommand : IRequest<Result<Exception, bool>>
    {
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string Imagem { get; private set; }
        public int QuantidadeEstoque { get; private set; }
    }
}
