using FluentValidation;
using GestorProdutos.Catalogo.Domain;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;

namespace GestorProdutos.Application.Features.Catalogo.AtualizarProduto
{
    public class AtualizarProdutoCommand : IRequest<Result<Exception, bool>>
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Imagem { get; set; }
        public int QuantidadeEstoque { get; set; }
    }

    public class AtualizarProdutoCommandValidator : AbstractValidator<AtualizarProdutoCommand>
    {
        public AtualizarProdutoCommandValidator() { }
    }
}
