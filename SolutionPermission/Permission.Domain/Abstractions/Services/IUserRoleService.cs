using Permission.Domain.Entities.Account;

namespace Permission.Domain.Abstractions.Services;

public interface IUserRoleService
{
	Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
	Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role);
	Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName);
	Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId);
	Task<ApplicationRole?> GetRoleByNameAsync(string roleName);
	Task<ApplicationRole?> GetRoleLoadRelatedAsync(string roleName);
	Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
	Task<(bool Success, string[] Errors)> TestCanDeleteRoleAsync(Guid roleId);
	Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role, IEnumerable<string>? claims);
}