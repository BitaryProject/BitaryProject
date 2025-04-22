using Domain.Exceptions;
using Shared.ErrorModels;
using System.Net;

namespace BitaryProject.Api.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
                    await HandleNotFoundEndPointAsync(httpContext);

            }

            catch (Exception ex)
            {
                _logger.LogError($"Something Went wrong: {ex.Message}");
                
                // Log inner exception details if present
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                    
                    // Log any further nested exceptions
                    var currentEx = ex.InnerException;
                    while (currentEx.InnerException != null)
                    {
                        currentEx = currentEx.InnerException;
                        _logger.LogError($"Nested Inner Exception: {currentEx.Message}");
                    }
                }

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleNotFoundEndPointAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ErrorMessage = $"The End Point {httpContext.Request.Path} Not Found"
            }.ToString();

            await httpContext.Response.WriteAsync(response);
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = new ErrorDetails
            {
                ErrorMessage = ex.Message
            };
            
            // Include inner exception details in the error message
            if (ex.InnerException != null)
            {
                response.ErrorMessage += $" Inner exception: {ex.InnerException.Message}";
            }

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                UnAuthorizedException=>(int)HttpStatusCode.Unauthorized,
                ValidationException validationException=>HandleValidationException(validationException,response),
                _ => (int)HttpStatusCode.InternalServerError
            };

            response.StatusCode=httpContext.Response.StatusCode;
            await httpContext.Response.WriteAsync(response.ToString());
        }

        private int HandleValidationException(ValidationException validationException, ErrorDetails response)
        {
            response.Errors=validationException.Errors;
            return (int)HttpStatusCode.BadRequest;
        }
    }
}