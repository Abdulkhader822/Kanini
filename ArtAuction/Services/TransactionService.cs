using ArtAuction.Dto;
using ArtAuction.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ArtAuction.Services
{
    public class TransactionService
    {
        private readonly ArtAuctionDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TransactionService(ArtAuctionDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<(bool success, TransactionDtos? txDto, string? receiptPath, string message)>
            CreateTransactionAsync(TransactionCreateDto dto)
        {
            // ✅ Load buyer with artwork
            var artwork = await _context.Artworks
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.ArtworkId == dto.ArtworkId);

            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.BuyerId);
            if (buyer == null)
                return (false, null, null, "Buyer not found");


            var now = DateTime.Now;
            if (now < artwork.AuctionEndTime)
                return (false, null, null, "Auction not ended yet");

            // ✅ Ensure only winner can pay
            var highest = artwork.Bids.OrderByDescending(b => b.BidAmount).FirstOrDefault();
            if (highest == null || highest.BuyerId != dto.BuyerId)
                return (false, null, null, "You are not the winning bidder");

            // ✅ Prevent duplicate payment
            var existingTx = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ArtworkId == artwork.ArtworkId);
            if (existingTx != null)
                return (false, null, null, "Payment already completed for this artwork");

            // ✅ Create transaction
            var txn = new Transaction
            {
                ArtworkId = artwork.ArtworkId,
                BuyerId = dto.BuyerId,
                FinalPrice = highest.BidAmount,
                TransactionDate = DateTime.Now,
                PaymentMethod = dto.PaymentMethod ?? "Mock",
                PaymentStatus = "Completed"
            };

            _context.Transactions.Add(txn);
            await _context.SaveChangesAsync();

            // ✅ Create receipt entry
            var receipt = new Receipt
            {
                TransactionId = txn.TransactionId,
                PdfFilePath = $"receipts/receipt-{txn.TransactionId}.pdf",
                GeneratedOn = DateTime.Now
            };

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            // ✅ Generate PDF receipt with QuestPDF
            var wwwRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
            var receiptsDir = Path.Combine(wwwRoot, "receipts");
            if (!Directory.Exists(receiptsDir)) Directory.CreateDirectory(receiptsDir);

            var pdfPath = Path.Combine(receiptsDir, $"receipt-{txn.TransactionId}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().Text("Art Auction Receipt").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Receipt ID: {receipt.ReceiptId}");
                        col.Item().Text($"Transaction ID: {txn.TransactionId}");
                        col.Item().Text($"Buyer: {buyer.FullName}");       

                        col.Item().Text($"Artwork: {artwork.Title}");
                        col.Item().Text($"Final Price: ₹{txn.FinalPrice}"); 
                        col.Item().Text($"Date: {txn.TransactionDate}");
                        col.Item().Text($"Payment Method: {txn.PaymentMethod}");
                        col.Item().Text($"Status: {txn.PaymentStatus}");
                    });
                    page.Footer().AlignCenter().Text("Thank you for using ArtAuction!");
                });
            }).GeneratePdf(pdfPath);

            var txDto = new TransactionDtos
            {
                TransactionId = txn.TransactionId,
                ArtworkId = txn.ArtworkId,
                BuyerId = txn.BuyerId,
                BuyerName = buyer.FullName,    

                FinalPrice = txn.FinalPrice,
                TransactionDate = txn.TransactionDate,
                PaymentMethod = txn.PaymentMethod,
                PaymentStatus = txn.PaymentStatus
            };

            var receiptUrl = $"/receipts/receipt-{txn.TransactionId}.pdf";
            return (true, txDto, receiptUrl, "Payment completed successfully. Receipt generated.");
        }
    }
}
