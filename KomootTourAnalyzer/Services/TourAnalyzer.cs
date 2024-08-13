using KomootTourAnalyzer.Data;

namespace KomootTourAnalyzer.Services;
public class TourAnalyzer : BaseLoggingService, ITourAnalyzer
{
    private readonly Action<string> logger = Console.WriteLine;
    private readonly ITourLoader tourLoader;
    private TourSummary? tourSummary;
    private Dictionary<int, TourSummary> summariesByYears = [];
    public TourAnalyzer(ITourLoader tourLoader, Action<string>? logger){
        this.tourLoader = tourLoader;
        this.logger = logger ?? this.logger;
    }

    public async Task Analyze()
    {
        var tours = await tourLoader.LoadAll();
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
        foreach (var tour in tours)
        {
            if (!summariesByYears.ContainsKey(tour.Date.Year)){
                summariesByYears[tour.Date.Year] = new()
                {
                    Date = DateTime.MaxValue
                };
            }
            var s = summariesByYears[tour.Date.Year];
            s.DistanceInMeters += tour.DistanceInMeters;
            s.ElevationDownInMeters += tour.ElevationDownInMeters;
            s.ElevationUpInMeters += tour.ElevationUpInMeters;
            s.SecondsInMotion += tour.SecondsInMotion;
            s.SecondsTotal += tour.SecondsTotal;
            s.Date = s.Date > tour.Date ? tour.Date : s.Date;
            s.EndDate = s.EndDate < tour.Date ? tour.Date : s.EndDate;
        }
    }

    public void Print()
    {
        logger("");
        PrintSelection(tourSummary);
        foreach (var k in summariesByYears.Keys.Order())
        {
            var v = summariesByYears[k];
            logger("");
            logger(TimePrefix() + "Year " + k);
            PrintSelection(v);
        }
    }

    private void PrintSelection(TourSummary? tourSummary){
        var distanceKm = (tourSummary?.DistanceInMeters ?? 0) / 1000.0;
        var timeInMotionH = (tourSummary?.SecondsInMotion ?? 1) / 3600.0;
        logger(TimePrefix() + "Total distance [km]:         " + distanceKm.ToString("0.00"));
        logger(TimePrefix() + "Total elevation up [km]:     " + (tourSummary?.ElevationUpInMeters / 1000.0 ?? 0).ToString("0.00"));
        logger(TimePrefix() + "Total elevation down [km]:   " + (tourSummary?.ElevationDownInMeters / 1000.0 ?? 0).ToString("0.00"));
        logger(TimePrefix() + "Total time in motion:        " + TimeSpan.FromSeconds(tourSummary?.SecondsInMotion ?? 0));
        logger(TimePrefix() + "Average velocity [km/h]:     " + (distanceKm / timeInMotionH).ToString("0.00"));
        logger(TimePrefix() + "First Tour [day.month.year]: " + tourSummary?.Date.ToString("dd.MM.yyyy") ?? "error");
        logger(TimePrefix() + "Last Tour [day.month.year]:  " + tourSummary?.EndDate.ToString("dd.MM.yyyy") ?? "error");
    }
}