using System;
using System.Collections.Generic;
using System.Text;

namespace GestorProdutos.Base.Respostas
{
    public class RespostaDeRequisicao
    {
        /// <summary>
        /// Codigo da mensagem
        /// </summary>
        public int Codigo { get; set; }
        /// <summary>
        /// Mensagem relacionada ao codigo
        /// </summary>
        public string Mensagem { get; set; }
        /// <summary>
        /// Tipo de resposta
        /// </summary>
        public TipoResposta TipoResposta { get; set; }

        public string Descricao { get; set; }
    }

    public enum TipoResposta
    {
        Informacao = 0,
        Aviso = 1,
        Erro = 2
    }
}
