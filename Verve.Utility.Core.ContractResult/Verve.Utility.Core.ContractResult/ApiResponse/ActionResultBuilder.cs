using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

namespace Verve.Utility.Core.ContractResult.ApiResponse
{
    /// <summary>
    /// Class to build api response
    /// </summary>
    [UsedImplicitly]
    public class ActionResultBuilder
    {
        /// <summary>
        /// Execute the function and get result. 
        /// </summary>
        /// <typeparam name="TContent">Result entity</typeparam>
        /// <param name="func">Function to be executed</param>
        /// <returns>Returns OK result of <typeparamref name="TContent"/> if successful, otherwise appropriate status code and error message</returns>
        public static async Task<IActionResult> ExecuteAndBuildResult<TContent>(Func<Task<Result<TContent>>> func)
        {
            var result = await func.Invoke();
            switch (result.ReasonCode)
            {
                case (ReasonCode.NoContent):
                    return new NoContentResult();
                case (ReasonCode.Accepted):
                    return new AcceptedResult(location: null, result.Entity);
                case ReasonCode.Success:
                    return new OkObjectResult(result.Entity);
                 default:
                    return result.ToJsonContentResult();
            }

        }

        /// <summary>
        /// Execute the function and get result. 
        /// </summary>
        /// <typeparam name="TContent">Result enity</typeparam>
        /// <param name="func">Function to be executed</param>
        /// <returns>Returns OK if successful, otherwise appropriate status code and error message</returns>
        public static async Task<IActionResult> ExecuteAndBuildResult(Func<Task<Result>> func)
        {
            var result = await func.Invoke();
            return result.Succeeded ? new OkObjectResult(result) : result.ToJsonContentResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="jsonResultConverter"></param>
        /// <returns></returns>
        public static async Task<IActionResult> ExecuteAndBuildResult(Func<Task<Result>> func, Func<Result, Task<IActionResult>> errorResultConverter)
        {
            var result = await func.Invoke();
            return result.Succeeded ? new OkObjectResult(result) : await errorResultConverter.Invoke(result);
        }
    }
}