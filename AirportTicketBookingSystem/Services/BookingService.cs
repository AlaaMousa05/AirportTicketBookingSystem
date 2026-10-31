using AirportTicketBooking.Models;
using AirportTicketBooking.Storage;

namespace AirportTicketBooking.Services;

public class BookingService
{
    private readonly List<Booking> _bookings;
    private readonly List<Flight> _flights;

    private readonly JsonFileStorage<Booking> _bookingStorage;
    private readonly JsonFileStorage<Flight> _flightStorage;

    public BookingService(
        JsonFileStorage<Booking> bookingStorage,
        JsonFileStorage<Flight> flightStorage)
    {
        _bookingStorage = bookingStorage;
        _flightStorage = flightStorage;

        _bookings = _bookingStorage.Load();
        _flights = _flightStorage.Load();
    }

    // Add a flight and save it
    public void AddFlight(Flight flight)
    {
        ArgumentNullException.ThrowIfNull(flight);

        if (_flights.Any(f => f.Id == flight.Id))
        {
            throw new InvalidOperationException(
                "This flight already exists.");
        }

        _flights.Add(flight);
        _flightStorage.Save(_flights);
    }

    // Get all flights
    public List<Flight> GetAllFlights()
    {
        return _flights.ToList();
    }

    // Create a booking
    public Booking CreateBooking(
        Guid passengerId,
        Guid flightId,
        FlightClass flightClass)
    {
        if (passengerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Passenger ID is required.",
                nameof(passengerId));
        }

        Flight? flight = _flights.FirstOrDefault(
            f => f.Id == flightId);

        if (flight is null)
        {
            throw new InvalidOperationException(
                "Flight was not found.");
        }

        FlightClassInventory inventory =
            flight.GetInventory(flightClass);

        var booking = new Booking(
            passengerId,
            flight.Id,
            flightClass,
            inventory.Price);

        inventory.BookSeat();

        _bookings.Add(booking);

        _flightStorage.Save(_flights);
        _bookingStorage.Save(_bookings);

        return booking;
    }

    // Cancel a booking
    public void CancelBooking(Guid bookingId)
    {
        Booking? booking = GetBookingById(bookingId);

        if (booking is null)
        {
            throw new InvalidOperationException(
                "Booking was not found.");
        }

        if (booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");
        }

        Flight? flight = _flights.FirstOrDefault(
            f => f.Id == booking.FlightId);

        if (flight is null)
        {
            throw new InvalidOperationException(
                "The flight associated with this booking was not found.");
        }

        FlightClassInventory inventory =
            flight.GetInventory(booking.FlightClass);

        inventory.CancelSeat();

        booking.Cancel();

        _flightStorage.Save(_flights);
        _bookingStorage.Save(_bookings);
    }

    // Get passenger bookings
    public List<Booking> GetPassengerBookings(Guid passengerId)
    {
        return _bookings
            .Where(b => b.PassengerId == passengerId)
            .ToList();
    }

    // Get all bookings
    public List<Booking> GetAllBookings()
    {
        return _bookings.ToList();
    }

    // Find a booking by ID
    public Booking? GetBookingById(Guid bookingId)
    {
        return _bookings.FirstOrDefault(
            b => b.Id == bookingId);
    }
}