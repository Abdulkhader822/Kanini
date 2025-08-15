using System.ComponentModel.DataAnnotations;

namespace CodeFirst_Assignment.Models
{
    public class BookReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(100)]
        public string BookTitle { get; set; }

        [Required]
        [StringLength(60)]
        public string ReviewerName { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

    }
}
