namespace Permission.Domain.Abstractions.Services;

public interface IDatabaseSeeder
{
	Task SeedAsync();
}