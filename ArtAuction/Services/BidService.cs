using ArtAuction.Dto;
using ArtAuction.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ArtAuction.Hubs;

namespace ArtAuction.Services
{
    public class BidService
    {
        private readonly ArtAuctionDbContext _context;
        private readonly IHubContext<AuctionHub> _hub;

        public BidService(ArtAuctionDbContext context, IHubContext<AuctionHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // Place a bid
        public async Task<BidDtos?> PlaceBidAsync(BidCreateDto dto)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.ArtworkId == dto.ArtworkId);

            if (artwork == null) return null;

            var now = DateTime.Now; // Local time
            if (now < artwork.AuctionStartTime || now > artwork.AuctionEndTime)
                return null;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var highest = await _context.Bids
                    .Where(b => b.ArtworkId == dto.ArtworkId)
                    .OrderByDescending(b => b.BidAmount)
                    .FirstOrDefaultAsync();

                decimal currentHigh = highest?.BidAmount ?? artwork.StartingPrice;

                if (dto.BidAmount <= currentHigh)
                {
                    await tx.RollbackAsync();
                    return null;
                }

                var bid = new Bid
                {
                    BidAmount = dto.BidAmount,
                    BidTime = DateTime.Now, // Local time
                    ArtworkId = dto.ArtworkId,
                    BuyerId = dto.BuyerId
                };

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var bidDto = new BidDtos
                {
                    BidId = bid.BidId,
                    BidAmount = bid.BidAmount,
                    BidTime = bid.BidTime,
                    ArtworkId = bid.ArtworkId,
                    BuyerId = bid.BuyerId
                };

                // Broadcast to clients
                await _hub.Clients.Group($"artwork-{dto.ArtworkId}")
                    .SendAsync("BidPlaced", bidDto);

                return bidDto;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // Get highest bid
        public async Task<BidDtos?> GetHighestBidAsync(int artworkId)
        {
            var highest = await _context.Bids
                .Where(b => b.ArtworkId == artworkId)
                .OrderByDescending(b => b.BidAmount)
                .FirstOrDefaultAsync();

            if (highest == null) return null;

            return new BidDtos
            {
                BidId = highest.BidId,
                BidAmount = highest.BidAmount,
                BidTime = highest.BidTime, // Local
                ArtworkId = highest.ArtworkId,
                BuyerId = highest.BuyerId
            };
        }

        // Full history
        public async Task<IEnumerable<BidDtos>> GetBidHistoryAsync(int artworkId)
        {
            return await _context.Bids
                .Where(b => b.ArtworkId == artworkId)
                .OrderByDescending(b => b.BidTime)
                .Select(b => new BidDtos
                {
                    BidId = b.BidId,
                    BidAmount = b.BidAmount,
                    BidTime = b.BidTime, // Local
                    ArtworkId = b.ArtworkId,
                    BuyerId = b.BuyerId
                })
                .ToListAsync();
        }
    }
}
