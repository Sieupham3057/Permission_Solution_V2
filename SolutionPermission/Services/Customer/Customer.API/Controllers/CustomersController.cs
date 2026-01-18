using Authorization.Abstractions;
using Authorization.Claims;
using Customer.API.Authorization;
using Customer.API.Contracts;
using Customer.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IAuthorizationService _authorization;

    public CustomersController(ICustomerService service, IAuthorizationService authorization)
    {
        _service = service;
        _authorization = authorization;
    }

    // LIST → POLICY
    [HttpGet]
    [Authorize(Policy = CustomerPolicies.ViewList)]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET BY ID → CRUD.READ
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();

        var auth = await _authorization.AuthorizeAsync(
            User, customer,
            new ResourceAuthorizationRequirement(CrudOperations.Read));

        if (!auth.Succeeded) return Forbid();

        return Ok(customer);
    }

    // CREATE → POLICY
    [HttpPost]
    [Authorize(Policy = CustomerPolicies.Create)]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        var id = await _service.CreateAsync(
            request.Code,
            request.Name,
            request.Email,
            User.GetUserId());

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    // UPDATE → CRUD.UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();

        var auth = await _authorization.AuthorizeAsync(
            User,
            customer,
            new ResourceAuthorizationRequirement(CrudOperations.Update));

        if (!auth.Succeeded) return Forbid();

        await _service.UpdateAsync(id, request.Name, request.Email);
        return NoContent();
    }

    // DELETE → CRUD.DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();

        var auth = await _authorization.AuthorizeAsync(
            User,
            customer,
            new ResourceAuthorizationRequirement(CrudOperations.Delete));

        if (!auth.Succeeded) return Forbid();

        await _service.DeleteAsync(id);
        return NoContent();
    }
}