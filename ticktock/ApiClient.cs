using System;
using System.Net.Http;
using System.Threading.Tasks;
using TimeGetterAPI;
using System.Text.Json;
using System.Windows.Navigation;
using ticktock;

public class ApiClient : IApiClient
{
	private readonly HttpClient _client;
	private readonly string _baseUrl;

	public ApiClient(string baseUrl)
	{
		_baseUrl = baseUrl;
		_client = new HttpClient();
		_client.BaseAddress = new Uri(baseUrl);
	}


	public async Task<string> GetTimeFromIp()
	{
		try
		{
			var response = await _client.GetAsync("api/ip");

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error: {response.StatusCode}");
			}

			var content = await response.Content.ReadAsStringAsync();
			WorldTime worldTime = JsonSerializer.Deserialize<WorldTime>(content);
			DateTime dateTime = new DateTime();
			bool parseResult = DateTime.TryParse(worldTime?.datetime, out dateTime);
			return dateTime.ToString("T") ?? "000000";
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"Request exception: {e.Message}");
			Console.WriteLine($"Inner exception: {e.InnerException}");
			return "000000";
		}
	}

	public async Task<string> GetCurrentTimeAsync()
	{
		try
		{
			var response = await _client.GetAsync("WorldTime/GetCurrentTime");

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error: {response.StatusCode}");
			}

			return await response.Content.ReadAsStringAsync();
		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"Request exception: {e.Message}");
			Console.WriteLine($"Inner exception: {e.InnerException}");
			throw;
		}
	}
}
