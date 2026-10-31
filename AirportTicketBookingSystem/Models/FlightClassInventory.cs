using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirportTicketBooking.Models;

public class FlightClassInventory
{
    public const decimal MinimumPrice = 0;
    public const int MinimumSeats = 0;

    [Required]
    [Range((double)MinimumPrice, double.MaxValue)]
    public decimal Price { get; }

    [Required]
    [Range(MinimumSeats, int.MaxValue)]
    [Display(Name = "Seats")]
    public int TotalSeats { get; }

    public int BookedSeats { get; private set; }

    public int AvailableSeats => TotalSeats - BookedSeats;

    [JsonConstructor]
    public FlightClassInventory(
        decimal price,
        int totalSeats,
        int bookedSeats)
    {
        if (price < MinimumPrice)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price), price, $"Price must be at least {MinimumPrice}.");
        }

        if (totalSeats < MinimumSeats)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalSeats), totalSeats, $"Total seats must be at least {MinimumSeats}.");
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

    public void BookSeat()
    {
        if (AvailableSeats <= 0)
        {
            throw new InvalidOperationException(
                "No available seats in this class.");
        }

        BookedSeats++;
    }

    public void CancelSeat()
    {
        if (BookedSeats <= 0)
        {
            throw new InvalidOperationException(
                "There are no booked seats to cancel.");
        }

        BookedSeats--;
    }
}
