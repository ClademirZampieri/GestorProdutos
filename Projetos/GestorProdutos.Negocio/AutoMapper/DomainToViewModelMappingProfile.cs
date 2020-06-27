using AutoMapper;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Negocio.ViewModels;

namespace GestorProdutos.Negocio.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Produto, ProdutoViewModel>();
        }
    }
}
