using AutoMapper;
using GestorProdutos.Base.Respostas;

namespace GestorProdutos.Sincronizacao.API.Features.Share
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RespostaDeRequisicao, RespostaDeRequisicaoViewModel>();
        }
    }
}
