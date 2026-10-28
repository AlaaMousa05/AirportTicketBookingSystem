
namespace AirportTicketBooking.Models;

public class Flight
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DepartureCountry { get; set; } = string.Empty;

    public string DestinationCountry { get; set; } = string.Empty;

    public string DepartureAirport { get; set; } = string.Empty;

    public string ArrivalAirport { get; set; } = string.Empty;

    public DateTime DepartureDate { get; set; }

    public FlightPricing Pricing { get; set; } = new();

    public int AvailableSeats { get; set; }
}
