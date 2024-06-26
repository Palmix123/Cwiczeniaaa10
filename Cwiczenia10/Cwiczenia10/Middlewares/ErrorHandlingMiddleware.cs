using System.Net;

namespace Cwiczenia10.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate Next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);
        }
        catch(UnauthorizedAccessException e)
        {
            await HandleExceptionAsync(context, e);
        }
    }
    private Task HandleExceptionAsync(HttpContext context, UnauthorizedAccessException e)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        var response = new
        {
            error = "Error in request",
            message = e.Message
        };
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}