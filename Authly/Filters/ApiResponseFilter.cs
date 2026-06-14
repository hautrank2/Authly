using Authly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Authly.Filters
{
    /// <summary>
    /// Tự động wrap mọi IActionResult thành ApiResponse<T>.
    /// Controller chỉ cần return Ok(data) hoặc BadRequest(...).
    /// </summary>
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Validate model binding errors (DataAnnotations)
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                context.Result = new BadRequestObjectResult(
                    ApiResponse.Fail(errors, "Validation failed"));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                // Không wrap nếu đã là ApiResponse
                if (objectResult.Value is ApiResponse<object>)
                    return;

                var isSuccess = objectResult.StatusCode is null or >= 200 and < 300;

                if (isSuccess)
                {
                    objectResult.Value = new ApiResponse<object>
                    {
                        Success = true,
                        Data = objectResult.Value,
                    };
                }
                else
                {
                    var errors = objectResult.Value switch
                    {
                        string s => [s],
                        IEnumerable<string> list => list.ToList(),
                        _ => new List<string> { objectResult.Value?.ToString() ?? "An error occurred" }
                    };

                    objectResult.Value = ApiResponse.Fail(errors);
                }
            }
        }
    }
}
