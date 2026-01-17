using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Permission.API.Abstractions;
using Permission.API.DependencyInjection.Options;
using Permission.API.Models;
using Permission.Domain.Abstractions.Services;
using Permission.Domain.Constances;
using Permission.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Permission.API.Controllers.V1;

public class AuthorizationController : BaseApiController
{
	private readonly JwtOption jwtOption = new JwtOption();
	private readonly IUserAccountService _userAccountService;
	private readonly IUserRoleService _userRoleService;

	public AuthorizationController(ILogger<AuthorizationController> logger, IMapper mapper,
		IConfiguration configuration, IUserAccountService userAccountService, IUserRoleService userRoleService) : base(logger, mapper)
	{
		configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);
		_userAccountService = userAccountService;
		_userRoleService = userRoleService;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
	{

		var user = await _userAccountService.GetUserByUserNameAsync(loginRequest.UserName)
			?? throw new UserNotFoundException();

		var checkPasswrod = await _userAccountService.CheckPasswordAsync(user, loginRequest.Password);
		if (!checkPasswrod)
			throw new UserAccountException("Password is not correct.");

		var roleNames = await _userAccountService.GetUserRolesAsync(user);

		if (roleNames.Count() == 0)
			throw new UserAccountException("Invalid user. User must have at least one role.");

		var permissions = new List<string>();

		foreach (var roleName in roleNames)
		{
			var roleClaims = await _userRoleService.GetRoleLoadRelatedAsync(roleName);
			permissions.AddRange(roleClaims?.Claims.Where(x => x.ClaimType == CustomClaims.Permission).Select(x => x.ClaimValue).ToList());
		}

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Name, user.FullName),
		};

		// Add each roleName as a separate claim
		claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
		// Add each permission as a separate claim
		claims.AddRange(permissions.Select(permission => new Claim(CustomClaims.Permission, permission)));

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
		var tokenDescriptor = new JwtSecurityToken(
		   issuer: jwtOption.Issuer,
		   audience: jwtOption.Audience,
		   claims: claims,
		   expires: DateTime.Now.AddMinutes(120),
		   signingCredentials: credentials);

		var result = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

		var resultResponse = new
		{
			AccessToken = result
		};

		return Ok(resultResponse);
	}

	[HttpPost("refreshLogin")]
	public async Task<IActionResult> RefreshLogin()
	{
		var userId = GetCurrentUserId($"Error retrieving the userId.");
		var user = await _userAccountService.GetUserByIdAsync(userId)
			?? throw new UserNotFoundException();

		var roleNames = await _userAccountService.GetUserRolesAsync(user);

		if (roleNames.Count() == 0)
			throw new UserAccountException("Invalid user. User must have at least one role.");

		var permissions = new List<string>();

		foreach (var roleName in roleNames)
		{
			var roleClaims = await _userRoleService.GetRoleLoadRelatedAsync(roleName);
			permissions.AddRange(roleClaims?.Claims.Where(x => x.ClaimType == CustomClaims.Permission).Select(x => x.ClaimValue).ToList());
		}

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Name, user.FullName),
		};

		// Add each roleName as a separate claim
		claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
		// Add each permission as a separate claim
		claims.AddRange(permissions.Select(permission => new Claim(CustomClaims.Permission, permission)));

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
		var tokenDescriptor = new JwtSecurityToken(
		   issuer: jwtOption.Issuer,
		   audience: jwtOption.Audience,
		   claims: claims,
		   expires: DateTime.Now.AddMinutes(120),
		   signingCredentials: credentials);

		var result = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

		var resultResponse = new
		{
			AccessToken = result
		};

		return Ok(resultResponse);

	}
}
