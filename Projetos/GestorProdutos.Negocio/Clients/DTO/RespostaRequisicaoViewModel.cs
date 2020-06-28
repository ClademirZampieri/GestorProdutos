using System.ComponentModel.DataAnnotations;

namespace NDD.Gerenciamento.Geral.Clients.DTO
{
    /// <summary>
    /// Resposta de requisição
    /// </summary>
    public class RespostaRequisicaoViewModel
    {
        /// <summary>
        /// Codigo da resposta
        /// </summary>
        [Required]
        public int Codigo { get; set; }
        /// <summary>
        /// Mensagem da resposta
        /// </summary>
        [Required]
        public string Mensagem { get; set; }
        /// <summary>
        /// Tipo de resposta, onde ela pode assumir os seguintes retornos
        /// <code>0 = Informacao </code>
        /// <code>1 = Aviso </code>
        /// <code>2 = Erro </code>
        /// </summary>
        [Required]
        public int TipoResposta { get; set; }
        /// <summary>
        /// Descricao detalhada de uma resposta, este campo é opcional.
        /// </summary>
        public string Descricao { get; set; }
    }
}
