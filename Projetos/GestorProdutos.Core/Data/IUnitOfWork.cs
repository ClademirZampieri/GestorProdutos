using System.Threading.Tasks;

namespace GestorProdutos.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}