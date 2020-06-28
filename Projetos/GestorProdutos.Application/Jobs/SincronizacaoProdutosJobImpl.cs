using Quartz;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Jobs
{
    public class SincronizacaoProdutosJobImpl : ISincronizacaoProdutosJob
    {
        private readonly IProcessarRenovacaoAutomaticaLicenca _processador;

        public SincronizacaoProdutosJobImpl(IProcessarRenovacaoAutomaticaLicenca processador)
        {
            _processador = processador;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _processador.Processar();

            return Task.CompletedTask;
        }
    }
}