namespace GestorProdutos.Sincronizacao.API.Features.Share
{
    /// <summary>
    /// Resposta padrão do adaptador no produto
    /// </summary>
    public class RespostaDeRequisicaoViewModel
    {
        /// <summary>
        /// Codigo da resposta
        /// </summary>
        public int Codigo { get; set; }
        /// <summary>
        /// Mensagem da resposta
        /// </summary>
        public string Mensagem { get; set; }
        /// <summary>
        /// Tipo de resposta, onde ela pode assumir os seguintes retornos
        /// <code>0 = Informacao </code>
        /// <code>1 = Aviso </code>
        /// <code>2 = Erro </code>
        /// </summary>
        public int TipoResposta { get; set; }
        /// <summary>
        /// Descricao detalhada de uma resposta, este campo é opcional.
        /// </summary>
        public string Descricao { get; set; }
    }
}
