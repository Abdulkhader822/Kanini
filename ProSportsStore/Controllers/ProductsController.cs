// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProSportsStore.Interface;
using ProSportsStore.Models;

namespace ProSportsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct _repo;
        public ProductsController(IProduct repo) => _repo = repo;

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllProducts());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _repo.GetProductById(id);
            return p is null ? NotFound() : Ok(p);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(Product product)
        {
            var created = await _repo.AddProduct(product);
            return CreatedAtAction(nameof(Get), new { id = created.ProductId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();
            return Ok(await _repo.UpdateProduct(product));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) =>
            (await _repo.DeleteProduct(id)) ? NoContent() : NotFound();
    }
}
