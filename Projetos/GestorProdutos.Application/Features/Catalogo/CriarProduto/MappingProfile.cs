using AutoMapper;
using GestorProdutos.Negocio.ViewModels;

namespace GestorProdutos.Application.Features.Catalogo.CriarProduto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CriarProdutoCommand, ProdutoViewModel>();
        }
    }
}
