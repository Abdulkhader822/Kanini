using ArtAuction.Dto;

namespace ArtAuction.Interface
{
    public interface IReceipt
    {
        Task<IEnumerable<ReceiptDtos>> GetAllAsync();
        Task<ReceiptDtos?> GetByIdAsync(int id);
        Task<ReceiptDtos?> GetByTransactionIdAsync(int transactionId);
    }
}
