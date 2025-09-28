using ArtAuction.Dto;
using ArtAuction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtAuction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworkController : ControllerBase
    {
        private readonly ArtworkService _service;

        public ArtworkController(ArtworkService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (role == "Admin")
                return Ok(await _service.GetAllArtworksAsync());

            else if (role == "Artist")
                return Ok(await _service.GetArtworksByArtistIdAsync(userId));

            else if (role == "Buyer")
                return Ok(await _service.GetActiveArtworksAsync());

            return Forbid();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artwork = await _service.GetArtworkByIdAsync(id);
            return artwork == null ? NotFound() : Ok(artwork);
        }

        [Authorize(Roles = "Artist,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ArtworkCreateDto dto)   // ✅ from form
        {
            dto.ArtistId = GetCurrentUserId();
            var artwork = await _service.CreateArtworkAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = artwork.ArtworkId }, artwork);
        }

        [Authorize(Roles = "Artist,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ArtworkUpdateDto dto)   // ✅ from form
        {
            var artwork = await _service.UpdateArtworkAsync(id, dto);
            return artwork == null ? NotFound() : Ok(artwork);
        }

        [Authorize(Roles = "Artist,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteArtworkAsync(id);
            return result ? NoContent() : NotFound();
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return idClaim != null ? int.Parse(idClaim) : 0;
        }
    }
}
