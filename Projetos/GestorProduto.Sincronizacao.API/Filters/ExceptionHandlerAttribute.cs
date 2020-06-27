using GestorProduto.Sincronizacao.API.Exceptions;
using GestorProdutos.Base.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GestorProduto.Sincronizacao.API.Filters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Método invocado quando ocorre uma exceção no controller
        /// </summary>
        /// <param name="context">É o contexto atual da requisição</param>
        public override void OnException(ExceptionContext context)
        {
            context.Exception = context.Exception;
            context.HttpContext.Response.StatusCode = ErrorCodes.Unhandled.GetHashCode();
            context.Result = new JsonResult(ExceptionPayload.New(context.Exception, ErrorCodes.Unhandled.GetHashCode()));
        }
    }
}
