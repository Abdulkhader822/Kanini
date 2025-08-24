using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProSportsStore.Interface;
using ProSportsStore.Models;
using ProSportsStore.DTOs;

namespace ProSportsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItem _repo;
        public OrderItemController(IOrderItem repo) => _repo = repo;

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllOrderItemsAsync();

            var result = items.Select(i => new OrderItemDTO
            {
                OrderItemId = i.OrderItemId,
                OrderId = i.OrderId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _repo.GetOrderItemByIdAsync(id);
            if (item is null) return NotFound();

            var dto = new OrderItemDTO
            {
                OrderItemId = item.OrderItemId,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            };

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Post(OrderItemDTO dto)
        {
            var orderItem = new OrderItem
            {
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            var created = await _repo.AddOrderItemAsync(orderItem);

            var result = new OrderItemDTO
            {
                OrderItemId = created.OrderItemId,
                OrderId = created.OrderId,
                ProductId = created.ProductId,
                Quantity = created.Quantity,
                Price = created.Price
            };

            return CreatedAtAction(nameof(Get), new { id = result.OrderItemId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Put(int id, OrderItemDTO dto)
        {
            if (id != dto.OrderItemId) return BadRequest();

            var orderItem = new OrderItem
            {
                OrderItemId = dto.OrderItemId,
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            var updated = await _repo.UpdateOrderItemAsync(orderItem);
            if (updated is null) return NotFound();

            var result = new OrderItemDTO
            {
                OrderItemId = updated.OrderItemId,
                OrderId = updated.OrderId,
                ProductId = updated.ProductId,
                Quantity = updated.Quantity,
                Price = updated.Price
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id) =>
            (await _repo.DeleteOrderItemAsync(id)) ? NoContent() : NotFound();
    }
}
