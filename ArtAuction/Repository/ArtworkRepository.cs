using ArtAuction.Dto;
using ArtAuction.Interface;
using ArtAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Repositories
{
    public class ArtworkRepository : IArtwork
    {
        private readonly ArtAuctionDbContext _context;

        public ArtworkRepository(ArtAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArtworkDtos>> GetAllAsync()
        {
            return await _context.Artworks
                .Select(a => new ArtworkDtos
                {
                    ArtworkId = a.ArtworkId,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    // ✅ convert byte[] → Base64 string
                    ImageBase64 = a.ImageData != null ? Convert.ToBase64String(a.ImageData) : null,
                    StartingPrice = a.StartingPrice,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    ArtistId = a.ArtistId
                })
                .ToListAsync();
        }

        public async Task<ArtworkDtos?> GetByIdAsync(int id)
        {
            var a = await _context.Artworks.FindAsync(id);
            if (a == null) return null;

            return new ArtworkDtos
            {
                ArtworkId = a.ArtworkId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category,
                ImageBase64 = a.ImageData != null ? Convert.ToBase64String(a.ImageData) : null,
                StartingPrice = a.StartingPrice,
                AuctionStartTime = a.AuctionStartTime,
                AuctionEndTime = a.AuctionEndTime,
                ArtistId = a.ArtistId
            };
        }

        public async Task<ArtworkDtos> CreateAsync(ArtworkCreateDto dto)
        {
            byte[]? imageBytes = null;
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var entity = new Artwork
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                ImageData = imageBytes,
                StartingPrice = dto.StartingPrice,
                AuctionStartTime = dto.AuctionStartTime,
                AuctionEndTime = dto.AuctionEndTime,
                ArtistId = dto.ArtistId
            };

            _context.Artworks.Add(entity);
            await _context.SaveChangesAsync();

            return new ArtworkDtos
            {
                ArtworkId = entity.ArtworkId,
                Title = entity.Title,
                Description = entity.Description,
                Category = entity.Category,
                ImageBase64 = entity.ImageData != null ? Convert.ToBase64String(entity.ImageData) : null,
                StartingPrice = entity.StartingPrice,
                AuctionStartTime = entity.AuctionStartTime,
                AuctionEndTime = entity.AuctionEndTime,
                ArtistId = entity.ArtistId
            };
        }

        public async Task<ArtworkDtos?> UpdateAsync(int id, ArtworkUpdateDto dto)
        {
            var a = await _context.Artworks.FindAsync(id);
            if (a == null) return null;

            a.Title = dto.Title;
            a.Description = dto.Description;
            a.Category = dto.Category;
            a.StartingPrice = dto.StartingPrice ?? a.StartingPrice;
            a.AuctionStartTime = dto.AuctionStartTime ?? a.AuctionStartTime;
            a.AuctionEndTime = dto.AuctionEndTime ?? a.AuctionEndTime;

            // ✅ update image if provided
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);
                a.ImageData = ms.ToArray();
            }

            await _context.SaveChangesAsync();

            return new ArtworkDtos
            {
                ArtworkId = a.ArtworkId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category,
                ImageBase64 = a.ImageData != null ? Convert.ToBase64String(a.ImageData) : null,
                StartingPrice = a.StartingPrice,
                AuctionStartTime = a.AuctionStartTime,
                AuctionEndTime = a.AuctionEndTime,
                ArtistId = a.ArtistId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var a = await _context.Artworks.FindAsync(id);
            if (a == null) return false;

            _context.Artworks.Remove(a);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ArtworkDtos>> GetByArtistIdAsync(int artistId)
        {
            return await _context.Artworks
                .Where(a => a.ArtistId == artistId)
                .Select(a => new ArtworkDtos
                {
                    ArtworkId = a.ArtworkId,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    ImageBase64 = a.ImageData != null ? Convert.ToBase64String(a.ImageData) : null,
                    StartingPrice = a.StartingPrice,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    ArtistId = a.ArtistId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ArtworkDtos>> GetActiveArtworksAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Artworks
                .Where(a => a.AuctionStartTime <= now && a.AuctionEndTime >= now)
                .Select(a => new ArtworkDtos
                {
                    ArtworkId = a.ArtworkId,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    ImageBase64 = a.ImageData != null ? Convert.ToBase64String(a.ImageData) : null,
                    StartingPrice = a.StartingPrice,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    ArtistId = a.ArtistId
                })
                .ToListAsync();
        }
    }
}
