namespace AirportTicketBooking.Models;

public class Booking
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid PassengerId { get; init; }

    public Guid FlightId { get; private set; }

    public FlightClass FlightClass { get; private set; }

    public decimal Price { get; private set; }

    public BookingStatus Status { get; private set; } = BookingStatus.Confirmed;
}