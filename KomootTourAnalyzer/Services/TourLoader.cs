
using System.Text;
using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services;

public class TourLoader : ITourLoader
{
    private readonly IConfiguration configuration;
    private readonly IAuthenticationService authenticationService;

    public TourLoader(IConfiguration configuration, IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
        this.configuration = configuration;
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
}