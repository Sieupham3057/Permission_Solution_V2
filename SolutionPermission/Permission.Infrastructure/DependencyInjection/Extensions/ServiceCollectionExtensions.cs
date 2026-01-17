using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Permission.Domain.Abstractions.Services;
using Permission.Domain.Entities.Account;
using Permission.Infrastructure.DependencyInjection.Options;
using Permission.Infrastructure.Seeder;
using Permission.Infrastructure.Services;

namespace Permission.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddSqlServerInfrastructure(this IServiceCollection services)
	{

		services.AddDbContext<ApplicationDbContext>((provider, builder) =>
		{
			var configuration = provider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString("ConnectionStrings")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));
		});

		// Add Identity
		services.AddIdentity<ApplicationUser, ApplicationRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		var passwordValidatorOptions =
		services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<PasswordValidatorOptions>>();

		// Configure Identity options and password complexity here
		services.Configure<IdentityOptions>(options =>
		{

			// User settings
			options.User.RequireUniqueEmail = passwordValidatorOptions.CurrentValue.RequireUniqueEmail >= 1 ? true : false;

			// Password settings
			options.Password.RequireDigit = passwordValidatorOptions.CurrentValue.RequireDigitLength >= 1 ? true : false;
			options.Password.RequiredLength = passwordValidatorOptions.CurrentValue.RequiredMinLength;
			options.Password.RequireNonAlphanumeric = passwordValidatorOptions.CurrentValue.RequireNonLetterOrDigitLength >= 1 ? true : false;
			options.Password.RequireUppercase = passwordValidatorOptions.CurrentValue.RequireUppercaseLength >= 1 ? true : false;
			options.Password.RequireLowercase = passwordValidatorOptions.CurrentValue.RequireLowercaseLength >= 1 ? true : false;
			options.Password.RequiredUniqueChars = passwordValidatorOptions.CurrentValue.RequiredUniqueChars;

			// Lockout settings
			//options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
			//options.Lockout.MaxFailedAccessAttempts = 10;
		});
	}

	public static OptionsBuilder<PasswordValidatorOptions> ConfigurePasswordValidatorOptionsInfrastructure(this IServiceCollection services, IConfigurationSection section)
		=> services
			.AddOptions<PasswordValidatorOptions>()
			.Bind(section)
			.ValidateDataAnnotations()
			.ValidateOnStart();

	public static void AddServicesInfrastructure(this IServiceCollection services)
	{
		services.AddTransient<IUserAccountService, UserAccountService>();
		services.AddTransient<IUserRoleService, UserRoleService>();
		services.AddTransient<IUserIdAccessor, UserIdAccessor>();
		services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
	}
}