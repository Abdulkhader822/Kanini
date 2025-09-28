using ArtAuction.Dto;

namespace ArtAuction.Interface
{
    public interface IArtwork
    {
        Task<IEnumerable<ArtworkDtos>> GetAllAsync();
        Task<ArtworkDtos?> GetByIdAsync(int id);
        Task<ArtworkDtos> CreateAsync(ArtworkCreateDto dto);
        Task<ArtworkDtos?> UpdateAsync(int id, ArtworkUpdateDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<ArtworkDtos>> GetByArtistIdAsync(int artistId);
        Task<IEnumerable<ArtworkDtos>> GetActiveArtworksAsync();


    }
}
