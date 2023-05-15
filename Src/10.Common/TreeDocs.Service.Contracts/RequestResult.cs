using Domain.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeDocs.Service.Contracts
{
    //public sealed class Notification
    //{
    //    public string Property { get; set; }
    //    public string Message { get; set; }
    //}

    //public enum RequestResultType
    //{
    //    Success = 0,
    //    InvalidRequest = -1,
    //    Unnauthorized = -2,
    //    ObjectNotFound = -3,
    //}

    public class RequestResult : RequestResult<Object>
    {
    }
}
