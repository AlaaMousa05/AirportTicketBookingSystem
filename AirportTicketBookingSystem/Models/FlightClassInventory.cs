using System.Text.Json.Serialization;

namespace AirportTicketBooking.Models;

public class FlightClassInventory
{
    public decimal Price { get; }

    public int TotalSeats { get; }

    public int BookedSeats { get; private set; }

    public int AvailableSeats => TotalSeats - BookedSeats;

    [JsonConstructor]
    public FlightClassInventory(
        decimal price,
        int totalSeats,
        int bookedSeats)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price), price, "Price cannot be negative.");
        }

        if (totalSeats < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalSeats), totalSeats, "Total seats cannot be negative.");
        }

        if (bookedSeats < 0 || bookedSeats > totalSeats)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bookedSeats),
                bookedSeats,
                "Booked seats must be between zero and total seats.");
        }

        Price = price;
        TotalSeats = totalSeats;
        BookedSeats = bookedSeats;
    }

    public FlightClassInventory(decimal price, int totalSeats)
        : this(price, totalSeats, 0)
    {
    }
}
