using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.WebApi.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        this.logger.LogError(context.Exception, "Unhandled exception occurred while processing the request.");

        context.Result = new ObjectResult("An error occurred while processing your request.")
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };

        context.ExceptionHandled = true;
    }
}
