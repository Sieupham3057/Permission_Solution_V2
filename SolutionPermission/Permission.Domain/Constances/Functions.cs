namespace Permission.Domain.Constances;

public static class Functions
{
	public const string Users = nameof(Users);
	public const string UsersGroupName = "User_Permissions";
	public const string UsersViewDes = "Permission to view other users account details";
	public const string UsersManageDes = "Permission to create, delete and modify other users account details";

	public const string Roles = nameof(Roles);
	public const string RolesGroupName = "Role_Permissions";
	public const string RolesViewDes = "Permission to view available roles";
	public const string RolesManageDes = "Permission to create, delete and modify roles";
	public const string RolesAssignDes = "Permission to assign roles to users";

	public const string Products = nameof(Products);
	public const string ProductsGroupName = "Product_Permissions";
	public const string ProductsViewDes = "Permission to view products";
	public const string ProductsManageDes = "Permission to create, delete and modify product";

	public const string Customers = nameof(Customers);
	public const string CustomersGroupName = "Customer_Permissions";
	public const string CustomersViewDes = "Permission to view customers";
	public const string CustomersManageDes = "Permission to create, delete and modify customer";
}