using GestorProdutos.Application.Jobs;
using GestorProdutos.Infra.Configuracoes;
using Quartz;

namespace GestorProdutos.Application.Schedules
{
    public class ScheduleSincronizacaoProdutos
    {
        private const string TriggerId = "Trigger_SincronizacaoProdutos";
        private const string JobId = "Job_SincronizacaoProdutos";
        private readonly GestorProdutosConfiguracoes _configuracoes;

        public ScheduleSincronizacaoProdutos(GestorProdutosConfiguracoes configuracoes)
        {
            _configuracoes = configuracoes;
        }

        public void Inicializacao(IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create<ISincronizacaoProdutosJob>()
                .WithIdentity(JobId)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(TriggerId)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_configuracoes.ParametorsDeSincronizacao.IntervaloExecuacaoEmSegundos)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}