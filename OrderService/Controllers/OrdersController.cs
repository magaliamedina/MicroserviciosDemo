using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using System.Text;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly PaymentServiceClient _paymentClient;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(PaymentServiceClient paymentClient,
            ILogger<OrdersController> logger)
        {
            _paymentClient = paymentClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            _logger.LogInformation("Obteniendo lista de órdenes");

            var orders = new[]
            {
                new { Id = 1, Product = "Laptop", Quantity = 1, Total = 1200 },
                new { Id = 2, Product = "Mouse", Quantity = 2, Total = 50 }
            };

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest order)
        {
            _logger.LogInformation("Creando nueva orden");
            int orderId = 3;

            var paymentRequest = new
            {
                OrderId = orderId,
                Amount = order.Total
            };

            var paymentResult = await _paymentClient.ProcessPaymentAsync(paymentRequest);

            return Ok(new
            {
                Order = order,
                Payment = paymentResult
            });
        }
    }

    public class OrderRequest
    {
        public string Product { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}