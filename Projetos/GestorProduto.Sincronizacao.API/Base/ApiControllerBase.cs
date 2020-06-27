using AutoMapper;
using FluentValidation;
using GestorProduto.Sincronizacao.API.Exceptions;
using GestorProdutos.Base.Exceptions;
using GestorProdutos.Infra.Csv;
using GestorProdutos.Infra.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using NDD.CentralSolucoes.Base.Estruturas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GestorProduto.Sincronizacao.API.Base
{
    public class ApiControllerBase : ControllerBase
    {
        #region Handlers

        /// <summary>
        /// Retorna o Id do usário que está fazendo a chamada.
        /// </summary>
        protected int UserId
        {
            get
            {
                var claim = Convert.ToInt32(GetClaimValue("UserId"));

                return claim;
            }
        }

        #region Handlers    

        /// <summary>
        /// Manuseia o result. Valida se é necessário retornar erro ou o próprio TSuccess
        /// </summary> 
        /// <typeparam name="TFailure"></typeparam>
        /// <typeparam name="TSuccess"></typeparam>
        /// <param name="result">Objeto Result utilizado nas chamadas.</param>
        /// <returns></returns>
        protected IActionResult HandleCommand<TFailure, TSuccess>
            (Result<TFailure, TSuccess> result) where TFailure : Exception
        {
            return result.IsFailure ? HandleFailure(result.Failure) : Ok(result.Success);
        }

        /// <summary>
        /// Manuseia a query para aplicar as opções do odata.
        ///
        /// Esse método vai gerar o PageResult associando os dados (query) com as opções do odata (queryOptions) 
        /// após isso ele monta a resposta HTTP solicitada, conforme headers.
        /// 
        /// </summary>
        /// <typeparam name="TQueryOptions">Tipo do obj de origem (domínio)</typeparam>
        /// <typeparam name="TResult">Tipo de retorno </typeparam>
        /// <param name="result">Objeto Result retornado pelas chamadas.(TQueryOptions)</param>
        /// <param name="queryOptions">OdataQueryOptions(TQueryOptions)</param>
        /// <returns>IActionResult(TResult) com o resultado da operação</returns>
        protected async Task<IActionResult> HandleQueryable<TQueryOptions, TResult>(
                Result<Exception, IQueryable<TQueryOptions>> result,
                ODataQueryOptions<TQueryOptions> queryOptions)
        {
            if (result.IsFailure)
                return HandleFailure(result.Failure);

            var mediaType = MediaTypeWithQualityHeaderValue.Parse("text/csv").MediaType;
            var accept = Request.Headers["Accept"].ToString();

            if (accept.Contains(mediaType))
                return Accepted(await HandleCSVFile<TQueryOptions, TResult>(result.Success, queryOptions));

            return Ok(await HandlePageResult<TQueryOptions, TResult>(result.Success, queryOptions));
        }

        /// <summary>
        /// Aplica o filtro (odata) a query e retora um pageresult criado através do project da TQueryOptions para TResult. 
        /// </summary>
        /// <typeparam name="TQueryOptions">Tipo do obj de origem (domínio)</typeparam>
        /// <typeparam name="TResult">Tipo de retorno objQuery</typeparam>
        /// <param name="query">IQueryable(TQueryOptions)</param>
        /// <param name="queryOptions">OdataQueryOptions</param>
        /// <returns>PageResult(TResult)</returns>
        protected async Task<PageResult<TResult>> HandlePageResult<TQueryOptions, TResult>
            (IQueryable<TQueryOptions> query, ODataQueryOptions<TQueryOptions> queryOptions)
        {
            var queryResults = queryOptions.ApplyTo(query);
            var list = await queryResults.ProjectToListAsync<TResult>();
            var pageResult = new PageResult<TResult>(list,
                                                    Request.HttpContext.ODataFeature().NextLink,
                                                    Request.HttpContext.ODataFeature().TotalCount);
            return pageResult;
        }

        /// <summary>
        /// Verifica a exceção passada por parametro para passar o StatusCode correto para o frontend.
        /// </summary>
        /// <typeparam name="T">Qualquer classe que herde de Exeption</typeparam>
        /// <param name="exceptionToHandle">obj de exceção</param>
        /// <returns></returns>
        protected IActionResult HandleFailure<T>(T exceptionToHandle) where T : Exception
        {
            HttpStatusCode code;
            ExceptionPayload payload;

            if (exceptionToHandle is BusinessException)
            {
                code = HttpStatusCode.BadRequest;
                payload = ExceptionPayload.New(exceptionToHandle, (exceptionToHandle as BusinessException).ErrorCode.GetHashCode());
            }
            else if (exceptionToHandle is ValidationException)
            {
                code = HttpStatusCode.BadRequest;
                payload = ExceptionPayload.New(
                                exceptionToHandle,
                                ErrorCodes.BadRequest.GetHashCode(),
                                new ValidationFailureMapper().Map((exceptionToHandle as ValidationException).Errors));
            }
            else
            {
                code = HttpStatusCode.InternalServerError;
                payload = ExceptionPayload.New(exceptionToHandle, ErrorCodes.Unhandled.GetHashCode());
            }

            return StatusCode(code.GetHashCode(), payload);
        }

        /// <summary>
        /// Retorna IHttpStatusCode de erro + erros da validação.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validationFailure">Erros de validação (ValidationFailure)</param>
        /// <returns>IActionResult com os erros e status code padrão</returns>
        protected IActionResult HandleValidationFailure<T>(IList<T> validationFailure) where T : FluentValidation.Results.ValidationFailure
        {
            return StatusCode(HttpStatusCode.BadRequest.GetHashCode(), validationFailure);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Método responsável por ler do token, no contexto da requisição, as claims codificadas.
        /// </summary>
        /// <param name="type">É o nome da claim (atributo) desejada para leitura no token</param>
        /// <returns>O valor correspondente a claim</returns>
        private string GetClaimValue(string type)
        {
            return ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(type).Value;
        }

        /// <summary>
        /// Manuseia o result. Verifica se a resposta é uma falha ou sucesso, retornando os dados apropriados. 
        /// É importante destacar que este método realiza o mapeamento da classe TSource em TResult
        /// </summary> 
        /// <typeparam name="TSource">Classe de origem (ex.: domínio)</typeparam>
        /// <typeparam name="TResult">ViewModel</typeparam>
        /// <param name="result">Objeto Result retornado pelas chamadas.</param>
        /// <returns>Resposta apropriada baseado no result enviado como parâmetro</returns>
        protected IActionResult HandleQuery<TSource, TResult>(Result<Exception, TSource> result)
        {
            return result.IsSuccess ? Ok(Mapper.Map<TSource, TResult>(result.Success)) : HandleFailure(result.Failure);
        }
        #endregion

        /// <summary>
        /// Aplica o filtro (odata) a query, monta um HttpResultMessage com o arquivo CSV
        /// </summary>
        /// <typeparam name="TQueryOptions">Tipo do obj de origem (domínio)</typeparam>
        /// <typeparam name="TResult">Tipo de retorno objQuery</typeparam>
        /// <param name="query">IQueryable(TQueryOptions)</param>
        /// <param name="queryOptions">OdataQueryOptions</param>
        /// <returns>HttpResponseMessage</returns>
        private async Task<HttpResponseMessage> HandleCSVFile<TQueryOptions, TResult>(IQueryable<TQueryOptions> query, ODataQueryOptions<TQueryOptions> queryOptions)
        {
            var queryResults = queryOptions.ApplyTo(query);
            var project = await queryResults.ProjectToListAsync<TResult>();

            var csv = project.ToCsv<TResult>(";");
            var bytes = Encoding.UTF8.GetBytes(csv ?? "");
            var stream = new MemoryStream(bytes, 0, bytes.Length, false, true);

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(stream.GetBuffer()) };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("export{0}.csv", DateTime.UtcNow.ToString("yyyyMMddhhmmss"))
            };

            return result;
        }
        #endregion
    }
}
