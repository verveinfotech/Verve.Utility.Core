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