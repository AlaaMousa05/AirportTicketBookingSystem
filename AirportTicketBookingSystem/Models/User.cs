using System.Text.Json.Serialization;

namespace AirportTicketBooking.Models;

public class User
{
    public Guid Id { get; private set; }

    public string Username { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    [JsonConstructor]
    public User(
        Guid id,
        string username,
        string passwordHash,
        UserRole role)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.");

        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role));

        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}
