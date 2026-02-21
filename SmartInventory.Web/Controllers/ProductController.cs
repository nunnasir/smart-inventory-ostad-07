using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Interfaces;
using SmartInventory.BLL.Mapping;
using SmartInventory.Contract.Request;
using SmartInventory.Contract.Response;
using SmartInventory.Model;
using SmartInventory.Web.Constants;

namespace SmartInventory.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetDataTables([FromForm] DataTablesRequest request)
    {
        if (request is null)
        {
            return BadRequest(CreateEmptyDataTablesResponse(ProductMessages.InvalidRequest));
        }

        var response = await _productService.GetDataTablesAsync(request);
        return Json(response);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _productService.AddAsync(request);

        if (result.Success)
        {
            return RedirectToIndexWithSuccess(ProductMessages.CreateSuccess);
        }

        TempData[TempDataKeys.ErrorMessage] = result.Error;
        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            return RedirectToIndexWithError(result.Error ?? ProductMessages.NotFound);
        }

        return View(result.Data.ToUpdateRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _productService.UpdateAsync(request);

        if (result.Success)
        {
            return RedirectToIndexWithSuccess(ProductMessages.UpdateSuccess);
        }

        TempData[TempDataKeys.ErrorMessage] = result.Error ?? ProductMessages.UpdateFailed;
        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            return RedirectToIndexWithError(result.Error ?? ProductMessages.NotFound);
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (result.Success)
        {
            return RedirectToIndexWithSuccess(ProductMessages.DeleteSuccess);
        }

        return RedirectToIndexWithError(result.Error ?? ProductMessages.DeleteFailed);
    }

    #region Private Helpers

    private IActionResult RedirectToIndexWithSuccess(string message)
    {
        TempData[TempDataKeys.SuccessMessage] = message;
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectToIndexWithError(string message)
    {
        TempData[TempDataKeys.ErrorMessage] = message;
        return RedirectToAction(nameof(Index));
    }

    private static DataTablesResponse<Product> CreateEmptyDataTablesResponse(string error)
    {
        return new DataTablesResponse<Product>
        {
            Draw = 0,
            RecordsTotal = 0,
            RecordsFiltered = 0,
            Data = new List<Product>(),
            Error = error
        };
    }

    #endregion
}
