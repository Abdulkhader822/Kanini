using SkillAssessmentPortal.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Question
{
    [Key]
    public int QuestionId { get; set; }

    [Required]
    public int TestLevelId { get; set; }

    [Required]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public string OptionA { get; set; } = string.Empty;

    [Required]
    public string OptionB { get; set; } = string.Empty;

    [Required]
    public string OptionC { get; set; } = string.Empty;

    [Required]
    public string OptionD { get; set; } = string.Empty;

    [Required]
    [StringLength(1)]
    public string CorrectOption { get; set; } = string.Empty; // 'A', 'B', 'C', 'D'

    // Navigation
    [ForeignKey("TestLevelId")]
    public TestLevel? TestLevel { get; set; }
}
