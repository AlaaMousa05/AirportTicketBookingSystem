using AirportTicketBooking.Models;
using AirportTicketBooking.Services;
using AirportTicketBooking.Storage;

var dataDirectory = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..",
        "Data"));

var bookingStorage = new JsonFileStorage<Booking>(
    Path.Combine(dataDirectory, "bookings.json"));

var flightStorage = new JsonFileStorage<Flight>(
    Path.Combine(dataDirectory, "flights.json"));

var bookingService = new BookingService(
    bookingStorage,
    flightStorage);

// Add sample flights only if no flights exist
if (!bookingService.GetAllFlights().Any())
{
    var flights = new List<Flight>
    {
        new Flight(
            "Palestine",
            "Jordan",
            "Jenin Airport",
            "Queen Alia Airport",
            new DateTime(2026, 12, 15),
            new FlightClassInventory(100, 100),
            new FlightClassInventory(250, 30),
            new FlightClassInventory(500, 10)
        ),

        new Flight(
            "Palestine",
            "Turkey",
            "Ramallah Airport",
            "Istanbul Airport",
            new DateTime(2026, 12, 20),
            new FlightClassInventory(150, 100),
            new FlightClassInventory(300, 30),
            new FlightClassInventory(600, 10)
        ),

        new Flight(
            "Jordan",
            "Turkey",
            "Queen Alia Airport",
            "Istanbul Airport",
            new DateTime(2026, 12, 25),
            new FlightClassInventory(120, 100),
            new FlightClassInventory(280, 30),
            new FlightClassInventory(550, 10)
        )
    };

    foreach (var flight in flights)
    {
        bookingService.AddFlight(flight);
    }
}

Guid bookingId = Guid.Parse(
    "a7ef2331-0dc2-4f31-b5c3-34e5a882f91e");

bookingService.CancelBooking(bookingId);

var firstFlight = bookingService.GetAllFlights().First();

Console.WriteLine($"Available seats: {firstFlight.Economy.AvailableSeats}");
Console.WriteLine($"Booked seats: {firstFlight.Economy.BookedSeats}");

var cancelledBooking = bookingService.GetBookingById(bookingId);

Console.WriteLine($"Booking status: {cancelledBooking?.Status}");




// Display loaded flights
var loadedFlights = bookingService.GetAllFlights();

Console.WriteLine($"Loaded flights: {loadedFlights.Count}");

foreach (var flight in loadedFlights)
{
    Console.WriteLine($"\nFlight ID: {flight.Id}");
    Console.WriteLine(
        $"{flight.DepartureCountry} -> {flight.DestinationCountry}");

    Console.WriteLine($"Date: {flight.DepartureDate:d}");

    Console.WriteLine(
        $"Economy: {flight.Economy.Price} | " +
        $"Available seats: {flight.Economy.AvailableSeats}");

    Console.WriteLine(
        $"Business: {flight.Business.Price} | " +
        $"Available seats: {flight.Business.AvailableSeats}");

    Console.WriteLine(
        $"First Class: {flight.FirstClass.Price} | " +
        $"Available seats: {flight.FirstClass.AvailableSeats}");
}