using AutoMapper;
using GestorProdutos.Negocio.ViewModels;

namespace GestorProdutos.Application.Features.Catalogo.AtualizarProduto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AtualizarProdutoCommand, ProdutoViewModel>();
        }
    }
}
