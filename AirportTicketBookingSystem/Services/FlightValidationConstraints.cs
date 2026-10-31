

using AirportTicketBooking.Models;

namespace AirportTicketBooking.Services;

public class FlightValidationConstraints
{
    public List<ValidationFieldDetail> GetConstraints()
    {
        return new List<ValidationFieldDetail>
        {
            new(
                "DepartureCountry",
                "Free Text",
                "Required"),

            new(
                "DestinationCountry",
                "Free Text",
                "Required"),

            new(
                "DepartureAirport",
                "Free Text",
                "Required"),

            new(
                "ArrivalAirport",
                "Free Text",
                "Required"),

            new(
                "DepartureDate",
                "Date Time",
                "Required, Allowed Range (today → future), Format: yyyy-MM-dd"),

            new(
                "EconomyPrice",
                "Decimal",
                "Required, Minimum: 0"),

            new(
                "EconomySeats",
                "Integer",
                "Required, Minimum: 0"),

            new(
                "BusinessPrice",
                "Decimal",
                "Required, Minimum: 0"),

            new(
                "BusinessSeats",
                "Integer",
                "Required, Minimum: 0"),

            new(
                "FirstClassPrice",
                "Decimal",
                "Required, Minimum: 0"),

            new(
                "FirstClassSeats",
                "Integer",
                "Required, Minimum: 0")
        };
    }
}
