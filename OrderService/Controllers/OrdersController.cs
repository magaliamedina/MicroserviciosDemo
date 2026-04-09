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

        public OrdersController(PaymentServiceClient paymentClient)
        {
            _paymentClient = paymentClient;
        }

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
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest order)
        {
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