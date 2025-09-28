using ArtAuction.Dto;
using ArtAuction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtAuction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly BidService _svc;
        public BidController(BidService svc) { _svc = svc; }

        [HttpPost]
        [Authorize(Roles = "Buyer,Admin,Artist")]
        public async Task<IActionResult> PlaceBid([FromBody] BidCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var buyerId)) return Unauthorized();

            dto.BuyerId = buyerId;
            var result = await _svc.PlaceBidAsync(dto);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid bid. Your bid must be higher than the current highest bid." });
            }

            return CreatedAtAction(nameof(GetHighest), new { artworkId = dto.ArtworkId }, result);
        }

        [HttpGet("highest/{artworkId}")]
        public async Task<IActionResult> GetHighest(int artworkId)
        {
            var r = await _svc.GetHighestBidAsync(artworkId);
            return Ok(r);
        }

        [HttpGet("history/{artworkId}")]
        [Authorize(Roles = "Buyer,Admin,Artist")]
        public async Task<IActionResult> GetHistory(int artworkId)
        {
            var bids = await _svc.GetBidHistoryAsync(artworkId);
            return Ok(bids);
        }
    }
}
