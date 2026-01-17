using Permission.Domain.Entities.Account;

namespace Permission.Domain.Abstractions.Services;

public interface IUserAccountService
{
	Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
	Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password);
	Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user);
	Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(Guid userId);
	Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(Guid userId);
	Task<ApplicationUser?> GetUserByEmailAsync(string email);
	Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
	Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
	Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
	Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);
	Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string newPassword);
	Task<(bool Success, string[] Errors)> TestCanDeleteUserAsync(Guid userId);
	Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
	Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user);
	Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, IEnumerable<string>? roles);
}