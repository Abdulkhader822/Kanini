using ArtAuction.Models;
using ArtAuction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly ReceiptService _svc;
        private readonly IWebHostEnvironment _env;
        private readonly ArtAuctionDbContext _context; // needed for direct queries

        public ReceiptController(ReceiptService svc, IWebHostEnvironment env, ArtAuctionDbContext context)
        {
            _svc = svc;
            _env = env;
            _context = context;
        }

        // ✅ Get all receipts (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _svc.GetAllReceiptsAsync();
            return Ok(receipts);
        }

        // ✅ Get single receipt by ID and stream PDF
        [HttpGet("{id}")]
        [Authorize(Roles = "Buyer,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _svc.GetReceiptByIdAsync(id);
            if (receipt == null) return NotFound("Receipt not found");

            return await ReturnFileFromPath(receipt.PdfFilePath);
        }

        // ✅ Get receipt by TransactionId and stream PDF
        [HttpGet("transaction/{transactionId}")]
        [Authorize(Roles = "Buyer,Admin")]
        public async Task<IActionResult> GetByTransactionId(int transactionId)
        {
            var receipt = await _svc.GetReceiptByTransactionIdAsync(transactionId);
            if (receipt == null) return NotFound("Receipt not found");

            return await ReturnFileFromPath(receipt.PdfFilePath);
        }

        // ✅ Explicit download endpoint (matches what I suggested earlier)
        [HttpGet("download/{transactionId}")]
        [Authorize(Roles = "Buyer,Admin")]
        public async Task<IActionResult> Download(int transactionId)
        {
            var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.TransactionId == transactionId);
            if (receipt == null) return NotFound();

            return await ReturnFileFromPath(receipt.PdfFilePath);
        }

        // 🔹 Helper method: returns file from wwwroot
        private async Task<IActionResult> ReturnFileFromPath(string relativePath)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot", relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!System.IO.File.Exists(filePath)) return NotFound("Receipt file not found");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }
    }
}
