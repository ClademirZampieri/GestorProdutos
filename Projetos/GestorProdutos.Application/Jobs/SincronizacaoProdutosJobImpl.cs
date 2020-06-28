using GestorProdutos.Negocio.Services;
using Quartz;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Jobs
{
    public class SincronizacaoProdutosJobImpl : ISincronizacaoProdutosJob
    {
        private readonly IProdutoAppService _servico;

        public SincronizacaoProdutosJobImpl(IProdutoAppService servico)
        {
            _servico = servico;
        }

        public Task Execute(IJobExecutionContext context)
        {
            // _processador.Processar();

            return Task.CompletedTask;
        }
    }
}