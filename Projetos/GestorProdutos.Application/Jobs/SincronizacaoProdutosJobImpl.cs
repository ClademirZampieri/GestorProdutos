using GestorProdutos.Negocio.RNA.Sincronizacao.ProcessarDados;
using GestorProdutos.Negocio.Services;
using Quartz;
using System;
using System.Threading.Tasks;

namespace GestorProdutos.Application.Jobs
{
    public class SincronizacaoProdutosJobImpl : ISincronizacaoProdutosJob
    {
        private readonly IProcessarDados _processarDados;

        public SincronizacaoProdutosJobImpl(IProcessarDados processarDados)
        {
            _processarDados = processarDados;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _processarDados.Processar();

            return Task.CompletedTask;
        }
    }
}