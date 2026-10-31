using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AirportTicketBooking.Validation;

namespace AirportTicketBooking.Models;

public class Flight
{
    public const string DepartureDateFormat = "yyyy-MM-dd";

    public Guid Id { get; private set; }

    [Required]
    public string DepartureCountry { get; private set; }

    [Required]
    public string DestinationCountry { get; private set; }

    [Required]
    public string DepartureAirport { get; private set; }

    [Required]
    public string ArrivalAirport { get; private set; }

    [Required]
    [FutureDate]
    [DisplayFormat(DataFormatString = DepartureDateFormat)]
    public DateTime DepartureDate { get; private set; }

    public FlightClassInventory Economy { get; private set; }

    public FlightClassInventory Business { get; private set; }

    public FlightClassInventory FirstClass { get; private set; }

    [JsonConstructor]
    public Flight(
        Guid id,
        string departureCountry,
        string destinationCountry,
        string departureAirport,
        string arrivalAirport,
        DateTime departureDate,
        FlightClassInventory economy,
        FlightClassInventory business,
        FlightClassInventory firstClass)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Flight ID is required.");

        if (string.IsNullOrWhiteSpace(departureCountry))
            throw new ArgumentException("Departure country is required.");

        if (string.IsNullOrWhiteSpace(destinationCountry))
            throw new ArgumentException("Destination country is required.");

        if (string.IsNullOrWhiteSpace(departureAirport))
            throw new ArgumentException("Departure airport is required.");

        if (string.IsNullOrWhiteSpace(arrivalAirport))
            throw new ArgumentException("Arrival airport is required.");

        ArgumentNullException.ThrowIfNull(economy);
        ArgumentNullException.ThrowIfNull(business);
        ArgumentNullException.ThrowIfNull(firstClass);

        Id = id;
        DepartureCountry = departureCountry;
        DestinationCountry = destinationCountry;
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
        DepartureDate = departureDate;
        Economy = economy;
        Business = business;
        FirstClass = firstClass;
    }

    public Flight(
        string departureCountry,
        string destinationCountry,
        string departureAirport,
        string arrivalAirport,
        DateTime departureDate,
        FlightClassInventory economy,
        FlightClassInventory business,
        FlightClassInventory firstClass)
        : this(
            Guid.NewGuid(),
            departureCountry,
            destinationCountry,
            departureAirport,
            arrivalAirport,
            departureDate,
            economy,
            business,
            firstClass)
    {
    }

    public FlightClassInventory GetInventory(FlightClass flightClass)
    {
        return flightClass switch
        {
            FlightClass.Economy => Economy,
            FlightClass.Business => Business,
            FlightClass.FirstClass => FirstClass,
            _ => throw new ArgumentOutOfRangeException(nameof(flightClass))
        };
    }
}