using KomootTourAnalyzer.DTOs;

namespace KomootTourAnalyzer.Services;
public interface ITourLoader
{
    Task<TourResponseDto?> LoadToursPaged(int size, int page);

    Task<IEnumerable<TourDto>> LoadAll(bool force = false);
}