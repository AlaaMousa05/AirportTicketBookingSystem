using System.Text.Json.Serialization;

namespace AirportTicketBooking.Models;

public class Booking
{
    [JsonInclude]
    public Guid Id { get; private set; }

    public Guid PassengerId { get; private set; }

    public Guid FlightId { get; private set; }

    public FlightClass FlightClass { get; private set; }

    public decimal Price { get; private set; }

    [JsonInclude]
    public BookingStatus Status { get; private set; }

    public Booking(
        Guid passengerId,
        Guid flightId,
        FlightClass flightClass,
        decimal price)
    {
        if (passengerId == Guid.Empty)
            throw new ArgumentException("Passenger ID is required.");

        if (flightId == Guid.Empty)
            throw new ArgumentException("Flight ID is required.");

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        Id = Guid.NewGuid();
        PassengerId = passengerId;
        FlightId = flightId;
        FlightClass = flightClass;
        Price = price;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");
        }

        Status = BookingStatus.Cancelled;
    }

    public void Modify(
        Guid flightId,
        FlightClass flightClass,
        decimal price)
    {
        if (flightId == Guid.Empty)
            throw new ArgumentException("Flight ID is required.");

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        FlightId = flightId;
        FlightClass = flightClass;
        Price = price;
    }
}