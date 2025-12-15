using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;

namespace SmartInventory.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var product = new Model.Product
            {
                Name = "Sample Product",
                Description = "This is a sample product.",
                Price = 19.99M
            };

            _productService.AddAsync(product);

            return View();
        }
    }
}
