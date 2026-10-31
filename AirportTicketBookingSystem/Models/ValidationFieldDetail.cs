namespace AirportTicketBooking.Models;

public class ValidationFieldDetail
{
    public string FieldName { get; }
    public string Type { get; }
    public string Constraint { get; }

    public ValidationFieldDetail(
        string fieldName,
        string type,
        string constraint)
    {
        FieldName = fieldName;
        Type = type;
        Constraint = constraint;
    }
}
