using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Apps.Sdk.Extensions;
using Apps.Sdk.Exceptions;

namespace Apps.Sdk.Middlewares
{
    public class ExceptionInterceptorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionInterceptorMiddleware> _logger;

        public ExceptionInterceptorMiddleware(RequestDelegate next, ILogger<ExceptionInterceptorMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this._next.Invoke(context);
            }
            catch (HttpException ex)
            {
                var message = "Status Code: {0}, Message: {1}";

                _logger?.LogError(ex, message, ex.StatusCode, ex.Message);

                await WriteHttpExceptionResponse(context.Response, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger?.LogWarning(ex, $"UnauthorizedAccessException: {ex.Message}");
                await WriteRequestResponse(context.Response, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                _logger?.LogWarning(ex, $"ForbiddenException: {ex.Message}");
                await WriteRequestResponse(context.Response, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ObjectNotFoundException ex)
            {
                _logger?.LogError(ex, $"ObjectNotFoundException: {ex.Message}");
                await WriteRequestResponse(context.Response, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger?.LogDebug(ex, $"Argument Exception: {ex.Message}");
                await WriteRequestResponse(context.Response, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger?.LogDebug(ex, $"Validation Exception on: {ex.Result.GetType().FullName}");
                await WriteRequestResponse(context.Response, HttpStatusCode.BadRequest, ex.Result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Unexpected Exception: {ex.Message}");
                if( EnvironmentStep.Current == BuildType.Development)
                    await WriteUnhandledExceptionResponse(context.Response,
                                                        new { Message = ex.Message, StackTrace = ex.StackTrace });
                else
                    await WriteUnhandledExceptionResponse(context.Response, 
                                                    new { Message = ex.Message });
            }
        }

        private async Task WriteUnhandledExceptionResponse(HttpResponse response, params object[] result)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            await new MemoryStream(bytes).CopyToAsync(response.Body);
        }

        private async Task WriteHttpExceptionResponse(HttpResponse response, HttpException ex)
        {
            byte[] bytes;
            response.StatusCode = (int)ex.StatusCode;

            if (EnvironmentStep.Current == BuildType.Development)
                bytes = Encoding.UTF8.GetBytes($"(HttpException) StatusCode:{ex.StatusCode}\r\nRequest:{ex.Request}\r\n{ex.Content}\r\n{ex.StackTrace}");
            else
                bytes = Encoding.UTF8.GetBytes($"(HttpException) StatusCode:{ex.StatusCode}\r\nRequest:{ex.Request}\r\n{ex.Content}");

            await new MemoryStream(bytes).CopyToAsync(response.Body);
        }

        private async Task WriteRequestResponse(HttpResponse response, HttpStatusCode httpStatusCode, string message)
        {
            byte[] bytes;
            response.StatusCode = (int)httpStatusCode;

            bytes = Encoding.UTF8.GetBytes(message);

            await new MemoryStream(bytes).CopyToAsync(response.Body);
        }

        private async Task WriteRequestResponse(HttpResponse response, HttpStatusCode httpStatusCode, object result)
        {
            response.StatusCode = (int)httpStatusCode;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            await new MemoryStream(bytes).CopyToAsync(response.Body);
        }

    }
}
