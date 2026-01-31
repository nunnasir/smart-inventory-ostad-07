using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Require authentication for all endpoints
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:Product.Read")]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var result = await _productService.GetAllAsync();

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:Product.Read")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Error ?? "Product not found" });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:Product.Create")]
    public async Task<ActionResult> Create([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.AddAsync(request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data, message = "Product created successfully" });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:Product.Update")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "ID mismatch between route and request body" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _productService.UpdateAsync(request);

        if (!result.Success)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(new { message = result.Error });
            }
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:Product.Delete")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.Success)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(new { message = result.Error });
            }
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }
}
