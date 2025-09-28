using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtAuction.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }   // PK

        [Required]
        public decimal BidAmount { get; set; }

        public DateTime BidTime { get; set; }

        [ForeignKey("Artwork")]
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }

        [ForeignKey("Buyer")]
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
    }
}
