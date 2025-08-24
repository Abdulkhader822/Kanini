// Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProSportsStore.Interface;
using ProSportsStore.Models;

namespace ProSportsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUser _repo;
        public UsersController(IUser repo) => _repo = repo;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _repo.GetAllUsers());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get(int id)
        {
            var u = await _repo.GetUserById(id);
            return u is null ? NotFound() : Ok(u);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Post(User user) => Ok(await _repo.AddUser(user));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, User user)
        {
            if (id != user.UserId) return BadRequest();
            var updated = await _repo.UpdateUser(user);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) =>
            (await _repo.DeleteUser(id)) ? NoContent() : NotFound();

        // Search by name/email
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var results = await _repo.SearchUsers(keyword);
            if (!results.Any()) return NotFound("No users found.");
            return Ok(results);
        }

        // Filter by role
        [HttpGet("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter([FromQuery] string role)
        {
            var results = await _repo.FilterByRole(role);
            if (!results.Any()) return NotFound($"No users with role '{role}'.");
            return Ok(results);
        }

    }
}
