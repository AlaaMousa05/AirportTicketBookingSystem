using AirportTicketBooking.Models;
using AirportTicketBooking.Storage;

namespace AirportTicketBooking.Services;

public class FlightService
{
    private readonly List<Flight> _flights;
    private readonly JsonFileStorage<Flight> _flightStorage;

    public FlightService(JsonFileStorage<Flight> flightStorage)
    {
        _flightStorage = flightStorage;
        _flights = _flightStorage.Load();
    }

    public List<Flight> GetAllFlights()
    {
        return _flights.ToList();
    }

    public Flight? FindFlight(Guid flightId)
    {
        return _flights.FirstOrDefault(
            f => f.Id == flightId);
    }

    public List<string> AddFlights(IEnumerable<Flight> flights)
    {
        var errors = new List<string>();
        bool hasChanges = false;

        foreach (Flight flight in flights)
        {
            if (FlightAlreadyExists(flight))
            {
                errors.Add(
                    $"Duplicate flight: {flight.DepartureCountry} to " +
                    $"{flight.DestinationCountry} on {flight.DepartureDate:d}.");
                continue;
            }

            _flights.Add(flight);
            hasChanges = true;
        }

        if (hasChanges)
        {
            SaveFlights();
        }

        return errors;
    }

    public void SaveFlights()
    {
        _flightStorage.Save(_flights);
    }

    private bool FlightAlreadyExists(Flight flight)
    {
        return _flights.Any(existingFlight =>
            existingFlight.Id == flight.Id ||

            (existingFlight.DepartureCountry.Equals(
                flight.DepartureCountry,
                StringComparison.OrdinalIgnoreCase) &&

             existingFlight.DestinationCountry.Equals(
                flight.DestinationCountry,
                StringComparison.OrdinalIgnoreCase) &&

             existingFlight.DepartureAirport.Equals(
                flight.DepartureAirport,
                StringComparison.OrdinalIgnoreCase) &&

             existingFlight.ArrivalAirport.Equals(
                flight.ArrivalAirport,
                StringComparison.OrdinalIgnoreCase) &&

             existingFlight.DepartureDate == flight.DepartureDate));
    }
}
