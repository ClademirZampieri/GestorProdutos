using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;

namespace GestorProduto.Sincronizacao.API.Exceptions
{
    public class ValidationFailureMapper
    {
        public List<ValidationFailure> Map(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        {
            return failures.AsQueryable().ProjectTo<ValidationFailure>().ToList();
        }
    }
}
