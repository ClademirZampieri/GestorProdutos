using GestorProdutos.Base.Exceptions;
using NDD.CentralSolucoes.Base.Estruturas;
using NDD.Gerenciamento.Geral.Clients.DTO;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;

namespace NDD.Gerenciamento.Geral.Clients.ApiCentralSolucoes
{
    public static class ResponseExtensions
    {
        public static Result<Exception, Unit> ProcessResultadoApi(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseObject = JsonConvert.DeserializeObject<RespostaRequisicaoViewModel>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (responseObject.Codigo != 1)
                    return new BusinessException(ErrorCodes.Unhandled, responseObject.Mensagem);
                else
                    return Unit.Successful;
            }
            else
            {
                var responseObject = JsonConvert.DeserializeObject<ExceptionPayload>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                return new BusinessException(ErrorCodes.Unhandled, responseObject.ErrorMessage);
            }
        }
    }
}
