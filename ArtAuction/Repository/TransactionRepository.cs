using ArtAuction.Dto;
using ArtAuction.Interface;
using ArtAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Repositories
{
    public class TransactionRepository : ITransaction
    {
        private readonly ArtAuctionDbContext _context;

        public TransactionRepository(ArtAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionDtos>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Receipt)
                .Select(t => new TransactionDtos
                {
                    TransactionId = t.TransactionId,
                    ArtworkId = t.ArtworkId,
                    BuyerId = t.BuyerId,
                    FinalPrice = t.FinalPrice,
                    TransactionDate = t.TransactionDate,
                    PaymentMethod = t.PaymentMethod,
                    PaymentStatus = t.PaymentStatus,
                    ReceiptFilePath = t.Receipt != null ? t.Receipt.PdfFilePath : string.Empty
                })
                .ToListAsync();
        }

        public async Task<TransactionDtos?> GetByIdAsync(int id)
        {
            var t = await _context.Transactions
                .Include(x => x.Receipt)
                .FirstOrDefaultAsync(x => x.TransactionId == id);

            if (t == null) return null;

            return new TransactionDtos
            {
                TransactionId = t.TransactionId,
                ArtworkId = t.ArtworkId,
                BuyerId = t.BuyerId,
                FinalPrice = t.FinalPrice,
                TransactionDate = t.TransactionDate,
                PaymentMethod = t.PaymentMethod,
                PaymentStatus = t.PaymentStatus,
                ReceiptFilePath = t.Receipt != null ? t.Receipt.PdfFilePath : string.Empty
            };
        }

        public async Task<TransactionDtos?> CreateTransactionAsync(TransactionCreateDto dto)
        {
            // 1️⃣ Find artwork + bids
            var artwork = await _context.Artworks
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.ArtworkId == dto.ArtworkId);

            if (artwork == null || !artwork.Bids.Any()) return null;

            // 2️⃣ Auction must be ended
            if (DateTime.UtcNow <= artwork.AuctionEndTime) return null;

            // 3️⃣ Get winning bid
            var winningBid = artwork.Bids.OrderByDescending(b => b.BidAmount).FirstOrDefault();
            if (winningBid == null || winningBid.BuyerId != dto.BuyerId) return null;

            // 4️⃣ Create transaction
            var transaction = new Transaction
            {
                ArtworkId = dto.ArtworkId,
                BuyerId = dto.BuyerId,
                FinalPrice = winningBid.BidAmount,
                TransactionDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Completed"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // 5️ Create receipt
            var receipt = new Receipt
            {
                TransactionId = transaction.TransactionId,
                PdfFilePath = $"/receipts/receipt_{transaction.TransactionId}.pdf",
                GeneratedOn = DateTime.UtcNow
            };

            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            return new TransactionDtos
            {
                TransactionId = transaction.TransactionId,
                ArtworkId = transaction.ArtworkId,
                BuyerId = transaction.BuyerId,
                FinalPrice = transaction.FinalPrice,
                TransactionDate = transaction.TransactionDate,
                PaymentMethod = transaction.PaymentMethod,
                PaymentStatus = transaction.PaymentStatus,
                ReceiptFilePath = receipt.PdfFilePath
            };
        }
    }
}
