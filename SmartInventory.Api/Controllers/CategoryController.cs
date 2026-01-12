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

    [HttpGet("{id}")]
    public IActionResult GetCategory(int id)
    {
        var category = GetAllCategories().Where(x => x.Id == id);

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        var category = GetAllCategories().Where(x => x.Id == id);

        return Ok(category);
    }

    private List<Category> GetAllCategories()
    {
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category1"},
            new Category { Id = 2, Name = "Category2"},
            new Category { Id = 3, Name = "Category3"},
            new Category { Id = 4, Name = "Category4"},
            new Category { Id = 5, Name = "Category5"},
            new Category { Id = 6, Name = "Category6"},
        };

        return categories;
    }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

