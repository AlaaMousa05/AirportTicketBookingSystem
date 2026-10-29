namespace AirportTicketBooking.Models;

public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Username { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }
}