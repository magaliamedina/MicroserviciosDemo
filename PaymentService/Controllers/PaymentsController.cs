using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("PaymentService está funcionando");
        }

        // POST /api/payments
        [HttpPost]
        public IActionResult ProcessPayment([FromBody] PaymentRequest request)
        {
            // Simular lentitud
            Thread.Sleep(10000); // 10 segundos

            var paymentResult = new PaymentResult
            {
                PaymentId = Guid.NewGuid(),
                OrderId = request.OrderId,
                Amount = request.Amount,
                Status = "Success",
                Message = "Pago procesado correctamente"
            };

            return Ok(paymentResult);
        }

        // Modelo de request
        public class PaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    // Modelo de respuesta
    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
