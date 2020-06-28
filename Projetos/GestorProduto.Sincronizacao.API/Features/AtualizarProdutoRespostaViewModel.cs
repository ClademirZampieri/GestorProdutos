using GestorProdutos.Sincronizacao.API.Features.Share;
using System;

namespace GestorProdutos.Sincronizacao.API.Features
{
    public class AtualizarProdutoRespostaViewModel
    {
        public Guid ProtocoloRequisicao { get; set; }

        public RespostaDeRequisicaoViewModel Resposta { get; set; }
    }
}
