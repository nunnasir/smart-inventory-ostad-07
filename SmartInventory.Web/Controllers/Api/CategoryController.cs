using Microsoft.AspNetCore.Mvc;

namespace SmartInventory.Web.Controllers.Api;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = GetAllCategories();

        return Ok(categories);
    }

    private List<Category> GetAllCategories()
    {
        List<Category> categories = new List<Category>
        {
            new Category {Id = 1, Name = "Category1"}
        };

        return categories;
    }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

