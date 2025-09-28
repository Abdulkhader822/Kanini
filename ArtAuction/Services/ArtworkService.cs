using ArtAuction.Dto;
using ArtAuction.Interface;

namespace ArtAuction.Services
{
    public class ArtworkService
    {
        private readonly IArtwork _artworkRepo;

        public ArtworkService(IArtwork artworkRepo)
        {
            _artworkRepo = artworkRepo;
        }

        public Task<IEnumerable<ArtworkDtos>> GetAllArtworksAsync()
            => _artworkRepo.GetAllAsync();

        public Task<ArtworkDtos?> GetArtworkByIdAsync(int id)
            => _artworkRepo.GetByIdAsync(id);

        public async Task<ArtworkDtos> CreateArtworkAsync(ArtworkCreateDto dto)
        {
            dto.AuctionStartTime = DateTime.SpecifyKind(dto.AuctionStartTime, DateTimeKind.Local);
            dto.AuctionEndTime = DateTime.SpecifyKind(dto.AuctionEndTime, DateTimeKind.Local);

            return await _artworkRepo.CreateAsync(dto);
        }

        public async Task<ArtworkDtos?> UpdateArtworkAsync(int id, ArtworkUpdateDto dto)
        {
            if (dto.AuctionStartTime.HasValue)
                dto.AuctionStartTime = DateTime.SpecifyKind(dto.AuctionStartTime.Value, DateTimeKind.Local);

            if (dto.AuctionEndTime.HasValue)
                dto.AuctionEndTime = DateTime.SpecifyKind(dto.AuctionEndTime.Value, DateTimeKind.Local);

            return await _artworkRepo.UpdateAsync(id, dto);
        }

        public Task<bool> DeleteArtworkAsync(int id)
            => _artworkRepo.DeleteAsync(id);

        public Task<IEnumerable<ArtworkDtos>> GetArtworksByArtistIdAsync(int artistId)
            => _artworkRepo.GetByArtistIdAsync(artistId);

        public Task<IEnumerable<ArtworkDtos>> GetActiveArtworksAsync()
            => _artworkRepo.GetActiveArtworksAsync();
    }
}
