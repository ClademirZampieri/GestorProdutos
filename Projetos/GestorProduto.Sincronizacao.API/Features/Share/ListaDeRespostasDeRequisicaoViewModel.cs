using System.Collections.Generic;

namespace GestorProdutos.Sincronizacao.API.Features.Share
{
    /// <summary>
    /// Resposta padrao a uma requisicao realizada a Api de servicos
    /// </summary>
    public class ListaDeRespostasDeRequisicaoViewModel
    {
        /// <summary>
        /// Colecao de respostas dadas pelos adaptadores
        /// </summary>
        public List<RespostaDeRequisicaoViewModel> Respostas { get; set; } = new List<RespostaDeRequisicaoViewModel>();
    }
}
