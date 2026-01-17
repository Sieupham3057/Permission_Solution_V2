using Microsoft.AspNetCore.Authorization;
using Permission.API.Authorization;
using Permission.API.Authorization.Requirements;
using Permission.API.DependencyInjection.Extensions;
using Permission.API.Middleware;
using Permission.Domain.Abstractions.Services;
using Permission.Domain.Constances;
using Permission.Domain.Permissions;
using Permission.Infrastructure.DependencyInjection.Extensions;
using Permission.Infrastructure.DependencyInjection.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddApiVersioningAndDocumentationAPI(); // Adds API versioning and OpenAPI
builder.Services.AddJwtAuthenticationAPI(builder.Configuration, builder.Environment);

builder.Services.AddSqlServerInfrastructure(); 
builder.Services.ConfigurePasswordValidatorOptionsInfrastructure(builder.Configuration.GetSection(nameof(PasswordValidatorOptions)));
builder.Services.AddServicesInfrastructure();

builder.Services.AddAuthorizationBuilder()
	.AddPolicy(AuthPolicies.ViewAllUsersPolicy,
		policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewUsers))
	.AddPolicy(AuthPolicies.ManageAllUsersPolicy,
		policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageUsers))
	.AddPolicy(AuthPolicies.ViewAllRolesPolicy,
		policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewRoles))
	.AddPolicy(AuthPolicies.ViewRoleByRoleNamePolicy,
		policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()))
	.AddPolicy(AuthPolicies.ManageAllRolesPolicy,
		policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageRoles))
	.AddPolicy(AuthPolicies.AssignAllowedRolesPolicy,
		policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));

// Auth Handlers
builder.Services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();

// Add Middleware => Remember using middleware
//builder.Services.AddTransient<ExceptionHandlingMiddlewareV1>();

//跨域设置
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		policy => policy.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader());

});
var app = builder.Build();

// Using middleware
//app.UseMiddleware<ExceptionHandlingMiddlewareV1>();
app.UseMiddleware<ExceptionHandlingMiddlewareV2>();  // Register custom exception handling middleware


// Configure the HTTP request pipeline.
//app.UseOpenApiUIAPI();

app.MapOpenApi();

Action<ScalarOptions> configureOptions = options =>
	options
		.WithCdnUrl("https://cdn.jsdelivr.net/npm/@scalar/api-reference")
		.WithFavicon("/favicon.png")
		//.WithPreferredScheme(AuthConstants.ApiKey)
		//.WithApiKeyAuthentication(x => x.Token = "my-api-key")
		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

app.MapScalarApiReference((options, context) =>
{
	configureOptions.Invoke(options);
	options.Title = context.Request.Path;
	options.WithTheme(ScalarTheme.Mars);
});

app.MapScalarApiReference("/", configureOptions);

app.MapScalarApiReference("/scalar-url-pattern", (options, context) =>
{
	configureOptions.Invoke(options);
	options.OpenApiRoutePattern = $"{context.Request.Scheme}://{context.Request.Host}/openapi/{{documentName}}.json";
});

//app.UseHttpsRedirection(); // In case you want to convert from Https to Http and vice versa on Scalar UI

// Enable CORS
app.UseCors("AllowAll");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

// Redirect root to Scalar UI
app.MapGet("/", () => Results.Redirect("/scalar/v1"))
   .ExcludeFromDescription();

app.MapControllers();

/************* SEED DATABASE *************/

using var scope = app.Services.CreateScope();
try
{
	var dbSeeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
	await dbSeeder.SeedAsync();
}
catch (Exception ex)
{
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
	logger.LogCritical(ex, "An error occurred whilst creating/seeding database");

	throw;
}

await app.RunAsync();
