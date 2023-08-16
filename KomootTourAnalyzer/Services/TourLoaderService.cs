using System;
using System.Net;
using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services
{
	public class TourLoaderService
	{
		private readonly CookieContainer _cookies;
		private readonly IConfiguration _config;
		public TourLoaderService(CookieContainer cookies, IConfiguration config)
		{
			_cookies = cookies;
			_config = config;
		}

		public async Task<TourResponseDto?> GetToursPaged(int size, int page)
		{
            using var handler = new HttpClientHandler() { CookieContainer = _cookies };
            using var client = new HttpClient(handler) { BaseAddress = new Uri(_config["KomootApiUrl"]) };
            var userid = _config["KomootUserId"] ?? throw new ArgumentNullException("appsettings:KomootUserId");
            var query = "users/" + userid + "/tours/?sort_field=date&type=tour_recorded&sort_direction=desc&page=" + page + "&limit=" + size;
            var message = new HttpRequestMessage(HttpMethod.Get, query);
            message.Headers.Add("onlyprops", "true");
            var response = await client.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<TourResponseDto>(content);
		}

		public async Task<TourDto?> GetSummarizedData(Action<string> statusUpdate)
		{
			var tours = new List<TourDto>();
            var page = 0;
            int nTotalTours;
            do
            {
                var toursChunk = await GetToursPaged(50, page++);
                if (toursChunk is null) return null;
                nTotalTours = toursChunk.Page.TotalElements;
                tours.AddRange(toursChunk.Embedded.Tours);
                statusUpdate.Invoke($"Got {tours.Count} of {nTotalTours} tours");
            }
            while (tours.Count < nTotalTours);
            var result = tours.Aggregate(new TourDto(), (s, e) =>
            {
                s.DistanceInMeters += e.DistanceInMeters;
                s.ElevationUpInMeters += e.ElevationUpInMeters;
                s.ElevationDownInMeters += e.ElevationDownInMeters;
                s.SecondsInMotion += e.SecondsInMotion;
                s.SecondsTotal += e.SecondsTotal;
                return s;
            });
            return result;
		}
	}
}

