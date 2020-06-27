using FluentValidation;
using MediatR;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestorProduto.Sincronizacao.API.Behaviours
{
    public class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<Exception, TResponse>>
        where TRequest : IRequest<Result<Exception, TResponse>>
    {
        private readonly IValidator<TRequest>[] _validators;
        private readonly string _descritivo;

        public ValidationPipeline(IValidator<TRequest>[] validators)
        {
            _validators = validators;
            _descritivo = validators.Select(validador => validador.GetType().Name).Aggregate((atual, proximo) => atual + ", " + proximo);
        }

        public async Task<Result<Exception, TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<Exception, TResponse>> next)
        {
            try
            {
                var failures = _validators
                    .Select(v => v.Validate(request))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();

                if (failures.Any())
                {
                    var erros = failures.Select(falha => falha.ToString()).Aggregate((atual, proximo) => atual + ", " + proximo);
                    return new ValidationException(failures);
                }
                return await next();
            }
            finally
            {
            }
        }
    }
}
