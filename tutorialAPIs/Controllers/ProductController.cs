using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tutorialAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context) 
        {
            _dataContext = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            return Ok(await _dataContext.Products.ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
                return BadRequest("Product not found");
            return Ok(product);
        }
        [HttpPost]
        public async Task<ActionResult<List<Product>>> AddProduct(Product product)
        {
            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();
            return Ok(await _dataContext.Products.ToListAsync());
        }
        [HttpPut]
        public async Task<ActionResult<List<Product>>> UpdateProduct(Product request)
        {
            var product = await _dataContext.Products.FindAsync(request.Id);
            if (product == null)
                return BadRequest("Product not found");
            product.Name = request.Name;
            product.Description = request.Description;
            product.InStock = request.InStock;
            product.Price = request.Price;
            product.image = request.image;

            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Products.ToListAsync());
        }
        [HttpDelete]
        public async Task<ActionResult<List<Product>>> DeleteProduct(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
                return BadRequest("Product not found");

            _dataContext.Remove(product);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Products.ToListAsync());
        }
    }
}
