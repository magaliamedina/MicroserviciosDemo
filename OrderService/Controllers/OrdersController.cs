using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = new[]
            {
                new { Id = 1, Product = "Laptop", Quantity = 1, Total = 1200 },
                new { Id = 2, Product = "Mouse", Quantity = 2, Total = 50 }
            };
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] dynamic order)
        {
            // Aquí podrías guardar el pedido en una base de datos.
            return CreatedAtAction(nameof(GetOrders), new { id = 3 }, order);
        }
    }
}
