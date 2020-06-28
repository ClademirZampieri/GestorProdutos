using System;

namespace NDD.MicroServico.Base.Hosting
{
    public interface IPlataformaHostLifetime
    {
        void AntesDeExecutar(HostDeAplicacao host);
        void DepoisDeExecutar(HostDeAplicacao host);
        void NaInicializacao(HostDeAplicacao host);
        void NaFinalizacao(HostDeAplicacao host);
    }
}
