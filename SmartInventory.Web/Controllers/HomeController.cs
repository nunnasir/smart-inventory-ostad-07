using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Web.Models;

namespace SmartInventory.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // ViewBag
            //ViewBag.Title = "Smart Inventory System";
            //ViewBag.Total = 100;

            // ViewData
            //Dictionary<string, object>
            //ViewData["Title"] = "Smart Inventory System";
            //ViewData["Total"] = 100;

            // TempData
            //TempData["Success"] = "Successfully Saved";

            // Model

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
