namespace Verve.Utility.Core.ContractResult.Messages
{
    public class Warnings
    {
        public const string LoginRetriesRemaining = "Login failed, remaining retries ";

        public const string WeakPass = "Your password is weak";
        public const string MediumStrength = "Your password strength is medium";
        public const string StrongPass = "Your password is strong";

        public const string ConsistencyWarning = " has been updated since you last retrieved it. Other changes may be overwritten";
    }
}