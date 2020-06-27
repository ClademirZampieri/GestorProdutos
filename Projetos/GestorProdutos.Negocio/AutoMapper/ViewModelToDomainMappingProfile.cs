using AutoMapper;
using GestorProdutos.Catalogo.Domain;
using GestorProdutos.Negocio.ViewModels;

namespace GestorProdutos.Negocio.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProdutoViewModel, Produto>()
                .ConstructUsing(p =>
                    new Produto(p.Nome, p.Ativo,
                        p.Valor, p.DataCadastro,
                        p.Imagem));
        }
    }
}