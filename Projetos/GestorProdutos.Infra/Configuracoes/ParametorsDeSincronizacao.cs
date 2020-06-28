using System;
using System.Collections.Generic;
using System.Text;

namespace GestorProdutos.Infra.Configuracoes
{
    public class ParametorsDeSincronizacao
    {
        public string UrlWebAPI { get; set; }

        public int IntervaloExecuacaoEmSegundos { get; set; }
    }
}
