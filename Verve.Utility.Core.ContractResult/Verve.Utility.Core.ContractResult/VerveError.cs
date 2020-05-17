using System;
using System.Diagnostics.CodeAnalysis;

namespace Verve.Utility.Core.ContractResult
{
    public class VerveError
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; }

        [MaybeNull]
        public string? DetailError { get; set; }

        public Exception Exception { get; set; }

        public VerveError(int errorCode, string errorMessage, string? detailError, Exception exception)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            DetailError = detailError;
            Exception = exception;
        }

#pragma warning disable 8618
        public VerveError(int code, string errorMessage)
#pragma warning restore 8618
        {
            ErrorCode = code;
            ErrorMessage = errorMessage;
        }

    }
}