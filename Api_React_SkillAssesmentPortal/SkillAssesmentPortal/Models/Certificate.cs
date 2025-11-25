using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillAssessmentPortal.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TestId { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required, MaxLength(255)]
        public string CertificateURL { get; set; } = string.Empty;

        //  Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("TestId")]
        public Test? Test { get; set; }
    }
}
