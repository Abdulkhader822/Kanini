using ArtAuction.Dto;
using ArtAuction.Interface;

namespace ArtAuction.Services
{
    public class ReceiptService
    {
        private readonly IReceipt _receiptRepo;

        public ReceiptService(IReceipt receiptRepo)
        {
            _receiptRepo = receiptRepo;
        }

        public Task<IEnumerable<ReceiptDtos>> GetAllReceiptsAsync() => _receiptRepo.GetAllAsync();
        public Task<ReceiptDtos?> GetReceiptByIdAsync(int id) => _receiptRepo.GetByIdAsync(id);
        public Task<ReceiptDtos?> GetReceiptByTransactionIdAsync(int transactionId) => _receiptRepo.GetByTransactionIdAsync(transactionId);
    }
}
