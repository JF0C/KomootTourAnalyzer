
using KomootTourAnalyzer.DTOs;

public interface ITourLoader
{
    Task<TourResponseDto?> LoadToursPaged(int size, int page);
}