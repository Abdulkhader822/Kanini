using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillAssessmentPortal.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        public int TestLevelId { get; set; }

        public int AttemptNumber { get; set; } = 1;

        [Range(0, 100)]
        public decimal Score { get; set; }

        [Range(0, 100)]
        public decimal Percentage { get; set; }

        public int TimeTakenSecs { get; set; }

        public DateTime DateAttempted { get; set; } = DateTime.Now;

        [Required, MaxLength(10)]
        public string ResultStatus { get; set; } = "Fail"; // Pass or Fail

        [MaxLength(255)]
        public string? Suggestion { get; set; }

        //  Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("TestId")]
        public Test? Test { get; set; }

        [ForeignKey("TestLevelId")]
        public TestLevel? TestLevel { get; set; }
    }
}
