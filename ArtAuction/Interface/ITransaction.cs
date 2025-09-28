using ArtAuction.Dto;

namespace ArtAuction.Interface
{
    public interface ITransaction
    {
        Task<IEnumerable<TransactionDtos>> GetAllAsync();
        Task<TransactionDtos?> GetByIdAsync(int id);
        Task<TransactionDtos?> CreateTransactionAsync(TransactionCreateDto dto);
    }
}
