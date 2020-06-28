using FluentValidation;
using GestorProdutos.Base.Respostas;
using GestorProdutos.Catalogo.Domain;
using MediatR;
using GestorProdutos.Base.Estruturas;
using System;

namespace GestorProdutos.Application.Features.Catalogo.AtualizarProduto
{
    public class AtualizarProdutoCommand : IRequest<Result<Exception, RespostaDeRequisicao>>
    {
        public Guid Id { get; set; }
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
