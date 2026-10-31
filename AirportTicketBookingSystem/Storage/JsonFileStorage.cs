
using System.Text.Json;

namespace AirportTicketBooking.Storage;

public class JsonFileStorage<T> where T : class
{
    private readonly string _filePath;

    public JsonFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void Save(List<T> items)
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(
            items,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_filePath, json);
    }
}
