using System;
using GestorProdutos.Catalogo.Domain.Enums;
using GestorProdutos.Core.DomainObjects;

namespace GestorProdutos.Catalogo.Domain
{
    public class Produto : Entity, IAggregateRoot, IIdentifier
    {
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public string Imagem { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public StatusSincronizacaoEnum StatusSincronizacao { get; set; }

        protected Produto() { }
        public Produto(string nome, bool ativo, decimal valor, DateTime dataCadastro, string imagem)
        {
            Nome = nome;
            Ativo = ativo;
            Valor = valor;
            DataCadastro = dataCadastro;
            Imagem = imagem;

            Validar();
        }

        public void Ativar() => Ativo = true;

        public void Desativar() => Ativo = false;


        public void DebitarEstoque(int quantidade)
        {
            if (quantidade < 0) quantidade *= -1;
            if (!PossuiEstoque(quantidade)) throw new DomainException("Estoque insuficiente");
            QuantidadeEstoque -= quantidade;
        }

        public void ReporEstoque(int quantidade)
        {
            QuantidadeEstoque += quantidade;
        }

        public bool PossuiEstoque(int quantidade)
        {
            return QuantidadeEstoque >= quantidade;
        }

        public void Validar()
        {
            Validacoes.ValidarSeVazio(Nome, "O campo Nome do produto não pode estar vazio");
            Validacoes.ValidarSeMenorQue(Valor, 1, "O campo Valor do produto não pode se menor igual a 0");
            Validacoes.ValidarSeVazio(Imagem, "O campo Imagem do produto não pode estar vazio");
        }
    }
}