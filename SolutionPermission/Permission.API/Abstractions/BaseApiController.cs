using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Permission.Domain.Exceptions;
using System.Security.Claims;

namespace Permission.API.Abstractions;

[ApiController] /*
                 * Great! Since you're using the [ApiController] attribute in your ASP.NET Core RESTful API, 
                 * the framework automatically handles model validation for you. 
                 * This means you don't need to manually check ModelState.IsValid in your controller actions, 
                 * and the framework will return a 400 Bad Request response with validation errors 
                 * if the model is invalid.
                 * 
                 *  If ModelState is invalid after manually adding errors, You'll need to manually check 
                 *  the ModelState.IsValid return BadRequest with errors
                 */
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class BaseApiController : ControllerBase
{
	protected readonly IMapper _mapper;
	protected readonly ILogger<BaseApiController> _logger;

	public BaseApiController(ILogger<BaseApiController> logger, IMapper mapper)
	{
		_logger = logger;
		_mapper = mapper;
	}

	protected Guid GetCurrentUserId(string errorMsg = "Error retrieving the userId for the current user.")
		=> Guid.TryParse(HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId)
		? userId : throw new UserNotFoundException(errorMsg);

	protected void AddModelError(IEnumerable<string> errors, string key = "")
	{
		foreach (var error in errors)
		{
			AddModelError(error, key);
		}
	}

	protected void AddModelError(string error, string key = "") => ModelState.AddModelError(key, error);
}