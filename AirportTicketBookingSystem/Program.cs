using AirportTicketBooking.Models;
using AirportTicketBooking.Services;

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

var searchService = new FlightSearchService();

var criteria = new FlightSearchCriteria
{
    DepartureDate = new DateTime(2026, 12, 20),
    DepartureAirport = "Ramallah Airport",
    ArrivalAirport = "Istanbul Airport"
};

var results = searchService.Search(flights, criteria);

Console.WriteLine($"Found {results.Count} flights:");

foreach (var flight in results)
{
    Console.WriteLine(
        $"{flight.DepartureCountry} -> {flight.DestinationCountry} | " +
        $"Price: {flight.Economy.Price} | " +
        $"Date: {flight.DepartureDate:d}");
}