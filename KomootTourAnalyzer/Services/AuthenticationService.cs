using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services
{
	public class AuthenticationService
	{
		private readonly HttpClient _client;
		private readonly CookieContainer _cookieContainer = new();
		private readonly HttpClientHandler _handler;
		private readonly IConfiguration _config;

		public AuthenticationService(IConfiguration configuration)
		{
			_handler = new() { CookieContainer = _cookieContainer };
			_config = configuration;
			var komootUrl = configuration["KomootLoginUrl"] ?? throw new KeyNotFoundException("KomootLoginUrl is required");
			_client = new HttpClient(_handler) { BaseAddress = new Uri(komootUrl) };
		}

		public async Task<string> Login()
		{
			_ = await _client.GetAsync("");
			var request = new HttpRequestMessage(HttpMethod.Post, "v1/signin");
			var email = _config["KomootUsername"] ?? throw new KeyNotFoundException("KomootUsername is required");
			var password = _config["KomootPassword"] ?? throw new KeyNotFoundException("KomootPassword is required");
			request.Content = JsonContent.Create(new { email, password, reason = (string?)null });
			var response = await _client.SendAsync(request);
			_ = await _client.GetAsync("actions/transfer?type=signin");
			return await response.Content.ReadAsStringAsync();
		}

		public CookieContainer Cookies => _cookieContainer;
		public string UserId
		{
			get
			{
                var koa_value = (_cookieContainer.GetAllCookies().FirstOrDefault(c => c.Name == "koa_rt")?.Value)
					?? throw new KeyNotFoundException("Cookie koa_rt was not found. Try to login first.");
                return koa_value[0..koa_value.IndexOf("%")];
            }
		}
			
	}
}

