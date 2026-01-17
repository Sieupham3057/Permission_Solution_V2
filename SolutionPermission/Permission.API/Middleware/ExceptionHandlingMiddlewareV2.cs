using Permission.Domain.Exceptions.Abstractions;
using Permission.Domain.Exceptions;
using System.Text.Json;

namespace Permission.API.Middleware;

internal sealed class ExceptionHandlingMiddlewareV2
{
	private readonly ILogger<ExceptionHandlingMiddlewareV2> _logger;
	private readonly RequestDelegate _next;

	public ExceptionHandlingMiddlewareV2(ILogger<ExceptionHandlingMiddlewareV2> logger, RequestDelegate next)
	{
		_logger = logger;
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception e)
		{
			_logger.LogError(e, e.Message);

			await HandleExceptionAsync(context, e);
		}
	}

	private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
	{
		var statusCode = GetStatusCode(exception);

		var response = new
		{
			title = GetTitle(exception),
			status = statusCode,
			detail = exception.Message,
			errors = GetErrors(exception),
		};

		httpContext.Response.ContentType = "application/json";

		httpContext.Response.StatusCode = statusCode;

		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
	}

	private static int GetStatusCode(Exception exception) =>
		exception switch
		{
			BadRequestException => StatusCodes.Status400BadRequest,
			UserAccountException => StatusCodes.Status400BadRequest,
			NotFoundException => StatusCodes.Status404NotFound,
			ForbiddenException => StatusCodes.Status403Forbidden,
			_ => StatusCodes.Status500InternalServerError
		};

	private static string GetTitle(Exception exception) =>
		exception switch
		{
			DomainException applicationException => applicationException.Title,
			_ => "Server Error"
		};

	private static IReadOnlyCollection<ValidationError> GetErrors(Exception exception)
	{
		IReadOnlyCollection<ValidationError> errors = null;

		if (exception is ValidationException validationException)
		{
			errors = validationException.Errors;
		}

		return errors;
	}

}
