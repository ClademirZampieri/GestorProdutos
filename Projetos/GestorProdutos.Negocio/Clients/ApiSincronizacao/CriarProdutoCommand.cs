﻿
using System;

namespace NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes.NFSe
{
    public class CriarProdutoCommand
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Imagem { get; set; }
        public int QuantidadeEstoque { get; set; }
    }
}
