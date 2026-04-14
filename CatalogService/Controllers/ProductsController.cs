using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.Models;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            CatalogDbContext context,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("📦 Obteniendo productos");

            var products =
                await _context.Products.ToListAsync();

            return Ok(products);
        }

        // GET api/products/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product =
                await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST api/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            Product product)
        {
            _logger.LogInformation(
                "➕ Creando producto {Name}",
                product.Name
            );

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product
            );
        }

        // PUT api/products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            Product updatedProduct)
        {
            if (id != updatedProduct.Id)
                return BadRequest();

            _context.Entry(updatedProduct)
                    .State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(
            int id)
        {
            var product =
                await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}