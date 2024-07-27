using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services;

public class TourLoader : BaseLoggingService, ITourLoader
{
    private readonly IConfiguration configuration;
    private readonly IAuthenticationService authenticationService;
    private readonly Action<string> logger = Console.WriteLine;
    private readonly List<TourDto> tours = [];

    public TourLoader(IConfiguration configuration, IAuthenticationService authenticationService, Action<string>? logger)
    {
        this.authenticationService = authenticationService;
        this.configuration = configuration;
        this.logger = logger ?? this.logger;
    }

    public async Task<TourResponseDto?> LoadToursPaged(int size, int page)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(configuration["KomootApiUrl"] ?? throw new KeyNotFoundException("KomootApiUrl must be set"))
        };
        var uri = "users/" + await authenticationService.GetUserId() + "/tours/?sort_types=&type=tour_recorded&sort_field=date&sort_direction=desc&name=&status=private&hl=de&page=" + page + "&limit=" + size;
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        await authenticationService.AddAuthHeader(request);
        var response = await client.SendAsync(request);
        var data = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TourResponseDto>(data);
    }

    public async Task<IEnumerable<TourDto>> LoadAll(bool force = false)
    {
        if (tours.Count == 0 || force)
        {
            tours.Clear();
            var page = 0;
            int nTotalTours = 0;
            do
            {
                var toursChunk = await LoadToursPaged(50, page++);
                if (toursChunk is null) continue;
                nTotalTours = toursChunk.Page.TotalElements;
                tours.AddRange(toursChunk.Embedded.Tours);
                logger.Invoke($"{TimePrefix()} Got {tours.Count} of {nTotalTours} tours");
            }
            while (tours.Count < nTotalTours);
        }
        return tours.OrderBy(t => t.Date);
    }
}