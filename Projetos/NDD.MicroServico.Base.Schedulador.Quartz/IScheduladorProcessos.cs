using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace GestorProdutos.Base.Schedulador.Quartz
{
    public interface IScheduladorProcessos<TScheduler>
    {
        void Inicializar(Action<TScheduler> criacaoSchedules, NameValueCollection properties = null);
        void InicializarJobStore(NameValueCollection properties);
        void Atualizar(Action<TScheduler> atualizacaoSchedules, NameValueCollection properties = null);
        void Finalizar();
        void PararProcessos();
        Task<KeyValuePair<string, DateTimeOffset>[]> CapturarDataHoraDeExecucaoDosProcessos();
    }
}
