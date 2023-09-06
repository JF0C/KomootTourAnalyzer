using System;
using System.Net;
using System.Text.Json;
using KomootTourAnalyzer.Data;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services
{
	public class TourLoaderService
	{
		private readonly AuthenticationService _authService;
		private readonly IConfiguration _config;
		public TourLoaderService(AuthenticationService authService, IConfiguration config)
		{
			_authService = authService;
			_config = config;
		}

		public async Task<TourResponseDto?> GetToursPaged(int size, int page)
		{
            using var handler = new HttpClientHandler() { CookieContainer = _authService.Cookies };
            var url = _config["KomootApiUrl"] ?? throw new KeyNotFoundException("KomootApiUrl is required");
            using var client = new HttpClient(handler) { BaseAddress = new Uri(url) };
            var query = "users/" + _authService.UserId + "/tours/?sort_field=date&type=tour_recorded&sort_direction=desc&page=" + page + "&limit=" + size;
            var message = new HttpRequestMessage(HttpMethod.Get, query);
            message.Headers.Add("onlyprops", "true");
            var response = await client.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<TourResponseDto>(content);
		}

		public async Task<TourSummary?> GetSummarizedData(Action<string> statusUpdate)
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
            var result = tours.Aggregate(new TourSummary() { Date = DateTime.MaxValue, EndDate = DateTime.MinValue}, (s, e) =>
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
            return result;
		}
	}
}

