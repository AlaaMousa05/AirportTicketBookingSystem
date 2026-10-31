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

    public User? FindUserByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }

    public User? FindUserById(Guid userId)
    {
        return _users.FirstOrDefault(u => u.Id == userId);
    }

    public User? Login(string username, string password)
    {
        User? user = FindUserByUsername(username);

        if (user is null || !PasswordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}
