using KomootTourAnalyzer.Data;
using KomootTourAnalyzer.DTOs;

namespace KomootTourAnalyzer;
public class TourAnalyzer : ITourAnalyzer
{
    private readonly Action<string> logger = Console.WriteLine;
    private readonly ITourLoader tourLoader;
    private TourSummary? tourSummary;

    public TourAnalyzer(ITourLoader tourLoader, Action<string>? logger){
        this.tourLoader = tourLoader;
        this.logger = logger ?? this.logger;
    }

    public async Task Analyze()
    {
        var tours = new List<TourDto>();
        var page = 0;
        int nTotalTours;
        do
        {
            var toursChunk = await this.tourLoader.LoadToursPaged(50, page++);
            if (toursChunk is null) return;
            nTotalTours = toursChunk.Page.TotalElements;
            tours.AddRange(toursChunk.Embedded.Tours);
            logger.Invoke($"{TimePrefix()} Got {tours.Count} of {nTotalTours} tours");
        }
        while (tours.Count < nTotalTours);
        tourSummary = tours.Aggregate(new TourSummary() { Date = DateTime.MaxValue, EndDate = DateTime.MinValue}, (s, e) =>
        {
            s.DistanceInMeters += e.DistanceInMeters;
            s.ElevationUpInMeters += e.ElevationUpInMeters;
            s.ElevationDownInMeters += e.ElevationDownInMeters;
            s.SecondsInMotion += e.SecondsInMotion;
            s.SecondsTotal += e.SecondsTotal;
            s.Date = s.Date > e.Date ? e.Date : s.Date;
            s.EndDate = s.EndDate < e.Date ? e.Date : s.EndDate;
            return s;
        });
    }

    public void Print()
    {
        logger(TimePrefix() + "Total distance [km]:         " + (tourSummary?.DistanceInMeters / 1000.0 ?? 0).ToString("0.00"));
        logger(TimePrefix() + "Total elevation up [km]:     " + (tourSummary?.ElevationUpInMeters / 1000.0 ?? 0).ToString("0.00"));
        logger(TimePrefix() + "Total elevation down [km]:   " + (tourSummary?.ElevationDownInMeters / 1000.0 ?? 0).ToString("0.00"));
        logger(TimePrefix() + "Total time in motion:        " + TimeSpan.FromSeconds(tourSummary?.SecondsInMotion ?? 0));
        logger(TimePrefix() + "First Tour [day.month.year]: " + tourSummary?.Date.ToString("dd.MM.yyyy") ?? "error");
        logger(TimePrefix() + "Last Tour [day.month.year]:  " + tourSummary?.EndDate.ToString("dd.MM.yyyy") ?? "error");
    }

    private static string TimePrefix()
    {
        return $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: ";
    }
}