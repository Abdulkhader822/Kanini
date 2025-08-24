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
    public class OrderController : ControllerBase
    {
        private readonly IOrder _repo;
        public OrderController(IOrder repo) => _repo = repo;

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _repo.GetAllOrdersAsync();

            // map to DTOs
            var result = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _repo.GetOrderByIdAsync(id);
            if (o is null) return NotFound();

            var dto = new OrderDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            };

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Post(OrderDTO dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status
            };

            var created = await _repo.AddOrderAsync(order);

            var result = new OrderDTO
            {
                OrderId = created.OrderId,
                UserId = created.UserId,
                OrderDate = created.OrderDate,
                TotalAmount = created.TotalAmount,
                Status = created.Status
            };

            return CreatedAtAction(nameof(Get), new { id = result.OrderId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Put(int id, OrderDTO dto)
        {
            if (id != dto.OrderId) return BadRequest();

            var order = new Order
            {
                OrderId = dto.OrderId,
                UserId = dto.UserId,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status
            };

            var updated = await _repo.UpdateOrderAsync(order);
            if (updated is null) return NotFound();

            var result = new OrderDTO
            {
                OrderId = updated.OrderId,
                UserId = updated.UserId,
                OrderDate = updated.OrderDate,
                TotalAmount = updated.TotalAmount,
                Status = updated.Status
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int id) =>
            (await _repo.DeleteOrderAsync(id)) ? NoContent() : NotFound();
    }
}
