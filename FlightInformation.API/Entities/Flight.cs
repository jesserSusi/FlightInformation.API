using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightInformation.API.Entities;

public class Flight
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Display(Name = "Flight Number"), Required]
    [StringLength(5, MinimumLength = 5)]
    [RegularExpression(@"^([A-Z]{2})+([0-9]{3}$)")]
    public string FlightNumber { get; set; }

    [Display(Name = "Airline"), Required]
    public string Airline { get; set; }
    
    [Display(Name = "Departure Airport"), Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$")]
    public string DepartureAirport { get; set; }
    
    [Display(Name = "Arrival Airport"), Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$")]
    public string ArrivalAirport { get; set; }
    
    [Required]
    public DateTime DepartureTime { get; set; }
    
    [Required]
    public DateTime ArrivalTime { get; set; }
    
    // The max value (5) should be updated when there are changes to the FlightStatus enum
    [Range(1, 5), Required]
    public FlightStatus Status { get; set; }
}

public class Status
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Name { get; set; }
}

/// <summary>
/// Collection of Flight Status.
/// When this collection is added, the Range data annotation
/// should also be updated on the Flight.Status prop
/// </summary>
public enum FlightStatus
{
    Scheduled = 1,
    Delayed = 2,
    Cancelled = 3,
    InAir = 4,
    Landed = 5
}