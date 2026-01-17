using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Permission.API.Attributes;
using Permission.API.DependencyInjection.Options;
using Permission.Domain.Permissions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Permission.API.DependencyInjection.Extensions;

public static class JwtExtensions
{
	public static void AddJwtAuthenticationAPI(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		//.AddCookie(options =>
		//{
		//	//options.LoginPath = "/Account/Login"; // Avoid redirection to login page
		//	//options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page

		//	//options.Events.OnRedirectToLogin = context =>
		//	//{
		//	//	// Prevent the redirect to login page for unauthorized requests
		//	//	context.Response.ContentType = "a/html";
		//	//	context.Response.WriteAsync("Access Denied. Access forbidden.");
		//	//	context.Response.StatusCode = StatusCodes.Status403Forbidden;
		//	//	return Task.CompletedTask;
		//	//};
		//})
		.AddJwtBearer(options =>
		{
			// Bind configuration to JwtOption
			var jwtOption = configuration.GetSection(nameof(JwtOption)).Get<JwtOption>()
				?? throw new Exception("JWT options are not properly configured.");

			options.SaveToken = true; // Save token in AuthenticationProperties

			var key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = !env.IsDevelopment(),
				ValidateAudience = !env.IsDevelopment(),
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtOption.Issuer,
				ValidAudience = jwtOption.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			};

			// Custom events for handling errors and token validation
			options.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					if (context.Exception is SecurityTokenExpiredException)
					{
						context.Response?.Headers?.Add("IS-TOKEN-EXPIRED", "True");
					}
					return Task.CompletedTask;
				}
			};

			// Set a custom event type
			options.EventsType = typeof(CustomJwtBearerEvents);
		});

		services.AddAuthorization(options =>
		{
			options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
			{
				policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireAuthenticatedUser();
			});
		});

		services.AddScoped<CustomJwtBearerEvents>();
	}
}
