using Apps.Sdk.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Apps.Sdk.Exceptions
{

#pragma warning disable RCS1194
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; }
        public string ReasonPhrase { get; }
        public string Request { get; }
        public HttpResponseMessage Response { get; }

        public HttpException(HttpResponseMessage httpResponse)
            : base( getMessage(httpResponse) )
        {
            this.Response = httpResponse;
            this.StatusCode = httpResponse.StatusCode;
            this.ReasonPhrase = httpResponse.ReasonPhrase;
            this.Request = $"({httpResponse.RequestMessage.Method}) {httpResponse.RequestMessage.RequestUri}";
            this.Content = AsyncHelper.RunSync( ()=>httpResponse.Content.ReadAsStringAsync());
        }

        class ErrorReturned
        {
            public string Message { get; set; }
        }

        private static string getMessage(HttpResponseMessage httpResponse)
        {
            var requestUri = $"({httpResponse.RequestMessage.Method}) {httpResponse.RequestMessage.RequestUri}";
            var httpStatusStr = $"{httpResponse.StatusCode}({httpResponse.ReasonPhrase})";
            try
            {
                var responseContent = AsyncHelper.RunSync(() => httpResponse.Content.ReadAsStringAsync());
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    return $"HttpStatus={httpStatusStr}. Request=({requestUri}). ResponseMessage={responseContent}";

                try
                {
                    var messages = JsonConvert.DeserializeObject<IEnumerable<ErrorReturned>>(responseContent);
                    if (messages?.Any() == true)
                    {
                        var message = (messages.FirstOrDefault()?.Message) ?? "No messages in response";
                        return $"HttpStatus={httpStatusStr}. Request=({requestUri}). ResponseMessage={message}";
                    }
                }
                catch
                {
                    try
                    {
                        if (responseContent.StartsWith("Message:") && responseContent.Contains("ActivityId:"))
                            responseContent = responseContent.Substring(0, responseContent.IndexOf("ActivityId:"));
                        var message = JsonConvert.DeserializeObject<ErrorReturned>(responseContent);
                        if (message?.Message != null)
                            return $"HttpStatus={httpStatusStr}. Request=({requestUri}). ResponseMessage={message.Message}";
                    }
                    catch
                    {}
                }
                return $"HttpStatus={httpStatusStr}. Request=({requestUri}). ResponseContent={responseContent}";
            }
            catch {  }
            return $"HttpStatus={httpStatusStr}.\r\nRequest =({requestUri}).";
        }
    }
}
