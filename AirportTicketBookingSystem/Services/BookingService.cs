using AirportTicketBooking.Models;
using AirportTicketBooking.Storage;

namespace AirportTicketBooking.Services;

public class BookingService
{
    private readonly List<Booking> _bookings;
    private readonly JsonFileStorage<Booking> _storage;

    public BookingService(JsonFileStorage<Booking> storage)
    {
        _storage = storage;
        _bookings = _storage.Load();
    }

    public Booking CreateBooking(
        Guid passengerId,
        Flight flight,
        FlightClass flightClass)
    {
        ArgumentNullException.ThrowIfNull(flight);

        if (passengerId == Guid.Empty)
            throw new ArgumentException("Passenger ID is required.");

        FlightClassInventory inventory = flight.GetInventory(flightClass);

        inventory.BookSeat();

        var booking = new Booking(
            passengerId,
            flight.Id,
            flightClass,
            inventory.Price);

        _bookings.Add(booking);

        _storage.Save(_bookings);



        return booking;
    }

    public void CancelBooking(Guid bookingId, Flight flight)
    {
        ArgumentNullException.ThrowIfNull(flight);

        Booking? booking = GetBookingById(bookingId);

        if (booking is null)
            throw new InvalidOperationException("Booking was not found.");

        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");

        if (booking.FlightId != flight.Id)
            throw new ArgumentException(
                "The provided flight does not match the booking.");

        FlightClassInventory inventory =
            flight.GetInventory(booking.FlightClass);

        inventory.CancelSeat();

        booking.Cancel();

        _storage.Save(_bookings);
    }

    public List<Booking> GetPassengerBookings(Guid passengerId)
    {
        return _bookings
            .Where(b => b.PassengerId == passengerId)
            .ToList();
    }

    public List<Booking> GetAllBookings()
    {
        return _bookings.ToList();
    }

    public Booking? GetBookingById(Guid bookingId)
    {
        return _bookings.FirstOrDefault(b => b.Id == bookingId);
    }
}