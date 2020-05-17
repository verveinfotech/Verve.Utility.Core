using System;

namespace Verve.Utility.Core.ContractResult
{
    public class Result
    {
        protected Result()
        {

        }

        public Result(bool success, ReasonCode reason, string? userFriendlyMessage, string? developerFriendlyMessage)
        {
            Succeeded = success;
            ReasonCode = reason;
            ErrorMessage = userFriendlyMessage;
            DetailErrorMessage = developerFriendlyMessage;
        }

        public Result(string errorMessage, ReasonCode reasonCode)
        {
            ErrorMessage = errorMessage;
            Succeeded = false;
            ReasonCode = reasonCode;
        }

        public bool Succeeded { get; protected set; }
        public string? ErrorMessage { get; protected set; }
        public  string? DetailErrorMessage { get; protected set; }
        public ReasonCode ReasonCode { get; set; }
        public Exception? Exception { get; protected set; }

        public bool Failed => !Succeeded;

        public static Result Success => new Result { Succeeded = true, ReasonCode = ReasonCode.Success};
        
        public static Result Failure(string errorMessage, Exception exception)
        {
            return Failure(errorMessage, ReasonCode.UnknownError, exception);
        }

        public static Result Failure(string errorMessage, ReasonCode reasonCode, Exception exception)
        {
            return new Result
            {
                ErrorMessage = errorMessage,
                ReasonCode = reasonCode,
                Exception = exception
            };
        }

    }
}
