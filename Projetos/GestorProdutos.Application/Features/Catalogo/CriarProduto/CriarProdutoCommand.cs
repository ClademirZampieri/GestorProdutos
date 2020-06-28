using FluentValidation;
using GestorProdutos.Base.Respostas;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;

namespace GestorProdutos.Application.Features.Catalogo.CriarProduto
{
    public class CriarProdutoCommand : IRequest<Result<Exception, RespostaDeRequisicao>>
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Imagem { get; set; }
        public int QuantidadeEstoque { get; set; }
    }

    public class CriarProdutoCommandValidator : AbstractValidator<CriarProdutoCommand>
    {
        public CriarProdutoCommandValidator() { }
    }
}
