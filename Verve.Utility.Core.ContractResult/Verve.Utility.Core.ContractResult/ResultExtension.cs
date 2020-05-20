using JetBrains.Annotations;
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
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                ContentType = "application/json",
                StatusCode = GetStatusCode(result)
            };
        }

        private static int GetStatusCode(Result result)
        {
            if (result.ReasonCode == ReasonCode.UnknownError)
            {
                return 500;
            }

            if ((int)result.ReasonCode > 599)
            {
                return 500;
            }

            return (int) result.ReasonCode;
        }
    }
}