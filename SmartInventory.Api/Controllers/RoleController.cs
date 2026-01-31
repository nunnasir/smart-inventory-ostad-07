using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;

namespace SmartInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetAll()
    {
        var result = await _roleService.GetAllRolesAsync();

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdentityRole>> GetById(string id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);

        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Error ?? "Role not found" });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _roleService.CreateRoleAsync(request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data, message = "Role created successfully" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] UpdateRoleRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "ID mismatch between route and request body" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _roleService.UpdateRoleAsync(request);

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
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _roleService.DeleteRoleAsync(id);

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

    [HttpPost("users/{userId}/assign")]
    public async Task<ActionResult> AssignUserToRole(string userId, [FromBody] string roleName)
    {
        var result = await _roleService.AssignUserToRoleAsync(userId, roleName);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    [HttpPost("users/{userId}/remove")]
    public async Task<ActionResult> RemoveUserFromRole(string userId, [FromBody] string roleName)
    {
        var result = await _roleService.RemoveUserFromRoleAsync(userId, roleName);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var result = await _roleService.GetUserRolesAsync(userId);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Data);
    }
}
