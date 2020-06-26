using System;

namespace GestorProdutos.Infra.Settings.Entities
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public int TimeoutTransacaoEmSegundos { get; set; }
        public string NivelIsolamento { get; set; }
    }
}
