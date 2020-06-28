using System.ComponentModel.DataAnnotations;

namespace NDD.Gerenciamento.Geral.Clients.DTO
{
    /// <summary>
    /// Resposta padr�o de requisi��es
    /// </summary>
    public class RespostaRequisicaoPadraoViewModel
    {
        /// <summary>
        /// Resposta do processamento da requisi��o
        /// </summary>
        [Required]
        public RespostaRequisicaoViewModel Resposta { get; set; }
    }
}
