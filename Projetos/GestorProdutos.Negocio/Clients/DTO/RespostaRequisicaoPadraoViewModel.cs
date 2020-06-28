using System.ComponentModel.DataAnnotations;

namespace NDD.Gerenciamento.Geral.Clients.DTO
{
    /// <summary>
    /// Resposta padrão de requisições
    /// </summary>
    public class RespostaRequisicaoPadraoViewModel
    {
        /// <summary>
        /// Resposta do processamento da requisição
        /// </summary>
        [Required]
        public RespostaRequisicaoViewModel Resposta { get; set; }
    }
}
