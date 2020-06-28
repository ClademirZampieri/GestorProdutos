using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Concurrent;

namespace NDD.MicroServico.Base.Schedulador.Quartz
{
    public class ScheduladorProcessosFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<IJob, IServiceScope> jobs = new ConcurrentDictionary<IJob, IServiceScope>();

        public ScheduladorProcessosFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IServiceScope scope = _serviceProvider.CreateScope();
            try
            {
                IJob job = (IJob)scope.ServiceProvider.GetService(bundle.JobDetail.JobType);
                jobs.TryAdd(job, scope);
                return job;
            }
            catch (Exception)
            {
                scope.Dispose();

                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            jobs.TryRemove(job, out IServiceScope scope);
            scope?.Dispose();
        }
    }
}
