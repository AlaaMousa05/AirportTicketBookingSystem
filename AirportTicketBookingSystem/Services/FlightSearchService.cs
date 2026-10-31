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

        results = results.Where(f => f.DepartureDate.Date >= DateTime.Today);

        results = results.Where(f => criteria.FlightClass.HasValue
            ? MatchesClass(f.GetInventory(criteria.FlightClass.Value), criteria.MaxPrice)
            : MatchesClass(f.Economy, criteria.MaxPrice) ||
              MatchesClass(f.Business, criteria.MaxPrice) ||
              MatchesClass(f.FirstClass, criteria.MaxPrice));

        return results.ToList();
    }

    private static bool MatchesClass(FlightClassInventory inventory, decimal? maxPrice)
    {
        return inventory.AvailableSeats > 0 &&
            (!maxPrice.HasValue || inventory.Price <= maxPrice.Value);
    }
}
