using System.Net;
using System.Text;
using System.Text.Json;
using Dyvenix.Core.ApiClients;
using Dyvenix.Core.DTOs;
using Dyvenix.Core.Exceptions;
using Xunit;

namespace Dyvenix.Core.Tests.ApiClients;

public class ApiClientBaseTests
{
	private sealed record Sample(string Name, int Value);

	/// <summary>
	/// Stub handler that returns a preconfigured response and captures the last request.
	/// </summary>
	private sealed class StubHttpMessageHandler : HttpMessageHandler
	{
		private readonly HttpStatusCode _statusCode;
		private readonly string? _content;

		public HttpRequestMessage? LastRequest { get; private set; }
		public string? LastRequestBody { get; private set; }

		public StubHttpMessageHandler(HttpStatusCode statusCode, string? content)
		{
			_statusCode = statusCode;
			_content = content;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			LastRequest = request;
			if (request.Content is not null)
				LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);

			var response = new HttpResponseMessage(_statusCode);
			if (_content is not null)
				response.Content = new StringContent(_content, Encoding.UTF8, "application/json");

			return response;
		}
	}

	/// <summary>
	/// Test double exposing the protected members of <see cref="ApiClientBase"/>.
	/// </summary>
	private sealed class TestApiClient : ApiClientBase
	{
		public TestApiClient(HttpClient httpClient) : base(httpClient) { }

		public Task<T> GetPublicAsync<T>(string uri) => GetAsync<T>(uri);
		public Task<T> PostPublicAsync<T>(string uri, object payload) => PostAsync<T>(uri, payload);
		public Task PostPublicAsync(string uri, object payload) => PostAsync(uri, payload);
		public Task<string> PostPublicAsyncStr(string uri, object payload) => PostAsyncStr(uri, payload);
		public Task<T> PostWithReturnPublicAsync<T>(string uri, object payload) => PostAsyncWithReturn<T>(uri, payload);
	}

	private static TestApiClient CreateClient(HttpStatusCode statusCode, string? content, out StubHttpMessageHandler handler)
	{
		handler = new StubHttpMessageHandler(statusCode, content);
		var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://test.local/") };
		return new TestApiClient(httpClient);
	}

	[Fact]
	public async Task GetAsync_SuccessfulResponse_ReturnsDeserializedData()
	{
		var payload = Result<Sample>.Ok(new Sample("widget", 7));
		var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		var client = CreateClient(HttpStatusCode.OK, json, out _);

		var result = await client.GetPublicAsync<Sample>("items/1");

		Assert.Equal("widget", result.Name);
		Assert.Equal(7, result.Value);
	}

	[Fact]
	public async Task GetAsync_NoContent_ReturnsDefault()
	{
		var client = CreateClient(HttpStatusCode.NoContent, null, out _);

		var result = await client.GetPublicAsync<Sample>("items/1");

		Assert.Null(result);
	}

	[Fact]
	public async Task GetAsync_NonSuccessStatus_ThrowsHttpException()
	{
		var client = CreateClient(HttpStatusCode.InternalServerError, "error", out _);

		var ex = await Assert.ThrowsAsync<HttpException>(() => client.GetPublicAsync<Sample>("items/1"));
		Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
	}

	[Fact]
	public async Task PostAsync_SerializesPayloadAndReturnsData()
	{
		var responsePayload = Result<Sample>.Ok(new Sample("created", 1));
		var json = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		var client = CreateClient(HttpStatusCode.OK, json, out var handler);

		var result = await client.PostPublicAsync<Sample>("items", new Sample("input", 5));

		Assert.Equal("created", result.Name);
		Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
		Assert.Contains("input", handler.LastRequestBody);
	}

	[Fact]
	public async Task PostAsyncStr_ReturnsRawResponseBody()
	{
		var client = CreateClient(HttpStatusCode.OK, "raw-body", out _);

		var result = await client.PostPublicAsyncStr("items", new Sample("x", 1));

		Assert.Equal("raw-body", result);
	}

	[Fact]
	public async Task PostAsyncWithReturn_DeserializesRawResponse()
	{
		var sample = new Sample("direct", 9);
		var json = JsonSerializer.Serialize(sample, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		var client = CreateClient(HttpStatusCode.OK, json, out _);

		var result = await client.PostWithReturnPublicAsync<Sample>("items", new Sample("in", 1));

		Assert.Equal("direct", result.Name);
		Assert.Equal(9, result.Value);
	}

	[Fact]
	public async Task PostAsync_NonSuccessStatus_ThrowsHttpException()
	{
		var client = CreateClient(HttpStatusCode.BadRequest, "bad", out _);

		await Assert.ThrowsAsync<HttpException>(() => client.PostPublicAsync("items", new Sample("x", 1)));
	}
}
