using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Responses;
using System.Net;
using Table_Chair_Application.Exceptions;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var statusCode = HttpStatusCode.InternalServerError; 
        ErrorResponse response;

        _logger.LogError(exception, "Xatolik yuz berdi: {Message}", exception.Message);

        switch (exception)
        {
            case NotFoundException notFound:
                statusCode = HttpStatusCode.NotFound;
                response = new ErrorResponse(notFound.Message);
                break;

            case ValidationException validation:
                statusCode = HttpStatusCode.BadRequest;
                var errorList = validation.Errors
                    .SelectMany(x => x.Value)
                    .ToList();
                response = new ValidationErrorResponse(errorList);
                break;

            case UnauthorizedAccessException unauthorized:
                statusCode = HttpStatusCode.Unauthorized;
                response = new ErrorResponse("Ruxsat yo‘q");
                break;

            case AppException appEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new ErrorResponse(appEx.Message);
                break;

            default:
                response = new ErrorResponse("Ichki server xatosi yuz berdi");
                break;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };
        context.ExceptionHandled = true;
    }
}
