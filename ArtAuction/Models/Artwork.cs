using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtAuction.Models
{
    public class Artwork
    {

        [Key]
        public int ArtworkId { get; set; }   

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Category { get; set; }
        public byte[]? ImageData { get; set; }

        [Required]
        public decimal StartingPrice { get; set; }

        public DateTime AuctionStartTime { get; set; }
        public DateTime AuctionEndTime { get; set; }

        
        [ForeignKey("Artist")]
        public int ArtistId { get; set; }
        public User Artist { get; set; }

        // Navigation
        public ICollection<Bid> Bids { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
