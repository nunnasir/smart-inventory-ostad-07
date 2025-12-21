using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        
        {
            var products = await _productService.GetAllAsync();

            return View(products.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _productService.AddAsync(product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (result.Success)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, result.Error ?? "An error occured while updating the product");
            return BadRequest();
        }
    }
}
