using System.ComponentModel.DataAnnotations;

namespace AirportTicketBooking.Validation;

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is DateTime date && date.Date >= DateTime.Today;
    }
}
