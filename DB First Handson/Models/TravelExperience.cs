using System;
using System.Collections.Generic;

namespace DB_First_Handson.Models;

public partial class TravelExperience
{
    public int ExperienceId { get; set; }

    public string Destination { get; set; } = null!;

    public DateOnly TravelDate { get; set; }

    public decimal? Rating { get; set; }

    public bool? IsSoloTravel { get; set; }
}
