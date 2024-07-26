
using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using System.Linq;

namespace KomootTourAnalyzer.Services;

public class TourStorage : ITourStorage
{
    private readonly ITourLoader tourLoader;
    public TourStorage(ITourLoader tourLoader)
    {
        this.tourLoader = tourLoader;
    }
    public async Task SaveTours(string filePath)
    {
        var textData = JsonSerializer.Serialize(await tourLoader.LoadAll());
        await File.WriteAllTextAsync(filePath, textData);
    }

    public async Task SaveToursAsCsv(string filePath, string separator = ";")
    {
        var tours = await tourLoader.LoadAll();
        var lines = new List<string>();
        var propertyNames = typeof(TourDto).GetProperties().Select(property => property.Name);
        lines.Add(string.Join(separator, propertyNames));
        foreach (var tour in tours)
        {
            lines.Add(string.Join(separator, propertyNames.Select(p => typeof(TourDto).GetProperty(p)?.GetValue(tour)?.ToString() ?? "")));
        }
        File.WriteAllLines(filePath, lines);
    }
}