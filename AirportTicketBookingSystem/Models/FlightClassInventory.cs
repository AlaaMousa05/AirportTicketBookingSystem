using System.Text.Json.Serialization;

namespace AirportTicketBooking.Models;

public class FlightClassInventory
{
    public decimal Price { get; }

    public int TotalSeats { get; }

    public int AvailableSeats { get; private set; }

   
    [JsonConstructor]
    public FlightClassInventory(decimal price, int totalSeats, int availableSeats)
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

        if (availableSeats < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(availableSeats), availableSeats, "Available seats cannot be negative.");
        }

        if (availableSeats > totalSeats)
        {
            throw new ArgumentException(
                "Available seats cannot exceed total seats.",
                nameof(availableSeats));
        }

        Price = price;
        TotalSeats = totalSeats;
        AvailableSeats = availableSeats;
    }

    
    public FlightClassInventory(decimal price, int totalSeats)
        : this(price, totalSeats, totalSeats)
    {
    }
}
