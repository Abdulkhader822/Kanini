namespace ArtAuction.Dto
{
    public class ArtworkDtos
    {
        public int ArtworkId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal? StartingPrice { get; set; }
        public DateTime? AuctionStartTime { get; set; }
        public DateTime? AuctionEndTime { get; set; }
        public int ArtistId { get; set; }

        public string? ImageBase64 { get; set; }

    }

    public class ArtworkCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public DateTime AuctionStartTime { get; set; }
        public DateTime AuctionEndTime { get; set; }
        public int ArtistId { get; set; }
        public IFormFile? ImageFile { get; set; }

    }

    public class ArtworkUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? StartingPrice { get; set; }
        public DateTime? AuctionStartTime { get; set; }
        public DateTime? AuctionEndTime { get; set; }

        public IFormFile? ImageFile { get; set; }

    }
}

