
using AirportTicketBooking.Models;
using AirportTicketBooking.Services;
using AirportTicketBooking.Storage;

// 1. Set up the Data directory
var dataDirectory = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "Data"));

Directory.CreateDirectory(dataDirectory);

string csvFilePath = Path.Combine(dataDirectory, "flights-import.csv");
string flightsFilePath = Path.Combine(dataDirectory, "flights.json");
string bookingsFilePath = Path.Combine(dataDirectory, "bookings.json");

Console.WriteLine("=== AIRPORT TICKET BOOKING - FLIGHT MANAGEMENT TEST ===");

// 2. Display validation constraints
Console.WriteLine("\n--- 1. FLIGHT VALIDATION CONSTRAINTS ---");

var constraintsService = new FlightValidationConstraints();
List<ValidationFieldDetail> constraints =
    constraintsService.GetConstraints();

foreach (ValidationFieldDetail detail in constraints)
{
    Console.WriteLine($"\nField: {detail.FieldName}");
    Console.WriteLine($"Type: {detail.Type}");
    Console.WriteLine($"Constraint: {detail.Constraint}");
}

// 3. Initialize storage and services
var flightStorage = new JsonFileStorage<Flight>(flightsFilePath);
var bookingStorage = new JsonFileStorage<Booking>(bookingsFilePath);

var bookingService = new BookingService(
    bookingStorage,
    flightStorage);

// 4. Import CSV
Console.WriteLine("\n--- 2. CSV IMPORT ---");

var csvImportService = new FlightCsvImportService();

try
{
    CsvImportResult importResult =
        csvImportService.Import(csvFilePath);

    Console.WriteLine(
        $"Valid flights found: {importResult.ImportedFlights.Count}");

    Console.WriteLine(
        $"CSV validation errors: {importResult.Errors.Count}");

    // 5. Display imported flights
    Console.WriteLine("\n--- 3. IMPORTED FLIGHTS ---");

    foreach (Flight flight in importResult.ImportedFlights)
    {
        Console.WriteLine(
            $"{flight.DepartureCountry} -> {flight.DestinationCountry} | " +
            $"{flight.DepartureDate:yyyy-MM-dd}");

        Console.WriteLine(
            $"Economy: Price={flight.Economy.Price}, Seats={flight.Economy.TotalSeats}");

        Console.WriteLine(
            $"Business: Price={flight.Business.Price}, Seats={flight.Business.TotalSeats}");

        Console.WriteLine(
            $"First Class: Price={flight.FirstClass.Price}, Seats={flight.FirstClass.TotalSeats}");
    }

    // 6. Display CSV validation errors
    Console.WriteLine("\n--- 4. CSV VALIDATION ERRORS ---");

    if (importResult.Errors.Count == 0)
    {
        Console.WriteLine("No CSV validation errors.");
    }
    else
    {
        foreach (string error in importResult.Errors)
        {
            Console.WriteLine(error);
        }
    }

    // 7. Add valid flights and detect duplicates
    Console.WriteLine("\n--- 5. ADD FLIGHTS TO STORAGE ---");

    List<string> addErrors =
        bookingService.AddFlights(importResult.ImportedFlights);

    Console.WriteLine(
        $"Duplicate flights: {addErrors.Count}");

    if (addErrors.Count == 0)
    {
        Console.WriteLine("No duplicate flights found.");
    }
    else
    {
        foreach (string error in addErrors)
        {
            Console.WriteLine(error);
        }
    }

    // 8. Verify all saved flights
    Console.WriteLine("\n--- 6. SAVED FLIGHTS ---");

    List<Flight> savedFlights = bookingService.GetAllFlights();

    Console.WriteLine($"Total flights in storage: {savedFlights.Count}");

    foreach (Flight flight in savedFlights)
    {
        Console.WriteLine(
            $"{flight.DepartureCountry} -> {flight.DestinationCountry} | " +
            $"{flight.DepartureDate:yyyy-MM-dd}");
    }

    // 9. Verify persistence by loading JSON again
    Console.WriteLine("\n--- 7. PERSISTENCE CHECK ---");

    var verificationStorage =
        new JsonFileStorage<Flight>(flightsFilePath);

    List<Flight> reloadedFlights = verificationStorage.Load();

    Console.WriteLine(
        $"Flights loaded again from JSON: {reloadedFlights.Count}");

    Console.WriteLine(
        reloadedFlights.Count == savedFlights.Count
            ? "Persistence check passed."
            : "Persistence check failed.");

    Console.WriteLine("\n=== TEST COMPLETED ===");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
