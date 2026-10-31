using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AirportTicketBooking.Models;

namespace AirportTicketBooking.Validation;

public class FlightValidationConstraints
{
    public List<ValidationFieldDetail> GetConstraints()
    {
        var details = new List<ValidationFieldDetail>();

        foreach (PropertyInfo property in typeof(Flight).GetProperties())
        {
            if (property.PropertyType == typeof(FlightClassInventory))
            {
                foreach (PropertyInfo inventoryProperty in typeof(FlightClassInventory).GetProperties())
                {
                    AddField(details, inventoryProperty, property.Name);
                }
            }
            else
            {
                AddField(details, property, string.Empty);
            }
        }

        return details;
    }

    private static void AddField(
        List<ValidationFieldDetail> details,
        PropertyInfo property,
        string namePrefix)
    {
        List<string> constraints = GetFieldConstraints(property);

        // Properties without validation rules, like Id or BookedSeats, are not input fields.
        if (constraints.Count == 0)
            return;

        string name = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? property.Name;

        details.Add(new ValidationFieldDetail(
            namePrefix + name,
            GetTypeName(property.PropertyType),
            string.Join(", ", constraints)));
    }

    private static List<string> GetFieldConstraints(PropertyInfo property)
    {
        var constraints = new List<string>();

        if (property.GetCustomAttribute<RequiredAttribute>() is not null)
            constraints.Add("Required");

        if (property.GetCustomAttribute<FutureDateAttribute>() is not null)
            constraints.Add("Allowed Range (today → future)");

        if (property.GetCustomAttribute<DisplayFormatAttribute>() is { } format)
            constraints.Add($"Format: {format.DataFormatString}");

        if (property.GetCustomAttribute<RangeAttribute>() is { } range)
            constraints.Add($"Minimum: {range.Minimum}");

        return constraints;
    }

    private static string GetTypeName(Type type)
    {
        if (type == typeof(string))
            return "Free Text";

        if (type == typeof(DateTime))
            return "Date Time";

        if (type == typeof(decimal))
            return "Decimal";

        if (type == typeof(int))
            return "Integer";

        return type.Name;
    }
}
