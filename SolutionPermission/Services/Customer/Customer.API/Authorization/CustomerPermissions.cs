namespace Customer.API.Authorization;

/**
 * Best practice khuyến nghị (thực tế nhất): “Permission Canonicalization” + “Single Claim”
1) Quy ước 1 format duy nhất cho permission

Chọn 1 chuẩn và dán lên tường:
    customers.read
    customers.create
    customers.update
    customers.delete
👉 Lý do chọn lowercase + dot:

    - DevOps, IAM, policy-as-code quen kiểu này
    - Không vướng PascalCase / snake_case
    - Dễ diff, dễ grep, dễ audit
    - Nếu team bạn đang có Customers.Read rồi, vẫn ok — miễn là 1 chuẩn duy nhất.
        Nhưng lowercase thường ít sai hơn.
 */

public static class CustomerPermissions
{
    public const string Read = "customers.read";
    public const string Create = "customers.create";
    public const string Update = "customers.update";
    public const string Delete = "customers.delete";
}