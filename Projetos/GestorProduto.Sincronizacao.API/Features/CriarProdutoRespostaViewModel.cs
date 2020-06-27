using GestorProduto.Sincronizacao.API.Features.Share;
using System;

namespace GestorProduto.Sincronizacao.API.Features
{
    public class CriarProdutoRespostaViewModel
    {
        public Guid ProtocoloRequisicao { get; set; }

        public RespostaDeRequisicaoViewModel Resposta { get; set; }
    }
}
