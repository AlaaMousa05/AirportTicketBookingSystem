using System.Globalization;
using AirportTicketBooking.Models;

namespace AirportTicketBooking.Services;

public class FlightCsvImportService
{
    private const string DateFormat = Flight.DepartureDateFormat;

    private static readonly string[] ExpectedHeaders =
    {
        "DepartureCountry",
        "DestinationCountry",
        "DepartureAirport",
        "ArrivalAirport",
        "DepartureDate",
        "EconomyPrice",
        "EconomySeats",
        "BusinessPrice",
        "BusinessSeats",
        "FirstClassPrice",
        "FirstClassSeats"
    };

    public CsvImportResult Import(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "CSV file was not found.",
                filePath);
        }

        string[] lines = File.ReadAllLines(filePath);
        var result = new CsvImportResult();

        if (lines.Length == 0)
        {
            result.Errors.Add("The CSV file is empty.");
            return result;
        }

        string[] headers = lines[0].Split(',');

        if (!headers.SequenceEqual(
                ExpectedHeaders,
                StringComparer.OrdinalIgnoreCase))
        {
            result.Errors.Add(
                "CSV headers are invalid or in the wrong order.");
            return result;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            int rowNumber = i + 1;
            var rowErrors = new List<string>();

            Flight? flight = ParseRow(lines[i], rowErrors);

            if (flight is not null)
                result.ImportedFlights.Add(flight);

            foreach (string error in rowErrors)
            {
                result.Errors.Add($"Row {rowNumber}: {error}");
            }
        }

        return result;
    }

    private static Flight? ParseRow(string line, List<string> errors)
    {
        string[] columns = line.Split(',');

        if (columns.Length != ExpectedHeaders.Length)
        {
            errors.Add(
                $"Expected {ExpectedHeaders.Length} columns, but found {columns.Length}.");
            return null;
        }

        string departureCountry = ReadRequiredText(columns[0], "DepartureCountry", errors);
        string destinationCountry = ReadRequiredText(columns[1], "DestinationCountry", errors);
        string departureAirport = ReadRequiredText(columns[2], "DepartureAirport", errors);
        string arrivalAirport = ReadRequiredText(columns[3], "ArrivalAirport", errors);
        DateTime departureDate = ReadDepartureDate(columns[4], errors);
        decimal economyPrice = ReadPrice(columns[5], "EconomyPrice", errors);
        int economySeats = ReadSeats(columns[6], "EconomySeats", errors);
        decimal businessPrice = ReadPrice(columns[7], "BusinessPrice", errors);
        int businessSeats = ReadSeats(columns[8], "BusinessSeats", errors);
        decimal firstClassPrice = ReadPrice(columns[9], "FirstClassPrice", errors);
        int firstClassSeats = ReadSeats(columns[10], "FirstClassSeats", errors);

        if (errors.Count > 0)
            return null;

        return new Flight(
            departureCountry,
            destinationCountry,
            departureAirport,
            arrivalAirport,
            departureDate,
            new FlightClassInventory(economyPrice, economySeats),
            new FlightClassInventory(businessPrice, businessSeats),
            new FlightClassInventory(firstClassPrice, firstClassSeats));
    }

    private static string ReadRequiredText(
        string value,
        string fieldName,
        List<string> errors)
    {
        string text = value.Trim();

        if (string.IsNullOrWhiteSpace(text))
            errors.Add($"{fieldName} is required.");

        return text;
    }

    private static DateTime ReadDepartureDate(string value, List<string> errors)
    {
        string text = value.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            errors.Add("DepartureDate is required.");
            return default;
        }

        if (!DateTime.TryParseExact(
                text,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime departureDate))
        {
            errors.Add($"Invalid departure date. Expected format: {DateFormat}.");
            return default;
        }

        if (departureDate.Date < DateTime.Today)
            errors.Add("Departure date cannot be in the past.");

        return departureDate;
    }

    private static decimal ReadPrice(
        string value,
        string fieldName,
        List<string> errors)
    {
        string text = value.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            errors.Add($"{fieldName} is required.");
            return default;
        }

        if (!decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal price))
        {
            errors.Add($"{fieldName} must be a valid decimal.");
            return default;
        }

        if (price < FlightClassInventory.MinimumPrice)
            errors.Add($"{fieldName} must be at least {FlightClassInventory.MinimumPrice}.");

        return price;
    }

    private static int ReadSeats(
        string value,
        string fieldName,
        List<string> errors)
    {
        string text = value.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            errors.Add($"{fieldName} is required.");
            return default;
        }

        if (!int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int seats))
        {
            errors.Add($"{fieldName} must be a valid integer.");
            return default;
        }

        if (seats < FlightClassInventory.MinimumSeats)
            errors.Add($"{fieldName} must be at least {FlightClassInventory.MinimumSeats}.");

        return seats;
    }
}
