using Microsoft.AspNetCore.Mvc;

namespace SmartInventory.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}


// API: Application Programming Interface
// REST: Representational State Transfer

// REST Constraints:
// 1. Client-Server Architecture / Separation of Concerns
// Rule: 
// UI and Server should be separate 
// React, Angular, Mobile Apps, etc. can be clients
// Server should only provide APIs (Database operations, business logic, etc.)
// 
// 2. Statelessness
// Rule:
// Each request from client to server must contain all the information needed to understand and process the request

// 3. resource-based URIs
// Rule:
// Resources should be identified in the request URIs
// URIs Example
// ProductController:
// [GET] https://123.com/products    // Return All Products 
// [POST] https://123.com/products  // Create Product
// [GET] https://123.com/products/1  // Return Product
// [DELETE] https://123.com/products/1  // Product
// [POST] https://123.com/products  // Product
// baseUrl/products
// baseUrl/products/1

// 4. Proper Use of HTTP Methods
// GET      -> Read
// GET/1    -> Read (Single)
// POST     -> Write
// PUT      -> Full Update
// PACTH    -> Partial Update
// DELETE   -> Remove

// 5. Uniform Interface 
// Rule:
// A standardized way of communicating between client and server

// 6. Representations
// Rule:
// 

// 7. HATEOAS (Hypermedia As The Engine Of Application State)

// SOAP 

// PName;100;TestCategory;

/*    
    {
      "id": 1,
      "name": "Category1"
    }
 */
