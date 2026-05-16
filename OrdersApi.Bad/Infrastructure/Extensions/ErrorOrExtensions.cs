using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace OrdersApi.Bad.Infrastructure.Extensions
{
    public static class ErrorOrExtensions
    {
        public static IActionResult ToActionResult<T>(this ErrorOr<T> errorOr)
        {
            if (errorOr.IsError)
            {
                var primeiroErro = errorOr.Errors.First();

                var details = new ProblemDetails
                {
                    Title = primeiroErro.Code,
                    Detail = primeiroErro.Description,
                    Status = primeiroErro.Type switch
                    {
                        ErrorType.NotFound => StatusCodes.Status404NotFound,
                        ErrorType.Validation => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    }
                };

                return new ObjectResult(details) { StatusCode = details.Status };
            }

            return new OkObjectResult(errorOr.Value);
        }
    }

}
