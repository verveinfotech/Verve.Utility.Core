using JetBrains.Annotations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Verve.Utility.Core.ContractResult
{
    [UsedImplicitly]
    public static class ResultExtension
    {
        [UsedImplicitly]
        public static IActionResult ToJsonContentResult(this Result result)
        {
            var statusCode = GetStatusCode(result);

            return statusCode == StatusCodes.Status204NoContent ? new NoContentResult() : CheckErrorAndCreateContentResult(result, statusCode);
        }

        private static IActionResult CheckErrorAndCreateContentResult(Result result, int statusCode)
        {
            return (statusCode > 299) ? CreateErrorResult(result, statusCode) : CreateContentResult(result, statusCode);
        }

        private static IActionResult CreateErrorResult(Result result, int statusCode)
        {
            var errorResult = Result.FromOtherResult(result);

            return CreateContentResult(errorResult, statusCode);
        }

        private static IActionResult CreateContentResult(Result result, int statusCode)
        {
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }

        private static int GetStatusCode(Result result)
        {
            if (result.ReasonCode == ReasonCode.UnknownError)
            {
                return StatusCodes.Status500InternalServerError;
            }

            if ((int)result.ReasonCode > 599)
            {
                return StatusCodes.Status500InternalServerError;
            }

            return (int)result.ReasonCode;
        }
    }
}