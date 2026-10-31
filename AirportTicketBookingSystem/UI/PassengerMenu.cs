using AirportTicketBooking.Models;
using AirportTicketBooking.Services;

namespace AirportTicketBooking.UI;

public class PassengerMenu
{
    private readonly User _passenger;
    private readonly BookingService _bookingService;
    private readonly FlightService _flightService;
    private readonly FlightSearchService _flightSearchService;

    public PassengerMenu(
        User passenger,
        BookingService bookingService,
        FlightService flightService,
        FlightSearchService flightSearchService)
    {
        _passenger = passenger;
        _bookingService = bookingService;
        _flightService = flightService;
        _flightSearchService = flightSearchService;
    }

    public void Show()
    {
        while (true)
        {
            Console.WriteLine("\n=== PASSENGER MENU ===");
            Console.WriteLine("1. Search flights");
            Console.WriteLine("2. Book a flight");
            Console.WriteLine("3. View my bookings");
            Console.WriteLine("4. Modify a booking");
            Console.WriteLine("5. Cancel a booking");
            Console.WriteLine("0. Exit");
            Console.Write("Choice: ");

            string choice = Console.ReadLine() ?? "0";

            try
            {
                switch (choice.Trim())
                {
                    case "1":
                        ConsoleHelper.PrintFlights(SearchFlights());
                        break;
                    case "2":
                        BookFlight();
                        break;
                    case "3":
                        ViewBookings();
                        break;
                    case "4":
                        ModifyBooking();
                        break;
                    case "5":
                        CancelBooking();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private List<Flight> SearchFlights()
    {
        var criteria = new FlightSearchCriteria
        {
            MaxPrice = ConsoleHelper.ReadOptionalPrice("Max price"),
            DepartureCountry = ConsoleHelper.ReadOptionalText("Departure country"),
            DestinationCountry = ConsoleHelper.ReadOptionalText("Destination country"),
            DepartureDate = ConsoleHelper.ReadOptionalDate("Departure date"),
            DepartureAirport = ConsoleHelper.ReadOptionalText("Departure airport"),
            ArrivalAirport = ConsoleHelper.ReadOptionalText("Arrival airport"),
            FlightClass = ConsoleHelper.ReadOptionalClass()
        };

        return _flightSearchService.Search(
            _flightService.GetAllFlights(),
            criteria);
    }

    private Flight? SelectFlight()
    {
        List<Flight> flights = SearchFlights();
        ConsoleHelper.PrintFlights(flights);

        if (flights.Count == 0)
            return null;

        int? index = ConsoleHelper.ReadOptionalIndex("Flight number", flights.Count);

        return index.HasValue ? flights[index.Value] : null;
    }

    private void BookFlight()
    {
        Flight? flight = SelectFlight();

        if (flight is null)
            return;

        FlightClass? flightClass = ConsoleHelper.ReadOptionalClass();

        if (!flightClass.HasValue)
            return;

        Booking booking = _bookingService.CreateBooking(
            _passenger.Id,
            flight.Id,
            flightClass.Value);

        Console.WriteLine(
            $"Booked: {ConsoleHelper.DescribeBooking(booking, flight)}");
    }

    private void ViewBookings()
    {
        List<Booking> bookings = _bookingService.GetPassengerBookings(_passenger.Id);

        PrintBookings(bookings);
    }

    private void ModifyBooking()
    {
        Booking? booking = SelectConfirmedBooking();

        if (booking is null)
            return;

        Console.WriteLine("Choose the new flight:");
        Flight? flight = SelectFlight();

        if (flight is null)
            return;

        FlightClass? flightClass = ConsoleHelper.ReadOptionalClass();

        if (!flightClass.HasValue)
            return;

        _bookingService.ModifyBooking(
            booking.Id,
            _passenger.Id,
            flight.Id,
            flightClass.Value);

        Console.WriteLine(
            $"Modified: {ConsoleHelper.DescribeBooking(booking, flight)}");
    }

    private void CancelBooking()
    {
        Booking? booking = SelectConfirmedBooking();

        if (booking is null)
            return;

        _bookingService.CancelBooking(booking.Id, _passenger.Id);

        Console.WriteLine("Booking cancelled.");
    }

    private Booking? SelectConfirmedBooking()
    {
        List<Booking> bookings = _bookingService
            .GetPassengerBookings(_passenger.Id)
            .Where(b => b.Status == BookingStatus.Confirmed)
            .ToList();

        PrintBookings(bookings);

        if (bookings.Count == 0)
            return null;

        int? index = ConsoleHelper.ReadOptionalIndex("Booking number", bookings.Count);

        return index.HasValue ? bookings[index.Value] : null;
    }

    private void PrintBookings(List<Booking> bookings)
    {
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        for (int i = 0; i < bookings.Count; i++)
        {
            Flight? flight = _flightService.FindFlight(bookings[i].FlightId);

            Console.WriteLine(
                $"{i + 1}. {ConsoleHelper.DescribeBooking(bookings[i], flight)}");
        }
    }
}
