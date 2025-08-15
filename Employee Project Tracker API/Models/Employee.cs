using System.ComponentModel.DataAnnotations;

namespace Employee_Project_Tracker_API.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(8)]
        public string EmployeeCode { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Designation { get; set; }

        [Required]
        public decimal Salary { get; set; }

        // Foreign Key
        public int ProjectId { get; set; }

        // Navigation Property
        public Project? Project { get; set; }
    }
}
