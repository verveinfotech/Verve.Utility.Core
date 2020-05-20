namespace Verve.Utility.Core.ContractResult.Messages
{
    public static class Errors
    {
        public const string AlreadyExist = "Already Exist";

        public const string UnknownError = "Unknown Error occured, please check logs for more details";

        public const string ServerTimeout = "Timeout expired before response received";

        public const string LoginExpired = "Login expired, please login again";

        public const string InvalidCredentials = "Invalid User Id or Password, Please try again";

        public const string LoginRetriesExceeded = "Login retries exceeded, please try later";

        public const string UserLocked = "This user has been locked, please wait or contact to administrator to reset it.";

        public const string DoesNotExist = " does not exist";

        public const string UnauthorizedOperation = "You are not authorized to perform this operation";
    }
}