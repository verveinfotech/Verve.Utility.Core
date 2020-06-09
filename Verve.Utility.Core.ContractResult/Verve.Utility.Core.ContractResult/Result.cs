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

        public static Result Success() => new Result { Succeeded = true, ReasonCode = ReasonCode.Success};

        public static Result Failure(Exception exception)
            => Failure(exception.Message, exception.StackTrace?? exception.Message, ReasonCode.InternalServerError);

        public static Result Failure(string errorMessage) 
            =>  Failure(errorMessage, ReasonCode.UnknownError, null);

        public static Result Failure(string errorMessage, ReasonCode reasonCode)
            => Failure(errorMessage, reasonCode, null);
        

        public static Result Failure(string errorMessage, ReasonCode reasonCode, Exception? exception)
            => Failure(errorMessage, errorMessage, reasonCode, exception);
        
        public static Result Failure(string errorMessage, string detailErrorMessage, ReasonCode reasonCode, Exception? exception)
        {
            return new Result
            {
                ErrorMessage = errorMessage,
                ReasonCode = reasonCode,
                Exception = exception,
                DetailErrorMessage = detailErrorMessage
            };
        }
        
        public static Result Failure(string errorMessage, string detailError, ReasonCode reasonCode)
        {
            return Failure(errorMessage, reasonCode, null);
        }
    }
}
