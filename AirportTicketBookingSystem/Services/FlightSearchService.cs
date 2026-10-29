
using AirportTicketBooking.Models;

namespace AirportTicketBooking.Services;

public class FlightSearchService
{
    public List<Flight> Search(
        IEnumerable<Flight> flights,
        FlightSearchCriteria criteria)
    {
        IEnumerable<Flight> results = flights;

        if (!string.IsNullOrWhiteSpace(criteria.DepartureCountry))
        {
            results = results.Where(f =>
                f.DepartureCountry.Equals(
                    criteria.DepartureCountry,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.DestinationCountry))
        {
            results = results.Where(f =>
                f.DestinationCountry.Equals(
                    criteria.DestinationCountry,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.DepartureDate.HasValue)
        {
            results = results.Where(f =>
                f.DepartureDate.Date == criteria.DepartureDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(criteria.DepartureAirport))
        {
            results = results.Where(f =>
                f.DepartureAirport.Equals(
                    criteria.DepartureAirport,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.ArrivalAirport))
        {
            results = results.Where(f =>
                f.ArrivalAirport.Equals(
                    criteria.ArrivalAirport,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.FlightClass.HasValue)
        {
            results = results.Where(f =>
                GetPrice(f, criteria.FlightClass.Value) > 0);
        }

        if (criteria.MaxPrice.HasValue && criteria.FlightClass.HasValue)
        {
            results = results.Where(f =>
                GetPrice(f, criteria.FlightClass.Value)
                <= criteria.MaxPrice.Value);
        }

        return results.ToList();
    }

    private static decimal GetPrice(Flight flight, FlightClass flightClass)
    {
        return flightClass switch
        {
            FlightClass.Economy => flight.Pricing.Economy,
            FlightClass.Business => flight.Pricing.Business,
            FlightClass.FirstClass => flight.Pricing.FirstClass,
            _ => 0
        };
    }
}
