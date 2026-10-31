using System.Globalization;
using AirportTicketBooking.Models;

namespace AirportTicketBooking.UI;

public static class ConsoleHelper
{
    private const string DateFormat = Flight.DepartureDateFormat;

    public static string? ReadOptionalText(string label)
    {
        Console.Write($"{label} (Enter to skip): ");
        string? text = Console.ReadLine()?.Trim();

        return string.IsNullOrEmpty(text) ? null : text;
    }

    public static DateTime? ReadOptionalDate(string label)
    {
        while (true)
        {
            string? text = ReadOptionalText($"{label} [{DateFormat}]");

            if (text is null)
                return null;

            if (DateTime.TryParseExact(
                    text,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
            {
                return date;
            }

            Console.WriteLine($"Invalid date. Expected format: {DateFormat}.");
        }
    }

    public static decimal? ReadOptionalPrice(string label)
    {
        while (true)
        {
            string? text = ReadOptionalText(label);

            if (text is null)
                return null;

            if (decimal.TryParse(
                    text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal price) && price >= 0)
            {
                return price;
            }

            Console.WriteLine("Invalid price.");
        }
    }

    public static FlightClass? ReadOptionalClass()
    {
        while (true)
        {
            string? text = ReadOptionalText("Class: 1) Economy 2) Business 3) First Class");

            switch (text)
            {
                case null:
                    return null;
                case "1":
                    return FlightClass.Economy;
                case "2":
                    return FlightClass.Business;
                case "3":
                    return FlightClass.FirstClass;
                default:
                    Console.WriteLine("Invalid class.");
                    break;
            }
        }
    }

    // Returns the zero-based index of the chosen item, or null when skipped.
    public static int? ReadOptionalIndex(string label, int itemCount)
    {
        while (true)
        {
            string? text = ReadOptionalText(label);

            if (text is null)
                return null;

            if (int.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int number) && number >= 1 && number <= itemCount)
            {
                return number - 1;
            }

            Console.WriteLine("Invalid number.");
        }
    }

    public static void PrintFlights(List<Flight> flights)
    {
        if (flights.Count == 0)
        {
            Console.WriteLine("No flights found.");
            return;
        }

        for (int i = 0; i < flights.Count; i++)
        {
            Flight flight = flights[i];

            Console.WriteLine(
                $"{i + 1}. {flight.DepartureCountry} ({flight.DepartureAirport}) -> " +
                $"{flight.DestinationCountry} ({flight.ArrivalAirport}) | " +
                $"{flight.DepartureDate.ToString(DateFormat, CultureInfo.InvariantCulture)}");

            Console.WriteLine(
                $"   Economy: {flight.Economy.Price} ({flight.Economy.AvailableSeats} seats) | " +
                $"Business: {flight.Business.Price} ({flight.Business.AvailableSeats} seats) | " +
                $"First Class: {flight.FirstClass.Price} ({flight.FirstClass.AvailableSeats} seats)");
        }
    }

    public static string DescribeBooking(Booking booking, Flight? flight)
    {
        string route = flight is null
            ? "Unknown flight"
            : $"{flight.DepartureCountry} -> {flight.DestinationCountry} | " +
              $"{flight.DepartureDate.ToString(DateFormat, CultureInfo.InvariantCulture)}";

        return $"{route} | {booking.FlightClass} | {booking.Price} | {booking.Status}";
    }
}
