using PSM.Barcode.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PSM.Barcode.Services;

public class RestResult<T>
{
	public string? Error { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public T? Value { get; set; }
}

public class RestService
{
	private readonly OptionsService _options;
	private readonly HttpClient _httpClient;
	private readonly JsonSerializerOptions _serializerOptions;

	public RestService(OptionsService options)
    {
		_options = options;
		_options.ServerChanged += (s,e) =>
		{
			if (_httpClient == null) return;
			if (!string.IsNullOrEmpty(_options.BaseUri))
				_httpClient.BaseAddress = new Uri(_options.BaseUri);
		};

		_httpClient = new HttpClient ();
		if (!string.IsNullOrEmpty(_options.BaseUri))
			_httpClient.BaseAddress = new Uri(_options.BaseUri);

		_serializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		};
	}

	public async Task<RestResult<bool>> Test()
	{
		var result = new RestResult<bool>();
		try
		{
			var responce = await _httpClient.GetAsync("test");
			result.StatusCode = responce.StatusCode;
			if (responce.IsSuccessStatusCode)
			{
				var content = await responce.Content.ReadAsStringAsync();
				result.Value = content == "test";
				if (!result.Value)
					result.Error = content;
			}
			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error += $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}

	public async Task<RestResult<List<BarcodePair>>> GetPairs(string last, int size)
	{
		var result = new RestResult<List<BarcodePair>>();
		try
		{
			var responce = await _httpClient.GetAsync($"api/Barcodes/{last}/{size}");
			result.StatusCode = responce.StatusCode;
			if (responce.IsSuccessStatusCode)
			{
				var content = await responce.Content.ReadAsStringAsync();
				result.Value = JsonSerializer.Deserialize<List<Pair>>(content, _serializerOptions)
					?.Select(p => new BarcodePair { Barcode = p.BarCode, Outcode = p.OutCode })
					.ToList();
			}
			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error += $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}

	public async Task<RestResult<List<User>>> GetUsers()
	{
		var result = new RestResult<List<User>>();
		try
		{
			var responce = await _httpClient.GetAsync("api/Users");

			result.StatusCode = responce.StatusCode;
			if (responce.IsSuccessStatusCode)
			{
				var content = await responce.Content.ReadAsStringAsync();
				result.Value = JsonSerializer.Deserialize<List<User>>(content, _serializerOptions);
			}
			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error += $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}

	public async Task<RestResult<int>> LogIn(AuthDto user)
	{
		RestResult<int> result = new();
		try
		{
			string json = JsonSerializer.Serialize(user, _serializerOptions);
			StringContent data = new(json, Encoding.UTF8, "application/json");

			HttpResponseMessage responce = await _httpClient.PostAsync("api/login", data);
			result.StatusCode = responce.StatusCode;
			var content = await responce.Content.ReadAsStringAsync();
			if (!responce.IsSuccessStatusCode)
				result.Error = content;
			else
				result.Value = int.Parse(content);
			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error += $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}

	public async Task<RestResult<int>> DeleteBarcodes(int userId)
	{
		RestResult<int> result = new();
		try
		{
			HttpResponseMessage responce = await _httpClient.DeleteAsync($"api/Basket/{userId}");
			result.StatusCode = responce.StatusCode;

			var content = await responce.Content.ReadAsStringAsync();
			if (!responce.IsSuccessStatusCode)
				result.Error = content;
			else
				result.Value = int.Parse(content);

			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error += $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}

	public async Task<RestResult<int>> SendBarcodes(int userId, IEnumerable<string> items)
	{
		RestResult<int> result = new();
		try
		{
			string json = JsonSerializer.Serialize(items, _serializerOptions);
			StringContent data = new(json, Encoding.UTF8, "application/json");

			HttpResponseMessage responce = await _httpClient.PostAsync($"api/Basket/{userId}/list", data);
			result.StatusCode = responce.StatusCode;

			var content = await responce.Content.ReadAsStringAsync();
			if (!responce.IsSuccessStatusCode)
				result.Error = content;
			else
				result.Value = int.Parse(content);

			return result;
		}
		catch (Exception ex)
		{
			result.Error = ex.Message;
			var i = 0;
			var exi = ex.InnerException;
			while (exi != null)
			{
				result.Error = result.Error + $"\n\n{exi.Message}";
				exi = exi.InnerException;
				i++;
				if (i > 5) break;
			}
			return result;
		}
	}
}