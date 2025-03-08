using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;
using System.Net;

namespace Talabat.Api.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult CustomValidationErrorResponse(ActionContext context)
        {
            var errors = context.ModelState.Where(error => error.Value.Errors.Any())
                .Select(error => new ValidationError
                {
                    Field = error.Key,
                    Errors = error.Value.Errors.Select(e => e.ErrorMessage)
                });

            var response = new ValidationErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "Validation Failed"
                ,
                Errors = errors
            };

            return new BadRequestObjectResult(response);
        }
    }
}
