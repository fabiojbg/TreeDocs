namespace Apps.Sdk
{
    public enum ResultCodes
    {
        Error = -1,
        Success = 0
    }

    public interface IResult
    {
        /// <summary>
        /// Result State
        /// </summary>
        bool Success { get; set; }

        int ResultCode { get; set; }

        /// <summary>
        /// Result Message
        /// </summary>
        object ResultObj { get; }
    }

    public class Result : IResult
    {
        public bool Success
        {
            get { return ResultCode == (int)ResultCodes.Success; }
            set { if (value) ResultCode = (int)ResultCodes.Success; else ResultCode = (int)ResultCodes.Error; }
        }

        public int ResultCode { get; set; }

        public object ResultObj { get; set; }

        public static Result New(int resultCode, object resultObj)
        {
            return new Result() { ResultCode = resultCode, ResultObj = resultObj };
        }

        public static Result SuccessResult => new Result { Success = true };
        
        public static Result ErrorResult =>  new Result { Success = false };
        
        public static Result ErrorResultObject(object resultObj)
        {
            return new Result { Success = false, ResultObj = resultObj };
        }
    }
}
