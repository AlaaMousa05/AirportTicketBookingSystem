using AirportTicketBooking.Models;
using AirportTicketBooking.Storage;

namespace AirportTicketBooking.Services;

public class BookingService
{
    private readonly List<Booking> _bookings;
    private readonly JsonFileStorage<Booking> _bookingStorage;
    private readonly FlightService _flightService;

    public BookingService(
        JsonFileStorage<Booking> bookingStorage,
        FlightService flightService)
    {
        _bookingStorage = bookingStorage;
        _flightService = flightService;

        _bookings = _bookingStorage.Load();
    }

    public Booking CreateBooking(
        Guid passengerId,
        Guid flightId,
        FlightClass flightClass)
    {
        Flight flight = GetFlight(flightId);

        FlightClassInventory inventory =
            flight.GetInventory(flightClass);

        var booking = new Booking(
            passengerId,
            flight.Id,
            flightClass,
            inventory.Price);

        inventory.BookSeat();

        _bookings.Add(booking);

        SaveChanges();

        return booking;
    }

    public void CancelBooking(Guid bookingId, Guid passengerId)
    {
        Booking booking = GetConfirmedBooking(bookingId, passengerId, "cancelled");

        Flight flight = GetFlight(booking.FlightId);

        flight.GetInventory(booking.FlightClass).CancelSeat();

        booking.Cancel();

        SaveChanges();
    }

    public void ModifyBooking(
        Guid bookingId,
        Guid passengerId,
        Guid newFlightId,
        FlightClass newFlightClass)
    {
        Booking booking = GetConfirmedBooking(bookingId, passengerId, "modified");

        if (booking.FlightId == newFlightId &&
            booking.FlightClass == newFlightClass)
        {
            throw new InvalidOperationException(
                "The booking already has this flight and class.");
        }

        Flight oldFlight = GetFlight(booking.FlightId);
        Flight newFlight = GetFlight(newFlightId);

        FlightClassInventory newInventory =
            newFlight.GetInventory(newFlightClass);

        // Book first: if the new class is full, nothing has changed yet.
        newInventory.BookSeat();

        oldFlight.GetInventory(booking.FlightClass).CancelSeat();

        booking.Modify(newFlightId, newFlightClass, newInventory.Price);

        SaveChanges();
    }

    public List<Booking> GetPassengerBookings(Guid passengerId)
    {
        return _bookings
            .Where(b => b.PassengerId == passengerId)
            .ToList();
    }

    public List<Booking> FilterBookings(BookingFilterCriteria criteria)
    {
        return _bookings
            .Where(b => MatchesFilter(b, criteria))
            .ToList();
    }

    private Booking GetConfirmedBooking(
        Guid bookingId,
        Guid passengerId,
        string action)
    {
        Booking? booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking is null || booking.PassengerId != passengerId)
        {
            throw new InvalidOperationException(
                "Booking was not found.");
        }

        if (booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException(
                $"Only confirmed bookings can be {action}.");
        }

        return booking;
    }

    private Flight GetFlight(Guid flightId)
    {
        Flight? flight = _flightService.FindFlight(flightId);

        if (flight is null)
        {
            throw new InvalidOperationException(
                "Flight was not found.");
        }

        return flight;
    }

    private void SaveChanges()
    {
        _flightService.SaveFlights();
        _bookingStorage.Save(_bookings);
    }

    private bool MatchesFilter(Booking booking, BookingFilterCriteria criteria)
    {
        Flight? flight = _flightService.FindFlight(booking.FlightId);

        if (flight is null)
            return false;

        return MatchesBooking(booking, criteria) && MatchesFlight(flight, criteria);
    }

    private static bool MatchesBooking(Booking booking, BookingFilterCriteria criteria)
    {
        return (!criteria.FlightId.HasValue || booking.FlightId == criteria.FlightId) &&
            (!criteria.PassengerId.HasValue || booking.PassengerId == criteria.PassengerId) &&
            (!criteria.FlightClass.HasValue || booking.FlightClass == criteria.FlightClass) &&
            (!criteria.MaxPrice.HasValue || booking.Price <= criteria.MaxPrice);
    }

    private static bool MatchesFlight(Flight flight, BookingFilterCriteria criteria)
    {
        return MatchesText(flight.DepartureCountry, criteria.DepartureCountry) &&
            MatchesText(flight.DestinationCountry, criteria.DestinationCountry) &&
            MatchesText(flight.DepartureAirport, criteria.DepartureAirport) &&
            MatchesText(flight.ArrivalAirport, criteria.ArrivalAirport) &&
            (!criteria.DepartureDate.HasValue ||
                flight.DepartureDate.Date == criteria.DepartureDate.Value.Date);
    }

    private static bool MatchesText(string value, string? expected)
    {
        return string.IsNullOrWhiteSpace(expected) ||
            value.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}
