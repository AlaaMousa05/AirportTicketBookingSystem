using AirportTicketBooking.Models;
using AirportTicketBooking.Services;
using AirportTicketBooking.Validation;

namespace AirportTicketBooking.UI;

public class ManagerMenu
{
    private readonly BookingService _bookingService;
    private readonly FlightService _flightService;
    private readonly AuthenticationService _authenticationService;
    private readonly FlightCsvImportService _flightCsvImportService;
    private readonly FlightValidationConstraints _flightValidationConstraints;

    public ManagerMenu(
        BookingService bookingService,
        FlightService flightService,
        AuthenticationService authenticationService,
        FlightCsvImportService flightCsvImportService,
        FlightValidationConstraints flightValidationConstraints)
    {
        _bookingService = bookingService;
        _flightService = flightService;
        _authenticationService = authenticationService;
        _flightCsvImportService = flightCsvImportService;
        _flightValidationConstraints = flightValidationConstraints;
    }

    public void Show()
    {
        while (true)
        {
            Console.WriteLine("\n=== MANAGER MENU ===");
            Console.WriteLine("1. Filter bookings");
            Console.WriteLine("2. Import flights from CSV");
            Console.WriteLine("3. View flight validation details");
            Console.WriteLine("0. Exit");
            Console.Write("Choice: ");

            string choice = Console.ReadLine() ?? "0";

            switch (choice.Trim())
            {
                case "1":
                    FilterBookings();
                    break;
                case "2":
                    ImportFlights();
                    break;
                case "3":
                    ShowValidationDetails();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private void FilterBookings()
    {
        var criteria = new BookingFilterCriteria
        {
            FlightId = SelectFlightId(),
            MaxPrice = ConsoleHelper.ReadOptionalPrice("Max price"),
            DepartureCountry = ConsoleHelper.ReadOptionalText("Departure country"),
            DestinationCountry = ConsoleHelper.ReadOptionalText("Destination country"),
            DepartureDate = ConsoleHelper.ReadOptionalDate("Departure date"),
            DepartureAirport = ConsoleHelper.ReadOptionalText("Departure airport"),
            ArrivalAirport = ConsoleHelper.ReadOptionalText("Arrival airport")
        };

        string? username = ConsoleHelper.ReadOptionalText("Passenger username");

        if (username is not null)
        {
            User? passenger = _authenticationService.FindUserByUsername(username);

            if (passenger is null)
            {
                Console.WriteLine("Passenger was not found.");
                return;
            }

            criteria.PassengerId = passenger.Id;
        }

        criteria.FlightClass = ConsoleHelper.ReadOptionalClass();

        List<Booking> bookings = _bookingService.FilterBookings(criteria);

        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        foreach (Booking booking in bookings)
        {
            Flight? flight = _flightService.FindFlight(booking.FlightId);
            User? passenger = _authenticationService.FindUserById(booking.PassengerId);

            Console.WriteLine(
                $"{passenger?.Username ?? "Unknown passenger"} | " +
                ConsoleHelper.DescribeBooking(booking, flight));
        }
    }

    private Guid? SelectFlightId()
    {
        List<Flight> flights = _flightService.GetAllFlights();

        ConsoleHelper.PrintFlights(flights);

        int? index = ConsoleHelper.ReadOptionalIndex("Flight number", flights.Count);

        return index.HasValue ? flights[index.Value].Id : null;
    }

    private void ImportFlights()
    {
        Console.Write("CSV file path: ");
        string filePath = (Console.ReadLine() ?? string.Empty).Trim();

        try
        {
            CsvImportResult result = _flightCsvImportService.Import(filePath);

            foreach (string error in result.Errors)
            {
                Console.WriteLine(error);
            }

            List<string> duplicateErrors = _flightService.AddFlights(result.ImportedFlights);

            foreach (string error in duplicateErrors)
            {
                Console.WriteLine(error);
            }

            Console.WriteLine(
                $"Flights added: {result.ImportedFlights.Count - duplicateErrors.Count}");
            Console.WriteLine(
                $"Validation errors: {result.Errors.Count}, duplicates: {duplicateErrors.Count}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ShowValidationDetails()
    {
        foreach (ValidationFieldDetail detail in _flightValidationConstraints.GetConstraints())
        {
            Console.WriteLine($"\n{detail.FieldName}:");
            Console.WriteLine($"  Type: {detail.Type}");
            Console.WriteLine($"  Constraint: {detail.Constraint}");
        }
    }
}
