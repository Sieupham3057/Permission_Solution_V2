using Authorization.Handlers;
using System.Security.Claims;

namespace Customer.API.Authorization;

using Customer.Domain.Entities;
using global::Authorization.Claims;

public sealed class CustomerAuthorizationHandler : BaseCrudAuthorizationHandler<Customer>
{
    protected override string CreatePermission => CustomerPermissions.Create;
    protected override string ReadPermission => CustomerPermissions.Read;
    protected override string UpdatePermission => CustomerPermissions.Update;
    protected override string DeletePermission => CustomerPermissions.Delete;

    protected override bool IsOwner(ClaimsPrincipal user, Customer customer)
        => customer.CreatedBy == user.GetUserId();
}