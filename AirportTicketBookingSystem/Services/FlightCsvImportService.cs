using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AirportTicketBooking.Models;
using Microsoft.VisualBasic.FileIO;

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

        string[]? headers = SplitLine(lines[0]);

        if (headers is null ||
            !headers.SequenceEqual(
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
        string[]? columns = SplitLine(line);

        if (columns is null)
        {
            errors.Add("Invalid CSV format. Check the double quotes.");
            return null;
        }

        if (columns.Length != ExpectedHeaders.Length)
        {
            errors.Add(
                $"Expected {ExpectedHeaders.Length} columns, but found {columns.Length}. " +
                "Put values containing commas in double quotes.");
            return null;
        }

        var economy = new FlightClassInventory(
            ReadPrice(columns[5], "EconomyPrice", errors),
            ReadSeats(columns[6], "EconomySeats", errors));

        var business = new FlightClassInventory(
            ReadPrice(columns[7], "BusinessPrice", errors),
            ReadSeats(columns[8], "BusinessSeats", errors));

        var firstClass = new FlightClassInventory(
            ReadPrice(columns[9], "FirstClassPrice", errors),
            ReadSeats(columns[10], "FirstClassSeats", errors));

        var flight = new Flight(
            columns[0].Trim(),
            columns[1].Trim(),
            columns[2].Trim(),
            columns[3].Trim(),
            ReadDepartureDate(columns[4], errors),
            economy,
            business,
            firstClass);

        ValidateModel(flight, string.Empty, errors);
        ValidateModel(economy, "Economy", errors);
        ValidateModel(business, "Business", errors);
        ValidateModel(firstClass, "FirstClass", errors);

        return errors.Count == 0 ? flight : null;
    }

    // Returns null when the line has invalid double quotes.
    private static string[]? SplitLine(string line)
    {
        using var parser = new TextFieldParser(new StringReader(line));

        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;
        parser.TrimWhiteSpace = false;

        try
        {
            return parser.ReadFields();
        }
        catch (MalformedLineException)
        {
            return null;
        }
    }

    private static void ValidateModel(
        object model,
        string fieldPrefix,
        List<string> errors)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            results,
            validateAllProperties: true);

        foreach (ValidationResult result in results)
        {
            errors.Add(fieldPrefix + result.ErrorMessage);
        }
    }

    // The Read methods below convert a cell from text. When a cell cannot be converted,
    // they report the error and return a valid placeholder, so the Flight can still be
    // built and the rest of the row can be validated. The row is rejected either way.
    private static DateTime ReadDepartureDate(string value, List<string> errors)
    {
        string text = value.Trim();

        if (text.Length == 0)
        {
            errors.Add("DepartureDate is required.");
            return DateTime.Today;
        }

        if (!DateTime.TryParseExact(
                text,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime departureDate))
        {
            errors.Add($"Invalid departure date. Expected format: {DateFormat}.");
            return DateTime.Today;
        }

        return departureDate;
    }

    private static decimal ReadPrice(
        string value,
        string fieldName,
        List<string> errors)
    {
        string text = value.Trim();

        if (text.Length == 0)
        {
            errors.Add($"{fieldName} is required.");
            return FlightClassInventory.MinimumPrice;
        }

        if (!decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal price))
        {
            errors.Add($"{fieldName} must be a valid decimal.");
            return FlightClassInventory.MinimumPrice;
        }

        return price;
    }

    private static int ReadSeats(
        string value,
        string fieldName,
        List<string> errors)
    {
        string text = value.Trim();

        if (text.Length == 0)
        {
            errors.Add($"{fieldName} is required.");
            return FlightClassInventory.MinimumSeats;
        }

        if (!int.TryParse(
                text,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int seats))
        {
            errors.Add($"{fieldName} must be a valid integer.");
            return FlightClassInventory.MinimumSeats;
        }

        return seats;
    }
}
