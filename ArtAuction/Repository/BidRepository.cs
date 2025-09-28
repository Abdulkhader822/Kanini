using ArtAuction.Dto;
using ArtAuction.Interface;
using ArtAuction.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtAuction.Repositories
{
    public class BidRepository : IBid
    {
        private readonly ArtAuctionDbContext _context;

        public BidRepository(ArtAuctionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BidDtos>> GetAllAsync()
        {
            return await _context.Bids
                .Select(b => new BidDtos
                {
                    BidId = b.BidId,
                    BidAmount = b.BidAmount,
                    BidTime = b.BidTime,
                    ArtworkId = b.ArtworkId,
                    BuyerId = b.BuyerId
                })
                .ToListAsync();
        }

        public async Task<BidDtos?> GetByIdAsync(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null) return null;

            return new BidDtos
            {
                BidId = bid.BidId,
                BidAmount = bid.BidAmount,
                BidTime = bid.BidTime,
                ArtworkId = bid.ArtworkId,
                BuyerId = bid.BuyerId
            };
        }

        public async Task<BidDtos?> PlaceBidAsync(BidCreateDto dto)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.ArtworkId == dto.ArtworkId);

            if (artwork == null) return null;

            // Business rules
            var highestBid = artwork.Bids.Any() ? artwork.Bids.Max(b => b.BidAmount) : artwork.StartingPrice;
            if (dto.BidAmount <= highestBid) return null;

            if (DateTime.UtcNow < artwork.AuctionStartTime || DateTime.UtcNow > artwork.AuctionEndTime)
                return null;

            var bid = new Bid
            {
                BidAmount = dto.BidAmount,
                BidTime = DateTime.UtcNow,
                ArtworkId = dto.ArtworkId,
                BuyerId = dto.BuyerId
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return new BidDtos
            {
                BidId = bid.BidId,
                BidAmount = bid.BidAmount,
                BidTime = bid.BidTime,
                ArtworkId = bid.ArtworkId,
                BuyerId = bid.BuyerId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null) return false;

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
