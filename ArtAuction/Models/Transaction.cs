using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtAuction.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }   // PK

        
        [ForeignKey("Artwork")]
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; }

        [ForeignKey("Buyer")]
        public int BuyerId { get; set; }
        public User Buyer { get; set; }

        public decimal FinalPrice { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } 
        public string PaymentStatus { get; set; } 

        
        public Receipt Receipt { get; set; }
    }
}
