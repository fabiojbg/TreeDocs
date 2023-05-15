using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Apps.Sdk.Extensions
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Indica que a operação foi executada com sucesso. 
        /// Operaões validadas: (OK, Accepted, Created, NoContent)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool Success( this HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.OK ||
                  response.StatusCode == HttpStatusCode.Accepted ||
                  response.StatusCode == HttpStatusCode.Created ||
                  response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Indica que a aplicação retornou um erro na operação.
        /// Resubmeter a mesma operação irá provavelmente falhar novamente.
        /// Operações validadas: (BadRequest, Unauthorized, Forbidden, NotImplemented, PreconditionFailed, NotFound, InternalServerError, 
        /// ExpectationFailed, MethodNotAllowed, NotAcceptable, RequestUriTooLong, UnsupportedMediaType )
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool IsApplicationError(this HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.BadRequest ||
                  response.StatusCode == HttpStatusCode.Unauthorized ||
                  response.StatusCode == HttpStatusCode.Forbidden ||
                  response.StatusCode == HttpStatusCode.NotImplemented ||
                  response.StatusCode == HttpStatusCode.PreconditionFailed ||
                  response.StatusCode == HttpStatusCode.NotFound ||
                  response.StatusCode == HttpStatusCode.InternalServerError ||
                  response.StatusCode == HttpStatusCode.ExpectationFailed ||
                  response.StatusCode == HttpStatusCode.MethodNotAllowed ||
                  response.StatusCode == HttpStatusCode.NotAcceptable ||
                  response.StatusCode == HttpStatusCode.RequestUriTooLong ||
                  response.StatusCode == HttpStatusCode.UnsupportedMediaType ||
                  response.StatusCode == HttpStatusCode.LengthRequired || 
                  response.StatusCode == HttpStatusCode.Ambiguous ||
                  response.StatusCode == HttpStatusCode.HttpVersionNotSupported;
        }

        /// <summary>
        /// Erros que representam inaccessibilidade temporária do servidor
        /// Operações validadas: (RequestTimeout, ServiceUnavailable)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool IsTemporaryServerError(this HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.RequestTimeout ||
                   response.StatusCode == HttpStatusCode.ServiceUnavailable;
        }

        public static bool IsAccessDenied(this HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized ||
                   response.StatusCode == HttpStatusCode.Forbidden;
        }

    }
}
