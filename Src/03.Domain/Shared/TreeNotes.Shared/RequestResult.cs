using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Shared.Validations;

namespace Domain.Shared
{

    public enum RequestResultType
    { 
        Success = 0,
        InvalidRequest = -1,
        Unauthorized = -2,
        ObjectNotFound = -3,
        OperationError = -4
    }

    public class RequestResult<TData>
    {
        public string _Message { get; set; }
        public List<Notification> _Notifications { get; set; }       
        public RequestResultType _Result {get; set;}
        public TData _Data { get; set; }

        public RequestResult()
        {
            _Result = RequestResultType.Success;
            _Notifications = null;
            _Data = default;
            _Message = null;
        }

        public RequestResult(TData data, string message=null)
        {
            _Result = RequestResultType.Success;
            _Notifications = null;
            _Data = data;
            _Message = message;
        }

        public RequestResult(string message, RequestResultType result = RequestResultType.InvalidRequest)
        {
            _Result = result;
            _Data = default;
            _Message = message;
            _Notifications = null;
        }

        public RequestResult(IEnumerable<Notification> notifications, string message = null, RequestResultType result = RequestResultType.InvalidRequest)
        {
            if (notifications == null)
                notifications = Enumerable.Empty<Notification>();

            _Result = result;
            _Notifications = notifications.Any() ? notifications.ToList() : null;
            _Message = message ?? String.Join("\r\n", notifications.Select(x => x.Message));
            _Data = default;
        }
    }
}
