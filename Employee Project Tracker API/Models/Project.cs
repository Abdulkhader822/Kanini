using System.ComponentModel.DataAnnotations;

namespace Employee_Project_Tracker_API.Models
{
   
    
        public class Project
        {
            public int ProjectId { get; set; }

            [Required, StringLength(10)]
            public string ProjectCode { get; set; }

            [Required, StringLength(100)]
            public string ProjectName { get; set; }

            [Required]
            public DateTime StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            [Required]
            public decimal Budget { get; set; }

            // Navigation Property
            public List<Employee> Employees { get; set; } = new();
        }
}

