using GestorProdutos.Base.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestorProdutos.Base.Estruturas;
using GestorProdutos.Sincronizacao.API.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestorProdutos.Sincronizacao.API.Features.Share
{
    public class ApiBaseExecucaoControlador : ApiControllerBase
    {
        protected readonly IMediator _mediator;

        public ApiBaseExecucaoControlador(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Executar<TCommand, TResponse, TResponseView>(
            TCommand requisicao,
            Func<string> funcaoErro,
            Func<Task<Result<Exception, TResponse>>> executarAcaoMediador)
        {
            IActionResult resultadoDaAcaoControlador = null;

            try
            {
                if (RequisicaoInvalida(requisicao))
                {
                    string errosEspecificos = CapturarErrosEspecificos();

                    var businessException = new BusinessException(ErrorCodes.InvalidObject, funcaoErro() + " Erros: " + errosEspecificos);

                    resultadoDaAcaoControlador = HandleFailure<BusinessException>(businessException);
                }
                else
                {
                    try
                    {
                        var resultado = await executarAcaoMediador();

                        if (resultado.IsSuccess)
                            resultadoDaAcaoControlador = HandleQuery<TResponse, TResponseView>(resultado.Success);
                        else
                            resultadoDaAcaoControlador = HandleFailure<Exception>(resultado.Failure);
                    }
                    catch (Exception e)
                    {
                        resultadoDaAcaoControlador = HandleFailure<Exception>(e);
                    }
                }
                return resultadoDaAcaoControlador;
            }
            finally
            {
            }
        }

        private string CapturarErrosEspecificos()
        {
            if (this.ModelState.Values.Count() == 0)
                return "Não foi possivel fornecer mensagem mais detalhada";
            var listaErros = this.ModelState.Values.Select(campo =>
            {
                return campo.Errors
                    .Select(erro => string.IsNullOrEmpty(erro.ErrorMessage) ? erro.Exception.Message : erro.ErrorMessage)
                    .Aggregate(String.Empty, (erroAtual, erroProximo) => $"{erroAtual}[Mensagem: {erroProximo}]");
            });
            return listaErros.Count() == 1 ?
                listaErros.First() :
                listaErros.Aggregate(String.Empty, (atual, proximo) => String.IsNullOrEmpty(atual) ? proximo : atual + ", " + proximo);
        }

        private bool RequisicaoInvalida(object requisicao)
        {
            return requisicao == null;
        }
    }
}
