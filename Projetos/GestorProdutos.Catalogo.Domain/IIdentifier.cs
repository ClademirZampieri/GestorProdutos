using System;
using System.Collections.Generic;
using System.Text;

namespace GestorProdutos.Catalogo.Domain
{
    public interface IIdentifier
    {
        Guid Id { get; set; }
    }
}
