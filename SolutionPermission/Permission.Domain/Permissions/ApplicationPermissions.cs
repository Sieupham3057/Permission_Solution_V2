using Permission.Domain.Constances;
using Permission.Domain.Enums;
using System.Collections.ObjectModel;

namespace Permission.Domain.Permissions;

public static class ApplicationPermissions
{
    /************* USER PERMISSIONS *************/

    public static readonly ApplicationPermission ReadUsers = new(
        $"{ActionType.Read} {Functions.Users}",// "View Users",
        $"{Functions.Users}.{ActionType.Read}",// "Users.View",
        Functions.UsersGroupName,
        Functions.UsersViewDes);

    public static readonly ApplicationPermission ManageUsers = new(
        $"{ActionType.Manage} {Functions.Users}", // "Manage Users",
        $"{Functions.Users}.{ActionType.Manage}", // "Users.Manage",
        Functions.UsersGroupName,
        Functions.UsersManageDes);

    /************* ROLE PERMISSIONS *************/

    public static readonly ApplicationPermission ReadRoles = new(
        $"{ActionType.Read} {Functions.Roles}", // "View Roles",
        $"{Functions.Roles}.{ActionType.Read}", // "roles.view",
        Functions.RolesGroupName,
        Functions.RolesViewDes);

    public static readonly ApplicationPermission ManageRoles = new(
        $"{ActionType.Manage} {Functions.Roles}", // "Manage Roles",
        $"{Functions.Roles}.{ActionType.Manage}", // "roles.manage",
        Functions.RolesGroupName,
        Functions.RolesManageDes);

    public static readonly ApplicationPermission AssignRoles = new(
        $"{ActionType.Assign} {Functions.Roles}", // "Assign Roles",
        $"{Functions.Roles}.{ActionType.Assign}", // "roles.assign",
        Functions.RolesGroupName,
        Functions.RolesAssignDes);

    /************* PRODUCT PERMISSIONS *************/

    public static readonly ApplicationPermission ReadProducts = new(
        $"{ActionType.Read} {Functions.Products}", // "View Products",
        $"{Functions.Products}.{ActionType.Read}", // "products.view",
        Functions.ProductsGroupName,
        Functions.ProductsViewDes);

    public static readonly ApplicationPermission ManageProducts = new(
        $"{ActionType.Manage} {Functions.Products}", // "Manage Products",
        $"{Functions.Products}.{ActionType.Manage}", // "products.manage",
        Functions.ProductsGroupName,
        Functions.ProductsManageDes);

    /************* CUSTOMER PERMISSIONS *************/

    public static readonly ApplicationPermission ReadCustomers = new(
        $"{ActionType.Read} {Functions.Customers}", // "View Customers",
        $"{Functions.Customers}.{ActionType.Read}", // "customers.view",
        Functions.CustomersGroupName,
        Functions.CustomersViewDes);

    public static readonly ApplicationPermission ManageCustomers = new(
        $"{ActionType.Manage} {Functions.Customers}", // "Manage Customers",
        $"{Functions.Customers}.{ActionType.Manage}", // "customers.manage",
        Functions.CustomersGroupName,
        Functions.CustomersManageDes);

    public static readonly ApplicationPermission CreateCustomers = new(
        $"{ActionType.Create} {Functions.Customers}", // "Create Customers",
        $"{Functions.Customers}.{ActionType.Create}", // "customers.create",
        Functions.CustomersGroupName,
        Functions.CustomersManageDes);

    public static readonly ApplicationPermission UpdateCustomers = new(
        $"{ActionType.Update} {Functions.Customers}", // "Update Customers",
        $"{Functions.Customers}.{ActionType.Update}", // "customers.update",
        Functions.CustomersGroupName,
        Functions.CustomersManageDes);

    public static readonly ApplicationPermission DeleteCustomers = new(
        $"{ActionType.Delete} {Functions.Customers}", // "Delete Customers",
        $"{Functions.Customers}.{ActionType.Delete}", // "customers.delete",
        Functions.CustomersGroupName,
        Functions.CustomersManageDes);

    /************* ALL PERMISSIONS *************/

    public static readonly ReadOnlyCollection<ApplicationPermission> AllPermissions =
        new List<ApplicationPermission> {
                ReadUsers, ManageUsers, // User service
                ReadRoles, ManageRoles, AssignRoles,// role service
                ReadProducts, ManageProducts, // product service
                ReadCustomers, ManageCustomers, CreateCustomers, UpdateCustomers, DeleteCustomers, // customer service
        }.AsReadOnly();

    /************* HELPER METHODS *************/

    public static ApplicationPermission? GetPermissionByName(string? permissionName)
    {
        return AllPermissions.SingleOrDefault(p => p.Name == permissionName);
    }

    public static ApplicationPermission? GetPermissionByValue(string? permissionValue)
    {
        return AllPermissions.SingleOrDefault(p => p.Value == permissionValue);
    }

    public static string[] GetAllPermissionValues()
    {
        var result = AllPermissions.Select(p => p.Value).ToArray();
        return result;
    }

    public static string[] GetAdministrativePermissionValues()
    {
        return [ManageUsers, ManageRoles, AssignRoles];
    }
}