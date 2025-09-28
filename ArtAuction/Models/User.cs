using System.ComponentModel.DataAnnotations;

namespace ArtAuction.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }   

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } 

        public DateTime CreatedAt { get; set; }

        
        public ICollection<Artwork> Artworks { get; set; }
        public ICollection<Bid> Bids { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
