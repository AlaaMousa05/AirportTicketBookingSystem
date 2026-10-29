
namespace AirportTicketBooking.Models;

public class Flight
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string DepartureCountry { get; private set; } = string.Empty;

    public string DestinationCountry { get; private set; } = string.Empty;

    public string DepartureAirport { get; private set; } = string.Empty;

    public string ArrivalAirport { get; private set; } = string.Empty;

    public DateTime DepartureDate { get; private set; }

    public FlightPricing Pricing { get; private set; } = new();

    public int AvailableSeats { get; private set; }
}
