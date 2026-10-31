using AirportTicketBooking.Models;
using AirportTicketBooking.Services;
using AirportTicketBooking.Storage;
using AirportTicketBooking.UI;
using AirportTicketBooking.Validation;

var dataDirectory = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "Data"));

Directory.CreateDirectory(dataDirectory);

var flightStorage = new JsonFileStorage<Flight>(Path.Combine(dataDirectory, "flights.json"));
var bookingStorage = new JsonFileStorage<Booking>(Path.Combine(dataDirectory, "bookings.json"));
var userStorage = new JsonFileStorage<User>(Path.Combine(dataDirectory, "users.json"));

var flightService = new FlightService(flightStorage);
var bookingService = new BookingService(bookingStorage, flightService);
var authenticationService = new AuthenticationService(userStorage);

User currentUser = LoginScreen.Show(authenticationService);

if (currentUser.Role == UserRole.Manager)
{
    new ManagerMenu(
        bookingService,
        flightService,
        authenticationService,
        new FlightCsvImportService(),
        new FlightValidationConstraints()).Show();
}
else
{
    new PassengerMenu(
        currentUser,
        bookingService,
        flightService,
        new FlightSearchService()).Show();
}
