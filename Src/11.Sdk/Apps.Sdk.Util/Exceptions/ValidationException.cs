using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Sdk.Exceptions
{
    public class ValidationException : Exception
    {
        public object Result { get; set; }
        public ValidationException(string message, object result) : base(message)
        {
            Result = result;
        }
    }
}
