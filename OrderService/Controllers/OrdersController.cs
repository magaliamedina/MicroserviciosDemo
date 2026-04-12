using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
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
        private readonly OrderDbContext _context;

        public OrdersController(PaymentServiceClient paymentClient,
            ILogger<OrdersController> logger, 
            OrderDbContext context)
        {
            _paymentClient = paymentClient;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            _logger.LogInformation("Obteniendo lista de órdenes");

            var orders = await _context.Orders.ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound(new
                {
                    message = $"Order con id {id} no encontrada"
                });

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            try
            {
                _logger.LogInformation(
                    "Creando nueva orden para producto {Product}",
                    order.Product
                );

                order.CreatedAt = DateTime.UtcNow;
                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                Console.WriteLine($"Orden guardada en DB: {order.Id}");

                _logger.LogInformation(
                    "✅ Orden guardada con ID {OrderId}",
                    order.Id
                );

                // Crear request de pago
                var paymentRequest = new
                {
                    OrderId = order.Id,
                    Amount = order.Total
                };

                var paymentResult = await _paymentClient.ProcessPaymentAsync(paymentRequest);

                return Ok(new
                {
                    order,
                    Payment = paymentResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                        ex,
                        "❌ Error procesando orden"
                    );
                throw;
            }            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = $"Order con id {id} no encontrada"
                });
            }

            // Actualizar campos
            order.Product = updatedOrder.Product;
            order.Quantity = updatedOrder.Quantity;
            order.Total = updatedOrder.Total;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Order con id {id} actualizada correctamente",
                order
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = $"Order con id {id} no encontrada"
                });
            }

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Order con id {id} eliminada correctamente"
            });
        }
    }
}