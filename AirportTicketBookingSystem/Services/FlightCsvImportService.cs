
using System.Globalization;
using AirportTicketBooking.Models;

namespace AirportTicketBooking.Services;

public class FlightCsvImportService
{
    private const int ExpectedColumnCount = 11;

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
            string[] columns = lines[i].Split(',');
            int rowNumber = i + 1;

            if (columns.Length != ExpectedColumnCount)
            {
                result.Errors.Add(
                    $"Row {rowNumber}: Expected {ExpectedColumnCount} columns, but found {columns.Length}.");
                continue;
            }

            string departureCountry = columns[0].Trim();
            string destinationCountry = columns[1].Trim();
            string departureAirport = columns[2].Trim();
            string arrivalAirport = columns[3].Trim();

            var rowErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(departureCountry))
                rowErrors.Add("DepartureCountry is required.");

            if (string.IsNullOrWhiteSpace(destinationCountry))
                rowErrors.Add("DestinationCountry is required.");

            if (string.IsNullOrWhiteSpace(departureAirport))
                rowErrors.Add("DepartureAirport is required.");

            if (string.IsNullOrWhiteSpace(arrivalAirport))
                rowErrors.Add("ArrivalAirport is required.");

            if (rowErrors.Count > 0)
            {
                foreach (string error in rowErrors)
                {
                    result.Errors.Add($"Row {rowNumber}: {error}");
                }

                continue;
            }

            string dateValue = columns[4].Trim();

            if (string.IsNullOrWhiteSpace(dateValue))
            {
                result.Errors.Add(
                    $"Row {rowNumber}: DepartureDate is required.");
                continue;
            }

            if (!DateTime.TryParseExact(
                    dateValue,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime departureDate))
            {
                result.Errors.Add(
                    $"Row {rowNumber}: Invalid departure date. Expected format: yyyy-MM-dd.");
                continue;
            }

            if (departureDate.Date < DateTime.Today)
            {
                result.Errors.Add(
                    $"Row {rowNumber}: Departure date cannot be in the past.");
                continue;
            }


            if (!decimal.TryParse(
                    columns[5].Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal economyPrice))
            {
                rowErrors.Add(
                    "EconomyPrice must be a valid decimal.");
            }

            if (!int.TryParse(
                    columns[6].Trim(),
                    out int economySeats))
            {
                rowErrors.Add(
                    "EconomySeats must be a valid integer.");
            }

            if (!decimal.TryParse(
                    columns[7].Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal businessPrice))
            {
                rowErrors.Add(
                    "BusinessPrice must be a valid decimal.");
            }

            if (!int.TryParse(
                    columns[8].Trim(),
                    out int businessSeats))
            {
                rowErrors.Add(
                    "BusinessSeats must be a valid integer.");
            }

            if (!decimal.TryParse(
                    columns[9].Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal firstClassPrice))
            {
                rowErrors.Add(
                    "FirstClassPrice must be a valid decimal.");
            }

            if (!int.TryParse(
                    columns[10].Trim(),
                    out int firstClassSeats))
            {
                rowErrors.Add(
                    "FirstClassSeats must be a valid integer.");
            }

            if (rowErrors.Count > 0)
            {
                foreach (string error in rowErrors)
                {
                    result.Errors.Add(
                        $"Row {rowNumber}: {error}");
                }

                continue;
            }

            if (economyPrice < 0)
                rowErrors.Add("EconomyPrice cannot be negative.");

            if (economySeats < 0)
                rowErrors.Add("EconomySeats cannot be negative.");

            if (businessPrice < 0)
                rowErrors.Add("BusinessPrice cannot be negative.");

            if (businessSeats < 0)
                rowErrors.Add("BusinessSeats cannot be negative.");

            if (firstClassPrice < 0)
                rowErrors.Add("FirstClassPrice cannot be negative.");

            if (firstClassSeats < 0)
                rowErrors.Add("FirstClassSeats cannot be negative.");

            if (rowErrors.Count > 0)
            {
                foreach (string error in rowErrors)
                {
                    result.Errors.Add(
                        $"Row {rowNumber}: {error}");
                }

                continue;
            }

            var flight = new Flight(
                departureCountry,
                destinationCountry,
                departureAirport,
                arrivalAirport,
                departureDate,
                new FlightClassInventory(economyPrice, economySeats),
                new FlightClassInventory(businessPrice, businessSeats),
                new FlightClassInventory(firstClassPrice, firstClassSeats));

            result.ImportedFlights.Add(flight);
        }

        return result;
    }
}
