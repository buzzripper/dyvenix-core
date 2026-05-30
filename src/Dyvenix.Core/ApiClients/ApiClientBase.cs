using Dyvenix.Core.DTOs;
using Dyvenix.Core.Exceptions;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dyvenix.Core.ApiClients;

public interface IApiClientBase
{
}

public abstract class ApiClientBase : IApiClientBase
{
	#region Fields

	private readonly HttpClient _httpClient;
	private readonly JsonSerializerOptions _jsonSerializerOptionsGet = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true
	};
	private readonly JsonSerializerOptions _jsonSerializerOptionsPost = new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	#endregion

	#region Ctors / Init

	public ApiClientBase(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_jsonSerializerOptionsGet.Converters.Add(new JsonStringEnumConverter());
	}

	#endregion

	#region Methods

	protected async Task<TResult> GetAsync<TResult>(string uri)
	{
		var httpResponse = await _httpClient.GetAsync(uri);

		if (!httpResponse.IsSuccessStatusCode)
			throw new HttpException(httpResponse.StatusCode, $"{httpResponse.Content.ReadAsStringAsync()}");

		if (httpResponse.StatusCode == HttpStatusCode.NoContent)
			return default!;

		var responseString = await httpResponse.Content.ReadAsStringAsync();
		var response = JsonSerializer.Deserialize<Result<TResult>>(responseString, _jsonSerializerOptionsGet);
		if (response is null)
			throw new HttpDeserializationException("Failed to deserialize the response.");

		return response.Data!;
	}

	#endregion

	#region Post

	protected async Task PostAsync(string uri, object payload)
	{
		await CallAsync(MethodType.Post, uri, payload);
	}

	protected async Task<TResult> PostAsync<TResult>(string uri, object payload)
	{
		return await CallAsync<TResult>(MethodType.Post, uri, payload);
	}

	protected async Task<string> PostAsyncStr(string uri, object payload)
	{
		return await CallAsyncStr(MethodType.Post, uri, payload);
	}

	protected async Task<TResult> PostAsyncWithReturn<TResult>(string uri, object payload)
	{
		var resultJson = await CallAsyncStr(MethodType.Post, uri, payload);

		var result = JsonSerializer.Deserialize<TResult>(resultJson, _jsonSerializerOptionsGet);
		if (result is null)
			throw new HttpDeserializationException("Failed to deserialize the response.");

		return result;
	}

	#endregion

	#region Delete

	protected async Task DeleteAsync(string uri, object payload)
	{
		await CallAsync(MethodType.Delete, uri, payload);
	}

	protected async Task<TResult> DeleteAsync<TResult>(string uri, object payload)
	{
		return await CallAsync<TResult>(MethodType.Delete, uri, payload);
	}

	protected async Task<string> DeleteAsyncStr(string uri, object payload)
	{
		return await CallAsyncStr(MethodType.Delete, uri, payload);
	}

	#endregion

	#region Put

	protected async Task PutAsync(string uri, object payload)
	{
		await CallAsync(MethodType.Put, uri, payload);
	}

	protected async Task<TResult> PutAsync<TResult>(string uri, object payload)
	{
		return await CallAsync<TResult>(MethodType.Put, uri, payload);
	}

	protected async Task<string> PutAsyncStr(string uri, object payload)
	{
		return await CallAsyncStr(MethodType.Put, uri, payload);
	}

	#endregion

	#region Patch

	protected async Task PatchAsync(string uri, object payload)
	{
		await CallAsync(MethodType.Patch, uri, payload);
	}

	protected async Task<TResult> PatchAsync<TResult>(string uri, object payload)
	{
		return await CallAsync<TResult>(MethodType.Patch, uri, payload);
	}

	protected async Task<string> PatchAsyncStr(string uri, object payload)
	{
		return await CallAsyncStr(MethodType.Patch, uri, payload);
	}

	#endregion

	#region Call

	protected async Task CallAsync(MethodType methodType, string uri, object payload)
	{
		var responseString = await CallAsyncStr(methodType, uri, payload);

		var response = JsonSerializer.Deserialize<Result>(responseString, _jsonSerializerOptionsGet);
		if (response is null)
			throw new HttpDeserializationException("Failed to deserialize the response.");
	}

	protected async Task<T> CallAsync<T>(MethodType methodType, string uri, object payload)
	{
		var responseString = await CallAsyncStr(methodType, uri, payload);

		var response = JsonSerializer.Deserialize<Result<T>>(responseString, _jsonSerializerOptionsGet);
		if (response is null)
			throw new HttpDeserializationException("Failed to deserialize the response.");

		return response.Data!;
	}

	protected async Task<string> CallAsyncStr(MethodType methodType, string uri, object payload)
	{
		var json = JsonSerializer.Serialize(payload, _jsonSerializerOptionsPost);
		using var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

		HttpResponseMessage httpResponse;

		switch (methodType)
		{
			case MethodType.Post:
				httpResponse = await _httpClient.PostAsync(uri, stringContent);
				break;
			case MethodType.Put:
				httpResponse = await _httpClient.PutAsync(uri, stringContent);
				break;
			default:
				httpResponse = await _httpClient.PatchAsync(uri, stringContent);
				break;
		}

		if (!httpResponse.IsSuccessStatusCode)
			throw new HttpException(httpResponse.StatusCode, $"{httpResponse.Content.ReadAsStringAsync()}");

		return await httpResponse.Content.ReadAsStringAsync();
	}

	#endregion
}
