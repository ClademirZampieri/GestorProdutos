using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace NDD.MicroServico.Base.Schedulador.Quartz
{
    internal class ScheduladorProcessosImpl : IScheduladorProcessos<IScheduler>
    {
        private readonly IServiceProvider _provider;
        private StdSchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public ScheduladorProcessosImpl(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }
        public void Atualizar(Action<IScheduler> atualizacaoSchedules, NameValueCollection properties = null)
        {
            Finalizar();
            InicializarSchedule(properties);
            atualizacaoSchedules(_scheduler);
        }

        public async void Finalizar()
        {
            if (_scheduler != null && _scheduler.IsStarted)
                await _scheduler.Shutdown();
            _scheduler = null;
        }

        public void Inicializar(Action<IScheduler> criacaoSchedules, NameValueCollection properties = null)
        {
            InicializarSchedule(properties);
            criacaoSchedules(_scheduler);
        }

        public async void InicializarJobStore(NameValueCollection properties)
        {
            _schedulerFactory = new StdSchedulerFactory(properties);
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();
        }

        private async void InicializarSchedule(NameValueCollection properties = null)
        {
            if (properties == null)
                properties = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
            _schedulerFactory = new StdSchedulerFactory(properties);
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new ScheduladorProcessosFactory(_provider);
            await _scheduler.Start();
        }

        public async Task<KeyValuePair<string, DateTimeOffset>[]> CapturarDataHoraDeExecucaoDosProcessos()
        {
            IReadOnlyCollection<TriggerKey> triggerKeys = await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            Dictionary<string, DateTimeOffset> values = new Dictionary<string, DateTimeOffset>();
            foreach (TriggerKey key in triggerKeys)
            {
                ITrigger trigger = await _scheduler.GetTrigger(key);
                DateTimeOffset? proximaChamada = trigger.GetNextFireTimeUtc();
                if (proximaChamada != null)
                    values.Add(key.ToString(), proximaChamada.Value);
            }
            return values.ToArray();
        }

        public void PararProcessos()
        {
            _scheduler.PauseAll();
        }
    }
}
