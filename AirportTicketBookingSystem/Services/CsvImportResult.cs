using AirportTicketBooking.Models;

namespace AirportTicketBooking.Services;

public class CsvImportResult
{
    public List<Flight> ImportedFlights { get; } = new();

    public List<string> Errors { get; } = new();
}