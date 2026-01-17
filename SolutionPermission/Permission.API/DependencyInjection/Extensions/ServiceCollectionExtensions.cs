using Microsoft.AspNetCore.Authentication;
using Permission.API.DependencyInjection.Options;
using Permission.API.Attributes;
using Scalar.AspNetCore;
using Asp.Versioning.ApiExplorer;
using APIWeaver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Asp.Versioning;

namespace Permission.API.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
	internal static void AddApiVersioningAndDocumentationAPI(this IServiceCollection services)
	{
		//GroupNameFormat = "'v'V" uses:
		//'v' for literal prefix
		//'V' = Major version only
		//'VVV' = Major + Minor + Patch

		// Enable API versioning
		services.AddApiVersioning(options =>
		{
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.ReportApiVersions = true;
			options.ApiVersionReader = new UrlSegmentApiVersionReader(); // 👈 This is key!
		})
		.AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV"; // The format for versioning in URLs == Creates groups like "v1", "v2"
			options.SubstituteApiVersionInUrl = true; // Add version to URL (e.g., /v1.0)
		});

		services.AddControllers();
		services.AddEndpointsApiExplorer(); // Add API Explorer for versioned APIs

		// Get version provider from DI container
		var serviceProvider = services.BuildServiceProvider();
		var versionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

		//string[] versions = ["v1", "v2"];

		//foreach (var version in versions)
		foreach (var description in versionProvider.ApiVersionDescriptions)
		{
			var version = description.GroupName.ToString(); 
			services.Configure<ScalarOptions>(options => options.AddDocument(version, $"Version {version}"));
			services.AddOpenApi(version, options =>
			{
				options.AddDocumentTransformer( async (document, context, _) =>
				{
					var descriptionProvider = context.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
					var versionDescription = descriptionProvider.ApiVersionDescriptions.FirstOrDefault(x => x.GroupName == version);
					document.Info.Version = versionDescription?.ApiVersion.ToString();
				});
				options.AddDocumentTransformer((document, _) => document.Servers = []);

				options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
			});
		}
	}

}

internal sealed class BearerSecuritySchemeTransformer(
	IAuthenticationSchemeProvider authenticationSchemeProvider
		) : IOpenApiDocumentTransformer
{
	public async Task TransformAsync(
		OpenApiDocument document,
		OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
		if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
		{
			var requirements = new Dictionary<string, OpenApiSecurityScheme>
			{
				[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					Scheme = JwtBearerDefaults.AuthenticationScheme, // "bearer" refers to the header name here
					In = ParameterLocation.Header,
					BearerFormat = "Json Web Token",
				},
			};
			document.Components ??= new OpenApiComponents();
			document.Components.SecuritySchemes = requirements;
		}
	}
}


