namespace ArtAuction.Dto
{
    public class BidDtos
    {
        public int BidId { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
        public int ArtworkId { get; set; }
        public int BuyerId { get; set; }
    }

    public class BidCreateDto
    {
        public decimal BidAmount { get; set; }
        public int ArtworkId { get; set; }
        public int BuyerId { get; set; }
    }
}
