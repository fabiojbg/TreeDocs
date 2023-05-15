using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Sdk.Middlewares
{
    public static class SdkMiddlewareExtensions
    {
        public static IApplicationBuilder UseSdkRequestLanguageDetection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLanguageMiddleware>();
        }

        public static IApplicationBuilder UseSdkExceptionInterceptor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionInterceptorMiddleware>();
        }

    }
}
