using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of all products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var result = await _productService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Error ?? "Product not found" });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product ID</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update request</param>
    /// <returns>No content</returns>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
