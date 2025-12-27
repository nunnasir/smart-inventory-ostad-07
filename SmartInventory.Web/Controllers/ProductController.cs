using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;
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
        public async Task<IActionResult> Create(CreateProductRequest product)
        {
            if(ModelState.IsValid == false)
            {
                return View(product);
            }

            var result = await _productService.AddAsync(product);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var productResult = await _productService.GetByIdAsync(id);
            
            if (!productResult.Success || productResult.Data == null)
            {
                TempData["ErrorMessage"] = productResult.Error ?? "Product not found.";
                return RedirectToAction("Index");
            }

            var product = productResult.Data;
            var updateRequest = new UpdateProductRequest
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            return View(updateRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductRequest product)
        {
            if (ModelState.IsValid == false)
            {
                return View(product);
            }

            var result = await _productService.UpdateAsync(product);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            
            TempData["ErrorMessage"] = result.Error ?? "An error occurred while updating the product.";
            return View(product);
        }

        public async Task<IActionResult> Details(int id)
        {
            var productResult = await _productService.GetByIdAsync(id);
            
            if (!productResult.Success || productResult.Data == null)
            {
                TempData["ErrorMessage"] = productResult.Error ?? "Product not found.";
                return RedirectToAction("Index");
            }

            return View(productResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Product deleted successfully!";
                return RedirectToAction("Index");
            }
            
            TempData["ErrorMessage"] = result.Error ?? "An error occurred while deleting the product.";
            return RedirectToAction("Index");
        }

    }
}
