using AirportTicketBooking.Models;
using AirportTicketBooking.Storage;

namespace AirportTicketBooking.Services;

public class AuthenticationService
{
    private readonly List<User> _users;

    public AuthenticationService(JsonFileStorage<User> userStorage)
    {
        _users = userStorage.Load();
    }

    public User? Login(string username, string password)
    {
        User? user = _users.FirstOrDefault(u => u.Username == username);

        if (user is null || !PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}
