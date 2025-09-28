using ArtAuction.Dto;

namespace ArtAuction.Interface
{
    public interface IBid
    {
        Task<IEnumerable<BidDtos>> GetAllAsync();
        Task<BidDtos?> GetByIdAsync(int id);
        Task<BidDtos?> PlaceBidAsync(BidCreateDto dto); // main operation
        Task<bool> DeleteAsync(int id);
    }
}
