using ArtAuction.Dto;
using ArtAuction.Interface;
using ArtAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Repositories
{
    public class ReceiptRepository : IReceipt
    {
        private readonly ArtAuctionDbContext _context;

        public ReceiptRepository(ArtAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReceiptDtos>> GetAllAsync()
        {
            return await _context.Receipts
                .Select(r => new ReceiptDtos
                {
                    ReceiptId = r.ReceiptId,
                    TransactionId = r.TransactionId,
                    PdfFilePath = r.PdfFilePath,
                    GeneratedOn = r.GeneratedOn
                })
                .ToListAsync();
        }

        public async Task<ReceiptDtos?> GetByIdAsync(int id)
        {
            var r = await _context.Receipts.FindAsync(id);
            if (r == null) return null;

            return new ReceiptDtos
            {
                ReceiptId = r.ReceiptId,
                TransactionId = r.TransactionId,
                PdfFilePath = r.PdfFilePath,
                GeneratedOn = r.GeneratedOn
            };
        }

        public async Task<ReceiptDtos?> GetByTransactionIdAsync(int transactionId)
        {
            var r = await _context.Receipts
                .FirstOrDefaultAsync(x => x.TransactionId == transactionId);

            if (r == null) return null;

            return new ReceiptDtos
            {
                ReceiptId = r.ReceiptId,
                TransactionId = r.TransactionId,
                PdfFilePath = r.PdfFilePath,
                GeneratedOn = r.GeneratedOn
            };
        }
    }
}
