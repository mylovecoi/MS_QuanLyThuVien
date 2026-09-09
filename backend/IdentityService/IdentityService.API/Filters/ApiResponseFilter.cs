using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityService.API.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var formattedResult = new
                {
                    success = true,
                    statusCode = objectResult.StatusCode ?? 200,
                    data = objectResult.Value
                };
                context.Result = new ObjectResult(formattedResult) { StatusCode = objectResult.StatusCode };
            }
        }
    }
}
