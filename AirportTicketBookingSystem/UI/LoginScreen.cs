using AirportTicketBooking.Models;
using AirportTicketBooking.Services;

namespace AirportTicketBooking.UI;

public static class LoginScreen
{
    public static User Show(AuthenticationService authenticationService)
    {
        Console.WriteLine("=== LOGIN ===");

        while (true)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? string.Empty;

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            User? user = authenticationService.Login(username, password);

            if (user is not null)
            {
                Console.WriteLine($"Welcome, {user.Username}! Role: {user.Role}");
                return user;
            }

            Console.WriteLine("Invalid username or password.");
        }
    }
}
