using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services
{
	public class AuthenticationService
	{
		private readonly HttpClient _client;
		private readonly CookieContainer _cookieContainer = new();
		private readonly HttpClientHandler _handler;

		public AuthenticationService(IConfiguration configuration)
		{
			_handler = new() { CookieContainer = _cookieContainer };
			_client = new HttpClient(_handler) { BaseAddress = new Uri(configuration["KomootLoginUrl"]) };
		}

		public async Task<string> Login(string email, string password)
		{
			_ = await _client.GetAsync("");
			var request = new HttpRequestMessage(HttpMethod.Post, "v1/signin");
			request.Content = JsonContent.Create(new { email, password, reason = (string?)null });
			var response = await _client.SendAsync(request);
			_ = await _client.GetAsync("actions/transfer?type=signin");
			return await response.Content.ReadAsStringAsync();
		}

		public CookieContainer Cookies => _cookieContainer;
	}
}

