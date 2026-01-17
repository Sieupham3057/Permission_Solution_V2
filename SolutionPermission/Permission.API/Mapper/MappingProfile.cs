using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Permission.API.Models;
using Permission.Domain.Entities.Account;
using Permission.Domain.Permissions;

namespace Permission.API.Mapper;

public class MappingProfile : Profile
{
	public MappingProfile()
	{

		// ============= ApplicationUser =============
		CreateMap<ApplicationUser, UserVM>()
			   .ForMember(d => d.Roles, map => map.Ignore());

		CreateMap<UserVM, ApplicationUser>()
			.ForMember(d => d.Roles, map => map.Ignore())
			.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

		CreateMap<ApplicationUser, UserEditVM>()
			.ForMember(d => d.Roles, map => map.Ignore());

		CreateMap<UserEditVM, ApplicationUser>()
			.ForMember(d => d.Roles, map => map.Ignore())
			.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

		CreateMap<ApplicationUser, UserPatchVM>()
			.ReverseMap();

		// ============= ApplicationRole =============
		// Map individual ApplicationRole to RoleVM
		CreateMap<ApplicationRole, RoleVM>()
			.ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
			.ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users != null ? s.Users.Count : 0))
			.ReverseMap();

		CreateMap<RoleVM, ApplicationRole>()
			.ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

		CreateMap<IdentityRoleClaim<Guid>, ClaimVM>()
			.ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
			.ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
			.ReverseMap();

		CreateMap<ApplicationPermission, PermissionVM>()
			.ReverseMap();

		CreateMap<IdentityRoleClaim<Guid>, PermissionVM>()
			.ConvertUsing(s => ((PermissionVM)ApplicationPermissions.GetPermissionByValue(s.ClaimValue))!);
	}
}
