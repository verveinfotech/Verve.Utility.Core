namespace Verve.Utility.Core.ContractResult
{
    public enum ReasonCode
    {
        UnknownError = 0,
        Success = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        NotAcceptable = 406,
        RequestTimeout = 408,
        Conflict = 409,

        InternalServerError = 500,
        BadGateway = 502,
        
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        NetworkTimeout = 511,

        WrongCredential = 801,

        //Db errors
        NotExist = 600,
        UniqueConstrainError = 601,
        ForeignKeyViolation = 602,
    }
}