using ArtAuction.Dto;
using ArtAuction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtAuction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _svc;

        public TransactionController(TransactionService svc)
        {
            _svc = svc;
        }

        [HttpPost("pay")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Pay([FromBody] TransactionCreateDto dto)
        {
            try
            {
                // ✅ Get BuyerId from JWT (not from request)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var buyerId))
                    return Unauthorized(new { message = "Invalid user token" });

                dto.BuyerId = buyerId;

                // ✅ Call service
                var (success, tx, receiptPath, message) = await _svc.CreateTransactionAsync(dto);

                if (!success)
                    return BadRequest(new { success = false, message });

                return Ok(new
                {
                    success = true,
                    message,
                    transaction = tx,
                    receiptUrl = receiptPath
                });
            }
            catch (Exception ex)
            {
                // ✅ Always JSON
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
