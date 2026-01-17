using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Permission.API.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Permission.API.Controllers.V3;

[ApiVersion("3.0")]
public class ProductsController : BaseApiController
{
	public ProductsController(ILogger<ProductsController> logger, IMapper mapper) : base(logger, mapper)
	{
	}

	[HttpGet]
	public async Task<IActionResult> GetProducts()
	{
		return Ok("Get list products from api version 3");
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetProducts(int id)
	{
		return Ok("Get product by id from api version 3");
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateProducts(int id, [FromBody] ProductRequest request)
	{
		return Ok("Update product from api version 3");
	}

	[HttpPost]
	public async Task<IActionResult> CreateProducts([FromBody] ProductRequest request)
	{
		return Ok("Create new product from api version 3");
	}
}

public class ProductRequest
{
	[Required] public int Id { get; set; }
	[Required] public string Name { get; set; }
	[Required] public string Description { get; set; }

}