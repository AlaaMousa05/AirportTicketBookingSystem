# Airport Ticket Booking System

A console application for booking flight tickets, built with C# and .NET 10. Data is stored in JSON files.

## How to run

```
cd AirportTicketBookingSystem
dotnet run
```

## Login accounts

| Username | Password | Role |
|---|---|---|
| manager | Manager@123 | Manager |
| passenger1 | Passenger@123 | Passenger |
| passenger2 | Passenger@123 | Passenger |

## Features

**Passenger**
- Search for available flights
- Book a flight
- View, modify, and cancel bookings

**Manager**
- Filter bookings
- Import flights from a CSV file, for example `Data\flights-import.csv`
- View flight validation details
