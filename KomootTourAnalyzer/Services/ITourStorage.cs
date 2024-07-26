namespace KomootTourAnalyzer.Services;
public interface ITourStorage
{
    Task SaveTours(string filePath);
    Task SaveToursAsCsv(string filePath, string separator = ";");
}