using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Permission>>> GetAll()
    {
        var result = await _permissionService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Permission>> GetById(int id)
    {
        var result = await _permissionService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Error ?? "Permission not found" });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePermissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _permissionService.CreateAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data, message = "Permission created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePermissionRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "ID mismatch between route and request body" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _permissionService.UpdateAsync(request);
        
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
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _permissionService.DeleteAsync(id);
        
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

    [HttpPost("roles/{roleId}/permissions/{permissionId}")]
    public async Task<ActionResult> AssignPermissionToRole(string roleId, int permissionId)
    {
        var userId = User.Identity?.Name ?? "System";
        var result = await _permissionService.AssignPermissionToRoleAsync(roleId, permissionId, userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("roles/{roleId}/permissions/{permissionId}")]
    public async Task<ActionResult> RemovePermissionFromRole(string roleId, int permissionId)
    {
        var result = await _permissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    [HttpGet("roles/{roleId}")]
    public async Task<ActionResult<IEnumerable<Permission>>> GetPermissionsByRole(string roleId)
    {
        var result = await _permissionService.GetPermissionsByRoleIdAsync(roleId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{permissionId}/roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetRolesByPermission(int permissionId)
    {
        var result = await _permissionService.GetRolesByPermissionIdAsync(permissionId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }
}
