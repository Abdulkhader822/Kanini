using System.ComponentModel.DataAnnotations;

namespace JWTsample.Models
{
    public class Passengers
    {
        [Key]
        public int PassengersId { get; set; }
        public string PassengersName { get; set; }
        public int PassengersNumber { get; set; }
    }
}
