namespace Customer.Infrastructure.Seed;

// Permissions thường được seed tại Identity / Auth service, không phải Customer DB.
// Nếu dùng SSO thì bỏ qua bước này
public static class CustomerPermissionSeed
{
    public static readonly string[] Permissions =
    {
        "Customers.Read",
        "Customers.Create",
        "Customers.Update",
        "Customers.Delete"
    };
}